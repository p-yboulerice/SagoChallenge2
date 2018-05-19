namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(FiniteStateMachine), true)]
	public class FiniteStateMachineEditor : Editor {

		private string NewStateName = "";

		public override void OnInspectorGUI() {
			
			EditorGUILayout.Space();
			Rect space = EditorGUILayout.BeginHorizontal();
			space.width = 1000;
			space.height = 10;
			EditorGUI.DrawRect(space, new Color(0.2f, 0.41f, 0.74f, 1f));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();


			FiniteStateMachine fsm = target as FiniteStateMachine;

			EditorGUILayout.BeginHorizontal();

			this.NewStateName = GUILayout.TextField(this.NewStateName);
			if (GUILayout.Button("Add New State")) {
				GameObject go = new GameObject();
				go.name = "State_" + this.NewStateName;
				go.transform.parent = fsm.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				go.AddComponent<State>();

				GameObject logic = new GameObject();
				logic.name = "Logic";
				logic.transform.parent = go.transform;
				logic.transform.localPosition = Vector3.zero;
				logic.AddComponent<StateLogic>();

				if (fsm.DefautState == null) {
					fsm.DefautState = go.GetComponent<State>();
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Clean & Refresh")) {
				fsm.CleanAndRefresh();
			}

			EditorGUILayout.EndHorizontal();

		}
	}
}