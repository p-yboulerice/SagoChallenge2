namespace SagoApp.ContentDownloader {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SagoCore.AssetBundles;

	/// <summary>
	/// The DownloadAssetBundleReference class emulates the AssetBundleReference class in order to have
	/// a specific CreateAdaptor method for download, passing the downloadOnly flag.
	/// </summary>

	/// <example>
	/// <para>
	/// The following code gets an asset bundle reference, retains the reference 
	/// while it's in use and releases the reference when it's not in use anymore.
	/// </para>
	/// <code>
	/// private IEnumerator Start() {
	/// 	
	/// 	DownloadAssetBundleReference reference;
	/// 	reference = DownloadAssetBundleReference.FindOrCreateReference("my_asset_bundle");
	/// 	reference.Retain();
	/// 	
	/// 	yield return reference;
	/// 	Debug.Log(reference.assetBundle);
	/// 	
	/// 	reference.Release();
	/// 	
	/// }
	/// </code>
	/// </example>

	public class DownloadAssetBundleReference : CustomYieldInstruction {


		#region Static Fields

		/// <summary>
		/// The dictionary of asset bundle references.
		/// </summary>
		/// <remarks>
		/// The keys are asset bundle names. The values are asset bundle references.
		/// </remarks>
		private static Dictionary<string, DownloadAssetBundleReference> _References;

		#endregion


		#region Properties

		/// <summary>
		/// Gets the dictionary of asset bundle references.
		/// </summary>
		private static Dictionary<string, DownloadAssetBundleReference> References {
			get { return _References = _References ?? new Dictionary<string, DownloadAssetBundleReference>(); }
		}

		#endregion


		#region Methods

		/// <summary>
		/// Gets and sets the create adaptor method.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		/// <returns>
		/// The IAssetBundleAdaptor instance to use for the specified asset bundle.
		/// </returns>
		public static System.Func<string, IAssetBundleAdaptor> CreateAdaptor {
			get; set;
		}

		/// <summary>
		/// Finds an existing asset bundle reference for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public static DownloadAssetBundleReference FindReference(string assetBundleName) {

			if (string.IsNullOrEmpty(assetBundleName)) {
				throw new System.ArgumentNullException("assetBundleName");
			}

			if (References.ContainsKey(assetBundleName)) {
				DownloadAssetBundleReference reference = References[assetBundleName];
				reference.Retain();
				reference.Autorelease();
				return reference;
			}

			return null;

		}

		/// <summary>
		/// Finds an existing asset bundle reference or creates a new asset 
		/// bundle reference for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public static DownloadAssetBundleReference FindOrCreateReference(string assetBundleName) {

			if (string.IsNullOrEmpty(assetBundleName)) {
				throw new System.ArgumentNullException("assetBundleName");
			}

			if (!References.ContainsKey(assetBundleName)) {
				References.Add(assetBundleName, new DownloadAssetBundleReference(assetBundleName));
			}

			return FindReference(assetBundleName);

		}

		#endregion


		#region Fields

		/// <summary>
		/// The asset bundle adaptor.
		/// </summary>
		private IAssetBundleAdaptor m_Adaptor;

		/// <summary>
		/// The asset bundle.
		/// </summary>
		private AssetBundle m_AssetBundle;

		/// <summary>
		/// The asset bundle name.
		/// </summary>
		private string m_AssetBundleName;

		/// <summary>
		/// The coroutine.
		/// </summary>
		private IEnumerator m_Coroutine;

		/// <summary>
		/// The error that occurred while loading the asset bundle.
		/// </summary>
		private string m_Error;

		/// <summary>
		/// The flag indicating whether the asset bundle has finished loading or an error has occurred.
		/// </summary>
		private bool m_KeepWaiting;

		/// <summary>
		/// The progress of the load operation.
		/// </summary>
		private float m_Progress;

		/// <summary>
		/// The reference count.
		/// <see cref="Retain()" />
		/// <see cref="Release()" />
		/// <see cref="Autorelease()" />
		/// </summary>
		private int m_ReferenceCount;

		#endregion


		#region Properties

		/// <summary>
		/// Gets the asset bundle adaptor.
		/// </summary>
		public IAssetBundleAdaptor adaptor {
			get { return m_Adaptor; }
		}

		/// <summary>
		/// Gets the asset bundle.
		/// </summary>
		/// <returns>
		/// <c>null</c> unless the asset bundle has finished loading, or if an error occurs.
		/// </remarks>
		public AssetBundle assetBundle {
			get { return m_AssetBundle; }
		}

		/// <summary>
		/// Gets the name of the asset bundle.
		/// </summary>
		public string assetBundleName {
			get { return m_AssetBundleName; }
		}

		/// <summary>
		/// Gets the error that occurred while loading the asset bundle.
		/// </summary>
		/// <returns>
		/// <c>null</c> unless there was an error loading the asset bundle.
		/// </remarks>
		public string error {
			get { return m_Error; }
		}

		/// <summary>
		/// <see cref="keepWaiting" />
		/// </summary>
		public bool isDone {
			get { return !keepWaiting; }
		}

		/// <summary>
		/// Gets the flag indicating whether the asset bundle has finished loading or an error has occurred.
		/// </summary>
		/// <returns>
		/// <c>false</c> if the asset bundle has finished loading or an error has occurred. Otherwise, <c>true</c>.
		/// </returns>
		override public bool keepWaiting {
			get { return m_KeepWaiting; }
		}

		/// <summary>
		/// Gets the progress of the load operation.
		/// </summary>
		public float progress {
			get { return m_Progress; }
		}

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new <see cref="DownloadAssetBundleReference" /> instance.
		/// <summary>
		/// <remarks>
		/// The constructor is private in order to make sure there are never two 
		/// <see cref="DownloadAssetBundleReference" /> instances with the same asset bundle 
		/// name. To create an <see create="AssetBundleReference" />, use the static 
		/// <see cref="FindOrCreateReference()" /> method.
		/// </remarks>
		private DownloadAssetBundleReference(string assetBundleName) {
			if (AssetBundleLoader.Instance) {

				m_Adaptor = null;
				m_AssetBundle = null;
				m_AssetBundleName = assetBundleName;
				m_Error = null;
				m_KeepWaiting = true;
				m_ReferenceCount = 0;

				Retain();
				Autorelease();

				m_Coroutine = DownloadAssetBundleReferenceImpl(assetBundleName);
				AssetBundleLoader.Instance.StartCoroutine(m_Coroutine);

			}
		}

		#endregion


		#region Methods

		/// <summary>
		/// Increments the reference count.
		/// </summary>
		public void Retain() {
			m_ReferenceCount++;
		}

		/// <summary>
		/// Decrements the reference count. When the reference count reaches zero, 
		/// the asset bundle will be unloaded and the reference will be removed 
		/// from the dictionary.
		/// </summary>
		public void Release() {
			if (m_ReferenceCount > 0) {
				m_ReferenceCount--;
			}
			if (m_ReferenceCount == 0) {
				m_ReferenceCount = -1;
				Dispose();
			}
		}

		/// <summary>
		/// Releases the reference on the next frame.
		/// </summary>
		/// <remarks>
		/// The autorelease method allows <see cref="FindOrCreateReference()" /> to 
		/// return an object with a reference count of one and make sure it gets 
		/// cleaned up on the next frame if nothing else retains it.
		/// </remarks>
		public void Autorelease() {
			if (AssetBundleLoader.Instance) {
				AssetBundleLoader.Instance.StartCoroutine(AutoreleaseImpl());
			}
		}

		/// <summary>
		/// Disposes the reference.
		/// </summary>
		private void Dispose() {

			Debug.LogWarningFormat("DownloadAssetBundleReference.Dispose(): {0}", m_AssetBundleName);

			if (References.ContainsKey(m_AssetBundleName)) {
				References.Remove(m_AssetBundleName);
				m_AssetBundleName = null;
			}

			if (m_Coroutine != null) {
				if (AssetBundleLoader.Instance) {
					AssetBundleLoader.Instance.StopCoroutine(m_Coroutine);
				}
				m_Coroutine = null;
			}

			if (m_AssetBundle != null) {
				m_AssetBundle.Unload(false);
				m_AssetBundle = null;
			}

			if (m_Adaptor != null) {
				m_Adaptor.Dispose();
				m_Adaptor = null;
			}

			m_Error = null;
			m_KeepWaiting = false;

		}

		#endregion


		#region Other

		/// <summary>
		/// <see cref="DownloadAssetBundleReference" />
		/// </summary>
		private IEnumerator DownloadAssetBundleReferenceImpl(string assetBundleName) {

			Retain();

			if (CreateAdaptor == null) {
				m_Error = string.Format("Could not create adaptor: not implemented.");
				m_KeepWaiting = false;
				m_Progress = -1;
				Release();
				yield break;
			}

			m_Adaptor = CreateAdaptor(assetBundleName);

			if (m_Adaptor == null) {
				m_Error = string.Format("Could not create adaptor for asset bundle: {0}", assetBundleName);
				m_KeepWaiting = false;
				m_Progress = -1;
				Release();
				yield break;
			}

			while (!m_Adaptor.isDone) {
				m_Progress = m_Adaptor.progress;
				yield return null;
			}

			if (!string.IsNullOrEmpty(m_Adaptor.error)) {
				m_Error = m_Adaptor.error;
				m_Adaptor.Dispose();
				m_Adaptor = null;
				m_KeepWaiting = false;
				m_Progress = -1;
				Release();
				yield break;
			}

			m_AssetBundle = m_Adaptor.assetBundle;
			m_KeepWaiting = false;
			m_Progress = 1;
			Release();
			yield break;

		}

		/// <summary>
		/// <see cref="Autorelease" />
		/// </summary>
		private IEnumerator AutoreleaseImpl() {
			yield return null;
			Release();
		}

		#endregion


	}



}
