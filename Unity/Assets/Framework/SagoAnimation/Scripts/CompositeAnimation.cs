namespace SagoAnimation {
	
	using SagoAudio;
	using SagoMesh;
	using UnityEngine;
	
	public class CompositeAnimation : ScriptableObject {
		
		
		#region Fields
		
		[SerializeField]
		protected AudioAnimation m_AudioAnimation;
		
		[SerializeField]
		protected MeshAnimation m_MeshAnimation;
		
		#endregion
		
		
		#region Properties
		
		public AudioAnimation AudioAnimation {
			get { return m_AudioAnimation; }
			set {
				var oldValue = m_AudioAnimation;
				var newValue = value;
				if (oldValue != newValue) {
					m_AudioAnimation = newValue;
					#if UNITY_EDITOR
						UnityEditor.EditorUtility.SetDirty(this);
					#endif
				}
			}
		}
		
		public MeshAnimation MeshAnimation {
			get { return m_MeshAnimation; }
			set {
				var oldValue = m_MeshAnimation;
				var newValue = value;
				if (oldValue != newValue) {
					m_MeshAnimation = newValue;
					#if UNITY_EDITOR
						UnityEditor.EditorUtility.SetDirty(this);
					#endif
				}
			}
		}
		
		#endregion
		
		
	}
	
}