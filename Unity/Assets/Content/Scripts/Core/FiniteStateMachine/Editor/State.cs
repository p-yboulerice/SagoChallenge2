namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Juice.FSM;

	[CustomEditor(typeof(State), true)]
	public class StateEditor : Editor {


		private string LogicName = "Logic";
		private string TrasitionName = "Transition";
		private string FlagActionName = "FlagAction";

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

			EditorGUILayout.BeginHorizontal();

			this.LogicName = GUILayout.TextField(this.LogicName);
			if (GUILayout.Button("Add Logic")) {
				GameObject go = new GameObject(this.LogicName);
				go.transform.parent = (target as State).transform;
				go.AddComponent<StateLogic>();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();

			this.TrasitionName = GUILayout.TextField(this.TrasitionName);
			if (GUILayout.Button("Add Transition Condition")) {
				GameObject go = new GameObject("=>" + TrasitionName);
				go.transform.parent = (target as State).transform;
				go.AddComponent<StateTransitionCondition>();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();

			this.FlagActionName = GUILayout.TextField(this.FlagActionName);
			if (GUILayout.Button("Add Flag Action")) {
				GameObject go = new GameObject(">>>" + FlagActionName);
				go.transform.parent = (target as State).transform;
				go.AddComponent<StateFlagAction>();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
		}
	}
}