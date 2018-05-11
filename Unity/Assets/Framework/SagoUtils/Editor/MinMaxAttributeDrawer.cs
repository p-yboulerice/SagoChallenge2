namespace SagoUtils {
	
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(MinMaxAttribute))]
	public class MinMaxAttributeDrawer : PropertyDrawer {


		#region Properties

		private bool IsInteger {
			get { return (attribute as MinMaxAttribute).IsInteger; }
		}

		private Vector2 Range {
			get { return (attribute as MinMaxAttribute).Range; }
		}

		#endregion


		#region Methods

		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {

			if (property.propertyType != SerializedPropertyType.Vector2) {
				EditorGUI.LabelField(rect, label, new GUIContent("Use MinMax with a Vector2."));
				return;
			}

			GUIContent propLabel = EditorGUI.BeginProperty(rect, label, property);

			if (this.IsInteger) {
				DrawIntMinMaxField(rect, property, propLabel);
			} else {
				DrawFloatMinMaxField(rect, property, propLabel);
			}

			EditorGUI.EndProperty();

		}

		private void DrawFloatMinMaxField(Rect rect, SerializedProperty property, GUIContent label) {
			
			EditorGUI.BeginChangeCheck();

			Vector2 range = this.Range;
			Vector2 propertyValue = property.vector2Value;
			float minValue = propertyValue[0];
			float maxValue = propertyValue[1];

			EditorGUI.PrefixLabel(rect, label);
			rect.position += new Vector2(EditorGUIUtility.labelWidth, 0f);
			rect.width -= EditorGUIUtility.labelWidth;

			const float numberFieldWidth = 50f;
			const float spacing = 5f;
			bool changed = false;

			Rect rectSlider = new Rect(rect.position.x, rect.position.y, Mathf.Max(0f, rect.width - 2f * numberFieldWidth - spacing), rect.height);
			EditorGUI.MinMaxSlider(rectSlider, ref minValue, ref maxValue, range[0], range[1]);

			Rect rectMin = new Rect(rectSlider.max.x + spacing, rect.position.y, Mathf.Min(rect.width * 0.5f, numberFieldWidth), rect.height);
			const string MinEdit = "Min";
			GUI.SetNextControlName(MinEdit);
			minValue = Mathf.Clamp(EditorGUI.FloatField(rectMin, minValue), range[0], range[1]);
			if (Event.current.isKey && IsEnter(Event.current.keyCode) && GUI.GetNameOfFocusedControl() == MinEdit) {
				if (minValue > maxValue) {
					maxValue = minValue;
					changed = true;
				}
			}

			Rect rectMax = new Rect(rectMin.max.x, rect.position.y, rectMin.width, rect.height);
			const string MaxEdit = "Max";
			GUI.SetNextControlName(MaxEdit);
			maxValue = Mathf.Clamp(EditorGUI.FloatField(rectMax, maxValue), range[0], range[1]);
			if (Event.current.isKey && IsEnter(Event.current.keyCode) && GUI.GetNameOfFocusedControl() == MaxEdit) {
				if (maxValue < minValue) {
					minValue = maxValue;
					changed = true;
				}
			}

			if (EditorGUI.EndChangeCheck() || changed) {
				property.vector2Value = new Vector2(minValue, maxValue);
			}
		}

		private void DrawIntMinMaxField(Rect rect, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginChangeCheck();

			Vector2 range = this.Range;
			int rangeMin = Mathf.FloorToInt(range[0]);
			int rangeMax = Mathf.FloorToInt(range[1]);

			Vector2 propertyValue = property.vector2Value;
			float minValueFloat = propertyValue[0];
			float maxValueFloat = propertyValue[1];

			EditorGUI.PrefixLabel(rect, label);
			rect.position += new Vector2(EditorGUIUtility.labelWidth, 0f);
			rect.width -= EditorGUIUtility.labelWidth;

			const float numberFieldWidth = 50f;
			const float spacing = 5f;
			bool changed = false;

			Rect rectSlider = new Rect(rect.position.x, rect.position.y, Mathf.Max(0f, rect.width - 2f * numberFieldWidth - spacing), rect.height);
			EditorGUI.MinMaxSlider(rectSlider, ref minValueFloat, ref maxValueFloat, rangeMin, rangeMax);

			int minValue = Mathf.RoundToInt(minValueFloat);
			int maxValue = Mathf.RoundToInt(maxValueFloat);

			Rect rectMin = new Rect(rectSlider.max.x + spacing, rect.position.y, Mathf.Min(rect.width * 0.5f, numberFieldWidth), rect.height);
			const string MinEdit = "Min";
			GUI.SetNextControlName(MinEdit);
			minValue = Mathf.Clamp(EditorGUI.IntField(rectMin, minValue), rangeMin, rangeMax);
			if (Event.current.isKey && IsEnter(Event.current.keyCode) && GUI.GetNameOfFocusedControl() == MinEdit) {
				if (minValue > maxValue) {
					maxValue = minValue;
					changed = true;
				}
			}

			Rect rectMax = new Rect(rectMin.max.x, rect.position.y, rectMin.width, rect.height);
			const string MaxEdit = "Max";
			GUI.SetNextControlName(MaxEdit);
			maxValue = Mathf.Clamp(EditorGUI.IntField(rectMax, maxValue), rangeMin, rangeMax);
			if (Event.current.isKey && IsEnter(Event.current.keyCode) && GUI.GetNameOfFocusedControl() == MaxEdit) {
				if (maxValue < minValue) {
					minValue = maxValue;
					changed = true;
				}
			}

			if (EditorGUI.EndChangeCheck() || changed) {
				property.vector2Value = new Vector2(minValue, maxValue);
			}
		}

		private static bool IsEnter(KeyCode code) {
			return code == KeyCode.Return || code == KeyCode.KeypadEnter;
		}

		#endregion

		
	}
	
}
