namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(TagManager))]
	public class TagManagerEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/TagManager/Import TagManager From Project...")]
		private static void ImportTagManagerFromProjectContextMenuItem(MenuCommand command) {
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Choose A Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				TagManager tagManager;
				tagManager = command.context as TagManager;
				ImportTagManagerFromProject(tagManager, projectPath);
			}
			
		}
		
		[MenuItem("CONTEXT/TagManager/Update Project Settings")]
		private static void UpdateProjectSettingsMenuItem() {
			UpdateProjectSettings();
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Imports the TagManager from another project.
		/// </summary>
		/// <param name="tagManager">
		/// The <see cref="TagManager" /> object.
		/// </param>
		/// <param name="projectPath">
		/// The path to the project.
		/// </param>
		public static void ImportTagManagerFromProject(TagManager tagManager, string projectPath) {
			
			if (tagManager == null) {
				throw new System.ArgumentNullException("tagManager");
			}
			
			if (string.IsNullOrEmpty(projectPath)) {
				throw new System.ArgumentNullException("projectPath");
			}
			
			string assetPath;
			assetPath = Path.Combine(projectPath, "ProjectSettings/TagManager.asset");
			
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
			
			SerializedObject contentObject = new SerializedObject(tagManager);
			do {
				SerializedProperty contentProperty = contentObject.FindProperty(tempProperty.name);
				if (contentProperty != null) {
					tempProperty.CopyTo(contentProperty);
				}
			} while (tempProperty.Next(false));
			contentObject.ApplyModifiedPropertiesWithoutUndo();
			
			File.Delete(tempPath);
			
		}
		
		/// <summary>
		/// Updates the project settings.
		/// </summary>
		public static void UpdateProjectSettings() {
			
			TagManager[] tagManagers;
			tagManagers = (
				AssetDatabase
				.FindAssets("t:SagoApp.Content.TagManager")
				.Select(guid => AssetDatabase.LoadAssetAtPath<TagManager>(AssetDatabase.GUIDToAssetPath(guid)))
				.ToArray()
			);
			
			SerializedObject obj;
			obj = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			
			// layers
			{
				string[] layers;
				layers = (
					tagManagers.Length == 1 ? 
					tagManagers[0].Layers : 
					null
				);
				
				SerializedProperty prop;
				prop = obj.FindProperty("layers");
				prop.arraySize = 32;
				
				for (int index = 8; index < prop.arraySize; index++) {
					if (layers != null) {
						prop.GetArrayElementAtIndex(index).stringValue = (index < layers.Length ? layers[index] : null);
					} else {
						prop.GetArrayElementAtIndex(index).stringValue = string.Format("Sago{0}", index);
					}
				}
			}
			
			// tags
			{
				string[] tags;
				tags = (
					tagManagers
					.SelectMany(tagManager => tagManager.Tags)
					.Where(tag => !string.IsNullOrEmpty(tag))
					.Distinct()
					.ToArray()
				);
				
				SerializedProperty prop;
				prop = obj.FindProperty("tags");
				prop.arraySize = tags.Length;
				
				for (int index = 0; index < prop.arraySize; index++) {
					prop.GetArrayElementAtIndex(index).stringValue = tags[index];
				}
			}
			
			obj.ApplyModifiedPropertiesWithoutUndo();
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="TagManagerEditor" /> instance.
		/// </summary>
		/// <param name="tagManager">
		/// The <see cref="TagManager" /> object to edit.
		/// </param>
		public static TagManagerEditor CreateEditor(TagManager tagManager) {
			return Editor.CreateEditor(tagManager, typeof(TagManagerEditor)) as TagManagerEditor;
		}
		
		/// <summary>
		/// Creates a new <see cref="TagManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TagManager CreateTagManager(System.Type type) {
			string assetPath = GetTagManagerPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TagManager>(), assetPath);
			return FindTagManager(type);
		}
		
		/// <summary>
		/// Finds an existing <see cref="TagManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TagManager FindTagManager(System.Type type) {
			string assetPath = GetTagManagerPath(type);
			return AssetDatabase.LoadAssetAtPath(assetPath,typeof(TagManager)) as TagManager;
		}
		
		/// <summary>
		/// Finds an existing <see cref="TagManager" /> asset or creates a new <see cref="TagManager" /> asset at the specified path.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TagManager FindOrCreateTagManager(System.Type type) {
			return FindTagManager(type) ?? CreateTagManager(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="TagManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetTagManagerPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Settings", "TagManager.asset"));
		}
		
		/// <summary>
		/// Updates the <see cref="TagManager" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static TagManager UpdateTagManager(System.Type type) {
			
			TagManager tagManager;
			tagManager = FindOrCreateTagManager(type);
			
			SerializedObject settingsObject;
			settingsObject = new SerializedObject(tagManager);
			settingsObject.FindProperty("m_ContentInfo").objectReferenceValue = ContentInfoEditor.FindContentInfo(type);
			settingsObject.ApplyModifiedPropertiesWithoutUndo();
			
			return tagManager;
			
		}
		
		#endregion
		
		
	}
	
}