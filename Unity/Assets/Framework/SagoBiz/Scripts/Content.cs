namespace SagoBiz {
	
	using SagoEasing;
	using SagoLayout;
	using System.Collections;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// ContentAnimationDirection defines the direction content will animate 
	/// when it is turned on. The content will animate in the opposite direction 
	/// when it is turned off.
	/// </summary>
	public enum ContentAnimationDirection {
		None,
		Right,
		Down,
		Left,
		Up
	}
	
	/// <summary>
	/// ContentState defines the possible states for a <see cref="Content" /> component.
	/// </summary>
	public enum ContentState {
		Unknown,
		TurnOn,
		On,
		TurnOff,
		Off
	}
	
	/// <summary>
	/// The Content class provides functionality for turning content on and off.
	/// </summary>
	public class Content : MonoBehaviour {
		
		
		#region Fields
		
		/// <summary>
		/// The direction the content will animate when it is turned on.
		/// </summary>
		[SerializeField]
		protected ContentAnimationDirection m_AnimationDirection;
		
		/// <summary>
		/// The duration of the animation when the content is turned on or off, in seconds.
		/// </summary>
		[SerializeField]
		protected float m_AnimationDuration;
		
		/// <summary>
		/// Cached reference to the <see cref="Button" /> component associated with the content.
		/// </summary>
		[System.NonSerialized]
		protected Button m_Button;

		[System.NonSerialized]
		protected IEnumerator m_TurnOnAsyncCoroutine;

		[System.NonSerialized]
		protected IEnumerator m_TurnOffAsyncCoroutine;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets and sets the direction the content will animate when it is turned on.
		/// </summary>
		public ContentAnimationDirection AnimationDirection {
			get { return m_AnimationDirection; }
			set { m_AnimationDirection = value; }
		}
		
		/// <summary>
		/// Gets and sets the duration of the animation when the content is turned on or off, in seconds.
		/// </summary>
		public float AnimationDuration {
			get { return m_AnimationDuration; }
			set { m_AnimationDuration = value; }
		}
		
		/// <summary>
		/// Gets the reference to the <see cref="Button" /> component associated with the content.
		/// </summary>
		public Button Button {
			get {
				m_Button = m_Button ?? this.GetComponentInChildren<Button>();
				return m_Button;
			}
		}
		
		/// <summary>
		/// Gets the local position of the <see cref="Button" /> when the content is turned off.
		/// </summary>
		public Vector3 OffPosition {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the local position of the <see cref="Button" /> when the content is turned on.
		/// </summary>
		public Vector3 OnPosition {
			get; protected set;
		}
		
		/// <summary>
		/// Gets and sets the current state.
		/// </summary>
		public ContentState State {
			get; protected set;
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Turns off the content and resets it to the default state.
		/// </summary>
		public void Clear() {
			StopAllAsyncCoroutines();
			this.TurnOff(false);
			this.State = ContentState.Unknown;
		}
		
		/// <summary>
		/// Turns the content on.
		/// </summary>
		public void TurnOn(bool animated) {
			switch (this.State) {
				case ContentState.Unknown:
				case ContentState.Off:
				case ContentState.TurnOff:
					StopAllAsyncCoroutines();
					StartTurnOnAsyncCoroutine(animated);
				break;
				case ContentState.TurnOn:
					if (!animated) {
						StopAllAsyncCoroutines();
						StartTurnOnAsyncCoroutine(animated);
					}
				break;
			}
		}
		
		/// <summary>
		/// Turns the content off.
		/// </summary>
		public void TurnOff(bool animated) {
			switch (this.State) {
				case ContentState.Unknown:
				case ContentState.On:
				case ContentState.TurnOn:
					StopAllAsyncCoroutines();
					StartTurnOffAsyncCoroutine(animated);
				break;
				default:
					// if turn off get called while animating out, it will skip 
					// the rest of the animation. right now, we're not animating 
					// the turn off (it causes visual glitched when returning 
					// from the background), but if we ever decide to turn the 
					// animation back on, we may have to revisit this code to 
					// make sure the animation doesn't get interrupted or restarted.
					StopAllAsyncCoroutines();
					StartTurnOffAsyncCoroutine(false);
				break;
			}
		}
		
		#endregion
		
		
		#region Coroutine Methods
		
		/// <summary>
		/// Turns the content on using a coroutine to animate the transition.
		/// </summary>
		virtual protected IEnumerator TurnOnAsync(bool animated) {
			
			this.State = ContentState.TurnOn;
			
			Layout();
			
			Vector3 initialPosition;
			initialPosition = this.OffPosition;
			
			Vector3 targetPosition;
			targetPosition = this.OnPosition;
			
			float duration;
			duration = this.AnimationDuration;
			
			float elapsed;
			elapsed = 0f;
			
			float t;
			t = 0;
			
			this.gameObject.SetActive(true);
			if (animated && duration > 0) {
				while (elapsed < duration) {
					elapsed += Time.deltaTime;
					t = Mathf.Clamp(elapsed / duration, 0f, 1f);
					this.Button.Transform.localPosition = Cubic.EaseOut(initialPosition, targetPosition, t);
					yield return null;
				}
			}
			
			this.Button.enabled = true;
			this.Button.Transform.localPosition = targetPosition;
			this.State = ContentState.On;
			yield break;
			
		}
		
		/// <summary>
		/// Turns the content off using a coroutine to animate the transition.
		/// </summary>
		virtual protected IEnumerator TurnOffAsync(bool animated) {
			
			this.State = ContentState.TurnOff;
			this.Button.enabled = false;
			
			Layout();
			
			Vector3 initialPosition;
			initialPosition = this.OnPosition;
			
			Vector3 targetPosition;
			targetPosition = this.OffPosition;
			
			float duration;
			duration = this.AnimationDuration;
			
			float elapsed;
			elapsed = 0f;
			
			float t;
			t = 0;
			
			if (animated && duration > 0) {
				while (elapsed < duration) {
					elapsed += Time.deltaTime;
					t = Mathf.Clamp(elapsed / duration, 0f, 1f);
					this.Button.Transform.localPosition = Cubic.EaseIn(initialPosition, targetPosition, t);
					yield return null;
				}
			}
			
			this.Button.Transform.localPosition = targetPosition;
			this.State = ContentState.Off;
			this.gameObject.SetActive(false);
			yield break;
			
		}
		
		#endregion
		
		
		#region Helper Methods

		protected void StartTurnOnAsyncCoroutine(bool animated) {
			if (CoroutineRunner.Instance == null) {
				return;
			}

			StopTurnOnAsyncCoroutine();

			m_TurnOnAsyncCoroutine = this.TurnOnAsync(animated);
			CoroutineRunner.Instance.StartCoroutine(m_TurnOnAsyncCoroutine);
		}

		protected void StartTurnOffAsyncCoroutine(bool animated) {
			if (CoroutineRunner.Instance == null) {
				return;
			}

			StopTurnOffAsyncCoroutine();

			m_TurnOffAsyncCoroutine = this.TurnOffAsync(animated);
			CoroutineRunner.Instance.StartCoroutine(m_TurnOffAsyncCoroutine);
		}

		protected void StopTurnOnAsyncCoroutine() {
			if (CoroutineRunner.Instance == null) {
				return;
			}

			if (m_TurnOnAsyncCoroutine != null) {
				CoroutineRunner.Instance.StopCoroutine(m_TurnOnAsyncCoroutine);
				m_TurnOnAsyncCoroutine = null;
			}
		}

		protected void StopTurnOffAsyncCoroutine() {
			if (CoroutineRunner.Instance == null) {
				return;
			}

			if (m_TurnOffAsyncCoroutine != null) {
				CoroutineRunner.Instance.StopCoroutine(m_TurnOffAsyncCoroutine);
				m_TurnOffAsyncCoroutine = null;
			}
		}

		protected void StopAllAsyncCoroutines() {
			StopTurnOnAsyncCoroutine();
			StopTurnOffAsyncCoroutine();
		}
		
		protected void Layout() {
			
			// move the button to the on position
			foreach (LayoutComponent component in GetComponentsInChildren<LayoutComponent>()) {
				component.Apply();
			}
			
			// set the on position
			OnPosition = Button.Transform.localPosition;
			
			// calculate the off position
			switch (AnimationDirection) {
				case ContentAnimationDirection.Right:
					OffPosition = Button.Transform.localPosition + Vector3.left * Button.Align.Bounds.size.x;
				break;
				case ContentAnimationDirection.Down:
					OffPosition = Button.Transform.localPosition + Vector3.up * Button.Align.Bounds.size.y;
				break;
				case ContentAnimationDirection.Left:
					OffPosition = Button.Transform.localPosition + Vector3.right * Button.Align.Bounds.size.x;
				break;
				case ContentAnimationDirection.Up:
					OffPosition = Button.Transform.localPosition + Vector3.down * Button.Align.Bounds.size.y;
				break;
				default:
					OffPosition = Button.Transform.localPosition;
				break;
			}
			
		}
		
		#endregion
		
		
	}
	
}