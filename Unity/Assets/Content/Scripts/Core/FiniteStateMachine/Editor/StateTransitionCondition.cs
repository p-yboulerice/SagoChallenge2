namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(StateTransitionCondition), true)]
	public class StateTransitionConditionEditor : Editor {
		public override void OnInspectorGUI() {

			EditorGUILayout.Space();
			Rect space = EditorGUILayout.BeginHorizontal();
			space.width = 1000;
			space.height = 10;
			EditorGUI.DrawRect(space, new Color(0.7f, 0.23f, 0.19f, 1));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
			
		}
	}
}