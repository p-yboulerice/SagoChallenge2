namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(GraphicsSettings))]
	public class GraphicsSettingsEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/GraphicsSettings/Import GraphicsSettings From Project...")]
		private static void ImportGraphicsSettingsFromProjectContextMenuItem(MenuCommand command) {
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Choose A Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				GraphicsSettings graphicsSettings;
				graphicsSettings = command.context as GraphicsSettings;
				ImportGraphicsSettingsFromProject(graphicsSettings, projectPath);
			}
			
		}
		
		[MenuItem("CONTEXT/GraphicsSettings/Update Project Settings")]
		private static void UpdateProjectSettingsMenuItem() {
			UpdateProjectSettings();
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Imports the GraphicsSettings from another project.
		/// </summary>
		/// <param name="graphicsSettings">
		/// The <see cref="GraphicsSettings" /> object.
		/// </param>
		/// <param name="projectPath">
		/// The path to the other project.
		/// </param>
		public static void ImportGraphicsSettingsFromProject(GraphicsSettings graphicsSettings, string projectPath) {
			
			if (graphicsSettings == null) {
				throw new System.ArgumentNullException("graphicsSettings");
			}
			
			if (string.IsNullOrEmpty(projectPath)) {
				throw new System.ArgumentNullException("projectPath");
			}
			
			string assetPath;
			assetPath = Path.Combine(projectPath, "ProjectSettings/GraphicsSettings.asset");
			
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
			
			SerializedObject contentObject = new SerializedObject(graphicsSettings);
			while (tempProperty.Next(false)) {
				SerializedProperty contentProperty = contentObject.FindProperty(tempProperty.name);
				if (contentProperty != null) {
					tempProperty.CopyTo(contentProperty);
				}
			}
			contentObject.ApplyModifiedPropertiesWithoutUndo();
			
			File.Delete(tempPath);
			
		}
		
		/// <summary>
		/// Updates the project settings.
		/// </summary>
		public static void UpdateProjectSettings() {
			
			SerializedObject obj;
			obj = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
			
			SerializedProperty prop;
			prop = obj.FindProperty("m_AlwaysIncludedShaders");
			
			Shader[] defaultShaders;
			defaultShaders = new Shader[6] {
				Shader.Find("Legacy Shaders/Diffuse"),
				Shader.Find("Hidden/CubeBlur"),
				Shader.Find("Hidden/CubeCopy"),
				Shader.Find("Hidden/CubeBlend"),
				Shader.Find("UI/Default"),
				Shader.Find("UI/Default Font")
			};
			
			Shader[] customShaders;
			customShaders = (
				AssetDatabase
				.FindAssets("t:SagoApp.Content.GraphicsSettings")
				.Select(guid => AssetDatabase.LoadAssetAtPath<GraphicsSettings>(AssetDatabase.GUIDToAssetPath(guid)))
				.SelectMany(graphicsSettings => graphicsSettings.AlwaysIncludedShaders)
				.Where(shader => shader != null)
				.Distinct()
				.ToArray()
			);
			
			Shader[] alwaysIncludedShaders;
			alwaysIncludedShaders = defaultShaders.Concat(customShaders).Distinct().ToArray();
			
			prop.arraySize = alwaysIncludedShaders.Length;
			for (int index = 0; index < prop.arraySize; index++) {
				prop.GetArrayElementAtIndex(index).objectReferenceValue = alwaysIncludedShaders[index];
			}
			
			obj.ApplyModifiedPropertiesWithoutUndo();
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="GraphicsSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static GraphicsSettings CreateGraphicsSettings(System.Type type) {
			string assetPath = GetGraphicsSettingsPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GraphicsSettings>(), assetPath);
			return FindGraphicsSettings(type);
		}
		
		/// <summary>
		/// Finds an existing <see cref="GraphicsSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static GraphicsSettings FindGraphicsSettings(System.Type type) {
			string assetPath = GetGraphicsSettingsPath(type);
			return AssetDatabase.LoadAssetAtPath(assetPath,typeof(GraphicsSettings)) as GraphicsSettings;
		}
		
		/// <summary>
		/// Finds an existing <see cref="GraphicsSettings" /> asset or creates a new <see cref="GraphicsSettings" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static GraphicsSettings FindOrCreateGraphicsSettings(System.Type type) {
			return FindGraphicsSettings(type) ?? CreateGraphicsSettings(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="GraphicsSettings" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetGraphicsSettingsPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Settings", "GraphicsSettings.asset"));
		}
		
		/// <summary>
		/// Updates the <see cref="GraphicsSettings" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static GraphicsSettings UpdateGraphicsSettings(System.Type type) {
			
			GraphicsSettings graphicsSettings;
			graphicsSettings = FindOrCreateGraphicsSettings(type);
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(graphicsSettings);
			serializedObject.FindProperty("m_ContentInfo").objectReferenceValue = ContentInfoEditor.FindContentInfo(type);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			
			return graphicsSettings;
			
		}
		
		#endregion
		
		
	}
	
}