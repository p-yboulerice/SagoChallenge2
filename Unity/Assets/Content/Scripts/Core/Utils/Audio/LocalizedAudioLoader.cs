namespace Juice.Utils {
	
	using System.Collections;
	using UnityEngine;
	using SagoAudio;
	using SagoLocalization;
	
	/// <summary>
	/// Associates a LocalizedAudioClipReference with an AudioSetElement
	/// and loads and assigns it at startup.
	/// </summary>
	public class LocalizedAudioLoader : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		LocalizedAudioSetElement[] m_LocalizedAudio;

		#endregion


		#region Internal Types

		[System.Serializable]
		public class LocalizedAudioSetElement {

			[SerializeField]
			protected LocalizedAudioClipReference m_LocalizedAudioClip;

			[SerializeField]
			protected AudioSetElement m_AudioSetElement;


			public LocalizedAudioClipReference LocalizedAudioClip {
				get { return m_LocalizedAudioClip; }
			}

			public AudioSetElement AudioSetElement {
				get { return m_AudioSetElement; }
			}

		}

		#endregion


		#region MonoBehaviour

		void Start() {
			StartCoroutine(LoadAllAudioAsync());
		}

		#endregion


		#region Internal Properties

		LocalizedAudioSetElement[] LocalizedAudio {
			get { return m_LocalizedAudio; }
		}

		#endregion


		#region Internal Methods

		IEnumerator LoadAllAudioAsync() {

			for (int i = 0; i < this.LocalizedAudio.Length; ++i) {

				var item = this.LocalizedAudio[i];
				if (item != null && item.LocalizedAudioClip && item.AudioSetElement && !item.AudioSetElement.AudioClip) {

					var request = item.LocalizedAudioClip.LoadAsync();
					yield return request;

					if (request.asset && item.AudioSetElement) {
						item.AudioSetElement.AudioClip = request.asset;
					}

				}

			}

		}

		#endregion


	}
	
}
