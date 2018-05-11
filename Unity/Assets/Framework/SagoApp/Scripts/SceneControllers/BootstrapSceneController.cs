namespace SagoApp {
	
	using SagoApp.Analytics;
	using SagoCore.Scenes;
	using SagoNavigation;
	using UnityEngine;
	
	public class BootstrapSceneController : SceneController {
		
		
		#region Fields
		
		[SerializeField]
		private SceneReference m_NextScene;
		
		#endregion
		
		
		#region Properties
		
		public SceneReference NextScene {
			get { return m_NextScene; }
			set { m_NextScene = value; }
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		virtual protected void OnApplicationStart() {
			Application.targetFrameRate = 60;
			AnalyticsController.Init();
			AudioSessionUtil.ConfigureAudioSession();
			ImmersiveModeUtil.RegisterWithSceneNavigator();
			ImmersiveModeUtil.ActivateImmersiveMode();
			SagoBiz.Facade.OnApplicationStart();
		}
		
		virtual protected void Reset() {
			this.NextScene = SceneMapEditorAdaptor.GetSceneReference("Assets/Project/Scenes/Intro.unity");
		}
		
		virtual protected void Start() {
			
			OnApplicationStart();
			
			if (BackButtonController.Instance) {
				BackButtonController.Instance.ShouldEnable = true;
			}
			
			if (SceneNavigator.Instance) {
				SceneNavigator.Instance.NavigateToScene(this.NextScene, null, null, null, true, true);
			}
			
		}
		
		#endregion
		
		
	}

}
