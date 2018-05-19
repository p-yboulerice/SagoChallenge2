namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(StateLogic), true)]
	public class StateLogicEditor : Editor {
		public override void OnInspectorGUI() {

			EditorGUILayout.Space();
			Rect space = EditorGUILayout.BeginHorizontal();
			space.width = 1000;
			space.height = 10;
			EditorGUI.DrawRect(space, new Color(0.37f, 0.7f, 0.16f, 1f));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
			
		}
	}
}