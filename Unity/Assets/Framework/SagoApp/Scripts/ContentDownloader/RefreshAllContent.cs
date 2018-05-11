namespace SagoApp.ContentDownloader {
	
	using UnityEngine;
	using System.Collections;
	using SagoApp.Project;
	using SagoApp.Content;

	class RefreshAllContent {
		[RuntimeInitializeOnLoadMethod]
		static void OnRefreshAllContent() {
			if (ContentDownloader.Instance != null) {
				//We delay the call to refresh all downloaded content in order to avoid an iOS crash (SW-381)
				//that seems to occur sometimes when we try to access the resource tags too early.
				ContentDownloader.Instance.StartCoroutine(RefreshAllContentAfterDelay(5.0f));
			}
		}

		private static IEnumerator RefreshAllContentAfterDelay(float delay) {
			yield return new WaitForSeconds(delay);
			
			if (ProjectInfo.Instance != null && ContentDownloader.Instance != null) {
				ContentInfo[] contentInfo = ProjectInfo.Instance.ContentInfo;
				ContentDownloader.Instance.RequestRefresh(ContentDownloader.Instance.GetContentDownloadItems(contentInfo));
			}
		}
	}
}