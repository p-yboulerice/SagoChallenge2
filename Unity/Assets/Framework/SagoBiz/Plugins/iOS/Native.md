# Native Assets

SagoBiz contains native assets that need to be included in the Xcode project for iOS builds  (i.e. frameworks, bundles, plists, Objective-C source, etc). Since Unity ignores hidden files, we prevent them from being imported by putting them in the hidden `.Native` directory.

To open the hidden `.Native` directory in the Finder, you can browse to this directory, press `Command + Shift + G`, type `.Native` and press `Go`.

__________________________________________________________
If you want to modify or rebuild the native xcode project
==========================================================

UPDATE:
- In order to enable bitcode, you need to add -fembed-bitcode compiler flag in 
  library's Build Settings -> Apple LLVM Custom Compiler Flags -> Other C Flags and Other C++ Flags.
  Note that the framework compiles fine without this flag, but the bitcode data for all architectures won't be included and you will face issues [1] when archiving the final Unity Xcode project.

- Open the SagoBiz.xcodeproj within the src folder.
- Switch to either `Framework - Debug` or `Framework - Release` targets (Next to the stop button).
- Clean and build the project ( Command + Shift + K, then Command + B ).

This generate a SagoBiz.framework directory somewhere in you computer with a random string attached to its path (Thanks Apple!). If you actually browse to Products -> SagoBiz.a in your project hierarchy you should be able to see the full path and have an idea of where to look for the file.
In our test cases the pattern for the path looks like this:

/Users/YOUR_USER_NAME/Library/Developer/Xcode/DerivedData/SagoBiz-SOME_RANDOM_STRING/Build/Products/Release-iphoneuniversal

/Users/YOUR_USER_NAME/Library/Developer/Xcode/DerivedData/SagoBiz-SOME_RANDOM_STRING/Build/Products/Debug-iphoneuniversal

- Copy the .framework to .Native folder in your project and replace it with the original file.

It is recommended to clean the Unity xcode project before making a build.


References:
[1] Error when archiving: ld: bitcode bundle could not be generated because '/Users/aram/src/sagoapptest/Unity/Assets/External/SagoBiz/Plugins/iOS/.Native/SagoBiz.framework/SagoBiz(MPClassDescription.o)' was built without full bitcode. All object files and libraries for bitcode must be generated from Xcode Archive or Install build for architecture arm64