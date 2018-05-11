namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(PlatformSettingsMultiplexer))]
	public class PlatformSettingsMultiplexerEditor : Editor {
		
		
		#region Editor Callbacks
		
		[OnPlatformBootstrap(-1001)]
		public static void OnPlatformBootstrap() {
			BootstrapPlatformSettingsMultiplexer();
			BootstrapPlatformSettingsPrefabs();
		}
		
		private static void BootstrapPlatformSettingsMultiplexer() {
			
			PlatformSettingsMultiplexer mux;
			mux = PlatformSettingsMultiplexer.Instance;
			
			if (mux == null) {
				
				string path;
				path = "Assets/Platform/Resources/PlatformSettingsMultiplexer.prefab";
				
				// create instance
				PlatformSettingsMultiplexer instance;
				instance = new GameObject().AddComponent<PlatformSettingsMultiplexer>();
				
				// create prefab
				AssetDatabase.StartAssetEditing();
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				PrefabUtility.CreatePrefab(path, instance.gameObject);
				AssetDatabase.StopAssetEditing();
				
				// destroy instance
				DestroyImmediate(instance.gameObject, false);
				
			}
			
		}
		
		private static void BootstrapPlatformSettingsPrefabs() {
			
			PlatformSettingsMultiplexer mux;
			mux = PlatformSettingsMultiplexer.Instance;
			
			foreach (Platform platform in PlatformUtil.AllPlatforms) {
				
				PlatformSettingsPrefab oldPrefab;
				oldPrefab = mux.GetPrefab(platform);
				
				PlatformSettingsPrefab newPrefab;
				newPrefab = oldPrefab;
				
				if (newPrefab == null) {
					newPrefab = PlatformSettingsPrefabEditor.GetPrefabs(platform).FirstOrDefault();
				}
				
				if (newPrefab == null) {
					newPrefab = PlatformSettingsPrefabEditor.GetDefaultPrefab(platform);
				}
				
				if (newPrefab != oldPrefab) {
					mux.SetPrefab(platform, newPrefab);
					EditorUtility.SetDirty(mux);
				}
				
			}
			
		}
		
		#endregion
		
		
		#region Editor Methods
		
		override public void OnInspectorGUI() {
			
			PlatformSettingsMultiplexer mux;
			mux = this.target as PlatformSettingsMultiplexer;
			
			foreach (Platform platform in PlatformUtil.AllPlatforms) {
				
				string label;
				label = platform.ToString();
				
				System.Type type;
				type = typeof(PlatformSettingsPrefab);
				
				Object oldValue;
				oldValue = mux.GetPrefab(platform);
				
				Object newValue;
				newValue = EditorGUILayout.ObjectField(label, oldValue, type, false);
				
				if (oldValue != newValue) {
					mux.SetPrefab(platform, newValue as PlatformSettingsPrefab);
				}
				
			}
			
		}
		
		#endregion
		
		
	}
	
}