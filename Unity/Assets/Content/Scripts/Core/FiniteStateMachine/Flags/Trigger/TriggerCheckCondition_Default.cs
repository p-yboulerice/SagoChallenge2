namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class TriggerCheckCondition_Default : MonoBehaviour, ITriggerCheckCondition {


		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		#endregion


		#region ITriggerCheckCondition implementation

		public bool CheckTrigger(Collider2D trigger) {
			return true;
		}

		#endregion
	}
}