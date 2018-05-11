namespace SagoLocalization {
	
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using UnityEngine;
	
	public class LocaleProvider {
		
		
		#region Types
		
		[System.Serializable]
		public struct Json {
			
			[SerializeField]
			public string localeIdentifier;
			
			[SerializeField]
			public string[] preferredLanguageIdentifiers;
			
		}
		
		#endregion
		
		
		#region Constants
		
		private const string EditorPrefsKey = "SagoLocalization.LocaleProvider";
		
		private const string PlayerPrefsKey = "SagoLocalization.LocaleProvider";
		
		#endregion


		#region Static Fields

		#if UNITY_ANDROID && !UNITY_EDITOR
			private static AndroidJavaClass sagoLocalizationJavaClass;
		#endif

		#endregion
		
		
		#region Static Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			// Caching sagoLocalizationJavaClass.
			#if UNITY_ANDROID && !UNITY_EDITOR
				sagoLocalizationJavaClass = new AndroidJavaClass("com.sagosago.SagoLocalization.Localization");
			#endif
		}
		
		public static void ClearEditorPrefs() {
			#if UNITY_EDITOR
				UnityEditor.EditorPrefs.DeleteKey(EditorPrefsKey);
			#endif
		}
		
		public static void ClearPlayerPrefs() {
			PlayerPrefs.DeleteKey(PlayerPrefsKey);
		}
		
		public static Json ReadFromEditorPrefs() {
			#if UNITY_EDITOR
				return JsonUtility.FromJson<Json>(UnityEditor.EditorPrefs.GetString(EditorPrefsKey, JsonUtility.ToJson(default(Json))));
			#else
				return default(Json);
			#endif
		}
		
		#if UNITY_IOS && !UNITY_EDITOR
		
			[DllImport("__Internal")]
			private static extern string _SagoLocalization_currentLocaleJson();
			
		#endif
		
		public static Json ReadFromNativePrefs() {
			#if UNITY_EDITOR
				return default(Json);
			#elif UNITY_IOS
				return JsonUtility.FromJson<Json>(_SagoLocalization_currentLocaleJson());
			#elif UNITY_ANDROID
				string jsonString = sagoLocalizationJavaClass.CallStatic<string>("_SagoLocalization_currentLocaleJson");
				return JsonUtility.FromJson<Json>(jsonString);
			#else
				return default(Json);
			#endif
		}
		
		public static Json ReadFromPlayerPrefs() {
			return JsonUtility.FromJson<Json>(PlayerPrefs.GetString(PlayerPrefsKey, JsonUtility.ToJson(default(Json))));
		}
		
		public static void WriteToEditorPrefs(Json json) {
			#if UNITY_EDITOR
				UnityEditor.EditorPrefs.SetString(EditorPrefsKey, JsonUtility.ToJson(json, true));
			#endif
		}
		
		public static void WriteToPlayerPrefs(Json json) {
			PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(json, true));
		}
		
		#endregion
		
		
		#region Methods
		
		public static Locale GetLocale() {
			
			Json editorPrefs = ReadFromEditorPrefs();
			Json nativePrefs = ReadFromNativePrefs();
			Json playerPrefs = ReadFromPlayerPrefs();

			string localeIdentifier;
			localeIdentifier = GetLocaleIdentifier(playerPrefs, nativePrefs, editorPrefs);
			
			Language[] preferredLanguages;
			preferredLanguages = GetPreferredLanguages(playerPrefs, nativePrefs, editorPrefs);

			return new Locale(localeIdentifier, preferredLanguages);
			
		}
		
		private static string GetLocaleIdentifier(params Json[] args) {
			
			string localeIdentifier = (
				args
				.Where(arg => Locale.IsValid(arg.localeIdentifier))
				.Select(arg => arg.localeIdentifier)
				.FirstOrDefault()
			);
			
			if (!Locale.IsValid(localeIdentifier)) {
				localeIdentifier = "en-US";
			}
			
			return localeIdentifier;
			
		}
		
		private static Language[] GetPreferredLanguages(params Json[] args) {
			
			Language[] languages = (
				args
				.Where(arg => arg.preferredLanguageIdentifiers != null)
				.SelectMany(arg => arg.preferredLanguageIdentifiers)
				.Distinct()
				.Where(identifier => Language.IsValid(identifier))
				.Select(identifier => new Language(identifier))
				.ToArray()
			);
			
			if (languages.Length == 0) {
				languages = new Language[] {
					new Language("en")
				};
			}
			
			return languages;
			
		}
		
		#endregion
		
		
	}
	
}




/*
#if UNITY_EDITOR

namespace SagoLocalization {
	
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[System.Serializable]
	public struct LocaleProviderOptions {
		
		
		#region Constants
		
		private const string EditorPrefsKey = "SagoLocalization.LocaleProviderOptions";
		
		#endregion
		
		
		#region Static Methods
		
		public static LocaleProviderOptions Normalize(LocaleProviderOptions value) {
			if (string.IsNullOrEmpty(value.LocaleIdentifier)) {
				value.LocaleIdentifier = "en";
				value.PreferredLanguageIdentifiers = new string[]{ value.LocaleIdentifier };
			}
			if (value.PreferredLanguageIdentifiers == null || value.PreferredLanguageIdentifiers.Length == 0) {
				value.PreferredLanguageIdentifiers = new string[]{ value.LocaleIdentifier };
			}
			return value;
		}
		
		public static LocaleProviderOptions Read() {
			return Normalize(JsonUtility.FromJson<LocaleProviderOptions>(EditorPrefs.GetString(EditorPrefsKey, "{}")));
		}
		
		public static void Write(LocaleProviderOptions value) {
			EditorPrefs.SetString(EditorPrefsKey, JsonUtility.ToJson(Normalize(value)));
		}
		
		#endregion
		
		
		#region Constructor
		
		public LocaleProviderOptions(string localeIdentifier, string[] preferredLanguageIdentifiers) {
			LocaleIdentifier = localeIdentifier;
			PreferredLanguageIdentifiers = preferredLanguageIdentifiers;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		public string LocaleIdentifier;
		
		[SerializeField]
		public string[] PreferredLanguageIdentifiers;
		
		#endregion
		
		
		#region Properties
		
		public Locale Locale {
			get { return new Locale(LocaleIdentifier, PreferredLanguages); }
		}
		
		public Language[] PreferredLanguages {
			get {
				return (
					PreferredLanguageIdentifiers
					.Where(identifier => Language.IsValid(identifier))
					.Select(identifier => new Language(identifier))
					.ToArray()
				);
			}
		}
		
		#endregion
		
		
	}
	
	public static class LocaleProvider {
		
		public static Locale GetLocale() {
			return LocaleProviderOptions.Read().Locale;
		}
		
	}
	
}

#else

namespace SagoLocalization {
	
	public class LocaleProvider {
		
		public static Locale GetLocale() {
			return new Locale("en");
		}
		
	}
	
}

#endif
*/