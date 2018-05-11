namespace SagoTouch {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// A <see cref="TouchArea" /> subclass that performs hit tests using 
	/// all of the colliders in the game object and it's children.
	/// </summary>
	public class ColliderTouchArea : TouchArea {
		
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the flag indicating whether the list of colliders needs 
		/// to be updated. If the value is true, the list of colliders will be 
		/// updated the next time <see cref="HitTest" /> is called.
		/// </summary>
		/// <value>The flag indicating whether the list of colliders needs to be updated.</value>
		public bool IsDirty {
			get;
			set;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Tests whether the point is hitting the <see cref="TouchArea" />.
		/// </summary>
		/// <param name="screenPoint">The point to test, in screen space.</param>
		/// <returns><c>true</c> if the point is hitting the <see cref="TouchArea" />, otherwise <c>false</c>.</returns>
		override public bool HitTest(Vector3 screenPoint) {
			return base.HitTest(screenPoint) && (ColliderTest(screenPoint) || ColliderTest2D(screenPoint)); 
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		private List<Collider> m_Colliders;
		
		[System.NonSerialized]
		private List<Collider2D> m_Colliders2D;
		
		#endregion
		
		
		#region Internal Properties
		
		private List<Collider> Colliders {
			get {
				if (this.IsDirty || m_Colliders == null) {
					m_Colliders = new List<Collider>(GetComponentsInChildren<Collider>());
				}
				return m_Colliders;
			}
		}
		
		private List<Collider2D> Colliders2D {
			get {
				if (this.IsDirty || m_Colliders2D == null) {
					m_Colliders2D = new List<Collider2D>(GetComponentsInChildren<Collider2D>());
				}
				return m_Colliders2D;
			}
		}
		
		#endregion
		
		
		#region Internal Methods
		
		private bool ColliderTest(Vector2 screenPoint) {
			
			bool result;
			result = false;
			
			Ray ray;
			ray = this.Camera.ScreenPointToRay(screenPoint);
			
			RaycastHit hit;
			
			Dictionary<Collider, bool> enabledStatesByCollider;
			enabledStatesByCollider = new Dictionary<Collider, bool>();
			
			foreach (Collider collider in this.Colliders) {
				
				enabledStatesByCollider.Add(collider, collider.enabled);
				collider.enabled = true;
				
				if (collider.Raycast(ray, out hit, 2 * this.Camera.farClipPlane)) {
					result = true;
					break;
				}
				
			}
			
			foreach (Collider collider in enabledStatesByCollider.Keys) {
				collider.enabled = enabledStatesByCollider[collider];
			}
			
			return result;
			
		}
		
		private bool ColliderTest2D(Vector2 screenPoint) {
			
			bool result;
			result = false;
			
			Vector2 worldPoint;
			worldPoint = this.Camera.ScreenToWorldPoint(screenPoint);
			
			Dictionary<Collider2D, bool> enabledStatesByCollider;
			enabledStatesByCollider = new Dictionary<Collider2D, bool>();
			
			foreach (Collider2D collider in this.Colliders2D) {
				
				enabledStatesByCollider.Add(collider, collider.enabled);
				collider.enabled = true;
				
				if (collider.OverlapPoint(worldPoint)) {
					result = true;
					break;
				}
			
			}
			
			foreach (Collider2D collider in enabledStatesByCollider.Keys) {
				collider.enabled = enabledStatesByCollider[collider];
			}
			
			return result;
		
		}
		
		#endregion
		
		
	}

}