namespace SagoUtils
{
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public static class JsonConvert
	{


		#region Properties

		// set to true for debug output
		private static bool EnableLogging = false;

		#if !UNITY_WSA
		
		private static Newtonsoft.Json.JsonSerializerSettings _CSharpToJsonSettings;
		
		private static Newtonsoft.Json.JsonSerializerSettings _JsonToCSharpSettings;

		
		public static Newtonsoft.Json.JsonSerializerSettings CSharpToJsonSettings {
			get { 
				if (_CSharpToJsonSettings == null) {
					_CSharpToJsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
					_CSharpToJsonSettings.ContractResolver = new UnderscoreLowerCasePropertyNameContractResolver();
				}
				return _CSharpToJsonSettings;
			}
		}

		public static Newtonsoft.Json.JsonSerializerSettings JsonToCSharpSettings {
			get { 
				if (_JsonToCSharpSettings == null) {
					_JsonToCSharpSettings = new Newtonsoft.Json.JsonSerializerSettings();
					_JsonToCSharpSettings.ContractResolver = new UnderscoreLowerCasePropertyNameContractResolver();
				}
				return _JsonToCSharpSettings;
			}
		}

		
		private static Newtonsoft.Json.Formatting MapFormatting(Formatting type)
		{
			switch (type) {
			case Formatting.Indented:
				return Newtonsoft.Json.Formatting.Indented;
			}
			return Newtonsoft.Json.Formatting.None;
		}

		private static Newtonsoft.Json.JsonSerializerSettings MapSettings(Settings type)
		{
			switch (type) {
			case Settings.CSharpToJson:
				return CSharpToJsonSettings;
			case Settings.JsonToCSharp:
				return JsonToCSharpSettings;
			}
			return null;
		}
		
		#endif
		#endregion


		#region Types

		public enum Formatting
		{
			None,
			Indented
		}

		public enum Settings
		{
			None,
			CSharpToJson,
			JsonToCSharp
		}

		#endregion

		
		#region Methods

		public static string SerializeObject(object value)
		{
			return SerializeObject(value, Formatting.None, Settings.None);
		}

		public static string SerializeObject(object value, Formatting formatting)
		{
			return SerializeObject(value, formatting, Settings.None);
		}

		public static string SerializeObject(object value, Formatting formatting, Settings settings)
		{
			#if UNITY_WSA
	            string result = "";

				if (EnableLogging) {
	                Debug.LogWarning("SerializeObject not fully supported on the Windows Store platform.");
				}

	            if (value is IDictionary) {
	                // we can support <string, object> fairly well. Other types of IDictionary are less reliable
	                if (value is Dictionary<string, object>)
	                {
	                    if (EnableLogging)
	                        Debug.Log("SerializeObject using experimental Dictionary<string, object> support.");
	                    
	                    result = UnwrapDictionary((Dictionary<string,object>)value);

	                } else {

	                    if (EnableLogging)
	                       Debug.LogWarning("SerializeObject passed object of type IDictionary that is not Dictionary<string,object>. Likely will be empty Json");

	                    result = JsonUtility.ToJson(value);
	                }

	            } else {
	                // The built-in Unity JsonUtility can handle most basic class types well
	                // provided all desired fields are public or serialized
	                Debug.LogWarning("SerializeObject using JsonUtility from unknown object type. Checking output recommended.");

	                result = JsonUtility.ToJson(value);

	                Debug.Log(result);
	            }

                return result;
			#else
			return Newtonsoft.Json.JsonConvert.SerializeObject(value, MapFormatting(formatting), MapSettings(settings));
			#endif
		}

		public static T DeserializeObject<T>(string value)
		{
			return DeserializeObject<T>(value, Settings.None);
		}

		public static T DeserializeObject<T>(string value, Settings settings)
		{
			#if UNITY_WSA
				Debug.LogWarning("DeserializeObject is not fully supported on the Windows Store platform.");
                T result = JsonUtility.FromJson<T>(value);

				return result;
			#else
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, MapSettings(settings));
			#endif
		}

		private static string UnwrapDictionary(Dictionary<string,object> dict)
		{
			string propsString = "{\n";

			foreach (var kvp in dict) {
				if (kvp.Value is string) {
					propsString += "\"" + kvp.Key + "\": ";
					propsString += "\"" + kvp.Value + "\",\n";
				} else if (kvp.Value is float || kvp.Value is double || kvp.Value is int || kvp.Value is bool) {
					propsString += "\"" + kvp.Key + "\": ";
					propsString += kvp.Value + ",\n";
				} else if (kvp.Value is Dictionary<string, object>) {
					propsString += "\"" + kvp.Key + "\": ";
					propsString += UnwrapDictionary((Dictionary<string,object>)kvp.Value) + ",\n";
				} else {
					if (EnableLogging)
						Debug.Log("JsonConvert: Unsupported value type.\nKey: " + kvp.Key + ", Val: " + kvp.Value);
				}

			}
			propsString = propsString.Substring(0, propsString.Length - 2);
			propsString += "\n}";

			if (EnableLogging)
				Debug.Log(propsString);

			return propsString;
		}

		#endregion
		
		
	}
	
}


namespace SagoUtils
{
	
	using System.Collections;
	using System.Globalization;
	using UnityEngine;

	public static class JsonConvertStringExtensions
	{
		
		public static string SingleWordToPascalCase(this string word)
		{
#if UNITY_WSA
            string result = char.ToUpper(word[0]).ToString();
#else
			string result = char.ToUpper(word[0], CultureInfo.InvariantCulture).ToString((System.IFormatProvider)CultureInfo.InvariantCulture);
#endif
                   
			if (word.Length > 1) {
				result = result + word.Substring(1);
			}
			
			return result;
		}

		public static string UnderscoreWordsToPascalCase(this string underscore_lowercase_words)
		{
			string[] words = underscore_lowercase_words.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);
			string result = string.Empty;
			
			foreach (string word in words) {
				result += word.SingleWordToPascalCase();
			}
			
			return result;
		}

		public static string PascalToUnderscoreCase(this string words)
		{
			string result = string.Empty;
			int count = 0;
			while (count < words.Length) {
				
				if (count == 0) {
					result += char.ToLower(words[count]);
				} else if (char.IsUpper(words[count])) {
					result += "_";
					result += char.ToLower(words[count]);
				} else {
					result += char.ToLower(words[count]);
				}
				count++;
			}
			
			return result;
		}
		
	}
	
}


#if !UNITY_WSA
namespace SagoUtils
{
	
	using System.Collections;
	using System.Globalization;
	using UnityEngine;
	using Newtonsoft.Json.Serialization;

	public class PascalCasePropertyNameContractResolver : DefaultContractResolver
	{
		
		public PascalCasePropertyNameContractResolver() : 
#pragma warning disable 612, 618
		base(true) 
#pragma warning restore 612, 618
		{
		}

		/// <summary>
		/// Resolves the name of the property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>The property name camel cased.</returns>
		protected internal override string ResolvePropertyName(string propertyName)
		{
			return propertyName.UnderscoreWordsToPascalCase();
		}
	
	}
	
}
#endif


#if !UNITY_WSA
namespace SagoUtils
{
	
	using System.Collections;
	using System.Globalization;
	using UnityEngine;
	using Newtonsoft.Json.Serialization;

	public class UnderscoreLowerCasePropertyNameContractResolver : DefaultContractResolver
	{
		
		public UnderscoreLowerCasePropertyNameContractResolver() : 
#pragma warning disable 612, 618
		base(true) 
#pragma warning restore 612, 618
		{
		}

		/// <summary>
		/// Resolves the name of the property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>The property name camel cased.</returns>
		protected internal override string ResolvePropertyName(string propertyName)
		{
			return propertyName.PascalToUnderscoreCase();
		}
		
	}
	
}
#endif
