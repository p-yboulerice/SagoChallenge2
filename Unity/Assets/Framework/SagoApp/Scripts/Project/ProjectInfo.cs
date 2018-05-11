namespace SagoApp.Project {
	
	using SagoApp.Content;
	using SagoNavigation;
	using SagoCore.Scenes;
	using System.Linq;
	using UnityEngine;
	
	public class ProjectInfo : ScriptableObject {
		
		
		#region Singleton
		
		public static ProjectInfo Instance {
			get {
				ProjectInfo instance;
				instance = Resources.Load("ProjectInfo", typeof(ProjectInfo)) as ProjectInfo;
				return instance;
			}
		}
		
		#endregion
		
		
		#region Serialized Fields
		
		[SerializeField]
		private ContentInfo[] m_ContentInfo;
		
		[SerializeField]
		private SceneReference m_MainScene;
		
		[SerializeField]
		private SceneReference m_NavigateToContentScene;
		
		[SerializeField]
		private SceneTransition m_NavigateToContentTransition;
		
		[SerializeField]
		private GameObject m_NavigateToContentLoadingSpinner;
		
		[SerializeField]
		private SceneReference m_NavigateToProjectScene;
		
		[SerializeField]
		private SceneTransition m_NavigateToProjectTransition;

		[SerializeField]
		private SceneReference m_NavigateToErrorScene;

		[SerializeField]
		private SceneTransition m_NavigateToErrorTransition;
		
		#endregion
		
		
		#region Properties
		
		public ContentInfo[] ContentInfo {
			get { return m_ContentInfo; }
		}
		
		public SceneReference MainScene {
			get { return m_MainScene; }
		}
		
		public GameObject NavigateToContentLoadingSpinner {
			get { return m_NavigateToContentLoadingSpinner; }
		}
		
		public SceneReference NavigateToContentScene {
			get { return m_NavigateToContentScene; }
		}

		public SceneTransition NavigateToContentTransition {
			get { return m_NavigateToContentTransition; }
		}
		
		public SceneReference NavigateToProjectScene {
			get { return m_NavigateToProjectScene; }
		}
		
		public SceneTransition NavigateToProjectTransition {
			get { return m_NavigateToProjectTransition; }
		}

		public SceneReference NavigateToErrorScene {
			get { return m_NavigateToErrorScene; }
		}

		public SceneTransition NavigateToErrorTransition {
			get { return m_NavigateToErrorTransition; }
		}
		
		public bool IsCompositeProject {
			get { return m_ContentInfo != null && m_ContentInfo.Length > 1; }
		}
		
		public bool IsStandaloneProject {
			get { return !IsCompositeProject; }
		}
		
		#endregion
		
		
		#region Methods
		
		public ContentInfo GetContentInfo<T>() where T : ContentInfo {
			return GetContentInfo(typeof(T));
		}
		
		public ContentInfo GetContentInfo(System.Type type) {
			
			int index;
			index = IndexOf(type);
			
			if (index != -1 && index < m_ContentInfo.Length) {
				return m_ContentInfo[index];
			}
			
			return null;
			
		}
		
		private int IndexOf(System.Type type) {
			return (
				m_ContentInfo != null ? 
				System.Array.IndexOf(m_ContentInfo.Select(info => info.GetType()).ToArray(), type) : 
				-1
			);
		}
		
		private void Reset() {
			m_ContentInfo = new ContentInfo[0];
		}
		
		#endregion
		
		
	}
	
}