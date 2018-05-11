# SagoDrawing User Guide
  
* Author: [Luke Lutman](mailto:llutman@sagosago.com)
* Version: 1.0
* Unity: 5.3.5
* Dependencies: [SagoTouch][1]


----------
  

## Overview
  
SagoDrawing is a library of components and materials used to create a render texture in which one can draw on.
  
  
----------
  
  
## Examples
  
An example project for SagoDrawing can be found in the [SagoDrawingTest][2] project, which has a Unity scene showing how to setup SagoDrawing in the hierarchy.
  
  
----------
  
  
## Requirements
  
* Git
* Unity 5.3.5+ 
* Unfuddle account
  
  
----------
  
  
## Installing SagoDrawing
  
1. Create a new Unity project.
2. Open the Terminal and change to the project directory:
  
    `cd /path/to/unity/project`
  
3. Initialize a new git repository:
     
    `git init`
     
4. Add SagoDrawing as a submodule:
  
    `git submodule add git@github.com:SagoSago/sago-drawing.git Assets/External/SagoDrawing`
  
5. Similarly, add the SagoTouch submodules:

    `git submodule add git@sagosago.unfuddle.com:sagosago/sagotouch.git Assets/External/SagoTouch`
    
6. Switch back to Unity. You should see the SagoDrawing folder in the project in `External/SagoDrawing`.
  
  
----------
  
  
## Usage
  
Once you've setup the hierarchy (see [SagoDrawingTest][2] project for an example) set the material to either DrawingSurface or DrawingSurfaceStencilled depending on your needs. The main properties to be aware of are the: DrawingSurface.Color & LineDrawingTool.Color, used for setting the color. These can be set in the inspector or with code. If you change the surface color during runtime, use the DrawingSurface.Clear() method to clear and set the new color.

The DrawingController component attached to the root object uses [SagoTouch][1] to capture touches. You can set the Priority & SwallowTouches property on the DrawingController component via the inspector. Additionally, if you need to know when the drawing began or ended, subscribe to the DrawingBegan & DrawingEnded events.

## Bonus Points

Write your own custom drawing tool! Use the LineDrawingTool as a reference and add it to the submodule for the benefit of all :-)

  
------------
  
  [1]: https://sagosago.unfuddle.com/a#/repositories/40/browse
  [2]: https://github.com/SagoSago/sago-drawing-test
