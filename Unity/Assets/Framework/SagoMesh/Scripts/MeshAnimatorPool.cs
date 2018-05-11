namespace SagoMesh {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	public class MeshAnimatorPool : MonoBehaviour {
		
		
		#region Fields
		
		[SerializeField]
		protected List<MeshAnimator> m_AllAnimators;
		
		[System.NonSerialized]
		protected List<MeshAnimator> m_UnusedAnimators;
		
		#endregion
		
		
		#region Properties
		
		public List<MeshAnimator> AllAnimators {
			get {
				if (m_AllAnimators == null) {
					m_AllAnimators = new List<MeshAnimator>();
				}
				return m_AllAnimators;
			}
			set {
				m_AllAnimators = value;
				m_UnusedAnimators = null;
			}
		}
		
		public MeshAnimator NextAnimator {
			get {
				
				MeshAnimator animator;
				animator = null;
				
				if (this.AllAnimators.Count != 0) {
					if (this.UnusedAnimators.Count == 1) {
						animator = this.UnusedAnimators[0];
						this.UnusedAnimators = null;
					} else {
						animator = this.UnusedAnimators[Random.Range(0, this.UnusedAnimators.Count)];
					}
					this.UnusedAnimators.Remove(animator);
				}
				
				return animator;
				
			}
		}
		
		public List<MeshAnimator> UnusedAnimators {
			get {
				if (m_UnusedAnimators == null || m_UnusedAnimators.Count == 0) {
					m_UnusedAnimators = new List<MeshAnimator>(this.AllAnimators);
				}
				return m_UnusedAnimators;
			}
			set { m_UnusedAnimators = value; }
		}
		
		#endregion
		
		
	}
	
}