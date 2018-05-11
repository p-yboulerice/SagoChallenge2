namespace SagoAppEditor.Content {
	
	using UnityEditor;
	using UnityEngine;
	
	public static class SerializedPropertyExtensions {
		
		
		#region Extension Methods
		
		public static void CopyFrom(this SerializedProperty dst, SerializedProperty src) {
			
			if (src.propertyType != dst.propertyType) {
				throw new System.InvalidOperationException(string.Format(
					"Property types do not match: src.propertyType = {0}, dst.propertyType = {1}", 
					src.propertyType, 
					dst.propertyType
				));
			}
			
			switch (src.propertyType) {
				case SerializedPropertyType.Generic:
					if (src.isArray && dst.isArray) {
						dst.arraySize = src.arraySize;
						for (int index = 0; index < src.arraySize; index++) {
							src.GetArrayElementAtIndex(index).CopyTo(dst.GetArrayElementAtIndex(index));
						}
					}
					break;
				case SerializedPropertyType.Boolean:
					dst.boolValue = src.boolValue;
					break;
				case SerializedPropertyType.Float:
					dst.floatValue = src.floatValue;
					break;
				case SerializedPropertyType.Integer:
					dst.intValue = src.intValue;
					break;
				case SerializedPropertyType.ObjectReference:
					dst.objectReferenceValue = src.objectReferenceValue;
					break;
				case SerializedPropertyType.String:
					dst.stringValue = src.stringValue;
					break;
				case SerializedPropertyType.Vector2:
					dst.vector2Value = src.vector2Value;
					break;
				case SerializedPropertyType.Vector3:
					dst.vector3Value = src.vector3Value;
					break;
				default:
					throw new System.NotImplementedException(string.Format("propertyType = {0}", src.propertyType));
			}
			
		}
		
		public static void CopyTo(this SerializedProperty src, SerializedProperty dst) {
			CopyFrom(dst, src);
		}
		
		#endregion
		
		
	}
	
}