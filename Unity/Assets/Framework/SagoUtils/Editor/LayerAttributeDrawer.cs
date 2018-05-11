namespace SagoUtilsEditor {

	using SagoUtils;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeDrawer : PropertyDrawer {

		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			property.intValue = EditorGUI.LayerField(position, label, property.intValue);
			EditorGUI.EndProperty();
		}

	}

}
