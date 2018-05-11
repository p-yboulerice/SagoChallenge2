namespace SagoApp.Content {
	
	using UnityEngine;
	
	/// <summary>
	/// The TagManager class allows us to store different layer and tag settings for each content submodule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class should have the same fields as <c>ProjectSettings/TagManager.asset</c>. When 
	/// the fields in the <c>TagManager.asset</a> change, this class should be updated to match.
	/// </para>
	/// </remarks>
	[CreateAssetMenu(menuName = "TagManager", fileName = "TagManager", order = 5000)]
	public class TagManager : ScriptableObject {
		
		
		#region Fields
		
		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private string[] layers;
		
		[SerializeField]
		private string[] tags;
		
		#endregion
		
		
		#region Properties
		
		public string[] Layers {
			get { return layers; }
		}
		
		public string[] Tags {
			get { return tags; }
		}
		
		#endregion
		
		
		#region Methods
		
		private void Reset() {
			
			layers = new string[32];
			tags = new string[0];
			
			layers[0] = "Default";
			layers[1] = "TransparentFX";
			layers[2] = "Ignore Raycast";
			layers[3] = null;
			layers[4] = "Water";
			layers[5] = "UI";
			layers[6] = null;
			layers[7] = null;
			
		}
		
		#endregion
		
		
	}
	
}