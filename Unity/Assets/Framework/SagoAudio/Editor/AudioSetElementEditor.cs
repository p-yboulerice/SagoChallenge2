namespace SagoAudioEditor {

	using SagoAudio;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(AudioSetElement))]
	public class AudioSetElementEditor : Editor {


		#region Serialized Properties

		SerializedProperty AudioClip;
		SerializedProperty AudioSource;
		SerializedProperty Loop;
		SerializedProperty MaxChannels;
		SerializedProperty Type;
		SerializedProperty UseSourceClip;
		SerializedProperty UseSourceLoop;
		SerializedProperty UseSourceVolume;
		SerializedProperty Volume;

		#endregion


		#region Private Properties

		private AudioSetElement Target {
			get { return (AudioSetElement)this.target; }
		}

		#endregion


		#region Editor

		private void OnEnable() {
			this.AudioClip = serializedObject.FindProperty("AudioClip");
			this.AudioSource = serializedObject.FindProperty("AudioSource");
			this.Loop = serializedObject.FindProperty("Loop");
			this.MaxChannels = serializedObject.FindProperty("MaxChannels");
			this.Type = serializedObject.FindProperty("Type");
			this.UseSourceClip = serializedObject.FindProperty("UseSourceClip");
			this.UseSourceLoop = serializedObject.FindProperty("UseSourceLoop");
			this.UseSourceVolume = serializedObject.FindProperty("UseSourceVolume");
			this.Volume = serializedObject.FindProperty("Volume");
		}

		override public void OnInspectorGUI() {

			List<AudioSetElement> elements;
			elements = new List<AudioSetElement>(this.targets.Cast<AudioSetElement>());

			serializedObject.Update();

			EditorGUILayout.PropertyField(this.MaxChannels);
			LayoutTypeGUI(elements);

			bool isClipType;
			isClipType = elements.Any(element => element.Type == AudioSetElementType.AudioClip);

			if (isClipType) LayoutClipTypeGUI(elements);
			else LayoutSourceTypeGUI(elements);

			serializedObject.ApplyModifiedProperties();

		}

		private void LayoutTypeGUI(List<AudioSetElement> elements) {
			
			EditorGUI.BeginChangeCheck();

			EditorGUI.showMixedValue = this.Type.hasMultipleDifferentValues;

			AudioSetElementType type;
			type = (AudioSetElementType)EditorGUILayout.EnumPopup("Type", this.Target.Type);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {
				
				foreach (AudioSetElement element in elements) {
					
					bool useSource;
					useSource = (element.AudioClip == null);

					Undo.RecordObject(element, "Change Type");
					element.AudioSource = element.IsSourceType ? element.AudioSource : null;
					element.UseSourceClip = useSource;
					element.UseSourceLoop = useSource;
					element.UseSourceVolume = useSource;

				}

				serializedObject.Update();
				this.Type.enumValueIndex = (int)type;

			}

		}

		private void LayoutClipTypeGUI(List<AudioSetElement> elements) {
			EditorGUILayout.PropertyField(this.AudioClip);
			EditorGUILayout.PropertyField(this.Loop);
			EditorGUILayout.PropertyField(this.Volume);
		}

		private void LayoutSourceTypeGUI(List<AudioSetElement> elements) {
			EditorGUILayout.PropertyField(this.AudioSource);
			LayoutClipGUI(elements);
			LayoutPlaybackModeGUI(elements);
			LayoutVolumeGUI();
		}

		private void LayoutClipGUI(List<AudioSetElement> elements) {

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.UseSourceClip.hasMultipleDifferentValues;

			bool toggle;
			toggle = EditorGUILayout.Toggle(!this.Target.UseSourceClip, this.ToggleLayoutOptions);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {

				foreach (AudioSetElement element in elements) {
					Undo.RecordObject(element, "Use Audio Clip");
					element.AudioClip = toggle ? element.AudioClip : null;
				}

				serializedObject.Update();
				this.UseSourceClip.boolValue = !toggle;
			
			}

			EditorGUI.BeginDisabledGroup(!toggle);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.AudioClip.hasMultipleDifferentValues;

			AudioClip clip;
			clip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", this.Target.PlaybackClip, typeof(AudioClip), true);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObjects(this.serializedObject.targetObjects, "Change Audio Clip");
				this.AudioClip.objectReferenceValue = clip;
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

		}

		private void LayoutPlaybackModeGUI(List<AudioSetElement> elements) {

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.UseSourceLoop.hasMultipleDifferentValues;

			bool toggle;
			toggle = EditorGUILayout.Toggle(!this.Target.UseSourceLoop, this.ToggleLayoutOptions);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObjects(this.serializedObject.targetObjects, "Use Source Playback Mode");
				this.UseSourceLoop.boolValue = !toggle;
			}

			EditorGUI.BeginDisabledGroup(!toggle);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.Loop.hasMultipleDifferentValues;

			AudioSetElementPlaybackMode playbackMode;
			playbackMode = (AudioSetElementPlaybackMode)EditorGUILayout.EnumPopup("Playback Mode", this.Target.PlaybackMode);

			EditorGUI.showMixedValue = false;

			if (this.Target.PlaybackMode != playbackMode) {
				Undo.RecordObject(this.Target, "Change Playback Mode");
				this.Loop.boolValue = playbackMode == AudioSetElementPlaybackMode.Loop;
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

		}

		private void LayoutVolumeGUI() {

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.UseSourceVolume.hasMultipleDifferentValues;

			bool toggle;
			toggle = EditorGUILayout.Toggle(!this.Target.UseSourceVolume, this.ToggleLayoutOptions);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObjects(this.serializedObject.targetObjects, "Use Source Volume");
				this.UseSourceVolume.boolValue = !toggle;
			}

			EditorGUI.BeginDisabledGroup(!toggle);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.UseSourceVolume.hasMultipleDifferentValues;

			float volume;
			volume = EditorGUILayout.Slider("Volume", this.Target.PlaybackVolume, 0, 1);

			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObjects(this.serializedObject.targetObjects, "Change Volume");
				this.Volume.floatValue = volume;
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

		}

		private GUILayoutOption[] ToggleLayoutOptions {
			get { return new GUILayoutOption[] { GUILayout.Width(14) }; }
		}

		#endregion


	}

}
