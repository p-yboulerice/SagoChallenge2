namespace Juice.Utils {
	
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SubclassAttribute))]
	public class SubclassAttributeDrawer : PropertyDrawer {


		#region Static Methods

		private static List<System.Type> GetSubclasses(System.Type baseType) {
			if (!Subclasses.ContainsKey(baseType)) {
				Subclasses.Add(baseType, FindSubclasses(baseType));
			}
			return Subclasses[baseType];
		}

		public static List<System.Type> FindSubclasses(System.Type baseType) {
			return System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.BaseType == baseType).ToList<System.Type>();
		}

		#endregion


		#region Static Properties

		private static Dictionary<System.Type, List<System.Type>> Subclasses {
			get {
				s_Subclasses = s_Subclasses ?? new Dictionary<System.Type, List<System.Type>>();
				return s_Subclasses;
			}
		}

		#endregion


		#region Static Fields

		private static Dictionary<System.Type, List<System.Type>> s_Subclasses;

		#endregion


		#region Properties

		private System.Type BaseType {
			get { return (attribute as SubclassAttribute).BaseType; }
		}

		#endregion


		#region Methods

		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			
			List<System.Type> types;
			types = GetSubclasses(this.BaseType);

			int typeIndex;
			typeIndex = types.FindIndex(t => t.AssemblyQualifiedName == property.stringValue);

			int nameIndex;
			nameIndex = typeIndex + 1;

			List<string> displayNames;
			displayNames = GetDisplayNames(types);

			EditorGUI.BeginProperty(rect, label, property);
			EditorGUI.BeginChangeCheck();

			typeIndex = EditorGUI.Popup(rect, label.text, nameIndex, displayNames.ToArray()) - 1;

			if (EditorGUI.EndChangeCheck()) {

				System.Type selectedType;
				selectedType = (typeIndex < 0) ? null : types[typeIndex];

				property.stringValue = GetSerializedName(selectedType);

			}

			EditorGUI.EndProperty();

		}

		private List<string> GetDisplayNames(List<System.Type> types) {

			List<string> names;
			names = new List<string>() { "None" };

			foreach (System.Type type in types) {
				names.Add(ObjectNames.NicifyVariableName(type.Name));
			}

			return names;

		}

		private string GetSerializedName(System.Type type) {
			return SubclassAttribute.GetSerializedName(type);
		}

		#endregion


	}

}
