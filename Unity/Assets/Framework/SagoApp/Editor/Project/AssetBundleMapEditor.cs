namespace SagoAppEditor.Project {
	
	using SagoApp.Project;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The AssetBundleMapEditor provides a custom inspector for <see cref="AssetBundleMap" /> assets.
	/// </summary>
	[CustomEditor(typeof(AssetBundleMap))]
	public class AssetBundleMapEditor : Editor {
		
		
		#region Constants
		
		/// <summary>
		/// The path to the <see cref="AssetBundleMap" /> asset.
		/// </summary>
		private static string AssetBundleMapPath = "Assets/Project/Resources/AssetBundleMap.asset";
		
		#endregion
		
		
		#region Context Menu
		
		[MenuItem("CONTEXT/AssetBundleMap/Update")]
		private static void UpdateAssetBundleMapContextMenuItem(MenuCommand command) {
			UpdateAssetBundleMap();
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="AssetBundleMap" /> asset.
		/// </summary>
		public static AssetBundleMap CreateAssetBundleMap() {
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AssetBundleMap>(), AssetBundleMapPath);
			return FindAssetBundleMap();
		}
		
		/// <summary>
		/// Finds the existing <see cref="AssetBundleMap" /> asset.
		/// </summary>
		public static AssetBundleMap FindAssetBundleMap() {
			return AssetDatabase.LoadAssetAtPath(AssetBundleMapPath, typeof(AssetBundleMap)) as AssetBundleMap;
		}
		
		/// <summary>
		/// Finds the existing <see cref="AssetBundleMap" /> asset or creates a new <see cref="AssetBundleMap" /> asset.
		/// </summary>
		public static AssetBundleMap FindOrCreateAssetBundleMap() {
			return FindAssetBundleMap() ?? CreateAssetBundleMap();
		}
		
		/// <summary>
		/// Updates the <see cref="AssetBundleMap" /> asset.
		/// </summary>
		public static AssetBundleMap UpdateAssetBundleMap() {
			
			EditorUtility.DisplayProgressBar(
				"UpdateAssetBundleMap",
				"Finding asset bundle map...", 
				0
			);
			
			var serializedObject = new SerializedObject(FindOrCreateAssetBundleMap());
			
			EditorUtility.DisplayProgressBar(
				"UpdateAssetBundleMap",
				"Updating deployment info...", 
				0.5f
			);
			{
				var property = serializedObject.FindProperty("m_DeploymentInfo");
				var list = DeploymentInfoEditor.GetList(property);
				DeploymentInfoEditor.SetList(property, list);
			}
			
			EditorUtility.DisplayProgressBar(
				"UpdateAssetBundleMap",
				"Updating server info...", 
				1
			);
			{
				var property = serializedObject.FindProperty("m_ServerInfo");
				var list = ServerInfoEditor.GetList(property);
				ServerInfoEditor.SetList(property, list);
			}
			serializedObject.ApplyModifiedProperties();
			
			EditorUtility.ClearProgressBar();
			
			return FindAssetBundleMap();
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Gets the foldout flag for specified key.
		/// </summary>
		public static bool GetFoldout(string key) {
			key = string.Format("SagoAppEditor.Project.AssetBundleMapEditor.{0}", key);
			return EditorPrefs.GetBool(key, true);
		}
		
		/// <summary>
		/// Sets the foldout flag for specified key.
		/// </summary>
		public static void SetFoldout(string key, bool value) {
			key = string.Format("SagoAppEditor.Project.AssetBundleMapEditor.{0}", key);
			EditorPrefs.SetBool(key, value);
		}
		
		/// <summary>
		/// Gets the platform name.
		/// </summary>
		public static string GetPlaformName(Platform platform) {
			return platform.Equals(Platform.Unknown) ? "Default" : platform.ToString();
		}
		
		/// <summary>
		/// Gets the style for the specified key.
		/// </summary>
		public static GUIStyle GetStyle(string key) {
			GUIStyle style;
			if (!StyleDictionary.TryGetValue(key, out style)) {
				style = GUIStyle.none;
			}
			return style;
		}

		private static Dictionary<string,GUIStyle> s_StyleDictionary;
		private static Dictionary<string,GUIStyle> StyleDictionary {
			get {
				if (s_StyleDictionary == null) {
					Dictionary<string,GUIStyle> dictionary = new Dictionary<string, GUIStyle>();
					{
						GUIStyle style;
						style = new GUIStyle(GUIStyle.none);
						style.padding = new RectOffset(8,8,8,8);
						dictionary.Add("inspector", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(GUI.skin.button);
						style.fontStyle = FontStyle.Bold;
						style.margin = new RectOffset();
						style.padding = new RectOffset(0,0,0,1);
						style.fixedHeight = EditorGUIUtility.singleLineHeight;
						style.fixedWidth = 20;
						dictionary.Add("button", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(GUIStyle.none);
						style.padding = new RectOffset(16,0,0,16);
						dictionary.Add("content", style);
						dictionary.Add("content.assetbundle", style);
						dictionary.Add("content.servertype", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(GUI.skin.box);
						style.margin = new RectOffset(18,0,0,4);
						style.padding = new RectOffset(4,4,4,4);
						dictionary.Add("content.platform", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(EditorStyles.foldout);
						style.margin = new RectOffset();
						dictionary.Add("foldout", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(dictionary["foldout"]);
						style.fontStyle = FontStyle.Bold;
						dictionary.Add("foldout.platform", style);
					}
					{
						Color color;
						color = Color.white;
						
						GUIStyle style;
						style = new GUIStyle(dictionary["foldout"]);
						style.fontStyle = FontStyle.Bold;
						style.normal.textColor = color;
						style.onNormal.textColor = color;
						style.hover.textColor = color;
						style.onHover.textColor = color;
						style.focused.textColor = color;
						style.onFocused.textColor = color;
						style.active.textColor = color;
						style.onActive.textColor = color;
						
						dictionary.Add("foldout.assetbundle", style);
						dictionary.Add("foldout.servertype", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(GUIStyle.none);
						style.margin = new RectOffset(0,0,0,4);
						dictionary.Add("header", style);
						dictionary.Add("header.assetbundle", style);
						dictionary.Add("header.platform", style);
						dictionary.Add("header.servertype", style);
						dictionary.Add("row", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(EditorStyles.popup);
						style.margin = new RectOffset();
						style.fixedHeight = EditorGUIUtility.singleLineHeight;
						dictionary.Add("popup", style);
					}
					{
						GUIStyle style;
						style = new GUIStyle(EditorStyles.textField);
						style.margin = new RectOffset();
						dictionary.Add("textfield", style);
					}

					s_StyleDictionary = dictionary;
				}
				return s_StyleDictionary;
			}
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// <see cref="Editor.OnInspectorGUI" />
		/// </summary>
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.BeginVertical(GetStyle("inspector"));
				DeploymentInfoEditor.OnInspectorGUI(serializedObject.FindProperty("m_DeploymentInfo"));
				ServerInfoEditor.OnInspectorGUI(serializedObject.FindProperty("m_ServerInfo"));
			EditorGUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}
		
		#endregion
		
	}
	
	/// <summary>
	/// The DeploymentInfoEditor provides a custom user interface for the <see cref="AssetBundleMap.DeploymentInfo" /> property.
	/// </summary>
	static class DeploymentInfoEditor {
		
		
		#region Static Methods
		
		public static bool GetFoldout(string key) {
			return AssetBundleMapEditor.GetFoldout(key);
		}
		
		public static void SetFoldout(string key, bool value) {
			AssetBundleMapEditor.SetFoldout(key, value);
		}
		
		public static string GetFoldoutKey() {
			return "DeploymentInfo";
		}
		
		public static string GetFoldoutKey(string assetBundleName) {
			return string.Format("{0}.{1}", GetFoldoutKey(), assetBundleName);
			
		}
		
		public static string GetFoldoutKey(string assetBundleName, Platform platform) {
			return string.Format("{0}.{1}.{2}", GetFoldoutKey(), assetBundleName, platform.ToString());
			
		}
		
		public static List<AssetBundleDeploymentInfo> GetList(SerializedProperty property) {
			return NormalizeList(
				Enumerable
				.Range(0, property.arraySize)
				.Select(index => GetValue(property.GetArrayElementAtIndex(index)))
				.ToList()
			);
		}
		
		public static List<AssetBundleDeploymentInfo> NormalizeList(List<AssetBundleDeploymentInfo> list) {
			List<AssetBundleDeploymentInfo> normalized = list.GetRange(0, list.Count);
			foreach (string assetBundleName in AssetDatabase.GetAllAssetBundleNames()) {
				if (normalized.Where(value => value.AssetBundleName.Equals(assetBundleName) && value.Platform.Equals(Platform.Unknown)).Count() == 0) {
					AssetBundleDeploymentInfo value = new AssetBundleDeploymentInfo();
					value.AssetBundleName = assetBundleName;
					value.Platform = Platform.Unknown;
					value.DeploymentType = AssetBundleDeploymentType.Local;
					normalized.Add(value);
				}
			}
			return normalized;
		}
		
		public static void SetList(SerializedProperty property, List<AssetBundleDeploymentInfo> list) {
			List<AssetBundleDeploymentInfo> normalized = NormalizeList(list);
			property.arraySize = normalized.Count;
			for (int index = 0; index < normalized.Count; index++) {
				SetValue(property.GetArrayElementAtIndex(index), normalized[index]);
			}
		}
		
		public static string GetPlaformName(Platform platform) {
			return AssetBundleMapEditor.GetPlaformName(platform);
		}
		
		public static GUIStyle GetStyle(string key) {
			return AssetBundleMapEditor.GetStyle(key);
		}
		
		public static AssetBundleDeploymentInfo GetValue(SerializedProperty property) {
			AssetBundleDeploymentInfo value = new AssetBundleDeploymentInfo();
			value.AssetBundleName = property.FindPropertyRelative("AssetBundleName").stringValue;
			value.Platform = (Platform)property.FindPropertyRelative("Platform").intValue;
			value.DeploymentType = (AssetBundleDeploymentType)property.FindPropertyRelative("DeploymentType").intValue;
			return value;
		}
		
		public static void SetValue(SerializedProperty property, AssetBundleDeploymentInfo deploymentInfo) {
			property.FindPropertyRelative("AssetBundleName").stringValue = deploymentInfo.AssetBundleName;
			property.FindPropertyRelative("Platform").intValue = (int)deploymentInfo.Platform;
			property.FindPropertyRelative("DeploymentType").intValue = (int)deploymentInfo.DeploymentType;
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void PlatformDropDown(Rect rect, List<AssetBundleDeploymentInfo> list, string assetBundleName, System.Action<AssetBundleDeploymentInfo> block) {
			
			Platform[] allPlatforms;
			allPlatforms = (
				PlatformUtil
				.AllPlatforms
				.ToArray()
			);
			
			Platform[] existingPlatforms;
			existingPlatforms = (
				list
				.Where(value => value.AssetBundleName.Equals(assetBundleName))
				.Select(value => value.Platform)
				.ToArray()
			);
			
			GenericMenu menu;
			menu = new GenericMenu();
			
			for (int index = 0 ; index < allPlatforms.Length; index++) {
				
				Platform platform;
				platform = allPlatforms[index];
				
				GenericMenu.MenuFunction callback;
				callback = () => {
					AssetBundleDeploymentInfo value = new AssetBundleDeploymentInfo();
					value.AssetBundleName = assetBundleName;
					value.Platform = platform;
					value.DeploymentType = AssetBundleDeploymentType.Local;
					block(value);
				};
				
				GUIContent content;
				content = new GUIContent(GetPlaformName(platform));
				
				if (System.Array.IndexOf(existingPlatforms, platform) != -1) {
					menu.AddDisabledItem(content);
				} else {
					menu.AddItem(content, false, callback);
				}
				
			}
			
			menu.DropDown(rect);
			
		}

		public static bool Foldout(string foldoutKey, string label, GUIStyle style = null) {
			EditorGUI.BeginChangeCheck();
			bool foldout = EditorGUILayout.Foldout(
					GetFoldout(foldoutKey), 
					label, 
					style ?? GetStyle("foldout")
				);
			if (EditorGUI.EndChangeCheck()) {
				SetFoldout(foldoutKey, foldout);
			}
			return foldout;
		}

		public static void OnInspectorGUI(SerializedProperty property) {
			
			string foldoutKey;
			foldoutKey = GetFoldoutKey();
			
			EditorGUILayout.BeginHorizontal(GetStyle("header"));
				bool foldout = Foldout(foldoutKey, "Deployment Info");
			EditorGUILayout.EndHorizontal();
			
			if (foldout) {

				List<AssetBundleDeploymentInfo> list;
				list = GetList(property);

				EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginVertical(GetStyle("content"));
					
					List<string> assetBundleNames = (
						AssetDatabase
						.GetAllAssetBundleNames()
						.ToList()
					);

					GUIStyle styleHeaderPlatform = GetStyle("header.platform");
					GUIStyle styleFoldoutPlatform = GetStyle("foldout.platform");
					GUIStyle styleContentPlatform = GetStyle("content.platform");
					GUIStyle styleHeaderAssetBundle = GetStyle("header.assetbundle");
					GUIStyle styleFoldoutAssetBundle = GetStyle("foldout.assetbundle");
					GUIStyle styleContentAssetBundle = GetStyle("content.assetbundle");
					GUIStyle styleRow = GetStyle("row");
					GUIStyle styleButton = GetStyle("button");
					GUIStyle stylePopup = GetStyle("popup");

					foreach (string assetBundleName in assetBundleNames) {
						
						string assetBundleFoldoutKey;
						assetBundleFoldoutKey = GetFoldoutKey(assetBundleName);

						bool isAssetBundleFoldout;

						EditorGUILayout.BeginHorizontal(styleHeaderAssetBundle);
						{
							isAssetBundleFoldout = Foldout(assetBundleFoldoutKey, 
								assetBundleName, 
								styleFoldoutAssetBundle);

							Rect rect = GUILayoutUtility.GetRect(GUIContent.none,GUIStyle.none,GUILayout.Width(1),GUILayout.Height(1));
							if (GUILayout.Button("+", styleButton)) {
								PlatformDropDown(rect, list, assetBundleName, (AssetBundleDeploymentInfo info) => {
									
									int index = property.arraySize;
									property.InsertArrayElementAtIndex(index);
									SetValue(property.GetArrayElementAtIndex(index), info);
									property.serializedObject.ApplyModifiedProperties();
									
									SetFoldout(GetFoldoutKey(info.AssetBundleName), true);
									SetFoldout(GetFoldoutKey(info.AssetBundleName, info.Platform), true);
									
								});
							}
						}
						EditorGUILayout.EndHorizontal();
						
						if (isAssetBundleFoldout) {
							EditorGUILayout.BeginVertical(styleContentAssetBundle);
								
								List<AssetBundleDeploymentInfo> tempList = (
									list
									.Where(deploymentInfo => deploymentInfo.AssetBundleName.Equals(assetBundleName))
									.OrderBy(deploymentInfo => deploymentInfo.Platform)
									.ToList()
								);
								
								foreach (AssetBundleDeploymentInfo tempInfo in tempList) {
										
									AssetBundleDeploymentInfo info;
									info = tempInfo;
									
									int infoIndex;
									infoIndex = list.IndexOf(info);
									
									string infoFoldoutKey;
									infoFoldoutKey = GetFoldoutKey(info.AssetBundleName, info.Platform);

									bool isInfoFoldout;

									EditorGUILayout.BeginHorizontal(styleHeaderPlatform);
									{
										isInfoFoldout = Foldout(infoFoldoutKey, GetPlaformName(info.Platform), styleFoldoutPlatform);

										EditorGUI.BeginDisabledGroup(info.Platform == Platform.Unknown);
										if (GUILayout.Button("-", styleButton)) {
											infoIndex = -1;
											list.Remove(info);
										}
										EditorGUI.EndDisabledGroup();
									}
									EditorGUILayout.EndHorizontal();
									
									if (isInfoFoldout) {
										EditorGUILayout.BeginVertical(styleContentPlatform);
											EditorGUILayout.BeginHorizontal(styleRow);
												info.DeploymentType = (AssetBundleDeploymentType)EditorGUILayout.EnumPopup("Deployment Type", info.DeploymentType, stylePopup);
											EditorGUILayout.EndHorizontal();
										EditorGUILayout.EndVertical();
									}
									if (infoIndex != -1) {
										list[infoIndex] = info;
									}
									
								}
								
							EditorGUILayout.EndVertical();
						}
						
					}
					
				EditorGUILayout.EndVertical();

				if (EditorGUI.EndChangeCheck()) {
					SetList(property, list);
				}
			}
			
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The ServerInfoEditor provides a custom user interface for the <see cref="AssetBundleMap.ServerInfo" /> property.
	/// </summary>
	static class ServerInfoEditor {
		
		
		#region Static Methods
		
		public static bool GetFoldout(string key) {
			return AssetBundleMapEditor.GetFoldout(key);
		}
		
		public static void SetFoldout(string key, bool value) {
			AssetBundleMapEditor.SetFoldout(key, value);
		}
		
		public static string GetFoldoutKey() {
			return "ServerInfo";
		}
		
		public static string GetFoldoutKey(AssetBundleServerType serverType) {
			return string.Format("{0}.{1}", GetFoldoutKey(), serverType.ToString());
		}
		
		public static string GetFoldoutKey(AssetBundleServerType serverType, Platform platform) {
			return string.Format("{0}.{1}.{2}", GetFoldoutKey(), serverType.ToString(), platform.ToString());
		}
		
		public static List<AssetBundleServerInfo> GetList(SerializedProperty property) {
			return NormalizeList(
				Enumerable
				.Range(0, property.arraySize)
				.Select(index => GetValue(property.GetArrayElementAtIndex(index)))
				.ToList()
			);
		}
		
		public static List<AssetBundleServerInfo> NormalizeList(List<AssetBundleServerInfo> list) {
			
			List<AssetBundleServerInfo> normalized = list.GetRange(0, list.Count);
				
			AssetBundleServerType[] serverTypes = (
				System.Enum.GetValues(typeof(AssetBundleServerType))
				.OfType<AssetBundleServerType>()
				.Where(serverType => serverType != AssetBundleServerType.Unknown)
				.ToArray()
			);
			
			foreach (AssetBundleServerType serverType in serverTypes) {
				if (normalized.Where(info => info.ServerType.Equals(serverType) && info.Platform.Equals(Platform.Unknown)).Count() == 0) {
					AssetBundleServerInfo value = new AssetBundleServerInfo();
					value.Platform = Platform.Unknown;
					value.ServerType = serverType;
					normalized.Add(value);
				}
			}
			
			return normalized;
			
		}
		
		public static void SetList(SerializedProperty property, List<AssetBundleServerInfo> list) {
			List<AssetBundleServerInfo> normalized = NormalizeList(list);
			property.arraySize = normalized.Count;
			for (int index = 0; index < normalized.Count; index++) {
				SetValue(property.GetArrayElementAtIndex(index), normalized[index]);
			}
		}
		
		public static string GetPlaformName(Platform platform) {
			return AssetBundleMapEditor.GetPlaformName(platform);
		}
		
		public static GUIStyle GetStyle(string key) {
			return AssetBundleMapEditor.GetStyle(key);
		}
		
		public static AssetBundleServerInfo GetValue(SerializedProperty property) {
			AssetBundleServerInfo serverInfo = new AssetBundleServerInfo();
			serverInfo.Platform = (Platform)property.FindPropertyRelative("Platform").intValue;
			serverInfo.ServerType = (AssetBundleServerType)property.FindPropertyRelative("ServerType").intValue;
			serverInfo.Url = property.FindPropertyRelative("Url").stringValue;
			return serverInfo;
		}
		
		public static void SetValue(SerializedProperty property, AssetBundleServerInfo serverInfo) {
			property.FindPropertyRelative("Platform").intValue = (int)serverInfo.Platform;
			property.FindPropertyRelative("ServerType").intValue = (int)serverInfo.ServerType;
			property.FindPropertyRelative("Url").stringValue = serverInfo.Url;
		}

		public static bool Foldout(string foldoutKey, string label, GUIStyle style = null) {
			return DeploymentInfoEditor.Foldout(foldoutKey, label, style);
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void PlatformDropDown(Rect rect, List<AssetBundleServerInfo> list, AssetBundleServerType serverType, System.Action<AssetBundleServerInfo> block) {
			
			Platform[] allPlatforms;
			allPlatforms = (
				PlatformUtil
				.AllPlatforms
				.ToArray()
			);
			
			Platform[] existingPlatforms;
			existingPlatforms = (
				list
				.Where(value => value.ServerType.Equals(serverType))
				.Select(value => value.Platform)
				.ToArray()
			);
			
			GenericMenu menu;
			menu = new GenericMenu();
			
			for (int index = 0 ; index < allPlatforms.Length; index++) {
				
				Platform platform;
				platform = allPlatforms[index];
				
				GenericMenu.MenuFunction callback;
				callback = () => {
					
					AssetBundleServerInfo value;
					value = new AssetBundleServerInfo();
					value.Platform = platform;
					value.ServerType = serverType;
					
					block(value);
					
				};
				
				GUIContent content;
				content = new GUIContent(GetPlaformName(platform));
				
				if (System.Array.IndexOf(existingPlatforms, platform) != -1) {
					menu.AddDisabledItem(content);
				} else {
					menu.AddItem(content, false, callback);
				}
				
			}
			
			menu.DropDown(rect);
			
		}
		
		public static void OnInspectorGUI(SerializedProperty property) {
			
			string foldoutKey;
			foldoutKey = GetFoldoutKey();
			
			EditorGUILayout.BeginHorizontal(GetStyle("header"));
				bool foldout = Foldout(foldoutKey, "Server Info");
			EditorGUILayout.EndHorizontal();
			
			if (foldout) {
				
				List<AssetBundleServerInfo> list;
				list = GetList(property);
				EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginVertical(GetStyle("content"));
					
					AssetBundleServerType[] serverTypes = (
						System.Enum.GetValues(typeof(AssetBundleServerType))
						.OfType<AssetBundleServerType>()
						.Where(serverType => serverType != AssetBundleServerType.Unknown)
						.ToArray()
					);

					GUIStyle styleHeaderServerType = GetStyle("header.servertype");
					GUIStyle styleFoldoutServerType = GetStyle("foldout.servertype");
					GUIStyle styleButton = GetStyle("button");
					GUIStyle styleContentServerType = GetStyle("content.servertype");
					GUIStyle styleHeaderPlatform = GetStyle("header.platform");
					GUIStyle styleFoldoutPlatform = GetStyle("foldout.platform");
					GUIStyle styleContentPlatform = GetStyle("content.platform");
					GUIStyle styleRow = GetStyle("row");
					GUIStyle styleTextField = GetStyle("textfield");

					foreach (AssetBundleServerType serverType in serverTypes) {
						
						string serverTypeFoldoutKey;
						serverTypeFoldoutKey = GetFoldoutKey(serverType);
						
						EditorGUILayout.BeginHorizontal(styleHeaderServerType);
						{
							Foldout(serverTypeFoldoutKey, 
								serverType.ToString(), 
								styleFoldoutServerType
							);

							Rect rect = GUILayoutUtility.GetRect(
								GUIContent.none,
								GUIStyle.none,
								GUILayout.Width(1),
								GUILayout.Height(1)
							);
							if (GUILayout.Button("+", styleButton)) {
								PlatformDropDown(rect, list, serverType, (AssetBundleServerInfo serverInfo) => {
									
									int i = property.arraySize;
									property.InsertArrayElementAtIndex(i);
									SetValue(property.GetArrayElementAtIndex(i), serverInfo);
									property.serializedObject.ApplyModifiedProperties();
									
									SetFoldout(GetFoldoutKey(serverInfo.ServerType), true);
									SetFoldout(GetFoldoutKey(serverInfo.ServerType, serverInfo.Platform), true);
									
								});
							}
						}
						EditorGUILayout.EndHorizontal();
						
						if (GetFoldout(serverTypeFoldoutKey)) {
							EditorGUILayout.BeginVertical(styleContentServerType);
								
								List<AssetBundleServerInfo> tempList = (
									list
									.Where(serverInfo => serverInfo.ServerType.Equals(serverType))
									.OrderBy(serverInfo => serverInfo.Platform)
									.ToList()
								);
								
								foreach (AssetBundleServerInfo tempInfo in tempList) {
									
									AssetBundleServerInfo info;
									info = tempInfo;
									
									int infoIndex;
									infoIndex = list.IndexOf(info);
									
									string infoFoldoutKey;
									infoFoldoutKey = GetFoldoutKey(info.ServerType, info.Platform);
									
									EditorGUILayout.BeginHorizontal(styleHeaderPlatform);
									{
										Foldout(infoFoldoutKey,
											GetPlaformName(info.Platform), 
											styleFoldoutPlatform
										);

										EditorGUI.BeginDisabledGroup(info.Platform == Platform.Unknown);
										if (GUILayout.Button("-", styleButton)) {
											infoIndex = -1;
											list.Remove(info);
										}
										EditorGUI.EndDisabledGroup();
									}
									EditorGUILayout.EndHorizontal();
									
									if (GetFoldout(infoFoldoutKey)) {
										EditorGUILayout.BeginVertical(styleContentPlatform);
											EditorGUILayout.BeginHorizontal(styleRow);
												info.Url = EditorGUILayout.TextField("Url", info.Url, styleTextField);
											EditorGUILayout.EndHorizontal();
										EditorGUILayout.EndVertical();
									}
									
									if (infoIndex != -1) {
										list[infoIndex] = info;
									}
									
								}
								
							EditorGUILayout.EndVertical();
						}
						
					}
					
				EditorGUILayout.EndVertical();

				if (EditorGUI.EndChangeCheck()) {
					SetList(property, list);
				}
			}

		}
		
		#endregion
		
		
	}
	
}