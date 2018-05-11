namespace SagoPlatformEditor {

	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEngine;

	public class SagoPlatformBuildProcessor {


		#region Const Fields

		public const string RSP_DEFINE_MARKER_TEXT = "-define:";

		public const string DEFINES_REGEX = "^[a-zA-Z]";

		public const string ANALYTICS_DEFINE_SYMBOL_PREFIX = "SAGO_ANALYTICS_NAME_";

		#endregion


		#region Build Processor Callbacks

		[OnBuildPreprocess]
		public static void OnPreprocess(IBuildProcessor processor) {

			ProductInfo info;
			info = PlatformUtil.GetSettings<ProductInfo>();

			if (info) {

				ComputeAnalyticsNameOverride(info);

			}

		}

		#endregion



		#region Intenral Methods

		/// <summary>
		/// Computes current targeted platform's ProductInfo.AnalyticsName based on list of define symbols retrieved.
		/// We are now using SAGO_ANALYTICS_NAME_[OVERRIDE-NAME] define symbol that will override currently targeted
		/// platform's ProductInfo.AnalyticsName. This way we won't need to update target platform prefab to make get
		/// this change in for a build (good example of this use is serveral number of our business development deals).
		/// 
		/// Usage Examples:
		/// 	- Add Following Define Symbol: SAGO_ANALYTICS_NAME_Example
		/// 	- Original Analytics Name: AppName
		/// 	- Updated Analytics Name: AppNameExample
		/// 
		/// Link To Google Doc: https://docs.google.com/a/sagosago.com/document/d/1fW7F6rfcew4aauNEpj83zLGoi8QkG6oi-foG7Q7CZRc/edit?usp=drive_web
		/// 
		/// </summary>
		/// <param name="productInfo">Product info for targeted platform.</param>
		private static void ComputeAnalyticsNameOverride(ProductInfo productInfo) {

			// Unity Cloud Build injects define symbols for its compiler using a .rsp file.
			// Depending on Unity version and which compiler is being used determines what the
			// file gets named, and as well as whether the define symbols are for editor scripts or not.
			//
			// Following is a link to manual for Unity v5.4, Platform Dependent Compilation:
			// https://docs.unity3d.com/540/Documentation/Manual/PlatformDependentCompilation.html
			//
			// To make it more generic, we are basically looking for any files with .rsp extension under
			// [UNITY_PROEJCT]/Assets/ folder and try to process injected define symbols to see if any
			// matches what we are looking for.
			// This code will basically loop through all .rsp files found and terminate once there is
			// a first matching define symbol.
			{

				Debug.LogFormat("#UCB Looking for .rps files in folder: {0}", Application.dataPath);

				string[] rpsFilePaths = System.IO.Directory.GetFiles(Application.dataPath, "*.rsp");
				foreach (string path in rpsFilePaths) {
					Debug.LogFormat("#UCB Found .rsp file: {0}", path);

					string definesString = GetDefinesStringFromRspFile(path);

					Debug.LogFormat("#UCB {0} file has define symbols: {1}", path, definesString);

					// We are basically taking all define symbols for the build target group and
					// see if there is a define symbol with "SAGO_ANALYTICS_NAME_" prefix.
					if (definesString != null) {
						string[] definesArray = definesString.Split(';');
						if (TryAssignAnalyticsNameOverride(productInfo, definesArray)) {
							return;
						}
					}
				}

			}

			// This is a non-Unity-Cloud-Build way of getting define symbols that have been injected through PlayerSettings for a build target.
			{

				BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

				Debug.LogFormat("#UCB Build Target: {0}, Define Symbols In PlayerSettings: {1}", targetGroup, definesString);

				// We are basically taking all define symbols for the build target group and
				// see if there is a define symbol with "SAGO_ANALYTICS_NAME_" prefix.
				if (definesString != null) {
					string[] definesArray = definesString.Split(';');
					if (TryAssignAnalyticsNameOverride(productInfo, definesArray)) {
						return;
					}
				}

			}

		}

		private static bool TryAssignAnalyticsNameOverride(ProductInfo productInfo, string[] definesArray) {

			foreach (string df in definesArray) {

				if (df.StartsWith(ANALYTICS_DEFINE_SYMBOL_PREFIX)) {
					Debug.LogFormat("#UCB Contains SAGO_ANALYTICS_NAME_ define symbol prefix: {0}", df);

					string analyticsDefineSymbolPostfix = df.Replace(ANALYTICS_DEFINE_SYMBOL_PREFIX, "");

					var regexItem = new System.Text.RegularExpressions.Regex(DEFINES_REGEX);
					if (regexItem.IsMatch(analyticsDefineSymbolPostfix)) {
						if (!productInfo.AnalyticsName.Contains(analyticsDefineSymbolPostfix)) {
							productInfo.AnalyticsName = productInfo.AnalyticsName + analyticsDefineSymbolPostfix;
						}
						Debug.LogFormat("#UCB Using Alternative Analytics Name: {0}", productInfo.AnalyticsName);

						// Going to update analytics name with postfix of first prefix match that we find.
						return true;

					} else {
						Debug.LogWarning("#UCB Aborting updating analytics name. Analytics define symbol postfix should not be empty and only contain letters.");
					}
				}

			}

			return false;

		}

		private static string GetDefinesStringFromRspFile(string filePath) {

			try {

				var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

				using (var file = new System.IO.StreamReader(fileStream, System.Text.Encoding.UTF8)) {
					string line;
					while ((line = file.ReadLine()) != null) {
						if (line.Contains(RSP_DEFINE_MARKER_TEXT)) {
							return line.Replace(RSP_DEFINE_MARKER_TEXT, "");
						}
					}
				}

			} catch (System.Exception e) {

				Debug.LogException(e);

			}

			return null;

		}

		#endregion


	}

}
