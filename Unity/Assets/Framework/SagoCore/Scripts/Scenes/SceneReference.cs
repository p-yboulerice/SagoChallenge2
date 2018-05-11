namespace SagoCore.Scenes {
	
	using UnityEngine;
	
	/// <summary>
	/// The SceneReference class allows a scene or prefab to reference another 
	/// scene without hard-coding the scene name or path. The referenced scene 
	/// can be loaded directly or from an <c>AssetBundle</c>.
	/// </summary>
	
	/// <remarks>
	/// <c>SceneReference</c> objects use the guid (not the path) of the 
	/// referenced scene, which allows you to move or rename the referenced 
	/// scene without breaking anything.
	/// </para>
	/// </remarks>
	
	/// <example>
	/// <para>
	/// The following code shows how to use a <c>SceneReference</c> to reference 
	/// another scene. When you look at the <c>Example</c> component in the 
	/// inspector, you'll see an <see cref="SceneAsset" /> control. When you 
	/// drag a scene into the control, a new <c>SceneReference</c> struct will 
	/// be created and assigned to the <c>m_Scene</c> field.
	/// </para>
	/// <code>
	/// public class Example : MonoBehaviour {
	/// 	
	/// 	[SerializeField]
	/// 	private SceneReference m_Scene;
	/// 	
	/// }
	/// </code>
	/// </example>
	
	[System.Serializable]
	public struct SceneReference {
		
		
		#region Fields
		
		/// <summary>
		/// The guid for the referenced scene.
		/// </summary>
		[SerializeField]
		private string m_Guid;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the guid for the referenced scene.
		/// </summary>
		public string Guid {
			get { return m_Guid; }
			set { m_Guid = value; }
		}
		
		/// <summary>
		/// Gets the asset bundle name for the referenced scene.
		/// </summary>
		/// <returns>
		/// <c>null</c> if the scene is not in an asset bundle.
		/// </returns>
		public string AssetBundleName {
			get { return SceneMap.GetAssetBundleName(m_Guid); }
		}
		
		/// <summary>
		/// Gets the asset path for the referenced scene.
		/// </summary>
		public string AssetPath {
			get { return SceneMap.GetAssetPath(m_Guid); }
		}
		
		/// <summary>
		/// Gets the scene path for the referenced scene.
		/// </summary>
		public string ScenePath {
			get { return SceneMap.GetScenePath(m_Guid); }
		}
		
		#endregion
		
		
	}
	
}