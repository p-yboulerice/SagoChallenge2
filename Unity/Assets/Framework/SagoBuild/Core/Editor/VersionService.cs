namespace SagoBuildEditor.Core {
	
	using SagoPlatform;
	using System.Collections;
	using System.Net;
	using System.Text;
	using UnityEditor;
	using UnityEngine;
	
	[System.Serializable]
	public struct VersionServiceRequestJson {
		
	}
	
	[System.Serializable]
	public struct VersionServiceResponseJson {
		
		[SerializeField]
		public string appId;
		
		[SerializeField]
		public int code;
		
		[SerializeField]
		public string error;
		
		[SerializeField]
		public string platformId;
		
	}
	
	public enum VersionServiceServerType {
		Development,
		Staging,
		Production
	}
	
	public static class VersionService {
		
		
		#region Constants
		
		private const string ServerTypeKey = "SagoAppEditor.Workflow.VersionService.ServerType";
		
		#endregion
		
		
		#region Static Methods
		
		private static string GetApiToken(VersionServiceServerType serverType) {
			switch (serverType) {
				case VersionServiceServerType.Production:
					return "gQBcseHjiZa3ANRuqu6DqT7Wehw2WJho";
				case VersionServiceServerType.Staging: 
					return "KNt8PuDnwjUpBVFqeGk8cy2eLNUN8mTY";
				default: 
					return "cu8c42YzjioedUPpwKHcoTfvbrCBfb2J";
			}
		}
		
		private static string GetServerUrl(VersionServiceServerType serverType) {
			switch (serverType) {
				case VersionServiceServerType.Production:
					return "http://version-service.sagosago.com";
				case VersionServiceServerType.Staging: 
					return "http://version-service-staging.sagosago.com";
				default: 
					return "http://localhost:8000";
			}
		}
		
		private static string GetVersionUrl(VersionServiceServerType serverType, string appId, string platformId) {
			return string.Format(
				"{0}/api/versions/{1}/{2}", 
				GetServerUrl(serverType), 
				appId, 
				platformId
			);
		}
		
		#endregion
		
		
		#region Static Properties
		
		public static string ApiToken {
			get { return GetApiToken(ServerType); }
		}
		
		public static VersionServiceServerType ServerType {
			get {
				#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
					return VersionServiceServerType.Production;
				#else
					return (VersionServiceServerType)EditorPrefs.GetInt(ServerTypeKey, (int)VersionServiceServerType.Production);
				#endif
			}
			set {
				#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
					// noop
				#else
					EditorPrefs.SetInt(ServerTypeKey, (int)value);
				#endif
			}
		}
		
		public static string ServerUrl {
			get { return GetServerUrl(ServerType); }
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static int Bump(string appId, string platformId) {
			return Bump(appId, platformId, ServerType);
		}
		
		public static int Bump(string appId, string platformId, VersionServiceServerType serverType) {
			try {
			
				var request = (HttpWebRequest)HttpWebRequest.Create(GetVersionUrl(serverType, appId, platformId));
				request.Headers["Api-Token"] = GetApiToken(serverType);
				request.Method = "POST";
			
				var content = new UTF8Encoding().GetBytes(string.Empty);
				request.ContentLength = content.LongLength;
				request.ContentType = "application/json";
				request.GetRequestStream().Write(content, 0, content.Length);
				
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
					using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
						var responseText = reader.ReadToEnd();
						var json = JsonUtility.FromJson<VersionServiceResponseJson>(responseText);
						return json.code;
					}
				}
				
			} catch(System.Exception e) {
				Debug.LogWarning(e.ToString());
			} 
			return -1;
		}
		
		public static int Fetch(string appId, string platformId) {
			return Fetch(appId, platformId, ServerType);
		}
		
		public static int Fetch(string appId, string platformId, VersionServiceServerType serverType) {
			try {
				
				var request = (HttpWebRequest)HttpWebRequest.Create(GetVersionUrl(serverType, appId, platformId));
				request.Headers["Api-Token"] = GetApiToken(serverType);
				request.Method = "GET";
				
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
					using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
						var responseText = reader.ReadToEnd();
						var json = JsonUtility.FromJson<VersionServiceResponseJson>(responseText);
						return json.code;
					}
				}
				
			} catch(System.Exception e) {
				Debug.LogWarning(e.ToString());
			}
			return -1;
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static bool Bump(ProductInfo productInfo) {
			if (productInfo) {
				var build = VersionService.Bump(
					productInfo.Identifier, 
					productInfo.GetComponent<PlatformSettingsPrefab>().Platform.ToString()
				);
				if (build != -1) {
					productInfo.Build = build;
					EditorUtility.SetDirty(productInfo);
					return true;
				}
			}
			return false;
		}
		
		public static bool Fetch(ProductInfo productInfo) {
			if (productInfo) {
				var build = VersionService.Fetch(
					productInfo.Identifier, 
					productInfo.GetComponent<PlatformSettingsPrefab>().Platform.ToString()
				);
				if (build != -1) {
					productInfo.Build = build;
					EditorUtility.SetDirty(productInfo);
					return true;
				}
			}
			return false;
		}
		
		#endregion
		
		
	}
	
}