namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Reflection;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class AndroidProductInfoEditor : MonoBehaviour {
		
		[OnPlatformBootstrap]
		public static void OnPlatformBootstrap() {

			// Getting the list of all android platforms except Google Play and Google Play Free
			Platform[] androidPlatformsExceptGooglePlay = PlatformUtilEditor.AndroidPlatforms
				.Where(p => p != Platform.GooglePlay && p != Platform.GooglePlayFree).ToArray();

			// Adding AndroidProductInfo component to all platforms except Google Play and Google Play Free
			PlatformSettingsPrefabEditor.AddPrefabComponents<AndroidProductInfo>(androidPlatformsExceptGooglePlay);

			// Adding GooglePlayProductInfo to Google Play / Google Play Free platforms and remove the AndroidProductInfo
			PlatformSettingsPrefabEditor.AddPrefabComponents<GooglePlayProductInfo>(Platform.GooglePlay);
			PlatformSettingsPrefabEditor.AddPrefabComponents<GooglePlayProductInfo>(Platform.GooglePlayFree);

			// GooglePlay
			PlatformSettingsPrefab[] prefabs = PlatformSettingsPrefabEditor.GetPrefabs(Platform.GooglePlay);

			foreach (PlatformSettingsPrefab googlePlayPrefab in prefabs){
				AndroidProductInfo[] androidInfoComponents = googlePlayPrefab.GetComponents<AndroidProductInfo>();

				var obsoleteComponents = androidInfoComponents.Where( info => info.GetType() != typeof(GooglePlayProductInfo));
				if (obsoleteComponents.Count() != 0) {
					AndroidProductInfo newComponent = androidInfoComponents.Where( info => info.GetType() == typeof(GooglePlayProductInfo)).Single();
					AndroidProductInfo obsoleteComponent = obsoleteComponents.First();
					newComponent.name = obsoleteComponent.name;
					newComponent.Identifier = obsoleteComponent.Identifier;
					newComponent.Version = obsoleteComponent.Version;
					newComponent.Build = obsoleteComponent.Build;
					newComponent.SdkVersion = obsoleteComponent.SdkVersion;

					foreach ( AndroidProductInfo obs in obsoleteComponents) {
						DestroyImmediate(obs, true);
					}
				}
			}

			// GooglePlayFree
			PlatformSettingsPrefab[] prefabsGooglePlayFree = PlatformSettingsPrefabEditor.GetPrefabs(Platform.GooglePlay);
			
			foreach (PlatformSettingsPrefab googlePlayFreePrefab in prefabsGooglePlayFree){
				AndroidProductInfo[] androidInfoComponents = googlePlayFreePrefab.GetComponents<AndroidProductInfo>();
				
				var obsoleteComponents = androidInfoComponents.Where( info => info.GetType() != typeof(GooglePlayProductInfo));
				if (obsoleteComponents.Count() != 0) {
					AndroidProductInfo newComponent = androidInfoComponents.Where( info => info.GetType() == typeof(GooglePlayProductInfo)).Single();
					AndroidProductInfo obsoleteComponent = obsoleteComponents.First();
					newComponent.name = obsoleteComponent.name;
					newComponent.Identifier = obsoleteComponent.Identifier;
					newComponent.Version = obsoleteComponent.Version;
					newComponent.Build = obsoleteComponent.Build;
					newComponent.SdkVersion = obsoleteComponent.SdkVersion;
					
					foreach ( AndroidProductInfo obs in obsoleteComponents) {
						DestroyImmediate(obs, true);
					}
				}
			}
		}

	}
	
	[CustomPropertyDrawer(typeof(AndroidProductInfoSdkAttribute))]
	public class AndroidProductInfoSdkAttributeDrawer : PropertyDrawer {
		
		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			AndroidSdkVersions sdkVersion;
			sdkVersion = (AndroidSdkVersions)System.Enum.ToObject(typeof(Platform) , property.intValue);
			sdkVersion = (AndroidSdkVersions)EditorGUI.EnumPopup(position, label, sdkVersion);
			property.intValue = (int)sdkVersion;
			EditorGUI.EndProperty();
		}
		
	}
	
}