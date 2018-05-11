namespace SagoAudio {

	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Automatically sets all the Sago default audio settings
	/// when a new audio file is added to the project.
	/// </summary>
	public class AudioClipPostProcessor : AssetPostprocessor {


		#region AssetPostprocessor Methods

		public void OnPostprocessAudio(AudioClip audioClip) {
			
			var importer = AssetImporter.GetAtPath(assetPath);
			var serializedObj = new SerializedObject(importer);

			// Getting all the serialized properties

			var forceToMonoProp = serializedObj.FindProperty("m_ForceToMono");
			var normalizeProp = serializedObj.FindProperty("m_Normalize");
			var loadInBackgroundProp = serializedObj.FindProperty("m_LoadInBackground");
			var preloadAudioData = serializedObj.FindProperty("m_PreloadAudioData");
			var defaultSettingsProp = serializedObj.FindProperty("m_DefaultSettings");
			var loadTypeProp = defaultSettingsProp.FindPropertyRelative("loadType");
			var compressionFormatProp = defaultSettingsProp.FindPropertyRelative("compressionFormat");
			var qualityProp = defaultSettingsProp.FindPropertyRelative("quality");
			var sampleRateSettingProp = defaultSettingsProp.FindPropertyRelative("sampleRateSetting");

			// Setting the global settings

			forceToMonoProp.boolValue = true;
			normalizeProp.boolValue = false;
			loadInBackgroundProp.boolValue = false;

			// Setting the default settings

			loadTypeProp.intValue = 1;
			preloadAudioData.boolValue = true;
			compressionFormatProp.intValue = 1;
			qualityProp.floatValue = 0.01f;
			sampleRateSettingProp.intValue = 0;

			serializedObj.ApplyModifiedProperties();

		}

		#endregion


	}

}