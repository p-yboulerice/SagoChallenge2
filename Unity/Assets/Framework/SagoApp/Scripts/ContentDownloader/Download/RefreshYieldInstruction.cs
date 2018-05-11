namespace SagoApp.ContentDownloader {
	
	using UnityEngine;
	using System.Collections;
	using SagoApp.Project;
	using SagoCore.AssetBundles;

	public class RefreshYieldInstruction : AbstractDownloaderYieldInstruction {

		#region Constructor

		public RefreshYieldInstruction(MonoBehaviour monoBehaviour, string resourceAssetBundleName, string sceneAssetBundleName) : base(monoBehaviour) {
			if (monoBehaviour != null) {
				m_MonoBehaviour = monoBehaviour;
				m_Coroutine = RefreshDownload(resourceAssetBundleName, sceneAssetBundleName);
				m_MonoBehaviour.StartCoroutine(m_Coroutine);
			} else {
				m_IsDone = true;
				Debug.LogError("ContentDownloader-RefreshYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion

		#region Coroutines

		private IEnumerator RefreshDownload(string resourceAssetBundleName, string sceneAssetBundleName) {
			#if !UNITY_EDITOR
				AssetBundleAdaptorType resourceContentAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(resourceAssetBundleName);
				AssetBundleAdaptorType sceneContentAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(sceneAssetBundleName);

				if (resourceContentAssetBundleAdaptorType.IsRemote() || sceneContentAssetBundleAdaptorType.IsRemote()) {
					// Query to see if resources that we need to load are already downloaded and available
					using (var resourceQueryYieldInstruction = new ResourceQueryYieldInstruction(m_MonoBehaviour, resourceAssetBundleName, sceneAssetBundleName)) {
						Debug.Log("ContentDownloader-> Querying For Content Resource Availability.", DebugContext.SagoApp);
						yield return resourceQueryYieldInstruction;

						// Update state if the resource is already downloaded or not
						if (resourceQueryYieldInstruction.IsResourceAvailable) {
							m_DownloadState = ContentDownloadState.Complete;
						} else {
							m_DownloadState = ContentDownloadState.Unknown;
						}
					}
					Complete();
					yield break;
				} else {
					m_DownloadState = ContentDownloadState.Complete;
					Complete();
					yield break;
				}
			#else
				m_DownloadState = ContentDownloadState.Complete;
				Complete();
				yield break;
			#endif
		}

		#endregion
		
	}
}