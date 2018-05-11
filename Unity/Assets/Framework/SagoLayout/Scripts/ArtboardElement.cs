namespace SagoLayout {

	using UnityEngine;

	abstract public class ArtboardElement : LayoutComponent {


		//
		// Inspector Properties
		//
		public Vector2 Center;
		public Vector2 Extents;
		public bool UseRendererBounds;


		//
		// Properties
		//
		public Artboard Artboard {
			get {
				m_Artboard = m_Artboard ? m_Artboard : FindArtboard();
				return m_Artboard;
			}
		}

		public float ArtboardHeight {
			get { return this.Artboard.Bounds.size.y; }
		}

		virtual public Bounds Bounds {
			get {
				
				Bounds result;
				result = new Bounds();
				
				if (this.UseRendererBounds) {
					
					Vector2 min;
					min = new Vector2(Mathf.Infinity, Mathf.Infinity);
					
					Vector2 max;
					max = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
					
					foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
						
						Bounds bounds;
						bounds = renderer.bounds;
						
						if (bounds.size == Vector3.zero) {
							bounds.center = renderer.GetComponent<Transform>().position;
						}
						
						min.x = Mathf.Min(min.x, bounds.min.x);
						min.y = Mathf.Min(min.y, bounds.min.y);
						max.x = Mathf.Max(max.x, bounds.max.x);
						max.y = Mathf.Max(max.y, bounds.max.y);
						
					}
					
					min.x = (min.x != Mathf.Infinity) ? min.x : this.Transform.position.x;
					min.y = (min.y != Mathf.Infinity) ? min.y : this.Transform.position.y;
					max.x = (max.x != Mathf.NegativeInfinity) ? max.x : this.Transform.position.x;
					max.y = (max.y != Mathf.NegativeInfinity) ? max.y : this.Transform.position.y;
					
					result.center = (Vector3)(min + 0.5f * (max - min)) + this.Transform.position.z * Vector3.forward;
					result.size = (Vector3)(max - min);
					
				} else {
					result.center = this.Transform.TransformPoint(this.Center);
					result.size = 2 * (this.Transform.TransformPoint(this.Center + this.Extents) - result.center);
				}
				
				return result;
				
			}
		}
		
		public Bounds LocalBounds {
			get {
				Bounds bounds;
				bounds = this.Bounds;
				
				Vector3 localCenter;
				localCenter = this.Transform.InverseTransformPoint(bounds.center);
				
				Vector3 localMax;
				localMax = this.Transform.InverseTransformPoint(bounds.max);
				
				Vector3 localSize;
				localSize = 2 * (localMax - localCenter);
				
				return new Bounds(localCenter, localSize);
			}
		}


		//
		// Member Variables
		//
		private Artboard m_Artboard;


		//
		// MonoBehaviour
		//
		override protected void OnDrawGizmos() {
			base.OnDrawGizmos();
			Bounds bounds;
			bounds = this.Bounds;
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

		override protected void Reset() {
			base.Reset();
			this.UseRendererBounds = true;
		}


		//
		// Functions
		//
		protected Artboard FindArtboard() {
			
			Artboard artboard;
			artboard = null;
			
			Transform root;
			root = this.Transform.root;
			
			Transform transform;
			transform = this.Transform;
			
			while (transform != root) {
				transform = transform.parent;
				artboard = transform.GetComponent<Artboard>();
				if (artboard) return artboard;
			}
			
			throw new System.InvalidOperationException(string.Format("{0} must be a child of Artboard component", this));
			
		}


	}

}
