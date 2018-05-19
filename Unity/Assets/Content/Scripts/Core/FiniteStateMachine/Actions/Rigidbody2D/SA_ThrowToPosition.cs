namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	/// <summary>
	/// Set MoverVelocity.Velocity such that the item is thrown to the
	/// given position.
	/// </summary>
	public class SA_ThrowToPosition : StateAction {


		#region Fields

		[SerializeField]
		private float UpForce_Min;

		[SerializeField]
		private float UpForce_Max;

		[SerializeField]
		private Vector3 m_Position = Vector3.zero;

		[SerializeField]
		private Transform m_ThrowToPosition;

		[Range(0f, 2f)]
		[SerializeField]
		private float m_Clearance = 0.5f;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody2D;

		#endregion


		#region Properties

		public Rigidbody2D Rigidbody2D {
			get { return m_Rigidbody2D = m_Rigidbody2D ?? GetComponentInParent<Rigidbody2D>(); }	
		}

		public Vector3 Position {
			get { return this.SourceTransform != null ? this.SourceTransform.position : m_Position; }
			set { m_Position = value; }
		}

		public Transform SourceTransform {
			get { return m_ThrowToPosition; }
			set { m_ThrowToPosition = value; }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (this.Rigidbody2D != null) {
				Vector2 vel = GetThrowVelocity(this.Rigidbody2D.transform.position, this.Position, m_Clearance);
				this.Rigidbody2D.velocity = vel;
			}
		}

		protected Vector2 GetThrowVelocity(Vector2 current, Vector2 target, float clearance) {
			float dx = target.x - current.x;
			float dy = target.y - current.y;
			float dup, ddown;
			if (dy > 0) {
				dup = clearance + dy;
				ddown = clearance;
			} else {
				dup = clearance;
				ddown = clearance - dy;
			}
			float gravity = Mathf.Abs(Physics2D.gravity.y);

			float upv = Random.Range(this.UpForce_Min, this.UpForce_Max);
			float upStopTime = Mathf.Sqrt(upv / gravity);

			float vy = Mathf.Sqrt(2 * gravity * dup);
			float t = vy / gravity + Mathf.Sqrt(2f * ddown / gravity) + upStopTime;
			float vx = dx / t;
			return new Vector2(vx, vy + upv);
		}

		#endregion

	}
	
}
