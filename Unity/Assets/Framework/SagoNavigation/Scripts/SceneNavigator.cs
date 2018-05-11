namespace SagoNavigation {
	
	using SagoCore.Scenes;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	public class SceneNavigator : MonoBehaviour {
		
		
		//
		// Singleton
		//
		static public SceneNavigator Instance {
            get {
				s_Instance = s_Instance ?? CreateInstance();
				return s_Instance;
			}
		}
		
		static private SceneNavigator CreateInstance() {
			
			SceneNavigator instance;
			instance = null;
			
			if (!s_DidQuit) {
				instance = new GameObject().AddComponent<SceneNavigator>();
				instance.name = "SceneNavigator";
				DontDestroyOnLoad(instance);
			}
			
			return instance;
			
		}


		//
		// Static Variables
		//
		static private bool s_DidQuit;
		static private SceneNavigator s_Instance;
		
		
		//
		// States
		//
		private enum States {
			Ready,
			Busy
		}


		//
		// Delegates and Events
		//
		public delegate void SceneNavigatorEventHandler(SceneController sceneController);
		public event SceneNavigatorEventHandler OnWillNavigateToScene;
		public event SceneNavigatorEventHandler OnDidNavigateToScene;
		public event System.Action<SceneController, SceneTransition> OnSceneWillTransitionOut;
		public event System.Action<SceneController, SceneTransition> OnSceneDidTransitionOut;
		public event System.Action<SceneController, SceneTransition> OnSceneWillTransitionIn;
		public event System.Action<SceneController, SceneTransition> OnSceneDidTransitionIn;
		
		//
		// Properties
		//
		public SceneController CurrentSceneController {
			get {
				if (string.IsNullOrEmpty(this.CurrentScene.Guid)) return null;
				if (!this.SceneControllers.ContainsKey(this.CurrentScene)) return null;
				return this.SceneControllers[this.CurrentScene];
			}
		}
		
		public SceneReference CurrentScene {
			get;
			private set;
		}

		public bool IsBusy {
			get { return this.State == SceneNavigator.States.Busy; }
		}
		
		public bool IsReady {
			get { return this.State == SceneNavigator.States.Ready; }
		}

		public SceneReference PreviousScene {
			get;
			private set;
		}

		public float TargetSceneLoadProgress {
			get { return SceneLoadProgress(this.TargetScene); }
		}

		public SceneReference TargetScene {
			get;
			private set;
		}

		public Transform Transform {
			get {
				m_Transform = (m_Transform != null) ? m_Transform : GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		
		//
		// Private Properties
		//
		private AsyncOperation LoadOperation {
			get;
			set;
		}
		
		private SceneReference LoadOperationScene {
			get;
			set;
		}
		
		private List<SceneReference> LoadQueue {
			get {
				m_LoadQueue = (m_LoadQueue != null) ? m_LoadQueue : new List<SceneReference>();
				return m_LoadQueue;
			}
		}
		
		private Dictionary<SceneReference,SceneController> SceneControllers {
			get { return m_SceneControllers = m_SceneControllers ?? new Dictionary<SceneReference,SceneController>(); }
		}
		
		private States State {
			get;
			set;
		}

		
		//
		// Member Variables
		//
		private List<SceneReference> m_LoadQueue;
		private Dictionary<SceneReference,SceneController> m_SceneControllers;
		private Transform m_Transform;
		
		
		//
		// MonoBehaviour
		//
		void OnApplicationQuit() {
			SceneNavigator.s_DidQuit = true;	
		}
		
		IEnumerator Start() {
			while (enabled) {
				if (this.LoadQueue.Count > 0) {
					yield return StartCoroutine(RunLoadScene(ShiftLoadQueue()));
				} else {
					yield return null;
				}
			}
		}
		
		
		//
        // Public Methods
        //
		public void CancelSceneLoad(SceneReference sceneReference) {
			this.LoadQueue.Remove(sceneReference);
		}
		
		public void LoadScene(SceneReference sceneReference) {
			LoadScene(sceneReference, false);
		}
		
		public void LoadScene(SceneReference sceneReference, bool jumpQueue) {
			
			if (SceneIsLoaded(sceneReference)) {
				return;
			}
			
			if (SceneIsLoading(sceneReference)) {
				return;
			}
			
			if (jumpQueue) {
				this.LoadQueue.Remove(sceneReference);
				this.LoadQueue.Insert(0, sceneReference);
			} else if (!this.LoadQueue.Contains(sceneReference)) {
				this.LoadQueue.Add(sceneReference);
			}
			
		}
		
		/*
		public void NavigateToScene(string sceneName) {
			NavigateToScene(sceneName, null, true);
		}
		
		public void NavigateToScene(string sceneName, bool unloadCurrentScene) {
			NavigateToScene(sceneName, null, unloadCurrentScene);
		}
		
		public void NavigateToScene(string sceneName, LoadingIndicator progressIndicator) {
			NavigateToScene(sceneName, progressIndicator, true);
		}
		
		public void NavigateToScene(string sceneName, LoadingIndicator progressIndicator, bool unloadCurrentScene) {
			NavigateToScene(sceneName, FadeTransition.Create(), FadeTransition.Create(), progressIndicator, unloadCurrentScene);
		}
		
		public void NavigateToScene(string sceneName, SceneTransition transitionOut, SceneTransition transitionIn) {
			NavigateToScene(sceneName, transitionOut, transitionIn, null, true);
		}
		
		public void NavigateToScene(string sceneName, SceneTransition transitionOut, SceneTransition transitionIn, bool unloadCurrentScene) {
			NavigateToScene(sceneName, transitionOut, transitionIn, null, unloadCurrentScene);
		}
		
		public void NavigateToScene(string sceneName, SceneTransition transitionOut, SceneTransition transitionIn, LoadingIndicator progressIndicator) {
			NavigateToScene(sceneName, transitionOut, transitionIn, progressIndicator, true);
		}

		public void NavigateToScene(string sceneName, SceneTransition transitionOut, SceneTransition transitionIn, LoadingIndicator progressIndicator, bool unloadCurrentScene) {
			NavigateToScene(sceneName, transitionOut, transitionIn, progressIndicator, unloadCurrentScene, unloadCurrentScene);
		}
		*/
		
		public void NavigateToScene(
			SceneReference sceneReference, 
			SceneTransition transitionOut, 
			SceneTransition transitionIn, 
			LoadingIndicator progressIndicator, 
			bool unloadCurrentScene, 
			bool unloadUnusedAssets
		) {

			if (this.CurrentScene.Equals(sceneReference)) {
				DestroyNavigationElements(transitionOut, transitionIn, progressIndicator);
				Debug.LogWarning(string.Format(
					"SceneNavigator : NavigateToScene \"{0}\" aborted because it is the current scene.", 
					sceneReference.ScenePath
				));
				return;
			}

			if (this.IsBusy) {
				DestroyNavigationElements(transitionOut, transitionIn, progressIndicator);
				Debug.LogWarning(string.Format(
					"SceneNavigator : NavigateToScene \"{0}\" aborted because a navigation is already in progress.", 
					sceneReference.ScenePath
				));
				return;
			}
			
			StartCoroutine(RunNavigateToScene(
				sceneReference, 
				transitionOut, 
				transitionIn, 
				progressIndicator, 
				unloadCurrentScene, 
				unloadUnusedAssets
			));
			
		}
		
		public void RegisterSceneController(SceneController sceneController) {
			
			SceneReference sceneReference;
			sceneReference = SceneMap.GetSceneReference(SceneManager.GetActiveScene().path);
			
			RegisterSceneController(sceneController, sceneReference);
			
		}

		private void RegisterSceneController(SceneController sceneController, SceneReference sceneReference) {
			
			sceneController.Scene = string.IsNullOrEmpty(this.LoadOperationScene.Guid) ? sceneReference : this.LoadOperationScene;

			this.SceneControllers.Remove(sceneController.Scene);
			this.SceneControllers.Add(sceneController.Scene, sceneController);
			
			if (string.IsNullOrEmpty(this.CurrentScene.Guid)) {
				this.CurrentScene = sceneController.Scene;
				sceneController.OnSceneWillTransitionIn(sceneController, null);
				sceneController.OnSceneDidTransitionIn(sceneController, null);
			} else {
				sceneController.Content.SetActive(false);
			}
			
		}
		
		public bool SceneIsLoaded(SceneReference sceneReference) {
			return !string.IsNullOrEmpty(sceneReference.Guid) && this.SceneControllers.ContainsKey(sceneReference);
		}
		
		public bool SceneIsLoading(SceneReference sceneReference) {
			return this.LoadOperationScene.Equals(sceneReference);
		}
		
		public bool SceneIsQueuedForLoading(SceneReference sceneReference) {
			return this.LoadQueue.Contains(sceneReference);
		}

		public float SceneLoadProgress(SceneReference sceneReference) {
			if (SceneIsLoaded(sceneReference)) return 1;
			if (SceneIsLoading(sceneReference)) return this.LoadOperation.progress;
			return 0;
		}

        public YieldInstruction UnloadScene(SceneReference sceneReference) {
			return UnloadScene(sceneReference, true);
		}

        public YieldInstruction UnloadScene(SceneReference sceneReference, bool unloadUnusedAssets) {
            return StartCoroutine(UnloadSceneAsync(sceneReference, unloadUnusedAssets));
		}

        private IEnumerator UnloadSceneAsync(SceneReference sceneReference, bool unloadUnusedAssets) {

            if (SceneIsLoaded(sceneReference)) {

                SceneController sceneController;
                sceneController = this.SceneControllers[sceneReference];
                sceneController.OnWillBeDestroyed();

                this.SceneControllers.Remove(sceneReference);
                GameObject.Destroy(sceneController.Content);

                #if UNITY_EDITOR
                // TODO: this should also happen at runtime, but there's a bug in Unity 
                // 5.3.1 that unloads and destroys prefabs when you unload a scene when
                // the app is running on a device (yes, really).
                //
                // For now, we're destroying all the game objects anyway, so we can get 
                // away with just not unloading scene, but it's a hack. We should revisit
                // this code when Unity fixes the bug.
                //
                // The issue number is #758692
                // The issue url is: http://issuetracker.unity3d.com/issues/unloading-a-scene-can-destroy-non-instantiated-prefabs-that-are-indirectly-referred-to-by-other-currently-loaded-scene
                //
                //
                // Update – Jan 9, 2018
                //
                // UnloadScene is deprecated in Unity 2017.3, and has been replaced by UnloadSceneAsync.
                // UnloadSceneAsync has no way to check if the status of the unload operation, so we 
                // have to just brute force it and unload all each possible name/path.
                //
                // UnloadSceneAsync throws an argument exception if the name/path is not a valid scene,
                // so we have to guard it.

                string[] paths = {
                    sceneReference.ScenePath, // scene path
                    System.IO.Path.GetFileNameWithoutExtension(sceneReference.ScenePath) // scene name
                };

                foreach (string path in paths) {
                    YieldInstruction op = null;
                    try {
                        op = SceneManager.UnloadSceneAsync(path);
                    } catch (System.ArgumentException) {
                        op = null;
                    }
                    yield return op;
                }
                #endif

                if (this.CurrentScene.Equals(sceneReference)) {
                    this.CurrentScene = new SceneReference();
                }

            }

            if (unloadUnusedAssets) {
                yield return Resources.UnloadUnusedAssets();
            }

            yield break;

        }
        		
		
		//
		// Scene Loading
		//
		private IEnumerator AwaitSceneLoadComplete(SceneReference sceneReference) {
			while (!SceneIsLoaded(sceneReference)) {
				yield return null;
			}
		}

		private IEnumerator AwaitSceneReady(SceneController sceneController) {
			while (!sceneController.IsReady) {
				yield return null;
			}
		}

		private IEnumerator RunLoadScene(SceneReference sceneReference) {

			this.LoadOperation = SceneManager.LoadSceneAsync(sceneReference.ScenePath, LoadSceneMode.Additive);
			this.LoadOperationScene = sceneReference;
			
			while (!this.LoadOperation.isDone) {
				yield return null;
			}
			
			this.LoadOperation = null;
			this.LoadOperationScene = new SceneReference();

		}
		
		private SceneReference ShiftLoadQueue() {
			
			SceneReference result;
			result = new SceneReference();
			
			if (this.LoadQueue.Count > 0) {
				result = this.LoadQueue[0];
				this.LoadQueue.RemoveAt(0);
			}
			
			return result;
			
		}
		
		
		//
		// Scene Navigation
		//
		private IEnumerator RunNavigateToScene(
			SceneReference sceneReference, 
			SceneTransition transitionOut, 
			SceneTransition transitionIn, 
			LoadingIndicator loader, 
			bool unloadCurrentScene, 
			bool unloadUnusedAssets
		) {
			
			this.State = SceneNavigator.States.Busy;
			this.TargetScene = sceneReference;

			LoadScene(this.TargetScene, true);

			transitionIn = SetupTransition(transitionIn, SceneTransition.Directions.In);
			transitionOut = SetupTransition(transitionOut, SceneTransition.Directions.Out);
			loader = SetupLoadingIndicator(loader);
			
			SceneController prevSceneController;
			prevSceneController = this.CurrentSceneController;
			yield return StartCoroutine(RunTransitionOut(prevSceneController, transitionOut, transitionIn, loader));
			yield return StartCoroutine(AwaitSceneLoadComplete(sceneReference));
			this.PreviousScene = this.CurrentScene;
			this.CurrentScene = this.TargetScene;
			
			SceneController nextSceneController;
			nextSceneController = this.CurrentSceneController;

			if (nextSceneController == null) {
				throw new System.InvalidOperationException(string.Format(
					"SceneNavigator : Scene \"{0}\" was loaded but did not register with SceneNavigator.", 
					sceneReference.ScenePath
				));
			}

			// April 7, 2016
			//
			// We're moving scene unloading from after the next scene transitions in to between the transition in 
			// and transition out. This solves some issues with load times caused by UnloadUnusedAssets() fighting 
			// with things being loaded by the next scene.
			//
			// IMPORTANT: Moving the unloading prevents us from creating transitions where the scenes 
			// overlap (like cross-fading between scenes), which should be find because we've never 
			// actually implemented a transition that requires both scenes to exist.

			if (prevSceneController && unloadCurrentScene) {
				yield return UnloadScene(prevSceneController.Scene, unloadUnusedAssets);
			}

			if (this.OnWillNavigateToScene != null) {
				this.OnWillNavigateToScene(nextSceneController);
			}

			nextSceneController.Content.SetActive(true);
			yield return StartCoroutine(AwaitSceneReady(nextSceneController));
			SceneManager.SetActiveScene(nextSceneController.gameObject.scene);
			yield return StartCoroutine(RunTransitionIn(nextSceneController, transitionOut, transitionIn, loader));
			
			if (this.OnDidNavigateToScene != null) {
				this.OnDidNavigateToScene(nextSceneController);
			}
			
		}
		
		private IEnumerator RunTransitionIn(
			SceneController sceneController, 
			SceneTransition transitionOut, 
			SceneTransition transitionIn, 
			LoadingIndicator loader
		) {
			
			transitionIn.OnSceneWillTransitionIn(sceneController, transitionIn);
			transitionOut.OnSceneWillTransitionIn(sceneController, transitionIn);
			loader.OnSceneWillTransitionIn(sceneController, transitionIn);

			if (sceneController) {
				sceneController.OnSceneWillTransitionIn(sceneController, transitionIn);
			}

			if (this.OnSceneWillTransitionIn != null) {
				this.OnSceneWillTransitionIn(sceneController, transitionIn);
			}

			yield return StartCoroutine(transitionIn.Run());

			this.State = SceneNavigator.States.Ready;

			if (this.OnSceneDidTransitionIn != null) {
				this.OnSceneDidTransitionIn(sceneController, transitionIn);
			}

			if (sceneController) {
				sceneController.OnSceneDidTransitionIn(sceneController, transitionIn);
			}

			loader.OnSceneDidTransitionIn(sceneController, transitionIn);
			transitionOut.OnSceneDidTransitionIn(sceneController, transitionIn);
			transitionIn.OnSceneDidTransitionIn(sceneController, transitionIn);

		}
		
		private IEnumerator RunTransitionOut(
			SceneController sceneController, 
			SceneTransition transitionOut, 
			SceneTransition transitionIn, 
			LoadingIndicator loader
		) {
			
			transitionOut.OnSceneWillTransitionOut(sceneController, transitionOut);
			transitionIn.OnSceneWillTransitionOut(sceneController, transitionOut);
			loader.OnSceneWillTransitionOut(sceneController, transitionOut);

			if (sceneController) {
				sceneController.OnSceneWillTransitionOut(sceneController, transitionOut);
			}

			if (this.OnSceneWillTransitionOut != null) {
				this.OnSceneWillTransitionOut(sceneController, transitionOut);
			}

			yield return StartCoroutine(transitionOut.Run());

			if (this.OnSceneDidTransitionOut != null) {
				this.OnSceneDidTransitionOut(sceneController, transitionOut);
			}

			if (sceneController) {
				sceneController.OnSceneDidTransitionOut(sceneController, transitionOut);
			}

			loader.OnSceneDidTransitionOut(sceneController, transitionOut);
			transitionIn.OnSceneDidTransitionOut(sceneController, transitionOut);
			transitionOut.OnSceneDidTransitionOut(sceneController, transitionOut);
			
		}
		
		private SceneTransition SetupTransition(SceneTransition transition, SceneTransition.Directions direction) {
			transition = (transition == null) ? EmptyTransition.Create() : transition;
			transition.Direction = direction;
			transition.gameObject.name += direction.ToString();
			transition.gameObject.SetActive(false);
			transition.Transform.parent = this.Transform;
			return transition;
		}
		
		private LoadingIndicator SetupLoadingIndicator(LoadingIndicator loader) {
			loader = (loader == null) ? EmptyLoadProgressIndicator.Create() : loader;
			loader.gameObject.SetActive(false);
			loader.Transform.parent = this.Transform;
			return loader;
		}

		private void DestroyNavigationElements(SceneTransition transitionOut, SceneTransition transitionIn, LoadingIndicator progressIndicator) {
			if (transitionOut) {
				GameObject.Destroy(transitionOut.gameObject);
				transitionOut = null;
			}
			if (transitionIn) {
				GameObject.Destroy(transitionIn.gameObject);
				transitionIn = null;
			}
			if (progressIndicator) {
				GameObject.Destroy(progressIndicator.gameObject);
				progressIndicator = null;
			}
		}
		
	}
	
}
