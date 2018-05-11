namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;
	using SagoApp.Project;
	using SagoCore.AssetBundles;

	public class ContentDownloader : AbstractContentDownloader {

		#region Singleton

		private static ContentDownloader _Instance;

		public static ContentDownloader Instance {
			get {

				#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					return null;
				}
				#endif

				if (!_Instance) {
					_Instance = new GameObject("ContentDownloader").AddComponent<ContentDownloader> ();
					DontDestroyOnLoad (_Instance);
				}

				return _Instance;
			}
		}

		#endregion

		#region AbstractContentDownloader Implementation

		protected override AbstractDownloaderYieldInstruction CreateDownloadYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName) {
			return new DownloadYieldInstruction(this, resourceAssetBundleName, sceneAssetBundleName);
		}

		protected override AbstractDownloaderYieldInstruction CreateRefreshYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName) {
			return new RefreshYieldInstruction(this, resourceAssetBundleName, sceneAssetBundleName);
		}

		#endregion
	}

}
