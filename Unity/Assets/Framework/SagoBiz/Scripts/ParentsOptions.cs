namespace SagoBiz {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	using SagoPlatform;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// The ParentsOptions class provides options for the <see cref="Parents" /> component.
	/// </summary>
	public class ParentsOptions : MonoBehaviour {


		private static readonly string StagingUrlTemaplte = "https://for-parents.sagosago.com/view?language={0}&platform={1}&app={2}&sagobiz_sdk_version={3}";
		private static readonly string ProductionUrlTemaplte = "https://for-parents.sagosago.com/view?language={0}&platform={1}&app={2}&sagobiz_sdk_version={3}";

		public enum ParentsMode { Disabled, Staging, Production };

		private string BaseUrl {
			get {
				switch (Mode) {
					case ParentsMode.Production:
						return ProductionUrlTemaplte;
					case ParentsMode.Staging:
						return StagingUrlTemaplte;
					default: 
					case ParentsMode.Disabled:
						return null;
				}
			}
		}

		public string AppUrl {
			get {
				string url = BaseUrl;

				if (string.IsNullOrEmpty(url)){
					return null;
				} else {
					string appCode = PlatformUtil.GetSettings<ProductInfo>(PlatformUtil.ActivePlatform).AppCode;
					if (string.IsNullOrEmpty(appCode)) {
						return null;
					}
					Native native;
					native = Controller.Instance.Native;
					url = string.Format(url, native.Language, native.Platform.ToString(), appCode, Version.Current);
					Debug.Log(url);
					return url;
				}
			}
		}

		[FormerlySerializedAs("Mode")]
		[SerializeField]
		private ParentsMode m_Mode = ParentsMode.Production;

		public ParentsMode Mode {
			get {
				// Adding these define symbols overrides m_Mode serialized field on target platform prefab.
				#if SAGO_PARENTS_DISABLED
					return ParentsMode.Disabled;
				#elif SAGO_PARENTS_STAGING
					return ParentsMode.Staging;
				#elif SAGO_PARENTS_PRODUCTION
					return ParentsMode.Production;
				#endif

				return m_Mode;
			}
		}

		/// <summary>
		/// Gets whether for parents is enabled or not.
		/// </summary>
		public bool IsEnabled {
			get {
				return Mode != ParentsMode.Disabled;
			}
		}

		/// <see cref="MonoBehaviour.Reset" />
		void Reset() {
			#if UNITY_EDITOR
				
			m_Mode = ParentsMode.Production;
				
			#endif
		}
		
	}
	
}
