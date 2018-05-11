namespace SagoApp.ContentDownloader {

	using UnityEngine;
	using SagoCore.AssetBundles;
	using SagoCore.Scenes;

	public class ProgressReport {


		#region Properties

		public int Count {
			get;
			set;
		}

		public int Index {
			get;
			set;
		}

		public ProgressReportItem Item {
			get;
			set;
		}

		public float TotalProgress {
			get {

				if (Count < 1 || Index < 0) {
					return 0f;
				}

				float totalProgress;
				totalProgress = Mathf.Clamp ((float)Index / (float)Count, 0f, 1f);

				float itemProgress;
				itemProgress = Item != null ? Item.Progress : 0f;
				itemProgress = Mathf.Clamp (itemProgress / (float)Count, 0f, 1f);

				return totalProgress + itemProgress;

			}
		}

		#endregion


		#region Constructors

		public ProgressReport () {
			Reset ();
		}

		#endregion


		#region Methods

		public void Reset () {
			this.Count = -1;
			this.Index = -1;
			this.Item = null;
		}

		#endregion


	}

	public abstract class ProgressReportItem {


		#region Properties

		abstract public float Progress {
			get;
		}

		#endregion


	}

	public class LoadAssetBundleProgressReportItem : ProgressReportItem {


		#region Properties

		public DownloadAssetBundleReference AssetBundleReference {
			get;
			private set;
		}

		override public float Progress {
			get { return AssetBundleReference != null ? AssetBundleReference.progress : 0; }
		}

		#endregion


		#region Constructors

		public LoadAssetBundleProgressReportItem (DownloadAssetBundleReference assetBundleReference) {
			AssetBundleReference = assetBundleReference;
		}

		#endregion


	}
}