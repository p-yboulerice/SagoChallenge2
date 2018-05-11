**SagoApp Android Native project:**

The .Native folder contains an Android Studio project that implements native functionality for SagoPermission, SagoApp Bootstrap, Android Immersive Mode as well as GooglePlay Expansion Downloader

Currently the only exposed library to Unity is the control of immersive mode through Scripts/Utils/ImmersiveModeUtil.cs.

The Android Studio project consist of three modules:

 1. sagoPermission: This is the first activity that runs and checks for the permissions of the app depending on what was request through AndroidManifest
 
 1. sagoApp: Which holds the implementation for Bootstrap (src/main/java/com/sagosago/Bootstrap) and Immersive Mode (src/main/java/com/sagosago/Immersive)
 
 1. googlePlayDownloader: Which contains the implementation of googleplay expansion downloader by Google (src/main/java/com/google/*) as well as ours (src/main/java/com/sagosago/*). 

Note that the googleplaydownloder runs after sagopermission on GooglePlay platform but is skipped in any other Android platform.

The Application flow is shown in: 

Android Application LifeCycle.pdf

**For GooglePlay:**

*PermissionActivity **->** GooglePlayDownloaderActivity **->** BootStrapActivity **->** UnityNativePlayerActivity*

**For other Android platforms:**

*PermissionActivity **->** BootStrapActivity **->** UnityNativePlayerActivity**

*For more info on how the permission request works please refer to docs in .Native folder.*