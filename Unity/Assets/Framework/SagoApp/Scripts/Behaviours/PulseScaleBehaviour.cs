namespace SagoApp {
	
	using UnityEngine;

	public class PulseScaleBehaviour : MonoBehaviour {


		//
		// Inspector Properties
		//
		public int AmplitudeInPercent;
		public int PeriodInFrames;


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
		// Public Methods
		//
		public void ResetInitialScale() {
			this.IsInitialized = false;
		}
		
		public void Init() {
			if (!this.IsInitialized) {
				this.InitialScale = this.Transform.localScale;
				this.IsInitialized = true;
			}
		}

		public void Trigger() {
			Init();
			
			this.FrameCount = 0;
			this.Transform.localScale = this.InitialScale;
			this.enabled = true;
		}

		//
		// Private Properties
		//
		private bool IsInitialized {
			get;
			set;
		}

		private int FrameCount {
			get;
			set;
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

			this.FrameCount = ++this.FrameCount % this.PeriodInFrames;

			float theta;
			theta = Mathf.PI * this.FrameCount / (0.5f * this.PeriodInFrames);

			float scalar;
			scalar = 1 + Mathf.Sin(theta) * this.AmplitudeInPercent / 100f;

			this.Transform.localScale = scalar * this.InitialScale;

		}

		void Reset() {
			this.AmplitudeInPercent = 10;
			this.PeriodInFrames = 60;
		}


	}

}
