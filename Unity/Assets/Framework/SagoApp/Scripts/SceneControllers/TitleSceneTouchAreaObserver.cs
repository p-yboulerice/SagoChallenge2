namespace SagoApp {
	
	using SagoAudio;
	using SagoTouch;
	using UnityEngine;
	using UnityEngine.Serialization;
	using Touch = SagoTouch.Touch;

	public class TitleSceneTouchAreaObserver : TouchAreaObserver {
		
		
		#region Button Types
		
		public enum ButtonTypes {
			Start
		}
		
		#endregion
		
		
		#region Fields
		
		[FormerlySerializedAs("Audio")]
		[SerializeField]
		protected AudioClip m_Audio;
		
		[FormerlySerializedAs("ButtonType")]
		[SerializeField]
		protected ButtonTypes m_ButtonType;
		
		[System.NonSerialized]
		protected TitleSceneController m_TitleSceneController;
		
		#endregion
		
		
		#region Properties
		
		public AudioClip Audio {
			get { return m_Audio; }
			set { m_Audio = value; }
		}
		
		public ButtonTypes ButtonType {
			get { return m_ButtonType; }
			set { m_ButtonType = value; }
		}
		
		public TitleSceneController TitleSceneController {
			get {
				m_TitleSceneController = m_TitleSceneController ?? GetComponentInParent<TitleSceneController>();
				return m_TitleSceneController;
			}
		}
		
		#endregion
		
		
		#region MonoBehaviour
		
		virtual protected void Awake() {
			if (this.TitleSceneController) {
				if (this.ButtonType == ButtonTypes.Start) {
					this.TouchUpDelegate = this.OnTouchStartButton;
					return;
				}
			}
		}
		
		#endregion
		
		
		#region Helper Methods
		
		protected void OnTouchStartButton(TouchArea touchArea, Touch touch) {
			if (this.TitleSceneController) {
				PlayAudio();
				this.TitleSceneController.OnTouchStartButton(touchArea, touch);
			}
		}
		
		protected void PlayAudio() {
			if (this.Audio && AudioManager.Instance) {
				AudioManager.Instance.Play(this.Audio);
			}
		}
		
		#endregion
		
		
	}
	
}
