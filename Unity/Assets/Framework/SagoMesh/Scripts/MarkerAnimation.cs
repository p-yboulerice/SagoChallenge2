namespace SagoMesh {
	
	using SagoMesh.Internal;
	using UnityEngine;
	
	public enum MarkerAnimationFlip {
		None,
		Horizontal,
		Vertical
	}
	
	[System.Serializable]
	public struct MarkerAnimationFrame {
		
		
		#region Fields
		
		[SerializeField]
		public bool Active;
		
		[SerializeField]
		public Vector2 Position;
		
		[SerializeField]
		public float Rotation;
		
		[SerializeField]
		public Vector2 Scale;
		
		[SerializeField]
		public Vector2 Skew;
		
		#endregion
		
		
		#region Properties

		public Vector3 GetEulerAngles() {

			bool isFlipped;
			isFlipped = this.IsFlipped;

			Vector3 euler;
			euler = Vector3.zero;
			euler.x = isFlipped ? 180 : 0;
			euler.z = isFlipped ? this.Skew.y : this.Rotation;

			return euler;

		}
		
		public Vector2 GetPosition() {
			return this.Position;
		}
		
		public Quaternion GetRotation() {
			return Quaternion.Euler(GetEulerAngles());
		}
		
		public Vector2 GetScale() {
			return this.Scale;
		}
		
		#endregion


		#region Helper

		/// <summary>
		/// Skew is the skew vector read out of Flash, and is used to support Flash > Modify > Transform > Flip Vertical & Flip Horizontal only.
		/// Actual skew distortion is not supported.
		/// </summary>
		private bool IsFlipped {
			get {
				if (this.Skew != Vector2.zero) {
					float sum = Mathf.Abs(this.Skew.x) + Mathf.Abs(this.Skew.y);
					return Mathf.RoundToInt(sum / 180f) == 1;
				}
				return false;
			}
		}

		#endregion

		
	}
	
	public class MarkerAnimation : ScriptableObject {
		
		
		#region Fields

		[SerializeField]
		protected MarkerAnimationFlip m_Flip;
		
		[SerializeField]
		protected MarkerAnimationFrame[] m_Frames;
		
		[SerializeField]
		protected int m_FramesPerSecond;
		
		[SerializeField]
		protected int m_PixelsPerMeter;
		
		[HideInInspector]
		[SerializeField]
		protected string m_Version;
		
		#endregion
		
		
		#region Properties
		
		public float Duration {
			get { return (float)this.Frames.Length / this.FramesPerSecond; }
		}
		
		[System.Obsolete("Flip is obsolete and has no effect")]
		public MarkerAnimationFlip Flip {
			get { return m_Flip; }
			set { m_Flip = value; }
		}
		
		public MarkerAnimationFrame[] Frames {
			get { return m_Frames; }
			set {
				if (!ArrayUtil.Equal(m_Frames, value)) {
					m_Frames = value;
					AssetUtil.SetDirty(this);
				}
			}
		}
		
		public int FramesPerSecond {
			get { return m_FramesPerSecond; }
			set { 
				if (m_FramesPerSecond != value) {
					m_FramesPerSecond = value;
					AssetUtil.SetDirty(this);
				}
			}
		}
		
		public float MetersPerPixel {
			get { return 1f / this.PixelsPerMeter; }
		}
		
		public int PixelsPerMeter {
			get { return m_PixelsPerMeter; }
			set { 
				if (m_PixelsPerMeter != value) {
					m_PixelsPerMeter = value;
					AssetUtil.SetDirty(this);
				}
			}
		}
		
		public string Version {
			get { return m_Version; }
			protected set {
				if (m_Version != value) {
					m_Version = value;
					AssetUtil.SetDirty(this);
				}
			}
		}
		
		#endregion
		
		
		#region Methods
		
		public MarkerAnimationFrame FrameAt(int index) {
			if (m_Frames == null || m_Frames.Length == 0 || index < 0 || index >= m_Frames.Length) {
				return default(MarkerAnimationFrame);
			}
			return m_Frames[index];
		}
		
		public void ResetVersion() {
			this.Version = string.Format(
				"{0}_{1}_{2}", 
				this.name, 
				this.Frames.Length, 
				System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond
			);
		}
		
		#endregion
		
		
	}
	
}