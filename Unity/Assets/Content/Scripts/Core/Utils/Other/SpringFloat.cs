namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class SpringFloat : MonoBehaviour {

		#region Fields

		[Tweaker]
		[SerializeField]
		private float m_Damper = 0.1f;

		[Tweaker]
		[SerializeField]
		private float m_ForceStrength = 0.2f;

		[SerializeField]
		private float m_GoToValue = 1;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public float GoToValue {
			get { return m_GoToValue; }
			set { m_GoToValue = value; }
		}

		public float Value {
			get;
			set;
		}

		private float Damper {
			get { return m_Damper; }
			set { m_Damper = value; }
		}

		private float Velocity {
			get;
			set;
		}

		private float ForceStrength {
			get { return m_ForceStrength; }
			set { m_ForceStrength = value; }
		}

		#endregion


		#region Methods

		public void UpdateValue() {
			Velocity *= (1 - Damper);
			Velocity += ForceStrength * (GoToValue - Value);
			Value += Velocity;
		}

		public void AddVelocity(float amount) {
			this.Velocity += amount;
		}

		#endregion


	}
}
