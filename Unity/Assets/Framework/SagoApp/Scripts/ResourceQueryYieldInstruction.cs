namespace SagoApp.Project {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using AOT;
	using UnityEngine;
	using System.Linq;
	using SagoPlatform;
	
	/// <summary>
	/// Custom yield instruction that queries for a OnDemandResouce if it is already downloaded and available
	/// and makes internal coroutine to wait until an asynchronous callback occurs.
	/// </summary>
	public class ResourceQueryYieldInstruction : CustomYieldInstruction, System.IDisposable {

		#if SAGO_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		private static extern void _CheckResourceAvailability(int queryId, string[] resourceTag, int tagCount, SAResourceQueryCallback callback);

		#endif


		#region Static fields

		private static Dictionary<int,ResourceQueryYieldInstruction> m_ResourceQueryContainer =
			new Dictionary<int,ResourceQueryYieldInstruction>();

		#endregion


		#region Resource query callback

		public delegate void SAResourceQueryCallback(int queryId, bool available);

		[MonoPInvokeCallback(typeof(SAResourceQueryCallback))]
		public static void ResourceQueryCallback(int queryId, bool available) {
			// What's the probability of query object's hashcode collision?
			ResourceQueryYieldInstruction query = m_ResourceQueryContainer[queryId];
			if (query != null) {
				query.OnResourceQueryComplete(available);
			}
			m_ResourceQueryContainer.Remove(queryId);
		}

		#endregion


		#region Fields

		private bool m_IsDone;

		private bool m_IsResourceAvailable;

		private MonoBehaviour m_MonoBehaviour;

		#endregion


		#region Properties

		public override bool keepWaiting
		{
			get {
				return !m_IsDone;
			}
		}

		public bool IsDone {
			get { return m_IsDone; }
		}

		public bool IsResourceAvailable {
			get { return m_IsResourceAvailable; }
		}

		#endregion


		#region Method

		public void OnResourceQueryComplete(bool available) {
			m_IsResourceAvailable = available;
			Complete();
		}

		#endregion


		#region Constructor

		public ResourceQueryYieldInstruction(MonoBehaviour monoBehaviour, params string[] resourceTags) {
			m_MonoBehaviour = null;
			m_IsDone = false;
			m_IsResourceAvailable = false;
			if (monoBehaviour != null) {
				m_MonoBehaviour = monoBehaviour;

				if (!m_ResourceQueryContainer.ContainsValue(this)) {
					m_ResourceQueryContainer.Add(this.GetHashCode(), this);
					#if SAGO_IOS && !UNITY_EDITOR
						string[] versionedResourceTags = resourceTags.Select(tag => PlatformUtil.AppendVersion(tag)).ToArray();
						_CheckResourceAvailability(this.GetHashCode(), versionedResourceTags, versionedResourceTags.Length, ResourceQueryCallback);
					#endif
				}
			} else {
				m_IsDone = true;
				Debug.LogError("ResourcesQueryYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion


		#region Public Methods

		public void Dispose() {
			Debug.Log("ResourceQueryYieldInstruction-> Dispose", DebugContext.SagoApp);
			if (m_MonoBehaviour != null) {
				Complete();
			}
		}

		#endregion


		#region Private Methods

		private void Complete() {
			this.m_IsDone = true;
			this.m_MonoBehaviour = null;
		}

		#endregion
		
	}
	
}