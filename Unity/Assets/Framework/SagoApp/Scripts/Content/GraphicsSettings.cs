namespace SagoApp.Content {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The GraphicsSettings class allows us to store different graphics settings for each content submodule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class should have the same fields as <c>ProjectSettings/GraphicsSettings.asset</c>. When 
	/// the fields in the <c>GraphicsSettings.asset</a> change, this class should be updated to match.
	/// </para>
	/// </remarks>
	[CreateAssetMenu(menuName = "GraphicsSettings", fileName = "GraphicsSettings", order = 5000)]
	public class GraphicsSettings : ScriptableObject {
		
		
		#region Fields
		
		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private Shader[] m_AlwaysIncludedShaders;
		
		#endregion
		
		
		#region Methods
		
		public Shader[] AlwaysIncludedShaders {
			get { return m_AlwaysIncludedShaders; }
			set { m_AlwaysIncludedShaders = value; }
		}
		
		#endregion
		
		
		#region Methods
		
		private void Reset() {
			m_AlwaysIncludedShaders = new Shader[0];
		}
		
		#endregion
		
		
	}
	
}