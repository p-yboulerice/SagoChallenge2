namespace SagoBizEditor {
	
	using SagoBiz;
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEngine;
	using UnityEditor;
	
	public class ServiceOptionsEditor : MonoBehaviour {
		
		[OnPlatformBootstrap(3)]
		public static void OnPlatformBootstrap() {
			PlatformSettingsPrefabEditor.AddPrefabComponents<ServiceOptions>();
		}
		
	}
	
}