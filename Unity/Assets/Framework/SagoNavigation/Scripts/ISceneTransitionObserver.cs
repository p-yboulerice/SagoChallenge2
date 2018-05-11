namespace SagoNavigation {	
	
	public interface ISceneTransitionObserver {
		void OnSceneWillTransitionOut(SceneController sceneController, SceneTransition transition);
		void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition);
		void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition);
		void OnSceneDidTransitionIn(SceneController sceneController, SceneTransition transition);
	}
	
}