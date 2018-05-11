namespace SagoUtilsEditor {
	
	using SagoUtils;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributeDrawer : PropertyDrawer {


		#region GUI

		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, label, property);

			System.Enum value;
			value = (System.Enum)System.Enum.ToObject(this.Attribute.EnumType, property.intValue);

			EditorGUI.BeginChangeCheck();

			int newValue = System.Convert.ToInt32(EditorGUI.EnumFlagsField(position, label, value));

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = newValue;
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Helper

		protected EnumMaskAttribute Attribute {
			get { return this.attribute as EnumMaskAttribute; }
		}

		#endregion


	}
}
