namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class FiniteStateMachine : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float m_RunPriority;

		[SerializeField]
		private State m_DefaultState;

		[SerializeField]
		private bool m_InitializeOnStart = true;

		[SerializeField]
		private bool m_DebugLogChangeState = false;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<State> m_States;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<State> States {
			get {
				if (m_States == null) {
					m_States = new List<State>();
					State[] states = GetComponentsInChildren<State>(true);
					foreach (State s in states) {
						if (s.Transform.parent == this.Transform) {
							m_States.Add(s);
						}
					}
				}
				return m_States;
			}
		}

		public State CurrentState {
			get;
			private set;
		}

		public State DefautState {
			get { return m_DefaultState; }
			set { m_DefaultState = value; }
		}

		public float RunPriority {
			get { return m_RunPriority; }
		}

		private Coroutine UpdateCoroutine {
			get;
			set;
		}

		private bool DebugLogChangeState {
			get { return m_DebugLogChangeState; }
		}

		private bool InitializeOnStart {
			get { return m_InitializeOnStart; }
		}

		private bool Initialized {
			get;
			set;
		}

		#endregion


		#region Methods

		void Start() {
			if (this.InitializeOnStart) {
				this.Initialize();
			}
		}

		void OnEnable() {
			FiniteStateMachineManager.Instance.RegisterFiniteStateMachine(this);
			this.RestartStateMachine();
		}

		void OnDisable() {
			if (this.CurrentState != null) {
				this.CurrentState.ExitState();
				this.CurrentState.gameObject.SetActive(false);
				this.CurrentState = null;
			}
			this.DisableAllStates();
			if (FiniteStateMachineManager.Instance != null) {
				FiniteStateMachineManager.Instance.UnregisterFiniteStateMachine(this);
			}
		}

		private void DisableAllStates() {
			foreach (State state in this.States) {
				state.gameObject.SetActive(false);
			}
		}

		public void Initialize() {
			if (!this.Initialized) {
				this.Initialized = true;
				foreach (State s in this.States) {
					s.Initialize();
				}
			}
		}

		public void RestartStateMachine() {
			this.DisableAllStates();
			this.CurrentState = this.DefautState;
			this.CurrentState.gameObject.SetActive(true);
			this.CurrentState.EnterState();
		}

		public void ChangeState(State state) {

			if (this.DebugLogChangeState && this.CurrentState != null) {
				Debug.Log(this.gameObject.name + "   /    " + this.CurrentState.name + "   >>>>>    " + state.name, this);
			}

			if (this.CurrentState != null && this.CurrentState == state) {
				Debug.Log("Trying to change state but you are already on this state (" + this.CurrentState.name + ")", this);
				return;
			}

			if (!States.Contains(state)) {
				string rootCause = "";
				Transform parent = this.Transform;
				while (parent.parent != null) {
					parent = parent.parent;
					rootCause += parent.name + "<<<";

				}
				Debug.Log(rootCause);
					
				Debug.LogErrorFormat(this, "Trying to switch to State ({0}), but the game object {1} does not contain this state", new object[] {
					state.gameObject.name,
					this.gameObject.name
				});
			}

			this.ExitCurrentState();
				
			this.CurrentState = state;
			this.CurrentState.gameObject.SetActive(true);
			this.CurrentState.EnterState();
		}

		public void ExitCurrentState() {
			if (this.CurrentState != null) {
				this.CurrentState.ExitState();
				this.CurrentState.gameObject.SetActive(false);
			}
		}

		public void StateChange_Before_Update() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_Before_Update();
			}
		}

		public void StateChange_After_Update() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_After_Update();
			}
		}

		public void StateChange_Before_LateUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_Before_LateUpdate();
			}
		}

		public void StateChange_After_LateUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_After_LateUpdate();
			}
		}

		public void StateChange_Before_FixedUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_Before_FixedUpdate();
			}
		}

		public void StateChange_After_FixedUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.CheckStateTransitionCondition_After_FixedUpdate();
			}
		}

		public void UpdateStateFlagAction_Before_Update() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_Before_Update();
			}
		}

		public void UpdateStateFlagAction_After_Update() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_After_Update();
			}
		}

		public void UpdateStateFlagAction_Before_LateUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_Before_LateUpdate();
			}
		}

		public void UpdateStateFlagAction_After_LateUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_After_LateUpdate();
			}
		}

		public void UpdateStateFlagAction_Before_FixedUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_Before_FixedUpdate();
			}
		}

		public void UpdateStateFlagAction_After_FixedUpdate() {
			if (this.CurrentState != null) {
				this.CurrentState.UpdateFlagActions_After_FixedUpdate();
			}
		}

		public void UpdateState() {
			if (this.CurrentState != null) {
				this.CurrentState.StateUpdate();
			}
		}

		public void LateUpdateState() {
			if (this.CurrentState != null) {
				this.CurrentState.StateLateUpdate();
			}
		}

		public void FixedUpdateState() {
			if (this.CurrentState != null) {
				this.CurrentState.StateFixedUpdate();
			}
		}

		public void CleanAndRefresh() {
			bool cleanedJunk = false;
			StateLogic[] sls = GetComponentsInChildren<StateLogic>(true);

			foreach (StateLogic sl in sls) {
				if (sl.CleanAndRefresh()) {
					cleanedJunk = true;
				}
			}

			if (cleanedJunk) {
				Debug.Log("Finite State Machine was Cleaned : If Object is a Prefab you may want to Apply these Changes");
			} else {
				Debug.Log("All seems to be Good with this FiniteStateMachine");
			}

			//0 out all children and fsm it self within local space
		}


		#endregion

	
	}

}