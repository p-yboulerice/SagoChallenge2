namespace SagoNavigation {
	
	using SagoCore.Scenes;
	using UnityEngine;
	
	abstract public class SceneController : MonoBehaviour, ISceneTransitionObserver {	
		
		
		//
		// Properties
		//
		public GameObject Content {
			get { return this.gameObject; }
		}

		virtual public bool IsReady {
			get { return true; }
		}

		public SceneReference Scene {
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
		virtual protected void Awake() {
			RegisterWithSceneNavigator();
		}
		
		
		//
		// Messages
		//
		virtual public void OnWillBeDestroyed() {
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
		}


		//
		// SceneNavigator Registration
		//
		protected void RegisterWithSceneNavigator() {
			if (SceneNavigator.Instance) {
				SceneNavigator.Instance.RegisterSceneController(this);
			}
		}


	}
	
}
