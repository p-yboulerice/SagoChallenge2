#if UNITY_IOS || UNITY_TVOS

using SagoBuildEditor.Core;
using SagoBuildEditor.iOS;
using SagoUtilsEditor;
using PlistCS;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.iOS.Xcode;

public class DemoEditor : MonoBehaviour {
	
	
	[MenuItem("Sago/Demo/iOS/Turn On", false, 1000)]
	private static void iOSDemoOn() {
		DefineSymbolUtil.AddDefineSymbol(BuildTargetGroup.iOS, "SAGO_IOS_DEMO");
		DefineSymbolUtil.AddDefineSymbol(BuildTargetGroup.iOS, "SAGO_DISABLE_SAGOBIZ");
	}
	
	[MenuItem("Sago/Demo/iOS/Turn On", true, 1000)]
	private static bool iOSDemoOnIsEnabled() {
		#if SAGO_IOS_DEMO
			return false;
		#else
			return true;
		#endif
	}
	
	[MenuItem("Sago/Demo/iOS/Turn Off", false, 1000)]
	private static void iOSDemoOff() {
		DefineSymbolUtil.RemoveDefineSymbol(BuildTargetGroup.iOS, "SAGO_IOS_DEMO");
		DefineSymbolUtil.RemoveDefineSymbol(BuildTargetGroup.iOS, "SAGO_DISABLE_SAGOBIZ");
	}
	
	[MenuItem("Sago/Demo/iOS/Turn Off", true, 1000)]
	private static bool iOSDemoOffIsEnabled() {
		#if SAGO_IOS_DEMO
			return true;
		#else
			return false;
		#endif
	}
	
	
	[MenuItem("Sago/Demo/tvOS/Turn On", false, 2000)]
	private static void tvOSDemoOn() {
		#if UNITY_TVOS
			DefineSymbolUtil.AddDefineSymbol(BuildTargetGroup.tvOS, "SAGO_TVOS_DEMO");
			DefineSymbolUtil.AddDefineSymbol(BuildTargetGroup.tvOS, "SAGO_DISABLE_SAGOBIZ");
		#endif
	}
	
	[MenuItem("Sago/Demo/tvOS/Turn On", true, 2000)]
	private static bool tvOSDemoOnIsEnabled() {
		#if UNITY_TVOS && !SAGO_TVOS_DEMO
			return true;
		#else
			return false;
		#endif
	}
	
	[MenuItem("Sago/Demo/tvOS/Turn Off", false, 2000)]
	private static void tvOSDemoOff() {
		#if UNITY_TVOS
			DefineSymbolUtil.RemoveDefineSymbol(BuildTargetGroup.tvOS, "SAGO_TVOS_DEMO");
			DefineSymbolUtil.RemoveDefineSymbol(BuildTargetGroup.tvOS, "SAGO_DISABLE_SAGOBIZ");
		#endif
	}
	
	[MenuItem("Sago/Demo/tvOS/Turn Off", true, 2000)]
	private static bool tvOSDemoOffIsEnabled() {
		#if UNITY_TVOS && SAGO_TVOS_DEMO
			return true;
		#else
			return false;
		#endif
	}
	
	
	[OnBuildPreprocess]
	public static void OnBuildPreprocess(IBuildProcessor intstance) {
		
		PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Suspend;

		#if (SAGO_IOS_DEMO || SAGO_TVOS_DEMO) && !SAGO_DISABLE_SAGOBIZ
			throw new System.InvalidOperationException("Preprocessing error occured: SAGO_DISABLE_SAGOBIZ not defined. In order to build an iOS / tvOS demo, the SAGO_DISABLE_SAGOBIZ must be defined.");
		#endif

		#if (SAGO_IOS_DEMO || SAGO_TVOS_DEMO) && UNITY_5_3_OR_NEWER && UNITY_TVOS
			if (intstance is SagoBuildEditor.tvOS.tvOSBuildProcessor) {
				PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Exit;
			}
		#endif
			
	}
	
	[OnBuildPostprocess]
	public static void OnBuildPostprocess(IBuildProcessor instance) {
		
		#if SAGO_IOS_DEMO
			if (instance is iOSBuildProcessor) {
				
				iOSBuildProcessor processor;
				processor = instance as iOSBuildProcessor;
				
				Dictionary<string, object> dict = processor.ReadPlist();
				dict["CFBundleIdentifier"] = string.Format("{0}.demo", dict["CFBundleIdentifier"]);
				processor.WritePlist(dict);
				
			}
		#endif
		
		#if SAGO_TVOS_DEMO && UNITY_TVOS && UNITY_5_3_OR_NEWER
			if (instance is SagoBuildEditor.tvOS.tvOSBuildProcessor) {
				
				SagoBuildEditor.tvOS.tvOSBuildProcessor processor;
				processor = instance as SagoBuildEditor.tvOS.tvOSBuildProcessor;
				
				Dictionary<string, object> dict = processor.ReadPlist();
				dict["CFBundleIdentifier"] = string.Format("{0}.demo", dict["CFBundleIdentifier"]);
				processor.WritePlist(dict);
				
			}
		#endif
		
	}
	
	
}

#endif
