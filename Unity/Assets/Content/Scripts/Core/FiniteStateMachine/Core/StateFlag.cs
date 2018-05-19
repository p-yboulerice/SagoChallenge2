namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class StateFlag : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		virtual public bool IsActive {
			get { return false; }
		}

		#endregion


		#region Methods

		virtual public void Initialize() {
		}

		#endregion

	}

}