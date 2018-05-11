namespace SagoApp {
	
	using System.Runtime.InteropServices;
	using UnityEngine;
	
	public class AudioSessionUtil : MonoBehaviour {
		
		
		#region External Methods

		#if UNITY_IOS && !UNITY_EDITOR
			[DllImport("__Internal")]
			private static extern void _AudioSessionUtil_ConfigureAudioSession();
		#endif

		#endregion
		
		
		#region Static Methods
		
		public static void ConfigureAudioSession() {
			#if UNITY_IOS && !UNITY_EDITOR
				_AudioSessionUtil_ConfigureAudioSession();
			#endif
		}
		
		#endregion
		
		
	}
	
}