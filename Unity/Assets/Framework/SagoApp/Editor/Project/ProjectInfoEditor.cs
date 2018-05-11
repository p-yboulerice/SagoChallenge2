namespace SagoAppEditor.Project {
	
	using SagoApp.Content;
	using SagoAppEditor.Content;
	using SagoApp.Project;
	using SagoCore.Submodules;
	using SagoCoreEditor.Submodules;
	using SagoCoreEditor.Resources;
	using SagoCoreEditor.Scenes;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(ProjectInfo))]
	public class ProjectInfoEditor : Editor {
		
		
		#region Context Menu
		
		[MenuItem("CONTEXT/ProjectInfo/Update")]
		private static void UpdateProjectInfoContextMenuItem(MenuCommand command) {
			UpdateProjectInfo();
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static ProjectInfo CreateProjectInfo() {
			
			string path;
			path = GetProjectInfoPath();
			
			string folder;
			folder = Path.GetDirectoryName(path);
			
			AssetDatabase.StartAssetEditing();
			AssetDatabase.CreateFolder(Path.GetDirectoryName(folder), Path.GetFileNameWithoutExtension(folder));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ProjectInfo>(), path);
			AssetDatabase.StopAssetEditing();
			
			return FindProjectInfo();
			
		}
		
		public static ProjectInfo FindProjectInfo() {
			string assetPath = GetProjectInfoPath();
			return AssetDatabase.LoadAssetAtPath(assetPath, typeof(ProjectInfo)) as ProjectInfo;
		}
		
		public static ProjectInfo FindOrCreateProjectInfo() {
			return FindProjectInfo() ?? CreateProjectInfo();
		}
		
		public static ProjectInfo UpdateProjectInfo() {
			
			ProjectInfo projectInfo;
			projectInfo = FindOrCreateProjectInfo();
			
			ContentInfo[] contentInfo;
			contentInfo = (
				ContentInfoEditor
				.ContentInfoTypes
				.Select(type => ContentInfoEditor.UpdateContentInfo(type))
				.ToArray()
			);
			
			SerializedObject obj;
			obj = new SerializedObject(projectInfo);
				
			SerializedProperty prop;
			prop = obj.FindProperty("m_ContentInfo");
			prop.arraySize = contentInfo.Length;
			
			for (int index = 0; index < contentInfo.Length; index++) {
				prop.GetArrayElementAtIndex(index).objectReferenceValue = contentInfo[index];
			}
			
			obj.ApplyModifiedProperties();
			
			return FindProjectInfo();
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static string GetProjectInfoPath() {
			return "Assets/Project/Resources/ProjectInfo.asset";
			
		}
		
		#endregion
		
		
		#region Methods
		
		override public void OnInspectorGUI() {
			
			GUIStyle headerStyle;
			headerStyle = new GUIStyle(EditorStyles.label);
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.normal.textColor = Color.white;
			
			GUIStyle sectionStyle;
			sectionStyle = new GUIStyle(GUIStyle.none);
			sectionStyle.margin = new RectOffset(10,10,10,10);
			
			GUIStyle groupStyle;
			groupStyle = new GUIStyle(GUIStyle.none);
			groupStyle.margin = new RectOffset(10,10,10,10);
			
			
			serializedObject.Update();
			
			
			EditorGUILayout.BeginVertical(sectionStyle);
				GUILayout.Label("Project", headerStyle);
				EditorGUILayout.BeginVertical(groupStyle);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MainScene"));
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			
			EditorGUILayout.BeginVertical(sectionStyle);
				GUILayout.Label("Content", headerStyle);
				EditorGUILayout.BeginVertical(groupStyle);
				
				SerializedProperty contentProperty;
				contentProperty = serializedObject.FindProperty("m_ContentInfo");
				for (int index = 0; index < contentProperty.arraySize; index++) {
					SerializedProperty elementProperty;
					elementProperty = contentProperty.GetArrayElementAtIndex(index);
					ContentInfo contentInfo;
					contentInfo = elementProperty.objectReferenceValue as ContentInfo;
					EditorGUILayout.ObjectField(contentInfo.SubmoduleName, contentInfo, typeof(ContentInfo), false);
				}
				
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			
			EditorGUILayout.BeginVertical(sectionStyle);
				GUILayout.Label("Navigation", headerStyle);
				EditorGUILayout.BeginVertical(groupStyle);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToContentScene"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToContentTransition"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToContentLoadingSpinner"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToProjectScene"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToProjectTransition"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToErrorScene"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NavigateToErrorTransition"));
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();
			
		}
		
		#endregion
		
		
	}
	
}