namespace SagoBiz {
	
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// The iOSOptions class provides custom options for iOS apps.
	/// </summary>
	public class iOSOptions : MonoBehaviour {
		
		/// <summary>
		/// The app store id from iTunes Connect (i.e. 920007273) 
		/// </summary>
		[SerializeField]
		public string AppStoreId;
		
		/// <see cref="MonoBehaviour.Reset" />
		void Reset() {
			this.AppStoreId = null;
		}
		
	}
	
}