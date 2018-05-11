namespace SagoUtilsEditor {
	
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	using Version = System.Version;
	
	/// <summary>
	/// The DefineSymbolUtil class provides methods for adding and removing 
	/// define symbols from the player settings.
	/// </summary>
	public static class DefineSymbolUtil {
		
		/// <summary>
		/// Adds the define symbol to the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbol">
		/// The define symbol to add.
		/// </param>
		static public void AddDefineSymbol(BuildTargetGroup buildTargetGroup, string defineSymbol) {
			DefineSymbol(buildTargetGroup, defineSymbol, true);
		}
		
		/// <summary>
		/// Adds the define symbols to the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbols">
		/// The define symbols to add.
		/// </param>
		static public void AddDefineSymbols(BuildTargetGroup buildTargetGroup, HashSet<string> defineSymbols) {
			foreach (string defineSymbol in defineSymbols) {
				AddDefineSymbol(buildTargetGroup, defineSymbol);
			}
		}
		
		/// <summary>
		/// Removes the define symbol from the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbol">
		/// The define symbol to remove.
		/// </param>
		static public void RemoveDefineSymbol(BuildTargetGroup buildTargetGroup, string defineSymbol) {
			DefineSymbol(buildTargetGroup, defineSymbol, false);
		}
		
		/// <summary>
		/// Removes the define symbols from the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbols">
		/// The define symbols to remove.
		/// </param>
		static public void RemoveDefineSymbols(BuildTargetGroup buildTargetGroup, HashSet<string> defineSymbols) {
			foreach (string defineSymbol in defineSymbols) {
				RemoveDefineSymbol(buildTargetGroup, defineSymbol);
			}
		}
		
		/// <summary>
		/// Adds or removes the define symbol for the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbol">
		/// The define symbol to add or remove.
		/// </param>
		/// <param name="enabled">
		/// The flag indicated whether the define symbol should be added or removed.
		/// </param>
		static public void DefineSymbol(BuildTargetGroup buildTargetGroup, string defineSymbol, bool enabled) {
			if (buildTargetGroup != BuildTargetGroup.Unknown) {
				
				// fix for tvOS on Unity 5.3.
				Version currentVersion = new Version(Regex.Replace(Application.unityVersion, @"\D+", "."));
				Version requiredVersion = new Version("5.3.2");
				if (currentVersion < requiredVersion) {
					if ((int)buildTargetGroup == 25) {
						return;
					}
				}
				
				string delimiter;
				delimiter = ";";
				
				string value;
				value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
				
				HashSet<string> hash;
				hash = new HashSet<string>(value.Split(delimiter.ToCharArray()));
			 	
				if (enabled) {
					hash.Add(defineSymbol.ToString());
				} else {
					hash.Remove(defineSymbol.ToString());
				}
				
				value = string.Join(delimiter, hash.ToArray());
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, value);
				
			}
		}
		
		/// <summary>
		/// Adds or removes the define symbols for the build target group.
		/// </summary>
		/// <param name="buildTargetGroup">
		/// The build target group.
		/// </param>
		/// <param name="defineSymbol">
		/// The define symbols to add or remove.
		/// </param>
		/// <param name="enabled">
		/// The flag indicated whether the define symbol should be added or removed.
		/// </param>
		static public void DefineSymbols(BuildTargetGroup buildTargetGroup, HashSet<string> defineSymbols, bool enabled) {
			foreach (string defineSymbol in defineSymbols) {
				DefineSymbol(buildTargetGroup, defineSymbol, enabled);
			}
		}
		
	}
	
}