namespace SagoApp {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoPlatform;
	using SagoUtils;

	public class VersionDisplayUtil : MonoBehaviour {
		
		
		#region Constants
		
		/// <summary>
		/// The number of fingers that needs to be touching the screen to activate the version display.
		/// </summary>
		[System.NonSerialized]
		private const int MinTouchCount = 3;
		
		/// <summary>
		/// The duration of the touch.
		/// </summary>
		[System.NonSerialized]
		private const int TouchDuration = 3;
		
		/// <summary>
		/// The duration that the version info is displayed.
		/// </summary>
		[System.NonSerialized]
		private const int DisplayDuration = 10;
		
		#endregion
		
		
		#region Private Fields
		
		[System.NonSerialized]
		private float TouchTimer;
		
		[System.NonSerialized]
		protected bool Display;
		
		[System.NonSerialized]
		protected string VersionText;
		
		[System.NonSerialized]
		private GUIStyle m_VersionStyle;

		[System.NonSerialized]
		private GUIStyle m_CloudBuildStyle;

		[System.NonSerialized]
		private GUIStyle m_CloudBuildRightStyle;
		
		[System.NonSerialized]
		private IEnumerator m_TouchingCoroutine;
		
		#endregion


		#region Properties

		protected Dictionary<string,string> CloudBuildInfo {
			get;
			set;
		}

		private float DisplayExpiryTime {
			get;
			set;
		}

		private bool HasExtendedInfo {
			get {
				return this.CloudBuildInfo != null;
			}
		}

		private GUIStyle CloudBuildStyle {
			get {
				if (m_CloudBuildStyle == null) {
					m_CloudBuildStyle = new GUIStyle(GUI.skin.label);
					m_CloudBuildStyle.fontSize = 18;
				}
				return m_CloudBuildStyle;
			}
		}

		private GUIStyle CloudBuildRightStyle {
			get {
				if (m_CloudBuildRightStyle == null) {
					m_CloudBuildRightStyle = new GUIStyle(this.CloudBuildStyle);
					m_CloudBuildRightStyle.fontStyle = FontStyle.Italic;
					m_CloudBuildRightStyle.alignment = TextAnchor.MiddleRight;
					RectOffset margin = m_CloudBuildRightStyle.margin;
					margin.right = Mathf.Max(margin.right, 10);
					m_CloudBuildRightStyle.margin = margin;
				}
				return m_CloudBuildRightStyle;
			}
		}

		private bool ShowExtendedInfo {
			get;
			set;
		}

		private GUIStyle VersionStyle {
			get {
				if (m_VersionStyle == null) {
					m_VersionStyle = new GUIStyle(GUI.skin.label);
					m_VersionStyle.fontSize = 24;
					m_VersionStyle.fontStyle = FontStyle.Bold;
				}
				return m_VersionStyle;
			}
		}

		#endregion

		
		#region MonoBehaviour Methods


		void Start() {
			#if UNITY_EDITOR
				ProductInfo productInfo = PlatformUtil.GetSettings<ProductInfo>(PlatformUtil.ActivePlatform);
				if (productInfo != null) {
					VersionText = string.Format("{0}.{1} ({2}) built with Unity {3}", productInfo.Version, productInfo.Build, PlatformUtil.ActivePlatform, Application.unityVersion);
				} else {
					VersionText = "Version info not found.";
				}
			#elif UNITY_ANDROID
				VersionText = string.Format("{0}.{1} ({2}) built with Unity {3}", SagoBiz.Controller.Instance.Native.BundleVersion, SagoBiz.Controller.Instance.Native.BundleVersionCode, PlatformUtil.ActivePlatform, Application.unityVersion);
			#elif !SAGO_DISABLE_SAGOBIZ && UNITY_IOS
				VersionText = string.Format("{0} ({1}) built with Unity {2}", SagoBiz.Controller.Instance.Native.BundleVersionCode, PlatformUtil.ActivePlatform, Application.unityVersion);
			#endif

			TextAsset manifest = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
        	if (manifest) {
				this.CloudBuildInfo = JsonConvert.DeserializeObject<Dictionary<string,string>>(manifest.text);
        	}
		}
		
		#if !SAGO_DISABLE_SAGOBIZ || UNITY_ANDROID

			void Update () {

				#if UNITY_EDITOR
					if (Input.GetMouseButton(0) && MinTouchCount > 0)
				#else
					if (Input.touchCount >= MinTouchCount) 
				#endif
					{
						if (m_TouchingCoroutine == null) {
							m_TouchingCoroutine = TouchingCoroutine();
							StartCoroutine(m_TouchingCoroutine);
						}
					} else {
						if (m_TouchingCoroutine != null) {
							StopCoroutine(m_TouchingCoroutine);
							m_TouchingCoroutine = null;
						}
					}
			}
			
			void OnGUI() {
				if (Display) {
					
					GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
						GUILayout.BeginVertical(GUI.skin.box);
							GUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
								GUILayout.Label(VersionText, this.VersionStyle);
								GUILayout.FlexibleSpace();
							GUILayout.EndHorizontal();

							if (this.HasExtendedInfo) {
								
								bool showExtendedInfo = GUI.Toggle(GUILayoutUtility.GetLastRect(), 
									this.ShowExtendedInfo, GUIContent.none, GUIStyle.none);
								
								if (showExtendedInfo != this.ShowExtendedInfo) {
									this.ShowExtendedInfo = showExtendedInfo;
									if (this.ShowExtendedInfo) {
										ExtendDisplayExpiryTime(2f * DisplayDuration);
									}
								}
								
								if (this.ShowExtendedInfo) {
									if (this.CloudBuildInfo != null) {
										GUILayout.Space(10);
										foreach (var kvp in this.CloudBuildInfo) {
											GUILayout.BeginHorizontal();
											GUILayout.Label(string.Format("{0}:", kvp.Key), this.CloudBuildRightStyle, GUILayout.Width(Screen.width * 0.3f));
											GUILayout.Label(kvp.Value, this.CloudBuildStyle);
											GUILayout.EndHorizontal();
										}
									}
								}
							}
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndArea ();
					
				}
			}

		#endif

		#endregion
		
		
		#region Coroutines
		
		private IEnumerator TouchingCoroutine() {
			yield return new WaitForSeconds(TouchDuration);
			StartCoroutine(DisplayVersion());
		}
		
		private IEnumerator DisplayVersion() {
			SagoBiz.Facade.TrackEvent("Display Version");
			Display = true;
			this.ShowExtendedInfo = false;

			this.DisplayExpiryTime = Mathf.Max(this.DisplayExpiryTime, Time.time + DisplayDuration);
			while (Time.time < this.DisplayExpiryTime) {
				yield return null;
			}
			Display = false;
		}

		private void ExtendDisplayExpiryTime(float minExtension) {
			this.DisplayExpiryTime = Mathf.Max(this.DisplayExpiryTime, Time.time + minExtension);
		}

		#endregion
		
		
	}
}
