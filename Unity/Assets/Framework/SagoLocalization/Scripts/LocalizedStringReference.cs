namespace SagoLocalization {
	
	using SagoCore.Resources;
	using System.Collections.Generic;
	using UnityEngine;
	
	[CreateAssetMenu]
	public class LocalizedStringReference : LocalizedResourceReference<TextAsset> {
		
		
		#region Fields
		
		private Dictionary<string,LocalizedStringDictionary> m_LanguageDictionary;
		
		#endregion
		
		
		#region Methods
		
		public void Clear() {
			if (m_LanguageDictionary != null) {
				m_LanguageDictionary.Clear();
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		public string GetString(string key) {
			return GetString(key, key, Locale.Current);
		}
		
		public string GetString(string key, Language language) {
			return GetString(key, key, language);
		}
		
		public string GetString(string key, Locale locale) {
			return GetString(key, key, locale);
		}
		
		
		public string GetString(string key, string value) {
			return GetString(key, value, Locale.Current);
		}
		
		public string GetString(string key, string value, Language language) {
			LocalizedString localizedString = GetLocalizedString(key, language);
			if (!localizedString.Equals(default(LocalizedString))) {
				string localizedValue = localizedString.GetString();
				if (!string.IsNullOrEmpty(localizedValue)) {
					return localizedValue;
				}
			}
			Debug.LogWarningFormat("Could not get string for key: {0}", key);
			return value;
		}
		
		public string GetString(string key, string value, Locale locale) {
			for (int index = 0; index < locale.PreferredLanguages.Length; index++) {
				LocalizedString localizedString = GetLocalizedString(key, locale.PreferredLanguages[index]);
				if (!localizedString.Equals(default(LocalizedString))) {
					string localizedValue = localizedString.GetString();
					if (!string.IsNullOrEmpty(localizedValue)) {
						return localizedValue;
					}
				}
			}
			Debug.LogWarningFormat("Could not get string for key: {0}", key);
			return value;
		}
		
		
		public string GetStringFormat(string key, params object[] args) {
			return GetStringFormat(key, key, Locale.Current, args);
		}
		
		public string GetStringFormat(string key, Language language, params object[] args) {
			return string.Format(GetString(key, key, language), args);
		}
		
		public string GetStringFormat(string key, Locale locale, params object[] args) {
			return string.Format(GetString(key, key, locale), args);
		}
		
		
		public string GetStringFormat(string key, string value, params object[] args) {
			return GetStringFormat(key, value, Locale.Current, args);
		}
		
		public string GetStringFormat(string key, string value, Language language, params object[] args) {
			return string.Format(GetString(key, value, language), args);
		}
		
		public string GetStringFormat(string key, string value, Locale locale, params object[] args) {
			return string.Format(GetString(key, value, locale), args);
		}
		
		
		public string GetPluralString(string key, int count) {
			return GetPluralString(key, key, count, Locale.Current);
		}
		
		public string GetPluralString(string key, int count, Language language) {
			return GetPluralString(key, key, count, language);
		}
		
		public string GetPluralString(string key, int count, Locale locale) {
			return GetPluralString(key, key, count, locale);
		}
		
		
		public string GetPluralString(string key, string value, int count) {
			return GetPluralString(key, value, count, Locale.Current);
		}
		
		public string GetPluralString(string key, string value, int count, Language language) {
			
			LocalizedString localizedString;
			localizedString = GetLocalizedString(key, language);
			
			if (!localizedString.Equals(default(LocalizedString))) {
				
				string pluralString;
				pluralString = localizedString.GetPluralString(count, LocalizedString.GetPluralizationRule(language));
				
				if (!string.IsNullOrEmpty(pluralString)) {
					return pluralString;
				}
				
			}
			
			Debug.LogWarningFormat("Could not get plural string for key: {0}", key);
			return value;
			
		}
		
		public string GetPluralString(string key, string value, int count, Locale locale) {
			for (int index = 0; index < locale.PreferredLanguages.Length; index++) {
				
				Language language;
				language = locale.PreferredLanguages[index];
				
				LocalizedString localizedString;
				localizedString = GetLocalizedString(key, language);
				
				if (!localizedString.Equals(default(LocalizedString))) {
					
					string pluralString;
					pluralString = localizedString.GetPluralString(count, LocalizedString.GetPluralizationRule(language));
					
					if (!string.IsNullOrEmpty(pluralString)) {
						return pluralString;
					}
					
				}
			}
			
			Debug.LogWarningFormat("Could not get plural string for key: {0}", key);
			return value;
			
		}
		
		
		public string GetPluralStringFormat(string key, int count, params object[] args) {
			return GetPluralStringFormat(key, key, count, Locale.Current, args);
		}
		
		public string GetPluralStringFormat(string key, int count, Language language, params object[] args) {
			return string.Format(GetPluralString(key, key, count, language), args);
		}
		
		public string GetPluralStringFormat(string key, int count, Locale locale, params object[] args) {
			return string.Format(GetPluralString(key, key, count, locale), args);
		}
		
		
		public string GetPluralStringFormat(string key, string value, int count, params object[] args) {
			return GetPluralStringFormat(key, value, count, Locale.Current, args);
		}
		
		public string GetPluralStringFormat(string key, string value, int count, Language language, params object[] args) {
			return string.Format(GetPluralString(key, value, count, language), args);
		}
		
		public string GetPluralStringFormat(string key, string value, int count, Locale locale, params object[] args) {
			return string.Format(GetPluralString(key, value, count, locale), args);
		}
		
		#endregion
		
		
		#region Methods
		
		private LocalizedString GetLocalizedString(string key, Language language) {
			
			if (m_LanguageDictionary == null) {
				m_LanguageDictionary = new Dictionary<string,LocalizedStringDictionary>();
			}
			
			LocalizedStringDictionary localizedStringDictionary = null;
			if (!m_LanguageDictionary.TryGetValue(language.Identifier, out localizedStringDictionary)) {
				localizedStringDictionary = LoadLocalizedStringDictionary(language);
				m_LanguageDictionary.Add(language.Identifier, localizedStringDictionary);
			}
			
			LocalizedString localizedString = default(LocalizedString);
			localizedStringDictionary.TryGetValue(key, out localizedString);
			return localizedString;
			
		}
		
		private LocalizedStringDictionary LoadLocalizedStringDictionary(Language language) {
			var resourceReference = GetResourceReference(language);
			if (resourceReference) {
				try {
					var request = ResourceReferenceLoader.Load<TextAsset>(resourceReference);
					// TODO: requires sync load
					var asset = request.asset as TextAsset;
					var json = JsonUtility.FromJson<LocalizedStringDictionary.Json>(asset.text);
					var dictionary = new LocalizedStringDictionary(json);
					return dictionary;
				} catch {
					// Debug.LogWarningFormat("Could not load dictionary: {0}", path);
				}
			}
			return new LocalizedStringDictionary();
		}
		
		#endregion
		
	}
	
}