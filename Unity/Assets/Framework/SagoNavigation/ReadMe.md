# SagoNavigation User Guide

Authors: [John Park](john@sagosago.com), [Luke Lutman](luke@sagosago.com)
Version: 1.0
Unity: 5.4.5
Dependencies: [SagoCore][1], [SagoUtils][2]


---

## Overview

[SagoNavigation][3] is our library for navigating between scenes in Unity.


---

## Requirements

* Git
* Unity 5.4.5+ 
* Github account


---

## Install

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:

	`cd /path/to/unity/project`

3. Initialize a new git repository:
	
	`git init`
	
4. Add SagoNavigation, SagoCore and SagoUtils as submodules:

	`git submodule add --branch sago-world git@github.com:SagoSago/sago-navigation.git Unity/Assets/Framework/SagoNavigation`
	`git submodule add --branch master git@github.com:SagoSago/sago-core.git Unity/Assets/Framework/SagoCore`
	`git submodule add --branch sago-world git@github.com:SagoSago/sago-utils.git Unity/Assets/Framework/SagoUtils`
	
5. Switch back to Unity.

---

## Examples

All of the example below can be found in the [SagoNavigationTest][4] project.

---

## Scene Setup

In order to work correctly with SagoNavigation, scenes must follow a few conventions:

1. All of the scene's content must be inside one root game object.
	
	The hierarchy will look something like this:
	
		- AvocadoScene
			- Camera
			- Avocado
				- Shape
	
	Keeping all scene content in one game object allows the `SceneNavigator` to easily enable and 
	disable the scene content during navigation and destroy the scene content when it's no longer 
	needed. Be careful when instantiating new game objects, it's your responsibility to reparent 
	them inside the root game object. If you do leave anything outside the root game object, 
	it's your responsibility to clean them up in your scene controller's `OnSceneDidTransitionOut` 
	method.
	
	Example: [Avocado.unity][6]
	
2. The root game object must have a `SceneController` component (or a component that's a subclass of `SceneController`).
	
	The hierarchy (with components) will looks something like this:
	
		- AvocadoScene (Transform, AvocadoSceneController)
			- Camera (Transform, Camera)
			- Avocado (Tranform)
				- Shape (Transform, MeshFilter, MeshRenderer, etc.)
	
	Without a `SceneController` component on the root game object in the scene, the `SceneNavigator` can't 
	register or manage the scene. In the example above, `AvocadoSceneController` is a subclass of `SceneController`.
	
	Example: [Avocado.unity][6]
	
3. The scene must _not_ contain an `AudioListener` component.
	
	SagoNavigation loads scenes additively, so there may more than one scene enabled at the same time. 
	If more than one `AudioListener` exists, Unity gets angry and throws a bunch of warnings. In Sago 
	apps, the `AudioListener` lives on the [AudioManager][5] in SagoAudio, so you don't need one in 
	your scene.
	
4. The culling mask used by cameras in the scene must _not_ include the layer used by scene transitions.
	
	SceneTransition use their own camera, and should not be rendered by cameras in the scene. By 
	default, scene transitions use the `TransparentFX` layer.


---

## SceneNavigator

The `SceneNavigator` is used to navigate between scenes.
	
	[SerializeField]
	private SceneReference m_MainScene;
	
	if (!string.IsNullOrEmpty(m_MainScene.AssetBundleName)) {
		// load asset bundle...
	}
	
	if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {
		SceneNavigator.Instance.NavigateToScene(
			m_MainScene, 
			FadeTransition.Create(), 
			FadeTransition.Create(), 
			null, 
			true, 
			true
		);
	}
	
Example: [AvocadoSceneController.cs][7]

To navigate to another scene:
	
1. If the scene is deployed in an asset bundle, make sure the asset bundle and all of it's dependencies have been loaded.
	
	If you're using [SagoApp][9], the [ProjectNavigator][10] will take care of this for you. If you're 
	using `SagoNavigation` outside of `SagoApp`, you'll need to manage loading and unloading of asset 
	bundles yourself.
	
1. Make sure that the `SceneNavigator.Instance` is not null.
	
	Although the `SceneNavigator` is a singleton, `Instance` may be null when stopping playback in the 
	editor (due to objects being destroyed in an undefined order). Doing a null check will prevent your
	code from throwing exceptions when the `SceneNavigator` is destroyed before the object running 
	your code.
	
1. Make sure the `IsReady` property is `true`.
	
	`IsReady` will be `false` while the `SceneNavigator` is navigating to a scene (navigating to a scene 
	can't be interrupted). If you call `NavigateToScene` while the `SceneNavigator` is busy, it will 
	throw an exception.
	
	
1. Call SceneNavigator.Instance.NavigateToScene(...)
	
	* The first argument is the scene to navigate to, as a [SceneReference][8].
	* The second and third arguments are `SceneTransition` objects.
		* Use `FadeTransition.Create()` to create Sago's default white fade.
	* The fourth argument is a `LoadingIndicator` object.
		* Use `null`, unless you know what you're doing ;-)
	* The fifth arugment is the `unloadCurrentScene` flag.
		* Use `true`, unless you have a good reason to keep the scene in memory.
	* The sixth arugment is the `unloadUnusedAssets` flag.
		* Use `true`, unless you have a good reason to keep assets in memory.


---

## SceneTransition

`SceneTransition` objects allow you to customize how the current scene disappears and how the next scene appears when navigating.

There are two predefined `SceneTransition` implementations in `SagoNavigation` for you to use:

* EmptyTransition
	
	A hard cut from the current scene to the next scene.
	This is the default scene transition used if you pass `null` for `NavigateToScene()`'s `transitionOut` or `transitionIn` arguments.
	
* FadeTransition
	
	Fade the current scene to a solid color, then fade the next scene in.
	This is the standard fade through white used in Sago apps.
	You can make a custom FadeTransition by copying [FadeTransition.prefab][11] to your project and modifying it (see: [Cherry.unity][12], [CherrySceneController.cs][14]).

You can also subclass `SceneTransition` to implement your own custom transitions.


---

## SceneController

`SceneController` objects represent one scene in `SagoNavigation`.

For each scene:

1. Implement a `SceneController` subclass.
	
		namespace Fruit {
			
			public class BananaSceneController : SceneController {
				
			}
			
		}

1. Implement the `Start` method (if necessary).
	
		void Start() {
			GetComponentInChildren<TouchAreaObserver>().TouchUpDelegate = (touchArea, touch) => {
				if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {
					touchArea.enabled = false;
					SceneNavigator.Instance.NavigateToScene(
						m_MainScene, 
						FadeTransition.Create(), 
						FadeTransition.Create(), 
						null, 
						true, 
						true
					);
				}
			};
		}
	
	_Note:_ The start method will only be called once, the first time the scene is 
	used. Don't put code here that needs to run each time you navigate to the scene.

1. Override `ISceneTransitionObserver` methods as necessary.

	* `OnSceneWillTransitionOut`
	* `OnSceneDidTransitionOut`
	* `OnSceneWillTransitionIn`
	* `OnSceneDidTransitionIn`
	
	_Note:_ Avoid using the `Start`, `OnEnable` or `OnDisable` methods if possible – the 
	`ISceneTransitionObserver` methods are more consistent and provide better control 
	over the timing of when things happen.
	
	Example: [BananaSceneController.cs][13]
	
1. Override the `IsReady` property, if necessary.
	
	If you need to do any async setup (i.e. loading assets, http requests, etc), 
	the `IsReady` property should return false until it's safe for the `SceneNavigator` 
	to transition to your scene. Since `OnSceneWillTransitionIn` won't be called until 
	`IsReady` is `true`, you must start any async operations in the `Start` method 
	(see: [AvocadoSceneController.cs][7]).


---

  [1]: https://github.com/SagoSago/sago-core
  [2]: https://github.com/SagoSago/sago-utils/tree/sago-world
  [3]: https://github.com/SagoSago/sago-navigation/tree/sago-world
  [4]: https://github.com/SagoSago/sago-navigation-test
  [5]: https://github.com/SagoSago/sago-audio/blob/sago-world/Scripts/AudioManager.cs
  [6]: https://github.com/SagoSago/sago-navigation-test/blob/master/Unity/Assets/Project/Scenes/Avocado.unity
  [7]: https://github.com/SagoSago/sago-navigation-test/blob/master/Unity/Assets/Project/Scripts/AvocadoSceneController.cs
  [8]: https://github.com/SagoSago/sago-core/blob/master/Scripts/Scenes/SceneReference.cs
  [9]: https://github.com/SagoSago/sago-app/tree/sago-world
  [10]: https://github.com/SagoSago/sago-app/blob/sago-world/Scripts/Project/ProjectNavigator.cs
  [11]: https://github.com/SagoSago/sago-navigation/blob/sago-world/Resources/SagoNavigation/FadeTransition.prefab
  [12]: https://github.com/SagoSago/sago-navigation-test/blob/master/Unity/Assets/Project/Scenes/Cherry.unity
  [13]: https://github.com/SagoSago/sago-navigation-test/blob/master/Unity/Assets/Project/Scripts/BananaSceneController.cs
  [14]: https://github.com/SagoSago/sago-navigation-test/blob/master/Unity/Assets/Project/Scripts/CherrySceneController.cs
