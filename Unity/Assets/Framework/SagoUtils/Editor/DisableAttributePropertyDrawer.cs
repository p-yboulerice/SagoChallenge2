namespace SagoUtilsEditor {

	using SagoUtils;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// <see cref="DisableAttribute"/> property drawer.  A property in the default
	/// inspector can be disabled or hidden based on a user-defined callback.
	/// </summary>
	[CustomPropertyDrawer(typeof(DisableAttribute))]
	public class DisableAttributePropertyDrawer : PropertyDrawer {

		DisableAttribute Attribute { 
			get { 
				return (attribute as DisableAttribute); 
			}
		}
		
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
			
			if (IsVisible(prop)) {
				
				EditorGUI.indentLevel += this.Attribute.Indent;
				
				EditorGUI.BeginDisabledGroup(IsDisabled(prop));
				
				EditorGUI.PropertyField(pos, prop, label, prop.isExpanded);
				
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.indentLevel -= this.Attribute.Indent;
				
			}
		}
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
			if (IsVisible(prop)) {
				return EditorGUI.GetPropertyHeight(prop, label, prop.isExpanded);
			} else {
				return 0.0f;
			}
		}
		
		public bool IsDisabled(SerializedProperty prop) {
			if (this.Attribute.IsDisabled != null) {
				return this.Attribute.IsDisabled(prop.serializedObject.targetObject);
			} else {
				return false;
			}
		}
		
		public bool IsVisible(SerializedProperty prop) {
			return !(this.Attribute.HideWhenDisabled && IsDisabled(prop));
		}
	}

}