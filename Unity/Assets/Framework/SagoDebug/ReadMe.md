# SagoDebug User Guide

* Author: [Richard Hilmer](mailto:rhilmer@sagosago.com)
* Version: 1.1 (World)
* Unity: 5.4.3
* Dependencies: 
  * [`SagoLayout`](https://github.com/SagoSago/sago-layout)
  * [`SagoTouch`](https://github.com/SagoSago/sago-touch)
  * [`SagoCore`](https://github.com/SagoSago/sago-core)
  * [`SagoUtils`](https://github.com/SagoSago/sago-utils)

----------

## Overview

`SagoDebug` is a library of common debugging methods.  Its main purpose now is to provide a developer UI (`DevUI`) that works on iOS devices.  Much of the functionality is only enabled if `SAGO_DEBUG` is set.

Note: `SagoDebug` no longer wraps `UnityEngine.Debug` methods.

Note: all recent updates are in the `sago-world` branch, under the assumption that they will eventually become `master` as we migrate everything to _World_ format.

----------


## Examples

All of the example files referenced below can be found in the [`SagoDebugTest`](https://github.com/SagoSago/sago-debug-test) project, which has Unity scenes and `C#` code you can use to see SagoDebug in action. 


----------


## Requirements

* Git
* Unity 5.4.3+ 
* GitHub account

Most functionality is only enabled if `SAGO_DEBUG` is set in the *Player Settings* --> *Scripting Define Symbols* when building the project.  There is a simple menu option to change this in _Sago_ --> _Debug_ --> _Enable SAGO_DEBUG_


----------


## Installing SagoDebug

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:
 
    `cd /path/to/unity/project`

3. Initialize a new git repository:
    
    `git init`
    
4. Add the `SagoDebug` as a submodule:

    `git submodule add -b sago-world git@github.com:SagoSago/sago-debug.git Assets/External/SagoDebug`

5. Similarly, add the `SagoLayout`, `SagoTouch`, `SagoCore` and `SagoUtils` submodules:

    `git submodule add -b unity5 git@github.com:SagoSago/sago-layout.git Assets/External/SagoLayout`

    `git submodule add git@github.com:SagoSago/sago-touch.git Assets/External/SagoTouch`

    `git submodule add git@github.com:SagoSago/sago-core.git Assets/External/SagoCore`

    `git submodule add -b sago-world git@github.com:SagoSago/sago-utils.git Assets/External/SagoUtils`

    Note:  the branches are the current ones used (e.g. sago-world).  Once the sago-world branches are merged into master, you can remove the specific branch arguments.

6. Switch back to Unity. You should see the SagoDebug folder in the project in `External/SagoDebug`.


## Using the Debug Wrapper

`SagoDebug` wrappers for `UnityEngine.Debug` are now *deprecated*.  Just use the `UnityEngine.Debug` methods directly.  `Log()` methods are automatically captured and routed through the _DevUI Console_ when on a device.

`SagoDebug.Debug` still provides a few extra methods beyond those included in the `UnityEngine` version.  For example, `DrawBounds` and `DrawCross`.


## The Developer UI (DevUI)

The developer UI (`SagoDebug.DevUI`) is a singleton `MonoBehaviour` that displays various useful information about the game at run-time, even on target devices.

`DevUI` is now available regardless of whether or not `SAGO_DEBUG` is set in the project settings.  However, without it set (in shipping builds), the functionality is reduced, so even if it is activated by an end user, the damage is minimal (this was done for _World_).


### Accessing the DevUI in game

1. Press and hold in the upper left corner of the screen.  A `:-)` button will appear.
    If `SAGO_DEBUG` is enabled, you must hold for about 3 seconds.  If `SAGO_DEBUG` is not enabled, you must hold for 10 seconds.
2. Tap the `:-)` button.

After activating _DevUI_ the first time, it will open much faster for the remainder of the session.


### Pages

The _DevUI_ is split into multiple "pages", each accessible from a popup list button in the top left corner.

There are 3 pages that are always present, and developers can add custom pages to extend _DevUI_.


#### Settings Page

The Settings page is where simple configuration options for the DevUI are available.

* Scale
  * The scale of _DevUI_ will automatically increase on small, high-resolution devices (e.g. iPhone 6) to improve legibility.  The Scale popup allows you to select different values.
* Max Console Messages
  * All Debug.Log messages, which normally appear in the Unity Editor Console window, are automatically captured by the _DevUI_ console.  You can limit the number that are kept in memory with this slider.
* Window Docking
  * Move the _DevUI_ to different locations on the screen.


#### External Page

This page lists all Debug OnGUIs that have been added by code, and allows them to be individually toggled on and off.

See the _Debug OnGUI_ section below for more information.


#### Console Page

This page displays all messages sent to `UnityEngine.Debug.Log()` and its variants (e.g. `LogWarning()`, `LogError`()).  

Those messages that are sent with context objects can be clicked on and will show a debug camera in the corner of the screen following the context object.  e.g. 

`Debug.Log("This is a message from my script", this);`

See: [ExampleDebugExtensions.cs](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/ExampleDebugExtensions.cs)


#### Custom Page (Single Object)

You can add custom pages to the DevUI.  For example:

```cs
using SagoDebug;

public class MyScript : MonoBehaviour {

  void Start() {
    DevUI.AddPage("My page", this, DrawMyPage);
  }

  void DrawMyPage(DevUI.DevUIPage page) {
    GUILayout.Label("Hello, world!");
  }

}
```

Pages are automatically removed when the context object (in the example, `this`, i.e. your component) is destroyed.

See: [ExampleDevUIPage.cs](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/ExampleDevUIPage.cs)


#### Custom Page (Multiple Objects)

You can add a custom page to the DevUI that refers to multiple objects.
For example, you can add a single page for all your 'pigeons'.

To do this, you must maintain a static list of the objects in question,
and then pass them to

```cs
DevUI.AddGroupPage("My Page", MyListOfObjects, DrawFunction);
```

If you just want generic info about the `GameObject`s, you can use a
default drawing method:

```cs
DevUI.AddGroupPage("My Page", MyListOfObjects, DevUI.DefaultDrawGroupPage);
```

See: [ExampleDevUIGroupPage.cs](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/ExampleDevUIGroupPage.cs)


### Debug OnGUI

If you want to add a GUI outside of `DevUI`, i.e. when you would normally use an `OnGUI` method in your script, instead, do the following:

```cs
using SagoDebug;

public class MyScript : MonoBehaviour {

  void Start() {
    DevUI.AddDebugOnGUI("My testing UI", this, DebugOnGUI);
  }

  void DebugOnGUI() {
    GUILayout.Label("Hello, world!");
  }

}
```

Note that you should **remove/rename the OnGUI method** in your script so that Unity doesn't automatically run your GUI.  Add it to the DevUI and let DevUI run it, when necessary.  This way we can be confident that the code will not be run unless `SAGO_DEBUG` is set.

See: [ExampleDebugOnGUI.cs](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/ExampleDebugOnGUI.cs)


## Other Functionality


### Graphs

Some basic line and bar graphing functionality is available in `SagoDebug.Graph`.  The simplest usage is:

```cs
using SagoDebug;

public class MyCar : MonoBehaviour {

  public float Velocity { get; protected set; }

  private Graph VelocityGraph { get; set; }

  void Start() {
    this.VelocityGraph = Graph.Create("Speed", () => this.Velocity.magnitude);
  }

  void OnDestroy() {
    if (this.VelocityGraph) {
      Destroy(this.VelocityGraph.gameObject);
    }
  }

}
```

See [`TestGraph.cs`](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/TestGraph.cs) for more examples of usage.


### Labels

You can print text labels at world-space positions using `SagoDebug.Label`.  e.g.

```cs
using SagoDebug;

public class MyScript : MonoBehaviour {

  void Update() {
    Label.Draw(name, transform.position);
  }

}
```

See [`TestLabel.cs`](https://github.com/SagoSago/sago-debug-test/blob/master/Assets/Scripts/TestLabel.cs) for more examples.

---
