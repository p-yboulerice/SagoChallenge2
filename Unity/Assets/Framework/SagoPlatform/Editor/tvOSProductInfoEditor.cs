namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEditor;
	using UnityEngine;
	
	public class tvOSProductInfoEditor : MonoBehaviour {
		
		[OnPlatformBootstrap]
		public static void OnPlatformBootstrap() {
			PlatformSettingsPrefabEditor.AddPrefabComponents<tvOSProductInfo>(PlatformUtilEditor.tvOSPlatforms);
		}
		
	}
	
}