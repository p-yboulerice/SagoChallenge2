namespace SagoAppEditor.Workflow {
	
	using SagoApp.Content;
	using SagoApp.Project;
	using SagoAppEditor.Content;
	using SagoAppEditor.Project;
	using SagoBuildEditor.Core;
	using SagoCore.AssetBundles;
	using SagoCore.Resources;
	using SagoCore.Scenes;
	using SagoCore.Submodules;
	using SagoCoreEditor.AssetBundles;
	using SagoCoreEditor.Resources;
	using SagoCoreEditor.Scenes;
	using SagoCoreEditor.Submodules;
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	
	#if UNITY_ANDROID
	using SagoBuildEditor.Android;
	#endif
	
	#if UNITY_IOS
	using SagoBuildEditor.iOS;
	using iOSBuildType = SagoBuildEditor.iOS.iOSBuildType;
	#endif
	
	#if UNITY_TVOS
	using SagoBuildEditor.tvOS;
	#endif
	
	#if ENABLE_IOS_ON_DEMAND_RESOURCES
	using UnityEditor.iOS;
	#endif
	
	public struct WorkflowError {
		public string Message;
		public MessageType MessageType;
	}
	
	public enum WorkflowMode {
			
		/// <summary>
		/// The Develop Mode. Use this mode while you're working in the editor so 
		/// you don't have to build the asset bundles or update the map assets.
		/// </summary>
		Develop,
			
		/// <summary>
		/// The Test Mode. Use this mode to make sure the asset bundles and map 
		/// assets are working before you do a build.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In this mode, you must update the map assets and build the asset bundles 
		/// before playing. You must build the asset bundles for 
		/// <see cref="BuildTarget.StandaloneOSX" /> (even if you want to 
		/// test an another platform) because Unity can't use iOS or Android asset 
		/// bundles in the editor.
		/// </para>
		/// <para>
		/// In this mode, you can save time by only updating and building a subset 
		/// of the asset bundles for the project. Only the content submodules you 
		/// update and build the asset bundles for will work.
		/// </para>
		/// </remarks>
		Test,
			
		/// <summary>
		/// The Build Mode. Use this mode to quickly make builds to test on a device.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In this mode, the build process will NOT automatically update and build 
		/// the asset bundles every time you make a build. You are responsible for 
		/// updating the map assets and building the asset bundles when necessary.
		/// </para>
		/// <para>
		/// In this mode, you can save time by only updating and building a subset 
		/// of the asset bundles for the project. Only the content submodules you 
		/// update and build the asset bundles for will work.
		/// </para>
		/// <para>
		/// The build process in this mode does NOT match the build process in Cloud Build.
		/// </para>
		/// </remarks>
		BuildCustom,
			
		/// <summary>
		/// The Release Mode. Use this mode to make builds for release.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In this mode, the build process will automatically update and build the 
		/// all of the asset bundles for the project every time you make a build. 
		/// This mode is much slower, but guarantees that all of the necessary 
		/// actions happen in the correct order so that the build is complete. 
		/// </para>
		/// <para>
		/// The build process in this mode matches the build process in Cloud Build.
		/// </para>
		/// </remarks>
		BuildComplete
		
	}
		
	public static class WorkflowStyles {
		
		public static GUIStyle actionButton {
			get {
				if (s_actionButton == null) {
					var style = new GUIStyle(GUI.skin.button);
					style.alignment = TextAnchor.MiddleCenter;
					style.padding = new RectOffset(10,10,10,10);
					style.richText = true;
					s_actionButton = style;
				}
				return s_actionButton;
			}
		}
		private static GUIStyle s_actionButton;
		
		public static GUIStyle group {
			get {
				if (s_group == null) {
					var style = new GUIStyle(GUIStyle.none);
					style.margin = new RectOffset(10,10,10,10);
					s_group = style;
				}
				return s_group;
			}
		}
		private static GUIStyle s_group;
		
		public static GUIStyle header {
			get {
				if (s_header == null) {
					var style = new GUIStyle(EditorStyles.label);
					style.fontStyle = FontStyle.Bold;
					style.normal.textColor = Color.white;
					s_header = style;
				}
				return s_header;
			}
		}
		private static GUIStyle s_header;
		
		public static GUIStyle section {
			get {
				if (s_section == null) {
					var style = new GUIStyle(GUIStyle.none);
					style.margin = new RectOffset(10,10,10,10);
					s_section = style;
				}
				return s_section;
			}
		}
		private static GUIStyle s_section;

		public static GUIStyle finePrint {
			get {
				if (s_finePrint == null) {
					var style = new GUIStyle(EditorStyles.miniBoldLabel);
					style.richText = true;
					s_finePrint = style;
				}
				return s_finePrint;
			}
		}
		private static GUIStyle s_finePrint;

	}

	public class TeamCity {
	
		public static void SwitchPlatform() {

			// PlatformUtil.ActivePlatform returns appropriate platform value depending on which define symbols
			// have been enabled, such as SAGO_IOS, SAGO_GOOGLE_PLAY, and etc.
			bool result = PlatformUtilEditor.Activate(PlatformUtil.ActivePlatform);

			if (result) {
				Debug.LogFormat("Platform Switch Has Been Successful: {0}", PlatformUtil.ActivePlatform);
			} else {
				Debug.LogFormat("Platform Switch Failed: {0}", PlatformUtil.ActivePlatform);
			}
		}

	}
	
	public class WorkflowWindow : EditorWindow, IHasCustomMenu {
		
		
		#region Window
		
		[MenuItem("Sago/Workflow")]
		[MenuItem("Window/Sago/Workflow")]
		private static EditorWindow GetWindow() {
			return EditorWindow.GetWindow<WorkflowWindow>();
		}
		
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad() {
			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				Mode = WorkflowMode.BuildComplete;
			#else
				OnWorkflowModeChanged();
			#endif
			#if ENABLE_IOS_ON_DEMAND_RESOURCES
				UnityEditor.iOS.BuildPipeline.collectResources += CollectOnDemandResources;
			#endif
		}
		
		[OnBuildPreprocess]
		public static void OnBuildPreprocess(IBuildProcessor processor) {
			if (Mode == WorkflowMode.BuildComplete) {
				SubmoduleMapEditor.UpdateSubmoduleMap();
				ProjectInfoEditor.UpdateProjectInfo();
				AssetBundleMapEditor.UpdateAssetBundleMap();
				AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap();
				UpdateAssetBundleNames();
				BuildAssetBundles();
				UpdateProject();
			}
			MoveAssetBundlesToStreamingAssets();
		}
		
		[OnBuildPostprocess]
		public static void OnBuildPostprocess(IBuildProcessor processor) {
			RevertMoveAssetBundlesToStreamingAssets();
		}
		
		private static void OnWorkflowModeChanged() {
			switch (Mode) {
				case WorkflowMode.Develop:
					AssetBundleOptions.UseAssetBundlesInEditor = false;
					ResourceMap.Mode = ResourceMapMode.Editor;
					SceneMap.Mode = SceneMapMode.Editor;
					SubmoduleMap.Mode = SubmoduleMapMode.Editor;
				break;
				default:
					AssetBundleOptions.UseAssetBundlesInEditor = true;
					ResourceMap.Mode = ResourceMapMode.Player;
					SceneMap.Mode = SceneMapMode.Player;
					SubmoduleMap.Mode = SubmoduleMapMode.Player;
				break;
			}
		}
		
		public void AddItemsToMenu(GenericMenu menu) {
			menu.AddItem(new GUIContent("Workflow Window User Guide"), false, () => {
				Application.OpenURL("https://github.com/SagoSago/sago-app/tree/sago-world/Editor/Workflow/WorkflowWindow.md");
			});
		}
		
		private void OnDisable() {
			EditorApplication.playModeStateChanged -= OnPlayModeChanged;
		}
		
		private void OnEnable() {
			titleContent = new GUIContent("Workflow");
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}
		
		private void OnGUI() {
			
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
			
			EditorGUILayout.BeginVertical(WorkflowStyles.section);
				EditorGUILayout.LabelField("General", WorkflowStyles.header);
				EditorGUILayout.BeginVertical(WorkflowStyles.group);
					ModeField();
					PlatformField();
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(WorkflowStyles.section);
				EditorGUILayout.LabelField("Asset Bundles", WorkflowStyles.header);
				EditorGUILayout.BeginVertical(WorkflowStyles.group);
					ContentField();
					AssetBundleAdaptorTypeField();
					AssetBundleServerTypeField();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(WorkflowStyles.group);
					UpdateAssetBundlesButton();
					BuildAssetBundlesButton();
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			var storeConfigurationEditor = GetStoreConfigurationEditor();
			if (storeConfigurationEditor) {
				EditorGUILayout.BeginVertical(WorkflowStyles.section);
					EditorGUILayout.LabelField("Store", WorkflowStyles.header);
					EditorGUILayout.BeginVertical(WorkflowStyles.group);
						storeConfigurationEditor.OnInspectorGUI();
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.BeginVertical(WorkflowStyles.section);
				EditorGUILayout.LabelField("Project", WorkflowStyles.header);
				EditorGUILayout.BeginVertical(WorkflowStyles.group);
					PlatformPrefabField();
					PlayerSettingsField();
					DevelopmentField();
					ConnectWithProfilerField();
					AllowDebuggingField();
					#if UNITY_ANDROID
						AndroidBuildActionField();
					#elif UNITY_IOS
						iOSBuildTypeField();
						iOSBuildActionField();
					#elif UNITY_TVOS
						tvOSBuildTypeField();
						tvOSBuildActionField();
					#endif
					VersionField();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(WorkflowStyles.group);
					UpdateProjectButton();
					BuildProjectButton();
					FinePrint();
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			Errors.Clear();
			#if UNITY_ANDROID
				AndroidValidate();
			#elif UNITY_IOS
				iOSValidate();
			#elif UNITY_TVOS
				tvOSValidate();
			#endif
				
			if (Errors.Count != 0) {
				EditorGUILayout.BeginVertical(WorkflowStyles.section);
					EditorGUILayout.LabelField("Errors", WorkflowStyles.header);
					EditorGUILayout.BeginVertical(WorkflowStyles.group);
					foreach (WorkflowError error in Errors) {
						EditorStyles.helpBox.richText = true;
						EditorGUILayout.HelpBox(error.Message, error.MessageType, true);
					}
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.EndScrollView();
			
		}
		
		private void OnPlayModeChanged(PlayModeStateChange playState) {
			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
				switch (Mode) {
					case WorkflowMode.BuildCustom:
					case WorkflowMode.BuildComplete:
						if (EditorUtility.DisplayDialog(
								"Choose Workflow Mode", 
								string.Format("You cannot play in the editor using the {0} workflow mode.", Mode), 
								"Use Develop Mode", "Use Test Mode")) {
							Mode = WorkflowMode.Develop;
						} else {
							Mode = WorkflowMode.Test;
						}
					break;
				}
			}
			EditorUtility.SetDirty(this);
		}
		
		#endregion
		
		
		#region Fields
		
		private Vector2 m_ScrollPosition;
		
		#endregion
		
		
		#region Mode
		
		private const string ModeKey = "SagoAppEditor.Workflow.WorkflowWindow.Mode";
		
		private static WorkflowMode Mode {
			get {
				#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
					return WorkflowMode.BuildComplete;
				#else
					return (WorkflowMode)EditorPrefs.GetInt(ModeKey);
				#endif
			}
			set {
				if (Mode != value) {
					EditorPrefs.SetInt(ModeKey, (int)value);
					OnWorkflowModeChanged();
				}
			}
		}
		
		private static void ModeField() {
			
			WorkflowMode[] values;
			values = (WorkflowMode[])System.Enum.GetValues(typeof(WorkflowMode));
			
			string[] options;
			options = values.Select(value => ObjectNames.NicifyVariableName(value.ToString())).ToArray();
			
			int index;
			index = System.Array.IndexOf(values, Mode);
			index = Mathf.Max(index, 0);
			index = EditorGUILayout.Popup("Mode", index, options);
			index = Mathf.Max(index, 0);
			
			Mode = values[index];
			
		}
		
		#endregion
		
		
		#region Platform
		
		private static void PlatformField() {
			EditorGUI.BeginChangeCheck();
			Platform platform = (Platform)EditorGUILayout.EnumPopup("Platform", PlatformUtil.ActivePlatform);
			if (EditorGUI.EndChangeCheck()) {
				// Delay the call so that Unity doesn't error when we try to change platforms mid OnGUI render
				EditorApplication.delayCall += () => {
					PlatformUtilEditor.Activate(platform);
				};
			}
		}
		
		#endregion
		
		
		#region Content
		
		private static System.Type[] GetContentInfoTypes() {
			return ContentInfoEditor.ContentInfoTypes;
		}
		
		private static ContentInfo[] GetContentInfo() {
			return (
				ContentInfoEditor
				.ContentInfoTypes
				.Select(type => ContentInfoEditor.FindContentInfo(type))
				.ToArray()
			);
		}
		
		private static System.Type[] GetContentInfoTypesWithFlag(bool flag) {
			return (
				GetContentInfoTypes()
				.Where(type => GetContentFlag(type) == flag)
				.ToArray()
			);
		}
		
		private static ContentInfo[] GetContentInfoWithFlag(bool flag) {
			return (
				GetContentInfo()
				.Where(contentInfo => GetContentFlag(contentInfo.GetType()) == flag)
				.ToArray()
			);
		}
		
		private static bool GetContentFlag(System.Type type) {
			if (Mode == WorkflowMode.BuildComplete) {
				return true;
			} else {
				return EditorPrefs.GetBool(GetContentFlagKey(type), true);
			}
		}
		
		private static void SetContentFlag(System.Type type, bool value) {
			EditorPrefs.SetBool(GetContentFlagKey(type), value);
		}
		
		private static string GetContentFlagKey(System.Type type) {
			return string.Format("SagoAppEditor.Workflow.WorkflowWindow.ContentFlag:{0}", type);
		}
		
		private static void ContentField() {
			
			System.Type[] types;
			types = ContentInfoEditor.ContentInfoTypes;
			if (types.Length == 0) {
				return;
			}
			
			string[] options;
			options = (
				types
				.Select(type => SubmoduleMapEditorAdaptor.GetSubmoduleName(type))
				.Select(option => ObjectNames.NicifyVariableName(option))
				.ToArray()
			);
			
			if (Mode == WorkflowMode.BuildComplete) {
				
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.MaskField("Mask", -1, options);
				EditorGUI.EndDisabledGroup();
				
			} else {
				
				BitArray bitArray = new BitArray(32);
				for (int index = 0; index < types.Length; index++) {
					bitArray.Set(index, GetContentFlag(types[index]));
				}
				
				int[] bitMask = new int[1];
				bitArray.CopyTo(bitMask, 0);
				
				EditorGUI.BeginChangeCheck();
				
				bitMask[0] = EditorGUILayout.MaskField("Mask", bitMask[0], options);
				bitArray = new BitArray(bitMask);
				
				if (EditorGUI.EndChangeCheck()) {
					for (int index = 0; index < types.Length; index++) {
						SetContentFlag(types[index], bitArray.Get(index));
					}
				}
				
			}
			
		}
		
		#endregion
		
		
		#region Build
		
		private const string AllowDebuggingKey = "SagoAppEditor.Workflow.WorkflowWindow.AllowDebugging";
		
		private static bool AllowDebugging {
			get { return EditorPrefs.GetBool(AllowDebuggingKey, false); }
			set { EditorPrefs.SetBool(AllowDebuggingKey, value); }
		}
		
		private const string ConnectWithProfilerKey = "SagoAppEditor.Workflow.WorkflowWindow.ConnectWithProfiler";
		
		private static bool ConnectWithProfiler {
			get { return EditorPrefs.GetBool(ConnectWithProfilerKey, false); }
			set { EditorPrefs.SetBool(ConnectWithProfilerKey, value); }
		}
		
		private const string DevelopmentKey = "SagoAppEditor.Workflow.WorkflowWindow.Development";
		
		private static bool Development {
			get { return EditorPrefs.GetBool(DevelopmentKey, false); }
			set { EditorPrefs.SetBool(DevelopmentKey, value); }
		}
		
		private static void AllowDebuggingField() {
			if ((Mode == WorkflowMode.BuildCustom || Mode == WorkflowMode.BuildComplete) && Development) {
				AllowDebugging = EditorGUILayout.Toggle("Allow Debugging", AllowDebugging);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Toggle("Allow Debugging", false);
				EditorGUI.EndDisabledGroup();
			}
		}
		
		private static void ConnectWithProfilerField() {
			if ((Mode == WorkflowMode.BuildCustom || Mode == WorkflowMode.BuildComplete) && Development) {
				ConnectWithProfiler = EditorGUILayout.Toggle("Connect With Profiler", ConnectWithProfiler);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Toggle("Connect With Profiler", false);
				EditorGUI.EndDisabledGroup();
			}
		}
		
		private static void DevelopmentField() {
			if (Mode == WorkflowMode.BuildCustom || Mode == WorkflowMode.BuildComplete) {
				Development = EditorGUILayout.Toggle("Development Build", Development);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Toggle("Development Build", false);
				EditorGUI.EndDisabledGroup();
			}
		}
		
		private static void PlatformPrefabField() {
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Platform Settings");
				if (GUILayout.Button("Open", EditorStyles.miniButton)) {
					Selection.activeObject = PlatformSettingsMultiplexer.Instance.GetPrefab().gameObject;
				}
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		private static void PlayerSettingsField() {
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Player Settings");
				if (GUILayout.Button("Open", EditorStyles.miniButton)) {
					EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
				}
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		private static void VersionField() {
			
			var productInfo = PlatformUtil.GetSettings<ProductInfo>();
			
			if (productInfo) {
				EditorGUILayout.BeginHorizontal(GUIStyle.none);
					
					EditorGUILayout.LabelField(
						"Version", 
						string.Format("{0}.{1}", productInfo.Version, productInfo.Build)
					);
					
					GUILayout.FlexibleSpace();
					
					EditorGUI.BeginChangeCheck();
					VersionService.ServerType = (VersionServiceServerType)EditorGUILayout.EnumPopup(VersionService.ServerType, GUILayout.Width(100));
					if (EditorGUI.EndChangeCheck()) {
						VersionService.Fetch(productInfo);
					}
					
					if (GUILayout.Button("Fetch", EditorStyles.miniButton)) {
						if (!VersionService.Fetch(productInfo)) {
							Debug.LogError("Could not get version!");
						}
					}
					
					if (GUILayout.Button("Bump", EditorStyles.miniButton)) {
						if (!VersionService.Bump(productInfo)) {
							Debug.LogError("Could not bump version!");
						}
					}
					
				EditorGUILayout.EndHorizontal();
			} else {
				EditorGUILayout.LabelField("Version", "Unknown");
			}
			
		}
		
		
		#if UNITY_ANDROID
		
		private const string AndroidBuildActionKey = "SagoAppEditor.Workflow.WorkflowWindow.BuildAction";
		
		private static AndroidBuildAction AndroidBuildAction {
			get {
				#if TEAMCITY_BUILD
					return AndroidBuildAction.Build;
				#else
					return (AndroidBuildAction)EditorPrefs.GetInt(AndroidBuildActionKey, (int)AndroidBuildAction.BuildAndRun);
				#endif
			}
			set { EditorPrefs.SetInt(AndroidBuildActionKey, (int)value); }
		}
		
		private static void AndroidBuildActionField() {
			
			bool isDisabled;
			switch (Mode) {
				case WorkflowMode.BuildCustom:
				case WorkflowMode.BuildComplete:
					isDisabled = false;
				break;
				default:
					isDisabled = true;
				break;
			}
			
			EditorGUI.BeginDisabledGroup(isDisabled);
			AndroidBuildAction = (AndroidBuildAction)EditorGUILayout.EnumPopup("Build Action", AndroidBuildAction);
			EditorGUI.EndDisabledGroup();
			
		}

		private static void AndroidValidate() {
			
		}
		
		private static void AndroidBuild() {
			
			BuildOptions buildOptions;
			buildOptions = BuildOptions.None;
			
			if (AndroidBuildAction == AndroidBuildAction.BuildAndRun) {
				buildOptions = buildOptions | BuildOptions.AutoRunPlayer;
			}
			if (Development) {
				buildOptions = buildOptions | BuildOptions.Development;
			}
			if (Development && ConnectWithProfiler) {
				buildOptions = buildOptions | BuildOptions.ConnectWithProfiler;
			}
			if (Development && AllowDebugging) {
				buildOptions = buildOptions | BuildOptions.AllowDebugging;
			}
			
			string processorName;
			processorName = string.Format(
				"SagoBuildEditor.{0}.{0}BuildProcessor", 
				PlatformUtil.ActivePlatform
			);
			
			System.Type processorType;
			processorType = System.Type.GetType(processorName);
			
			if (processorType == null) {
				throw new System.InvalidOperationException(string.Format(
					"Could not find build processor for platform: {0}", 
					PlatformUtil.ActivePlatform
				));
			}
			
			string runnerName;
			runnerName = string.Format(
				"SagoBuildEditor.{0}.{0}BuildRunner", 
				PlatformUtil.ActivePlatform.ToString()
			);
			
			System.Type runnerType;
			runnerType = System.Type.GetType(runnerName);
			
			if (runnerType == null) {
				throw new System.InvalidOperationException(string.Format(
					"Could not find build runner for platform: {0}", 
					PlatformUtil.ActivePlatform
				));
			}
			
			AndroidBuildRunner runner;
			runner = (AndroidBuildRunner)System.Activator.CreateInstance(runnerType);
			runner.BuildOptions = buildOptions;
			runner.OnPreprocess = (BuildManifestObject buildManifest) => {
				processorType
					.GetMethod("OnPreprocess", BindingFlags.Public | BindingFlags.Static)
					.Invoke(null, new object[]{ buildManifest });
			};
			runner.OnPostprocess = BuildProcessor.OnPostprocess;
			runner.Run();
			
		}
		
		#endif
		
		
		#if UNITY_IOS
		
		private const string iOSBuildActionKey = "SagoAppEditor.Workflow.WorkflowWindow.iOSBuildAction";
		
		private static iOSBuildAction iOSBuildAction {

			get {
				#if TEAMCITY_BUILD
					return iOSBuildAction.Build;
				#else
					return (iOSBuildAction)EditorPrefs.GetInt(iOSBuildActionKey, (int)iOSBuildAction.BuildAndRun);
				#endif
			}

			set { EditorPrefs.SetInt(iOSBuildActionKey, (int)value); }
		}
		
		private static void iOSBuildActionField() {
			
			bool isDisabled;
			switch (Mode) {
				case WorkflowMode.BuildCustom:
				case WorkflowMode.BuildComplete:
					isDisabled = false;
				break;
				default:
					isDisabled = true;
				break;
			}
			
			EditorGUI.BeginDisabledGroup(isDisabled);
			iOSBuildAction = (iOSBuildAction)EditorGUILayout.EnumPopup("Build Action", iOSBuildAction);
			EditorGUI.EndDisabledGroup();
			
		}
		
		private const string iOSBuildTypeKey = "SagoAppEditor.Workflow.WorkflowWindow.iOSBuildType";
		
		public static iOSBuildType iOSBuildType {

			get {
				#if TEAMCITY_BUILD
					#if IS_ADHOC
						return iOSBuildType.AdHoc;
					#else
						return iOSBuildType.AppStore;
					#endif
				#else
					return (iOSBuildType)EditorPrefs.GetInt(iOSBuildTypeKey, (int)iOSBuildType.Device);
				#endif
			}

			set { EditorPrefs.SetInt(iOSBuildTypeKey, (int)value); }
		}
		
		private static void iOSBuildTypeField() {
			
			bool isDisabled;
			switch (Mode) {
				case WorkflowMode.BuildCustom:
				case WorkflowMode.BuildComplete:
					isDisabled = false;
				break;
				default:
					isDisabled = true;
				break;
			}
			
			EditorGUI.BeginDisabledGroup(isDisabled);
			iOSBuildType = (iOSBuildType)EditorGUILayout.EnumPopup("Build Type", iOSBuildType);
			EditorGUI.EndDisabledGroup();
			
		}
		
		private static void iOSValidate() {
			if (Mode == WorkflowMode.BuildCustom || Mode == WorkflowMode.BuildComplete) {
				if (iOSBuildType == iOSBuildType.Simulator && iOSBuildAction == iOSBuildAction.BuildAndArchive) {
					var error = new WorkflowError();
					error.Message = "Cannot use the build action <b><color=white>Build And Archive</color></b> with the build type <b><color=white>Simulator</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
				if (iOSBuildType == iOSBuildType.AppStore && Development) {
					var error = new WorkflowError();
					error.Message = "Cannot use <b><color=white>Development</color></b> with the build type <b><color=white>App Store</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
				if (iOSBuildType == iOSBuildType.AdHoc && AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.OnDemandResources) {
					var error = new WorkflowError();
					error.Message = "Cannot use the remote adaptor type <b><color=white>On Demand Resources</color></b> with the build type <b><color=white>Ad Hoc</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}

				// if (iOSBuildType == iOSBuildType.AppStore && ContentInfoEditor.ContentInfoTypes.Length > 1 && AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.StreamingAssets) {
				// 	var error = new WorkflowError();
				// 	error.Message = "Cannot use the remote adaptor type <b><color=white>Streaming Assets</color></b> with the build type <b><color=white>App Store</color></b> when the project has multiple content submodules.";
				// 	error.MessageType = MessageType.Error;
				// 	Errors.Add(error);
				// }
			}
		}
		
		private static void iOSBuild() {
			
			BuildOptions buildOptions;
			buildOptions = BuildOptions.None | BuildOptions.SymlinkLibraries;
			
			if (Development) {
				buildOptions = buildOptions | BuildOptions.Development;
			}
			if (Development && ConnectWithProfiler) {
				buildOptions = buildOptions | BuildOptions.ConnectWithProfiler;
			}
			if (Development && AllowDebugging) {
				buildOptions = buildOptions | BuildOptions.AllowDebugging;
			}
			
			iOSBuildRunner runner;
			runner = new iOSBuildRunner();
			runner.BuildOptions = buildOptions;
			runner.BuildAction = iOSBuildAction;
			runner.BuildSdkVersion = iOSBuildType == iOSBuildType.Simulator ? iOSSdkVersion.SimulatorSDK : iOSSdkVersion.DeviceSDK;
			
			switch (iOSBuildType) {
				case iOSBuildType.AdHoc:
					runner.OnPreprocess = iOSBuildProcessor.OnPreprocessAdHoc;
				break;
				case iOSBuildType.AppStore:
					runner.OnPreprocess = iOSBuildProcessor.OnPreprocessAppStore;
				break;
				case iOSBuildType.Device:
					runner.OnPreprocess = iOSBuildProcessor.OnPreprocessDevice;
				break;
				case iOSBuildType.Simulator:
					runner.OnPreprocess = iOSBuildProcessor.OnPreprocessSimulator;
				break;
			}
			
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
			
		}
		
		#endif
		
		
		#if UNITY_TVOS
		
		private const string tvOSBuildActionKey = "SagoAppEditor.Workflow.WorkflowWindow.tvOSBuildAction";
		
		private static tvOSBuildAction tvOSBuildAction {
			get { return (tvOSBuildAction)EditorPrefs.GetInt(tvOSBuildActionKey, (int)tvOSBuildAction.BuildAndRun); }
			set { EditorPrefs.SetInt(tvOSBuildActionKey, (int)value); }
		}
		
		private static void tvOSBuildActionField() {
			// EditorGUI.BeginDisabledGroup(Mode != WorkflowMode.Build);
			tvOSBuildAction = (tvOSBuildAction)EditorGUILayout.EnumPopup("Build Action", tvOSBuildAction);
			// EditorGUI.EndDisabledGroup();
		}
		
		private const string tvOSBuildTypeKey = "SagoAppEditor.Workflow.WorkflowWindow.tvOSBuildType";
		
		private static tvOSBuildType tvOSBuildType {
			get { return (tvOSBuildType)EditorPrefs.GetInt(tvOSBuildTypeKey, (int)tvOSBuildType.Device); }
			set { EditorPrefs.SetInt(tvOSBuildTypeKey, (int)value); }
		}
		
		private static void tvOSBuildTypeField() {
			// EditorGUI.BeginDisabledGroup(Mode != WorkflowMode.Build);
			tvOSBuildType = (tvOSBuildType)EditorGUILayout.EnumPopup("Build Type", tvOSBuildType);
			// EditorGUI.EndDisabledGroup();
		}
		
		private static void tvOSValidate() {
			if (Mode == WorkflowMode.BuildCustom || Mode == WorkflowMode.BuildComplete) {
				if (tvOSBuildType == tvOSBuildType.Simulator && tvOSBuildAction == tvOSBuildAction.BuildAndArchive) {
					var error = new WorkflowError();
					error.Message = "Cannot use the build action <b><color=white>Build And Archive</color></b> with the build type <b><color=white>Simulator</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
				if (tvOSBuildType == tvOSBuildType.AppStore && Development) {
					var error = new WorkflowError();
					error.Message = "Cannot use <b><color=white>Development</color></b> with the build type <b><color=white>App Store</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
				if (tvOSBuildType == tvOSBuildType.AdHoc && AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.OnDemandResources) {
					var error = new WorkflowError();
					error.Message = "Cannot use the remote adaptor type <b><color=white>On Demand Resources</color></b> with the build type <b><color=white>Ad Hoc</color></b>.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
				if (tvOSBuildType == tvOSBuildType.AppStore && ContentInfoEditor.ContentInfoTypes.Length > 1 && AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.StreamingAssets) {
					var error = new WorkflowError();
					error.Message = "Cannot use the remote adaptor type <b><color=white>Streaming Assets</color></b> with the build type <b><color=white>App Store</color></b> when the project has multiple content submodules.";
					error.MessageType = MessageType.Error;
					Errors.Add(error);
				}
			}
		}
		
		private static void tvOSBuild() {
			
			BuildOptions buildOptions;
			buildOptions = BuildOptions.None | BuildOptions.SymlinkLibraries;
			
			if (Development) {
				buildOptions = buildOptions | BuildOptions.Development;
			}
			if (Development && ConnectWithProfiler) {
				buildOptions = buildOptions | BuildOptions.ConnectWithProfiler;
			}
			if (Development && AllowDebugging) {
				buildOptions = buildOptions | BuildOptions.AllowDebugging;
			}
			
			tvOSBuildRunner runner;
			runner = new tvOSBuildRunner();
			runner.BuildOptions = buildOptions;
			runner.BuildAction = tvOSBuildAction;
			runner.BuildSdkVersion = tvOSBuildType == tvOSBuildType.Simulator ? iOSSdkVersion.SimulatorSDK : iOSSdkVersion.DeviceSDK;
			
			switch (tvOSBuildType) {
				case tvOSBuildType.AdHoc:
					runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessAdHoc;
				break;
				case tvOSBuildType.AppStore:
					runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessAppStore;
				break;
				case tvOSBuildType.Device:
					runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessDevice;
				break;
				case tvOSBuildType.Simulator:
					runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessSimulator;
				break;
			}
			
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		#endif
		
		#endregion
		
		
		#region Asset Bundles
		
		private static void MoveAssetBundlesToStreamingAssets() {
			
			string srcRoot;
			srcRoot = "Assets/Project/AssetBundles/StreamingAssets";
			
			if (Directory.Exists(srcRoot)) {
				
				string dstRoot;
				dstRoot = "Assets/StreamingAssets";
				
				if (!Directory.Exists(dstRoot)) {
					AssetDatabase.CreateFolder(
						Path.GetDirectoryName(dstRoot), 
						Path.GetFileNameWithoutExtension(dstRoot)
					);
				}
				
				if (Directory.Exists(dstRoot)) {
					foreach (string assetBundleName in AssetDatabase.GetAllAssetBundleNames()) {
						if (GetAssetBundleAdaptorType(assetBundleName) == AssetBundleAdaptorType.StreamingAssets) {
							
							string srcPath;
							srcPath = Path.Combine(srcRoot, assetBundleName);
							
							string dstPath;
							dstPath = Path.Combine(dstRoot, assetBundleName);
							
							AssetDatabase.DeleteAsset(dstPath);
							if (string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(srcPath, dstPath))) {
								AssetDatabase.MoveAsset(srcPath, dstPath);
							}
							
						}
					}
				}
				
			}
			
		}
		
		private static void RevertMoveAssetBundlesToStreamingAssets() {
			
			string srcRoot;
			srcRoot = "Assets/StreamingAssets";
			
			if (Directory.Exists(srcRoot)) {
			
				string dstRoot;
				dstRoot = "Assets/Project/AssetBundles/StreamingAssets";
				
				if (Directory.Exists(dstRoot)) {
					foreach (string assetBundleName in AssetDatabase.GetAllAssetBundleNames()) {
						if (GetAssetBundleAdaptorType(assetBundleName) == AssetBundleAdaptorType.StreamingAssets) {
							
							string srcPath;
							srcPath = Path.Combine(srcRoot, assetBundleName);
							
							string dstPath;
							dstPath = Path.Combine(dstRoot, assetBundleName);
							
							AssetDatabase.DeleteAsset(dstPath);
							if (string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(srcPath, dstPath))) {
								AssetDatabase.MoveAsset(srcPath, dstPath);
							}
							
						}
					}
				}
				
				bool empty = true;
				foreach (string path in AssetDatabase.GetAllAssetPaths()) {
					if (path.StartsWith(srcRoot + Path.DirectorySeparatorChar)) {
						empty = false;
						break;
					}
				}
				
				if (empty) {
					AssetDatabase.DeleteAsset(srcRoot);
				}
				
			}
			
		}
		
		private static AssetBundleAdaptorType GetAssetBundleAdaptorType(string assetBundleName) {

			AssetBundleAdaptorMap map = AssetBundleAdaptorMapEditor.FindAssetBundleAdaptorMap();

			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				// In CloudBuild, this asset seems to be removed during the build, so
				// we'll reconstruct it here.  This isn't the expected behaviour though
				// so we're only calling UpdateAssetBundleAdaptorMap() in this case.
				if (!map) {
					Debug.Log("GetAssetBundleAdaptorType was null, updating.");
					map = AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap();
				}
			#endif

			return map.GetAdaptorType(assetBundleName);
		}
		
		private static string GetAssetBundlePath(string assetBundleName) {
			return Path.Combine(Path.Combine("Assets/Project/AssetBundles", GetAssetBundleAdaptorType(assetBundleName).ToString()), assetBundleName);
		}
		
		private static void AssetBundleAdaptorTypeField() {
			
			EditorGUI.BeginDisabledGroup(Mode == WorkflowMode.Develop);
			
			/// WORKAROUND:  If you start up Unity with the Workflow window open,
			/// and it calls this during its first draw, Unity 2017.3 crashes hard
			/// if there is a progress bar.  So for the purpose of automatic changes
			/// call UpdateAssetBundleAdaptorMap(false) to disable the progress bar
			/// as a workaround until that is fixed.
			/// See Jira [SW-360].
			if (PlatformUtil.ActivePlatform == Platform.iOS) {
				if (AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.AssetBundleServer) {
					AssetBundleAdaptorMapEditor.SetRemoteAdaptorType(AssetBundleAdaptorType.OnDemandResources);
					AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap(false);
				}
			} else {
				if (AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.OnDemandResources) {
					AssetBundleAdaptorMapEditor.SetRemoteAdaptorType(AssetBundleAdaptorType.AssetBundleServer);
					AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap(false);
				}
			}
			
			AssetBundleAdaptorType[] values = null;
			if (PlatformUtil.ActivePlatform == Platform.iOS) {
				values = new AssetBundleAdaptorType[] {
					AssetBundleAdaptorType.Unknown,
					AssetBundleAdaptorType.StreamingAssets,
					AssetBundleAdaptorType.OnDemandResources
				};
			} else {
				values = new AssetBundleAdaptorType[] {
					AssetBundleAdaptorType.Unknown,
					AssetBundleAdaptorType.StreamingAssets,
					AssetBundleAdaptorType.AssetBundleServer
				};
			}
			
			int index;
			index = System.Array.IndexOf(values, AssetBundleAdaptorMapEditor.GetRemoteAdaptorType());
			
			string[] options;
			options = values.Select(value => value.ToString()).ToArray();
			
			EditorGUI.BeginChangeCheck();
			AssetBundleAdaptorType adaptorType;
			adaptorType = values[EditorGUILayout.Popup("Remote Adaptor Type", index, options)];
			if (EditorGUI.EndChangeCheck()) {
				AssetBundleAdaptorMapEditor.SetRemoteAdaptorType(adaptorType);
				AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap();
			}
			
			EditorGUI.EndDisabledGroup();
			
		}
		
		private static void AssetBundleServerTypeField() {
			if (AssetBundleAdaptorMapEditor.GetRemoteAdaptorType() == AssetBundleAdaptorType.AssetBundleServer) {
				EditorGUI.BeginDisabledGroup(Mode == WorkflowMode.Develop);
				EditorGUI.BeginChangeCheck();
				AssetBundleServerType serverType;
				serverType = (AssetBundleServerType)EditorGUILayout.EnumPopup("Server Type", AssetBundleAdaptorMapEditor.GetServerType());
				if (EditorGUI.EndChangeCheck()) {
					AssetBundleAdaptorMapEditor.SetServerType(serverType);
					AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap();
				}
				EditorGUI.EndDisabledGroup();
			}
		}
		
		#if ENABLE_IOS_ON_DEMAND_RESOURCES
		private static Resource[] CollectOnDemandResources() {
			List<Resource> list = new List<Resource>();
			foreach (string assetBundleName in AssetDatabase.GetAllAssetBundleNames()) {
				if (GetAssetBundleAdaptorType(assetBundleName) == AssetBundleAdaptorType.OnDemandResources) {
					list.Add(new Resource(assetBundleName, GetAssetBundlePath(assetBundleName)).AddOnDemandResourceTags(PlatformUtil.AppendVersion(assetBundleName)));
				}
			}
			return list.ToArray();
		}
		#endif
		
		#endregion
		
		
		#region Store
		
		private Editor GetStoreConfigurationEditor() {
			
			System.Type editorType;
			editorType = System.Type.GetType("World.Store.StoreConfigurationEditor");
			
			System.Type type;
			type = (
				System
				.AppDomain
				.CurrentDomain
				.GetAssemblies()
				.Select(a => a.GetType("World.Store.StoreConfiguration"))
				.Where(t => t != null)
				.FirstOrDefault()
			);
			
			if (type != null && editorType != null) {
				var flags = BindingFlags.Static | BindingFlags.GetProperty;
				var obj = type.GetProperty("Instance").GetValue(null, flags, null, null, null) as UnityEngine.Object;
				return Editor.CreateEditor(obj, editorType);
			}
			
			return null;
			
		}
		
		#endregion
		
		
		#region Errors
		
		private static List<WorkflowError> _Errors;
		
		private static List<WorkflowError> Errors {
			get { return _Errors = _Errors ?? new List<WorkflowError>(); }
		}
		
		#endregion
		
		
		#region Actions
		
		private static bool ConfirmUpdateAssetBundleNames() {
			
			ContentInfo[] contentInfoArray;
			contentInfoArray = GetContentInfoWithFlag(true);
			
			if (contentInfoArray.Length == 0) {
				
				// TODO: need a better message
				
				string title = "Apply Asset Bundle Names";
				string message = "TODO: Nothing to apply. Please choose at least one...";
				string ok = "Ok";
				
				return EditorUtility.DisplayDialog(title, message, ok) && false;
				
			} else {
				
				string title = "Apply Asset Bundle Names";
				string message = "Are you sure you want to apply the following asset bundle names?";
				string ok = "Apply Asset Bundle Names";
				string cancel = "Cancel";
				
				message += "\n\n";
				foreach (ContentInfo contentInfo in contentInfoArray) {
					message += string.Format("{0}\n", contentInfo.ResourceAssetBundleName);
					message += string.Format("{0}\n", contentInfo.SceneAssetBundleName);
				}
				
				return EditorUtility.DisplayDialog(title, message, ok, cancel);
				
			}
			
		}
		
		private static void UpdateAssetBundleNames() {
			ProjectInfoEditor.UpdateProjectInfo();
			foreach (ContentInfo contentInfo in GetContentInfoWithFlag(true)) {
				ContentInfoEditor.ApplyAssetBundleNames(contentInfo.GetType());
			}
		}
		
		private static void UpdateAssetBundlesButton() {
			
			bool isDisabled;
			isDisabled = (
				Mode == WorkflowMode.BuildComplete || 
				Errors.Count != 0
			);
			
			bool isVisible;
			isVisible = true;
			
			if (isVisible) {
				EditorGUI.BeginDisabledGroup(isDisabled);
				if (GUILayout.Button("Update Asset Bundles", WorkflowStyles.actionButton)) {
					EditorApplication.delayCall +=() => {
						if (ConfirmUpdateAssetBundleNames()) {
							UpdateAssetBundleNames();
						}
					};
				}
				EditorGUI.EndDisabledGroup();
			}
		}
		
		
		private static bool ConfirmBuildAssetBundles() {
			
			BuildTarget buildTarget;
			switch (Mode) {
				case WorkflowMode.BuildCustom:
				case WorkflowMode.BuildComplete:
					buildTarget = PlatformUtil.ActivePlatform.ToBuildTarget();
				break;
				default:
					buildTarget = Platform.Unknown.ToBuildTarget();
				break;
			}
			
			ContentInfo[] contentInfoArray;
			contentInfoArray = GetContentInfoWithFlag(true);
			
			if (contentInfoArray.Length == 0) {
				
				// TODO: need a better message
				
				string title = "Build Asset Bundles";
				string message = "Nothing to build. Please choose at least one asset bundle to build.";
				string ok = "Ok";
				
				return EditorUtility.DisplayDialog(title, message, ok) && false;
				
			} else {
				
				string title = "Build Asset Bundles";
				string message = string.Format("Are you sure you want to build the following asset bundles for {0}?", ObjectNames.NicifyVariableName(buildTarget.ToString()));
				string ok = "Build Asset Bundles";
				string cancel = "Cancel";
				
				message += "\n\n";
				foreach (ContentInfo contentInfo in contentInfoArray) {
					message += string.Format("{0}\n", contentInfo.ResourceAssetBundleName);
					message += string.Format("{0}\n", contentInfo.SceneAssetBundleName);
				}
				
				return EditorUtility.DisplayDialog(title, message, ok, cancel);
				
			}
			
		}

		private static string[] GetAllAssetPaths() {
			return (
				AssetDatabase.GetAllAssetPaths()
					.Where(p => !string.IsNullOrEmpty(p) && AssetImporter.GetAtPath(p) != null)
					.ToArray()
			);
		}
		
		private static void BuildAssetBundles() {
			
			Platform activePlatform = PlatformUtil.ActivePlatform;
			Platform targetPlatform = PlatformUtil.ActivePlatform;
			
			switch (Mode) {
				case WorkflowMode.Develop:
				case WorkflowMode.Test:
					targetPlatform = Platform.Unknown;
				break;
			}
			
			if (activePlatform != targetPlatform) {
				PlatformUtilEditor.Activate(targetPlatform);
			}
			
			BuildTarget buildTarget;
			buildTarget = targetPlatform.ToBuildTarget();
			
			AssetDatabase.StartAssetEditing();
			string path = "Assets/Project/AssetBundles";
			FileUtil.DeleteFileOrDirectory(path);
			Directory.CreateDirectory(path);
			AssetDatabase.StopAssetEditing();
			
			foreach (ContentInfo contentInfo in GetContentInfoWithFlag(true)) {
				
				string[] assetPaths = GetAllAssetPaths();
				
				AssetBundleBuild[] assetBundleBuilds;
				assetBundleBuilds = new AssetBundleBuild[2];
				
				assetBundleBuilds[0].assetBundleName = contentInfo.ResourceAssetBundleName;
				assetBundleBuilds[0].assetNames = (
					assetPaths.Where(p => AssetImporter.GetAtPath(p).assetBundleName.Equals(contentInfo.ResourceAssetBundleName))
						.ToArray()
				);
				
				assetBundleBuilds[1].assetBundleName = contentInfo.SceneAssetBundleName;
				assetBundleBuilds[1].assetNames = (
					assetPaths.Where(p => AssetImporter.GetAtPath(p).assetBundleName.Equals(contentInfo.SceneAssetBundleName))
						.ToArray()
				);
				
				UnityEditor.BuildPipeline.BuildAssetBundles(
					path,
					assetBundleBuilds,
					BuildAssetBundleOptions.UncompressedAssetBundle,
					buildTarget
				);
			}
			AssetDatabase.Refresh();
			
			AssetDatabase.StartAssetEditing();
			foreach (string assetBundleName in AssetDatabase.GetAllAssetBundleNames()) {
				
				string srcPath;
				srcPath = Path.Combine(path, assetBundleName);
				
				string dstPath;
				dstPath = GetAssetBundlePath(assetBundleName);
				
				if (File.Exists(srcPath)) {
					Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
					FileUtil.MoveFileOrDirectory(srcPath, dstPath);
				}
				
			}
			
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
			if (activePlatform != targetPlatform) {
				PlatformUtilEditor.Activate(activePlatform);
			}
			
		}
		
		private static void BuildAssetBundlesButton() {
			
			bool isDisabled;
			isDisabled = (
				Mode == WorkflowMode.Develop || 
				Mode == WorkflowMode.BuildComplete || 
				Errors.Count != 0
			);
			
			bool isVisible;
			isVisible = true;
			
			if (isVisible) {
				EditorGUI.BeginDisabledGroup(isDisabled);
				if (GUILayout.Button("Build Asset Bundles", WorkflowStyles.actionButton)) {
					EditorApplication.delayCall += () => {
						if (ConfirmBuildAssetBundles()) {
							BuildAssetBundles();
						}
					};
				}
				EditorGUI.EndDisabledGroup();
			}
		}
		
		
		private static bool ConfirmUpdateProject() {
			return true;
		}
		
		private static void UpdateProject() {

			SubmoduleMapEditor.UpdateSubmoduleMap();
			
			ProjectInfoEditor.UpdateProjectInfo();
			GraphicsSettingsEditor.UpdateProjectSettings();
			TagManagerEditor.UpdateProjectSettings();
			
			ResourceMapEditor.UpdateResourceMap();
			SceneMapEditor.UpdateSceneMap();
			
			AssetBundleMapEditor.UpdateAssetBundleMap();
			AssetBundleAdaptorMapEditor.UpdateAssetBundleAdaptorMap();
			
		}
		
		private static void UpdateProjectButton() {
			
			bool isDisabled;
			isDisabled = Mode == WorkflowMode.BuildComplete || Errors.Count != 0;
			
			bool isVisible;
			isVisible = true;
			
			if (isVisible) {
				EditorGUI.BeginDisabledGroup(isDisabled);
				if (GUILayout.Button("Update Project", WorkflowStyles.actionButton)) {
					EditorApplication.delayCall +=() => {
						if (ConfirmUpdateProject()) {
							UpdateProject();
						}
					};
				}
				EditorGUI.EndDisabledGroup();
			}
		}
		
		
		private static bool ConfirmBuildProject() {
			
			string title = "Confirm Build Project";
			string ok = "Build Project";
			string cancel = "Cancel";
			
			switch (Mode) {
				case WorkflowMode.BuildCustom: {
					// TODO: write a more detailed message
					string message = string.Format(
						"Are you sure you want to build the project in {0} mode?",
						ObjectNames.NicifyVariableName(Mode.ToString())
					);
					return EditorUtility.DisplayDialog(title, message, ok, cancel);
				}
				case WorkflowMode.BuildComplete: {
					// TODO: write a more detailed message
					string message = string.Format(
						"Are you sure you want to build the project in {0} mode?",
						ObjectNames.NicifyVariableName(Mode.ToString())
					);
					return EditorUtility.DisplayDialog(title, message, ok, cancel);
				}
				default: {
					return false;
				}
			}
			
		}
		
		private static void BuildProject() {
			
			#if UNITY_ANDROID
				if (System.Array.IndexOf(PlatformUtilEditor.AndroidPlatforms, PlatformUtil.ActivePlatform) != -1) {
					AndroidBuild();
				}
			#endif
			
			#if UNITY_IOS
				if (PlatformUtil.ActivePlatform == Platform.iOS) {
					iOSBuild();
				}
			#endif
				
			#if UNITY_TVOS
				if (PlatformUtil.ActivePlatform == Platform.tvOS) {
					tvOSBuild();
				}
			#endif
			
		}
		
		private static void BuildProjectButton() {
			
			bool isDisabled;
			isDisabled = (
				Mode == WorkflowMode.Develop || 
				Mode == WorkflowMode.Test || 
				PlatformUtil.ActivePlatform == Platform.Unknown || 
				Errors.Count != 0
			);
			
			bool isVisible;
			isVisible = true;
			
			if (isVisible) {
				EditorGUI.BeginDisabledGroup(isDisabled);
				if (GUILayout.Button("Build Project", WorkflowStyles.actionButton)) {
					EditorApplication.delayCall += () => {
						if (ConfirmBuildProject()) {
							BuildProject();
						}
					};
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		private static void FinePrint() {
			
			#if SAGO_DEBUG
			EditorGUILayout.LabelField("* SAGO_DEBUG is <color='green'>ON</color>", WorkflowStyles.finePrint);
			#else
			EditorGUILayout.LabelField("* SAGO_DEBUG is <color='red'>OFF</color>", WorkflowStyles.finePrint);
			#endif

		}
		
		#endregion
		
		
	}
	
}
