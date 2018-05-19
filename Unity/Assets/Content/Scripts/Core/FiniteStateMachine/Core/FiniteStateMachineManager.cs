namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class FiniteStateMachineManager : MonoBehaviour {


		#region Singleton

		[System.NonSerialized]
		private static FiniteStateMachineManager s_Instance;
		private static bool s_Running = true;

		public static FiniteStateMachineManager Instance {
			get {
				#if !UNITY_TVOS
				if (!UnityEngine.Application.isPlaying) {
					return null;
				}

				if (!s_Running) {
					return null;
				}
	
				if (s_Instance == null) {
					s_Instance = UnityEngine.GameObject.FindObjectOfType(typeof(FiniteStateMachineManager)) as FiniteStateMachineManager;
				}
				if (s_Instance == null) {
					s_Instance = new UnityEngine.GameObject().AddComponent<FiniteStateMachineManager>();
					s_Instance.name = "FiniteStateMachineManager";
					DontDestroyOnLoad(s_Instance.gameObject);
				}
				return s_Instance;
				#else
			return null;
				#endif
			}
		}

		#endregion


		#region Fields

		[System.NonSerialized]
		private List<FiniteStateMachine> m_FiniteStateMachines;

		#endregion


		#region Properties

		private List<FiniteStateMachine> FiniteStateMachines {
			get { return m_FiniteStateMachines = m_FiniteStateMachines ?? new List<Juice.FSM.FiniteStateMachine>(); }
		}

		#endregion


		#region Methods

		public void RegisterFiniteStateMachine(FiniteStateMachine fsm) {
			if (!this.FiniteStateMachines.Contains(fsm)) {
				this.FiniteStateMachines.Add(fsm);
				this.FiniteStateMachines.Sort(ListSortCompare_FSMPriority);
			}
		}

		public void UnregisterFiniteStateMachine(FiniteStateMachine fsm) {
			if (this.FiniteStateMachines.Contains(fsm)) {
				this.FiniteStateMachines.Remove(fsm);
			}
		}

		void Update() {
			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_Before_Update();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_Before_Update();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateState();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_After_Update();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_After_Update();
			}
		}

		void LateUpdate() {
			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_Before_LateUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_Before_LateUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.LateUpdateState();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_After_LateUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_After_LateUpdate();
			}
		}

		void FixedUpdate() {
			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_Before_FixedUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_Before_FixedUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.FixedUpdateState();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.StateChange_After_FixedUpdate();
			}

			foreach (FiniteStateMachine fsm in this.FiniteStateMachines.ToArray()) {
				fsm.UpdateStateFlagAction_After_FixedUpdate();
			}
		}

		private int ListSortCompare_FSMPriority(FiniteStateMachine a, FiniteStateMachine b) {
			if (a.RunPriority > b.RunPriority) {
				return 1;
			} else if (a.RunPriority < b.RunPriority) {
				return -1;
			}
			return 0;
		}

		void OnApplicationQuit() {
			s_Instance = null; 
			s_Running = false;
		}

		#endregion


	}
}