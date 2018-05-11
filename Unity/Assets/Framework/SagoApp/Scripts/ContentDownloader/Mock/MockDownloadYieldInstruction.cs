namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using System.Collections;
	using SagoCore.AssetBundles;
	using SagoApp.Project;

	public class MockDownloadYieldInstruction : AbstractDownloaderYieldInstruction {
		
		#region Fields

		private float m_MockDownloadTime;
		private ContentDownloadError m_MockError;

		#endregion

		#region Constructor

		public MockDownloadYieldInstruction(MonoBehaviour monoBehaviour, float mockDownloadTime,
		                                    ContentDownloadError mockError) : base(monoBehaviour) {
			if (monoBehaviour != null) {
				m_MockDownloadTime = mockDownloadTime;
				m_MockError = mockError;
				m_MonoBehaviour = monoBehaviour;
				m_Coroutine = Download();
				m_MonoBehaviour.StartCoroutine(m_Coroutine);
			} else {
				m_IsDone = true;
				Debug.LogError("ContentDownloader-MockDownloadYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion

		#region Coroutines

		private IEnumerator Download() {
			
			if (m_MockError == ContentDownloadError.None) {
				float durationInSeconds;
				durationInSeconds = m_MockDownloadTime;

				float elapsedTime;
				elapsedTime = 0;

				this.ProgressReport.Reset();
				this.ProgressReport.Index = 0;
				this.ProgressReport.Count = 1;
				this.ProgressReport.Item = new MockProgressReport();

				while (elapsedTime < durationInSeconds) {
					float totalProgress;
					totalProgress = Mathf.Clamp01(elapsedTime / durationInSeconds);

					((MockProgressReport)this.ProgressReport.Item).UpdateProgress(totalProgress);

					elapsedTime += Time.deltaTime;

					yield return null;
				}
			} else {
				m_DownloadError = m_MockError;
			}

			Complete();
		}

		#endregion
	}

	public class MockProgressReport : ProgressReportItem {

		#region Fields

		private float m_Progress;

		#endregion

		#region Properties

		override public float Progress {
			get { return m_Progress; }
		}

		#endregion

		#region Methods

		internal void UpdateProgress(float progress) {
			m_Progress = progress;
		}

		#endregion
	}
}
