# SagoBuild User Guide

Author: [Luke Lutman](luke@sagosago.com)  
Version: 1.0



## Overview

The SagoBuild library provides a workflow for automating the preprocessing and postprocessing steps in the build process for Sago Sago apps.

So far at Sago Sago, we've been manually adding the grow tool, updating the build settings and info plist, and committing the Xcode project to the repo every time we update to a new version of Unity. For apps that use asset bundles, we've been manually creating or updating the asset bundles before every build. In both cases, doing the processing manually has prevented us from using Cloud Build, which requires the app to build directly from Unity.

By automating the preprocessing and postprocessing steps, our build process can be faster,  more reliable *and* we can make our apps compatible with Cloud Build.



## Requirements

* Git
* Unity 4.5.5+ 
* Unfuddle account
* Xcode 6.1+ (for iOS builds)
* Android SDK (for Android and Kindle builds)


## Dependencies

* [SagoPlatform](https://sagosago.unfuddle.com/a#/projects/9/repositories/97/browse) – C# library for supporting multiple platforms.
* [PlistCS](https://github.com/animetrics/PlistCS/tree/0fc69e74c4e12fa9734ec830af5d615337749fa3) – C# library for manipulating Plist files.
* [Xcode Manipulation API](https://bitbucket.org/Unity-Technologies/xcodeapi/src/df0f783edfbe1ee05645c446ec40ecd51569d3fd) – C# library for manipulating Xcode projects.



## Installing SagoBuild

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:
 
    `cd /path/to/unity/project`

3. Initialize a new git repository:
    
    `git init`
    
4. Add SagoMesh as a submodule:

    `git submodule add git@sagosago.unfuddle.com:sagosago/sagobuild.git Assets/Framework/SagoBuild`
    
5. Switch back to Unity. You should see the build commands under the `Sago > Build` menu.



## How To Use SagoBuild

If you need project-specific processing before or after the build, you'll need to implement a custom build processor (see below). Otherwise, you can just use the commands in the `Sago > Build` menu for your target platform.




## Build Processors

Build processors are a family of classes used to implement custom preprocess and postprocess callbacks:

	virtual public void Preprocess() {
		// this happens before the build starts
	}
	
	virtual public void Postprocess(string buildPath) {
		// this happens after the build
	}

The base build processor class (`BuildProcessor`) has core functionality. Platform-specific subclasses (i.e. `iOSBuildProcessor`, `AndroidBuildProcessor`) extend the base class with additional functionality for each platform. Project-specific functionality should be implemented in an app-specific subclass (i.e. `ProjectBuildProcessor`) by overriding the `Preprocess()` and `Postprocess()` methods.

To integrate with Cloud Build, which requires static callback methods, build processor classes must also implement static callback methods that map to a build processor instance:

	static MyBuildProcessor Instance;
	
	static public void OnPreprocess() {
		Instance = new MyBuildProcessor();
		Instance.Easy = 123;
		Instance.Preprocess();
	}
	
	static public void OnPostprocess(string buildPath) {
		Instance.Postprocess(buildPath);
		Instance = null;
	}
	
The preprocess callback creates and configures a build processor instance and stores it in a static field. The postprocess callback uses the instance stored in the static field and then clears the field. That way, the same build processor instance is shared between the static preprocess and postprocess callbacks.

The `BuildProcessor` class defines a static `Preprocess(BuildProcessor processor)` helper method for handling the instance logic:

	static public void OnPreprocess() {
		BuildProcessor.OnPreprocess(new MyBuildProcessor());
	}
	
	static public void OnPostprocess(string buildPath) {
		BuildProcessor.OnPostprocess(buildPath);
	}



## Build Runners

When building in the cloud, Cloud Build takes care of setting custom define symbols, calling the static preprocess and postprocess callbacks and actually doing the build. All of the settings are configured via the [Cloud Build](https://build.cloud.unity3d.com) website.

Build runners are a family of classes that provide Cloud Build-like functionality in the Unity editor. The base build runner class (`BuildRunner`) has core functionality like setting define symbols and defining the build path and target. Platform-specific subclasses (i.e. `iOSBuildRunner`, `AndroidBuildRunner`) extend the base class with additional functionality for each platform. Project-specific build runners should not be necessary.

IMPORTANT: The build processor __must not__ depend on the build runner. When building in the cloud, Cloud Build will call the build processor directly and the build runner will not exist.



## Build Menu

To expose a build processor in the Unity editor, define a menu menu item that creates and configures a build runner for it:
	
	[MenuItem("Sago/Build/iOS: Build and Run")]
	public static void BuildAndRunDevice() {
		iOSBuildRunner runner = new iOSBuildRunner();
		runner.BuildOptions = BuildOptions.AutoRunPlayer;
		runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
		runner.OnPreprocess = iOSBuildProcessor.OnPreprocess;
		runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
		runner.Run();
	}

The callback methods assigned to the build runner determine which build processor class is used. Both callback methods __must__ belong to the same build processor class.



## Native Assets

SagoBuild contains native assets that need to be included in the Xcode project for iOS builds  (i.e. frameworks, bundles, plists, Objective-C source, etc). Since Unity ignores hidden files, we prevent them from being imported by putting them in the hidden `SagoBuild/iOS/.Native` directory.

Some of the native assets need to be customized per project (i.e. the `GrowToolSettings.plist`, `Images.xcassets`). The first time an iOS build processor runs, those files are copied to the `Assets/Build/iOS/.Native` directory in the Unity project, where you can customize them. The next time you build, the customized versions will be added to the Xcode project.

To open the hidden `.Native` directory in the Finder, you can browse to the `SagoBuild/iOS` or `Assets/Build/iOS` directory, press `Command + Shift + G`, type `.Native` and press `Go`.



## Grow Tool

The grow tool framework relies on several project-specific settings to function and communicate with third parties services. All of the settings are configured in `Assets/Build/iOS/.Native/GrowToolSettings.plist`. If the file does not exist, run one of the iOS build commands, which will create it for you. Do not modify `Assets/External/SagoBuild/iOS/.Native/GrowToolSettings.plist`.

* `AppStoreAppleId`  
  The iTunes Connect identifier for the app
  
* `AppTitle`   
  The app title displayed in the review prompt.
  
* `AppSchemesURL`  
  The url to a list of url schemes for Sago Sago and Toca Boca apps, the default 
  value is [https://s3.amazonaws.com/com.tocaboca.general/urlschemes.plist](https://s3.amazonaws.com/com.tocaboca.general/urlschemes.plist)
  
* `Production`   
  The production Mixpanel api token, the default value is `f324ebd0e71e68e80c132b40d8106536`
  
* `Development`  
  The development Mixpanel api token, the default value is `994dd4a1b4f8dfa5179bd6c7a79cb41c`.
  
* `ForceShowRating`   
  Use to force the review prompt for testing, the default value is `NO`.


To update `TBGrowTool.framework` and `TBGrowTool.bundle`, you'll need to clone the [SagoGrowToolIOSSource repo](https://sagosago.unfuddle.com/a#/projects/9/repositories/56/browse) and follow the instructions for building the framework and bundle in Xcode. Then, copy the updated `TBGrowTool.framework` and `TBGrowTool.bundle` to the `iOS/.Native` directory in your working copy of the [SagoBuild repo](https://sagosago.unfuddle.com/a#/projects/9/repositories/85/browse) and commit the changes.



## Legacy

The legacy `BuildHelper` class is included to help while we transition from our previous workflow. Once all of our apps have been transitioned, the be removed.

