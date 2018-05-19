namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class MoverBound : MonoBehaviour {


		#region Fields

		[SerializeField]
		private Transform Anchor;

		[SerializeField]
		private bool Solid = true;

		[System.NonSerialized]
		private Mover m_Mover;

		[System.NonSerialized]
		private Bounds m_Bounds;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.Mover.OnBeforeMoverIsUpdated += this.ApplyBounds;
		}

		void OnDisable() {
			this.Mover.OnBeforeMoverIsUpdated -= this.ApplyBounds;
		}

		public void ApplyBounds(Mover mover) {

//			List<CircleCollider2D> cc = new List<CircleCollider2D>(this.GetComponentsInChildren<CircleCollider2D>());
//
//			foreach (CircleCollider2D c in cc.ToArray()) {
//				if (c.isTrigger) {
//					cc.Remove(c);
//				}
//			}
//
//			if (cc.Count > 0) {
//				foreach (CircleCollider2D c in cc) {
//					float dif = 0;
//
//
//				
//		
//					this.Apply(dif);
//					break;
//		
//				}
//			} else {
//				this.Apply(0);
//			}
		
			Vector3 pos = this.BoundPosition(this.Mover.GoToPosition); 

			if (this.BoundAngle(this.Mover.GoToPosition, pos) == 180) {
				this.Mover.GoToPosition = new Vector3(pos.x, pos.y, this.Mover.GoToPosition.z);
			}

			if (this.Solid) {
				pos = this.BoundPosition(this.Mover.MoveTransform.position);
				if (this.BoundAngle(this.Mover.MoveTransform.position, pos) == 180) {
					this.Mover.MoveTransform.position = new Vector3(pos.x, pos.y, this.Mover.MoveTransform.position.z);
				}
			}
		}

		private float BoundAngle(Vector3 moverPosition, Vector3 intersectionPosition) {
			Vector3 dif = moverPosition - intersectionPosition;
			dif.z = 0;
			dif.Normalize();
			return Vector3.Angle(dif, this.Anchor.up);
		}

		private Vector3 BoundPosition(Vector3 checkPosition) {
			float vx = this.Anchor.right.x;
			if (Mathf.Abs(vx) < 0.001f) {
				vx = 0;
			}
			float vy = this.Anchor.right.y;
			if (Mathf.Abs(vy) < 0.001f) {
				vy = 0;
			}

			Vector3 v2 = new Vector3(vy, -vx, 0);

			float a, c, b, d, x, y;
			Gizmos.color = Color.red;
			Vector3 pos = new Vector3();

			a = vy / vx;
			c = v2.y / v2.x;

			if (a == 0) {
				x = checkPosition.x;
				y = this.Anchor.position.y;
				pos = new Vector3(x, y, this.Anchor.position.z);
			} else if (a == Mathf.Infinity || a == Mathf.NegativeInfinity) {
				x = this.Anchor.position.x;
				y = checkPosition.y;
				pos = new Vector3(x, y, this.Anchor.position.z);
			} else {
				b = this.Anchor.position.y - (a * this.Anchor.position.x); 
				d = checkPosition.y - c * checkPosition.x; 

				x = (d - b) / (a - c);
				y = a * x + b;
				pos = new Vector3(x, y, 0);
			}

//			Debug.DrawLine(pos - Vector3.right - Vector3.up, pos + Vector3.right + Vector3.up, Color.green);
//			Debug.DrawLine(pos + Vector3.up - Vector3.right, pos - Vector3.up + Vector3.right, Color.green);

			return pos;
		}

		private void Apply(float dif) {

			
			
		}

		void OnDrawGizmos() {
			
			if (this.Anchor == null) {
				return;
			} 

			Gizmos.DrawLine(this.Anchor.right * -100 + this.Anchor.position, this.Anchor.right * 100 + this.Anchor.position);
			Gizmos.DrawLine(this.Mover.MoveTransform.position, this.Anchor.up * 1 + this.Mover.MoveTransform.position);

		}

		#endregion


	}
}

