namespace SagoApp {

	using SagoMesh;
	using SagoNavigation;
	using SagoCore.Resources;
	using UnityEngine;
	using UnityEngine.UI;

	public class LoadingSpinner : LoadingIndicator {


		#region Progress update callback

		public System.Func<int> UpdateProgressImpl = null;

		#endregion


		//
		// Factory
		//
		static public LoadingSpinner Create() {
			
			// guid: SagoApp/Resources/SagoApp/LoadingSpinner.prefab
			
			ResourceReference reference;
			reference = ScriptableObject.CreateInstance<ResourceReference>();
			reference.Guid = "b83dfc0a5ad994b9d9f343a791779993";
			
			ResourceReferenceLoaderRequest request;
			request = ResourceReferenceLoader.Load(reference, typeof(GameObject));
			
			if (!string.IsNullOrEmpty(request.error)) {
				Debug.LogError(request.error);
				return null;
			}
			
			GameObject gameObject;
			gameObject = Instantiate(request.asset) as GameObject;
			gameObject.name = request.asset.name;
			gameObject.SetActive(false);
			
			LoadingIndicator loader;
			loader = gameObject.GetComponent<LoadingIndicator>();
			
			return loader as LoadingSpinner;
			
		}


		//
		// Inspector Properties
		//
		[HideInInspector]
		[SerializeField]
		[Range(0, 1)]
		public float SkipIfProgressIsBeyond;


		//
		// Properties
		//
		public MeshAnimator Animator {
			get {
				m_Animator = m_Animator ? m_Animator : GetComponentInChildren<MeshAnimator>();
				return m_Animator;
			}
		}


		//
		// Member Variables
		//
		private MeshAnimator m_Animator;


		/// <summary>
		/// Unity's Text UI.
		/// </summary>
		[System.NonSerialized]
		private Text m_Text;


		protected Text ProgressText {
			get { return m_Text = m_Text ?? GetComponentInChildren<Text>(); }
		}


		/// <summary>
		/// Variable updates enabled status of Text UI only.
		/// </summary>
		public bool IsProgressTextEnabled {
			set {
				if (ProgressText != null) {
					ProgressText.enabled = value;
				}
			}

			get {
				return ProgressText != null ? ProgressText.enabled : false;
			}
		}


		//
		// MonoBehaviour
		//
		private void Reset() {
			this.SkipIfProgressIsBeyond = 0.66f;
		}


		//
		// ISceneTransitionObserver
		//
		override public void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition) {
			
			base.OnSceneDidTransitionOut(sceneController, transition);

			// April 7, 2016
			// We're changing the logic here to always show the loading spinner. In recent versions of Unity (5.3.4), 
			// the value of LoadProgress is unreliable. The value comes from Unity's AsyncOperation class and is 
			// outside of our control. It's better to show the spinner when it's not necessary, than to leave the 
			// user with a blank screen.
			this.gameObject.SetActive(true); //this.LoadProgress <= this.SkipIfProgressIsBeyond);

			if (this.Animator && this.Animator.gameObject.activeInHierarchy) {
				this.Animator.Play();
			}

		}
		
		override public void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition) {
			base.OnSceneWillTransitionIn(sceneController, transition);
			this.gameObject.SetActive(false);
		}


		void Update() {
			if (this.ProgressText != null && this.UpdateProgressImpl != null) {
				this.ProgressText.text = this.UpdateProgressImpl().ToString() + "%";
			}
		}
	}

}
