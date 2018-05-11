# Workflow Window User Guide

The `WorkflowWindow` collects all of the options and tools for developing, testing and building our projects in one convenient place.


---


<a name="opening-the-workflow-window"></a>
## Opening the Workflow Window

You can open the `WorkflowWindow` window from the `Sago` menu:

	Sago > Workflow
	
Or from the `Window` menu:

	Window > Sago > Workflow


---


<a name="general"></a>
## General


<a name="mode"></a>
#### Mode
	
The mode popup allows you to choose the correct workflow mode for the task you're currently working on. You'll spend most of your time in `Develop` mode and then switch to one of the other modes when you're ready to test with asset bundles in the editor or on a device.

* <a name="mode-develop"></a> __Develop__
	
	Use this mode while you're working in the editor so you don't have to build the asset bundles or update the map assets.
	
* <a name="mode-test"></a> __Test__
	
	Use this mode to make sure the asset bundles and map assets are working in the editor before you build to a device.
	
	In this mode, you must update the map assets and build the asset bundles before playing. You must build the asset bundles for `BuildTarget.StandaloneOSXUniversal` (even if you want to test on another platform) because Unity can't use iOS or Android asset bundles in the editor.
	
	In this mode, you can save time by only updating and building a subset of the asset bundles for the project. Only the content submodules you update and build the asset bundles for will work.
	
* <a name="mode-build-custom"></a> __Build Custom__
	
	Use this mode to quickly make builds to test on a device.
	
	In this mode, the build process will _NOT_ automatically update and build the asset bundles every time you make a build. You are responsible for updating the map assets and building the asset bundles when necessary.
	
	In this mode, you can save time by only updating and building a subset of the asset bundles for the project. Only the content submodules you update and build the asset bundles for will work.
	
	The build process in this mode does _NOT_ match the build process in Cloud Build.
	
* <a name="mode-build-complete"></a> __Build Complete__
	
	Use this mode to make builds for release.
	
	In this mode, the build process will automatically update and build the all of the asset bundles for the project every time you make a build. This mode is much slower, but guarantees that all of the necessary actions happen in the correct order so that the build is complete.
	
	The build process in this mode matches the build process in Cloud Build.


#### Platform

The active platform.

_Note:_ Switching platforms can be very slow if doing so requires switching Unity's build target. Unity may have to reimport and recompile _a lot_ of assets and it doesn't update the platform popup until it's finished. Be patient! 

Turning on the Cache Server makes switching build targets _much_ faster. You can turn on the Cache Server by going to `Unity > Preferences > Cache Server` and entering the following settings:

	Use Cache Server: Yes
	IP Address: sagobot

If you are going to be building and testing asset bundles in the editor (i.e. [Test](#mode-test) mode), it's best to leave the platform set to `Unknown` (which corresponds to Unity's `StandaloneOSXUniversal` build target) to minimize the number of times Unity will need to switch build targets.

If you are going to be building and testing the app on a device (i.e. [Build Custom](#mode-build-custom) or [Build Complete](#mode-build-complete) mode), it's best to switch to the correct plaform for your target device leave it there (again minimizing the number of times Unity will need to switch build targets).


---


## Asset Bundles


<a name="asset-bundles-mask"></a>
#### Mask

The mask field allows you to select which content submodules should be included when [updating](#asset-bundles-update) and [building](#asset-bundles-build) asset bundles.

When you're working in [Test](#mode-test) or [Build Custom](#mode-build-custom), you can save a lot of time by unchecking content submodules you're not working on. Asset bundles for unchecked content submodules will not be updated or built and will not be available at runtime.

When you're working in [Build Complete](#mode-build-complete) mode, the mask field is ignored – asset bundles are updated and build for all content submodules.


<a name="asset-bundles-remote-adaptor-type"></a>
#### Remote Adaptor Type

The asset bundle adaptor type field allows you to set which adaptor type to use for asset bundles marked as `AssetBundleDeploymentType.Remote` in the `AssetBundleMap`. For all local and _Hockey_ builds, you should use [StreamingAssets](#asset-bundles-remote-adaptor-type-streaming-assets). Some of our iOS app store builds

* <a name="asset-bundles-remote-adaptor-type-unknown"></a> __Unknown__
	
	Do not deploy this asset bundle.
	
* <a name="asset-bundles-remote-adaptor-type-streaming-assets"></a> __StreamingAssets__
	
	Deploy remote asset bundles in the `StreamingAssets` folder.
	
* <a name="asset-bundles-remote-adaptor-type-on-demand-resources"></a> __OnDemandResources__

	Deploy remote asset bundles using On Demand Resources (_Note:_ iOS only).
	
* <a name="asset-bundles-remote-adaptor-type-asset-bundle-server"></a> __AssetBundleServer__
	
	Deploy remote asset bundles using an asset bundle server (_Note:_ We haven't actually implemented and asset bundle server, so choosing this option is equivalent to choosing [Unknown](#asset-bundles-remote-adaptor-type-unknown). We may use this option in the future if we deploy remote asset bundles on Android).


<a name="asset-bundles-update"></a>
#### Update Asset Bundles

Use the _Update Asset Bundles_ button to prepare the assets in each content submodule before building the asset bundles.

For each selected content submodule (see: [Mask](#asset-bundles-mask)), it will:

* Find all assets (plus dependencies) in the `_Resources_` folder and apply the `ResourceAssetBundleName`.
* Find all scenes (plus dependencies) in the `_Scenes_` folder and apply the `SceneAssetBundleName`.

You'll need to run _Update Asset Bundles_ before you run _Build Asset Bundles_ whenever you've added, changed or removed assets (or dependencies) in the `_Resources_` or `_Scenes_` folder.

_Important:_ You'll need to run _Update Asset Bundles_ when you add a new scene to a `_Scenes_` folder before that scene will work at runtime in the editor.

Commit any changes to the content submodules to their respective repositories.


<a name="asset-bundles-build"></a>
#### Build Asset Bundles

Use the _Build Asset Bundles_ button to build the asset bundles for each selected content submodule (see: [Mask](#asset-bundles-mask)).

In [Test](#mode-test) mode, this will always switch the build target to `StandaloneOSXUniversal` because asset bundles built for other platforms will not work in the editor (you'll see a lot of broken pink shaders). In all other modes, this will build the asset bundles for the selected platform.

The asset bundles will be built to `Assets/Project/AssetBundles/[AdaptorType]`. The adaptor type depends on the configuration for each asset bundle in the `AssetBundleMap` and `AssetBundleAdaptorMap`.

When building for a device, any asset bundles in `Assets/Project/AssetBundles/StreamingAssets` will be copied to `Assets/StreamingAssets` before the build begins, then copied back after the build is complete. Existing assets in `Assets/StreamingAssets` will _not_ be overwritten.

---


<a name="store"></a>
## Store

<a name="store-model"></a>
#### Model

The model popup allows you to choose from a list of mock models so you can test various store scenarios in the editor. This option is ignored in [Build Custom](#mode-build-custom) and [Build Complete](#mode-build-complete) modes.

_Important:_ Using mock models in the editor is great for testing the UI, but uses a completely separate code path than connecting to the live _App Store_ on a device. It is not a replacement for testing on the device!!


<a name="store-server-type"></a>
#### Server Type

The server type popup allows you to choose which server to use when connecting to Sago's `World Service API` server:

* <a name="store-server-type-production"></a> __Production__
	
	The production server. Used in live versions of the app available in the app store. It is hosted remotely and should always be available.
	
* <a name="store-server-type-staging"></a> __Staging__
	
	The staging server. Used in beta versions of the app available on Hockey or Test Flight. It is hosted remotely and should always be available.
	
* <a name="store-server-type-development"></a> __Development__
	
	The development server. Used in the editor and in local builds of the app. It's hosted locally and is only available when you have a copy of [the server](https://github.com/SagoSago/world-api) running on your development machine.


<a name="store-server-url"></a>
#### Server Url

The server url field allows you to set the url for each [Server Type](#store-server-type). The `Production` and `Staging` urls should almost never change. The `Development` url will be automatically updated with your development machine's ip address.


---


<a name="project"></a>
## Project


<a name="project-platform-settings"></a>
#### Platform Settings

A shortcut to open the platform settings prefab for the [active platform](#general-platform).


<a name="project-player-settings"></a>
#### Player Settings

A shortcut to the player settings for the active [build target](https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings-activeBuildTarget.html).


<a name="project-development-build"></a>
#### Development Build

See: [EditorUserBuildSettings.development](https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings-development.html).


<a name="project-connect-with-profiler"></a>
#### Connect With Profiler

See: [EditorUserBuildSettings.connectProfiler](https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings-connectProfiler.html)


<a name="project-allow-debugging"></a>
#### Allow Debugging

See: [EditorUserBuildSettings.allowDebugging](https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings-allowDebugging.html)


---


<a name="project-ios"></a>
## Project (iOS Only)


<a name="project-ios-build-type"></a>
#### Build Type

The build type popup allows you to choose which type of build to create.

* <a name="project-ios-build-type-simulator"></a> __Simulator__
	
	Create a development build for the iOS simulator.

* <a name="project-ios-build-type-device"></a> __Simulator__
	
	Create a development build for an iOS device.

* <a name="project-ios-build-type-ad-hoc"></a> __Ad Hoc__
	
	Create an ad hoc build that can be uploaded to _Hockey_.

* <a name="project-ios-build-type-app-store"></a> __App Store__
	
	Create an app store build that can be uploaded to _iTunes Connect_.


<a name="project-ios-build-action"></a>
#### Build Action

The build action popup allows you to choose how _Xcode_ behaves when building the app.

* <a name="project-ios-build-action-build"></a> __Build__
	
	Build the _Xcode_ project. Use this option when you just want to make sure the project compiles without errors.

* <a name="project-ios-build-action-build-and-run"></a> __Build And Run__
	
	Build and run the _Xcode_ project. Use this option when you want to run the app on a device connected to your development machine.

* <a name="project-ios-build-action-build-and-archive"></a> __Build And Archive__
	
	Archive the _Xcode_ project. Use this option when you want to upload the build to _Hockey_ or _iTunes Connect_.


---

<a name="project-continued"></a>
## Project (Continuted)

<a name="project-update-project"></a>
#### Update Project

Use the _Update Project_ button to prepare the project's assets before building the project.

When you click the _Update Project_ button, it will:

* Create or update the `SubmoduleMap` asset (see: `Assets/Project/Resources/SubmoduleMap.asset`).
* Create or update the `ContentInfo` asset and it's dependencies for all content submodules.
* Create or update the `ProjectInfo` asset.
* Update Unity's `GraphicsSettings` asset with any always includes shaders defined in content submodules.
* Update Unity's `TagManager` asset with any layers and tags defined in content submodules.
* Create or update the `ResourceMap` asset (see: `Assets/Project/Resources/ResourceMap.asset`).
* Create or update the `SceneMap` asset (see: `Assets/Project/Resources/SceneMap.asset`).
* Create or update the `AssetBundleMap` asset (see: `Assets/Project/Resources/AssetBundleMap.asset`)
* Create or update the `AssetBundleAdaptorMap` asset (see: `Assets/Project/Resources/AssetBundleAdaptorMap.asset`).

You'll need to run _Update Project_ before you run _Build Project_ when:

* you add or remove a submodule
* you add, change or remove assets in a `_Resources_` or `_Scenes_` folder in a content submodule.
* you change the `GraphicsSettings` or `TagManager` asset in a content submodule.
* you [update asset bundles](#asset-bundles-update).

Commit any changes to the project assets to the repository.


<a name="project-build-project"></a>
#### Build Project

Use the _Build Project_ button to build the project!!
























