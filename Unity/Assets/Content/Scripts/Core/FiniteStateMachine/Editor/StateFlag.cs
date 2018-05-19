namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(StateFlag), true)]
	[CanEditMultipleObjects]
	public class StateFlagEditor : Editor {
		public override void OnInspectorGUI() {

			StateFlag flag = this.target as StateFlag;
			StateTransitionCondition stateTransitionCondition = flag.GetComponent<StateTransitionCondition>();
			StateFlagAction stateFlagAction = flag.GetComponent<StateFlagAction>();

			if (stateTransitionCondition != null || stateFlagAction != null) {
				EditorGUILayout.Space();
				Rect space = EditorGUILayout.BeginVertical();
				space.width = 1000;
				space.height = 10;
				if ((stateTransitionCondition != null && stateTransitionCondition.ContainsFlag(flag)) || (stateFlagAction != null && stateFlagAction.ContainsFlag(flag))) {
					EditorGUI.DrawRect(space, Color.yellow);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
				} else {
					EditorGUI.DrawRect(space, Color.gray);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}

				EditorGUILayout.Space();
			} else {
				Rect space = EditorGUILayout.BeginVertical();
				EditorGUI.DrawRect(space, Color.gray);
				EditorGUILayout.LabelField("THIS ACTION MUST BE ADDED TO A GAMEOBJECT WITH StateTransitionCondition/StateFlagAction SCRIPT ATTACHED TO IT");
				EditorGUILayout.EndVertical();
			}

			base.OnInspectorGUI();
			
		}
	}
}