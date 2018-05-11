namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;

	public class MockContentDownloader : AbstractContentDownloader {

		#region Serialized Fields

		public ContentDownloadError m_MockError;
		public float m_MockQueuedTime = 2.0f;
		public float m_MockDownloadTime = 2.0f;

		#endregion

		#region Singleton

		private static MockContentDownloader _Instance;

		public static MockContentDownloader Instance {
			get {

#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					return null;
				}
#endif

				if (!_Instance) {
					_Instance = new GameObject("MockContentDownloader").AddComponent<MockContentDownloader>();
					DontDestroyOnLoad(_Instance);
				}

				return _Instance;
			}
		}

		#endregion

		#region AbstractContentDownloader Implementation

		protected override AbstractDownloaderYieldInstruction CreateDownloadYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName) {
			return new MockDownloadYieldInstruction(this, m_MockDownloadTime, m_MockError);
		}

		protected override AbstractDownloaderYieldInstruction CreateRefreshYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName) {
			return new MockRefreshYieldInstruction(this, m_MockQueuedTime);
		}

		#endregion
	}
}