namespace SagoLayout {

	using UnityEngine;
	
	public class SizeInPoints : ArtboardElement {


		//
		// Inspector Properties
		//
		public int HeightInPointsKindle;
		public int HeightInPointsPad;
		public int HeightInPointsPhone;
		public bool IsDeviceSpecific;


		//
		// Properties
		//
		public float Height {
			get { return this.Bounds.size.y; }
		}
		
		public float HeightInPoints {
			get { return Mathf.Max(1, ScreenUtility.HeightInPoints * this.Height / this.ArtboardHeight); }
			set { this.Transform.localScale = Vector3.one * Mathf.Max(1, value) * this.ArtboardHeight / (ScreenUtility.HeightInPoints * this.HeightLocal); }
		}
		
		public float HeightLocal {
			get { return this.LocalBounds.size.y; }
		}


		//
		// Member Variables
		//
		[SerializeField]
		private bool m_Initialized;


		//
		// MonoBehaviour
		//
		override protected void Reset() {
			base.Reset();
			this.IsDeviceSpecific = true;
			this.Transform.localScale = Vector3.one;
			m_Initialized = false;
		}


		//
		// Public Methods
		//
		override public void Apply() {
			Init();
			ApplyScale();
			ApplyAnchors();
		}


		//
		// Functions
		//
		private void Init() {
			if (!m_Initialized && this.Height > 0) {
				if (ScreenUtility.IsPhone) {
					this.Transform.localScale = Vector3.one;
					this.HeightInPointsPhone = Mathf.RoundToInt(this.HeightInPoints);
					this.HeightInPointsPad = Mathf.RoundToInt(this.HeightInPoints * this.DefaultPhoneToPadRatio);
				} else {
					this.Transform.localScale = this.DefaultPhoneToPadScale * Vector3.one;
					this.HeightInPointsPad = Mathf.RoundToInt(this.HeightInPoints);
					this.HeightInPointsPhone = Mathf.RoundToInt(this.HeightInPoints / this.DefaultPhoneToPadRatio);
				}
				this.HeightInPointsKindle = Mathf.RoundToInt(this.HeightInPointsPhone * this.DefaultPhoneToAndroidRatio);
				m_Initialized = true;
			}
		}

		private void ApplyScale() {
			if (m_Initialized) {
				this.HeightInPointsPad = Mathf.Max(1, this.HeightInPointsPad);
				this.HeightInPointsPhone = Mathf.Max(1, this.HeightInPointsPhone);
				this.HeightInPoints = (this.IsDeviceSpecific && !ScreenUtility.IsPhone) ? (ScreenUtility.IsKindle ? this.HeightInPointsKindle : this.HeightInPointsPad) : this.HeightInPointsPhone;
			}
		}

		private void ApplyAnchors() {
			foreach (Anchor anchor in GetComponentsInChildren<Anchor>()) {
				anchor.Apply();
			}
		}


		//
		// Helper
		//
		private float DefaultPhoneToPadRatio {
			get { return this.DefaultPhoneToPadScale * ScreenUtility.PAD_HEIGHT_IN_POINTS / (ScreenUtility.PHONE_HEIGHT_IN_POINTS * 1.0f); }
		}

		private float DefaultPhoneToPadScale {
			get { return 0.66f; }
		}

		private float DefaultPhoneToAndroidRatio {
			get { return 2.14f; } 
		}

	}

}
