namespace SagoEngine {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	
	public class MeshLoader : MonoBehaviour {
		
		
		#region Singleton
		
		private static MeshLoader _Instance;
		
		public static MeshLoader Instance {
			get {
				if (_Instance == null && _OnApplicationQuit == false) {
					_Instance = new GameObject().AddComponent<MeshLoader>();
					_Instance.name = "MeshLoader";
					Object.DontDestroyOnLoad(_Instance);
				}
				return _Instance;
			}
		}
		
		private static bool _OnApplicationQuit;
		
		private void OnApplicationQuit() {
			_OnApplicationQuit = true;
		}
		
		#endregion
		
		
		#region Public Methods
		
		public void CancelAllLoadOperations() {
			foreach (MeshLoadOperation operation in ActiveOperations) {
				operation.Cancel();
			}
		}
		
		public MeshLoadOperation Find(string assetGuid) {
			if (this.CachedOperations.ContainsKey(assetGuid)) {
				if (this.CachedOperations[assetGuid].IsAlive) {
					return this.CachedOperations[assetGuid].Target as MeshLoadOperation;
				}
				this.CachedOperations.Remove(assetGuid);
			}
			return null;
		}
		
		public MeshLoadOperation Load(string assetGuid) {
			return this.Load(assetGuid, 0);
		}
		
		public MeshLoadOperation Load(string assetGuid, int priority) {
			
			MeshLoadOperation operation;
			operation = this.Find(assetGuid);
			
			if (operation != null) {
				operation.Priority = System.Math.Max(operation.Priority, priority);
				operation.Timestamp = Time.realtimeSinceStartup;
			} else {
				operation = new MeshLoadOperation();
				operation.AssetGuid = assetGuid;
				operation.Behaviour = this;
				operation.Priority = priority;
				operation.Timestamp = Time.realtimeSinceStartup;
				this.CachedOperations.Add(assetGuid, new System.WeakReference(operation));
			}
			
			if (operation.Asset != null || operation.State == MeshLoadOperationState.Done) {
				operation.State = MeshLoadOperationState.Done;
				return operation;
			}
			
			if (operation.State == MeshLoadOperationState.Cancelled) {
				operation.State = MeshLoadOperationState.Unknown;
			}
			
			if (!this.ActiveOperations.Contains(operation)) {
				this.ActiveOperations.Add(operation);
			}
			
			return operation;
			
		}
		
		public static void UnloadUnusedAssets() {
			UnloadUnusedAssetsFlag = true;
		}
		
		#endregion
		
		
		#region Internal Properties
		
		List<MeshLoadOperation> ActiveOperations {
			get; set;
		}
		
		Dictionary<string, System.WeakReference> CachedOperations {
			get; set;
		}
		
		public System.Func<YieldInstruction> OnUnloadUnusedAssets {
			get; set;
		}
		
		static bool UnloadUnusedAssetsFlag {
			get; set;
		}
		
		#endregion
		
		
		#region Internal Methods
		
		void Awake() {
			this.CachedOperations = new Dictionary<string,System.WeakReference>();
			this.ActiveOperations = new List<MeshLoadOperation>();
		}
		
		void CleanupActiveOperations() {
			
			int index;
			index = this.ActiveOperations.Count;
			
			while (index-- > 0) {
				
				MeshLoadOperation operation;
				operation = this.ActiveOperations[index];
				
				if (operation.Asset != null) {
					operation.State = MeshLoadOperationState.Done;
				}
				
				if (operation.State == MeshLoadOperationState.Done) {
					this.ActiveOperations.RemoveAt(index);
				}
				
				if (operation.State == MeshLoadOperationState.Cancelled) {
					this.ActiveOperations.RemoveAt(index);
				}
				
			}
			
		}
		
		void SortActiveOperations() {
			this.ActiveOperations.Sort((a,b) => a.CompareTo(b));
		}
		
		IEnumerator Start() {
			while (true) {
				
				// load
				if (this.ActiveOperations.Count == 0) {
					yield return null;
				} else {
					
					// make sure cancelled and done operations are removed 
					// BEFORE running the next operation, otherwise an 
					// operation can keep adding itself to the queue every 
					// frame and get sorted to the top.
					this.CleanupActiveOperations();
					this.SortActiveOperations();
					
					// Calling CleanupActiveOperations() may remove operations 
					// from the list, so we need to check the count again to 
					// avoid throwing an ArgumentOutOfRangeException.
					// See: https://sagosago.atlassian.net/browse/SW-91
					if (this.ActiveOperations.Count != 0) {
						MeshLoadOperation operation = this.ActiveOperations[0];
						yield return operation.Run();
						operation = null;
					} else {
						yield return null;
					}
					
				}
				
				// cleanup
				this.CleanupActiveOperations();
				
				// unload
				if (UnloadUnusedAssetsFlag) {
					List<string>operationsToRemove = new List<string>();
					foreach (KeyValuePair<string,System.WeakReference> pair in CachedOperations) {
						MeshLoadOperation operation = (
							pair.Value.IsAlive ? 
							pair.Value.Target as MeshLoadOperation : 
							null
						);
						if (operation == null) {
							operationsToRemove.Add(pair.Key);
						}
					}
					foreach (string key in operationsToRemove) {
						this.CachedOperations.Remove(key);
					}
					if (OnUnloadUnusedAssets != null) {
						yield return OnUnloadUnusedAssets();
					} else {
						yield return Resources.UnloadUnusedAssets();
					}
					UnloadUnusedAssetsFlag = false;
					
				}
				
			}
		}
		
		#endregion
		
		
	}
	
}