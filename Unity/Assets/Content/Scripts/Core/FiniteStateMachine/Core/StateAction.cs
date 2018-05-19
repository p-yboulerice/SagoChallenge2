namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class StateAction : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		virtual public void Initialize() {
		}

		virtual public void Run() {
		}

		#endregion

	
	}

}