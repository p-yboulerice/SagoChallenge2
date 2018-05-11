namespace SagoNavigation {
	
	using UnityEngine;
	
	abstract public class LoadingIndicator : MonoBehaviour, ISceneTransitionObserver {


		//
		// Properties
		//
		public float LoadProgress {
			get { return SceneNavigator.Instance.TargetSceneLoadProgress; }
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
		private Transform m_Transform;
		private float m_LoadProgress;
		

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
