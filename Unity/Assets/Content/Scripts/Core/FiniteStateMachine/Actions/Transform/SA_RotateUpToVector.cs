namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class SA_RotateUpToVector : StateAction {

		#region Fields

		[SerializeField]
		private Transform m_TargetTransform;

		[SerializeField]
		private float m_Damper = 0.1f;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ForceStrength = 0.2f;

		[SerializeField]
		private float m_Ease = 3;

		#endregion


		#region Properties

		private Transform TargetTransform {
			get { return m_TargetTransform; }
		}

		private float Ease {
			get { return m_Ease; }
		}

		private float CurrentRotation {
			get;
			set;
		}

		private float GoToRotation {
			get;
			set;
		}

		private float Velocity {
			get;
			set;
		}

		private float Damper {
			get { return m_Damper; }
			set { m_Damper = value; }
		}

		private float ForceStrength {
			get { return m_ForceStrength; }
			set { m_ForceStrength = value; }
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.CurrentRotation = this.TargetTransform.eulerAngles.z > 180 ? this.TargetTransform.eulerAngles.z - 360 : this.TargetTransform.eulerAngles.z;
		}

		public override void Run() {

			Velocity *= (1 - Damper);
			Velocity += ForceStrength * (this.GoToRotation - this.CurrentRotation);
			this.CurrentRotation += Velocity;

			this.TargetTransform.eulerAngles = new Vector3(0, 0, this.CurrentRotation);

		}

		#endregion


	}

}
