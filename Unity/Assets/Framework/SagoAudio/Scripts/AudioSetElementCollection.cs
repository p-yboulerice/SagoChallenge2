namespace SagoAudio {

	using UnityEngine;

	public class AudioSetElementCollection : MonoBehaviour {


		#region Modes

		public enum Modes {
			Cycle,
			RandomOnEnable,
			RandomOnPlay
		}

		#endregion


		#region Serialized Properties

		[SerializeField]
		public Modes Mode;

		#endregion


		#region Properties

		public bool IsPlaying {
			get {
				foreach (AudioSetElement element in this.Elements) {
					if (element.IsPlaying) return true;
				}
				return false;
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

		public AudioPlayer FadeIn(float duration) {

			AudioSetElement element;
			element = this.ElementToPlay;

			AudioPlayer player;
			player = null;

			if (element) {
				player = element.FadeIn(duration);
			}

			return player;

		}

		/// <summary>
		/// Wraps FadeIn() with no return value, for messaging/UnityEvents.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public void FadeInCallback(float duration) {
			FadeIn(duration);
		}

		public void FadeOut(float duration) {

			AudioSetElement element;
			element = this.CurrentElement;

			if (element) {
				element.FadeOut(duration);
			}

		}

		public AudioSetElement ElementToPlay {
			get {
				AudioSetElement result;
				result = null;

				switch (this.Mode) {
					case Modes.Cycle:
						result = this.NextElement;
						break;
					case Modes.RandomOnEnable:
						result = this.CurrentElement;
						break;
					case Modes.RandomOnPlay:
						result = this.RandomElement;
						break;
				}

				return result;
			}
		}

		public AudioPlayer Play() {

			AudioSetElement element;
			element = this.ElementToPlay;

			return element ? element.Play() : null;

		}

		/// <summary>
		/// Wraps Play() with no return value, for messaging/UnityEvents.
		/// </summary>
		public void PlayCallback() {
			Play();
		}

		public void Stop() {
			foreach (AudioSetElement element in this.Elements) {
				element.Stop();
			}
		}

		#endregion


		#region Private Properties

		private AudioSetElement[] Elements {
			get {
				m_Elements = m_Elements ?? GetComponents<AudioSetElement>();
				return m_Elements;
			}
		}

		private int ElementIndex {
			get;
			set;
		}

		#endregion


		#region Fields

		private AudioSetElement[] m_Elements;
		private Transform m_Transform;

		#endregion


		#region MonoBehaviour

		private void OnEnable() {
			this.ElementIndex = this.RandomElementIndex;
		}

		#endregion


		#region Helper

		private AudioSetElement CurrentElement {
			get { return (this.Elements.Length > 0) ? this.Elements[this.ElementIndex] : null; }
		}

		private AudioSetElement NextElement {
			get {
				this.ElementIndex = (this.ElementIndex + 1) % this.Elements.Length;
				return this.CurrentElement;
			}
		}

		private AudioSetElement RandomElement {
			get {
				this.ElementIndex = this.RandomElementIndex;
				return this.CurrentElement;
			}
		}

		private int RandomElementIndex {
			get { return Random.Range(0, this.Elements.Length); }
		}

		#endregion

	}

}
