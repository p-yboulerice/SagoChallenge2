namespace SagoEngine {
	
	using SagoCore.Resources;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	
	public enum MeshLoadOperationSortOrder {
		Descending,
		Ascending
	}
	
	public enum MeshLoadOperationState {
		Unknown,
		Running,
		Cancelled,
		Done
	}
	
	public class MeshLoadOperation {
		
		
		#region Static Properties
		
		public static MeshLoadOperationSortOrder PrioritySortOrder {
			get;
			set;
		}
		
		public static MeshLoadOperationSortOrder TimestampSortOrder {
			get;
			set;
		}
		
		#endregion 
		
		
		#region Properties
		
		public MeshAnimationAsset Asset {
			get; internal set;
		}
		
		public string AssetGuid {
			get; internal set;
		}
		
		internal MonoBehaviour Behaviour {
			get; set;
		}
		
		public int Priority {
			get; set;
		}
		
		internal MeshLoadOperationState State {
			get; set;
		}
		
		internal float Timestamp {
			get; set;
		}
		
		#endregion
		
		
		#region Constructors
		
		public MeshLoadOperation() {
			
		}
		
		~MeshLoadOperation() {
			MeshLoader.UnloadUnusedAssets();
		}
		
		#endregion
		
		
		#region Methods
		
		public void Cancel() {
			if (this.State != MeshLoadOperationState.Done) {
				this.State = MeshLoadOperationState.Cancelled;
			}
		}
		
		public int CompareTo(MeshLoadOperation other) {
			if (this.Priority != other.Priority) {
				int order = PrioritySortOrder == MeshLoadOperationSortOrder.Ascending ? -1 : 1;
				int priority = this.Priority > other.Priority ? -1 : 1;
				return order * priority;
			}
			if (this.Timestamp != other.Timestamp) {
				int order = TimestampSortOrder == MeshLoadOperationSortOrder.Ascending ? -1 : 1;
				int priority = this.Timestamp > other.Timestamp ? -1 : 1;
				return order * priority;
			}
			return 0;
		}
		
		public Coroutine Run() {
			if (this.Asset != null) {
				this.State = MeshLoadOperationState.Done;
			}
			if (this.State == MeshLoadOperationState.Unknown) {
				this.State = MeshLoadOperationState.Running;
				this.Behaviour.StartCoroutine(this.RunAsync());
				return this.Wait();
			}
			return null;
		}
		
		public Coroutine Wait() {
			return this.Behaviour.StartCoroutine(this.WaitAsync());
		}
		
		#endregion
		
		
		#region Internal Methods
		
		IEnumerator RunAsync() {
			
			if (this.Asset != null) {
				this.State = MeshLoadOperationState.Done;
				yield break;
			}
			
			// set state
			this.State = MeshLoadOperationState.Running;
			
			ResourceReference reference;
			reference = ScriptableObject.CreateInstance<ResourceReference>();
			reference.Guid = this.AssetGuid;
			
			ResourceReferenceLoaderRequest request;
			request = ResourceReferenceLoader.LoadAsync(reference, typeof(MeshAnimationAsset));
			yield return request;
			
			// check state
			if (this.State == MeshLoadOperationState.Cancelled) {
				request = null;
				yield break;
			}
			
			// set asset
			if (request != null) {
				this.Asset = request.asset as MeshAnimationAsset;
			}
			
			// set state
			this.State = MeshLoadOperationState.Done;
			
			// null references
			request = null;
			
		}
		
		IEnumerator WaitAsync() {
			while (this.State == MeshLoadOperationState.Unknown || this.State == MeshLoadOperationState.Running) {
				yield return null;
			}
		}
		
		#endregion
		
		
	}
	
}
