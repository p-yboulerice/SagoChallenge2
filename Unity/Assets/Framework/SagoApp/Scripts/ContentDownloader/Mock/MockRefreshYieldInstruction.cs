namespace SagoApp.ContentDownloader {
	
	using UnityEngine;
	using System.Collections;
	using SagoApp.Project;
	using SagoCore.AssetBundles;

	public class MockRefreshYieldInstruction : AbstractDownloaderYieldInstruction {

		#region Constructor

		public MockRefreshYieldInstruction(MonoBehaviour monoBehaviour, float mockRefreshTime) : base(monoBehaviour) {
			if (monoBehaviour != null) {
				m_MonoBehaviour = monoBehaviour;
				m_Coroutine = RefreshDownload(mockRefreshTime);
				m_MonoBehaviour.StartCoroutine(m_Coroutine);
			} else {
				m_IsDone = true;
				Debug.LogError("ContentDownloader-MockRefreshYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion

		#region Coroutines

		private IEnumerator RefreshDownload(float mockRefreshTime) {
			yield return new WaitForSeconds(mockRefreshTime);
			m_DownloadState = ContentDownloadState.Unknown;
			Complete();
		}

		#endregion
		
	}
}