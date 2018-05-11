namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(DynamicsManager))]
	public class DynamicsManagerEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/DynamicsManager/Import Dynamics Manager From Project...")]
		private static void ImportDynamicsManagerFromProjectContextMenuItem(MenuCommand command) {
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Choose A Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				DynamicsManager dynamicsManager;
				dynamicsManager = command.context as DynamicsManager;
				ImportDynamicsManagerFromProject(dynamicsManager, projectPath);
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Imports the DynamicsManager from another project.
		/// </summary>
		/// <param name="dynamicsManager">
		/// The <see cref="DynamicsManager" /> object.
		/// </param>
		/// <param name="projectPath">
		/// The path to the project.
		/// </param>
		public static void ImportDynamicsManagerFromProject(DynamicsManager dynamicsManager, string projectPath) {
			
			if (dynamicsManager == null) {
				throw new System.ArgumentNullException("dynamicsManager");
			}
			
			if (string.IsNullOrEmpty(projectPath)) {
				throw new System.ArgumentNullException("projectPath");
			}
			
			string assetPath;
			assetPath = Path.Combine(projectPath, "ProjectSettings/DynamicsManager.asset");
			
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
			
			SerializedProperty tempProperty;
			tempProperty = tempObject.GetIterator();
			tempProperty.Next(true);
			
			SerializedObject contentObject = new SerializedObject(dynamicsManager);
			while (tempProperty.Next(false)) {
				SerializedProperty contentProperty = contentObject.FindProperty(tempProperty.name);
				if (contentProperty != null) {
					tempProperty.CopyTo(contentProperty);
				}
			}
			contentObject.ApplyModifiedPropertiesWithoutUndo();
			
			File.Delete(tempPath);
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="DynamicsManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static DynamicsManager CreateDynamicsManager(System.Type type) {
			string assetPath = GetDynamicsManagerPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DynamicsManager>(), assetPath);
			return FindDynamicsManager(type);
		}
		
		/// <summary>
		/// Finds an existing <see cref="DynamicsManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static DynamicsManager FindDynamicsManager(System.Type type) {
			string assetPath = GetDynamicsManagerPath(type);
			return AssetDatabase.LoadAssetAtPath(assetPath,typeof(DynamicsManager)) as DynamicsManager;
		}
		
		/// <summary>
		/// Finds an existing <see cref="DynamicsManager" /> asset or creates a new <see cref="DynamicsManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static DynamicsManager FindOrCreateDynamicsManager(System.Type type) {
			return FindDynamicsManager(type) ?? CreateDynamicsManager(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="DynamicsManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetDynamicsManagerPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Settings", "DynamicsManager.asset"));
		}
		
		/// <summary>
		/// Updates the <see cref="DynamicsManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static DynamicsManager UpdateDynamicsManager(System.Type type) {
			
			DynamicsManager dynamicsManager;
			dynamicsManager = FindOrCreateDynamicsManager(type);
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(dynamicsManager);
			serializedObject.FindProperty("m_ContentInfo").objectReferenceValue = ContentInfoEditor.FindContentInfo(type);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			
			return dynamicsManager;
			
		}
		
		#endregion
		
		
	}
	
}