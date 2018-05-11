# SagoTouch User Guide

Author: [Luke Lutman](luke@sagosago.com)
Version: 1.0
Unity: 5.4.0


----------


## Overview

SagoTouch is our framework for handing touch input in Unity. The design of the code is based on the TouchDispatchers from cocos2d (the Objective-C game engine we used before switching to Unity). As such, there's a few quirks and features that we probably wouldn't implement the same way if we were to do it again today. However, as our oldest and most-widely used library, keeping SagoTouch backwards-compatible with all of our existing apps is crucial.


----------


## Installing SagoTouch

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:

    `cd /path/to/unity/project`

3. Initialize a new git repository:
    
    `git init`
    
4. Add SagoTouch as a submodule:

    `git submodule add git@github.com:SagoSago/sago-touch.git Assets/Framework/SagoTouch`
    
5. Switch back to Unity.


----------


## The Basics

The two most fundamental parts of `SagoTouch` are the `TouchDispatcher` class and the `ISingleTouchObserver` interface. 

The `TouchDispatcher` is a singleton that reads touches from Unity's `Input` class and notifies `ISingleTouchObserver` objects about touch events. 

`ISingleTouchObserver` objects register with the `TouchDispatcher` and implement touch-handling logic.


----------


## Create An Observer

When you want to handle touch input using SagoTouch, the first thing you need to do is write a `MonoBehaviour` subclass that implements the `ISingleTouchObserver` interface.

	namespace Example {
		
		using SagoTouch;
		using UnityEngine;
		using Touch = SagoTouch.Touch;
		
		public class MyTouchObserver : MonoBehaviour, ISingleTouchObserver {
			
			public bool OnTouchBegan(Touch touch) {
				// ...
				return false;
			}
			
			public void OnTouchMoved(Touch touch) {
				// ...
			}
			
			public void OnTouchEnded(Touch touch) {
				// ...
			}
			
			public void OnTouchCancelled(Touch touch) {
				// ...
			}
			
		}
		
	}

_Important:_ Notice that the `OnTouchBegan` method has a different method signature than the other `ISingleTouchObserver` methods. 

	public bool OnTouchBegan(Touch touch) {
		// ...
		return false;
	}

When `OnTouchBegan` returns `true`, it tells the `TouchDispatcher` that this observer should receive future events for the touch. When `OnTouchBegan` returns `false`, it tells the `TouchDispatcher` that this observer is not interested in the touch and should _not_ receive future events for it. 

Your observer's `OnTouchBegan` method will be called for _every_ touch, but its `OnTouchMoved`, `OnTouchEnded` and `OnTouchCancelled` methods will only be called for touches where `OnTouchBegan` returned `true`.

`OnTouchBegan` will be called _a lot_, so it's important that it's as fast and efficient as possible.


----------


## Register with the TouchDispatcher


Once you've implemented the `ISingleTouchObserver` methods, you'll need to register your observer with the `TouchDispatcher`.

	void OnEnable() {
		if (TouchDispatcher.Instance) {
			TouchDispatcher.Instance.Add(this, 0, true);
		}
	}
	
	void OnDisable() {
		if (TouchDispatcher.Instance) {
			TouchDispatcher.Instance.Remove(this);
		}
	}

The `OnEnable`/`OnDisable` methods are a great place to register with the `TouchDispatcher`. That way, you're observer won't receive touches while it's disabled or after it's been destroyed.

The first argument to the `Add` and `Remove` methods is the observer.

The second argument to the `Add` method is the priority. The higher the value, the higher the priority. The `TouchDispatcher` will call `OnTouchBegan` on each observer in priority order.

The third argument to the `Add` method is the swallow touches flag, which affects how the `TouchDispatcher` behaves when the observer's `OnTouchBegan` returns `true`. If the flag is `true`, the `TouchDispatcher` will bind the touch to the observer and will not call any other observer's `OnTouchBegan` method for that touch. If the flag is `false`, the `TouchDispatcher` will bind the touch to the observer and continue calling `OnTouchBegan` on other observers (allowing the touch to be bound to multiple observers).

_Note:_ You must make sure `TouchDispatcher.Instance` is not null because there is an edge case where it has already been destroyed when the editor is exiting play mode.


----------


## Hit Testing

Your `OnTouchBegan` method will usually perform a hit test to know if it should return `true` or `false`. The hit test checks the position of the touch against a known hit area (i.e. the collider or renderer bounds).

Since `Touch.Position` is in screen coordinates, you'll need to convert it to local or world space to in order to perform the hit test. The `CameraUtils` class some helper methods to make this easier.

	private bool HitTest(Touch touch) {
		var camera = CameraUtils.FindRootCamera(GetComponent<Transform>());
		var bounds = GetComponent<Renderer>().bounds;
		bounds.extents += Vector3.forward;
		return bounds.Contains(CameraUtils.TouchToWorldPoint(touch, GetComponent<Transform>(), camera));
	}

As mentioned above, `OnTouchBegan` (and in turn `HitTest`) are going to get called _a lot_, so we need to avoid expensive operations (like `CameraUtils.FindRootCamera()` and `GetComponent()`). We usually do this with lazy-loaded properties.

	[System.NonSerialized]
	private Camera m_Camera;
	
	[System.NonSerialized]
	private Renderer m_Renderer;
	
	[System.NonSerialized]
	private Transform m_Transform;
	
	public Camera Camera {
		get { return m_Camera = m_Camera ?? FindRootCamera(this.Transform); }
	}
	
	public Renderer Renderer {
		get { return m_Renderer = m_Renderer ?? GetComponent<Renderer>(); }
	}
	
	public Transform Transform {
		get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
	}
	
	private bool HitTest(Touch touch) {
		var bounds = this.Renderer.bounds;
		bounds.extents += Vector3.forward;
		return bounds.Contains(CameraUtils.TouchToWorldPoint(touch, this.Transform, this.Camera));
	}
	

----------


## Tracking One Touch

A common touch-handling scenario is for an observer to track a single touch.

	namespace Example {
		
		using SagoTouch;
		using UnityEngine;
		using Touch = SagoTouch.Touch;
		
		public class MyTouchObserver : MonoBehaviour, ISingleTouchObserver {
			
			
			#region Fields
			
			[System.NonSerialized]
			private Camera m_Camera;
			
			[System.NonSerialized]
			private Renderer m_Renderer;
			
			[System.NonSerialized]
			private Touch m_Touch;
			
			[System.NonSerialized]
			private Transform m_Transform;
			
			#endregion
			
			
			#region Properties
			
			public Camera Camera {
				get { return m_Camera = m_Camera ?? FindRootCamera(this.Transform); }
			}
			
			public Renderer Renderer {
				get { return m_Renderer = m_Renderer ?? GetComponent<Renderer>(); }
			}
			
			public Transform Transform {
				get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
			}
			
			#endregion
			
			
			#region Methods
	
			private bool HitTest(Touch touch) {
				var bounds = this.Renderer.bounds;
				bounds.extents += Vector3.forward;
				return bounds.Contains(CameraUtils.TouchToWorldPoint(touch, this.Transform, this.Camera));
			}
			
			private void OnEnable() {
				if (TouchDispatcher.Instance) {
					TouchDispatcher.Instance.Add(this, 0, true);
				}
			}
			
			private void OnDisable() {
				if (TouchDispatcher.Instance) {
					TouchDispatcher.Instance.Remove(this);
				}
			}
			
			#endregion
			
			
			#region ISingleTouchObserver
			
			public bool OnTouchBegan(Touch touch) {
				if (m_Touch == null && HitTest(touch)) {
					m_Touch = touch;
					return true;
				} 
				return false;
			}
			
			public void OnTouchMoved(Touch touch) {
				// ...
			}
			
			public void OnTouchEnded(Touch touch) {
				m_Touch = null;
			}
			
			public void OnTouchCancelled(Touch touch) {
				OnTouchEnded(touch);
			}
			
			#endregion
			
			
		}
		
	}

In the code above, the first touch that passes the hit test will be stored in the `m_Touch` field. Other touches will be ignored until `OnTouchEnded` or `OnTouchCancelled` is called.

_Important:_: Always implement both `OnTouchEnded` and `OnTouchCancelled` as they are called under different circumstances. If you don't implement `OnTouchCancelled`, your observer may look as though it's stopped responding to touches (because you didn't clear the `m_Touch` field).


----------


## Tracking Multiple Touches

Another common touch-handling scenario is for an observer to track multiple touches.

	namespace Example {
		
		using SagoTouch;
		using System.Collections.Generic;
		using UnityEngine;
		using Touch = SagoTouch.Touch;
		
		public class MyTouchObserver : MonoBehaviour, ISingleTouchObserver {
			
			
			#region Fields
			
			[System.NonSerialized]
			private Camera m_Camera;
			
			[System.NonSerialized]
			private Renderer m_Renderer;
			
			[System.NonSerialized]
			private List<Touch> m_Touches;
			
			[System.NonSerialized]
			private Transform m_Transform;
			
			#endregion
			
			
			#region Properties
			
			public Camera Camera {
				get { return m_Camera = m_Camera ?? FindRootCamera(this.Transform); }
			}
			
			public Renderer Renderer {
				get { return m_Renderer = m_Renderer ?? GetComponent<Renderer>(); }
			}
			
			public List<Touch> Touches {
				get { return m_Touches = m_Touches ?? new List<Touch>(); }
			}
			
			public Transform Transform {
				get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
			}
			
			#endregion
			
			
			#region Methods
	
			private bool HitTest(Touch touch) {
				var bounds = this.Renderer.bounds;
				bounds.extents += Vector3.forward;
				return bounds.Contains(CameraUtils.TouchToWorldPoint(touch, this.Transform, this.Camera));
			}
			
			private void OnEnable() {
				if (TouchDispatcher.Instance) {
					TouchDispatcher.Instance.Add(this, 0, true);
				}
			}
			
			private void OnDisable() {
				if (TouchDispatcher.Instance) {
					TouchDispatcher.Instance.Remove(this);
				}
			}
			
			#endregion
			
			
			#region ISingleTouchObserver
			
			public bool OnTouchBegan(Touch touch) {
				if (HitTest(touch)) {
					this.Touches.Add(touch);
					return true;
				} 
				return false;
			}
			
			public void OnTouchMoved(Touch touch) {
				// ...
			}
			
			public void OnTouchEnded(Touch touch) {
				this.Touches.Remove(touch);
			}
			
			public void OnTouchCancelled(Touch touch) {
				OnTouchEnded(touch);
			}
			
			#endregion
			
			
		}
		
	}

In the code above, the all touches that passes the hit test will be stored a list.

_Important:_: Always implement both `OnTouchEnded` and `OnTouchCancelled` as they are called under different circumstances. If you don't implement `OnTouchCancelled`, your observer may look as though it's responding to touches that no longer exist (because you didn't remove them from the list).


----------


## TouchArea and TouchAreaObserver

Since hit testing against a collider or renderer is such a common use case, we've added higher level `TouchArea` and `TouchAreaObserver` classes to make it easier. 

The `TouchArea` class handles all of the hit testing and tracks touches. The `TouchAreaObserver` class allows you to implement and assign a subset of touch-handling methods. 

To get started, add a `TouchArea` and `TouchAreaObserver` component to a game object in your scene and configure them appropriately. Then implement a class that implements and assigns one or more of the `TouchAreaObserver`'s delegate methods.

	namespace Example {
		
		public class MyButton : MonoBehaviour {
		
		
			#region Fields
			
			[System.NonSerialized]
			private TouchArea m_TouchArea;
			
			[System.NonSerialized]
			private TouchAreaObserver m_TouchAreaObserver;
			
			#endregion
			
			
			#region Properties
			
			public TouchArea TouchArea {
				get { return m_TouchArea = m_TouchArea ?? GetComponent<TouchArea>(); }
			}
			
			public TouchAreaObserver TouchAreaObserver {
				get { return m_TouchAreaObserver = m_TouchAreaObserver ?? GetComponent<TouchAreaObserver>(); }
			}
			
			#endregion
			
			
			#region Methods
			
			private void OnEnable() {
				this.TouchAreaObserver.TouchUpDelegate = OnTouchUp;
			}
			
			private void OnDisable() {
				this.TouchAreaObserver.TouchUpDelegate = null;
			}
			
			public OnTouchUp(TouchArea touchArea, Touch touch) {
				Debug.Log("Touch Up!", this);
			}
			
			#endregion
			
			
		}
		
		
	}

When you're using a `TouchArea`, you make it stop responding to touches at any time by disabling the `TouchArea` component:

	void DisableTouchInput() {
		this.TouchArea.enabled = false;
	}
	
	void EnableTouchInput() {
		this.TouchArea.enabled = true;
	}


----------


## Disabling Touch Input

You can stop responding to all touch input at any time by disabling the `TouchDispatcher` component:

	void DisableAllTouchInput() {
		if (TouchDispatcher.Instance) {
			TouchDispatcher.Instance.enabled = false;
		}
	}

When you disable the `TouchDispatcher` component, it will cancel all touches (and notify any observers). Once it's disabled, the `TouchDispatcher` will ignore any touch input until it's reenabled:
	
	void EnableAllTouchInput() {
		if (TouchDispatcher.Instance) {
			TouchDispatcher.Instance.enabled = true;
		}
	}



























