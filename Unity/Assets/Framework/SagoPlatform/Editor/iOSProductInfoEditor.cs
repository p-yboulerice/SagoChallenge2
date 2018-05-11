namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEditor;
	using UnityEngine;
	
	public class iOSProductInfoEditor : MonoBehaviour {
		
		[OnPlatformBootstrap]
		public static void OnPlatformBootstrap() {
			PlatformSettingsPrefabEditor.AddPrefabComponents<iOSProductInfo>(PlatformUtilEditor.iOSPlatforms);
		}
		
	}
	
}