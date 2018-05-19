namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(StateAction), true)]
	[CanEditMultipleObjects]
	public class StateActionEditor : Editor {


		private StateLogic m_StateLogic;
		private StateTransitionCondition m_STC;
		private StateFlagAction m_SFA;
		private string m_LogicInfo;

		public override void OnInspectorGUI() {
			StateAction action = this.target as StateAction;

			if (Event.current.type == EventType.Layout) {
				m_StateLogic = action.GetComponent<StateLogic>();
				m_STC = action.GetComponent<StateTransitionCondition>();
				m_SFA = action.GetComponent<StateFlagAction>();
				if (m_StateLogic != null) {
					m_LogicInfo = m_StateLogic.ContainsAction(action);
				} else if (m_STC != null) {
					m_LogicInfo = "STC";
				} else if (m_SFA != null) {
					m_LogicInfo = "SFA";
				}


			}

			EditorGUILayout.Space();
			if (serializedObject.isEditingMultipleObjects) {
				EditorGUILayout.LabelField("Multiple selected");
			} else if (m_StateLogic != null || m_STC != null || m_SFA != null) {
				Rect space = EditorGUILayout.BeginVertical();
				space.height = 10;
				if (!string.IsNullOrEmpty(m_LogicInfo)) {
					EditorGUI.DrawRect(space, new Color(0.17f, 0.74f, 0.43f, 1));
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.LabelField(m_LogicInfo);
				} else {
					EditorGUI.DrawRect(space, Color.gray);
					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.Space();
			} else {
				Rect space = EditorGUILayout.BeginVertical();
				EditorGUI.DrawRect(space, Color.gray);
				EditorGUILayout.LabelField("THIS ACTION MUST BE ADDED TO A GAMEOBJECT WITH StateLogic/StateTransitionCondition/StateFlagAction SCRIPT ATTACHED TO IT");
				EditorGUILayout.EndVertical();
			}
			base.OnInspectorGUI();

			if (Event.current.type == EventType.Repaint) {
				m_StateLogic = null;
				m_STC = null;
				m_LogicInfo = null;
			}
		}
	}
}