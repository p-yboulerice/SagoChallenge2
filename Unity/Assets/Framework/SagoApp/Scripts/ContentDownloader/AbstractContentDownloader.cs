namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoUtils;
	using SagoApp.Content;
	using SagoCore.AssetBundles;
	using SagoApp.Project;

	public abstract class AbstractContentDownloader : MonoBehaviour {

		#region Events

		/// <summary>
		/// Fired when a global downloader error occurs.
		/// ContentDownloadItem can be null if the error occured when no particular item was downloading. 
		/// </summary>
		public event System.Action<ContentDownloadError, string, ContentDownloadItem> OnDownloaderDownloadError;

		/// <summary>
		/// Fired when a content download was canceled.
		/// </summary>
		public event System.Action<ContentDownloadItem> OnDownloaderDownloadCancel;

		/// <summary>
		/// Fired when a content download was started.
		/// </summary>
		public event System.Action<ContentDownloadItem> OnDownloaderDownloadStart;

		/// <summary>
		/// Fired when a content download was complete. Complete is still called after an error or a cancel.
		/// Second parameter is set to true if download was finished without a cancel/error and false otherwise.
		/// </summary>
		public event System.Action<ContentDownloadItem, bool> OnDownloaderDownloadFinish;

		#endregion

		#region Fields

		public const string NoInternetErrorMessage = "Internet is not reachable.";
		public const string LostInternetErrorMessage = AssetBundleAdaptorError.NoInternet;

		//ToDo: Use the key from here and change it in settings. (When cleaning out ProjectNavigator code)
		private const string DownloadViaWiFiOnlyKey = "SagoApp.Project.ProjectNavigator.DownloadViaWiFiOnly";

		private const float InternetConnectionCheckDelay = 0.5f;

		[System.NonSerialized]
		private DownloadQueue<ContentDownloadItem> m_DownloadQueue;

		private Dictionary<ContentInfo, ContentDownloadItem> m_DownloadItemsByContentInfo;

		/// <summary>
		/// Global downloader error
		/// </summary>
		protected ContentDownloadError m_DownloadError;

		#endregion

		#region Properties

		public static bool DownloadViaWiFiOnly {
			get { return PlayerPrefs.GetInt(DownloadViaWiFiOnlyKey, 1) == 1; }
		}

		private DownloadQueue<ContentDownloadItem> DownloadQueue {
			get { return m_DownloadQueue = m_DownloadQueue ?? new DownloadQueue<ContentDownloadItem>(); }
		}

		private Dictionary<ContentInfo, ContentDownloadItem> DownloadItemsByContentInfo {
			get { return m_DownloadItemsByContentInfo = m_DownloadItemsByContentInfo ?? new Dictionary<ContentInfo, ContentDownloadItem>(); }
		}

		#endregion

		#region MonoBehaviour methods

		private void Start() {
			DownloadAssetBundleReference.CreateAdaptor = (string assetBundleName) => {
				return AssetBundleAdaptorMap.Instance.CreateAdaptor(assetBundleName, true);
			};

			StartCoroutine(DownloadAll());
		}

		private void OnDestroy() {
			foreach (ContentDownloadItem contentDownloadItem in DownloadItemsByContentInfo.Values) {
				contentDownloadItem.Dispose();
			}

			StopAllCoroutines();
		}

		#endregion

		#region Public API

		/// <summary>
		/// Returns the ContentDownloadItem for given ContentInfo.
		/// Creates it if none was created before.
		/// </summary>
		/// <returns>The content download item.</returns>
		/// <param name="contentInfo">Content info.</param>
		public ContentDownloadItem GetContentDownloadItem(ContentInfo contentInfo) {
			if (!this.DownloadItemsByContentInfo.ContainsKey(contentInfo)) {
				ContentDownloadItem item = new ContentDownloadItem();
				item.ContentInfo = contentInfo;
				item.ContentDownloader = this;
				this.DownloadItemsByContentInfo.Add(contentInfo, item);
			}

			return this.DownloadItemsByContentInfo[contentInfo];
		}

		/// <summary>
		/// Returns the array of ContentDownloadItem for given ContentInfo.
		/// Creates them if they were not created before.
		/// </summary>
		/// <returns>The content download items.</returns>
		/// <param name="contentInfo">Content info array.</param>
		public  ContentDownloadItem[] GetContentDownloadItems(ContentInfo[] contentInfo) {
			ContentDownloadItem[] contentDownloadItems = new ContentDownloadItem[contentInfo.Length];
			for (int i = 0; i < contentInfo.Length; i++) {
				contentDownloadItems[i] = GetContentDownloadItem(contentInfo[i]);
			}

			return contentDownloadItems;
		}
		#endregion

		#region Internal API

		internal void RequestDownload(ContentDownloadItem contentDownloadItem) {
			if (contentDownloadItem.State == ContentDownloadState.Unknown) {
				if (!IsInternetAvailable()) {
					if (DownloadViaWiFiOnly) {
						contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Error, ContentDownloadError.NoWifi);
					} else {
						contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Error, ContentDownloadError.NoInternet);
					}
				} else {
					this.DownloadQueue.AddLast(contentDownloadItem);
					contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Queued);
				}
			}
		}

		internal void CancelDownload(ContentDownloadItem contentDownloadItem, bool forceReset = false) {
			if (contentDownloadItem.State != ContentDownloadState.Complete || forceReset) {
				if (contentDownloadItem.DownloadYieldInstruction != null) {
					contentDownloadItem.DownloadYieldInstruction.Dispose();
					contentDownloadItem.DownloadYieldInstruction = null;
				}

				DownloadQueue.Remove(contentDownloadItem);
				contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Unknown);

				if (this.OnDownloaderDownloadCancel != null) {
					this.OnDownloaderDownloadCancel(contentDownloadItem);
				}
			}
		}

		internal void RequestRefresh(ContentDownloadItem[] contentDownloadItems) {
			StartCoroutine(RefreshContent(contentDownloadItems));
		}

		internal void RequestRefresh(ContentDownloadItem contentDownloadItem) {
			if (contentDownloadItem.State == ContentDownloadState.Complete || contentDownloadItem.State == ContentDownloadState.Unknown) {
				StartCoroutine(RefreshContent(contentDownloadItem));
			} else {
				contentDownloadItem.NotifyRefreshStateChange();
			}
		}

		internal void CancelRefresh(ContentDownloadItem contentDownloadItem) {
			if (contentDownloadItem.RefreshYieldInstruction != null) {
				contentDownloadItem.RefreshYieldInstruction.Dispose();
				contentDownloadItem.RefreshYieldInstruction = null;
			}

			if (contentDownloadItem.State == ContentDownloadState.Refreshing) {
				contentDownloadItem.State = ContentDownloadState.Unknown;
			}
		}

		internal static bool IsInternetAvailable() {
			if ((Application.internetReachability == NetworkReachability.NotReachable) ||
			    (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork && DownloadViaWiFiOnly)) {
				return false;
			}

			return true;
		}

		#endregion

		#region ContentDownloader Interface

		/// <summary>
		/// Factory method for a refresh yield instruction. Used to have a mock implementation of refresh.
		/// </summary>
		/// <returns>The refresh yield instruction.</returns>
		protected abstract AbstractDownloaderYieldInstruction CreateRefreshYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName);

		/// <summary>
		/// Factory method for a download yield instruction. Used to have a mock implementation of download.
		/// </summary>
		/// <returns>The download yield instruction.</returns>
		protected abstract AbstractDownloaderYieldInstruction CreateDownloadYieldInstruction(string resourceAssetBundleName, string sceneAssetBundleName);

		#endregion

		#region Coroutines

		private IEnumerator DownloadAll() {
			while (true) {
				if (!this.DownloadQueue.IsEmpty()) {
					//Check for internet connection before starting/restarting download
					//Used to return NoInternet/NoWifi error states
					m_DownloadError = ContentDownloadError.None;
					yield return StartCoroutine(WaitForInternetConnection(InternetConnectionCheckDelay));

					//Dequeue next download and starts it
					ContentDownloadItem contentDownloadItem;
					while ((contentDownloadItem = this.DownloadQueue.RemoveFirst()) != null) {
						m_DownloadError = ContentDownloadError.None;
						yield return StartCoroutine(DownloadContent(contentDownloadItem));

						if (m_DownloadError != ContentDownloadError.None) {
							if (m_DownloadError == ContentDownloadError.LostInternet || m_DownloadError == ContentDownloadError.LostWifi) {
								//LostInternet/LostWifi : Queue back this one and wait
								this.DownloadQueue.AddFirst(contentDownloadItem);
								contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Queued);

								yield return StartCoroutine(WaitForInternetConnection(InternetConnectionCheckDelay));
							} else if (m_DownloadError == ContentDownloadError.RunOutOfDiskSpace) {
								//LowDiskSpace : NotifyError
								NotifyAndDequeueAll(ContentDownloadState.Error, contentDownloadItem.Error);
							}
						}
					}
				}

				yield return null;
			}
		}

		private IEnumerator RefreshContent(ContentDownloadItem[] contentDownloadItems) {
			foreach (ContentDownloadItem contentDownloadItem in contentDownloadItems) {
				if (contentDownloadItem.State == ContentDownloadState.Complete || contentDownloadItem.State == ContentDownloadState.Unknown) {
					yield return StartCoroutine(RefreshContent(contentDownloadItem));
				} else {
					contentDownloadItem.NotifyRefreshStateChange();
				}
			}
		}

		private IEnumerator RefreshContent(ContentDownloadItem contentDownloadItem) {
			string resourceAssetBundleName = contentDownloadItem.ContentInfo.ResourceAssetBundleName;
			string sceneAssetBundleName = contentDownloadItem.ContentInfo.SceneAssetBundleName;

			if (contentDownloadItem.RefreshYieldInstruction != null) {
				contentDownloadItem.RefreshYieldInstruction.Dispose();
				contentDownloadItem.RefreshYieldInstruction = null;
			}

			using (AbstractDownloaderYieldInstruction refreshYieldInstruction = CreateRefreshYieldInstruction(resourceAssetBundleName, sceneAssetBundleName)) {
				contentDownloadItem.RefreshYieldInstruction = refreshYieldInstruction;
				contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Refreshing);

				yield return refreshYieldInstruction;

				if (!refreshYieldInstruction.IsDisposed) {
					contentDownloadItem.State = refreshYieldInstruction.DownloadState;
					contentDownloadItem.Error = ContentDownloadError.None;

					if (AssetBundleOptions.UseAssetBundlesInEditor) {
						if (contentDownloadItem.State == ContentDownloadState.Complete) {
							//Resource is already on disk, we retain it
							if (contentDownloadItem.ResourceAssetBundle == null) {
								contentDownloadItem.ResourceAssetBundle = DownloadAssetBundleReference.FindOrCreateReference(resourceAssetBundleName);
								contentDownloadItem.ResourceAssetBundle.Retain();
							}

							if (contentDownloadItem.SceneAssetBundle == null) {
								contentDownloadItem.SceneAssetBundle = DownloadAssetBundleReference.FindOrCreateReference(sceneAssetBundleName);
								contentDownloadItem.SceneAssetBundle.Retain();
							}
						} else if (contentDownloadItem.State == ContentDownloadState.Unknown) {
							//Resource is not on disk any more, we release it
							contentDownloadItem.ReleaseAssetBundles();
						}
					}

					refreshYieldInstruction.Dispose();
					contentDownloadItem.RefreshYieldInstruction = null;

					contentDownloadItem.NotifyRefreshStateChange();
				}
			}
		}

		private IEnumerator DownloadContent(ContentDownloadItem contentDownloadItem) {
			if (this.OnDownloaderDownloadStart != null) {
				this.OnDownloaderDownloadStart(contentDownloadItem);
			}

			string resourceAssetBundleName = contentDownloadItem.ContentInfo.ResourceAssetBundleName;
			string sceneAssetBundleName = contentDownloadItem.ContentInfo.SceneAssetBundleName;


			if (contentDownloadItem.DownloadYieldInstruction != null) {
				contentDownloadItem.DownloadYieldInstruction.Dispose();
				contentDownloadItem.DownloadYieldInstruction = null;
			}

			using (AbstractDownloaderYieldInstruction downloadYieldInstruction = CreateDownloadYieldInstruction(resourceAssetBundleName, sceneAssetBundleName)) {
				contentDownloadItem.DownloadYieldInstruction = downloadYieldInstruction;

				contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Downloading);
				yield return downloadYieldInstruction;

				if (!downloadYieldInstruction.IsDisposed) {
					ContentDownloadError error = downloadYieldInstruction.DownloadError;
					string errorMessage = downloadYieldInstruction.DownloadErrorMessage;
					if (error == ContentDownloadError.None &&
					    downloadYieldInstruction.ResourceAssetBundle != null &&
					    downloadYieldInstruction.SceneAssetBundle != null) {
						//Download Complete, we retain the resources directly on this contentDownloadItem

						contentDownloadItem.ReleaseAssetBundles();
						contentDownloadItem.ResourceAssetBundle = downloadYieldInstruction.ResourceAssetBundle;
						contentDownloadItem.ResourceAssetBundle.Retain();
						contentDownloadItem.SceneAssetBundle = downloadYieldInstruction.SceneAssetBundle;
						contentDownloadItem.SceneAssetBundle.Retain();
					}

					downloadYieldInstruction.Dispose();
					contentDownloadItem.DownloadYieldInstruction = null;

					if (error != ContentDownloadError.None) {
						m_DownloadError = error;
						contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Error, error);
						if (this.OnDownloaderDownloadError != null) {
							this.OnDownloaderDownloadError(m_DownloadError, errorMessage, contentDownloadItem);
						}

						if (this.OnDownloaderDownloadFinish != null) {
							this.OnDownloaderDownloadFinish(contentDownloadItem, false);
						}
					} else {
						contentDownloadItem.NotifyDownloadStateChange(ContentDownloadState.Complete);

						if (this.OnDownloaderDownloadFinish != null) {
							this.OnDownloaderDownloadFinish(contentDownloadItem, true);
						}
					}
				} else {
					if (this.OnDownloaderDownloadFinish != null) {
						this.OnDownloaderDownloadFinish(contentDownloadItem, false);
					}
				}
			}
		}

		/// <summary>
		/// Constantly check for internet connection after given delay. 
		/// If we are not already in LostInternet/LostWiFi error state, notifies everyone of NoWiFi/NoInternet state.
		/// </summary>
		/// <param name="delay">Interval before each internet connection test.</param>
		private IEnumerator WaitForInternetConnection(float delay) {
			while (!this.DownloadQueue.IsEmpty()) {
				using (InternetReachabilityYieldInstruction reachabilityYieldInstruction = new InternetReachabilityYieldInstruction(this)) {
					yield return reachabilityYieldInstruction;

					if (this.DownloadQueue.IsEmpty()) {
						break;
					}

					if (!reachabilityYieldInstruction.IsInternetReachable) {
						if (m_DownloadError == ContentDownloadError.None) {
							if (DownloadViaWiFiOnly) {
								m_DownloadError = ContentDownloadError.NoWifi;
								NotifyAll(ContentDownloadState.Error, ContentDownloadError.NoWifi);
								NotifyAll(ContentDownloadState.Queued);
							} else {
								m_DownloadError = ContentDownloadError.NoInternet;
								NotifyAll(ContentDownloadState.Error, ContentDownloadError.NoInternet);
								NotifyAll(ContentDownloadState.Queued);
							}

							if (this.OnDownloaderDownloadError != null) {
								this.OnDownloaderDownloadError(m_DownloadError, NoInternetErrorMessage, this.DownloadQueue.GetFirst());
							}
						}
					} else {
						yield break;
					}
				}

				yield return new WaitForSeconds(delay);
			}
		}

		#endregion

		#region Private Methods

		private void NotifyAll(ContentDownloadState state, ContentDownloadError error = ContentDownloadError.None) {
			foreach (ContentDownloadItem contentDownloadItem in DownloadQueue.ToArray()) {
				contentDownloadItem.NotifyDownloadStateChange(state, error);
			}
		}

		private void NotifyAndDequeueAll(ContentDownloadState state, ContentDownloadError error = ContentDownloadError.None) {
			NotifyAll(state, error);
			DownloadQueue.Clear();
		}

		#endregion
	}
}