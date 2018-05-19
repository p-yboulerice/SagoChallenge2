namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class StateLogic : MonoBehaviour {

		#region Fields

		[SerializeField]
		private List<StateAction> m_OnEnterActions;

		[SerializeField]
		private List<StateAction> m_OnUpdateActions;

		[SerializeField]
		private List<StateAction> m_OnLateUpdateActions;

		[SerializeField]
		private List<StateAction> m_OnFixedUpdateActions;

		[SerializeField]
		private List<StateAction> m_OnExitActions;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private FiniteStateMachine m_FiniteStateMachine;

		[System.NonSerialized]
		private State m_State;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		protected FiniteStateMachine FiniteStateMachine {
			get { return m_FiniteStateMachine = m_FiniteStateMachine ?? this.Transform.GetComponentInParent<FiniteStateMachine>(); }
		}

		protected State State {
			get { return m_State = m_State ?? GetComponent<State>(); }
		}

		private List<StateAction> OnEnterActions {
			get {
				
				return m_OnEnterActions = m_OnEnterActions ?? new List<StateAction>();
			}
		}

		private List<StateAction> OnUpdateActions {
			get { 

				return m_OnUpdateActions = m_OnUpdateActions ?? new List<StateAction>();
			}
		}

		private List<StateAction> OnLateUpdateActions {
			get { 
	
				return m_OnLateUpdateActions = m_OnLateUpdateActions ?? new List<StateAction>();
			}
		}

		private List<StateAction> OnFixedUpdateActions {
			get { 
				return m_OnFixedUpdateActions = m_OnFixedUpdateActions ?? new List<StateAction>();
			}
		}

		private List<StateAction> OnExitActions {
			get { 

				return m_OnExitActions = m_OnExitActions ?? new List<StateAction>();
			}
		}

		#endregion


		#region Methods

		public void Initialize() {
			foreach (StateAction a in this.OnEnterActions) {
				a.Initialize();
			}
			foreach (StateAction a in this.OnUpdateActions) {
				a.Initialize();
			}
			foreach (StateAction a in this.OnLateUpdateActions) {
				a.Initialize();
			}
			foreach (StateAction a in this.OnExitActions) {
				a.Initialize();
			}
		}

		public void OnEnterState() {
			for (int i = 0; i < this.OnEnterActions.Count; i++) {
				this.OnEnterActions[i].Run();
			}
		}

		public void OnUpdateState() {
			for (int i = 0; i < this.OnUpdateActions.Count; i++) {
				this.OnUpdateActions[i].Run();
			}
		}

		public void OnLateUpdateState() {
			for (int i = 0; i < this.OnLateUpdateActions.Count; i++) {
				this.OnLateUpdateActions[i].Run();
			}
		}

		public void OnFixedUpdateState() {
			for (int i = 0; i < this.OnFixedUpdateActions.Count; i++) {
				this.OnFixedUpdateActions[i].Run();
			}
		}

		public void OnExitState() {
			for (int i = 0; i < this.OnExitActions.Count; i++) {
				this.OnExitActions[i].Run();
			}
		}

		public string ContainsAction(StateAction action) {

			string actionInfo = "";

			foreach (StateAction a in OnEnterActions) {
				if (a == action) {
					actionInfo += ":  Enter (" + this.OnEnterActions.IndexOf(a).ToString() + ")  ";
				}
			}

			foreach (StateAction a in OnUpdateActions) {
				if (a == action) {
					actionInfo += ":  Update (" + this.OnUpdateActions.IndexOf(a).ToString() + ")  ";
				}
			}

			foreach (StateAction a in OnLateUpdateActions) {
				if (a == action) {
					actionInfo += ":  LateUpdate (" + this.OnLateUpdateActions.IndexOf(a).ToString() + ")  ";
				}
			}

			foreach (StateAction a in OnFixedUpdateActions) {
				if (a == action) {
					actionInfo += ":  FixedUpdate (" + this.OnFixedUpdateActions.IndexOf(a).ToString() + ")  ";
				}
			}

			foreach (StateAction a in OnExitActions) {
				if (a == action) {
					actionInfo += ":  Exit (" + this.OnExitActions.IndexOf(a).ToString() + ")  ";
				}		
			}

			return actionInfo;
		}

		public bool CleanAndRefresh() {
			
			bool removedNullActions = false;

			foreach (StateAction sa in this.OnEnterActions.ToArray()) {
				if (sa == null) {
					this.OnEnterActions.Remove(sa);
					removedNullActions = true;
				}
			}

			foreach (StateAction sa in this.OnUpdateActions.ToArray()) {
				if (sa == null) {
					this.OnUpdateActions.Remove(sa);
					removedNullActions = true;
				}
			}

			foreach (StateAction sa in this.OnLateUpdateActions.ToArray()) {
				if (sa == null) {
					this.OnLateUpdateActions.Remove(sa);
					removedNullActions = true;
				}
			}

			foreach (StateAction sa in this.OnExitActions.ToArray()) {
				if (sa == null) {
					this.OnExitActions.Remove(sa);
					removedNullActions = true;
				}
			}

			return removedNullActions;
		}

		#endregion

	
	}

}