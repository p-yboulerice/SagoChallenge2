namespace SagoLocalizationEditor {
	
	using SagoLocalization;
	using UnityEditor;
	using UnityEngine;
	
	public class LocalizationWindow : EditorWindow {
		
		
		#region Static Methods
		
		[MenuItem("Window/Sago/Localization")]
		private static LocalizationWindow GetWindow() {
			return EditorWindow.GetWindow<LocalizationWindow>();
		}
		
		#endregion
		
		
		#region Methods
		
		private void OnEnable() {
			titleContent = new GUIContent("Localization");
		}
		
		private void OnGUI() {
			
			Adaptor adaptor;
			adaptor = ScriptableObject.CreateInstance<Adaptor>();
			adaptor.EditorPrefs = LocaleProvider.ReadFromEditorPrefs();
			adaptor.PlayerPrefs = LocaleProvider.ReadFromPlayerPrefs();
			
			SerializedObject adaptorObject;
			adaptorObject = new SerializedObject(adaptor);
			
			SerializedProperty editorPrefsProperty;
			editorPrefsProperty = adaptorObject.FindProperty("EditorPrefs");
			
			SerializedProperty playerPrefsProperty;
			playerPrefsProperty = adaptorObject.FindProperty("PlayerPrefs");
			
			EditorGUILayout.PropertyField(
				editorPrefsProperty,
				new GUIContent("Editor Prefs"), 
				true
			);
			
			EditorGUILayout.PropertyField(
				playerPrefsProperty,
				new GUIContent("Player Prefs"), 
				true
			);
			
			adaptorObject.ApplyModifiedProperties();
			
			LocaleProvider.WriteToEditorPrefs(adaptor.EditorPrefs);
			LocaleProvider.WriteToPlayerPrefs(adaptor.PlayerPrefs);
			
		}
		
		#endregion
		
		
	}
	
	class Adaptor : ScriptableObject {
		
		[SerializeField]
		public LocaleProvider.Json EditorPrefs;
		
		[SerializeField]
		public LocaleProvider.Json PlayerPrefs;
		
	}
	
}
