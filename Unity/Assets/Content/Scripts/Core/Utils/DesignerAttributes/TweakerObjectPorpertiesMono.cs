namespace Juice.Utils {
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;


	public class TweakerObjectPorpertiesMono {


		#region Fields

		private Dictionary<MonoBehaviour, List<TweakerProperty>> m_Property;

		private Dictionary<MonoBehaviour, List<TweakerFloat>> m_Float;

		private Dictionary<MonoBehaviour, List<TweakerInt>> m_Int;

		private GUIStyle m_GuiStyle;

		#endregion


		#region Properties

		private Dictionary<MonoBehaviour, List<TweakerProperty>> Property {
			get { return m_Property = m_Property ?? new Dictionary<MonoBehaviour, List<TweakerProperty>>(); }	
		}

		private Dictionary<MonoBehaviour, List<TweakerFloat>> Float {
			get { return m_Float = m_Float ?? new Dictionary<MonoBehaviour, List<TweakerFloat>>(); }	
		}

		private Dictionary<MonoBehaviour, List<TweakerInt>> Int {
			get { return m_Int = m_Int ?? new Dictionary<MonoBehaviour, List<TweakerInt>>(); }
		}

		private GUIStyle GuiStyle {
			get { return m_GuiStyle = m_GuiStyle ?? new GUIStyle(GUI.skin.box); }
		}

		#endregion


		#region Methods

		public void AddProperty(MonoBehaviour m, SerializedObject so, SerializedProperty sp, GUIContent label) {

			TweakerProperty df = new TweakerProperty();
			df.SerializedObject = so;
			df.SerializedProperty = sp;
			df.Label = label;

			if (!this.Property.ContainsKey(m)) {
				this.Property.Add(m, new List<TweakerProperty>());
			}

			this.Property[m].Add(df);

		}

		public void AddFloat(MonoBehaviour m, SerializedObject so, SerializedProperty sp, GUIContent label, float min, float max) {

			TweakerFloat df = new TweakerFloat();
			df.SerializedObject = so;
			df.SerializedProperty = sp;
			df.Label = label;
			df.Min = min;
			df.Max = max;

			if (!this.Float.ContainsKey(m)) {
				this.Float.Add(m, new List<TweakerFloat>());
			}

			this.Float[m].Add(df);

		}

		public void AddInt(MonoBehaviour m, SerializedObject so, SerializedProperty sp, GUIContent label, int min, int max) {
			TweakerInt df = new TweakerInt();
			df.SerializedObject = so;
			df.SerializedProperty = sp;
			df.Label = label;
			df.Min = min;
			df.Max = max;

			if (!this.Int.ContainsKey(m)) {
				this.Int.Add(m, new List<TweakerInt>());
			}

			this.Int[m].Add(df);
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

		private void DrawLabel(MonoBehaviour m) {
			string s = m.ToString();
			char[] c = s.ToCharArray();
			for (int i = c.Length - 1; i >= 0; i--) {
				if (c[i] == '.') {
					s = s.Remove(0, i + 1);
					break;
				}
			}
			s = s.Replace(')', ' ');
			GUILayout.Label(s);		
		}

		public void DrawGUI() {
			
			GUILayout.BeginVertical(this.GuiStyle);

			foreach (KeyValuePair<MonoBehaviour, List<TweakerFloat>> k in this.Float) {
				foreach (TweakerFloat d in k.Value) {
					d.SerializedObject.Update();
					this.DrawFloatSlider(d.Label, d.SerializedProperty, d.Min, d.Max);
					d.SerializedObject.ApplyModifiedProperties();
				}
			}

			foreach (KeyValuePair<MonoBehaviour, List<TweakerInt>> k in this.Int) {
				foreach (TweakerInt d in k.Value) {
					d.SerializedObject.Update();
					this.DrawIntSlider(d.Label, d.SerializedProperty, d.Min, d.Max);
					d.SerializedObject.ApplyModifiedProperties();
				}
			}

			foreach (KeyValuePair<MonoBehaviour, List<TweakerProperty>> k in this.Property) {
				foreach (TweakerProperty d in k.Value) {
					d.SerializedObject.Update();
					this.DrawDefault(d.Label, d.SerializedProperty);
					d.SerializedObject.ApplyModifiedProperties();
				}
			}

			GUILayout.EndVertical();

		}

		private void DrawDefault(GUIContent label, SerializedProperty property) {
			EditorGUIUtility.wideMode = true;
			EditorGUILayout.PropertyField(property);
		}

		private void DrawFloatSlider(GUIContent label, SerializedProperty property, float min, float max) {

			EditorGUI.BeginChangeCheck();

			float newValue = EditorGUILayout.Slider(label.text, property.floatValue, min, max);

			if (EditorGUI.EndChangeCheck()) {
				property.floatValue = newValue;
			}

		}

		private void DrawIntSlider(GUIContent label, SerializedProperty property, int min, int max) {

			EditorGUI.BeginChangeCheck();

			float newValue = EditorGUILayout.IntSlider(label.text, property.intValue, min, max);

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = (int)newValue;
			}
		}


		#endregion


	}
		
	internal struct TweakerProperty {
		public MonoBehaviour MonoBehaviour;
		public SerializedObject SerializedObject;
		public SerializedProperty SerializedProperty;
		public GUIContent Label;
	}

	internal struct TweakerFloat {
		public MonoBehaviour MonoBehaviour;
		public SerializedObject SerializedObject;
		public SerializedProperty SerializedProperty;
		public GUIContent Label;
		public float Min;
		public float Max;
	}

	internal struct TweakerInt {
		public MonoBehaviour MonoBehaviour;
		public SerializedObject SerializedObject;
		public SerializedProperty SerializedProperty;
		public GUIContent Label;
		public int Min;
		public int Max;
	}

	internal struct DesignerBool {
		public MonoBehaviour MonoBehaviour;
		public SerializedObject SerializedObject;
		public SerializedProperty SerializedProperty;
		public GUIContent Label;
	}

}