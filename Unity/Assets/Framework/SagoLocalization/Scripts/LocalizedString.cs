namespace SagoLocalization {
	
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEngine;
	
	[System.Serializable]
	public struct LocalizedString {
		
		
		#region Types
		
		[System.Serializable]
		public struct Json {
			
			
			#region Fields
			
			[SerializeField]
			public string reference;
			
			[SerializeField]
			public string comment;
			
			[SerializeField]
			public string key;
			
			[SerializeField]
			public bool plural;
			
			[SerializeField]
			public string[] values;
			
			#endregion
			
			
		}
		
		public delegate int PluralizationRule(float count);
		
		#endregion
		
		
		#region Static Properties

		/// If set, GetString*() will return key (no effect unless SAGO_DEBUG set)
		public static bool LocalizeToKeys = false;

		private static Dictionary<string,int> _PluralCounts;
		
		private static Dictionary<string,int> PluralCounts {
			get { return _PluralCounts = _PluralCounts ?? new Dictionary<string,int>(); }
		}
		
		private static Dictionary<string,PluralizationRule> _PluralizationRules;
		
		private static Dictionary<string,PluralizationRule> PluralizationRules {
			get { return _PluralizationRules = _PluralizationRules ?? new Dictionary<string,PluralizationRule>(); }
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static bool CountEndsWith(float count, params int[] values) {
			if (values != null) {
				
				string temp;
				temp = count.ToString();
				temp = temp[temp.Length - 1].ToString();
				
				int last;
				last = -1;
				
				for (int index = 0; index < values.Length; index++) {
					if (int.TryParse(temp, out last) && CountEquals(last, values[index])) {
						return true;
					}
				}
				
			}
			return false;
		}
		
		public static bool CountEquals(float count, params int[] values) {
			if (values != null) {
				float threshold = 0.001f;
				for (int index = 0; index < values.Length; index++) {
					if (Mathf.Abs(values[index] - count) < threshold) {
						return true;
					}
				}
			}
			return false;
		}
		
		public static int GetDefaultPluralCount(Language language) {
			if (Regex.IsMatch(language.Identifier, @"^(en|da|de|nl|no|se|fi|it|es|pt-PT|fr|pt-BR)")) {
				return 2;
			} else if (Regex.IsMatch(language.Identifier, @"^(ru)")) {
				return 3;
			} else {
				return 1;
			}
		}
		
		/// <summary>
		/// Gets the default pluralization rule for the specified language.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default pluralization rules were implemented for the languages we support based on Mozilla's documentation:
		/// <see cref="https://developer.mozilla.org/en-US/docs/Mozilla/Localization/Localization_and_Plurals" />
		/// </para>
		/// <para>
		/// One thing that's not clear is how to handle decimal values for count (i.e. count = 1.23). In English, 
		/// anything other than exactly one is plural, but that may not be the case for other languages. Without 
		/// better info to go on, the default pluralization rules use the exactly equal logic for all languages. 
		/// </para>
		/// </remarks>
		public static PluralizationRule GetDefaultPluralizationRule(Language language) {
			if (Regex.IsMatch(language.Identifier, @"^(zh|ja|ko|th|tr)")) {
				// Chinese, Japanese, Korean, Thai, Turkish
				return (count) => { return 0; };
			} else if (Regex.IsMatch(language.Identifier, @"^(en|da|de|nl|no|se|fi|it|es|pt-PT)")) {
				// English, Danish, German, Dutch, Norwegian, Swedish, Finnish, Italian, Spanish, Portugese
				return (count) => { return CountEquals(count, 1) ? 0 : 1; };
			} else if (Regex.IsMatch(language.Identifier, @"^(fr|pt-BR)")) {
				// French, Brazilian Portugese
				return (count) => { return CountEquals(count, 0) || CountEquals(count, 1) ? 0 : 1; };
			} else if (Regex.IsMatch(language.Identifier, @"^(ru)")) {
				// Russian
				return (count) => {
					if (CountEndsWith(count, 1) && !CountEquals(count, 11)) {
						return 0;
					} else if (CountEndsWith(count, 2,3,4) && !CountEquals(count, 12, 13, 14)) {
						return 1;
					} else {
						return 2;
					}
				};
			} else if (Regex.IsMatch(language.Identifier, @"^id")) {
				// Indonesian
				// TODO: the mozilla docs do not have a pluralization rule for Indonesian
				Debug.LogWarningFormat("Could not get pluralization rule for language: {0}", language.Identifier);
				return (count) => { return -1; };
			} else {
				Debug.LogWarningFormat("Could not get pluralization rule for language: {0}", language.Identifier);
				return (count) => { return -1; };
			}
		}
		
		public static PluralizationRule GetPluralizationRule(Language language) {
			if (!PluralizationRules.ContainsKey(language.Identifier)) {
				PluralizationRules.Add(language.Identifier, GetDefaultPluralizationRule(language));
			}
			return PluralizationRules[language.Identifier];
		}
		
		public static void SetPluralizationRule(Language language, PluralizationRule rule) {
			// TODO: Not implemented
			throw new System.InvalidOperationException("Not implemented");
		}
		
		public static int GetPluralCount(Language language) {
			if (!PluralCounts.ContainsKey(language.Identifier)) {
				PluralCounts.Add(language.Identifier, GetDefaultPluralCount(language));
			}
			return PluralCounts[language.Identifier];
		}
		
		public static void SetPluralCount(Language language, int value) {
			PluralCounts[language.Identifier] = value;
		}
		
		#endregion
		
		
		#region Constructors
		
		public LocalizedString(Json json) {
			Key = json.key;
			Values = json.values;
		}
		
		public LocalizedString(string key, string[] values) {
			Key = key;
			Values = values;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		public string Key;
		
		[SerializeField]
		public string[] Values;
		
		#endregion
		
		
		#region Methods
		
		public string GetString() {
			#if SAGO_DEBUG
			if (LocalizeToKeys) return Key;
			#endif
			return (
				Values != null && Values.Length != 0 && !string.IsNullOrEmpty(Values[0]) ? 
				Values[0] : 
				null
			);
		}
		
		public string GetPluralString(int count, PluralizationRule rule) {
			#if SAGO_DEBUG
			if (LocalizeToKeys) return Key;
			#endif
			if (Values != null && rule != null) {
				int index = rule(count);
				if (index >= 0 && index < Values.Length && !string.IsNullOrEmpty(Values[index])) {
					return Values[index];
				}
			}
			return null;
		}
		
		#endregion
		
		
	}
	
}