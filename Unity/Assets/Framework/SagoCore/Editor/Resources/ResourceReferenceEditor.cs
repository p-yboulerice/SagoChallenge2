namespace SagoCoreEditor.Resources {
	
	using SagoCore.Resources;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The ResourceReferenceEditor class provides a custom inspector for <see cref="ResourceReference" /> assets.
	/// </summary>
	[CustomEditor(typeof(ResourceReference), true)]
	public class ResourceReferenceEditor : Editor {
		
		
		#region Editor Methods
		
		/// <summary>
		/// <see cref="Editor.OnInspectorGUI" />
		/// </summary>
		override public void OnInspectorGUI() {
			
			ResourceReference reference;
			reference = target as ResourceReference;
				
			System.Type resourceType;
			resourceType = typeof(Object);
			
			Object resource;
			resource = AssetDatabase.LoadAssetAtPath(reference.AssetPath, resourceType);
			
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField("Resource", resource, resourceType, false);
			EditorGUILayout.TextField("Asset Bundle", reference.AssetBundleName);
			EditorGUILayout.TextField("Asset Path", reference.AssetPath);
			EditorGUILayout.TextField("Resource Path", reference.ResourcePath);
			EditorGUI.EndDisabledGroup();
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new resource reference for the selected assets.
		/// </summary>
		[MenuItem("Assets/Create/Sago/Core/Resource Reference")]
		private static void CreateReferenceMenuItem() {
			foreach (var asset in Selection.objects.Where(obj => AssetDatabase.Contains(obj))) {
				CreateReference(asset);
			}
		}
		
		/// <summary>
		/// Creates a new resource reference for the specified asset.
		/// </summary>
		/// <param name="asset">
		/// The asset.
		/// </param>
		public static ResourceReference CreateReference(Object asset) {
			return CreateReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)));
		}
		
		/// <summary>
		/// Creates a new resource reference for the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		public static ResourceReference CreateReference(string assetGuid) {
			
			ResourceReference reference;
			reference = ScriptableObject.CreateInstance<ResourceReference>();
			reference.Guid = assetGuid;
			
			AssetDatabase.CreateAsset(reference, GetReferencePath(assetGuid));
			AssetDatabase.SaveAssets();
			
			return FindReference(assetGuid);
			
		}
		
		/// <summary>
		/// Finds an existing resource reference for the specified asset.
		/// </summary>
		/// <param name="asset">
		/// The asset.
		/// </param>
		public static ResourceReference FindReference(Object asset) {
			return FindReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)));
		}
		
		/// <summary>
		/// Finds an existing resource reference for the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		public static ResourceReference FindReference(string assetGuid) {
			
			string path = null;
			ResourceReference reference = null;
			
			if (reference == null) {
				path = AssetDatabase.GUIDToAssetPath(assetGuid);
				path = Path.ChangeExtension(path, string.Format("{0}.{1}", assetGuid, Path.GetExtension(path)));
				reference = AssetDatabase.LoadAssetAtPath(path, typeof(ResourceReference)) as ResourceReference;
				reference = (reference != null && reference.Guid == assetGuid) ? reference : null;
			}
			
			if (reference == null) {
				path = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.Contains(assetGuid));
				reference = AssetDatabase.LoadAssetAtPath(path, typeof(ResourceReference)) as ResourceReference;
				reference = (reference != null && reference.Guid == assetGuid) ? reference : null;
			}
			
			return reference;
			
		}
		
		/// <summary>
		/// Finds an existing resource reference or creates a new resource reference for the specified asset.
		/// </summary>
		/// <param name="asset">
		/// The asset.
		/// </param>
		public static ResourceReference FindOrCreateReference(Object asset) {
			return FindOrCreateReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)));
		}
		
		/// <summary>
		/// Finds an existing resource reference or creates a new resource reference for the specified asset.
		/// </summary>
		/// <param name="asset">
		/// The guid of the asset.
		/// </param>
		public static ResourceReference FindOrCreateReference(string assetGuid) {
			return FindReference(assetGuid) ?? CreateReference(assetGuid);
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Finds unused resource reference assets.
		/// </summary>
		private static ResourceReference[] FindUnusedResourceReferences() {
			
			List<Object> assets = AssetDatabase
				.GetAllAssetPaths()
				.Where(p => p.StartsWith("Assets"))
				.Where(p => File.Exists(p))
				.Select(p => AssetDatabase.LoadAssetAtPath(p, typeof(Object)))
				.ToList();
				
			List<ResourceReference> used = new List<ResourceReference>();
			foreach (Object asset in assets) {
				used.AddRange(
					EditorUtility
					.CollectDependencies(new Object[]{ asset })
					.OfType<ResourceReference>()
					.Where(dependency => AssetDatabase.GetAssetPath(dependency) != AssetDatabase.GetAssetPath(asset))
				);
			}
			
			return (
				FindResourceReferences()
				.Except(used)
				.Where(reference => !AssetDatabase.GetAssetPath(reference).Contains("Content"))
				.Where(reference => !AssetDatabase.GetAssetPath(reference).Contains("External"))
				.Where(reference => !AssetDatabase.GetAssetPath(reference).Contains("Framework"))
				.Distinct()
				.ToArray()
			);
			
		}
		
		/// <summary>
		/// Removes unused resource reference assets.
		/// </summary>
		public static void RemoveUnusedResourceReferences() {
			AssetDatabase.StartAssetEditing();
			foreach (ResourceReference reference in FindUnusedResourceReferences()) {
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(reference));
			}
			AssetDatabase.StopAssetEditing();
		}
		
		/// <summary>
		/// Finds all resource reference assets.
		/// </summary>
		private static ResourceReference[] FindResourceReferences() {
			return (
				AssetDatabase
				.FindAssets("t:ResourceReference")
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(ResourceReference)) as ResourceReference)
				.ToArray()
			);
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Gets the asset path of the resource reference for the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		public static string GetReferencePath(string assetGuid) {
			#if UNITY_EDITOR
				string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
				string assetDirectory = Path.GetDirectoryName(assetPath);
				string assetName = Path.GetFileNameWithoutExtension(assetPath);
				return Path.Combine(assetDirectory, string.Format("{0}.{1}.asset", assetName, assetGuid));
			#else
				return null;
			#endif
		}
		
		/// <summary>
		/// Maps an asset guid to corresponding resource reference guid.
		/// </summary>
		/// <param name="assetGuid">
		/// The asset guid.
		/// </param>
		public static string AssetGuidToReferenceGuid(string assetGuid) {
			return (
				AssetDatabase
				.FindAssets("t:ResourceReference")
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.Where(path => path.Contains(assetGuid))
				.Select(path => AssetDatabase.AssetPathToGUID(path))
				.FirstOrDefault()
			);
		}
		
		/// <summary>
		/// Maps a resource reference guid to the corresponding asset guid.
		/// </summary>
		/// <param name="referenceGuid">
		/// The resource reference guid.
		/// </param>
		public static string ReferenceGuidToAssetGuid(string referenceGuid) {
			return Path.GetExtension(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(referenceGuid))).Remove(0,1);
		}
		
		#endregion
		
		
	}
	
}