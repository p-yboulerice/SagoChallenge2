namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public class PlatformSettingsPrefabEditor : MonoBehaviour {
		
		
		#region Path Methods
		
		public static string GetBaseAssetPath() {
			return "Assets/Platform/Prefabs";
		}
		
		public static string GetDefaultPrefabAssetPath(Platform platform) {
			string directory = GetBaseAssetPath();
			string filename = Path.ChangeExtension(platform.ToString(), "prefab");
			return Path.Combine(directory, filename);
		}
		
		#endregion
		
		
		#region Prefab Methods
		
		public static PlatformSettingsPrefab GetDefaultPrefab(Platform platform) {
			
			string path;
			path = GetDefaultPrefabAssetPath(platform);
			
			PlatformSettingsPrefab prefab;
			prefab = AssetDatabase.LoadAssetAtPath(path, typeof(PlatformSettingsPrefab)) as PlatformSettingsPrefab;
			
			if (prefab == null) {
				
				// create instance
				PlatformSettingsPrefab instance;
				instance = new GameObject().AddComponent<PlatformSettingsPrefab>();
				instance.Platform = platform;
				
				// create prefab
				AssetDatabase.StartAssetEditing();
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				PrefabUtility.CreatePrefab(path, instance.gameObject);
				AssetDatabase.StopAssetEditing();
				
				// destroy instance
				DestroyImmediate(instance.gameObject, false);
				
				// load prefab
				prefab = AssetDatabase.LoadAssetAtPath(path, typeof(PlatformSettingsPrefab)) as PlatformSettingsPrefab;
				
			}
			
			return prefab;
			
		}
		
		public static PlatformSettingsPrefab[] GetPrefabs() {
			return (
				AssetDatabase
				.GetAllAssetPaths()
				.Where(path => path.StartsWith(GetBaseAssetPath()) && path.EndsWith(".prefab"))
				.Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(PlatformSettingsPrefab)))
				.Cast<PlatformSettingsPrefab>()
				.Where(prefab => prefab != null)
				.ToArray()
			);
		}
		
		public static PlatformSettingsPrefab[] GetPrefabs(Platform platform) {
			return (
				GetPrefabs()
				.Where(prefab => platform == prefab.Platform)
				.ToArray()
			);
		}
		
		public static PlatformSettingsPrefab[] GetPrefabs(Platform[] platforms) {
			return (
				GetPrefabs()
				.Where(prefab => platforms.Contains(prefab.Platform))
				.ToArray()
			);
		}
		
		#endregion
		
		
		#region Prefab Component Methods
		
		public static T[] AddPrefabComponents<T>() where T : Component {
			return AddPrefabComponents<T>(GetPrefabs());
		}
		
		public static T[] AddPrefabComponents<T>(Platform platform) where T : Component {
			return AddPrefabComponents<T>(GetPrefabs(platform));
		}
		
		public static T[] AddPrefabComponents<T>(Platform[] platforms) where T : Component {
			return AddPrefabComponents<T>(GetPrefabs(platforms));
		}
		
		public static T[] AddPrefabComponents<T>(PlatformSettingsPrefab[] prefabs) where T : Component {
			List<T> components = new List<T>();
			foreach (PlatformSettingsPrefab prefab in prefabs) {
				if (!prefab.GetComponent<T>()) {
					components.Add(prefab.gameObject.AddComponent<T>());
				}
			}
			return components.ToArray();
		}
		
		public static T[] GetPrefabComponents<T>() where T : Component {
			return (
				GetPrefabs()
				.Select(p => p.GetComponent<T>())
				.ToArray()
			);
		}
		
		public static T[] GetPrefabComponents<T>(Platform platform) where T : Component {
			return (
				GetPrefabs(platform)
				.Select(p => p.GetComponent<T>())
				.ToArray()
			);
		}
		
		public static T[] GetPrefabComponents<T>(Platform[] platforms) where T : Component {
			return (
				GetPrefabs(platforms)
				.Select(p => p.GetComponent<T>())
				.ToArray()
			);
		}
		
		#endregion
		
		
	}
	
}