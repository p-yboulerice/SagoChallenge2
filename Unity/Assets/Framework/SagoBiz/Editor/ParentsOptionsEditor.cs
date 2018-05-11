namespace SagoBizEditor {
	
	using SagoBiz;
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEngine;
	using UnityEditor;
	
	public class ParentsOptionsEditor : MonoBehaviour {
		
		[OnPlatformBootstrap(2)]
		public static void OnPlatformBootstrap() {
			PlatformSettingsPrefabEditor.AddPrefabComponents<ParentsOptions>();
		}
		
	}
	
}