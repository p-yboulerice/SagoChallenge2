namespace SagoBiz {

	using System.Collections.Generic;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	[System.Serializable]
	public struct AdjustEventDescription {
		public string eventName;
		public string eventToken;
	}

	/// <summary>
	/// AdjustOptions defines configurable options for the Adjust SDK.
	/// </summary>
	public class AdjustOptions : MonoBehaviour, ISerializationCallbackReceiver {


		#region Properties

		/// <summary>
		/// Gets and sets the app's token specified by Adjust.
		/// </summary>
		[SerializeField]
		public string AppToken;
		
		/// <summary>
		/// Gets and sets the associated domains used for implementing universal links on iOS.
		/// </summary>
		[SerializeField]
		public string[] AssociatedDomains;
		
		/// <summary>
		/// Should the app be in debug mode?
		/// If debug is true then Adjust SDK's environment variable will be set to ADJEnvironmentSandbox
		/// otherwise, set to ADJEnvironmentProduction.
		/// </summary>
		[SerializeField]
		public bool IsDebug;

		[Range(1, 7)]
		[SerializeField]
		public int LogLevel;

		#endregion


		#region Fields

		private static AdjustOptions _Instance;

		[SerializeField]
		protected List<AdjustEventDescription> AdjustEventDescriptions;

		protected Dictionary<string, string> AdjustEventDescriptionsDictionary = new Dictionary<string, string>();

		#endregion


		#region Methods

		public void OnBeforeSerialize() {
		}

		public void OnAfterDeserialize() {
			AdjustEventDescriptionsDictionary.Clear();
			foreach (AdjustEventDescription adjEventDesc in AdjustEventDescriptions) {
				if (!AdjustEventDescriptionsDictionary.ContainsKey(adjEventDesc.eventName)) {
					AdjustEventDescriptionsDictionary.Add(adjEventDesc.eventName, adjEventDesc.eventToken);
				}
			}
		}

		/// <summary>
		/// Resets the AdjustOptions component to the default values.
		/// </summary>
		void Reset() {
			this.AppToken = "";
			this.IsDebug = false;
			this.LogLevel = 3;
		}

		public string GetAdjustEventToken(string eventName) {
			if (AdjustEventDescriptionsDictionary.ContainsKey(eventName)) {
				return AdjustEventDescriptionsDictionary[eventName];
			}
			Debug.LogError(string.Format("AdjustOptions-> Cannot find Adjust event name: {0}", eventName));
			return null;
		}

		#endregion

	}

}