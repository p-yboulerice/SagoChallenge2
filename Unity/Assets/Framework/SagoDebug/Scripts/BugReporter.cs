namespace SagoDebug {
	
	using UnityEngine;
	using UnityEngine.Networking;    
	using System.Collections;
	using SagoUtils;

	public class BugReporter : MonoBehaviour {

		#region Types

		/// <summary>
		/// Occurs when a new bug report is created.
		/// </summary>
		public event System.Action<BugReport> OnCreateBugReport;

		#endregion

		#region Singleton

		private static BugReporter _Instance;

		public static BugReporter Instance {
			get {

#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					return null;
				}
#endif

				if (!_Instance) {
					_Instance = new GameObject("BugReporter").AddComponent<BugReporter>();
					DontDestroyOnLoad(_Instance);
				}

				return _Instance;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:SagoDebug.BugReporter"/> is sending bug report.
		/// </summary>
		/// <value><c>true</c> if is sending bug report; otherwise, <c>false</c>.</value>
		public bool IsSendingBugReport {
			get {
				return this.CoroutineHelper.IsRunningCoroutine(CoKeyExport);
			}
		}

		/// <summary>
		/// Sends the bug report. Calls OnCreateBugReport before sending.
		/// </summary>
		public void SendBugReport() {
			if (!this.IsSendingBugReport) {

				BugReport bugReport = CreateBugReport();

				var json = bugReport.ToJsonString();

				string url = GetExportUrl();
				this.CoroutineHelper.StartCoroutine(CoKeyExport, ExportAsync(json, url));
			}
		}

		/// <summary>
		/// Creates a bug report object with device and bug data.
		/// </summary>
		/// <returns>A new instance of BugReport with device and bug data.</returns>
		public BugReport CreateBugReport() {

			BugReport bugReport = new BugReport();

			if (OnCreateBugReport != null) {
				OnCreateBugReport(bugReport);
			}

			return bugReport;
		}

		#endregion

		#region Internal Fields

		private const string CoKeyExport = "Export";

		[System.NonSerialized]
		private CoroutineHelper m_CoroutineHelper;

		#endregion

		#region Internal Properties

		private CoroutineHelper CoroutineHelper {
			get {
				return m_CoroutineHelper = m_CoroutineHelper ?? new CoroutineHelper(this);
			}
		}

		#endregion

		#region Internal Methods

		private static string GetExportUrl() {
			const string baseUrl = "https://world-api.sagosago.com";
			return string.Format(
				"{0}/api/1.0/support/uploadConsoleLog",
				baseUrl
			);
		}

		private IEnumerator ExportAsync(string data, string url) {

			using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)) {

				request.downloadHandler = new DownloadHandlerBuffer();
				request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(data));
				request.SetRequestHeader("Content-Type", "application/json");

				UnityEngine.Debug.LogFormat(this, "[Export] Starting upload to {0}...", url);
				yield return request.SendWebRequest();

				if (request.isNetworkError) {
					UnityEngine.Debug.LogWarningFormat(this, "[Export] Upload failed: status = {0}", request.error);
					yield break;
				}

				try {
					var responseJson = JsonUtility.FromJson<Json.ExportUploadResponse>(request.downloadHandler.text);
					if (request.responseCode == 200 && responseJson.status == 1) {
						UnityEngine.Debug.LogFormat(this, "[Export] Upload complete");
					} else {
						if (string.IsNullOrEmpty(responseJson.errorTitle)) {
							UnityEngine.Debug.LogFormat(this, "[Export] Request failed: data = {0}", request.downloadHandler.text);
						} else {
							UnityEngine.Debug.LogFormat(this, "[Export] Request failed: error = {0}: {1}", responseJson.errorTitle, responseJson.errorDescription);
						}
					}
				} catch {
					UnityEngine.Debug.LogFormat(this, "[Export] Request failed: data = {0}", request.downloadHandler.text);
				}
			}
		}

		#endregion
	}

}