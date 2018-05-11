namespace SagoAppEditor.Project {
	
	using SagoApp.Project;
	using SagoPlatform;
	using SagoCore.AssetBundles;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	
	/// <summary>
	/// The AssetBundleAdaptorMapEditor class provides a custom inspector for <see cref="AssetBundleAdaptorMap" /> assets.
	/// </summary>
	[CustomEditor(typeof(AssetBundleAdaptorMap))]
	public class AssetBundleAdaptorMapEditor : Editor {
		
		
		#region Constants
		
		/// <summary>
		/// The path to the <see cref="AssetBundleAdaptorMap" /> asset.
		/// </summary>
		private const string AssetBundleAdaptorMapPath = "Assets/Project/Resources/AssetBundleAdaptorMap.asset";
		
		/// <summary>
		/// The <see cref="EditorPrefs" /> key for the remote adaptor type.
		/// </summary>
		/// <seealso cref="GetRemoteAdaptorType" />
		/// <seealso cref="SetRemoteAdaptorType" />
		private const string RemoteAdaptorTypeKey = "SagoAppEditor.Project.AssetBundleAdaptorMapEditor.RemoteAdaptorType";
		
		/// <summary>
		/// The <see cref="EditorPrefs" /> key for the server type.
		/// </summary>
		/// <seealso cref="GetServerType" />
		/// <seealso cref="SetServerType" />
		private const string ServerTypeKey = "SagoAppEditor.Project.AssetBundleAdaptorMapEditor.ServerType";
		
		#endregion
		
		
		#region Context Menu
		
		[MenuItem("CONTEXT/AssetBundleAdaptorMap/Update")]
		private static void UpdateAssetBundleAdaptorMapContextMenuItem(MenuCommand command) {
			UpdateAssetBundleAdaptorMap();
		}
		
		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the adaptor type for the specified deployment type.
		/// </summary>
		/// <param name="deploymentType">
		/// The deployment type.
		/// </param>
		public static AssetBundleAdaptorType GetAdaptorType(AssetBundleDeploymentType deploymentType) {
			switch (deploymentType) {
				case AssetBundleDeploymentType.Unknown: {
					return AssetBundleAdaptorType.Unknown;
				}
				case AssetBundleDeploymentType.Local: {
					return AssetBundleAdaptorType.StreamingAssets;
				}
				case AssetBundleDeploymentType.Remote: {
					return GetRemoteAdaptorType();
				}
			}
			return AssetBundleAdaptorType.Unknown;
		}
		
		/// <summary>
		/// Gets the remote adaptor type.
		/// </summary>
		public static AssetBundleAdaptorType GetRemoteAdaptorType() {
			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_ON_DEMAND_RESOURCES
				return AssetBundleAdaptorType.OnDemandResources;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_DEVELOPMENT_SERVER
				return AssetBundleAdaptorType.AssetBundleServer;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_STAGING_SERVER
				return AssetBundleAdaptorType.AssetBundleServer;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_PRODUCTION_SERVER
				return AssetBundleAdaptorType.AssetBundleServer;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				return AssetBundleAdaptorType.StreamingAssets;
			#else
				return (AssetBundleAdaptorType)EditorPrefs.GetInt(RemoteAdaptorTypeKey, 0);
			#endif
		}
		
		/// <summary>
		/// Sets the remote adaptor type.
		/// </summary>
		/// <param name="adaptorType">
		/// The adaptor type.
		/// </param>
		/// <remarks>
		/// <para>
		/// The remote adaptor type must be set before bootstrapping the <see cref="AssetBundleAdaptorMap" /> 
		/// (so the <see cref="AssetBundleDeploymentType" /> in the <see cref="AssetBundleMap" /> can be
		/// resolved to an <see cref="AssetBundleAdaptorType" /> in the <see cref="AssetBundleAdaptorMap" />).
		/// </para>
		/// <para>
		/// In Cloud Build, the remote adaptor type will need to be set using one of the following define symbols:
		/// <code>
		/// SAGO_ASSET_BUNDLES_USE_ON_DEMAND_RESOURCES
		/// SAGO_ASSET_BUNDLES_USE_DEVELOPMENT_SERVER
		/// SAGO_ASSET_BUNDLES_USE_STAGING_SERVER
		/// SAGO_ASSET_BUNDLES_USE_PRODUCTION_SERVER
		/// </code>
		/// </para>
		/// <para>
		/// In the editor, the remote adaptor type will need be set by the method that 
		/// starts a build. We don't want to use define symbols in the editor because 
		/// the remote adaptor type is build-specific and shouldn't be committed to the 
		/// repository (define symbols are stored in Unity's project settings which are
		/// committed to the repository).
		/// </para>
		/// </remarks>
		public static void SetRemoteAdaptorType(AssetBundleAdaptorType adaptorType) {
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				if (adaptorType == AssetBundleAdaptorType.Unknown) {
					EditorPrefs.DeleteKey(RemoteAdaptorTypeKey);
				} else {
					EditorPrefs.SetInt(RemoteAdaptorTypeKey, (int)adaptorType);
				}
			#endif
		}
		
		/// <summary>
		/// Gets the server type.
		/// </summary>
		public static AssetBundleServerType GetServerType() {
			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_DEVELOPMENT_SERVER
				return AssetBundleServerType.Development;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_STAGING_SERVER
				return AssetBundleServerType.Staging;
			#elif (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_ASSET_BUNDLES_USE_PRODUCTION_SERVER
				return AssetBundleServerType.Production;
			#else
				return (AssetBundleServerType)EditorPrefs.GetInt(ServerTypeKey, 0);
			#endif
		}
		
		/// <summary>
		/// Sets the server type (only used when the remote adaptor type 
		/// is <see cref="AssetBundleAdaptorType.AssetBundleServer" />).
		/// </summary>
		/// <param name="serverType">
		/// The server type.
		/// </param>
		/// <remarks>
		/// <para>
		/// The server type must be set before bootstrapping the <see cref="AssetBundleAdaptorMap" /> 
		/// (so the <see cref="AssetBundleServerInfo" /> in the <see cref="AssetBundleMap" /> can be
		/// resolved to <see cref="ServerAdaptorOptions" /> in the <see cref="AssetBundleAdaptorMap" />).
		/// </para>
		/// <para>
		/// In Cloud Build, the server type will need to be set using one of the following define symbols:
		/// <code>
		/// SAGO_ASSET_BUNDLES_USE_DEVELOPMENT_SERVER
		/// SAGO_ASSET_BUNDLES_USE_STAGING_SERVER
		/// SAGO_ASSET_BUNDLES_USE_PRODUCTION_SERVER
		/// </code>
		/// </para>
		/// <para>
		/// In the editor, the server type will need be set by the method that 
		/// starts a build. We don't want to use define symbols in the editor 
		/// because the server type is build-specific and shouldn't be committed 
		/// to the repository (define symbols are stored in Unity's project 
		/// settings which are committed to the repository).
		/// </para>
		/// </remarks>
		public static void SetServerType(AssetBundleServerType serverType) {
            #if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
			if (serverType == AssetBundleServerType.Unknown) {
				EditorPrefs.DeleteKey(ServerTypeKey);
			} else {
				EditorPrefs.SetInt(ServerTypeKey, (int)serverType);
			}
			#endif
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="AssetBundleAdaptorMap" /> asset.
		/// </summary>
		public static AssetBundleAdaptorMap CreateAssetBundleAdaptorMap() {
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AssetBundleAdaptorMap>(), AssetBundleAdaptorMapPath);
			return FindAssetBundleAdaptorMap();
		}
		
		/// <summary>
		/// Finds the existing <see cref="AssetBundleAdaptorMap" /> asset.
		/// </summary>
		public static AssetBundleAdaptorMap FindAssetBundleAdaptorMap() {
			return AssetDatabase.LoadAssetAtPath(AssetBundleAdaptorMapPath, typeof(AssetBundleAdaptorMap)) as AssetBundleAdaptorMap;
		}
		
		/// <summary>
		/// Finds the existing <see cref="AssetBundleAdaptorMap" /> asset or 
		/// creates a new <see cref="AssetBundleAdaptorMap" /> asset.
		/// </summary>
		public static AssetBundleAdaptorMap FindOrCreateAssetBundleAdaptorMap() {
			return FindAssetBundleAdaptorMap() ?? CreateAssetBundleAdaptorMap();
		}
		
		/// <summary>
		/// Updates the <see cref="AssetBundleAdaptorMap" /> asset by resolving 
		/// the metadata in the <see cref="AssetBundleMap" /> using the remote 
		/// adaptor type and server type.
		/// </summary>
		public static AssetBundleAdaptorMap UpdateAssetBundleAdaptorMap(bool showProgress = true) {

			if (showProgress) {
				EditorUtility.DisplayProgressBar(
					"UpdateAssetBundleAdaptorMap",
					"Finding asset bundle adaptor map...", 
					0
				);
			}
			
			AssetBundleMap assetBundleMap;
			assetBundleMap = AssetBundleMapEditor.FindAssetBundleMap();
			
			AssetBundleAdaptorMap assetBundleAdaptorMap;
			assetBundleAdaptorMap = FindOrCreateAssetBundleAdaptorMap();
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(assetBundleAdaptorMap);
			
			{
				string[] assetBundleNames;
				assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
				
				SerializedProperty assetBundleNamesProperty;
				assetBundleNamesProperty = serializedObject.FindProperty("m_AssetBundleNames");
				assetBundleNamesProperty.arraySize = assetBundleNames.Length;
				
				SerializedProperty adaptorTypesProperty;
				adaptorTypesProperty = serializedObject.FindProperty("m_AdaptorTypes");
				adaptorTypesProperty.arraySize = assetBundleNames.Length;
				
				for (int index = 0; index < assetBundleNames.Length; index++) {
			
					if (showProgress) {
						EditorUtility.DisplayProgressBar(
							"UpdateAssetBundleAdaptorMap",
							"Updating adaptor types...", 
							index / (float)(assetBundleNames.Length)
						);
					}

					string assetBundleName;
					assetBundleName = assetBundleNames[index];
					
					AssetBundleDeploymentInfo info;
					info = (
						assetBundleMap
						.DeploymentInfo
						.Where(i => i.AssetBundleName.Equals(assetBundleName))
						.Where(i => i.Platform == PlatformUtil.ActivePlatform || i.Platform == Platform.Unknown)
						.OrderByDescending(i => i.Platform)
						.FirstOrDefault()
					);
					
					assetBundleNamesProperty.GetArrayElementAtIndex(index).stringValue = info.AssetBundleName;
					adaptorTypesProperty.GetArrayElementAtIndex(index).intValue = (int)GetAdaptorType(info.DeploymentType);
					
				}
				
			}
			
			{
				
				if (showProgress) {
					EditorUtility.DisplayProgressBar(
						"UpdateAssetBundleAdaptorMap",
						"Updating server info...", 
						1
					);
				}
				
				SerializedProperty property;
				property = serializedObject.FindProperty("m_ServerAdaptorOptions");
				
				if (GetRemoteAdaptorType() == AssetBundleAdaptorType.AssetBundleServer) {
				
					AssetBundleServerInfo info;
					info = (
						assetBundleMap
						.ServerInfo
						.Where(i => i.ServerType.Equals(GetServerType()))
						.Where(i => i.Platform == PlatformUtil.ActivePlatform || i.Platform == Platform.Unknown)
						.OrderByDescending(i => i.Platform)
						.FirstOrDefault()
					);
					
					property.FindPropertyRelative("Url").stringValue = info.Url;
					
				} else {
					
					property.FindPropertyRelative("Url").stringValue = string.Empty;
					
				}
			}
			
			serializedObject.ApplyModifiedProperties();
			
			if (showProgress) EditorUtility.ClearProgressBar();
			
			return FindAssetBundleAdaptorMap();
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// <see cref="Editor.OnInspectorGUI" />
		/// </summary>
		override public void OnInspectorGUI() {
			
			GUIStyle containerStyle;
			containerStyle = new GUIStyle(GUIStyle.none);
			containerStyle.margin = new RectOffset(0,16,16,16);
			
			GUIStyle headingStyle;
			headingStyle = new GUIStyle(GUI.skin.label);
			headingStyle.fontStyle = FontStyle.Bold;
			headingStyle.normal.textColor = Color.white;
			
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.EndDisabledGroup();
			
			EditorGUILayout.BeginVertical(containerStyle);
				EditorGUILayout.LabelField("Build", headingStyle);
				EditorGUI.BeginChangeCheck();
				SetRemoteAdaptorType((AssetBundleAdaptorType)EditorGUILayout.EnumPopup("Remote Adaptor Type", GetRemoteAdaptorType()));
				if (GetRemoteAdaptorType() == AssetBundleAdaptorType.AssetBundleServer) {
					SetServerType((AssetBundleServerType)EditorGUILayout.EnumPopup("Server Type", GetServerType()));
				}
				if (EditorGUI.EndChangeCheck()) {
					UpdateAssetBundleAdaptorMap();
				}
			EditorGUILayout.EndVertical();
			
			serializedObject.Update();
			
			SerializedProperty adaptorTypesProperty;
			adaptorTypesProperty = serializedObject.FindProperty("m_AdaptorTypes");
			
			SerializedProperty assetBundleNamesProperty;
			assetBundleNamesProperty = serializedObject.FindProperty("m_AssetBundleNames");
			
			EditorGUILayout.BeginVertical(containerStyle);
				EditorGUILayout.LabelField("Asset Bundles", headingStyle);
				EditorGUI.BeginDisabledGroup(true);
					for (int index = 0; index < assetBundleNamesProperty.arraySize; index++) {
						
						SerializedProperty nameProperty;
						nameProperty = assetBundleNamesProperty.GetArrayElementAtIndex(index);
						
						SerializedProperty typeProperty;
						typeProperty = adaptorTypesProperty.GetArrayElementAtIndex(index);
						typeProperty.intValue = (int)(AssetBundleAdaptorType)EditorGUILayout.EnumPopup(
							nameProperty.stringValue,
							(AssetBundleAdaptorType)typeProperty.intValue
						);
						
					}
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
			
			if (GetRemoteAdaptorType() == AssetBundleAdaptorType.AssetBundleServer) {
				EditorGUILayout.BeginVertical(containerStyle);
					EditorGUILayout.LabelField("Server", headingStyle);
					EditorGUI.BeginDisabledGroup(true);
						
						SerializedProperty tokenProperty;
						tokenProperty = serializedObject.FindProperty("m_ServerAdaptorOptions.Token");
						EditorGUILayout.PropertyField(tokenProperty);
						
						SerializedProperty urlProperty;
						urlProperty = serializedObject.FindProperty("m_ServerAdaptorOptions.Url");
						EditorGUILayout.PropertyField(urlProperty);
						
					EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}
			
			serializedObject.ApplyModifiedProperties();
			
		}
		
		#endregion
		
		
	}
	
}