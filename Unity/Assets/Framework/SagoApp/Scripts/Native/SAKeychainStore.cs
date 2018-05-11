namespace SagoApp {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using UnityEngine;

	/// <summary>
	/// Class where all generic native methods are exposed.
	/// </summary>
	public class SAKeychainStore : MonoBehaviour {


		#region SAKeychainStore binding

		#if SAGO_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		private static extern bool _SetStringInKeychain(string key, string value);

		[DllImport ("__Internal")]
		private static extern bool _SetSynchronizedStringInKeychain(string key, string value);

		[DllImport ("__Internal")]
		private static extern string _GetStringFromKeychain(string key);

		[DllImport ("__Internal")]
		private static extern string _GetSynchronizedStringFromKeychain(string key);

		[DllImport ("__Internal")]
		private static extern void _DeleteStringFromKeychain(string key);

		[DllImport ("__Internal")]
		private static extern void _DeleteSynchronizedStringFromKeychain(string key);

		[DllImport ("__Internal")]
		private static extern bool _KeychainContainsKey(string key, bool synchronized);

		#endif

		#endregion


		#region Methods

		public static bool SetStringInKeychain(string key, string value, bool synchronized = false) {
			#if SAGO_IOS && !UNITY_EDITOR
			if (synchronized) {
				return _SetSynchronizedStringInKeychain(key, value);
			} else {
				return _SetStringInKeychain(key, value);
			}
			#else
			return false;
			#endif
		}

		public static string GetStringFromKeychain(string key, bool synchronized = false) {
			#if SAGO_IOS && !UNITY_EDITOR
			if (synchronized) {
				return _GetSynchronizedStringFromKeychain(key);
			} else {
				return _GetStringFromKeychain(key);
			}
			#else
			return null;
			#endif
		}

		public static void DeleteStringFromKeychain(string key, bool synchronized = false) {
			#if SAGO_IOS && !UNITY_EDITOR
			if (synchronized) {
				_DeleteSynchronizedStringFromKeychain(key);
			} else {
				_DeleteStringFromKeychain(key);
			}
			#endif
		}

		public static bool KeychainContainsKey(string key, bool synchronized = false) {
			#if SAGO_IOS && !UNITY_EDITOR
			return _KeychainContainsKey(key, synchronized);
			#else
			return false;
			#endif
		}

		#endregion
		
	}
	
}