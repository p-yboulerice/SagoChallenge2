# Native Assets

SagoBuild contains native assets that need to be included in the Xcode project for iOS builds  (i.e. frameworks, bundles, plists, Objective-C source, etc). Since Unity ignores hidden files, we prevent them from being imported by putting them in the hidden `.Native` directory.

To open the hidden `.Native` directory in the Finder, you can browse to this directory, press `Command + Shift + G`, type `.Native` and press `Go`.