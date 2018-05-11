namespace SagoApp.Content {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using PlistCS;
	using UnityEditor;
	using SagoBuildEditor.Core;
	using SagoApp.Project;
	using LocalizationDictionaryValue = System.Collections.Generic.Dictionary<string, string>;
	using LocalizationDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

	public class iOSSettingsBuildProcessor : MonoBehaviour {
		
		
		[MenuItem("Sago/App/Update iOS Settings Bundle")]
		public static void UpdateSettingsBundle() {
			UpdateSettingsBundle(null);
		}

		[OnBuildPreprocess]
		public static void UpdateSettingsBundle(IBuildProcessor processor) {
			List<object> prefs = new List<object>();
			LocalizationDictionary localization = CreateLocalizationDictionary();


			foreach (var type in FindSettingsGroupTypes()) {
				List<object> tempPrefs = new List<object>();
				AddSettingsGroup(type, tempPrefs, localization);
				AddToggleSwitch(type, tempPrefs, localization);
				AddMultiValue(type, tempPrefs, localization);
				if (tempPrefs.Count > 1) {
					for (int i = 0; i < tempPrefs.Count; i++) {
						prefs.Add(tempPrefs[i]);
					}
				}
			}
			
			WriteLocalization(localization);
			WritePrefs(prefs);
			
			UnityEditor.AssetDatabase.Refresh();
		}

		
		static private IEnumerable<System.Type> FindSettingsGroupTypes() {
			IEnumerable<System.Type> types = (
			    System.AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany((assembly) => {
				return assembly.GetTypes();
			})
				.Where((type) => {
				return HasCustomAttribute<iOSSettingsGroupAttribute>(type);
			})
				.OrderByDescending((type) => {
				return GetCustomAttribute<iOSSettingsGroupAttribute>(type).Config.Priority;
			})
				.ThenBy((type) => {
				return GetCustomAttribute<iOSSettingsGroupAttribute>(type).Config.TitleKey;
			})
			);
			return types;
		}

		static private T GetCustomAttribute<T>(System.Type type) {
			return (T)type.GetCustomAttributes(typeof(T), false).FirstOrDefault();
		}

		static private bool HasCustomAttribute<T>(System.Type type) {
			return type.GetCustomAttributes(typeof(T), false).Count() > 0;
		}

		static private T GetCustomAttribute<T>(FieldInfo fieldInfo) {
			return (T)fieldInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
		}

		static private bool HasCustomAttribute<T>(FieldInfo fieldInfo) {
			return fieldInfo.GetCustomAttributes(typeof(T), false).Count() > 0;
		}

		
		static private LocalizationDictionary CreateLocalizationDictionary() {
			LocalizationDictionary localization = new LocalizationDictionary();
			localization.Add("en", new LocalizationDictionaryValue());
			localization.Add("de", new LocalizationDictionaryValue());
			localization.Add("fr", new LocalizationDictionaryValue());
			localization.Add("ja", new LocalizationDictionaryValue());
			localization.Add("ru", new LocalizationDictionaryValue());
			localization.Add("zh-Hans", new LocalizationDictionaryValue());
			localization.Add("zh-Hant", new LocalizationDictionaryValue());
			localization.Add("es", new LocalizationDictionaryValue());
			localization.Add("pt-BR", new LocalizationDictionaryValue());
			return localization;
		}

		static private void AddToLocalizationDictionary(string key, Dictionary<string, string> localizedValues, LocalizationDictionary localization) {
			foreach (var kvp in localizedValues) {
				if (localization.ContainsKey(kvp.Key) && !localization[kvp.Key].ContainsKey(key)) {
					Debug.Log("SettingBundleLocalization[" + kvp.Key + "]  :   " + key + " = " + kvp.Value);
					localization[kvp.Key].Add(key, kvp.Value);
				}
			}
		}

		static private void AddSettingsGroup(System.Type type, List<object> prefs, LocalizationDictionary localization) {
			iOSSettingsGroupAttribute groupAttribute = GetCustomAttribute<iOSSettingsGroupAttribute>(type);
			iOSSettingsGroupConfig config = groupAttribute.Config;
			if (!config.Equals(default(iOSSettingsGroupConfig))) {
				AddToLocalizationDictionary(config.TitleKey, config.TitleLocalization, localization);
				prefs.Add(new Dictionary<string,object>() {
					{ "Title",  config.TitleKey },
					{ "Type", "PSGroupSpecifier" }
				});
			}
		}

		static private void AddToggleSwitch(System.Type type, List<object> prefs, LocalizationDictionary localization) {
			IEnumerable<FieldInfo> fields = (
			    type
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where((field) => {
				return field.GetCustomAttributes(typeof(iOSToggleSwitchAttribute), false).Count() > 0;
			}).Where((field) => {
				if (ProjectInfo.Instance.IsStandaloneProject && field.GetCustomAttributes(typeof(iOSCompositeOnlyAttribute), false).Count() != 0) {
					return false;
				}
				if (ProjectInfo.Instance.IsCompositeProject && field.GetCustomAttributes(typeof(iOSStandaloneOnlyAttribute), false).Count() != 0) {
					return false;
				}
				return true;
			})
			);

			foreach (var field in fields) {
				iOSToggleSwitchAttribute toggleSwitchAttribute = GetCustomAttribute<iOSToggleSwitchAttribute>(field);
				iOSToggleSwitchConfig config = toggleSwitchAttribute.Config;
				if (!config.Equals(default(iOSToggleSwitchConfig))) {
					AddToLocalizationDictionary(config.TitleKey, config.TitleLocalization, localization);
					prefs.Add(new Dictionary<string,object>() {
						{ "DefaultValue", config.DefaultValue },
						{ "Key", field.GetValue(null) },
						{ "Title", config.TitleKey },
						{ "Type", "PSToggleSwitchSpecifier" }
					});
				}
			}
		}

		static private void AddMultiValue(System.Type type, List<object> prefs, LocalizationDictionary localization) {
			IEnumerable<FieldInfo> fields = (
			     type
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where((field) => {
				return field.GetCustomAttributes(typeof(iOSMultiValueAttribute), false).Count() > 0;
			}).Where((field) => {
				if (ProjectInfo.Instance.IsStandaloneProject && field.GetCustomAttributes(typeof(iOSCompositeOnlyAttribute), false).Count() != 0) {
					return false;
				}
				if (ProjectInfo.Instance.IsCompositeProject && field.GetCustomAttributes(typeof(iOSStandaloneOnlyAttribute), false).Count() != 0) {
					return false;
				}
				return true;
			})
			);

			foreach (var field in fields) {
				iOSMultiValueAttribute multiValueAttribute = GetCustomAttribute<iOSMultiValueAttribute>(field);
				iOSMultiValueConfig config = multiValueAttribute.Config;
				List<object> titles = new List<object>();
				List<object> values = new List<object>();
				if (!config.Equals(default(iOSMultiValueConfig))) {
					foreach (KeyValuePair<string,Dictionary<string, string>> k in config.Values) {
						titles.Add(k.Key);
					}
					AddToLocalizationDictionary(config.TitleKey, config.TitleLocalization, localization);
					foreach (KeyValuePair<string, Dictionary<string, string>> k in config.Values) {
						AddToLocalizationDictionary(k.Key, k.Value, localization);
						values.Add(k.Key);
					}
					prefs.Add(new Dictionary<string,object>() {
						{ "Title" , config.TitleKey },
						{ "Key", field.GetValue(null) },
						{ "Type", "PSMultiValueSpecifier" },
						{ "Titles", titles },
						{ "Values", values },
						{ "DefaultValue", values[0] }
					});
				}
			}
		}

		static private void WriteLocalization(LocalizationDictionary localization) {
			foreach (KeyValuePair<string, Dictionary<string, string>> kvp in localization) {
				string languageFile = "";
				Debug.Log(kvp.Key);
				foreach (KeyValuePair<string, string> k in kvp.Value) {
					languageFile += "\"" + k.Key + "\" = \"" + k.Value + "\";\n";
				}
				Debug.Log(languageFile);
				System.IO.Directory.CreateDirectory("Assets/Build/iOS/.Native/Settings.bundle/" + kvp.Key + ".lproj");
				System.IO.File.WriteAllText("Assets/Build/iOS/.Native/Settings.bundle/" + kvp.Key + ".lproj/Root.strings", languageFile, System.Text.Encoding.GetEncoding("utf-16"));
			}
		}

		static private void WritePrefs(List<object> prefs) {
			Dictionary<string,object> plist = new Dictionary<string,object>();
			plist["PreferenceSpecifiers"] = prefs;
			plist.Add("StringsTable", "Root");
			Plist.writeXml(plist, "Assets/Build/iOS/.Native/Settings.bundle/Root.plist");	
		}
		
		
	}
}