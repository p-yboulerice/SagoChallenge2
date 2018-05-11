namespace SagoLayoutEditor {
	
	using SagoLayout;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Scale))]
	public class ScaleEditor : LayoutComponentEditor {
		
		
		#region Fields
		
		SerializedProperty Factors;
		SerializedProperty Preset;
		
		#endregion
		
		
		#region Editor Methods
		
		override protected void OnEnable() {
			base.OnEnable();
			Factors = serializedObject.FindProperty("m_Factors");
			Preset = serializedObject.FindProperty("m_Preset");
		}
		
		override public void OnInspectorGUI() {
			
			EditorGUI.BeginChangeCheck(); {
				
				// update
				serializedObject.Update();
				
				// set the array to the same length as the enum and apply
				Factors.arraySize = System.Enum.GetNames(typeof(ScaleDevice)).Length;
				serializedObject.ApplyModifiedProperties();
				
				// draw preset
				EditorGUILayout.PropertyField(Preset);
				if (!Preset.hasMultipleDifferentValues) {
					if (ScalePreset.None == (ScalePreset)Preset.enumValueIndex) {
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Unknown).floatValue = 1;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Phone).floatValue = 1;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Tablet).floatValue = 1;
					} else if (ScalePreset.Phone == (ScalePreset)Preset.enumValueIndex) {
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Unknown).floatValue = 1;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Phone).floatValue = 1;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Tablet).floatValue = 0.8f;
					} else if (ScalePreset.Tablet == (ScalePreset)Preset.enumValueIndex) {
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Unknown).floatValue = 1;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Phone).floatValue = 1.2f;
						Factors.GetArrayElementAtIndex((int)ScaleDevice.Tablet).floatValue = 1f;
					}
				}
				serializedObject.ApplyModifiedProperties();
				
				// draw factors
				if (!Preset.hasMultipleDifferentValues) {
					EditorGUI.BeginDisabledGroup(ScalePreset.Custom != (ScalePreset)Preset.enumValueIndex);
					for (int index = 1; index < Factors.arraySize; index++) {
						SerializedProperty factor = Factors.GetArrayElementAtIndex(index);
						Rect rect = EditorGUILayout.GetControlRect(true);
						GUIContent label = new GUIContent(((ScaleDevice)index).ToString());
						EditorGUI.BeginProperty(rect, label, factor);
							factor.floatValue = EditorGUI.Slider(rect, label, factor.floatValue, 0, 2);
						EditorGUI.EndProperty();
					}
					EditorGUI.EndDisabledGroup();
				}
				serializedObject.ApplyModifiedProperties();
				
			}
			
			base.OnInspectorGUI();
			
			if (EditorGUI.EndChangeCheck()) {
				Apply();
			}
			
		}
		
		#endregion
		
	}
	
}