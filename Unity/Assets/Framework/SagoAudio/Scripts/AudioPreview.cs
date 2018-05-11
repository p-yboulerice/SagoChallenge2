namespace SagoAudio {
	
	#if UNITY_EDITOR

	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public class AudioPreview : MonoBehaviour {
		
	}
	
	[InitializeOnLoad]
	public static class AudioPreviewHelper {
	
		static AudioPreviewHelper() {
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				EditorApplication.update += Update;
			#endif
		}

		static List<AudioPreview> ActiveAudioPreviews;

	
		public static AudioPreview Play(AudioClip clip, float pitch, float volume, bool loop = false) {
		
			GameObject gameObject;
			gameObject = new GameObject();
			gameObject.name = "AudioPreview";
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			// gameObject.hideFlags = HideFlags.DontSave;
			gameObject.AddComponent<AudioSource>();
		
			AudioSource audioSource;
			audioSource = gameObject.GetComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.loop = loop;
			audioSource.pitch = pitch;
			audioSource.volume = volume;
			audioSource.Play();
			
			AudioPreview audioPreview;
			audioPreview = gameObject.AddComponent<AudioPreview>();

			ActiveAudioPreviews.Add(audioPreview);

			return audioPreview;
		
		}
		
		public static void Stop() {
			foreach (AudioPreview audioPreview in ActiveAudioPreviews) {

				if (!audioPreview) {
					continue;
				}

				AudioSource audioSource;
				audioSource = audioPreview.GetComponent<AudioSource>();
				
				if (audioSource) {
					audioSource.Stop();
				}
				
				Object.DestroyImmediate(audioPreview.gameObject);
				
			}
			ActiveAudioPreviews.Clear();
		}

		static void Update() {
			
			if (ActiveAudioPreviews == null) {
				
				ActiveAudioPreviews = new List<AudioPreview>();
				SearchAndDestroyIfNecessary();

			} else {
				
				for (int i = ActiveAudioPreviews.Count - 1; i >= 0; --i) {
					if (!ActiveAudioPreviews[i] || DestroyIfNecessary(ActiveAudioPreviews[i])) {
						ActiveAudioPreviews.RemoveAt(i);
					}
				}

			}
		}

		static void SearchAndDestroyIfNecessary() {
			foreach (AudioPreview audioPreview in Resources.FindObjectsOfTypeAll<AudioPreview>()) {
				if (audioPreview && !DestroyIfNecessary(audioPreview)) {
					ActiveAudioPreviews.Add(audioPreview);
				}
			}
		}

		static bool DestroyIfNecessary(AudioPreview audioPreview) {
			AudioSource audioSource = audioPreview.GetComponent<AudioSource>();
			if (!audioSource || !audioSource.isPlaying) {
				Object.DestroyImmediate(audioPreview.gameObject);
				return true;
			}
			return false;
		}
	
	}
		
	#else
		
	using UnityEngine;
	
	public class AudioPreview : MonoBehaviour {
		
		void Start() {
			Destroy(gameObject);
		}
		
	}
	
	#endif
	
}
