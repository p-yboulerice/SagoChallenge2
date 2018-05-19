namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoUtils;
	using Juice.Utils;

	public class SF_Speed : StateFlag {


		#region StateFlag

		public override bool IsActive {
			get {
				bool result;

				float speed = this.Speed;
				if (this.MustBeAboveMin && this.MustBeBelowMax) {
					result = (speed >= this.MinSpeed && speed <= this.MaxSpeed);
				} else if (this.MustBeAboveMin) {
					result = (speed >= this.MinSpeed);
				} else if (this.MustBeBelowMax) {
					result = (speed <= this.MaxSpeed);
				} else {
					result = true;
				}

				if (m_Invert) {
					result = !result;
				}

				return result;
			}
		}

		#endregion


		#region Serialized Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private bool m_Invert;

		[SerializeField]
		private bool m_MustBeAboveMin;

		[Disable(typeof(SF_Speed), "DisableMinSpeed", 1, true)]
		[SerializeField]
		private float m_MinSpeed;

		[SerializeField]
		private bool m_MustBeBelowMax;

		[Disable(typeof(SF_Speed), "DisableMaxSpeed", 1, true)]
		[SerializeField]
		private float m_MaxSpeed;

		#endregion


		#region Internal Properties

		private bool Invert {
			get { return m_Invert; }
		}

		private float MaxSpeed {
			get { return m_MaxSpeed; }
		}

		private float MinSpeed {
			get { return m_MinSpeed; }
		}
		
		private Mover Mover {
			get { return m_Mover; }
		}

		private bool MustBeAboveMin {
			get { return m_MustBeAboveMin; }
		}

		private bool MustBeBelowMax {
			get { return m_MustBeBelowMax; }
		}

		private float Speed {
			get {
				return this.Mover ? ((Vector2)this.Mover.Velocity).magnitude : 0f;
			}
		}

		#endregion


		#region Internal Methods

		private static bool DisableMinSpeed(Object obj) { return !(obj as SF_Speed).MustBeAboveMin; }
		private static bool DisableMaxSpeed(Object obj) { return !(obj as SF_Speed).MustBeBelowMax; }

		private void Reset() {
			m_Mover = GetComponentInParent<Mover>();
			m_Invert = false;
			m_MustBeAboveMin = false;
			m_MustBeBelowMax = true;
			m_MaxSpeed = 1f;
		}

		#endregion


	}

}