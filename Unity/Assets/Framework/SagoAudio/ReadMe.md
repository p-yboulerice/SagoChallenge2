# SagoAudio User Guide

Authors: [John Park](john@sagosago.com), [Luke Lutman](luke@sagosago.com)
Version: 1.0
Unity: 5.4.5
Dependencies: [SagoEasing][1]


---

## Overview

[SagoAudio][2] is our library for handling audio in Unity.


---

## Requirements

* Git
* Github account
* Unity 5.4.5+ 


---

## Install

1. Create a new Unity project.
2. Open the Terminal and change to the project directory:

	`cd /path/to/unity/project`

3. Initialize a new git repository:
	
	`git init`
	
4. Add SagoAudio, SagoCore and SagoUtils as submodules:

	`git submodule add --branch sago-world git@github.com:SagoSago/sago-audio.git Unity/Assets/Framework/SagoAudio`
	`git submodule add --branch master git@github.com:SagoSago/sago-easing.git Unity/Assets/Framework/SagoEasing`
	
5. Switch back to Unity.

---

## Examples

All of the example below can be found in the [SagoAudioTest][3] project.


---

  [1]: https://github.com/SagoSago/sago-easing
  [2]: https://github.com/SagoSago/sago-audio/tree/sago-world
  [3]: https://github.com/SagoSago/sago-audio-test