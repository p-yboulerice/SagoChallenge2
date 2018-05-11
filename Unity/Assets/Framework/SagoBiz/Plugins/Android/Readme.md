SagoBiz Android Native project:

The .Native folder contains an Android Studio project that implements native functionality for SagoBiz Device Info and SagoBiz Promo system.

Most of its libraries are accessible through SagoBiz/Scripts/Native.cs.

The Android Studio project consist of one module:

1 - SagoBiz: which holds implementation for DeviceInfo, WebView for Promo and Parents as well as Mixpanel.

There is also an externalLibs directory. It includes third party libraries that are necessary for building SagoBiz and the unity project:
	- mixpanel-android-*.aar
	- play-services-base-*.aar
	- play-services-gcm-*.aar

To update the external libraries, you simply replace the ones in the libs folder with the newer versions, and then rebuild the android studio project (assemble). If the file name is not the same you will have to update the references in the build.grale (Module: sagoBiz) file.

Current Versions for external libraries are:
- Mixpanel: 4.6.4
- Play Service base (Used by Mixpanel): 8.1.0
- Play Service gcm (Used by Mixpanel): 8.1.0


-------------
Project Setup
-------------

1. Verify path to Android SDK and SDK and Build Tool Versions

a. Open local.properties.
- Make sure the sdk.dir points to your correct Andorid SDK path.

b. Open build.gradel (Module: sagoBiz)
- Note the compiledSdkVersion.
- Note the buildToolsVersion.

c. Open Tools -> Android -> SDK Manager -> SDK Platforms
- Make sure you have the compiledSdkVersion SDK installed on your system.

d. Open Tools -> Android -> SDK Manager -> SDK Tools
- Make sure you have the buildToolsVersion SDK Platform-Tools installed on your system.


2. Add Unity classes.jar to Classpath

a. Open build.gradle (Module: sagoBiz)

b. Verify the path to the Unity classes.jar is correct and using the correct version of Unity.
- Eg: /Applications/Unity/Unity.app/Contents/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar

c. If you changed the path click the "Sync Now" button (top right).


----------------
Building Project
----------------

1. Expand Grandle vertical tab on right hand side.
2. Expand .Native -> :sagoBiz -> Tasks -> build
3. Double click the "assemble" task.
4. Verify project AAR's were updated
- SagoBiz/build/outputs/aar/sagoBiz-debug.aar
- SagoBiz/build/outputs/aar/sagoBiz-release.aar
5. Git add/commit both AARs.

TODO: Investigate why Build -> Make Project doesn't run the "assemble" task.