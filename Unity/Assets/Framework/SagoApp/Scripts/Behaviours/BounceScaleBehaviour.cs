namespace SagoApp {
	
	using UnityEngine;

	public class BounceScaleBehaviour : MonoBehaviour {


		//
		// Inspector Properties
		//
		public float AmplitudeInPercent;
		public int DurationInFrames;
		public float Damping;
		public float Strength;


		//
		// Properties
		//
		public Vector3 InitialScale {
			get;
			private set;
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ? m_Transform : transform;
				return m_Transform;
			}
		}


		//
		// Private Properties
		//
		private int FrameCount {
			get;
			set;
		}

		private bool IsInitialized {
			get;
			set;
		}

		private Vector3 ScaleVelocity {
			get;
			set;
		}


		//
		// Public Methods
		//
		public void Init() {
			if (!this.IsInitialized) {
				this.InitialScale = this.Transform.localScale;
				this.IsInitialized = true;
			}
		}
		
		public void ResetInitialScale() {
			this.IsInitialized = false;
		}

		public void Trigger() {
			if (!this.IsInitialized) {
				Init();
			}
			this.FrameCount = 0;
			this.Transform.localScale = (1 - this.AmplitudeInPercent / 100f) * this.InitialScale;
			this.enabled = true;
		}


		//
		// Member Variables
		//
		private Transform m_Transform;


		//
		// MonoBehaviour
		//
		void Update() {

			Init();
			this.ScaleVelocity *= (1 - this.Damping);
			this.ScaleVelocity += this.Strength * (this.InitialScale - this.Transform.localScale);
			this.Transform.localScale += this.ScaleVelocity;

			if (++this.FrameCount == this.DurationInFrames) {
				this.Transform.localScale = this.InitialScale;
				this.enabled = false;
			}

		}
		
		void Reset() {
			this.AmplitudeInPercent = 30;
			this.DurationInFrames = 60;
			this.Damping = 0.15f;
			this.Strength = 0.15f;
		}


	}

}
