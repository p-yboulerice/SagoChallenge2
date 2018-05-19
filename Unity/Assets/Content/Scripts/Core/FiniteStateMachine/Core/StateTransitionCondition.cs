namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class StateTransitionCondition : MonoBehaviour {


		#region Fields

		[Header("Check If State Should Change: ")]

		[SerializeField]
		private bool m_OnEnterBefore = false;

		[SerializeField]
		private bool m_OnEnterAfter = false;

		[SerializeField]
		private bool m_OnUpdateBefore = false;

		[SerializeField]
		private bool m_OnUpdateAfter = false;

		[SerializeField]
		private bool m_OnLateUpdateBefore = false;

		[SerializeField]
		private bool m_OnLateUpdateAfter = true;

		[SerializeField]
		private bool m_OnFixedUpdateBefore = false;

		[SerializeField]
		private bool m_OnFixedUpdateAfter = false;
		

		[SerializeField]
		private State m_GoToState;

		[System.NonSerialized]
		private List<StateFlag> m_Flags;

		[System.NonSerialized]
		private List<StateAction> m_Actions;

		#endregion


		#region Properties

		public State GoToState {
			get { return m_GoToState; }
		}

		public bool AllFlagsActive {
			get {

				if (this.Flags.Count <= 0) {
					return false;
				}

				foreach (StateFlag sf in this.Flags) {
					if (!sf.IsActive) {
						return false;
					}
				}

				return true;
			}
		}

		private List<StateFlag> Flags {
			get {
				
				#if UNITY_EDITOR
				m_Flags = new List<StateFlag>(GetComponents<StateFlag>());
				return m_Flags;
				#endif
				#if !UNITY_EDITOR
				if (m_Flags == null) {
					m_Flags = new List<StateFlag>(GetComponents<StateFlag>());
				}
				return m_Flags;
				#endif

			}
		}

		private List<StateAction> Actions {
			get { 
				#if UNITY_EDITOR
				m_Actions = new List<StateAction>(GetComponents<StateAction>());
				return m_Actions;
				#endif
				#if !UNITY_EDITOR
				if (m_Actions == null) {
				m_Actions = new List<StateAction>(GetComponents<StateAction>());
				}
				return m_Actions;
				#endif
			}
		}

		public bool RunBeforeOnEnter {
			get { return m_OnEnterBefore; }
		}

		public bool RunAfterOnEnter {
			get { return m_OnEnterAfter; }
		}

		public bool RunBeforeOnUpdate {
			get { return m_OnUpdateBefore; }
		}

		public bool RunAfterOnUpdate {
			get { return m_OnUpdateAfter; }
		}

		public bool RunBeforeOnLateUpdate {
			get { return m_OnLateUpdateBefore; }
		}

		public bool RunAfterOnLateUpdate {
			get { return m_OnLateUpdateAfter; }
		}

		public bool RunBeforeFixedUpdate {
			get { return m_OnFixedUpdateBefore; }
		}

		public bool RunAfterFixedUpdate {
			get { return m_OnFixedUpdateAfter; }
		}

		#endregion


		#region Methods

		virtual public void Initialize() {
			foreach (StateFlag sf in this.Flags) {
				sf.Initialize();
			}
		}

		public bool ContainsFlag(StateFlag flag) {
			foreach (StateFlag f in this.Flags) {
				if (f != null && f == flag) {
					return true;
				}
			}
			return false;
		}

		public void ExecuteActions() {
			foreach (StateAction sa in this.Actions) {
				sa.Run();
			}
		}

		#endregion
	
	}

}