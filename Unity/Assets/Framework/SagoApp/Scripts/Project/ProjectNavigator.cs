namespace SagoApp.Project {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using SagoEngine;
	using SagoNavigation;
	using SagoUtils;
	using SagoCore.AssetBundles;
	using SagoCore.Resources;
	using SagoCore.Scenes;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	
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
				totalProgress = Mathf.Clamp((float)Index / (float)Count, 0f, 1f);
				
				float itemProgress;
				itemProgress = Item != null ? Item.Progress : 0f;
				itemProgress = Mathf.Clamp(itemProgress / (float)Count, 0f, 1f);
				
				return totalProgress + itemProgress;
				
			}
		}
		
		#endregion
		
		
		#region Constructors
		
		public ProgressReport() {
			Reset();
		}
		
		#endregion
		
		
		#region Methods
		
		public void Reset() {
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
		
		public AssetBundleReference AssetBundleReference {
			get;
			private set;
		}
		
		override public float Progress {
			get { return AssetBundleReference != null ? AssetBundleReference.progress : 0; }
		}
		
		#endregion
		
		
		#region Constructors
		
		public LoadAssetBundleProgressReportItem(AssetBundleReference assetBundleReference) {
			AssetBundleReference = assetBundleReference;
		}
		
		#endregion
		
		
	}
	
	public class LoadSceneProgressReportItem : ProgressReportItem {
		
		
		#region Properties
		
		public SceneReference SceneReference {
			get;
			private set;
		}
		
		override public float Progress {
			get { return !string.IsNullOrEmpty(SceneReference.Guid) ? SceneNavigator.Instance.SceneLoadProgress(SceneReference) : 0; }
		}
		
		#endregion
		
		
		#region Constructors
		
		public LoadSceneProgressReportItem(SceneReference sceneReference) {
			SceneReference = sceneReference;
		}
		
		#endregion
		
		
	}

	public enum ProjectNavigatorState {
		Unknown,
		Project,
		NavigateToContent,
		Content,
		NavigateToProject,
		NavigateToError,
		Error
	}

	public enum ProjectNavigatorError {
		None,
		OdrLoadingCancelled,
		OdrErrorNoWiFi,
		OdrErrorNoInternet,
		OdrErrorUnknown,
		LowDiskSpace
	}
	
	public class ProjectNavigator : MonoBehaviour {
		
		
		#region Static Methods
		
		[RuntimeInitializeOnLoadMethod]
		private static void Init() {
			
			CreateDefaultSettings();
			Instance.ContentInfo = null;
			Instance.ProgressReport = new ProgressReport();
			Instance.State = ProjectNavigatorState.Project;
			
			#if UNITY_EDITOR
				
				System.Type type;
				type = SubmoduleMap.GetSubmoduleType(SceneManager.GetActiveScene().path);
				
				ContentInfo contentInfo;
				contentInfo = ProjectInfo.Instance.GetContentInfo(type);
				
				if (contentInfo != null) {
					ApplyContentSettings(contentInfo);
					Instance.ContentInfo = contentInfo;
					Instance.State = ProjectNavigatorState.Content;
				}
				
			#endif
			
		}
		
		private static void ApplyContentSettings(ContentInfo contentInfo) {
			contentInfo.DynamicsManager.Push();
			contentInfo.Physics2DSettings.Push();
			contentInfo.TimeManager.Push();
		}
		
		private static void ApplyDefaultSettings() {
			DynamicsManager.DefaultDynamicsManager.Push();
			Physics2DSettings.DefaultPhysics2DSettings.Push();
			TimeManager.DefaultTimeManager.Push();
		}
		
		private static void CreateDefaultSettings() {
			DynamicsManager.CreateDefaultDynamicsManager();
			Physics2DSettings.CreateDefaultPhysics2DSettings();
			TimeManager.CreateDefaultTimeManager();
		}
		
		#endregion
		
		
		#region Singleton
		
		private static ProjectNavigator _Instance;
		
		public static ProjectNavigator Instance {
			get {
				
				#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					return null;
				}
				#endif
				
				if (!_Instance) {
					_Instance = new GameObject("ProjectNavigator").AddComponent<ProjectNavigator>();
					DontDestroyOnLoad(_Instance);
				}
				
				return _Instance;
				
			}
		}
		
		#endregion


		#region Constants

		public const string DownloadViaWiFiOnlyKey = "SagoApp.Project.ProjectNavigator.DownloadViaWiFiOnly";

		#endregion
		
		
		#region Fields

		public bool DownloadViaWiFiOnly {
			get { return PlayerPrefs.GetInt(DownloadViaWiFiOnlyKey, 1) == 1; }
			set { PlayerPrefs.SetInt(DownloadViaWiFiOnlyKey, value ? 1 : 0); }
		}

		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		[System.NonSerialized]
		private CoroutineHelper m_CoroutineHelper;
		
		[SerializeField]
		private ProjectNavigatorState m_State;
		
		#endregion
		
		
		#region Events
		
		public event System.Action<ContentInfo> DidNavigateToContent;
		
		public event System.Action<ContentInfo> WillNavigateToContent;
		
		public event System.Action<ContentInfo> DidNavigateToProject;
		
		public event System.Action<ContentInfo> WillNavigateToProject;

		public event System.Action<ContentInfo, string, string> DidNavigateToError;

		public event System.Action<ContentInfo, string, string> WillNavigateToError;
		
		
		public event System.Action<ContentInfo> NavigateToContentWillLoadAssetBundles;
		
		public event System.Action<ContentInfo,bool> NavigateToContentDidLoadAssetBundles;
		
		public event System.Action<ContentInfo> NavigateToContentWillLoadMainScene;
		
		public event System.Action<ContentInfo,bool> NavigateToContentDidLoadMainScene;
		
		#endregion
		
		
		#region Properties

		public ProjectNavigatorError Error {
			get;
			private set;
		}
		
		public ContentInfo ContentInfo {
			get { return m_ContentInfo; }
			private set {
				m_ContentInfo = value;
				#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
		}
		
		public bool IsBusy {
			get {
				switch (this.State) {
					case ProjectNavigatorState.Content:
					case ProjectNavigatorState.Project:
					case ProjectNavigatorState.Error:
						return SceneNavigator.Instance && SceneNavigator.Instance.IsBusy;
					default:
						return true;
				}
			}
		}
		
		public bool IsReady {
			get { return !IsBusy; }
		}
		
		public ProgressReport ProgressReport {
			get;
			private set;
		}
		
		
		private CoroutineHelper CoroutineHelper {
			get { return m_CoroutineHelper = m_CoroutineHelper ?? new CoroutineHelper(this); }
		}
		
		private AssetBundleReference ResourceAssetBundle {
			get;
			set;
		}
		
		private AssetBundleReference SceneAssetBundle {
			get;
			set;
		}
		
		private ProjectNavigatorState State {
			get { return m_State; }
			set {
				m_State = value;
				#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
		}
		
		#endregion
		
		
		#region Methods
		
		public void NavigateToProject() {
			if (this.IsReady && (this.State == ProjectNavigatorState.Content || this.State == ProjectNavigatorState.Error)) {
				this.ProgressReport.Reset();
				this.State = ProjectNavigatorState.NavigateToProject;
				this.CoroutineHelper.StartCoroutine("NavigateToProject", NavigateToProjectImpl());
			} else {
				throw new System.InvalidOperationException("Cannot navigate to project.");
			}
		}
		
		public void NavigateToContent(ContentInfo contentInfo) {
			if (this.IsReady && this.State == ProjectNavigatorState.Project) {
				this.ContentInfo = contentInfo;
				this.ProgressReport.Reset();
				this.State = ProjectNavigatorState.NavigateToContent;
				this.CoroutineHelper.StartCoroutine("NavigateToContent", NavigateToContentImpl());
			} else {
				throw new System.InvalidOperationException("Cannot navigate to content.");
			}
		}

		/// <summary>
		/// Call to navigate to error scene.
		/// </summary>
		/// <param name="errorType">This is meant for errors that we know of and defined in some form of data like enum types.</param>
		/// <param name="errorMessage">This is a raw string in the case of any errors that we do not know of.</param>
		public void NavigateToError(ProjectNavigatorError errorType, string errorMessage) {
			this.State = ProjectNavigatorState.NavigateToError;
			this.CoroutineHelper.StartCoroutine("NavigateToError", NavigateToErrorImpl(errorType, errorMessage));
		}
		
		#endregion
		
		
		#region Methods
		
		private void OnApplicationQuit() {
			
			if (m_CoroutineHelper != null) {
				m_CoroutineHelper.StopAllCoroutines();
			}
			
			if (ResourceAssetBundle != null) {
				ResourceAssetBundle.Release();
				ResourceAssetBundle = null;
			}
			
			if (SceneAssetBundle != null) {
				SceneAssetBundle.Release();
				SceneAssetBundle = null;
			}
			
		}
		
		#endregion
		
		
		#region Orientation
		
		[System.NonSerialized]
		private Queue<bool> m_ScreenAutoRotationQueue;
		
		[System.NonSerialized]
		private ScreenOrientation m_ScreenOrientation;
		
		private void ApplyScreenAutoRotationAndOrientation() {
			if (m_ScreenAutoRotationQueue != null && m_ScreenAutoRotationQueue.Count == 4) {
				Screen.autorotateToPortrait = m_ScreenAutoRotationQueue.Dequeue();
				Screen.autorotateToPortraitUpsideDown = m_ScreenAutoRotationQueue.Dequeue();
				Screen.autorotateToLandscapeLeft = m_ScreenAutoRotationQueue.Dequeue();
				Screen.autorotateToLandscapeRight = m_ScreenAutoRotationQueue.Dequeue();
				Screen.orientation = m_ScreenOrientation;
			}
		}
		
		private void ClearScreenAutoRotationAndOrientation() {
			m_ScreenAutoRotationQueue = null;
		}
		
		private void SaveScreenAutoRotationAndOrientation() {
			m_ScreenAutoRotationQueue = new Queue<bool>(4);
			m_ScreenAutoRotationQueue.Enqueue(Screen.autorotateToPortrait);
			m_ScreenAutoRotationQueue.Enqueue(Screen.autorotateToPortraitUpsideDown);
			m_ScreenAutoRotationQueue.Enqueue(Screen.autorotateToLandscapeLeft);
			m_ScreenAutoRotationQueue.Enqueue(Screen.autorotateToLandscapeRight);
			m_ScreenOrientation = Screen.orientation;
		}
		
		#endregion
		
		
		#region

		private IEnumerator DeactivateContent(ContentInfo contentInfo) {
			// event
			contentInfo.OnProjectNavigatorWillDeactivateContent();

			// reset the mesh loader (it may have operations left in the queue)
			if (MeshLoader.Instance) {
				MeshLoader.Instance.CancelAllLoadOperations();
				yield return null;
			}

			// unload the content resource bundle
			if (this.ResourceAssetBundle != null) {
				this.ResourceAssetBundle.Release();
				this.ResourceAssetBundle = null;
			}

			// unload the content scene bundle
			if (this.SceneAssetBundle != null) {
				this.SceneAssetBundle.Release();
				this.SceneAssetBundle = null;
			}

			// clear the content info
			if (this.ContentInfo != null) {
				this.ContentInfo = null;
			}

			// apply the default settings
			ApplyDefaultSettings();

			// unload content resources
			yield return Resources.UnloadUnusedAssets();

			// event
			contentInfo.OnProjectNavigatorDidDeactivateContent();

			yield return null;
		}

		private IEnumerator UnloadContentScenes(ContentInfo contentInfo) {
			// unloading scenes that may have been preloaded
			List<Scene> scenesToUnload = new List<Scene>();

			for (int i = 0; i < SceneManager.sceneCount; i++) {
				Scene scene = SceneManager.GetSceneAt(i);
				if (scene.path.StartsWith(contentInfo.SubmodulePath)) {
					scenesToUnload.Add(scene);
				}
			}

			foreach (Scene scene in scenesToUnload) {
				SceneReference sceneReference = SceneMap.GetSceneReference(scene.path);
				if (!string.IsNullOrEmpty(sceneReference.Guid)) {
					yield return SceneNavigator.Instance.UnloadScene(sceneReference, true);
				}
			}

			yield return null;
		}
		
		private IEnumerator NavigateToProjectImpl() {
			if (SceneNavigator.Instance) {
				
				// store content info in a local variable so that it can be used after the ContentInfo property is cleared
				ContentInfo contentInfo;
				contentInfo = this.ContentInfo;
				
				if (WillNavigateToProject != null) {
					WillNavigateToProject(contentInfo);
				}

				CreateLoadingSpinner();

				// go to the navigate to project scene
				SceneNavigator.Instance.NavigateToScene(
					ProjectInfo.Instance.NavigateToProjectScene,
					Instantiate(ProjectInfo.Instance.NavigateToProjectTransition),
					Instantiate(ProjectInfo.Instance.NavigateToProjectTransition),
					null,
					true,
					true
				);
				while (!SceneNavigator.Instance.IsReady) {
					yield return null;
				}
				
				yield return StartCoroutine(DeactivateContent(contentInfo));
				
				yield return StartCoroutine(UnloadContentScenes(contentInfo));
				
				// clear auto rotation and orientation
				ClearScreenAutoRotationAndOrientation();
				
				// load the home scene
				SceneNavigator.Instance.LoadScene(ProjectInfo.Instance.MainScene);
				while (!SceneNavigator.Instance.SceneIsLoaded(ProjectInfo.Instance.MainScene)) {
					yield return null;
				}
				
				// go to the home scene
				SceneNavigator.Instance.NavigateToScene(
					ProjectInfo.Instance.MainScene,
					Instantiate(ProjectInfo.Instance.NavigateToProjectTransition),
					Instantiate(ProjectInfo.Instance.NavigateToProjectTransition),
					null,
					true,
					true
				);
				while (!SceneNavigator.Instance.IsReady) {
					yield return null;
				}
				
				// set state
				this.State = ProjectNavigatorState.Project;
				
				if (DidNavigateToProject != null) {
					DidNavigateToProject(contentInfo);
				}
				
			}
		}
		
		private IEnumerator NavigateToContentImpl() {
			if (SceneNavigator.Instance) {
				
				if (WillNavigateToContent != null) {
					WillNavigateToContent(this.ContentInfo);
				}
				
				CreateLoadingSpinner(
					ProjectInfo.Instance.NavigateToContentLoadingSpinner,
					() => {
						return (int)(this.ProgressReport != null ? this.ProgressReport.TotalProgress * 100f : 0);
					});
				
				// go to the navigate to content scene
				SceneNavigator.Instance.NavigateToScene(
					ProjectInfo.Instance.NavigateToContentScene,
					Instantiate(ProjectInfo.Instance.NavigateToContentTransition),
					Instantiate(ProjectInfo.Instance.NavigateToContentTransition),
					null,
					true,
					true
				);
				while (SceneNavigator.Instance.IsBusy) {
					yield return null;
				}
				
				// unload the home scene
				yield return Resources.UnloadUnusedAssets();
				
				// save auto rotation and orientation settings
				SaveScreenAutoRotationAndOrientation();
				
				// event
				this.ContentInfo.OnProjectNavigatorWillActivateContent();

				// apply the content settings
				ApplyContentSettings(this.ContentInfo);

				#if !UNITY_EDITOR
				AssetBundleAdaptorType resourceContentAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(this.ContentInfo.ResourceAssetBundleName);
				AssetBundleAdaptorType sceneContentAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(this.ContentInfo.SceneAssetBundleName);

				if ((resourceContentAssetBundleAdaptorType == AssetBundleAdaptorType.OnDemandResources || resourceContentAssetBundleAdaptorType == AssetBundleAdaptorType.AssetBundleServer) ||
					(sceneContentAssetBundleAdaptorType == AssetBundleAdaptorType.OnDemandResources || sceneContentAssetBundleAdaptorType == AssetBundleAdaptorType.AssetBundleServer))
				{
					// Query to see if resources that we need to load are already downloaded and available
					using (var resourceQueryYieldInstruction = new ResourceQueryYieldInstruction(this, this.ContentInfo.ResourceAssetBundleName, this.ContentInfo.SceneAssetBundleName))
					{
						
						Debug.Log("ProjectNavigator-> Querying For Content Resource Availability.", DebugContext.SagoApp);
						yield return resourceQueryYieldInstruction;

						// If resource and scene are not already downloaded and available we need to test internet reachability before before start downloading.
						if (!resourceQueryYieldInstruction.IsResourceAvailable)
						{
							// Testing internet reachability and stop loading ODR if there is any error.
							using (InternetReachabilityYieldInstruction reachabilityYieldInstruction = new InternetReachabilityYieldInstruction(this))
							{
								yield return reachabilityYieldInstruction;

								if (!reachabilityYieldInstruction.IsInternetReachable)
								{
									this.Error = reachabilityYieldInstruction.Error;
									NavigateToError(this.Error, "Internet is not reachable.");
									// Since internet is not reachable we need to stop all remaining processes.
									yield break;
								}
							}
						}
					}
				}
				#endif
					
				if (NavigateToContentWillLoadAssetBundles != null) {
					NavigateToContentWillLoadAssetBundles(this.ContentInfo);
				}
				
				// load the content asset bundles
				if (AssetBundleOptions.UseAssetBundlesInEditor) {
					
					Debug.LogFormat(DebugContext.SagoApp, "ProjectNavigator-> Finding or creating reference to asset bundle: {0}", this.ContentInfo.ResourceAssetBundleName);
					this.ResourceAssetBundle = AssetBundleReference.FindOrCreateReference(this.ContentInfo.ResourceAssetBundleName);
					this.ResourceAssetBundle.Retain();
					
					this.ProgressReport.Index = 0;
					this.ProgressReport.Count = 3;
					this.ProgressReport.Item = new LoadAssetBundleProgressReportItem(this.ResourceAssetBundle);
					yield return this.ResourceAssetBundle;
					
					Debug.LogFormat(DebugContext.SagoApp, "ProjectNavigator-> Completed finding or creating reference to asset bundle: {0}", this.ContentInfo.ResourceAssetBundleName);
					if (!string.IsNullOrEmpty(this.ResourceAssetBundle.error)) {
						Debug.LogError(this.ResourceAssetBundle.error, DebugContext.SagoApp);
						if (string.Equals(this.ResourceAssetBundle.error, LowDiskSpaceAssetBundleAdaptor.LowDiskSpaceError)) {
							this.Error = ProjectNavigatorError.LowDiskSpace;
						} else if (string.Equals(this.ResourceAssetBundle.error, AssetBundleAdaptorError.NoInternet)) {
							this.Error = ProjectNavigatorError.OdrErrorNoInternet;
						} else {
							this.Error = ProjectNavigatorError.OdrErrorUnknown;
						}
						if (NavigateToContentDidLoadAssetBundles != null) {
							NavigateToContentDidLoadAssetBundles(this.ContentInfo, false);
						}
						NavigateToError(this.Error, this.ResourceAssetBundle.error);
						yield break;
					}

					Debug.LogFormat(DebugContext.SagoApp, "ProjectNavigator-> Finding or creating reference to asset bundle: {0}", this.ContentInfo.SceneAssetBundleName);
					this.SceneAssetBundle = AssetBundleReference.FindOrCreateReference(this.ContentInfo.SceneAssetBundleName);
					this.SceneAssetBundle.Retain();
					
					this.ProgressReport.Index = 1;
					this.ProgressReport.Item = new LoadAssetBundleProgressReportItem(this.SceneAssetBundle);
					yield return this.SceneAssetBundle;

					Debug.LogFormat(DebugContext.SagoApp, "ProjectNavigator-> Completed finding or creating reference to asset bundle: {0}", this.ContentInfo.SceneAssetBundleName);
					if (!string.IsNullOrEmpty(this.SceneAssetBundle.error)) {
						Debug.LogError(this.SceneAssetBundle.error, DebugContext.SagoApp);
						if (string.Equals(this.SceneAssetBundle.error, LowDiskSpaceAssetBundleAdaptor.LowDiskSpaceError)) {
							this.Error = ProjectNavigatorError.LowDiskSpace;
						} else if (string.Equals(this.SceneAssetBundle.error, AssetBundleAdaptorError.NoInternet)) {
							this.Error = ProjectNavigatorError.OdrErrorNoInternet;
						} else {
							this.Error = ProjectNavigatorError.OdrErrorUnknown;
						}
						if (NavigateToContentDidLoadAssetBundles != null) {
							NavigateToContentDidLoadAssetBundles(this.ContentInfo, false);
						}
						NavigateToError(this.Error, this.SceneAssetBundle.error);
						yield break;
					}
					
				}
					
				if (NavigateToContentDidLoadAssetBundles != null) {
					NavigateToContentDidLoadAssetBundles(this.ContentInfo, true);
				}
				
				// event
				this.ContentInfo.OnProjectNavigatorDidActivateContent();
				
				if (NavigateToContentWillLoadMainScene != null) {
					NavigateToContentWillLoadMainScene(this.ContentInfo);
				}
				
				// load the initial scene
				SceneNavigator.Instance.LoadScene(this.ContentInfo.MainScene);
				this.ProgressReport.Index = 2;
				this.ProgressReport.Item = new LoadSceneProgressReportItem(this.ContentInfo.MainScene);
				while (!SceneNavigator.Instance.SceneIsLoaded(this.ContentInfo.MainScene)) {
					yield return null;
				}
				
				if (NavigateToContentDidLoadMainScene != null) {
					NavigateToContentDidLoadMainScene(this.ContentInfo, true);
				}
				
				// go to the initial scene
				SceneNavigator.Instance.NavigateToScene(
					this.ContentInfo.MainScene,
					Instantiate(ProjectInfo.Instance.NavigateToContentTransition),
					Instantiate(ProjectInfo.Instance.NavigateToContentTransition),
					null,
					true,
					true
				);
				while (!SceneNavigator.Instance.IsReady) {
					yield return null;
				}

				this.ProgressReport.Reset();
				
				// set state
				this.State = ProjectNavigatorState.Content;
				
				if (DidNavigateToContent != null) {
					DidNavigateToContent(this.ContentInfo);
				}
				
			}
		}

		/// <summary>
		/// Coroutine implementation for navigating to error scene.
		/// </summary>
		/// <returns>IEnumerator</returns>
		/// <param name="errorType">This is meant for errors that we know of and defined in some form of data like enum types.</param>
		/// <param name="errorMessage">This is a raw string in the case of any errors that we do not know of.</param>
		private IEnumerator NavigateToErrorImpl(ProjectNavigatorError errorType, string errorMessage) {
			// Shouldn't call deactivate content here since error is being treated as a medium that
			// sits between project navigation and content navigation, and project navigation logic takes
			// care of this aspect; so if we try to call deactivate content here then any navigation logic
			// that uses content info (in this case, navigating to project) will make the app to crash.
			
			if (WillNavigateToError != null) {
				WillNavigateToError(this.ContentInfo, errorType.ToString(), errorMessage);
			}

			// After we've handled unloading unnecessary resources, we can now navigate to error scene
			SceneNavigator.Instance.NavigateToScene(
				ProjectInfo.Instance.NavigateToErrorScene,
				Instantiate(ProjectInfo.Instance.NavigateToErrorTransition),
				Instantiate(ProjectInfo.Instance.NavigateToErrorTransition),
				null,
				true,
				true
			);

			while (!SceneNavigator.Instance.IsReady) {
				yield return null;
			}
			
			// apply auto rotation and orientation
			ApplyScreenAutoRotationAndOrientation();
			
			this.State = ProjectNavigatorState.Error;
			
			if (DidNavigateToError != null) {
				DidNavigateToError(this.ContentInfo, errorType.ToString(), errorMessage);
			}
		}

		/// <summary>
		/// Creates a default loading spinner, and then modifies it for the
		/// ProjectNavigator's purposes.  It should not be passed to the
		/// SceneNavigator; it will directly register for the events so that
		/// it can control its own visibility.  Specifically, it stays
		/// visible until the second 'will in' transition.
		/// </summary>
		/// <returns>The loading spinner.</returns>
		private LoadingSpinner CreateLoadingSpinner(GameObject loadingSpinnerPrefab = null, System.Func<int> updateProgressImpl = null) {

			LoadingSpinner loadingSpinner = null;
			if (loadingSpinnerPrefab) {
				GameObject go = Instantiate(loadingSpinnerPrefab);
				go.SetActive(false);
				loadingSpinner = go.GetComponentInChildren<LoadingSpinner>();
				if (!loadingSpinner) {
					Destroy(go);
				}
			}
			loadingSpinner = loadingSpinner ?? LoadingSpinner.Create();
			loadingSpinner.transform.parent = this.transform;
			foreach (var layout in loadingSpinner.GetComponentsInChildren<SagoLayout.LayoutComponent>()) {
				layout.ApplyOnUpdate = true;
			}

			if (updateProgressImpl != null) {
				loadingSpinner.IsProgressTextEnabled = true;
				loadingSpinner.UpdateProgressImpl += updateProgressImpl;
			} else {
				loadingSpinner.IsProgressTextEnabled = false;
			}

			bool allowShutdown = false;

			System.Action<SceneController, SceneTransition> onDidOut = null;
			onDidOut = (x,y) => {
					loadingSpinner.gameObject.SetActive(true);
					SceneNavigator.Instance.OnSceneDidTransitionOut -= onDidOut;
				};
			SceneNavigator.Instance.OnSceneDidTransitionOut += onDidOut;

			System.Action<SceneController, SceneTransition> onWillIn = null;
			onWillIn = (x,y) => {
					if (loadingSpinner && allowShutdown) {
						Destroy(loadingSpinner.gameObject);
						SceneNavigator.Instance.OnSceneWillTransitionIn -= onWillIn;
					}
				};
			SceneNavigator.Instance.OnSceneWillTransitionIn += onWillIn;

			System.Action<SceneController, SceneTransition> onDidIn = null;
			onDidIn = (x,y) => {
					allowShutdown = true;
					SceneNavigator.Instance.OnSceneDidTransitionIn -= onDidIn;
				};
			SceneNavigator.Instance.OnSceneDidTransitionIn += onDidIn;

			return loadingSpinner;
		}


		#endregion
		
		
		
		
	}
	
}
