namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(StateFlagAction), true)]
	public class StateFlagActionEditor : Editor {
		public override void OnInspectorGUI() {

			EditorGUILayout.Space();
			Rect space = EditorGUILayout.BeginHorizontal();
			space.width = 1000;
			space.height = 10;
			EditorGUI.DrawRect(space, new Color(1, 0.5f, 0f, 1));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			base.OnInspectorGUI();

		}
	}
}