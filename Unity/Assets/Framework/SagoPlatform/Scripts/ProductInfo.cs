namespace SagoPlatform {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	using Version = System.Version;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	public class ProductInfoVersionAttribute : PropertyAttribute {
		
	}
	
	public class ProductInfo : MonoBehaviour {
		
		
		#region Static Methods
		
		public static int CheckBuild(int input) {
			return Mathf.Max(input, 0);
		}
		
		public static string CheckVersion(string input) {
			Version version = null;
			try {
				version = new Version(input);
				version = new Version(version.Major, version.Minor);
			} catch {
				version = new Version(1,0);
			}
			return version.ToString();
		}
		
		#endregion


		#region Properties

		public string DisplayName {
			get {
				return m_DisplayName;
			}

			set {
				m_DisplayName = value;
			}
		}

		public string AnalyticsName {
			get {
				return m_AnalyticsName;
			}

			set {
				m_AnalyticsName = value;
			}
		}

		public string AppCode {
			get {
				return m_AppCode;
			}

			set {
				m_AppCode = value;
			}
		}

		public string Identifier {
			get {
				return m_Identifier;
			}

			set {
				m_Identifier = value;
			}
		}

		public string Version {
			get {
				return m_Version;
			}

			set {
				m_Version = value;
			}
		}

		public int Build {
			get {
				return m_Build;
			}

			set {
				m_Build = value;
			}
		}

		#endregion
		
		
		#region Serialized Fields

		[FormerlySerializedAs("Name")]
		[FormerlySerializedAs("DisplayName")]
		[SerializeField]
		private string m_DisplayName;

		[FormerlySerializedAs("CodeName")]
		[FormerlySerializedAs("AnalyticsName")]
		[SerializeField]
		private string m_AnalyticsName;

		[Tooltip("The app code is the short version name of app name which is the same as our source control code. This is used for analytics purposes. Ex. smfr, smbabies etc")]
		[FormerlySerializedAs("AppCode")]
		[SerializeField]
		private string m_AppCode;

		[FormerlySerializedAs("Identifier")]
		[SerializeField]
		private string m_Identifier;
		
		[ProductInfoVersion]
		[FormerlySerializedAs("Version")]
		[SerializeField]
		private string m_Version;

		[FormerlySerializedAs("Build")]
		[SerializeField]
		private int m_Build;
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		#if UNITY_EDITOR
		virtual public void Reset() {
			this.m_DisplayName = PlayerSettings.productName;
			this.m_AnalyticsName = this.DisplayName.Replace(" ","");
			this.m_Identifier = PlayerSettings.applicationIdentifier;
			this.m_Version = CheckVersion(PlayerSettings.bundleVersion);
			this.m_Build = CheckBuild(0);
		}
		#endif
		
		#endregion
		
		
	}
	
}