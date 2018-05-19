namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public enum RotationSpace {
		World,
		Local
	}

	public class Rotator : MonoBehaviour {


		#region Fields

		[SerializeField]
		private RotationSpace m_RotationSpace;

		[SerializeField]
		private SpringFloat Angle;

		[TweakerFloatRangeAttribute(0f, 100f)]
		[Range(0f, 100f)]
		[SerializeField]
		private float VelocityDamper = 1;

		[System.NonSerialized]
		private float m_Velocity;

		[System.NonSerialized]
		private Transform m_Transform;

		public event System.Action<Rotator> OnBeforeRotatorIsUpdated;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public float GoToAngle {
			get { return this.Angle.GoToValue; }
			set { this.Angle.GoToValue = value; }
		}

		public float CurrentAngle {
			get { return this.Angle.Value; }
			set { this.Angle.Value = value; }
		}

		public float Velocity {
			get { return m_Velocity; }
			set { m_Velocity = value; }
		}

		public RotationSpace RotationSpace {
			get { return m_RotationSpace; }
			set { m_RotationSpace = value; }
		}

		#endregion

		#region Methods

		void Reset() {
			if (this.Angle == null) {
				this.Angle = this.gameObject.AddComponent<SpringFloat>();
			}
		}

		void Start() {
			this.Angle.GoToValue = 0;
		}

		void Update() {
			if (this.OnBeforeRotatorIsUpdated != null) {
				this.OnBeforeRotatorIsUpdated(this);
			}
			this.Angle.GoToValue += this.Velocity;
			this.Velocity -= this.Velocity * this.VelocityDamper * Time.deltaTime;
			this.Angle.UpdateValue();
			this.SetCurrentAngle();
		}

		public void ForceAngle(float angle) {
			this.Angle.GoToValue = angle;
			this.Angle.Value = angle;
			this.SetCurrentAngle(); 
		}

		private void SetCurrentAngle() {
			if (this.RotationSpace == RotationSpace.World) {
				this.Transform.eulerAngles = new Vector3(0, 0, this.Angle.Value);
			} else if (this.RotationSpace == RotationSpace.Local) {
				this.Transform.localEulerAngles = new Vector3(0, 0, this.Angle.Value);
			}
		}

		public void ReturnLeaverToOriginalRotation() {
			this.Angle.GoToValue = 0;
		}

		#endregion


	}
}   