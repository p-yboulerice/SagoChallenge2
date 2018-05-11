namespace SagoAudio {

	using System.Collections.Generic;
	using UnityEngine;

	public class AudioSet : MonoBehaviour {


		#region Properties

		public Dictionary<string, Transform> TransformsByName {
			get {
				if (m_TransformsByName == null) {
					m_TransformsByName = new Dictionary<string, Transform>();
					foreach (Transform transform in GetComponentsInChildren<Transform>()) {
						if (m_TransformsByName.ContainsKey(transform.name)) continue;
						if (!transform.GetComponent<AudioSetElement>()) continue;
						m_TransformsByName.Add(transform.name, transform);
					}
				}
				return m_TransformsByName;
			}
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		#endregion


		#region Methods

		public AudioPlayer FadeIn(string name, float duration) {

			AudioSetElementCollection collection;
			collection = CollectionByName(name);
			
			if (collection) return collection.FadeIn(duration);
			
			AudioSetElement element;
			element = ElementByName(name);
			
			if (element) return element.FadeIn(duration);
			
			return null;

		}

		public void FadeOut(string name, float duration) {

			AudioSetElementCollection collection;
			collection = CollectionByName(name);
			
			if (collection) {
				collection.FadeOut(duration);
				return;
			}
			
			AudioSetElement element;
			element = ElementByName(name);
			
			if (element) {
				element.FadeOut(duration);
			}

		}

		public bool IsPlaying(string name) {

			AudioSetElementCollection collection;
			collection = CollectionByName(name);
			
			if (collection) {
				return collection.IsPlaying;
			}
			
			AudioSetElement element;
			element = ElementByName(name);
			
			if (element) {
				return element.IsPlaying;
			}

			return false;

		}

		public AudioPlayer Play(string name) {

			AudioSetElementCollection collection;
			collection = CollectionByName(name);

			if (collection) return collection.Play();

			AudioSetElement element;
			element = ElementByName(name);

			if (element) return element.Play();

			return null;

		}

		public void Stop(string name) {

			AudioSetElementCollection collection;
			collection = CollectionByName(name);
			
			if (collection) {
				collection.Stop();
				return;
			}
			
			AudioSetElement element;
			element = ElementByName(name);
			
			if (element) {
				element.Stop();
			}

		}

		public AudioSetElementCollection CollectionByName(string name) {

			Transform transform;
			transform = this.TransformByName(name);

			return transform ? transform.GetComponent<AudioSetElementCollection>() : null;

		}

		public AudioSetElement ElementByName(string name) {

			Transform transform;
			transform = this.TransformByName(name);

			return transform ? transform.GetComponent<AudioSetElement>() : null;

		}

		public Transform TransformByName(string name) {
			return this.TransformsByName.ContainsKey(name) ? this.TransformsByName[name] : null;
		}

		#endregion


		#region Fields

		private Dictionary<string, Transform> m_TransformsByName;
		private Transform m_Transform;

		#endregion

	}

}
