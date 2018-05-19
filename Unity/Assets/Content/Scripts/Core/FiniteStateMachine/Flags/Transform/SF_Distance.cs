namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public enum DistanceConditionType :int {
		LessThan = 0,
		MoreThan = 1
	}

	public class SF_Distance : StateFlag {


		#region Fields

		[SerializeField]
		private Transform m_Target;

		[SerializeField]
		private Transform m_From;

		[SerializeField]
		private float m_Distance = 0.1f;

		[SerializeField]
		private DistanceConditionType m_DistanceCondition;

		#endregion


		#region Properties

		private float Distance {
			get { return m_Distance; }
		}

		public Transform Target {
			get { return m_Target; }
			set { m_Target = value; }
		}

		private DistanceConditionType DistanceCondition {
			get { return m_DistanceCondition; }
		}

		private Transform From {
			get { return m_From; }
		}

		#endregion


		#region Methods


		public override bool IsActive {
			get {
				
				if (this.Target == null || this.From == null) {
					return false;
				}

				switch (this.DistanceCondition) {
				case DistanceConditionType.LessThan:
					return Vector3.Distance(this.From.position, this.Target.position) < this.Distance;
				case DistanceConditionType.MoreThan:
					return Vector3.Distance(this.From.position, this.Target.position) > this.Distance;
				}

				return false;
			}
		}

		#endregion


	}
}