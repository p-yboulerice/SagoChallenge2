namespace SagoPlatform {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	public class AndroidProductInfoSdkAttribute : PropertyAttribute {
		
	}
	
	public class AndroidProductInfo : ProductInfo {

		#region Properties

		public int SdkVersion {
			get {
				return m_SdkVersion;
			}
			
			set {
				m_SdkVersion = value;
			}
		}

		public string UrlScheme {
			get {
				return m_UrlScheme;
			}

			set {
				m_UrlScheme = value;
			}
		}

		#endregion
		
		#region Serialized Fields
		
		[AndroidProductInfoSdkAttribute]
		[SerializeField]
		[FormerlySerializedAs("SdkVersion")]
		private int m_SdkVersion;
		
		[SerializeField]
		[FormerlySerializedAs("URLScheme")]
		private string m_UrlScheme;
		
		#endregion
		
		#region MonoBehaviour Methods
		
		#if UNITY_EDITOR
		override public void Reset() {
			base.Reset();
			this.Build = PlayerSettings.Android.bundleVersionCode;
			this.m_SdkVersion = (int)AndroidSdkVersions.AndroidApiLevel16;
			this.m_UrlScheme = string.Empty;
		}
		#endif
		
		#endregion
	}
	
}