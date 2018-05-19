namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class State : MonoBehaviour {


		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<StateLogic> m_StateLogics;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditions;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsBeforeEnter;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsAfterEnter;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsBeforeUpdate;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsAfterUpdate;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsBeforeLateUpdate;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsAfterLateUpdate;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsBeforeFixedUpdate;

		[System.NonSerialized]
		private List<StateTransitionCondition> m_StateTransitionConditionsAfterFixedUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActions;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsBeforeEnter;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsAfterEnter;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsBeforeUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsAfterUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsBeforeLateUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsAfterLateUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsBeforeFixedUpdate;

		[System.NonSerialized]
		private List<StateFlagAction> m_StateFlagActionsAfterFixedUpdate;

		[System.NonSerialized]
		private FiniteStateMachine m_FiniteStateMachine;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		public List<StateTransitionCondition> StateTransitionConditions {
			get { 
				return m_StateTransitionConditions = m_StateTransitionConditions ?? new List<StateTransitionCondition>(GetComponentsInChildren<StateTransitionCondition>()); 
			}
		}

		public List<StateTransitionCondition> STCBeforeOnEnter {
			get { 
				if (m_StateTransitionConditionsBeforeEnter == null) {
					m_StateTransitionConditionsBeforeEnter = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunBeforeOnEnter) {
							m_StateTransitionConditionsBeforeEnter.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsBeforeEnter;
			}
		}

		public List<StateTransitionCondition> STCAfterOnEnter {
			get { 
				if (m_StateTransitionConditionsAfterEnter == null) {
					m_StateTransitionConditionsAfterEnter = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunAfterOnEnter) {
							m_StateTransitionConditionsAfterEnter.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsAfterEnter;
			}
		}

		public List<StateTransitionCondition> STCBeforeUpdate {
			get { 
				if (m_StateTransitionConditionsBeforeUpdate == null) {
					m_StateTransitionConditionsBeforeUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunBeforeOnUpdate) {
							m_StateTransitionConditionsBeforeUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsBeforeUpdate;
			}
		}

		public List<StateTransitionCondition> STCAfterUpdate {
			get { 
				if (m_StateTransitionConditionsAfterUpdate == null) {
					m_StateTransitionConditionsAfterUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunAfterOnUpdate) {
							m_StateTransitionConditionsAfterUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsAfterUpdate;
			}
		}

		public List<StateTransitionCondition> STCBeforeLateUpdate {
			get { 
				if (m_StateTransitionConditionsBeforeLateUpdate == null) {
					m_StateTransitionConditionsBeforeLateUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunBeforeOnLateUpdate) {
							m_StateTransitionConditionsBeforeLateUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsBeforeLateUpdate;
			}
		}

		public List<StateTransitionCondition> STCAfterLateUpdate {
			get { 
				if (m_StateTransitionConditionsAfterLateUpdate == null) {
					m_StateTransitionConditionsAfterLateUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunAfterOnLateUpdate) {
							m_StateTransitionConditionsAfterLateUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsAfterLateUpdate;
			}
		}

		public List<StateTransitionCondition> STCBeforeFixedUpdate {
			get { 
				if (m_StateTransitionConditionsBeforeFixedUpdate == null) {
					m_StateTransitionConditionsBeforeFixedUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunBeforeFixedUpdate) {
							m_StateTransitionConditionsBeforeFixedUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsBeforeFixedUpdate;
			}
		}

		public List<StateTransitionCondition> STCAfterFixedUpdate {
			get { 
				if (m_StateTransitionConditionsAfterFixedUpdate == null) {
					m_StateTransitionConditionsAfterFixedUpdate = new List<StateTransitionCondition>();
					foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
						if (stc.RunAfterFixedUpdate) {
							m_StateTransitionConditionsAfterFixedUpdate.Add(stc);
						}
					}
				}
				return m_StateTransitionConditionsAfterFixedUpdate;
			}
		}

		public List<StateFlagAction> StateFlagActions {
			get { 
				return m_StateFlagActions = m_StateFlagActions ?? new List<StateFlagAction>(GetComponentsInChildren<StateFlagAction>()); 
			}
		}

		public List<StateFlagAction> SFABeforeOnEnter {
			get { 
				if (m_StateFlagActionsBeforeEnter == null) {
					m_StateFlagActionsBeforeEnter = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunBeforeOnEnter) {
							m_StateFlagActionsBeforeEnter.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsBeforeEnter;
			}
		}

		public List<StateFlagAction> SFAAfterOnEnter {
			get { 
				if (m_StateFlagActionsAfterEnter == null) {
					m_StateFlagActionsAfterEnter = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunAfterOnEnter) {
							m_StateFlagActionsAfterEnter.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsAfterEnter;
			}
		}

		public List<StateFlagAction> SFABeforeUpdate {
			get { 
				if (m_StateFlagActionsBeforeUpdate == null) {
					m_StateFlagActionsBeforeUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunBeforeOnUpdate) {
							m_StateFlagActionsBeforeUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsBeforeUpdate;
			}
		}

		public List<StateFlagAction> SFAAfterUpdate {
			get { 
				if (m_StateFlagActionsAfterUpdate == null) {
					m_StateFlagActionsAfterUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunAfterOnUpdate) {
							m_StateFlagActionsAfterUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsAfterUpdate;
			}
		}

		public List<StateFlagAction> SFABeforeLateUpdate {
			get { 
				if (m_StateFlagActionsBeforeLateUpdate == null) {
					m_StateFlagActionsBeforeLateUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunBeforeOnLateUpdate) {
							m_StateFlagActionsBeforeLateUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsBeforeLateUpdate;
			}
		}

		public List<StateFlagAction> SFAAfterLateUpdate {
			get { 
				if (m_StateFlagActionsAfterLateUpdate == null) {
					m_StateFlagActionsAfterLateUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunAfterOnLateUpdate) {
							m_StateFlagActionsAfterLateUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsAfterLateUpdate;
			}
		}

		public List<StateFlagAction> SFABeforeFixedUpdate {
			get { 
				if (m_StateFlagActionsBeforeFixedUpdate == null) {
					m_StateFlagActionsBeforeFixedUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunBeforeFixedUpdate) {
							m_StateFlagActionsBeforeFixedUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsBeforeFixedUpdate;
			}
		}

		public List<StateFlagAction> SFAAfterFixedUpdate {
			get { 
				if (m_StateFlagActionsAfterFixedUpdate == null) {
					m_StateFlagActionsAfterFixedUpdate = new List<StateFlagAction>();
					foreach (StateFlagAction sfa in this.StateFlagActions) {
						if (sfa.RunAfterFixedUpdate) {
							m_StateFlagActionsAfterFixedUpdate.Add(sfa);
						}
					}
				}
				return m_StateFlagActionsAfterFixedUpdate;
			}
		}

		private List<StateLogic> StateLogics {
			get { return m_StateLogics = m_StateLogics ?? new List<StateLogic>(GetComponentsInChildren<StateLogic>()); }
		}

		private FiniteStateMachine FiniteStateMachine {
			get { return m_FiniteStateMachine = m_FiniteStateMachine ?? GetComponentInParent<FiniteStateMachine>(); }
		}

		#endregion


		#region Methods

		public void Initialize() {
			foreach (StateLogic s in this.StateLogics) {
				if (this.FiniteStateMachine.CurrentState == this) {
					s.Initialize();
				}
			}
			foreach (StateTransitionCondition stc in this.StateTransitionConditions) {
				stc.Initialize();
			}
		}

		public void EnterState() {
			if (this.UpdateCheckChangeState(this.STCBeforeOnEnter)) {
				return;
			}
			this.UpdateFlagActions(this.SFABeforeOnEnter);
			
			foreach (StateLogic s in this.StateLogics) {
				if (this.FiniteStateMachine.CurrentState == this) {
					s.OnEnterState();
				}
			}

			if (this.UpdateCheckChangeState(this.STCAfterOnEnter)) {
				return;
			}
			this.UpdateFlagActions(this.SFAAfterOnEnter);
		}

		public void ExitState() {
			foreach (StateLogic s in this.StateLogics) {
				s.OnExitState();
			}
		}

		public void CheckStateTransitionCondition_Before_Update() {
			this.UpdateCheckChangeState(this.STCBeforeUpdate);
		}

		public void CheckStateTransitionCondition_After_Update() {
			this.UpdateCheckChangeState(this.STCAfterUpdate);
		}

		public void CheckStateTransitionCondition_Before_LateUpdate() {
			this.UpdateCheckChangeState(this.STCBeforeLateUpdate);
		}

		public void CheckStateTransitionCondition_After_LateUpdate() {
			this.UpdateCheckChangeState(this.STCAfterLateUpdate);
		}

		public void CheckStateTransitionCondition_Before_FixedUpdate() {
			this.UpdateCheckChangeState(this.STCBeforeFixedUpdate);
		}

		public void CheckStateTransitionCondition_After_FixedUpdate() {
			this.UpdateCheckChangeState(this.STCAfterFixedUpdate);
		}

		public void UpdateFlagActions_Before_Update() {
			this.UpdateFlagActions(this.SFABeforeUpdate);
		}

		public void UpdateFlagActions_After_Update() {
			this.UpdateFlagActions(this.SFAAfterUpdate);
		}

		public void UpdateFlagActions_Before_LateUpdate() {
			this.UpdateFlagActions(this.SFABeforeLateUpdate);
		}

		public void UpdateFlagActions_After_LateUpdate() {
			this.UpdateFlagActions(this.SFAAfterLateUpdate);
		}

		public void UpdateFlagActions_Before_FixedUpdate() {
			this.UpdateFlagActions(this.SFABeforeFixedUpdate);
		}

		public void UpdateFlagActions_After_FixedUpdate() {
			this.UpdateFlagActions(this.SFAAfterFixedUpdate);
		}

		public void StateUpdate() {
			foreach (StateLogic s in this.StateLogics) {
				s.OnUpdateState();
			}
		}

		public void StateLateUpdate() {
			foreach (StateLogic s in this.StateLogics) {
				s.OnLateUpdateState();
			}
		}

		public void StateFixedUpdate() {
			foreach (StateLogic s in this.StateLogics) {
				s.OnFixedUpdateState();
			}
		}

		private bool UpdateCheckChangeState(List<StateTransitionCondition> stateTransitionCondition) {
			foreach (StateTransitionCondition stc in stateTransitionCondition) {
				if (stc.AllFlagsActive) {
					stc.ExecuteActions();
					if (stc.GoToState != null) {
						this.FiniteStateMachine.ChangeState(stc.GoToState);
						return true;
					} else {
						string rootCause = "(" + this.Transform.name + ") this STC does not have assigned a state to go to :: ";
						Transform parent = this.Transform;
						while (parent.parent != null) {
							parent = parent.parent;
							rootCause += parent.name + "<<<";

						}
						Debug.LogError(rootCause);
					}
				}
			}
			return false;
		}

		private void UpdateFlagActions(List<StateFlagAction> stateTransitionCondition) {
			foreach (StateFlagAction sfa in stateTransitionCondition) {
				if (sfa.AllFlagsActive) {
					sfa.ExecuteActions();
				}
			}
		}

		#endregion

	
	}

}