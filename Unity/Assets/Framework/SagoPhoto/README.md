# SagoPhoto User Guide

* Author: [John Park](mailto:jpark@sagosago.com) / [Luke Lutman](mailto:llutman@sagosago.com) / [Peter Hamilton](mailto:phamilton@sagosago.com)
* Version: 1.0
* Unity: 5.6.3f1
* Dependencies: [SagoEasing][1]

----------


## Overview

SagoPhoto is a library of classes used for capturing textures from a camera and saving them to the camera roll. In addition to this core functionality, there is a PhotoFlash class for triggering a flash effect and a set of presets for post-processing effects.

----------


## Examples

Check out the [SagoPhotoTest][2] project to see how to use the SagoPhoto submodule.

----------


## Requirements

* Git
* Unity 5.6.3f1+
* Git Account

----------


## Installing SagoPhoto

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:
 
    `cd /path/to/unity/project`

3. Initialize a new git repository:
    
    `git init`
    
4. Add SagoPhoto as a submodule:

    `git submodule add git@github.com:SagoSago/sago-photo.git Assets/External/SagoPhoto`

5. Similarly, add the SagoEasing submodule:

	`git submodule add git@sagosago.unfuddle.com:sagosago/sagoeasing.git Assets/External/SagoEasing`
   
6. Switch back to Unity. You should see the SagoPhoto folder in the project in `External/SagoPhoto`.

----------


## General Usage

Make a new instance of the preset you'd like to use, and set any properties you require. Now find your instance of `PhotoCamera` in the scene and call the `TakePhoto` method passing in `PhotoWidth`, `PhotoHeight` and `OnPostRender` from the preset. Additionally you'll want to write a method to pass it in as an argument for the `onComplete` parameter. If you want to capture the photo to device, call the static method `CameraRoll.Save` and it will save the photo, cross-platform.

Note that using a preset isn't required, and passing in null instead of `OnPostRender` will return a plain capture, with whatever you specify for the height and width.

After calling the `PhotoCamera.TakePhoto` method you'll likely want to call `PhotoFlash.Create` to instantiate the `PhotoFlash` effect.

------------

## Presets

The `CameraRollPhotoPreset` applies the SagoMini logo and crops the photo to a standard resolution which should be determined by the speed of the device taking the photo. All of the properties are set to the default standard and don't need to be changed unless specified by the designer.

`MeshOverlay` is similar to the `CameraRollPhotoPreset` except you pass in your own mesh that you want overlayed on top of the photo. The CropToMesh property will automatically set the `PhotoWidth` & `PhotoHeight` properties such that they match the mesh.

The `MaterialPreset` allows you to apply effects with using a shader via a material. Create a material and set it to the shader of your choosing. Once your material is configured using the inspector or with code, set is at the `Material` property on the instance of the `MaterialPreset` that you've created.

If you need to use multiple presets, try using the `MultiPreset` class. It's `Presets` property allows you to add instances of objects that have implement `IPhotoPreset` and calls their `OnPostRender` method in the order in which they are added.

------------

  [1]: https://sagosago.unfuddle.com/a#/repositories/54/browse
  [2]: https://github.com/SagoSago/sago-photo-test
  