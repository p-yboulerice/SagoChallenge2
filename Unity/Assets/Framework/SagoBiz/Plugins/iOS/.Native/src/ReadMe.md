# SagoBiz.framework User Guide

Authors: [Aram Azhari](aram@sagosago.com), [Luke Lutman](luke@sagosago.com)  
Version: 1.0  
Unity: 4.6.7
Xcode: 6.4  
Dependencies: [Mixpanel][1], [WebViewJavascriptBridge][2]



### How To Build SagoBiz.framework

1. Open `Plugins/iOS/.Native/src/SagoBiz.xcodeproj`
1. Select the `Framework - Release` scheme
1. Choose `Product > Build`

Every time you build any of these targets:

- `Framework - Debug`
- `Framework - Release`
- `Test` 

The bindings and framework:

- `SagoBiz.h`
- `SagoBiz.mm`
- `SagoBiz.framework`

Will be copied from the build folder to:

- `Plugins/iOS/.Native` 
	This is the version of the framework that will be committed to the SagoBiz repo and used by all of our apps.
- `Plugins/iOS/.Native/src/Test`
	This is the version of the framework that will be used by the test app to make sure the bindings and framework compile correctly.

__IMPORTANT__: Please make sure you only commit the release version of the framework to the SagoBiz repo!



### How To Update Mixpanel

mixpanel-iphone
v2.8.1
https://github.com/mixpanel/mixpanel-iphone/releases/tag/v2.8.1

Mixpanel has resources (images, storyboards, json, etc.) related to the survey 
and feedback features. We're not using any of those features at the moment, 
but to keep everything clean, we're packaging all of those resources into 
SagoBiz.bundle just in case. 

So, when you update Mixpanel, you need to:

1. Add any new resources to `SagoBiz.bundle`.
	
	You'll need to dig around to make sure any resources (`.png`, `.nib`, 
	`.storyboard`, `.xcassets`, `.json`, etc.) used by Mixpanel have been 
	added to the `Bundle` target.
	
2. Update the Mixpanel code to load from `SagoBiz.bundle`.
	
	You'll need to dig around to find and fix each place where Mixpanel is 
	loading or referencing a resource. Here are the changes we've had to 
	make so far... (not tested!)
		
	- Mixpanel.m
	
			// before
			[UIStoryboard storyboardWithName:@"MPNotification" bundle:nil];
			[UIStoryboard storyboardWithName:@"MPSurvey" bundle:nil];
	
			// after
			[UIStoryboard storyboardWithName:@"SagoBiz.bundle/MPNotification" bundle:nil];
			[UIStoryboard storyboardWithName:@"SagoBiz.bundle/MPSurvey" bundle:nil];
	
	- MPABTestDesignerSnapshotRequestMessage.m
		
			// before
			[[NSBundle mainBundle] URLForResource:@"snapshot_config" withExtension:@"json"]
	
			// after
			[[NSBundle mainBundle] URLForResource:@"SagoBiz.bundle/snapshot_config" withExtension:@"json"]
	
	- MPNotification.storyboard
	
			// before
			<resources>
				<image name="MPCloseBtn" width="25" height="25"/>
			</resources>
		
			// after
			<resources>
				<image name="SagoBiz.bundle/MPCloseBtn" width="25" height="25"/>
			</resources>
		
	- MPSurvey.storyboard
	
			// before
			<resources>
				<image name="MPArrowLeft" width="24" height="16"/>
				<image name="MPArrowRight" width="24" height="16"/>
				<image name="MPCheckmark" width="13" height="14"/>
				<image name="MPDismissKeyboard" width="30" height="14"/>
				<image name="MPLogo" width="66" height="22"/>
			</resources>
		
			// after
			<resources>
				<image name="SagoBiz.bundle/MPArrowLeft" width="24" height="16"/>
				<image name="SagoBiz.bundle/MPArrowRight" width="24" height="16"/>
				<image name="SagoBiz.bundle/MPCheckmark" width="13" height="14"/>
				<image name="SagoBiz.bundle/MPDismissKeyboard" width="30" height="14"/>
				<image name="SagoBiz.bundle/MPLogo" width="66" height="22"/>
			</resources>



### How To Update WebViewJavascriptBridge

WebViewJavascriptBridge
v4.1.4
https://github.com/marcuswestin/WebViewJavascriptBridge/tree/v4.1.4/WebViewJavascriptBridge

When you update WebViewJavascriptBridge, you need to:

1. Add `WebViewJavascriptBridge.js.txt` to `SagoBiz.bundle`.

	To allow WebViewJavascriptBridge to load the javascript code from 
	`WebViewJavascriptBridge.js.txt` at runtime, the text file has to be 
	included in `SagoBiz.bundle`. Make sure `WebViewJavascriptBridge.js.txt` 
	has been added to the `Bundle` target.




  [1]: https://github.com/mixpanel/mixpanel-iphone/releases/tag/v2.8.1
  [2]: https://github.com/marcuswestin/WebViewJavascriptBridge/tree/v4.1.4/WebViewJavascriptBridge