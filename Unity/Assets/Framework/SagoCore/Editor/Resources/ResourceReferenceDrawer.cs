namespace SagoCoreEditor.Resources {
	
	using SagoCore.Resources;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The ResourceReferenceDrawer class provides a custom inspector for <see cref="ResourceReference" /> properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(ResourceReference), true)]
	public class ResourceReferenceDrawer : PropertyDrawer {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the type of asset that may be referenced.
		/// </summmary>
		private System.Type ResourceType {
			get {
				object[] attributes = fieldInfo.GetCustomAttributes(typeof(ResourceReferenceTypeAttribute), true);
				if (attributes != null && attributes.Length > 0) {
					return ((ResourceReferenceTypeAttribute)attributes[0]).Type;
				}
				return typeof(Object);
			}
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summmary>
		/// <see cref="PropertyDrawer.OnGUI" />
		/// </summmary>
		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			
			EditorGUI.BeginProperty(rect, label, property);
			EditorGUI.BeginChangeCheck();
			
			ResourceReference reference;
			reference = property.objectReferenceValue as ResourceReference;
			
			Object resource;
			resource = AssetDatabase.LoadAssetAtPath(reference != null ? AssetDatabase.GUIDToAssetPath(reference.Guid) : null, ResourceType);
			resource = EditorGUI.ObjectField(rect, label, resource, ResourceType, false);
			
			if (EditorGUI.EndChangeCheck()) {
				if (resource != null) {
					reference = ResourceReferenceEditor.FindOrCreateReference(resource);
					property.objectReferenceValue = reference;
				} else if (reference != null) {
					reference = null;
					property.objectReferenceValue = null;
				}
			}
			
			EditorGUI.EndProperty();
			
		}
		
		#endregion
		
		
	}
	
}
