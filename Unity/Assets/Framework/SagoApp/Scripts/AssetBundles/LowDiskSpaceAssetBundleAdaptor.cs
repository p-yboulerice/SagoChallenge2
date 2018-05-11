namespace SagoApp.Project {

	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using AOT;
	using SagoCore.AssetBundles;
	using UnityEngine;

	public class LowDiskSpaceAssetBundleAdaptor : CustomYieldInstruction, IAssetBundleAdaptor {

		/// <summary>
		/// This error string is meant only for being checked to see if there has been a low disk space error thrown.
		/// </summary>
		public const string LowDiskSpaceError = "SagoApp.Project.LowDiskSpaceAssetBundleAdaptor.LowDiskSpaceError";

		#region Fields

		[System.NonSerialized]
		private IAssetBundleAdaptor m_Adaptor;

		[System.NonSerialized]
		private bool m_IsCancelled;

		#endregion


		#region Properties

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.assetBundle" />
		/// </summary>
		public AssetBundle assetBundle {
			get { return m_Adaptor != null ? m_Adaptor.assetBundle : null; }
		}

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.assetBundleName" />
		/// </summary>
		public string assetBundleName {
			get { return m_Adaptor != null ? m_Adaptor.assetBundleName : null; }
		}

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.error" />
		/// </summary>
		public string error {
			get {
				if (m_Adaptor != null) {
					if (isCancelled) {
						return LowDiskSpaceError;
					} else {
						return m_Adaptor.error;
					}
				}
				return "";
			}
		}

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.isDone" />
		/// </summary>
		public bool isDone {
			get { return !keepWaiting; }
		}

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.keepWaiting" />
		/// </summary>
		override public bool keepWaiting {
			get { return m_Adaptor != null ? !isCancelled && m_Adaptor.keepWaiting : false; }
		}

		/// <summary>
		/// <see cref="IAssetBundleAdaptor.progress" />
		/// </summary>
		public float progress {
			get { return m_Adaptor != null ? m_Adaptor.progress : 0f; }
		}

		public bool isCancelled {
			get { return m_IsCancelled; }
		}

		#endregion


		#region Static Fields

		private static List<LowDiskSpaceAssetBundleAdaptor> m_LowDiskSpaceAssetBundleAdaptors = new List<LowDiskSpaceAssetBundleAdaptor>();

		#endregion


		#region Constructor

		public LowDiskSpaceAssetBundleAdaptor(IAssetBundleAdaptor adaptor) {
			if (adaptor == null) {
				Debug.LogWarning("LowDiskSpaceAssetBundleAdaptor-> adaptor instance passed to constructor is null.", DebugContext.SagoApp);
				return;
			}

			m_IsCancelled = false;
			m_Adaptor = adaptor;

			if (!m_LowDiskSpaceAssetBundleAdaptors.Contains(this)) {
				m_LowDiskSpaceAssetBundleAdaptors.Add(this);
			}
		}

		#endregion


		#region Methods

		public void Cancel() {
			Debug.Log("LowDiskSpaceAssetBundleAdaptor-> Cancel", DebugContext.SagoApp);
			m_IsCancelled = true;
		}

		public void Dispose() {
			Debug.Log("LowDiskSpaceAssetBundleAdaptor-> Dispose", DebugContext.SagoApp);

			if (m_Adaptor != null) {
				m_Adaptor.Dispose();
				m_Adaptor = null;
			}

			if (m_LowDiskSpaceAssetBundleAdaptors.Contains(this)) {
				m_LowDiskSpaceAssetBundleAdaptors.Remove(this);
			}
		}

		#endregion


		#region Native binding

		#if UNITY_IOS && !UNITY_EDITOR

		public delegate void lowDiskSpaceWarningCallback();

		[DllImport ("__Internal")]
		private static extern void _SetLowDiskSpaceWarningCallback(lowDiskSpaceWarningCallback callback);

		[MonoPInvokeCallback(typeof(lowDiskSpaceWarningCallback))]
		public static void LowDiskSpaceWarningCallback() {
			Debug.Log("LowDiskSpaceAssetBundleAdaptor-> LowDiskSpaceWarningCallback", DebugContext.SagoApp);
			foreach (LowDiskSpaceAssetBundleAdaptor adaptor in m_LowDiskSpaceAssetBundleAdaptors) {
				if (adaptor != null) {
					adaptor.Cancel();
				}
			}
		}

		#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void OnRuntimeSetCallback() {
			Debug.Log("LowDiskSpaceAssetBundleAdaptor-> OnRuntimeSetCallback", DebugContext.SagoApp);
			#if UNITY_IOS && !UNITY_EDITOR
				_SetLowDiskSpaceWarningCallback(LowDiskSpaceWarningCallback);
			#endif
		}

		#endregion

	}

}