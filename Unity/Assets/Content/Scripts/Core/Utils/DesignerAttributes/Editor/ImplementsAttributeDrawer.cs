namespace Juice.Utils {

	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ImplementsAttribute))]
	public class ImplementsAttributeDrawer : PropertyDrawer {


		#region Properties

		private System.Type ObjectType {
			get { return (attribute as ImplementsAttribute).ObjectType; }
		}

		private System.Type InterfaceType {
			get { return (attribute as ImplementsAttribute).InterfaceType; }
		}

		#endregion


		#region Methods

		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			
			EditorGUI.BeginProperty(rect, label, property);
			EditorGUI.BeginChangeCheck();

			label.text = string.Format("{0} ({1})", label.text, this.InterfaceType.Name);

			Object o;
			o = EditorGUI.ObjectField(rect, label, property.objectReferenceValue, this.ObjectType, true);

			if (EditorGUI.EndChangeCheck()) {
				
				if (o == null || DoesImplement(o, this.InterfaceType)) {
					property.objectReferenceValue = FindObjectImplementing(o, this.InterfaceType);
				}

			}

			EditorGUI.EndProperty();

		}

		private bool DoesImplement(Object o, System.Type interfaceType) {
			o = FindObjectImplementing(o, interfaceType);
			return (o != null);
		}

		private Object FindObjectImplementing(Object o, System.Type interfaceType) {

			if (o is Component && !interfaceType.IsAssignableFrom(o.GetType())) {
				o = (o as Component).gameObject;
			}

			if (o is GameObject) {
				o = (o as GameObject).GetComponent(interfaceType);
			}

			if (o != null && !interfaceType.IsAssignableFrom(o.GetType())) {
				o = null;
			}

			return o;

		}

		#endregion


	}

}
