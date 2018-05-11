namespace SagoLocalizationEditor {
	
	using SagoLocalization;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	using Uri = System.Uri;
	using Mono.Csv;
	
	[CustomEditor(typeof(LocalizedStringReference))]
	public class LocalizedStringReferenceEditor : LocalizedResourceReferenceEditor {
		
		
		#region Static Methods
		
		public static Language GetDefaultLanguage() {
			return new Language("en");
		}
		
		public static string GetSourceDirectory() {
			
			string absoluteScriptsPath;
			absoluteScriptsPath = EditorUtility.OpenFolderPanel("Source Directory", string.Empty, string.Empty);
			
			if (string.IsNullOrEmpty(absoluteScriptsPath) || !Directory.Exists(absoluteScriptsPath)) {
				return null;
			}
			
			string relativeScriptsPath;
			relativeScriptsPath = new Uri(Application.dataPath).MakeRelativeUri(new Uri(absoluteScriptsPath)).ToString();
			
			if (string.IsNullOrEmpty(relativeScriptsPath) || !Directory.Exists(relativeScriptsPath) || !AssetDatabase.IsValidFolder(relativeScriptsPath)) {
				return null;
			}
			
			return relativeScriptsPath;
			
		}
		
		public static IEnumerable<LocalizedString.Json> GetStringsFromTextAsset(TextAsset textAsset) {
			LocalizedString.Json[] value = null;
			try {
				value = JsonUtility.FromJson<LocalizedStringDictionary.Json>(textAsset.text).localizedStrings;
			} catch {
				
			} finally {
				if (value == null) {
					value = new LocalizedString.Json[0];
				}
			}
			return value;
		}
		
		public static IEnumerable<LocalizedString.Json> GetStringsFromMonoScript(string guid) {
			
			string path;
			path = AssetDatabase.GUIDToAssetPath(guid);
			
			string text;
			text = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
			
			Dictionary<string,LocalizedString.Json> dictionary;
			dictionary = new Dictionary<string,LocalizedString.Json>();
			
			{
				string pattern;
				pattern = @"GetString(?:Format)?\(   [\n\r\s\t]*(\/\*\s*(.+?)\s*\*\/)?   [\n\r\s\t]*(\""(.+?)\"")   .+? \)";
				
				MatchCollection matches;
				matches = Regex.Matches(text, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
				
				if (matches.Count != 0) {
					foreach (Match match in matches) {
						LocalizedString.Json json;
						json = new LocalizedString.Json();
						json.reference = string.Format("{0}:{1}", path, GetLineNumber(text, match));
						json.comment = match.Groups[2].Value.ToString();
						json.key = match.Groups[4].Value.ToString();
						json.plural = false;
						json.values = string.IsNullOrEmpty(json.comment) ? new string[0] : new string[]{ json.comment };
						dictionary[json.key] = json;
					}
				}
			}
			
			{
				string pattern;
				pattern = @"GetPluralString(?:Format)?\(   [\n\r\s\t]*(\/\*\s*(.+?)\s*\*\/)?   [\n\r\s\t]*(\""(.+?)\"")   .+? \)";
				
				MatchCollection matches;
				matches = Regex.Matches(text, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
				
				if (matches.Count != 0) {
					foreach (Match match in matches) {
						LocalizedString.Json json;
						json = new LocalizedString.Json();
						json.reference = string.Format("{0}:{1}", path, GetLineNumber(text, match));
						json.comment = match.Groups[2].Value.ToString();
						json.key = match.Groups[4].Value.ToString();
						json.plural = true;
						json.values = string.IsNullOrEmpty(json.comment) ? new string[0] : new string[]{ json.comment };
						dictionary[json.key] = json;
					}
				}
			}
			
			{
				string pattern;
				pattern = @"GetString(?:Format)?\(   [\n\r\s\t]*(""(.+?)""),   [\n\r\s\t]*(""(.+?)"")   .+?   \)";
				
				MatchCollection matches;
				matches = Regex.Matches(text, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
				
				if (matches.Count != 0) {
					foreach (Match match in matches) {
						LocalizedString.Json json;
						json = new LocalizedString.Json();
						json.reference = string.Format("{0}:{1}", path, GetLineNumber(text, match));
						json.comment = match.Groups[4].Value.ToString();
						json.key = match.Groups[2].Value.ToString();
						json.plural = false;
						json.values = string.IsNullOrEmpty(json.comment) ? new string[0] : new string[]{ json.comment };
						dictionary[json.key] = json;
					}
				}
			}
			
			{
				string pattern;
				pattern = @"GetPluralString(?:Format)?\(   [\n\r\s\t]*(""(.+?)""),   [\n\r\s\t]*(""(.+?)"")   .+? \)";
				
				MatchCollection matches;
				matches = Regex.Matches(text, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
				
				if (matches.Count != 0) {
					foreach (Match match in matches) {
						LocalizedString.Json json;
						json = new LocalizedString.Json();
						json.reference = string.Format("{0}:{1}", path, GetLineNumber(text, match));
						json.comment = match.Groups[4].Value.ToString();
						json.key = match.Groups[2].Value.ToString();
						json.plural = true;
						json.values = string.IsNullOrEmpty(json.comment) ? new string[0] : new string[]{ json.comment };
						dictionary[json.key] = json;
					}
				}
			}
			
			return dictionary.Values;
			
		}
		
		public static IEnumerable<LocalizedString.Json> GetStringsFromMonoScriptsInDirectory(string directory) {
			return AssetDatabase.FindAssets("t:MonoScript", new string[]{ directory }).SelectMany(guid => GetStringsFromMonoScript(guid));
		}
		
		public static TextAsset CreateTextAsset(LocalizedStringReference reference, Language language) {
			var path = CreateTextAssetPath(reference, language);
			File.WriteAllText(path, JsonUtility.ToJson(default(LocalizedStringDictionary.Json)).ReplaceSpacesWithTabs());
			AssetDatabase.Refresh();
			UpdateGuids(reference);
			return GetTextAsset(reference, language);
		}
		
		public static string CreateTextAssetPath(LocalizedStringReference reference, Language language) {
			var path = AssetDatabase.GetAssetPath(reference);
			var directory = Path.GetDirectoryName(path);
			var name = Path.GetFileNameWithoutExtension(path);
			var ext = string.Format(".{0}.json", language.Identifier);
			return Path.Combine(directory, Path.ChangeExtension(name, ext));
		}
		
		public static int GetLineNumber(string text, Match match) {
			if (!string.IsNullOrEmpty(text) && match != null) {
				return text.Take(match.Index).Count(c => c == '\n') + 1;
			}
			return -1;
		}
		
		public static TextAsset GetTextAsset(LocalizedStringReference reference, Language language) {
			return AssetDatabase.LoadAssetAtPath<TextAsset>(GetTextAssetPath(reference, language));
		}
		
		public static string GetTextAssetPath(LocalizedStringReference reference, Language language) {
			string guid = reference.GetInfo(language).Guid;
			string path = AssetDatabase.GUIDToAssetPath(guid);
			return !string.IsNullOrEmpty(path) ? path : null;
		}
		
		public static IEnumerable<LocalizedString.Json> MergeStrings(IEnumerable<LocalizedString.Json> jsonStrings, IEnumerable<LocalizedString.Json> sourceStrings) {
			
			var dictionary = new Dictionary<string,LocalizedString.Json>();
			
			foreach (var jsonString in jsonStrings) {
				dictionary[jsonString.key] = jsonString;
			}
			
			foreach (var sourceString in sourceStrings) {
				if (dictionary.ContainsKey(sourceString.key)) {
					var mergedString = dictionary[sourceString.key];
					mergedString.comment = !string.IsNullOrEmpty(sourceString.comment) ? sourceString.comment : mergedString.comment;
					mergedString.reference = !string.IsNullOrEmpty(sourceString.reference) ? sourceString.reference : mergedString.reference;
					mergedString.plural = mergedString.plural || sourceString.plural;
					dictionary[mergedString.key] = mergedString;
				} else {
					dictionary[sourceString.key] = sourceString;
				}
			}
			
			return dictionary.Values;
			
		}
		
		public static void UpdateGuids(LocalizedStringReference reference) {
			
			SerializedObject obj;
			obj = new SerializedObject(reference);
			
			SerializedProperty identifiers;
			identifiers = obj.FindProperty("m_Identifiers");
			
			SerializedProperty guids;
			guids = obj.FindProperty("m_Guids");
			
			for (int index = 0; index < Mathf.Min(identifiers.arraySize, guids.arraySize); index++) {
				var guid = guids.GetArrayElementAtIndex(index);
				if (string.IsNullOrEmpty(guid.stringValue)) {
					var language = new Language(identifiers.GetArrayElementAtIndex(index).stringValue);
					guid.stringValue = AssetDatabase.AssetPathToGUID(CreateTextAssetPath(reference, language));
				}
			}
			
			obj.ApplyModifiedProperties();
			
		}
		
		#endregion
		
		
		#region
		
		[MenuItem("CONTEXT/LocalizedStringReference/Extract...", false, 1000)]
		public static void ExtractStrings(MenuCommand command) {
			
			LocalizedStringReference reference;
			reference = command.context as LocalizedStringReference;
			
			Language language;
			language = GetDefaultLanguage();
			
			TextAsset textAsset;
			textAsset = GetTextAsset(reference, language) ?? CreateTextAsset(reference, language);
			
			IEnumerable<LocalizedString.Json> jsonStrings;
			jsonStrings = GetStringsFromTextAsset(textAsset);
			
			IEnumerable<LocalizedString.Json> sourceStrings;
			sourceStrings = GetStringsFromMonoScriptsInDirectory(GetSourceDirectory());
			
			IEnumerable<LocalizedString.Json> mergedStrings;
			mergedStrings = MergeStrings(jsonStrings, sourceStrings);
			
			LocalizedStringDictionary.Json json;
			json = new LocalizedStringDictionary.Json(mergedStrings.ToArray());
			
			File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), JsonUtility.ToJson(json, true).ReplaceSpacesWithTabs());
			AssetDatabase.Refresh();
			
		}
		
		[MenuItem("CONTEXT/LocalizedStringReference/Import...", false, 2000)]
		public static void ImportStrings(MenuCommand command) {
			LocalizedStringReferenceImportWindow.Open(command.context as LocalizedStringReference);
		}
		
		[MenuItem("CONTEXT/LocalizedStringReference/Export...", false, 2000)]
		public static void ExportStrings(MenuCommand command) {
			LocalizedStringReferenceExportWindow.Open(command.context as LocalizedStringReference);
		}
		
		#endregion
		
		
	}
		
	internal static class StringExtensions {
		
		public static string ReplaceSpacesWithTabs(this string value) {
			return Regex.Replace(
				value, 
				@"^(\s{4})+", 
				(match) => { return string.Join("\t", new string[match.Groups[1].Captures.Count + 1]); }, 
				RegexOptions.Multiline
			);
		}
	
	}
	
	internal abstract class LocalizedStringReferenceAuxWindow : EditorWindow {
		
		
		#region Types
		
		[System.Serializable]
		public struct EditorPrefsValue {
			
			
			#region Fields
			
			[SerializeField]
			public string[] identifiers;
			
			[SerializeField]
			public string path;
			
			#endregion
			
			
			#region Properties
			
			public bool HasValidIdentifiers {
				get { return identifiers != null && identifiers.Length != 0; }
			}
			
			public bool HasValidPath {
				get { return ValidatePath(path); }
			}
			
			public bool IsValid {
				get { return HasValidIdentifiers && HasValidPath; }
			}
			
			#endregion
			
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void ExportToCSV(LocalizedStringReference reference) {
			
			EditorPrefsValue prefs;
			prefs = GetEditorPrefsValue(reference);
			
			if (!prefs.HasValidIdentifiers) {
				throw new System.InvalidOperationException(string.Format("Invalid identifiers: {0}", string.Join(",", prefs.identifiers)));
			}
			
			if (!prefs.HasValidPath) {
				throw new System.InvalidOperationException(string.Format("Invalid path: {0}", prefs.path));
			}
			
			Dictionary<string,Dictionary<string,LocalizedString.Json>> lookup;
			lookup = new Dictionary<string,Dictionary<string,LocalizedString.Json>>();
			
			HashSet<string> identifiers;
			identifiers = new HashSet<string>();
			
			HashSet<string> keys;
			keys = new HashSet<string>();
			
			foreach (string identifier in reference.Identifiers) {
				
				lookup[identifier] = new Dictionary<string,LocalizedString.Json>();
				identifiers.Add(identifier);
				
				TextAsset asset;
				asset = LocalizedStringReferenceEditor.GetTextAsset(reference, new Language(identifier));
				
				foreach (var stringJson in LocalizedStringReferenceEditor.GetStringsFromTextAsset(asset)) {
					lookup[identifier][stringJson.key] = stringJson;
					keys.Add(stringJson.key);
				}
				
			}
			
			int maxPluralCount;
			maxPluralCount = 1;
			
			foreach (string identifier in prefs.identifiers) {
				maxPluralCount = Mathf.Max(maxPluralCount, LocalizedString.GetPluralCount(new Language(identifier)));
			}
			
			using (var writer = new CsvFileWriter(prefs.path)) {
				
				//
				// header
				//
				var header = new List<string>();
				header.Add("Key");
				header.Add("Comment");
				header.Add("Reference");
				header.Add("Plural");
				header.Add("Index");
				header.AddRange(prefs.identifiers);
				writer.WriteRow(header);
				
				// 
				// data
				// 
				foreach (var key in keys) {
					
					LocalizedString.Json src;
					src = lookup["en"][key];
					
					int valueCount;
					valueCount = src.plural ? maxPluralCount : 1;
					
					for (int valueIndex = 0; valueIndex < valueCount; valueIndex++) {
						
						var row = new List<string>();
						row.Add(key);
						row.Add(src.comment);
						row.Add(src.reference);
						row.Add(src.plural.ToString());
						row.Add(valueIndex.ToString());
						
						foreach (var identifier in prefs.identifiers) {
							
							int languagePluralCount;
							languagePluralCount = LocalizedString.GetPluralCount(new Language(identifier));
							
							try {
								if (valueIndex < languagePluralCount) {
									row.Add(lookup[identifier][key].values[valueIndex]);
								} else {
									row.Add("N/A");
								}
							} catch {
								row.Add(string.Empty);
							}
							
						}
						
						writer.WriteRow(row);
						
					}
				}
				
			}
			
			AssetDatabase.Refresh();
			
		}
		
		public static void ImportFromCSV(LocalizedStringReference reference, bool overwrite = false) {
			
			EditorPrefsValue prefs;
			prefs = GetEditorPrefsValue(reference);
			
			if (!prefs.HasValidIdentifiers) {
				throw new System.InvalidOperationException(string.Format("Invalid identifiers: {0}", string.Join(",", prefs.identifiers)));
			}
			
			if (!prefs.HasValidPath) {
				throw new System.InvalidOperationException(string.Format("Invalid path: {0}", prefs.path));
			}
			
			Dictionary<string,Dictionary<string,LocalizedString.Json>> lookup;
			lookup = new Dictionary<string,Dictionary<string,LocalizedString.Json>>();
			
			HashSet<string> identifiers;
			identifiers = new HashSet<string>();
			
			HashSet<string> keys;
			keys = new HashSet<string>();
			
			foreach (string identifier in reference.Identifiers) {
				
				lookup[identifier] = new Dictionary<string,LocalizedString.Json>();
				identifiers.Add(identifier);
				
				if (!overwrite) {
					
					TextAsset asset;
					asset = LocalizedStringReferenceEditor.GetTextAsset(reference, new Language(identifier));
					
					foreach (var stringJson in LocalizedStringReferenceEditor.GetStringsFromTextAsset(asset)) {
						lookup[identifier][stringJson.key] = stringJson;
						keys.Add(stringJson.key);
					}
					
				}
				
			}
			
			using (var reader = new CsvFileReader(prefs.path)) {
				
				List<string> header = new List<string>();
				reader.ReadRow(header);
				
				List<string> row = new List<string>();
				while (reader.ReadRow(row)) {
					
					string key;
					key = row[0];
					
					bool isPlural;
					isPlural = System.Convert.ToBoolean(row[3]);
					
					int valueIndex;
					valueIndex = isPlural ? System.Convert.ToInt32(row[4]) : 0;
					
					for (int index = 5; index < row.Count; index++) {
						
						string identifier;
						identifier = header[index];
						
						int valueCount;
						valueCount = isPlural ? LocalizedString.GetPluralCount(new Language(identifier)) : 1;
						
						if (valueIndex >= 0 && valueIndex < valueCount) {
							
							Dictionary<string,LocalizedString.Json> dictionary;
							if (!lookup.TryGetValue(identifier, out dictionary)) {
								lookup[identifier] = dictionary = new Dictionary<string,LocalizedString.Json>();
							}
							
							LocalizedString.Json localizedString;
							if (!dictionary.TryGetValue(key, out localizedString)) {
								localizedString = new LocalizedString.Json();
								localizedString.key = key;
								localizedString.comment = row[1];
								localizedString.plural = isPlural;
								localizedString.reference = row[2];
								localizedString.values = new string[valueCount];
							}
							
							string[] values;
							values = new string[valueCount];
							
							localizedString.values.CopyTo(values, 0);
							values[valueIndex] = row[index];
							
							localizedString.values = values;
							dictionary[key] = localizedString;
							
						}
						
					}
					
				}
				
				foreach (string identifier in prefs.identifiers) {
					Dictionary<string,LocalizedString.Json> dictionary;
					if (lookup.TryGetValue(identifier, out dictionary) && dictionary.Count != 0) {
						
						LocalizedString.Json[] localizedStrings;
						localizedStrings = dictionary.Values.ToArray();
						
						LocalizedStringDictionary.Json json;
						json = new LocalizedStringDictionary.Json(localizedStrings);
						
						Language language;
						language = new Language(identifier);
						
						string path;
						path = LocalizedStringReferenceEditor.GetTextAssetPath(reference, language) ?? LocalizedStringReferenceEditor.CreateTextAssetPath(reference, language);
						
						if (!string.IsNullOrEmpty(path)) {
							File.WriteAllText(path, JsonUtility.ToJson(json, true).ReplaceSpacesWithTabs());
						}
						
					}
				}
				
				AssetDatabase.Refresh();
				LocalizedStringReferenceEditor.UpdateGuids(reference);
				
			}
			
		}
		
		
		public static string GetEditorPrefsKey(LocalizedStringReference reference) {
			return string.Format(
				"SagoLocalizationEditor.LocalizedResourceReferenceEditor.ImportWindow.{0}", 
				AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(reference))
			);
		}
		
		public static EditorPrefsValue GetEditorPrefsValue(LocalizedStringReference reference) {
			EditorPrefsValue value = JsonUtility.FromJson<EditorPrefsValue>(
				EditorPrefs.GetString(
					GetEditorPrefsKey(reference), 
					JsonUtility.ToJson(default(EditorPrefsValue))
				)
			);
			if (value.identifiers == null) {
				value.identifiers = reference.Identifiers;
			}
			return value;
		}
		
		public static void SetEditorPrefsValue(LocalizedStringReference reference, EditorPrefsValue value) {
			EditorPrefs.SetString(GetEditorPrefsKey(reference), JsonUtility.ToJson(value));
		}
		
		
		public static bool ValidateIdentifiers(string[] identifiers) {
			return identifiers != null && identifiers.Length != 0;
		}
		
		public static bool ValidatePath(string path) {
			return !string.IsNullOrEmpty(path) && File.Exists(path);
		}
		
		#endregion
		
		
		#region Fields
		
		protected LocalizedStringReference m_Reference;
		
		#endregion
		
		
		#region Methods
		
		public EditorPrefsValue IdentifiersField(EditorPrefsValue value) {
			
			string[] identifiers;
			identifiers = m_Reference.Identifiers;
			
			BitArray bitArray;
			bitArray = new BitArray(
				identifiers
				.Select(identifier => System.Array.IndexOf(value.identifiers, identifier) != -1)
				.ToArray()
			);
			
			int[] intArray;
			intArray = new int[] {
				-1
			};
			
			bitArray.CopyTo(intArray, 0);
			intArray[0] = EditorGUILayout.MaskField("Identifiers", intArray[0], identifiers);
			bitArray = new BitArray(intArray);
			
			value.identifiers = (
				identifiers
				.Select((identifier, index) => { return bitArray[index] ? identifier : null; })
				.Where(identifier => !string.IsNullOrEmpty(identifier))
				.ToArray()
			);
			
			return value;
			
		}
		
		public EditorPrefsValue PathField(EditorPrefsValue value) {
			
			GUIStyle defaultStyle;
			defaultStyle = new GUIStyle(EditorStyles.textField);
			
			GUIStyle errorStyle;
			errorStyle = new GUIStyle(defaultStyle);
			errorStyle.active.textColor = Color.red;
			errorStyle.focused.textColor = Color.red;
			errorStyle.hover.textColor = Color.red;
			errorStyle.normal.textColor = Color.red;
			
			EditorGUILayout.BeginHorizontal();
				value.path = EditorGUILayout.TextField("File", value.path, value.HasValidPath ? defaultStyle : errorStyle);
				if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(30))) {
					
					EditorPrefsValue oldValue;
					oldValue = value;
					
					EditorPrefsValue newValue;
					newValue = value;
					newValue.path = EditorUtility.OpenFilePanel(
						"Choose Path", 
						value.HasValidPath ? Path.GetDirectoryName(value.path) : string.Empty, 
						"csv"
					);
					
					ShowAuxWindow();
					value = newValue.HasValidPath ? newValue : oldValue;
					
				}
			EditorGUILayout.EndHorizontal();
			
			return value;
			
		}
		
		#endregion
		
		
	}
	
	internal class LocalizedStringReferenceExportWindow : LocalizedStringReferenceAuxWindow {
		
		
		#region Static Methods
		
		public static LocalizedStringReferenceExportWindow Open(LocalizedStringReference reference) {
			LocalizedStringReferenceExportWindow window;
			window = ScriptableObject.CreateInstance<LocalizedStringReferenceExportWindow>();
			window.m_Reference = reference;
			window.ShowAuxWindow();
			return window;
		}
		
		#endregion
		
		
		#region Methods
		
		public void OnEnable() {
			titleContent = new GUIContent("Export To CSV...");
		}
		
		public void OnGUI() {
			
			GUIStyle groupStyle;
			groupStyle = new GUIStyle(GUIStyle.none);
			groupStyle.margin = new RectOffset(20,20,20,20);
			
			EditorGUILayout.BeginVertical(groupStyle);
				var prefs = GetEditorPrefsValue(m_Reference);
				prefs = IdentifiersField(prefs);
				prefs = PathField(prefs);
				SetEditorPrefsValue(m_Reference, prefs);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(groupStyle);
				EditorGUI.BeginDisabledGroup(!prefs.IsValid);
					if (GUILayout.Button("Export")) {
						ExportToCSV(m_Reference);
						Close();
					}
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
			
		}
		
		#endregion
		
		
	}
	
	internal class LocalizedStringReferenceImportWindow : LocalizedStringReferenceAuxWindow {
		
		
		#region Static Methods
		
		public static LocalizedStringReferenceImportWindow Open(LocalizedStringReference reference) {
			LocalizedStringReferenceImportWindow window;
			window = ScriptableObject.CreateInstance<LocalizedStringReferenceImportWindow>();
			window.m_Reference = reference;
			window.ShowAuxWindow();
			return window;
		}
		
		#endregion
		
		
		#region Methods
		
		public void OnEnable() {
			titleContent = new GUIContent("Import From CSV...");
		}
		
		public void OnGUI() {
			
			GUIStyle groupStyle;
			groupStyle = new GUIStyle(GUIStyle.none);
			groupStyle.margin = new RectOffset(20,20,20,20);
			
			var prefs = GetEditorPrefsValue(m_Reference);
			prefs = IdentifiersField(prefs);
			prefs = PathField(prefs);
			SetEditorPrefsValue(m_Reference, prefs);
			
			EditorGUILayout.BeginHorizontal(groupStyle);
				EditorGUI.BeginDisabledGroup(!prefs.IsValid);
					if (GUILayout.Button("Import and Merge")) {
						ImportFromCSV(m_Reference, false);
						Close();
					}
					GUILayout.Space(10);
					if (GUILayout.Button("Import and Overwrite")) {
						ImportFromCSV(m_Reference, true);
						Close();
					}
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
			
		}
		
		#endregion
	
	
	}
	
}