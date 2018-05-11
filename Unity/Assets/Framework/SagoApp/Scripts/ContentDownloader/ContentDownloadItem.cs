namespace SagoApp.ContentDownloader {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoApp.Content;
	using SagoCore.AssetBundles;

	[System.Serializable]
	public enum ContentDownloadState {
		Unknown, //Not started or canceled
		Refreshing,
		Queued,
		Downloading,
		Complete,
		Error
	}

	[System.Serializable]
	public enum ContentDownloadError {
		None,
		InsufficientDiskSpace,
		RunOutOfDiskSpace,
		NoInternet,
		NoWifi,
		LostInternet,
		LostWifi,
		UnknownError
	}

	/// <summary>
	/// Get the name of the analytics error string depending on given ContentDownloadError
	/// Used for backward comptability with error analytics.
	/// </summary>
	/// <returns>The analytics error string.</returns>
	/// <param name="error">Error.</param>
	public static class ContentDownloadErrorExtensions {
		public static string ToAnalyticsError(this ContentDownloadError error) {
			switch (error) {
			case ContentDownloadError.NoWifi:
			case ContentDownloadError.LostWifi:
				return "OdrErrorNoWiFi";
			case ContentDownloadError.NoInternet:
			case ContentDownloadError.LostInternet:
				return "OdrErrorNoInternet";
			case ContentDownloadError.InsufficientDiskSpace:
			case ContentDownloadError.RunOutOfDiskSpace:
				return "LowDiskSpace";
			case ContentDownloadError.None:
				return "None";
			default:
				return "OdrErrorUnknown";
			}
		}
	}

	public class ContentDownloadItem {

		#region Types

		public delegate void ContentDownloadItemCallback(ContentDownloadItem contentDownloadItem);

		#endregion

		#region Events

		public event ContentDownloadItemCallback OnRefreshStateChange;
		public event ContentDownloadItemCallback OnDownloadStateChange;

		#endregion

		#region Properties

		public AbstractContentDownloader ContentDownloader {
			get;
			internal set;
		}

		public ContentInfo ContentInfo {
			get;
			internal set;
		}

		public ContentDownloadState State {
			get;
			internal set;
		}

		public ContentDownloadError Error {
			get;
			internal set;
		}

		public float DownloadProgress {
			get {
				if (this.DownloadYieldInstruction != null) {
					return this.DownloadYieldInstruction.ProgressReport.TotalProgress;
				} else {
					return (this.State == ContentDownloadState.Complete) ? 1.0f : 0.0f;
				}
			}
		}

		internal AbstractDownloaderYieldInstruction RefreshYieldInstruction {
			get;
			set;
		}

		internal AbstractDownloaderYieldInstruction DownloadYieldInstruction {
			get;
			set;
		}

		internal DownloadAssetBundleReference ResourceAssetBundle {
			get;
			set;
		}

		internal DownloadAssetBundleReference SceneAssetBundle {
			get;
			set;
		}

		#endregion

		#region Public API

		/// <summary>
		/// Starts a download if this download item state is Unknown (not downloaded)
		/// OnStateChange will be called if download state change or an error occured.
		/// </summary>
		public void RequestDownload() {
			if (this.ContentDownloader != null) {
				this.ContentDownloader.RequestDownload(this);
			}
		}

		/// <summary>
		/// Cancels the download if this download item state is not Complete or Unknown.
		/// OnStateChange will be called when the state changes to Unknown.
		/// </summary>
		/// <param name="forceReset">If set to <c>true</c> force cancel if state is complete.</param>
		public void CancelDownload(bool forceReset = false) {
			if (this.ContentDownloader != null) {
				this.ContentDownloader.CancelDownload(this, forceReset);
			}
		}

		/// <summary>
		/// Refreshes the state if this download item state state is Complete or Unknown
		/// OnStateRefresh will be called in all cases.
		/// </summary>
		public void RequestRefresh() {
			//Note : We bypass any subsequent refresh after first refresh to avoid
			//an issue where refresh can take 5-10s (SW-341)
			if (m_IsRefreshed) {
				NotifyRefreshStateChange();
			} else {
				if (this.ContentDownloader != null) {
					this.ContentDownloader.RequestRefresh(this);
				}
			}
		}

		/// <summary>
		/// Stops a pending refresh request.
		/// Item state will be set to Unknown.
		/// </summary>
		public void CancelRefresh() {
			if (this.ContentDownloader != null) {
				this.ContentDownloader.CancelRefresh(this);
			}
		}

		#endregion

		#region Internal Fields

		[System.NonSerialized]
		private bool m_IsRefreshed;

		#endregion

		#region Internal API

		internal void Dispose() {
			if (this.RefreshYieldInstruction != null) {
				this.RefreshYieldInstruction.Dispose();
				this.RefreshYieldInstruction = null;
			}

			if (this.DownloadYieldInstruction != null) {
				this.DownloadYieldInstruction.Dispose();
				this.DownloadYieldInstruction = null;
			}

			ReleaseAssetBundles();
		}

		internal void ReleaseAssetBundles() {
			if (this.ResourceAssetBundle != null) {
				this.ResourceAssetBundle.Release();
				this.ResourceAssetBundle = null;
			}

			if (this.SceneAssetBundle != null) {
				this.SceneAssetBundle.Release();
				this.SceneAssetBundle = null;
			}
		}

		internal void NotifyRefreshStateChange() {
			m_IsRefreshed = true;

			string submoduleName = "Unknown";
			if (this.ContentInfo != null) {
				submoduleName = this.ContentInfo.SubmoduleName;
			}

			Debug.LogFormat("ContentDownloader->State Refreshed for {0} : State = {1}, Error = {2}",
				                submoduleName,
						this.State.ToString(),
				                this.Error.ToString());
			
			if (this.OnRefreshStateChange != null) {
				this.OnRefreshStateChange(this);
			}
		}

		internal void NotifyDownloadStateChange(ContentDownloadState state, ContentDownloadError error = ContentDownloadError.None) {
			if (this.State != state || this.Error != error) {
				string submoduleName = "Unknown";
				if (this.ContentInfo != null) {
					submoduleName = this.ContentInfo.SubmoduleName;
				}

				Debug.LogFormat("ContentDownloader->State changed for {0} : State = {1}, Error = {2}",
				                submoduleName,
						state.ToString(),
						error.ToString());
				
				this.State = state;
				this.Error = error;

				if (this.OnDownloadStateChange != null) {
					this.OnDownloadStateChange(this);
				}
			}
		}

		#endregion
	}
}