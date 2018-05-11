namespace SagoBizEditor {
	
	using SagoBiz;
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEngine;
	using UnityEditor;
	
	public class AnalyticsOptionsEditor : MonoBehaviour {
		
		[OnPlatformBootstrap(1)]
		public static void OnPlatformBootstrap() {
			PlatformSettingsPrefabEditor.AddPrefabComponents<AnalyticsOptions>();
		}
		
	}
	
}