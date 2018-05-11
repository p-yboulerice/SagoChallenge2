namespace SagoDebugEditor {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	
	/// <summary>
	/// Editor utilities for the SagoDebug submodule.
	/// </summary>
	public static class DebugEditorUtils {


		#region Public Methods

		/// <summary>
		/// Updates the define symbol for the current build target.
		/// </summary>
		/// <param name="defineSymbol">Define symbol.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void UpdateDefineSymbol(string defineSymbol, bool enabled) {
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.activeBuildTarget.ConvertToGroup();
			UpdateDefineSymbol(buildTargetGroup, defineSymbol, enabled);
		}

		/// <summary>
		/// Updates the define symbol for the given BuildTargetGroup
		/// </summary>
		/// <param name="buildTargetGroup">Build target group.</param>
		/// <param name="defineSymbol">Define symbol.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void UpdateDefineSymbol(BuildTargetGroup buildTargetGroup, string defineSymbol, bool enabled) {
			if (enabled) {
				UpdateDefineSymbols(buildTargetGroup, new string[] { defineSymbol }, null);
			} else {
				UpdateDefineSymbols(buildTargetGroup, null, new string[] { defineSymbol });
			}
        }

        /// <summary>
        /// Batch enable/disable of define symbols (so only one recompile) for
        /// the current build target.
        /// </summary>
        /// <param name="defineSymbols">List of symbols to define/enable</param>
        /// <param name="undefineSymbols">List of symbols to undefine/disable</param>
		public static void UpdateDefineSymbols(string[] defineSymbols, string[] undefineSymbols = null) {
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.activeBuildTarget.ConvertToGroup();
			UpdateDefineSymbols(buildTargetGroup, defineSymbols, undefineSymbols);
        }

        /// <summary>
        /// Batch enable/disable of define symbols (so only one recompile).
        /// </summary>
        /// <param name="buildTargetGroup">Build target group.</param>
        /// <param name="defineSymbols">Define symbols.</param>
        /// <param name="undefineSymbols">Undefine symbols.</param>
		public static void UpdateDefineSymbols(BuildTargetGroup buildTargetGroup, string[] defineSymbols, string[] undefineSymbols = null) {
			
			const string delimiter = ";";

			string value;
			value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

			HashSet<string> hash;
			hash = new HashSet<string>(
				value.Split(delimiter.ToCharArray()).
				Except(undefineSymbols ?? new string[]{}).
				Union(defineSymbols ?? new string[]{})
				);
			
			value = string.Join(delimiter, hash.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, value);

        }

        /// <summary>
        /// Determines if the given define symbol is defined for the current build target.
        /// </summary>
        /// <returns><c>true</c> if the symbol is defined for the current build target; otherwise, <c>false</c>.</returns>
        /// <param name="defineSymbol">Define symbol.</param>
        public static bool IsSymbolDefined(string defineSymbol) {
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.activeBuildTarget.ConvertToGroup();
        	return IsSymbolDefined(buildTargetGroup, defineSymbol);
        }

        /// <summary>
        /// Determines if the given define symbol is defined the specified buildTargetGroup.
        /// </summary>
        /// <returns><c>true</c> if the symbol is defined for the specified buildTargetGroup; otherwise, <c>false</c>.</returns>
        /// <param name="buildTargetGroup">Build target group.</param>
        /// <param name="defineSymbol">Define symbol.</param>
        public static bool IsSymbolDefined(BuildTargetGroup buildTargetGroup, string defineSymbol) {

			const string delimiter = ";";

			string value;
			value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

			HashSet<string> hash;
			hash = new HashSet<string>(value.Split(delimiter.ToCharArray()));

			return hash.Contains(defineSymbol);

        }

        /// <summary>
        /// Extension method to convert a BuildTarget to its corresponding BuildTargetGroup.
        /// </summary>
        /// <returns>The BuildTargetGroup that this BuildTarget belongs to.</returns>
        /// <param name="buildTarget">BuildTarget</param>
		public static BuildTargetGroup ConvertToGroup(this BuildTarget buildTarget) {
		    BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
		    switch (buildTarget)
		    {
		        case BuildTarget.StandaloneOSX:
		        case BuildTarget.StandaloneLinux:
		        case BuildTarget.StandaloneLinux64:
		        case BuildTarget.StandaloneLinuxUniversal:
		        case BuildTarget.StandaloneWindows:
		        case BuildTarget.StandaloneWindows64:
		            targetGroup = BuildTargetGroup.Standalone;
		            break;
		        case BuildTarget.iOS:
		            targetGroup = BuildTargetGroup.iOS;
		            break;
		        case BuildTarget.Android:
		            targetGroup = BuildTargetGroup.Android;
		            break;
		        case BuildTarget.WebGL:
		        	targetGroup = BuildTargetGroup.WebGL;
		        	break;
		        case BuildTarget.WSAPlayer:
		            targetGroup = BuildTargetGroup.WSA;
		            break;
		        case BuildTarget.PSP2:
		            targetGroup = BuildTargetGroup.PSP2;
		            break;
		        case BuildTarget.PS4:
		            targetGroup = BuildTargetGroup.PS4;
		            break;
		        case BuildTarget.PSM:
		            targetGroup = BuildTargetGroup.PSM;
		            break;
		        case BuildTarget.XboxOne:
		            targetGroup = BuildTargetGroup.XboxOne;
		            break;
		        default:
		            throw new System.ArgumentOutOfRangeException("buildTarget");
		    }
		    return targetGroup;
		}

		#endregion


		#region Menu Items

		#if SAGO_DEBUG
		[MenuItem("Sago/Debug/Disable SAGO_DEBUG", false, 151)]
		#else
		[MenuItem("Sago/Debug/Enable SAGO_DEBUG", false, 151)]
		#endif
		static void MenuToggleSagoDebug(MenuCommand command) {

			bool turnOn;
			#if SAGO_DEBUG
			turnOn = false;
			#else
			turnOn = true;
			#endif

			UpdateDefineSymbol("SAGO_DEBUG", turnOn);
			AssetDatabase.SaveAssets();
		}

		#endregion


	}
	
}
