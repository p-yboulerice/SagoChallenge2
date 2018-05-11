namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(Physics2DSettings))]
	public class Physics2DSettingsEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/Physics2DSettings/Import Physics2DSettings From Project...")]
		private static void ImportPhysics2DSettingsFromProjectContextMenuItem(MenuCommand command) {
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Choose A Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				Physics2DSettings physics2DSettings;
				physics2DSettings = command.context as Physics2DSettings;
				ImportPhysics2DSettingsFromProject(physics2DSettings, projectPath);
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Imports the Physics2DSettings from another project.
		/// </summary>
		/// <param name="physics2DSettings">
		/// The <see cref="Physics2DSettings" /> object.
		/// </param>
		/// <param name="projectPath">
		/// The path to the project.
		/// </param>
		public static void ImportPhysics2DSettingsFromProject(Physics2DSettings physics2DSettings, string projectPath) {
			
			if (string.IsNullOrEmpty(projectPath)) {
				throw new System.ArgumentNullException("projectPath");
			}
			
			string assetPath;
			assetPath = Path.Combine(projectPath, "ProjectSettings/Physics2DSettings.asset");
			
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
			
			SerializedObject contentObject = new SerializedObject(physics2DSettings);
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
		/// Creates a new <see cref="Physics2DSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static Physics2DSettings CreatePhysics2DSettings(System.Type type) {
			string assetPath = GetPhysics2DSettingsPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Physics2DSettings>(), assetPath);
			return FindPhysics2DSettings(type);
		}
		
		/// <summary>
		/// Finds an existing <see cref="Physics2DSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static Physics2DSettings FindPhysics2DSettings(System.Type type) {
			string assetPath = GetPhysics2DSettingsPath(type);
			return AssetDatabase.LoadAssetAtPath(assetPath,typeof(Physics2DSettings)) as Physics2DSettings;
		}
		
		/// <summary>
		/// Finds an existing <see cref="Physics2DSettings" /> asset or creates a new <see cref="Physics2DSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static Physics2DSettings FindOrCreatePhysics2DSettings(System.Type type) {
			return FindPhysics2DSettings(type) ?? CreatePhysics2DSettings(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="Physics2DSettings" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetPhysics2DSettingsPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Settings", "Physics2DSettings.asset"));
		}
		
		/// <summary>
		/// Updates the <see cref="Physics2DSettings" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static Physics2DSettings UpdatePhysics2DSettings(System.Type type) {
			
			Physics2DSettings physics2DSettings;
			physics2DSettings = FindOrCreatePhysics2DSettings(type);
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(physics2DSettings);
			serializedObject.FindProperty("m_ContentInfo").objectReferenceValue = ContentInfoEditor.FindContentInfo(type);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			
			return physics2DSettings;
			
		}
		
		#endregion
		
		
	}
	
}