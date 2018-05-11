namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;
	using SagoCore.AssetBundles;
	using SagoApp.Project;

	public class DownloadYieldInstruction : AbstractDownloaderYieldInstruction {

		#region Constructor

		public DownloadYieldInstruction(MonoBehaviour monoBehaviour, string resourceAssetBundleName, string sceneAssetBundleName) : base(monoBehaviour) {
			if (monoBehaviour != null) {
				m_MonoBehaviour = monoBehaviour;
				m_Coroutine = Download(resourceAssetBundleName, sceneAssetBundleName);
				m_MonoBehaviour.StartCoroutine(m_Coroutine);
			} else {
				m_IsDone = true;
				Debug.LogError("ContentDownloader-DownloadYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion

		#region Coroutines

		private IEnumerator Download(string resourceAssetBundleName, string sceneAssetBundleName) {
			if (AssetBundleOptions.UseAssetBundlesInEditor) {
				
				this.ProgressReport.Reset();

				Debug.LogFormat(DebugContext.SagoApp, "ContentDownloader-> Finding or creating reference to asset bundle: {0}", resourceAssetBundleName);
				this.ResourceAssetBundle = DownloadAssetBundleReference.FindOrCreateReference(resourceAssetBundleName);
				this.ResourceAssetBundle.Retain();

				this.ProgressReport.Index = 0;
				this.ProgressReport.Count = 2;
				this.ProgressReport.Item = new LoadAssetBundleProgressReportItem(this.ResourceAssetBundle);
				yield return this.ResourceAssetBundle;

				Debug.LogFormat(DebugContext.SagoApp, "ContentDownloader-> Completed finding or creating reference to asset bundle: {0}", resourceAssetBundleName);
				if (!string.IsNullOrEmpty(this.ResourceAssetBundle.error)) {
					Debug.LogError(this.ResourceAssetBundle.error, DebugContext.SagoApp);

					m_DownloadError = GetContentDownloadErrorFromString(this.ResourceAssetBundle.error);
					m_DownloadErrorMessage = this.ResourceAssetBundle.error;

					Complete();
					yield break;
				}

				Debug.LogFormat(DebugContext.SagoApp, "ContentDownloader-> Finding or creating reference to asset bundle: {0}", sceneAssetBundleName);
				this.SceneAssetBundle = DownloadAssetBundleReference.FindOrCreateReference(sceneAssetBundleName);
				this.SceneAssetBundle.Retain();

				this.ProgressReport.Index = 1;
				this.ProgressReport.Item = new LoadAssetBundleProgressReportItem(this.SceneAssetBundle);
				yield return this.SceneAssetBundle;

				Debug.LogFormat(DebugContext.SagoApp, "ContentDownloader-> Completed finding or creating reference to asset bundle: {0}", sceneAssetBundleName);
				if (!string.IsNullOrEmpty(this.SceneAssetBundle.error)) {
					Debug.LogError(this.SceneAssetBundle.error, DebugContext.SagoApp);

					m_DownloadError = GetContentDownloadErrorFromString(this.SceneAssetBundle.error);
					m_DownloadErrorMessage = this.SceneAssetBundle.error;

					Complete();
					yield break;
				}

			}

			Complete();
		}

		#endregion

		#region Private Methods

		protected ContentDownloadError GetContentDownloadErrorFromString(string errorString) {
			ContentDownloadError error;

			if (string.Equals(errorString, LowDiskSpaceAssetBundleAdaptor.LowDiskSpaceError)) {
				error = ContentDownloadError.RunOutOfDiskSpace;
			} else if (string.Equals(errorString, AssetBundleAdaptorError.NoInternet) || !AbstractContentDownloader.IsInternetAvailable()) {
				//If unknown error was returned and internet is not reachable, we assume the error is due to internet reachability
				if (AbstractContentDownloader.DownloadViaWiFiOnly) {
					error = ContentDownloadError.LostWifi;
				} else {
					error = ContentDownloadError.LostInternet;
				}
			} else {
				error = ContentDownloadError.UnknownError;
			}

			return error;
		}

		#endregion
	}
}
