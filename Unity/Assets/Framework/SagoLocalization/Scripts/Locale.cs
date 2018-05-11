namespace SagoLocalization {
	
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEngine;
	
	internal struct Data {
		
		
		#region Static Methods
		
		public static string GetParentIdentifier(string identifier) {
			
			var data = new Data(identifier);
			
			// en-CA => en
			if (string.IsNullOrEmpty(data.ScriptCode) && !string.IsNullOrEmpty(data.CountryCode)) {
				return new Data(data.LanguageCode, null, null).Identifier;
			}
			
			// zh-Hant-HK => zh-Hant
			if (!string.IsNullOrEmpty(data.CountryCode) && !string.IsNullOrEmpty(data.ScriptCode)) {
				return new Data(data.LanguageCode, data.ScriptCode, null).Identifier;
			}
			
			return null;
			
		}
		
		public static bool IsValid(string identifier) {
			try {
				new Data(identifier); return true;
			} catch {
				return false;
			}
		}
		
		public static bool IsValid(string languageCode, string scriptCode, string countryCode) {
			try {
				new Data(languageCode, scriptCode, countryCode); return true;
			} catch {
				return false;
			}
		}
		
		#endregion
		
		
		#region Constructors
		
		public Data(string identifier) {
			
			if (string.IsNullOrEmpty(identifier)) {
				throw new System.ArgumentNullException("identifier");
			}
			
			Match match;
			match = Regex.Match(identifier, @"^([a-z]{2,3})\W?([A-Z][a-z]{2,})?\W?([A-Z]{2})?$");
			
			if (!match.Success) {
				throw new System.ArgumentException("Invalid format", "identifier");
			}
			
			string languageCode = (
				match.Groups[1].Captures[0].Value.ToString()
			);
			
			string scriptCode = (
				match.Groups[2].Captures.Count > 0 ? 
				match.Groups[2].Captures[0].Value.ToString() : 
				null
			);
			
			string countryCode = (
				match.Groups[3].Captures.Count > 0 ? 
				match.Groups[3].Captures[0].Value.ToString() : 
				null
			);
			
			Identifier = identifier;
			CountryCode = countryCode;
			LanguageCode = languageCode;
			ScriptCode = scriptCode;
			
		}
		
		public Data(string languageCode, string scriptCode, string countryCode) {
			
			if (string.IsNullOrEmpty(languageCode)) {
				throw new System.ArgumentNullException("languageCode");
			}
			
			System.Text.StringBuilder builder;
			builder = new System.Text.StringBuilder();
			builder.Append(languageCode);
			
			if (!string.IsNullOrEmpty(scriptCode)) {
				builder.Append("-");
				builder.Append(scriptCode);
			}
			
			if (!string.IsNullOrEmpty(countryCode)) {
				builder.Append("-");
				builder.Append(countryCode);
			}
			
			Identifier = builder.ToString();
			CountryCode = countryCode;
			LanguageCode = languageCode;
			ScriptCode = scriptCode;
			
		}
		
		#endregion
		
		
		#region Fields
		
		public string Identifier;
		
		public string CountryCode;
		
		public string LanguageCode;
		
		public string ScriptCode;
		
		#endregion
		
		
	}
	
	public struct Language {
		
		
		#region Constants
		
		public static readonly Language[] DefaultLanguages = new Language[] {
			new Language("de"),      // German
			new Language("en"),      // English
			new Language("fr"),      // French
			new Language("ja"),      // Japanese
			new Language("ru"),      // Russian
			new Language("zh-Hans"), // Chinese (Simplified)
			new Language("zh-Hant"), // Chinese (Traditional)
			new Language("es"),      // Spanish
			new Language("pt-BR"),   // Brazilian Portuguese
		};
		
		#endregion
		
		
		#region Static Methods
		
		public static string GetParentIdentifier(string identifier) {
			return Data.GetParentIdentifier(identifier);
		}
		
		public static bool IsValid(string identifier) {
			return Data.IsValid(identifier);
		}
		
		public static bool IsValid(string languageCode, string scriptCode, string countryCode) {
			return Data.IsValid(languageCode, scriptCode, countryCode);
		}
		
		#endregion
		
		
		#region Constructors
		
		internal Language(Data data) {
			m_Data = data;
		}
		
		public Language(string identifier) : this(new Data(identifier)) {
			
		}
		
		public Language(string languageCode, string scriptCode, string countryCode) : this(new Data(languageCode, scriptCode, countryCode)) {
			
		}
		
		#endregion
		
		
		#region Fields
		
		private Data m_Data;
		
		#endregion
		
		
		#region Properties
		
		public string Identifier {
			get { return m_Data.Identifier; }
		}
		
		public string CountryCode {
			get { return m_Data.CountryCode; }
		}
		
		public string LanguageCode {
			get { return m_Data.LanguageCode; }
		}
		
		public string ScriptCode {
			get { return m_Data.ScriptCode; }
		}
		
		#endregion
		
		
	}
	
	public struct Locale {
		
		
		#region Static Properties
		
		private static Locale _Current;
		
		public static Locale Current {
			get {
				if (_Current.Equals(default(Locale))) {
					_Current = LocaleProvider.GetLocale();
				}
				return _Current;
			}
			set {
				if (!_Current.Equals(value)) {
					Locale oldValue = _Current;
					_Current = value;
					if (OnLocaleDidChange != null) {
						OnLocaleDidChange(oldValue, _Current);
					}
				}
			}
		}

		/// <summary>
		/// Occurs when the locale changes.  Passes old Locale, and new Locale.
		/// </summary>
		public static event System.Action<Locale, Locale> OnLocaleDidChange;

		#endregion
		
		
		#region Static Methods
		
		public static string GetParentIdentifier(string identifier) {
			return Data.GetParentIdentifier(identifier);
		}
		
		public static bool IsValid(string identifier) {
			return Data.IsValid(identifier);
		}
		
		public static bool IsValid(string languageCode, string scriptCode, string countryCode) {
			return Data.IsValid(languageCode, scriptCode, countryCode);
		}
		
		private static Language[] NormalizePreferredLanguages(Data data, Language[] preferredLanguages) {
			
			var identifiers = new List<string>();
			
			if (preferredLanguages != null && preferredLanguages.Length != 0) {
				identifiers.AddRange(preferredLanguages.Select(language => language.Identifier));
			} else {
				identifiers.Add(data.Identifier);
			}
			
			for (int index = 0; index < identifiers.Count; index++) {
				var parent = GetParentIdentifier(identifiers[index]);
				while (!string.IsNullOrEmpty(parent) && !identifiers.Contains(parent)) {
					index++;
					identifiers.Insert(index, parent);
					parent = GetParentIdentifier(parent);
				}
			}
			
			identifiers.Add("en");
			
			return identifiers.Distinct().Select(identifier => new Language(identifier)).ToArray();
			
		}
		
		
		#endregion
		
		
		#region Constructors
		
		internal Locale(Data data, Language[] preferredLanguages) {
			m_Data = data;
			m_PreferredLanguages = NormalizePreferredLanguages(m_Data, preferredLanguages);
		}
		
		public Locale(string identifier) : this(new Data(identifier), null) {
			
		}
		
		public Locale(string identifier, Language[] preferredLanguages) : this(new Data(identifier), preferredLanguages) {
			
		}
		
		public Locale(string languageCode, string scriptCode, string countryCode) : this(new Data(languageCode, scriptCode, countryCode), null) {
			
		}
		
		public Locale(string languageCode, string scriptCode, string countryCode, Language[] preferredLanguages) : this(new Data(languageCode, scriptCode, countryCode), preferredLanguages) {
			
		}
		
		#endregion
		
		
		#region Fields
		
		private Data m_Data;
		
		private Language[] m_PreferredLanguages;
		
		#endregion
		
		
		#region Properties
		
		public string Identifier {
			get { return m_Data.Identifier; }
		}
		
		public string CountryCode {
			get { return m_Data.CountryCode; }
		}
		
		public string LanguageCode {
			get { return m_Data.LanguageCode; }
		}
		
		/// <summary>
		/// Gets the array of preferred languages. The first language in the array is the most-preferred, the last element is the least-preferred.
		/// </summary>
		public Language[] PreferredLanguages {
			get { return m_PreferredLanguages; }
		}
		
		public string ScriptCode {
			get { return m_Data.ScriptCode; }
		}
		
		#endregion
		
		
	}
	
}
