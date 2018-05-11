# SagoMesh User Guide

Author: [Luke Lutman](luke@sagosago.com)
Version: 1.0
Unity: 4.5.0
Dependencies: [SagoAudio][1] (Optional)


----------


## Overview

SagoMesh is our library for displaying vector animation exported from Flash as meshes in Unity. Since our apps use lots (and lots) of hand-drawn animation, bitmap-based sprites quickly run into memory issues on mobile devices. Meshes load faster, use less memory, scale to fit any resolution and often render more efficiently than bitmaps. Bottom line, meshes let us pack as much animation into our apps as possible.

At the most basic level, you can think of a mesh animation as a flipbook, there's one mesh per frame of animation, and each mesh gets displayed one after the other to create an animation.


----------


## Examples

All of the example files referenced below can be found in the [SagoMeshTest][2] project, which has `.fla` files, Unity scenes and `C#` code you can use to see SagoMesh and SagoMeshTools in action. 


----------


## Requirements

* Git
* Unity 4.5+ 
* Unfuddle account

SagoMesh works in the free version of Unity, but some features that depend on Unity Pro are disabled.


----------


## Installing SagoMesh

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:
 
    `cd /path/to/unity/project`

3. Initialize a new git repository:
    
    `git init`
    
4. Add SagoMesh as a submodule:

    `git submodule add git@sagosago.unfuddle.com:sagosago/sagomesh.git Assets/External/SagoMesh`
    
5. Switch back to Unity. You should see the SagoMesh settings asset in the root of your project and the SagoMesh menu item under `Sago > Mesh`.


----------


## Import Settings

* **Auto Import** – Should SagoMesh import the `.xml` and `.bytes` files exported from Flash automatically? When Auto Import is turned off, switching between Unity and another application doesn't trigger SagoMesh's import process. You can manually trigger the import process at any time from the menu: `Sago > Mesh > Import`.

* **Delete Intermediate Files** – Should SagoMesh delete the `.xml` and `.bytes` files after they've been imported? Not deleting the intermediate files allows you to manually inspect the `.xml` file, which can be helpful if you're having trouble getting an animation to import cleanly. If you have intermediate files that you're ready to clean up, just turn on `Delete Intermediate Files` and manually trigger the import process from the menu: `Sago > Mesh > Import`.

* **Pixels Per Meter** – Coordinates in Flash use pixels, but coordinates in Unity use meters. If you export a 100px by 100px circle from Flash and import it into Unity, it will be 100m by 100m – that's gigantic! While it's convenient to let one unit in Flash equal one unit in Unity, it can cause issues with the physics engine and rounding errors. The `Pixels Per Meter` setting will scale the meshes during import so they're a more reasonable size. You should choose a pixels per meter ratio at the beginning of a project (before importing any meshes) and then keep it the same, so that all of the meshes are scaled consistently. The default value is 100 (100px in Flash = 1m in Unity).

* **Sago Audio Mode** – To avoid making SagoMesh depend on SagoAudio (another library for handling audio), the `Sago Audio Mode` setting sets some compiler flags to enabled or disable SagoAudio integration:

    * **Exists** – Enable SagoAudio integration if SagoAudio exists, disable SagoAudio integration and don't throw any errors if SagoAudio doesn't exist.
    
    * **Always** – Always enable SagoAudio integration, thow errors if it doesn't exists.
    
    * **Never** – Never enable SagoAudio integration.
    
If SagoAudio integration is disabled, you'll need to implement a custom component to play audio clips associated with animations. See the `MeshAnimatorAudioObserver` class for an example of how to implement audio playback.

If you change the `Sago Audio Mode` setting and Unity doesn't pick up the change (i.e. you set it to `Never` and it still throws an error), you may need to tell SagoMesh to update the define symbols by choosing `Sago > Mesh > Update Define Symbols` from the menu.


----------


## How To Import Animations

Each symbol exported from Flash consists of two intermediate files: a `.xml` file that contains metadata about the animations in that symbol, and a `.bytes` file, which contains the vertex data for that symbol's layers and frames.

When you export from Flash and switch back to Unity, SagoMesh will look for intermediate files anywhere in the `Assets` folder. If `Auto Import` is turned on the the SagoMesh settings, the intermediate files will be imported automatically. Otherwise, you can manually import them at any time from the menu: `Sago > Mesh > Import`.

When SagoMesh imports the intermediate files, it converts them into a series of `MeshAnimation` assets and a `MeshAnimationAtlas` asset. Each `MeshAnimation` asset contains the metadata for one animation (a list of meshes, a list of audio clips, the framerate, etc). The `MeshAnimationAtlas` asset contains the meshes for all of the animations in the symbol.

**IMPORTANT:** If you need to re-export a symbol from Flash, make sure to export it to the same directory as the existing `MeshAnimation` and `MeshAnimationAtlas` assets (the intermediate files must be in the same directory as the corresponding assets), so that SagoMesh can update the existing assets. If you re-export to a different directory, SagoMesh will import the intermediate files as new assets. You'll end up with two copies of those assets in your project and any references will still be pointing at the old assets.


----------


## How To Preview Animations

**IMPORTANT:** Animation previews use Unity Pro's render texture features. Animation previews are not available in the free version of Unity.

To preview an animation, selected an `MeshAnimation` asset in the project window or a game object with a `MeshAnimator` component in the hierarchy and open the prevew window from the menu: `Window > MeshAnimation Preview`.


----------


## How To Play Animations

#### Basics

To play a `MeshAnimation`:

1. Create a game object in the scene.
2. Add a `MeshAnimator` component.
3. Add a `MeshAnimationSource` component.
4. Drag a `MeshAnimation` asset from the project window and to the `Animation` property on the `MeshAnimatorSource` component.
5. Check the `Auto Play` toggle on the `MeshAnimator` component.
6. Run the scene. The animation will play once, then stop.

To make setting up a `MeshAnimator` easier, you can use the `GameObject > Create Other > Mesh Animator` menu item. It will take care of steps create the game object and add the necessary components, and if you have a `MeshAnimation` selected in the `Project` window, step populate the source as well.

See: [Planets.unity][3]


----------


#### MeshAnimator

The `MeshAnimator` component is used to play `MeshAnimation` assets. It requires another component that implements the `IMeshAnimatorSource` interface on the same game object to tell it which `MeshAnimation` asset to use.

* **AutoPlay** – Should the animation start playing when the mesh animator's `Start()` method is called? The default value is false.

* **Direction** – Should the animation play forward or reverse? The default value is forward.

* **Frame** – The current frame of the animation. In the editor, you can use the slider to scrub through the animation. If `AutoPlay` is turned on, the animation will start from this frame. The default value is false.

* **Lock** – Should the mesh animator dynamically update it's layers to match the current animation, or should it leave the layers alone. If you've customized any of the animator's layers (assigned a custom material, added any components, etc.), locking the mesh animator will prevent your customizations from being lost if the animation changes. The default value is false.

* **Loop** – Should the animation loop? The default value is false.


----------


#### IMeshAnimatorSource

To keep `MeshAnimator` decoupled from how animations are loaded and assigned, it depends on having another component on the same object which implements the `IMeshAnimatorSouce` interface to tell it which `MeshAnimation` asset to use.

The default implementation of `IMeshAnimatorSource` is `MeshAnimationSource`, which keeps a serialized reference to a `MeshAnimation` asset. When you build your application, any `MeshAnimation` assets referenced by a `MeshAnimationSource` will be included. Referenced animations will be available immediately when the scene or resource is loaded.

In the future, we could be other `IMeshAnimatorSource` implementations to allow `MeshAnimation` assets to be loaded from the `Resources` folder, from an asset bundle or another custom loading implementation.


----------


#### MeshAnimatorLayer

To support multiple layers within an `MeshAnimation` asset, the `MeshAnimator` component dynamically manages a set of layers (child game objects with `MeshAnimatorLayer` components). Whenever its source's animation changes, the `MeshAnimator` destroys and recreates it's layers to match the new animation.

Each layer has it's own  `MeshFilter`, `MeshRenderer` (the `MeshAnimator` itself doesn't have a renderer). The `MeshAnimator` has `Bounds` and `IsVisible` properties that include all of the animator's layers.

See: [picnic_export.fla][4], [Picnic.unity][5]

Each layer in an animator can have different materials and can be offset on the z axis to avoid depth fighting (if necessary).

See: [marshmallow.fla][6], [Marshmallow.unity][7]

**IMPORTANT:** An animator and its layers are designed to work as a unit. If you want to use one layer by itself, move the layers relative to one another, or otherwise use the layers as individual parts, you should export the layers as separate animations.


----------


#### IMeshAnimatorObserver

Often, other components in your game need to know what's happening in an animator. The `IMeshAnimatorObserver` interface allows the `MeshAnimator` to notify other components when the animation starts playing, stops playing or enters a frame. The `MeshAnimator` component will look for any number of `IMeshAnimatorObserver` components on the same game object and call the following methods:

*  **OnMeshAnimatorJump** – Called when the animator's `CurrentIndex` changes. This method is called both when the animator is playing and when it is stopped. You should can check whether it's playing or not using the animator's `IsPlaying` method.

* **OnMeshAnimatorPlay** – Called when the animator is stopped and starts playing.

* **OnMeshAnimatorStop** – Called when the animator is playing and stops. You can check if the animator played to the end of the animation using the animator's `IsComplete` property.

**IMPORTANT:** If the animator's framerate is slower than Unity's framerate, one animation frame will be displayed for for multiple Unity frames (i.e. if the animator is running at 12 fps and Unity is running at 60fps, one animation frame would be displayed for 5 Unity frames).

If you need to execute some game logic on a specific frame of an animation, you should always use `OnMeshAnimatorJump`  (which is called once when the animator's `CurrentIndex` changes) instead of polling the animator's `CurrentIndex` property in an Update method or Coroutine. The difference in framerate will make `animator.CurrentIndex == 42` true for multiple Unity frames and could cause your game logic to be executed multiple times.

If you need execute some game logic when an animator has finished playing, you should check it's `IsComplete` property instead of checking `animator.CurrentIndex == animator.LastIndex`. The difference in framerate will make `animator.CurrentIndex == animator.LastIndex` true even though that frame of animation needs to be displayed for a few more Unity frames and could cause your game logic to execute too soon.

See: [Marshmallow.unity][8]


----------


#### MeshAnimatorAudioObserver

`MeshAnimationAudioObserver` implements the `IMeshAnimatorObserver` interface to play an animation's audio clips using SagoAudio (if it's enabled). If you're playing an animation with audio but you're not hearing anything, make sure you've enabled SagoAudio in the SagoMesh settings and added a `MeshAnimationAudioObserver` to the game object with the `MeshAnimator`.

See: [Picnic.unity][9]

----------


#### MeshAnimatorMultiplexer

We often use multiple animations to show different states of an object, with only one animation being displayed at a time (i.e. a hero object that has different animations for idle, jump and walk). The `MeshAnimatorMultiplexer` component helps you to implement this pattern by allowing you to set which of its child `MeshAnimator` game objects is active and deactivating the others.

* **Animator** – The current animator.

* **Editor Mask** – In the editor, should the multiplexer activate the current animator or should it activate all of the animators?

    * *Animator* – Activate the current animator.
    
    * *Everything* – Activate all of the animators. It's often helpful to have the editor activate everything to make sure all of your animators are registered correctly. The default value is `Animator`.

* **Layer Mask** – Which layers should the multiplexer include when searching for animators? Occasionally, you may want an animator to be nested inside a multiplexer, but not be controlled by the multiplexer. If you assign that animator a custom layer, then remove that layer from the multiplexer's layer mask, the multiplexer will ignore that animator.

See: [Picnic.unity][10], [Marshmallow.unity][11]

----------


## Other

By default, meshes created by SagoMesh are marked as read-only (read-only meshes use half as much memory as read-write meshes). In some cases, Unity needs meshes to be read-write (mesh colliders, non-uniform scaling). If you need to mark a mesh as read-only or read-write, you can select an individual mesh or a whole `MeshAnimationAtlas` in the project window and use one of the following menu items:

* `Sago > Mesh > Make Meshes Read-Only`
* `Sago > Mesh > Make Meshes ReadWrite`


  [1]: https://sagosago.unfuddle.com/a#/projects/9/repositories/42/browse
  [2]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/browse
  [3]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Planets.unity
  [4]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Flash/picnic_export.fla
  [5]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Picnic.unity
  [6]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Flash/marshmallow.fla
  [7]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Marshmallow.unity
  [8]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Marshmallow.unity
  [9]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Picnic.unity
  [10]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Picnic.unity
  [11]: https://sagosago.unfuddle.com/a#/projects/9/repositories/44/file?path=/Assets/Scenes/Marshmallow.unity