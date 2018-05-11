namespace SagoNavigation {

	using SagoUtils;
	using System.Collections;
	using UnityEngine;
	
	abstract public class SceneTransition : MonoBehaviour, ISceneTransitionObserver {


		//
		// Factory
		//
		static public SceneTransition Create(string resourcePath) {
			
			GameObject resource;
			resource = Resources.Load(resourcePath, typeof(GameObject)) as GameObject;
			
			GameObject gameObject;
			gameObject = Instantiate(resource) as GameObject;
			gameObject.name = resource.name;
			gameObject.SetActive(false);
			
			SceneTransition transition;
			transition = gameObject.GetComponent<SceneTransition>();
			
			return transition;
			
		}


		//
		// Fade Directions
		//
		public enum Directions {
			In,
			Out
		}


		//
		// Inspector Properties
		//
		[SerializeField]
		[Range(0, 100)]
		public int CameraDepth;

		[SerializeField]
		[Range(0, 5)]
		public float Duration;

		[SerializeField]
		[LayerAttribute]
		public int Layer;

		
		//
		// Properties
        //
		public Directions Direction {
			get;
			set;
		}

		public Transform Transform {
			get {
				m_Transform = (m_Transform != null) ? m_Transform : this.transform;
				return m_Transform;
			}
		}
		
		
		//
		// Member Variables
		//
		protected Transform m_Transform;
		

		//
		// MonoBehaviour
		//
		virtual protected void Reset() {
			this.CameraDepth = 100;
			this.Duration = 0.5f;
			this.Layer = 1;
		}

		virtual protected void Start() {
		}


		//
		// Public Methods
		//
		virtual public IEnumerator Run() {
			yield return null;
		}
		
		
		//
		// ISceneTransitionObserver
		//
		virtual public void OnSceneWillTransitionOut(SceneController sceneController, SceneTransition transition) {
		}

		virtual public void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition) {
		}

		virtual public void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition) {
		}

		virtual public void OnSceneDidTransitionIn(SceneController sceneController, SceneTransition transition) {
			Destroy(this.gameObject);
		}


	}
	
}
