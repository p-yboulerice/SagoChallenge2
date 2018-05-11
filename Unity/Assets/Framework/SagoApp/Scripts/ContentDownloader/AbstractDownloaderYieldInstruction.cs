namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;
	using SagoCore.AssetBundles;

	public abstract class AbstractDownloaderYieldInstruction : CustomYieldInstruction, System.IDisposable {
		
		#region Fields

		protected ContentDownloadState m_DownloadState;

		protected ContentDownloadError m_DownloadError;

		protected string m_DownloadErrorMessage;

		protected bool m_IsDone;

		protected bool m_IsDisposed;

		protected MonoBehaviour m_MonoBehaviour;

		protected IEnumerator m_Coroutine;

		#endregion

		#region Properties

		public DownloadAssetBundleReference ResourceAssetBundle {
			get;
			set;
		}

		public DownloadAssetBundleReference SceneAssetBundle {
			get;
			set;
		}

		public ProgressReport ProgressReport {
			get;
			private set;
		}

		public ContentDownloadState DownloadState {
			get { return m_DownloadState; }
		}

		public ContentDownloadError DownloadError {
			get { return m_DownloadError; }
		}

		public string DownloadErrorMessage {
			get { return m_DownloadErrorMessage; }
		}

		public bool IsDone {
			get { return m_IsDone; }
		}

		public bool IsDisposed {
			get { return m_IsDisposed; }
		}

		public override bool keepWaiting {
			get { return !m_IsDone; }
		}

		#endregion

		#region Constructor

		public AbstractDownloaderYieldInstruction(MonoBehaviour monoBehaviour) {
			m_MonoBehaviour = null;
			m_Coroutine = null;
			m_IsDone = false;
			m_IsDisposed = false;
			m_DownloadState = ContentDownloadState.Unknown;
			m_DownloadError = ContentDownloadError.None;
			this.ProgressReport = new ProgressReport();
		}

		#endregion

		#region Public Methods

		public void Dispose() {

			if (m_MonoBehaviour != null && m_Coroutine != null) {
				m_DownloadState = ContentDownloadState.Unknown;
				m_DownloadError = ContentDownloadError.None;
				m_MonoBehaviour.StopCoroutine(m_Coroutine);
				Complete();
			}

			ReleaseAssetBundles();

			m_IsDisposed = true;
		}

		#endregion


		#region Private Methods

		protected virtual void Complete() {
			this.ProgressReport.Reset();
			this.m_IsDone = true;
			this.m_MonoBehaviour = null;
			this.m_Coroutine = null;
		}

		private void ReleaseAssetBundles() {
			if (this.ResourceAssetBundle != null) {
				this.ResourceAssetBundle.Release();
				this.ResourceAssetBundle = null;
			}

			if (this.SceneAssetBundle != null) {
				this.SceneAssetBundle.Release();
				this.SceneAssetBundle = null;
			}
		}

		#endregion
	}
}