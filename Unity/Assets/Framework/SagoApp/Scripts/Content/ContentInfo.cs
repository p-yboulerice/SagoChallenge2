namespace SagoApp.Content {
	
	using SagoApp.Project;
	using SagoCore.Scenes;
	using UnityEngine;
	
	/// <summary>
	/// The ContentInfo class contains metadata about a content submodule.
	/// </summary>
	
	/// <remarks>
	/// <para>
	/// Each content submodule must implement it's own subclass. The script for 
	/// the subclass must be located in <c>{submodule_path}/Scripts/ContentInfo.cs</c> 
	/// (because the location is used determine the submodule path programatically).
	/// </para>
	/// <code>
	/// namespace MyContentSubmodule {
	/// 	sealed public class ContentInfo : SagoApp.Content.ContentInfo {
	/// 		
	/// 	}
	/// }
	/// </code>
	/// <para>
	/// Each content submodule will have a singleton <see cref="ContentInfo" /> asset, 
	/// which must be located in <c>{submodule_path}/Resources/{submodule_name}/ContentInfo.asset</c> 
	/// (so that it is accessible at runtime via <see cref="Resources.Load" />).
	/// </para>
	/// </remarks>
	
	public abstract class ContentInfo : SagoCore.Submodules.SubmoduleInfo {
		
		
		#region Serialized Fields
		
		[Header("Asset Bundles")]
		
		/// <summary>
		/// The name of the resource asset bundle.
		/// </summary>
		[SerializeField]
		private string m_ResourceAssetBundleName;
		
		/// <summary>
		/// The name of the scene asset bundle.
		/// </summary>
		[SerializeField]
		private string m_SceneAssetBundleName;
		
		
		[Header("Scenes")]
		
		/// <summary>
		/// The initial scene.
		/// </summary>
		[SerializeField]
		private SceneReference m_MainScene;
		
		
		[Header("Settings")]
		
		/// <summary>
		/// The 3D physics settings object.
		/// </summary>
		[SerializeField]
		private DynamicsManager m_DynamicsManager;
		
		/// <summary>
		/// The graphics settings object.
		/// </summary>
		[SerializeField]
		private GraphicsSettings m_GraphicsSettings;
		
		/// <summary>
		/// The 2d physics settings object.
		/// </summary>
		[SerializeField]
		private Physics2DSettings m_Physics2DSettings;
		
		/// <summary>
		/// The layer and tag settings object.
		/// </summary>
		[SerializeField]
		private TagManager m_TagManager;
		
		/// <summary>
		/// The time settings object.
		/// </summary>
		[SerializeField]
		private TimeManager m_TimeManager;
		
		#endregion
		
		
		#region Properties - Asset Bundles
		
		/// <summary>
		/// Gets the name of the resource asset bundle.
		/// </summary>
		public string ResourceAssetBundleName {
			get { return m_ResourceAssetBundleName; }
		}
		
		/// <summary>
		/// Gets the name of the scene asset bundle.
		/// </summary>
		public string SceneAssetBundleName {
			get { return m_SceneAssetBundleName; }
		}
		
		#endregion
		
		
		#region Properties - Scenes
		
		/// <summary>
		/// Gets the initial scene.
		/// </summary>
		public SceneReference MainScene {
			get { return m_MainScene; }
		}
		
		#endregion
		
		
		#region Properties - Settings
		
		/// <summary>
		/// Gets the 3D physics settings object.
		/// </summary>
		public DynamicsManager DynamicsManager {
			get { return m_DynamicsManager; }
		}
		
		/// <summary>
		/// Gets the graphics settings object.
		/// </summary>
		public GraphicsSettings GraphicsSettings {
			get { return m_GraphicsSettings; }
		}
		
		/// <summary>
		/// Gets the 2D physics settings object.
		/// </summary>
		public Physics2DSettings Physics2DSettings {
			get { return m_Physics2DSettings; }
		}
		
		/// <summary>
		/// Gets the layer and tag settings object.
		/// </summary>
		public TagManager TagManager {
			get { return m_TagManager; }
		}
		
		/// <summary>
		/// Gets the time settings object.
		/// </summary>
		public TimeManager TimeManager {
			get { return m_TimeManager; }
		}
		
		#endregion
		
		
		#region Methods - Events
		
		/// <summary>
		/// Called before content is activated when navigating from the project to a content submodule. 
		/// </summary>
		virtual public void OnProjectNavigatorWillActivateContent() {
			
		}
		
		/// <summary>
		/// Called after content is activated when navigating from the project to a content submodule.
		/// </summary>
		virtual public void OnProjectNavigatorDidActivateContent() {
			
		}
		
		/// <summary>
		/// Called before content is deactivated when navigating from a content submodule to the project.
		/// </summary>
		virtual public void OnProjectNavigatorWillDeactivateContent() {
			
		}
		
		/// <summary>
		/// Called after content is deactivated when navigating from a content submodule to the project.
		/// </summary>
		virtual public void OnProjectNavigatorDidDeactivateContent() {
			
		}
		
		#endregion
		
		
	}
	
}