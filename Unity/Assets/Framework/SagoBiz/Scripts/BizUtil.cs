namespace SagoBiz {

	using SagoUtils;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// Biz util class that reads sago biz web client preferences.
	/// </summary>
	/// <see cref="PlayerPrefs"/>
	/// <remarks>TODO: The WebClientPreferences dictionary is currently used in <see cref="Service"/> LoadPromoDataAsync() upon performing promo request. 
	/// It should be removed once the server code starts using realtime access from native.</remarks>
	public class BizUtil {

		public static readonly string  playerPrefsWebClientPreferences = "sagoBizWebClientPreferences";

		public static Dictionary<string,string> WebClientPreferences {
			
			get {
				if (PlayerPrefs.HasKey(playerPrefsWebClientPreferences)) {
					Debug.Log("-> Found key " + playerPrefsWebClientPreferences); 
					string jsonData = string.Empty;
					try {
						string base64Data = PlayerPrefs.GetString(playerPrefsWebClientPreferences);
						Debug.Log("base64Data:\n" + base64Data);
						byte[] data = System.Convert.FromBase64String(base64Data);
#if !UNITY_WSA
                        jsonData = System.Text.Encoding.UTF8.GetString(data);
#else
                        jsonData = data.ToString();
#endif
                        Debug.Log("Retrieved json data -> WebClientPreferences:\nBase64: " + base64Data);
						Debug.Log("Retrieved json data -> WebClientPreferences:\njson: " + jsonData);
						Dictionary<string,string> result = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonData);
						Debug.Log("Number of items in player prefs for web client: " + result.Count);
						return result;
						
					} catch (System.Exception e) {
						Debug.LogError("-> Could not parse json data for WebClientPreferences:\n" + jsonData);
						Debug.LogException(e);
						return new Dictionary<string, string>();
					}
				} else {
					return new Dictionary<string, string>();
				}
			}
			
		}

	}

}
