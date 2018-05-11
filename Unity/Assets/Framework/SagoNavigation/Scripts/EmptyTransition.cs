namespace SagoNavigation {
	
	using UnityEngine;
	
	public class EmptyTransition : SceneTransition {
		
		
		//
		// Factory
		//
		static public EmptyTransition Create() {
			EmptyTransition transition;
			transition = new GameObject("EmptyTransition").AddComponent<EmptyTransition>();
			return transition;
		}
		
		
		//
		// PreviousSceneController;
		//
		private SceneController PreviousSceneController {
			get;
			set;
		}
		
		
		//
		// ISceneTransitionObserver
		//
		override public void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition) {
			if (this.Direction == SceneTransition.Directions.In) {
				gameObject.SetActive(true);
				if (sceneController) {
					this.PreviousSceneController = sceneController;
				}
			}
		}
		
		override public void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition) {
			if (this.Direction == SceneTransition.Directions.In) {
				if (sceneController) {
					sceneController.Content.SetActive(true);
				}
				if (this.PreviousSceneController) {
					this.PreviousSceneController.Content.SetActive(false);
				}
			} else {
				gameObject.SetActive(false);
			}
		}


	}
	
}
