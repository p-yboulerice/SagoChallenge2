namespace Juice.Utils {
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;

	public class DesignerAttributeWindow : EditorWindow {

		private Vector2 Scroller;
		private List<TweakerObject> m_DesignerObjects;
		private Dictionary<string, List<TweakerObjectProperties>> m_Groups;
		private Dictionary<string, bool> m_GroupToggle;
		private List<string> m_Keys;


		private List<TweakerObject> DesignerObjects {
			get { return m_DesignerObjects; }
			set { m_DesignerObjects = value; }
		}

		private Dictionary<string, List<TweakerObjectProperties>> Groups {
			get { return m_Groups = m_Groups ?? new Dictionary<string, List<TweakerObjectProperties>>(); }
			set { m_Groups = value; }
		}

		private Dictionary<string, bool> GroupToggle {
			get { return m_GroupToggle = m_GroupToggle ?? new Dictionary<string, bool>(); }
			set { m_GroupToggle = value; }
		}

		private List<string> Keys {
			get { return m_Keys = m_Keys ?? new List<string>(); }
			set { m_Keys = value; }
		}

		[MenuItem("Sago/Tweaker")]
		static void Init() {

			DesignerAttributeWindow window = (DesignerAttributeWindow)EditorWindow.GetWindow<DesignerAttributeWindow>("Tweaker");
			window.Show();
		}

		private void RefreshDesignerObjects() {
			this.DesignerObjects = new List<TweakerObject>(GameObject.FindObjectsOfType<TweakerObject>());
			this.Groups = null;
			this.GroupToggle = null;
			this.Keys = null;

			foreach (TweakerObject d in this.DesignerObjects) {
				if (d == null) {
					this.DesignerObjects = null;
					break;
				}
				TweakerObjectProperties dop = new TweakerObjectProperties();
				dop.Initialize(d);

				string groupKey = d.Group == "" ? "NoGroup" : d.Group;
				if (!this.Groups.ContainsKey(groupKey)) {
					this.Groups.Add(groupKey, new List<TweakerObjectProperties>());
					this.GroupToggle.Add(groupKey, false);
					this.Keys.Add(groupKey);
				}
				this.Groups[groupKey].Add(dop);
			}

			this.Keys.Sort((a,b) => a.CompareTo(b));

		}

		void OnGUI() {

			if (GUILayout.Button("Refresh List <3") || this.DesignerObjects == null || Event.current.commandName=="Delete") {
				this.RefreshDesignerObjects();
			}


			GUILayout.BeginVertical();
			this.Scroller = GUILayout.BeginScrollView(this.Scroller, GUIStyle.none);

			foreach (KeyValuePair<string, List<TweakerObjectProperties>> k in this.Groups) {
				Rect r = GUILayoutUtility.GetRect(new GUIContent(k.Key), GUIStyle.none);
				Rect pr = GUILayoutUtility.GetLastRect();
				r.position = new Vector2(r.position.x, pr.position.y + pr.height + 7);
				this.GroupToggle[k.Key] = (EditorGUI.Foldout(r, this.GroupToggle[k.Key], k.Key));

				if (this.GroupToggle[k.Key]) {
					GUILayout.Space(8);
					foreach (TweakerObjectProperties dop in k.Value) {
						if (!dop.DrawGUI()) {
							this.RefreshDesignerObjects();
							return;
						}
					}
				}
			}
				
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}
			
	}

}