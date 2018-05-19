namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class RotatorSpring : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float m_GoToRotation;

		[SerializeField]
		private float Acceleration = 3;

		[SerializeField]
		private float Damper = 1;

		[SerializeField]
		private float AtRotation_Distance = 0.1f;

		[SerializeField]
		private float AtRotation_Speed = 0.2f;

		[SerializeField]
		private bool LimitRotation;

		[SerializeField]
		private float MaxRotation;

		[System.NonSerialized]
		private float CurrentRotation;

		[System.NonSerialized]
		public float Velocity;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		public float GoToRotation {
			get { return m_GoToRotation; }
			set { m_GoToRotation = value; }
		}

		public bool AtRotation {
			get {
				float dif = Mathf.Abs(this.GoToRotation - this.CurrentRotation);
				return dif < this.AtRotation_Distance & Mathf.Abs(this.Velocity) < this.AtRotation_Speed;
			}
		}

		#endregion


		#region Methods

		void Update() {
			this.Velocity -= this.Damper * this.Velocity * Time.deltaTime;
			float dif = this.GoToRotation - this.CurrentRotation;
			this.Velocity += dif * this.Acceleration * Time.deltaTime;
			this.CurrentRotation += this.Velocity;
			if (this.LimitRotation) {
				if (Mathf.Abs(this.CurrentRotation) > this.MaxRotation) {
					this.CurrentRotation = this.CurrentRotation > 0 ? this.MaxRotation : -this.MaxRotation;
				}
			}
			this.Transform.eulerAngles = new Vector3(0, 0, this.CurrentRotation);
		}

		public void ForceRotation(float rotation) {
			this.GoToRotation = rotation;
			this.Transform.eulerAngles = new Vector3(0, 0, this.CurrentRotation);
		}

		#endregion


	}
}