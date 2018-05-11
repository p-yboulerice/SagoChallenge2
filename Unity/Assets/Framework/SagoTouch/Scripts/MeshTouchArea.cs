namespace SagoTouch {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// A <see cref="TouchArea" /> subclass that performs hit tests using 
	/// a mesh collider for each of the <see cref="UnityEngine.MeshFilter" /> 
	/// components in the game object and it's children.
	/// </summary>
	public class MeshTouchArea : TouchArea {
		
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the flag indicating whether the list of mesh filters 
		/// needs to be updated. If the value is true, the list of mesh filters
		//  will be updated the next time <see cref="HitTest" /> is called.
		/// </summary>
		/// <value>The flag indicating whether the list of mesh filters needs to be updated.</value>
		public bool IsDirty {
			get;
			set;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Tests whether the point is hitting the <c>TouchArea</c>.
		/// </summary>
		/// <param name="screenPoint">The point to test, in screen space.</param>
		/// <returns>Returns true if the point is hitting the <c>TouchArea</c>, otherwise false.</returns>
		override public bool HitTest(Vector3 screenPoint) {
			return base.HitTest(screenPoint) && MeshCollidersTest(screenPoint); 
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		private List<MeshFilter> m_MeshFilters;
		
		#endregion
		
		
		#region Internal Properties
		
		private List<MeshFilter> MeshFilters {
			get {
				if (this.IsDirty || m_MeshFilters == null) {
					m_MeshFilters = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
				}
				return m_MeshFilters;
			}
		}
		
		#endregion
		
		
		#region Internal Methods
		
		private bool MeshCollidersTest(Vector2 screenPoint) {
			
			bool result;
			result = false;
			
			Ray ray;
			ray = this.Camera.ScreenPointToRay(screenPoint);
			
			List<MeshCollider> colliders;
			colliders = AddMeshColliders();
			
			foreach (MeshCollider collider in colliders) {
				if (MeshColliderTest(collider, ray)) {
					result = true;
					break;
				}
			}
			
			RemoveMeshColliders(colliders);
			return result;
			
		}
		
		private bool MeshColliderTest(MeshCollider collider, Ray ray) {
			RaycastHit hit;
			if (collider.Raycast(ray, out hit, this.Camera.farClipPlane)) {
				return true;
			}
			return false;
		}
		
		private List<MeshCollider> AddMeshColliders() {
			
			List<MeshCollider> colliders;
			colliders = new List<MeshCollider>();
			
			foreach (MeshFilter filter in this.MeshFilters) {
				
				MeshCollider collider;
				collider = this.gameObject.AddComponent<MeshCollider>();
				collider.sharedMesh = filter.sharedMesh;
				
				colliders.Add(collider);
				
			}
			
			return colliders;
			
		}
		
		private void RemoveMeshColliders(List<MeshCollider> colliders) {
			foreach (MeshCollider collider in colliders) {
				Destroy(collider);
			}
		}
		
		#endregion
		
		
	}
	
}