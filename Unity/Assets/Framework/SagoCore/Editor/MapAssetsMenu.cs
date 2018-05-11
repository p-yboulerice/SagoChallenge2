namespace SagoCoreEditor {

	using SagoCoreEditor.Resources;
	using SagoCoreEditor.Scenes;
	using SagoCoreEditor.Submodules;
	using SagoCore.Resources;
	using SagoCore.Scenes;
	using SagoCore.Submodules;
	using UnityEditor;
	using UnityEngine;
	
	public static class MapAssetsMenu {
		
		
		#region Init
		
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad() {
			if (!Application.isPlaying) {
				if (!ResourceMapEditor.FindResourceMap()) {
					ResourceMapEditor.UpdateResourceMap();
				}
				if (!SceneMapEditor.FindSceneMap()) {
					SceneMapEditor.UpdateSceneMap();
				}
				if (!SubmoduleMapEditor.FindSubmoduleMap()) {
					SubmoduleMapEditor.UpdateSubmoduleMap();
				}
			}
		}
		
		#endregion
		
		
		#region Editor Mode
		
		private const string EditorModeMenuItemKey = "Sago/Core/Map Assets/Editor Mode";
		
		[MenuItem(EditorModeMenuItemKey, false, 100)]
		private static void EditorModeMenuItem() {
			EditorMode();
			InitializeOnLoad();
		}
		
		[MenuItem(EditorModeMenuItemKey, true)]
		private static bool EditorModeMenuItemValidate() {
			bool valid = !Application.isPlaying && (
				ResourceMap.Mode != ResourceMapMode.Editor || 
				SceneMap.Mode != SceneMapMode.Editor ||
				SubmoduleMap.Mode != SubmoduleMapMode.Editor
			);
			UnityEditor.Menu.SetChecked(EditorModeMenuItemKey, !valid);
			return valid;
		}
		
		private static void EditorMode() {
			ResourceMap.Mode = ResourceMapMode.Editor;
			SceneMap.Mode = SceneMapMode.Editor;
			SubmoduleMap.Mode = SubmoduleMapMode.Editor;
		}
		
		#endregion
		
		
		#region Player Mode
		
		private const string PlayerModeMenuItemKey = "Sago/Core/Map Assets/Player Mode";
		
		[MenuItem(PlayerModeMenuItemKey, false, 100)]
		private static void PlayerModeMenuItem() {
			PlayerMode();
			InitializeOnLoad();
		}
		
		[MenuItem(PlayerModeMenuItemKey, true)]
		private static bool PlayerModeMenuItemValidate() {
			bool valid = !Application.isPlaying && (
				ResourceMap.Mode != ResourceMapMode.Player || 
				SceneMap.Mode != SceneMapMode.Player ||
				SubmoduleMap.Mode != SubmoduleMapMode.Player
			);
			UnityEditor.Menu.SetChecked(PlayerModeMenuItemKey, !valid);
			return valid;
		}
		
		private static void PlayerMode() {
			ResourceMap.Mode = ResourceMapMode.Player;
			SceneMap.Mode = SceneMapMode.Player;
			SubmoduleMap.Mode = SubmoduleMapMode.Player;
		}
		
		#endregion
		
		
		#region Update
		
		private const string UpdateMenuItemKey = "Sago/Core/Map Assets/Update";
		
		[MenuItem(UpdateMenuItemKey, false, 200)]
		private static void UpdateMenuItem() {
			Update();
		}
		
		[MenuItem(UpdateMenuItemKey, true)]
		private static bool UpdateMenuItemValidate() {
			return !Application.isPlaying;
		}
		
		private static void Update() {
			ResourceMapEditor.UpdateResourceMap();
			SceneMapEditor.UpdateSceneMap();
			SubmoduleMapEditor.UpdateSubmoduleMap();
		}
		
		#endregion
		
		
	}
	
}
