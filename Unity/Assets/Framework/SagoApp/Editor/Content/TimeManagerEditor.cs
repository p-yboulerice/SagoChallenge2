namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(TimeManager))]
	public class TimeManagerEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/TimeManager/Import TimeManager From Project...")]
		private static void ImportTimeManagerFromProjectContextMenuItem(MenuCommand command) {
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Choose A Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				TimeManager timeManager;
				timeManager = command.context as TimeManager;
				ImportTimeManagerFromProject(timeManager, projectPath);
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Imports the TimeManager from the specified project.
		/// </summary>
		/// <param name="projectPath">
		/// The <see cref="TimeManager" /> object.
		/// </param>
		/// <param name="projectPath">
		/// The path to the project.
		/// </param>
		public static void ImportTimeManagerFromProject(TimeManager timeManager, string projectPath) {
			
			if (string.IsNullOrEmpty(projectPath)) {
				throw new System.ArgumentNullException("projectPath");
			}
			
			string assetPath;
			assetPath = Path.Combine(projectPath, "ProjectSettings/TimeManager.asset");
			
			if (!Directory.Exists(projectPath) || string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath)) {
				throw new System.ArgumentException(string.Format("Invalid project path: {0}", projectPath), "projectPath");
			}
			
			string tempPath;
			tempPath = FileUtil.GetUniqueTempPathInProject();
			
			if (string.IsNullOrEmpty(tempPath)) {
				throw new System.InvalidOperationException(string.Format("Could not get temporary path: {0}",tempPath));
			}
			
			FileUtil.ReplaceFile(assetPath, tempPath);
			
			if (!File.Exists(tempPath)) {
				throw new System.InvalidOperationException(string.Format("Could not copy asset to temporary path: {0}", tempPath));
			}
			
			SerializedObject tempObject;
			tempObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(tempPath)[0]);
			
			if (tempObject == null) {
				throw new System.InvalidOperationException(string.Format("Could not load temporary asset: {0}", tempPath));
			}
			
			SerializedObject contentObject;
			contentObject = new SerializedObject(timeManager);
			contentObject.FindProperty("m_FixedTimestep").floatValue = tempObject.FindProperty("Fixed Timestep").floatValue;
			contentObject.FindProperty("m_MaximumAllowedTimestep").floatValue = tempObject.FindProperty("Maximum Allowed Timestep").floatValue;
			contentObject.FindProperty("m_TimeScale").floatValue = tempObject.FindProperty("m_TimeScale").floatValue;
			contentObject.ApplyModifiedPropertiesWithoutUndo();
			
			File.Delete(tempPath);
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="TimeManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TimeManager CreateTimeManager(System.Type type) {
			string assetPath = GetTimeManagerPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TimeManager>(), assetPath);
			return FindTimeManager(type);
		}
		
		/// <summary>
		/// Finds an existing <see cref="TimeManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TimeManager FindTimeManager(System.Type type) {
			string assetPath = GetTimeManagerPath(type);
			return AssetDatabase.LoadAssetAtPath(assetPath,typeof(TimeManager)) as TimeManager;
		}
		
		/// <summary>
		/// Finds an existing <see cref="TimeManager" /> asset or creates a new <see cref="TimeManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TimeManager FindOrCreateTimeManager(System.Type type) {
			return FindTimeManager(type) ?? CreateTimeManager(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="TimeManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetTimeManagerPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Settings", "TimeManager.asset"));
		}
		
		/// <summary>
		/// Updates the <see cref="TimeManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TimeManager UpdateTimeManager(System.Type type) {
			
			TimeManager timeManager;
			timeManager = FindOrCreateTimeManager(type);
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(timeManager);
			serializedObject.FindProperty("m_ContentInfo").objectReferenceValue = ContentInfoEditor.FindContentInfo(type);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			
			return timeManager;
			
		}
		
		#endregion
		
		
	}
	
}