# SagoLocalization User Guide

Author: [Luke Lutman](luke@sagosago.com)
Version: 1.0
Unity: 5.4.5
Dependencies: [SagoCore][1]


----------

## Overview

SagoLocalization is our library for localizing text and resources in Unity.


----------

## Examples

All of the example below can be found in the [SagoLocalizationTest][2] project, which has Unity scenes and `C#` code so you can see SagoLocalization in action. 


----------

## Requirements

* Git
* Github account
* Unity 5.4.5+ 


----------

## Install

1. Create a new Unity project.

1. Open the Terminal and change to the project directory:

	`cd /path/to/unity/project`

1. Initialize a new git repository:
	
	`git init`
	
1. Add SagoLocalization as a submodule:
	
	`git submodule add --branch master git@sagosago.unfuddle.com:sagosago/sago-localization.git Unity/Assets/Framework/SagoLocalization`
	
1. Switch back to Unity.

1. Open the Localization Window:
	
	`Window > Sago > Localization`


---

## Identifiers

Identifiers have three parts:

* LanguageCode
	
	A two or three letter string that identifies the base language (see: [ISO 639-2](https://en.wikipedia.org/wiki/List_of_ISO_639-2_codes)).
		
		en = English
		fr = French
		haw = Hawaiian
	
* ScriptCode
	
	A four letter script code that identifies the script used to write the language (see: [ISO 15924](https://en.wikipedia.org/wiki/ISO_15924)).
		
		zh-Hans = Simplified Chinese
		zh-Hant = Traditional Chinese

* CountryCode
	
	A two letter country code that identifies a country-specific variant of the language (see: [ISO 3166-2](https://en.wikipedia.org/wiki/ISO_3166-2))
		
		en-CA = Canadian English
		pt-BR = Brazilian Portugese
		zh-Hant-HK = Hong Kong Traditional Chinese

## Language

Languages (represented by the `Language` struct) are used to describe _what_ a user can read.

The `Language` struct only has an `Identifier`. 

The identifier has three parts – `LanguageCode` is required, `ScriptCode` and `CountryCode` are optional.

	zh  = Chinese (Least Specific)
	zh-Hant = Traditional Chinese (More Specific)
	zh-Hant-HK = Traditional Chinese in Hong Kong (Most Specific)


---

## Locale

Locales (represented by the `Locale` struct) are used to represent _where_ a user is located.

The `Locale` struct has an `Identifier` and an array of `PreferredLanguages`.

The identifier has three parts – `LanguageCode` and `CountryCode` are required, `ScriptCode` is optional:
	
	en-CA = Canada
	pt-BR = Brazil
	zh-CN = China
	
The preferred languages array contains languages in order of most-preferred to least-preferred:

	en = English (Most Preferred)
	fr = French
	zh-Hant (Least Preferred)


---

## Language vs Locale

Although they look similar, languages and locales are conceptually different:

* Language is about what the user can understand
* Locale is about where the user is located

You can think of it as the difference between "I'm Australian" and "I'm in Australia":

If I'm Australian:

* I'll understand what you mean when you say _"G'day, mate!"_ (doesn't matter where I'm located)
* I could be located anywhere in the World
* I should see prices in the local currency, not in AUD

If I'm in Australia:

* I might not understand _"G'day, mate!"_ (maybe I only speak German)
* I'm definitely located __in__ Australia and not anywhere else
* I should see prices in AUD (the local currency), not in EUR (German currency)

Below are some examples of possible locale and language preferences:

* The American:
	
		identifier: en-US
		preferred languages: en
	
	I'm located in the United States and I understand English.

* The Aussie:
	
		identifier: en-AU
		preferred languages: en-AU, en
	
	I'm located in Australia and I prefer Australian English (_G'day mate!_) when it's available.

* The World Traveller:
	
		identifier: zh-HK
		preferred languages: fr-CA, fr-FR, fr, zh-Hant, en
		
	I'm located in Hong Kong, but I'm from Montréal. 
	I prefer Canadian French over French. 
	If French isn't available, I'd prefer Traditional Chinese. 
	If all else fails, I understand English.

When displaying localized content, we look for a version in the user's most-preferred 
language. If we don't have a version in that language, we fallback to their next 
most-preferred language, and so on. If we don't find anything, we display English 
content.


---

## Locale.Current

To get a `Locale` object based on the user's current settings, use the `Locale.Current` property.

Under the hood, `Locale.Current` uses the `LocaleProvider` class to:

1. Check the player prefs
1. Check the device settings (ios, android) or the editor prefs (editor)
1. Fall back to `en-US` and `en`

In the editor, you can modify the player and editor prefs using the `Localization Window` (`Window > Sago > Localization`). 
At runtime, you can override the current locale by assigning a new `Locale` object to `Locale.Current` (the new value will 
be stored in the player prefs).


---

## LocalizedResourceReference

The `LocalizedResourceReference` class is ScriptableObject subclass that serializes a 
dictionary of language identifiers and asset guids for one asset in multiple languages. 
At runtime, the dictionary is used to find and load the appropriate asset for the 
current language.

`SagoLocalization` implements two concrete subclasses of `LocalizedResourceReference`:

* `LocalizedStringReference` for localizing strings
* `LocalizedPrefabReference` for localizing prefabs

If you need to localize other types of assets (i.e. `AudioClip`, `MeshAnimation`, etc.), 
you have to implement your own `LocalizedResourceReference` subclasses (it's easy and 
avoids creating a bunch of dependencies in `SagoLocalization`).


---

## LocalizedStringReference

`LocalizedStringReference` is used for localizing strings.

To localize one or more strings:

1. Create a `LocalizedResourceReference` asset:
	
	1. Go to the `Assets/Project/Resources` folder in the `Project` window.
	1. Right-click and choose `Create > Localized String Reference`.
	1. Rename the asset (i.e. `Strings`)
	
1. Find the string you want to localize in your C# source code.
	
	```C#
	m_Text.text = "Hello, world!";
	```
	
1. Wrap the string in with `GetString()` or `GetStringFormat()`:
	
	```C#
	m_Text.text = m_LocalizedStringReference.GetString("hello_world", "Hello, world!");
	m_Text.text = m_LocalizedStringReference.GetStringFormat("hello_x", "Hello, {0}!", "world");
	```
	
	A few things to note here:
	
	1. You'll need a reference to the `Strings` asset in order to use `GetString()`. 
	Usually, you'll create a serialized field and drag in a reference in the editor.
	
	1. You need to provide a key as the first argument to `GetString()`. The key 
	identifies a specific translation of the string. If you want to use the same 
	translation in multiple places, use the same key. If you want use different 
	translations in different places, use unique keys.
	
	1. `GetStringFormat()` works just like `string.Format()` and uses every 
	argument after the key and value as replacements.
	

1. Extract the strings from your C# source code:
	
	1. Select the `Strings` asset in the `Project` window.
	1. Click on the context menu in the top-right corner of the inspector.
	1. Choose `Extract...`
	1. Select the folder containing your source code (i.e. `Assets/Project/Scripts`).
	
	Extracting strings will:
	
	* scan your source code for calls to `GetString()` or `GetStringFormat()`
	* generate the English `json` asset (using the string in the source code as the English translation)
	* assign it to the `en` slot in the `Strings` asset
	
	You can manually create and edit the `json` file, but extracting the strings is a lot easier :-)
	
	
1. Export the strings to `csv` for translation:
	
	1. Select the `Strings` asset in the `Project` window.
	1. Click on the context menu in the top-right corner of the inspector.
	1. Choose `Export...`
	1. Choose which languages to export – if you're not sure, just choose `Everything`.
	1. Choose the `csv` file – if you don't already have a `csv` file, create an empty one and select it.
	
	
1. Wait until translations are ready...
	
	At this point, the code is fully functional – you'll just see English if you switch to another language.
	
	
1. Import the strings from `csv` after translation:
	
	1. Select the `Strings` asset in the `Project` window.
	1. Click on the context menu in the top-right corner of the inspector.
	1. Choose `Import...`
	1. Choose which languages to import – if you're not sure, just choose `Everything`.
	1. Choose the `csv` file.
	
	Importing strings will:
	* create or update the `json` assets for each language in the `csv` file
	* assign the `json` asset for each language to the appropriate slot in the `Strings` asset.


1. Test the translations:
	
	1. Open the `Localization` window from the menu: `Window > Sago > Localization`
	1. Expand `Player Prefs > Preferred Languages`
	1. Set `size` to 1.
	1. Add the language identifier for the language you want to test (i.e. `ja` for Japanese).
	1. Run your code, you should see your translation!
	

---

## Pluralization

When you're localizing strings, you often need different translations based on a number.

For example, in English, Saying _"I have 1 apples."_ is weird! Instead, you should use something like:

* 1 = I have one apple.
* 17 = I have 17 apples.

Other languages have different and much more complicated rules – Polish has six different plural forms! (see: [Localization and Plurals][8]). 

The `GetPluralString()` and `GetPluralStringFormat()` methods allow you to provide different translations based on a number. `GetPluralString()` use the third argument to determine which plural form to use (but doesn't do any substitution). `GetPluralStringFormat()` use the third argument to determine which plural form to use and the fourth argument for substitution.

```C#
m_Text.text = m_LocalizedStringReference.GetPluralString("i_have_apples", "I have {0} apple(s).", 1); // I have {0} apple.
m_Text.text = m_LocalizedStringReference.GetPluralString("i_have_apples", "I have {0} apple(s).", 17); // I have {0} apples.
m_Text.text = m_LocalizedStringReference.GetPluralStringFormat("i_have_apples", "I have {0} apple(s).", 1, 1); // I have one apple.
m_Text.text = m_LocalizedStringReference.GetPluralStringFormat("i_have_apples", "I have {0} apple(s).", 17, 17); // I have 17 apples.
```


---

## Json

`LocalizedStringReference` objects map language identifiers to string catalogs – `json` 
assets that contain metadata for one or more strings in a particular language. For 
each set of strings you want to localize, you'll have one `LocalizedStringReference` 
asset (see [Strings.asset][5]) and a `json` asset for each language you plan to support 
(see [Strings.en.json][6], [Strings.ja.json][7]). 


__Important:__ The `json` assets will not be serialized into the `Strings` asset, 
they must be in an asset bundle or a `Resources` folder so they can be loaded at 
runtime.

The structure of the `json` data looks like this:

```json
{
	"localizedStrings": [
		{
			"reference": "Assets/Project/Scripts/StringSceneController.cs:37",
			"comment": "Hello, World!",
			"key": "StringSceneController.Text.Simple",
			"plural": false,
			"values": [
				"Hello, World!"
			]
		},
		{
			"reference": "Assets/Project/Scripts/StringSceneController.cs:43",
			"comment": "I ate {0} shiny red apple(s) today!",
			"key": "StringSceneController.Text.Plural",
			"plural": true,
			"values": [
				"I ate {0} shiny red apple today!",
				"I ate {0} shiny red apples today!"
			]
		}
	]
}
```


---

## LocalizedPrefabReference

`LocalizedPrefabReference` is used for localizing prefabs.

To localize a prefab:

1. Create a `LocalizedPrefabReference` asset:
	
	1. Go to the `Assets/Project/Resources` folder in the `Project` window.
	1. Right-click and choose `Create > Localized Prefab Reference`.
	1. Rename the asset (i.e. `Prefab`)
	
1. Find the localized versions of the prefab in your Unity project and drag them into the corresponding slot in the `Prefab` asset:
	
	```
	en -> Assets/Project/Resources/Prefab.en.prefab
	ja -> Assets/Project/Resources/Prefab.ja.prefab
	...
	```
	
	__Important:__ The localized versions of the prefab are not serialized into the `Prefab` 
	asset, they must be in an asset bundle or a `Resources` folder so they can be 
	loaded at runtime.
	
1. Refactor the code to load a localized version of the prefab from the `Prefab` asset:
	
	```C#
	LocalizedResourceReferenceLoaderRequest<GameObject> request;
	request = m_PrefabReference.LoadAsync();
	yield return request;
	
	GameObject instance;
	instance = Instantiate(request.asset);
	```
	
	_Note:_ You'll need a reference to the `Prefab` asset in order to use `LoadAsync()`. 
	Usually, you'll create a serialized field and drag in a reference in the editor.


---

## LocalizedAudioClipReference

`LocalizedAudioClipReference` is used for localizing prefabs.

To localize a prefab:

1. Create a `LocalizedAudioClipReference` asset:
	
	1. Go to the `Assets/Project/Resources` folder in the `Project` window.
	1. Right-click and choose `Create > Localized AudioClip Reference`.
	1. Rename the asset (i.e. `AudioClip`)
	
1. Find the localized versions of the audio clip in your Unity project and drag them into the corresponding slot in the `AudioClip` asset:
	
	```
	en -> Assets/Project/Resources/AudioClip.en.aif
	ja -> Assets/Project/Resources/AudioClip.ja.aif
	...
	```
	
	__Important:__ The localized versions of the audio clip are not serialized into 
	the `AudioClip` asset, they must be in an asset bundle or a `Resources` folder 
	so they can be loaded at runtime.
	
1. Refactor the code to load a localized version of the audio clip from the `AudioClip` asset:
	
	```C#
	LocalizedResourceReferenceLoaderRequest<AudioClip> request;
	request = m_AudioClipReference.LoadAsync();
	yield return request;
	
	AudioSource source;
	source = GetComponent<AudioSource>();
	source.clip = request.asset;
	source.Play();
	```
	
	_Note:_ You'll need a reference to the `AudioClip` asset in order to use `LoadAsync()`. 
	Usually, you'll create a serialized field and drag in a reference in the editor.


---


---

  [1]: https://github.com/SagoSago/sago-core
  [2]: https://github.com/SagoSago/sago-localization-test
  [3]: https://github.com/SagoSago/sago-localization-test/blob/master/Unity/Assets/Project/Scenes/String.unity
  [4]: https://github.com/SagoSago/sago-localization-test/blob/master/Unity/Assets/Project/Scripts/StringSceneController.cs
  [5]: https://github.com/SagoSago/sago-localization-test/blob/master/Unity/Assets/Project/Resources/Strings.asset
  [6]: https://github.com/SagoSago/sago-localization-test/blob/master/Unity/Assets/Project/Resources/Strings.en.json
  [7]: https://github.com/SagoSago/sago-localization-test/blob/master/Unity/Assets/Project/Resources/Strings.ja.json
  [8]: https://developer.mozilla.org/en-US/docs/Mozilla/Localization/Localization_and_Plurals
  [9]:  