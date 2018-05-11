namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine;
	
	[CustomPropertyDrawer(typeof(ProductInfoVersionAttribute))]
	public class ProductInfoVersionAttributeDrawer : PropertyDrawer {
		
		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			string value = property.stringValue;
			value = EditorGUI.TextField(position, label, value);
			value = ProductInfo.CheckVersion(value);
			property.stringValue = value;
			EditorGUI.EndProperty();
		}
		
	}
	
}