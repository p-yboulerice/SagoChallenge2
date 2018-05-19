namespace Juice.Utils {
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;


	public class TweakerObjectProperties {


		#region Fields

		private Dictionary<MonoBehaviour, TweakerObjectPorpertiesMono> m_Properties;

		private GUIStyle m_GuiStyleBox;

		private GUIStyle m_GuiBoldLabel;


		#endregion


		#region Properties

		private TweakerObject DesignerObject {
			get;
			set;
		}

		private Dictionary<MonoBehaviour, TweakerObjectPorpertiesMono> Properties {
			get { return m_Properties = m_Properties ?? new Dictionary<MonoBehaviour, TweakerObjectPorpertiesMono>(); }
			set { m_Properties = value; }
		}

		private  bool Toggle {
			get;
			set;
		}

		private GUIStyle GuiStyleBox {
			get { return m_GuiStyleBox = m_GuiStyleBox ?? new GUIStyle(GUI.skin.box); }
		}

		private GUIStyle GuiStyleBoldLabel{
			get { 
				if (m_GuiBoldLabel == null) {
					m_GuiBoldLabel = m_GuiBoldLabel ?? new GUIStyle(GUI.skin.label); 
					m_GuiBoldLabel.fontStyle = FontStyle.Bold;
				}
				return m_GuiBoldLabel;
			}
		}

		#endregion


		#region Methods

		public void Initialize(TweakerObject designerObject) {

			this.DesignerObject = designerObject;


			MonoBehaviour[] monos = designerObject.GetComponents<MonoBehaviour>();

			foreach (MonoBehaviour m in monos) {

				SerializedObject so = new SerializedObject(m);
			
				FieldInfo[] fis = m.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

				foreach (FieldInfo f in fis) {

					SerializedProperty sp = so.FindProperty(f.Name);

					object a;
					GUIContent g = new GUIContent(this.GetCleanName(f.Name));

					object[] attributes = f.GetCustomAttributes(typeof(TweakerAttribute), true);
					a = attributes.Length > 0 ? attributes[0] : null;
					if (a != null) {
						this.CheckKey(m);
						if (a is TweakerFloatRangeAttribute) {
							this.Properties[m].AddFloat(m, so, sp, g, ((TweakerFloatRangeAttribute)a).Min, ((TweakerFloatRangeAttribute)a).Max);
						} else if (a is TweakerIntRangeAttribute) {
							this.Properties[m].AddInt(m, so, sp, g, ((TweakerIntRangeAttribute)a).Min, ((TweakerIntRangeAttribute)a).Max);
						} else {
							this.Properties[m].AddProperty(m, so, sp, g);
						}
					}
				}
			}
		}

		private void CheckKey(MonoBehaviour m) {
			if (!this.Properties.ContainsKey(m)) {
				this.Properties.Add(m, new TweakerObjectPorpertiesMono());
			}
		}

		private string GetCleanName(string name) {
			char[] n = name.ToCharArray();
			if (n.Length > 2) {
				if (n[0] == 'm' && n[1] == '_') {
					name = name.Remove(0, 2);
				}
			}
			return name;
		}

		#endregion


		#region Draw

		private string GetLabel(MonoBehaviour m) {
			string s = m.ToString();
			char[] c = s.ToCharArray();
			for (int i = c.Length - 1; i >= 0; i--) {
				if (c[i] == '.') {
					s = s.Remove(0, i + 1);
					break;
				}
			}
			s = s.Replace(')', ' ');
			return s;
		}

		public bool DrawGUI() {

			if (this.DesignerObject == null) {
				return false;
			}


			Rect r = GUILayoutUtility.GetRect(new GUIContent(this.DesignerObject.name), GUIStyle.none);
			Rect pr = GUILayoutUtility.GetLastRect();
			r.position = new Vector2(r.position.x + 12, pr.position.y + pr.height);
			this.Toggle = (EditorGUI.Foldout(r, this.Toggle, this.DesignerObject.name));


			if (this.Toggle) {
				GUILayout.Space(20);
				GUILayout.BeginVertical(this.GuiStyleBox);
				if (GUILayout.Button(this.DesignerObject.name)) {
					Selection.activeGameObject = this.DesignerObject.gameObject;
				}
				foreach (KeyValuePair<MonoBehaviour, TweakerObjectPorpertiesMono> k in this.Properties) {
					string label = this.GetLabel(k.Key);
					GUILayout.Label(label, this.GuiStyleBoldLabel);		
					k.Value.DrawGUI();
					GUILayout.Space(16);
				}
				GUILayout.EndVertical();
			}

			return true;
		}

		#endregion


	}

}