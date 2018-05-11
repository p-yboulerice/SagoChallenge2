# SagoBiz User Guide

Authors: [Luke Lutman](luke@sagosago.com), [Colin McCune](colin@sagosago.com)  
Version: 1.0  
Unity: 4.6.2  
Dependencies: [JSON.NET][1], [Mixpanel][2], [SagoEasing][3], [SagoLayout][4], [SagoTouch][5]


----------


## Overview

SagoBiz is our library for integrating non-game functionality into our apps. Non-game functionality includes analytics, cross promotion, the parents page and more. The goal of the library is to keep the game and non-game code decoupled and allow us to easily add and maintain the non-game code in all of our apps.


----------


## Quickstart

1. Create or open a Unity project.
2. Open the terminal and change to the project directory:  
    `cd /path/to/unity/project`

3. Initialize a new git repository:  
    `git init`

4. Add SagoBiz as a submodule:  
    `git submodule add git@sagosago.unfuddle.com:sagosago/sagobiz.git Assets/Framework/SagoBiz`

5. Add the application start callback to the app's first scene:  
    `SagoBiz.Facade.OnApplicationStart();`

6. Add the scene callbacks to the app's title scene:  
    `SagoBiz.Facade.OnSceneWillAppear();`  
    `SagoBiz.Facade.OnSceneDidAppear();`  
    `SagoBiz.Facade.OnSceneWillDisappear();`  
    `SagoBiz.Facade.OnSceneDidDisappear();`  

7. Run the first scene, which will copy the SagoBiz prefab from the submodule into the project as:
    `Assets/Resources/SagoBiz/Prefabs/SagoBiz.prefab`

8. Drag the prefab from the resources folder into the scene.
9. Customize the options in the prefab instance and apply them.
10. Delete the prefab instance from the scene.


----------


## Facade

The `Facade` component provides an easy, safe way for game code to communicate with non-game code. Game code should only use the facade and avoid accessing any of the other components directly.


----------


## Controller

The `Controller` component is the entry point to SagoBiz. It provides a singleton that knows how to load itself from a prefab in the resources folder, controls the flow of loading and displaying the parents and promo buttons and allows components to access each other.


----------


## Analytics

The `Analytics` component is used for tracking events, like the number app starts, number of cross promo button impressions, etc. The interface of the `Analytics` component is generic and game code will not have any dependency on the analytics service we're using (currently Mixpanel).

There is an unofficial Mixpanel implementation for Unity, but it's very basic. The most important feature that's missing is ability to track events offline. As a workaround, we're going to use a hybrid approach – platforms which have a native Mixpanel implementation will use it (via the `Native` component), other platforms will fall back to the Unity implementation.

Depending on the `AnalyticsMode` (configured via the `Options` component), the `Analytics` component will send events to the development server or to the production server.


----------


## Native

The `Native` component provides a platform-independent C# interface to native plugins that implement things we cannot do in Unity, like collecting device info, passing events to the native Mixpanel plugin or opening the web view.

Using conditional compilation, each platform will implement the entire interface. The default platform (used in the editor and on unknown platforms) will provide empty or null implementations for each method and property.


----------


## Options

The `Options` component provides a way to store multiple versions of options for different platforms and build configurations (so that options for different platforms can live side by side and don't require a new branch in the app's repository).


----------


## Parents

The `Parents` component provides a way to load and display the parents button. When the button is tapped, the `Parents` component will open the parents page in the web view. The url for the parents page is configured via the `Options` component. _Right now, the parents button and url are built into the app and cannot be changed on the fly. In the future, the button and url could be loaded by the `Service`, the same way as the promo button and url._


----------


## Promo

The `Promo` component provides a way to load and display the promo button. When the button is tapped, the `Promo` component will open the promo page the web view.


----------


## Service

The `Service` component provides a way to load the basic data required for the `Analytics`, `Parents` and `Promo` components.

The service loads in three steps:
1. Load the config data, which includes the list of url schemes of Sago Sago and Toca Boca apps.
2. Send device info to the server, using the list of url schemes to check which apps are installed.
3. Receive the promo data, which includes the url of the promo image and page.

Depending on the `ServiceMode` (configured via the `Options` component), the `Service` can load data from the app's `StreamingAssets` folder, from the development server or from the production server.


----------

## Dependencies
* __Json.NET__ – We're using the [Json.NET for Unity][1] asset from the [Unity Asset Store](https://www.assetstore.unity3d.com/en/) for serializing and deserializing data sent to and from the server.
* __Mixpanel-Unity-CSharp__ – We're using a customized version of [Mixpanel Unity CSharp][2] as a fallback for platforms that don't have an official Mixpanel SDK. We've replaced `LitJson` (which does not compile on Windows platforms) with `JSON.NET`.




  [1]: http://u3d.as/content/parent-element/json-net-for-unity/5q2
  [2]: https://github.com/waltdestler/Mixpanel-Unity-CSharp
  [3]: https://sagosago.unfuddle.com/a#/projects/9/repositories/54/browse
  [4]: https://sagosago.unfuddle.com/a#/projects/9/repositories/46/browse
  [5]: https://sagosago.unfuddle.com/a#/projects/9/repositories/40/browse