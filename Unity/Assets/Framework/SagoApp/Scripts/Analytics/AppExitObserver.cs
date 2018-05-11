namespace SagoApp.Analytics {
	
	using SagoApp.Project;
	using SagoNavigation;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;

	public class AppExitObserver : MonoBehaviour, IAnalyticsObserver {


		#region Serialized Fields

		[Range(-100, 100)]
		[SerializeField]
		private int m_Priority;

		#endregion


		#region Properties

		public string Key {
			get { return "Scene"; }
		}

		public int Priority {
			get { return m_Priority; }
			set { m_Priority = value; }
		}

		#endregion


		#region Methods

		public void OnTrackEvent(string eventName, Dictionary<string,object> eventInfo) {
			if (eventName == AnalyticsController.APP_EXIT) {
				if (SceneNavigator.Instance) {
					
					// Standalone apps track the scene name (there's existing data for those 
					// apps and we want to make sure the values we're passing match the 
					// existing ones). We're assuming that there are no duplicate scene names 
					// in standalone apps.
					
					// Composite apps, like the world app, track the scene path (there's no
					// existing data, and we know there will be duplicate scene names).
					
					string scenePath;
					scenePath = SceneNavigator.Instance.CurrentScene.AssetPath;
					
					string sceneName;
					sceneName = Path.GetFileNameWithoutExtension(scenePath);
					
					string value;
					value = ProjectInfo.Instance.IsStandaloneProject ? sceneName : scenePath;
					
					eventInfo[this.Key] = value;
					
				}
			}
		}

		#endregion


	}

}
