namespace Juice.FSM {
	
	using UnityEngine;
	using System.Collections;

	public class SA_SwitchFiniteStateMachine : StateAction {


		#region Fields

		[SerializeField]
		private FiniteStateMachine m_ToFiniteStateMachine;

		[System.NonSerialized]
		private FiniteStateMachine m_FiniteStateMachine;

		#endregion


		#region Properties

		private FiniteStateMachine ToFiniteStateMachine {
			get { return m_ToFiniteStateMachine; }
		}

		private FiniteStateMachine FiniteStateMachine {
			get { return m_FiniteStateMachine = m_FiniteStateMachine ?? GetComponentInParent<FiniteStateMachine>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.FiniteStateMachine.enabled = false;
			this.ToFiniteStateMachine.enabled = true;
		}

		#endregion

	}
}