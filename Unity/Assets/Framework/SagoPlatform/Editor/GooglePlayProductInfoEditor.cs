namespace SagoPlatformEditor {
	
	using SagoCore.Submodules;
	using SagoPlatform;
	using SagoUtilsEditor;
	using SagoUtils;
	using System.Xml.Linq;
	using System.IO;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(GooglePlayProductInfo))]
	public class GooglePlayProductInfoEditor : Editor {
		
		#region Static Fields
		
		/// <summary>
		/// Path at which the license public key is going to be stored.
		/// </summary>
		private static string licensePublicKeyPath {
			get { return Application.dataPath + "/Plugins/Android/.GooglePlay/res/values/strings.xml"; }
		}
		
		/// <summary>
		/// The license public key path template which resides in SagoPlatform submodule.
		/// </summary>
		private static string licensePublicKeyPathTemplate {
			get {
				return Path.Combine(
					SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoPlatform.SubmoduleInfo)), 
					"Plugins/Android/.GooglePlay/res/values/strings.xml"
				);
			}
		}

		/// <summary>
		/// Path at which the license public key is going to be stored (GooglePlayFree).
		/// </summary>
		private static string licensePublicKeyPathGooglePlayFree {
			get { return Application.dataPath + "/Plugins/Android/.GooglePlayFree/res/values/strings.xml"; }
		}
		
		/// <summary>
		/// The license public key path template which resides in SagoPlatform submodule (GooglePlayFree).
		/// </summary>
		private static string licensePublicKeyPathTemplateGooglePlayFree {
			get {
				return Path.Combine(
					SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoPlatform.SubmoduleInfo)), 
					"Plugins/Android/.GooglePlayFree/res/values/strings.xml"
				);
			}
		}

		/// <summary>
		/// The node in licensePublicKeyPath where the license value is to be stored.
		/// </summary>
		private static string xmlNodeLicensePublicKey = "google_play_license_public_key";

		private static string xmlNodeUseExpansionFile = "google_play_use_expansion_file";

		private static string xmlNodeDebugExpansionFile = "google_play_debug_expansion_file";
		#endregion 
		
		#region Public Methods
	
		/// <summary>
		/// Updates the strings.xml.
		/// </summary>
		/// <param name="productInfo">Product info.</param>
		public static void UpdateStringsXml(GooglePlayProductInfo productInfo, Platform targetPlatform) {
			
			bool defineSymbolSagoGooglePlayExpansionDownloader = false;
			#if SAGO_GOOGLE_PLAY_EXPANSION_DOWNLOADER
				defineSymbolSagoGooglePlayExpansionDownloader = true;
			#endif
			
			string licensePath = "";
			string licensePathTemplate = "";
			if (targetPlatform == SagoPlatform.Platform.GooglePlay) {
				licensePath = licensePublicKeyPath;
				licensePathTemplate = licensePublicKeyPathTemplate;
			} else {
				licensePath = licensePublicKeyPathGooglePlayFree;
				licensePathTemplate = licensePublicKeyPathTemplateGooglePlayFree;
			}

			if (!File.Exists(licensePath)) {
				AssetDatabase.StartAssetEditing();

				AssetUtil.CopyAsset(licensePathTemplate, licensePath, true);

				AssetDatabase.StopAssetEditing();
				AssetDatabase.Refresh();
			}

			XDocument xmlFile = XDocument.Load(licensePath); 

			try {
				var querySkipDownload = from c in xmlFile.Elements("resources").Elements("string")
										where c.Attribute("name").Value == xmlNodeUseExpansionFile
										select c;

				foreach (XElement googlePlayUseExpansionFile in querySkipDownload) {
					googlePlayUseExpansionFile.Value = defineSymbolSagoGooglePlayExpansionDownloader ? "true" : "false";
				}
				
			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}

			// Set License Public Key
			try {
				var queryLicensePublicKey = from c in xmlFile.Elements("resources").Elements("string")
											where c.Attribute("name").Value == xmlNodeLicensePublicKey
											select c;

				foreach (XElement licenseKey in queryLicensePublicKey) {
					if (!defineSymbolSagoGooglePlayExpansionDownloader || productInfo.LicensePublicKey == null) {
						licenseKey.Value= "";
					} else {
						licenseKey.Value= productInfo.LicensePublicKey;
					}
				}

			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}

			// Enable/Disable googleplay debug mode
			try {
				var queryDownloaderDebugMode = from c in xmlFile.Elements("resources").Elements("string")
											   where c.Attribute("name").Value == xmlNodeDebugExpansionFile
											   select c;
				
				foreach (XElement debugMode in queryDownloaderDebugMode) {
					debugMode.Value= productInfo.GooglePlayDownloaderDebugMode ? "true" : "false";
				}
				
			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}

			xmlFile.Save(licensePath);
		}

		/// <summary>
		/// Updates the google play android project resources.
		/// </summary>
		/// <param name="oldPlatform">Old platform.</param>
		/// <param name="newPlatform">New platform.</param>
		[OnPlatformDidChange(4)]
		public static void UpdateGooglePlayProjectResources(Platform oldPlatform, Platform newPlatform) {
			
			bool defineSymbolSagoGooglePlayExpansionDownloader = false;
			#if SAGO_GOOGLE_PLAY_EXPANSION_DOWNLOADER
				defineSymbolSagoGooglePlayExpansionDownloader = true;
			#endif

			GooglePlayProductInfo productInfo = null;
			if (newPlatform == Platform.GooglePlay) {
				productInfo = PlatformUtil.GetSettings<GooglePlayProductInfo>(Platform.GooglePlay);
			} else if (newPlatform == Platform.GooglePlayFree) {
				productInfo = PlatformUtil.GetSettings<GooglePlayProductInfo>(Platform.GooglePlayFree);
			}

			if (productInfo != null) {
				UpdateStringsXml(productInfo, newPlatform);
			}

		}

		#endregion

		#region Custom Inspector

		public override void OnInspectorGUI () {

			base.OnInspectorGUI ();

			GooglePlayProductInfo productInfo = target as GooglePlayProductInfo;

			EditorGUI.BeginChangeCheck();

			productInfo.GooglePlayDownloaderDebugMode = EditorGUILayout.Toggle("Debug Expansion File", productInfo.GooglePlayDownloaderDebugMode);

			EditorGUILayout.BeginVertical(GUI.skin.box); {

				productInfo.LicensePublicKey = EditorGUILayout.TextField("License Public Key", productInfo.LicensePublicKey);

			} EditorGUILayout.EndVertical();

			bool objectIsModified = EditorGUI.EndChangeCheck();

			if (objectIsModified) {
				EditorUtility.SetDirty(target);
			}

			#if SAGO_GOOGLE_PLAY_EXPANSION_DOWNLOADER
				if (string.IsNullOrEmpty(productInfo.LicensePublicKey)) { // The RSA key
					EditorGUILayout.HelpBox("Please enter the License Public Key.", MessageType.Error);
				}
			#endif
			
		}

		#endregion



	}
}