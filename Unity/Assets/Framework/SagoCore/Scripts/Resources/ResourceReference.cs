namespace SagoCore.Resources {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	
	/// <summary>
	/// The ResourceReferenceTypeAttribute class lets you to specify which 
	/// type of asset may be reference by a <see cref="ResourceReference" />.
	/// </summary>
	public class ResourceReferenceTypeAttribute : System.Attribute {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the type of asset that may be referenced.
		/// </summary>
		public System.Type Type {
			get;
			protected set;
		}
		
		#endregion
		
		
		#region Constructors
		
		/// <summary>
		/// Creates a new ResourceReferenceTypeAttribute instance.
		/// </summary>
		/// <param name="type">
		/// The type of asset that may be referenced.
		/// </param>
		public ResourceReferenceTypeAttribute(System.Type type) {
			this.Type = type;
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The ResourceReference class allows a scene or prefab to reference an 
	/// asset without loading that asset when the scene or prefab is loaded.
	/// The referenced asset can be loaded from a <c>Resources</c> folder 
	/// or an <c>AssetBundle</c>.
	/// </summary>
	
	/// <remarks>
	/// <para>
	/// Each <c>ResourceReference</c> is a <see cref="ScriptableObject" /> asset. 
	/// The scene or prefab references the <c>ResourceReference</c> object (which 
	/// is very small) instead of the actual asset (which could be very big). 
	/// </para>
	/// <para>
	/// <c>ResourceReference</c> assets are located beside the referenced asset
	/// on the file system. They have the same name as the referenced asset, with 
	/// the guid appended.
	/// <code>
	/// Assets/Resources/my_asset.asset // the asset
	/// Assets/Resources/my_asset.abc123.asset // the resource reference asset
	/// </code>
	/// </para>
	/// <para>
	/// <c>ResourceReference</c> objects use the guid (not the path) of the 
	/// referenced asset, which allows you to move or rename the referenced 
	/// asset without breaking anything (the resource reference asset will be
	/// moved automatically).
	/// </para>
	/// </remarks>
	
	/// <example>
	/// <para>
	/// The following code shows how to use a <c>ResourceReference</c> 
	/// to reference an <see cref="AudioClip" />. When you look at the 
	/// <c>Example</c> component in the inspector, you'll see an 
	/// <see cref="AudioClip" /> control. When you drag an asset into the 
	/// control, a new <c>ResourceReference</c> asset will be created and 
	/// assigned to the <c>m_AudioClip</c> field.
	/// <para>
	/// <code>
	/// public class Example : MonoBehaviour {
	/// 	
	/// 	[ResourceReferenceType(typeof(AudioClip))]
	/// 	[SerializeField]
	/// 	private ResourceReference m_AudioClip;
	/// 	
	/// }
	/// </code>
	/// </example>
	public class ResourceReference : ScriptableObject, IResourceReference {
		
		
		#region Fields
		
		/// <summary>
		/// The guid of the referenced asset.
		/// </summary>
		[FormerlySerializedAs("m_AssetGuid")]
		[SerializeField]
		protected string m_Guid;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets and sets the guid of the referenced asset.
		/// </summary>
		public string Guid {
			get { return m_Guid; }
			set {
				if (m_Guid != value) {
					m_Guid = value;
					#if UNITY_EDITOR
						UnityEditor.EditorUtility.SetDirty(this);
					#endif
				}
			}
		}
		
		/// <summary>
		/// Gets the asset bundle name of the referenced asset.
		/// </summary>
		/// <returns>
		/// <c>null</c> if the referenced asset is not in an asset bundle.
		/// </returns>
		public string AssetBundleName {
			get { return ResourceMap.GetAssetBundleName(m_Guid); }
		}
		
		/// <summary>
		/// Gets the asset path of the referenced asset.
		/// </summary>
		public string AssetPath {
			get { return ResourceMap.GetAssetPath(m_Guid); }
		}
		
		/// <summary>
		/// Gets the resource path of the referenced asset.
		/// </summary>
		/// <returns>
		/// <c>null</c> if the referenced asset is not in a resources folder.
		/// </returns>
		public string ResourcePath {
			get { return ResourceMap.GetResourcePath(m_Guid); }
		}
		
		#endregion
		
		
	}
	
}