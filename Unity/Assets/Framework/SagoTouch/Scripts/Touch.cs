namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// Represents a touch on a touch screen device.
	/// </summary>
	public class Touch : ITouch {
		
		
		#region Fields
		
		[System.NonSerialized]
		protected Vector2 m_Position;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// The maximum distance the touch has moved from it's initial position, in inches (or -1 if the screen dpi is not available).
		/// </summary>
		public float DistanceMovedInInches {
			get {
				if (Screen.dpi == 0 && Application.isEditor == false) {
					Debug.LogError(string.Format("Invalid dpi: {0}", Screen.dpi));
					return -1f;
				}
				return this.DistanceMovedInPixels / Mathf.Max(Screen.dpi, 72);
			}
		}
		
		/// <summary>
		/// The maximum distance the touch has moved from it's initial position, in pixels.
		/// </summary>
		public float DistanceMovedInPixels {
			get;
			protected set;
		}
		
		/// <summary>
		/// The length of time since the touch began, in seconds.
		/// </summary>
		public float Duration {
			get { return Mathf.Max(Time.time - this.Timestamp, 0); }
		}
		
		/// <summary>
		/// Has the touch ever been bound to an observer?
		/// </summary>
		public bool HasEverBeenBound {
			get;
			internal set;
		}
		
		/// <summary>
		/// The unique identifier for the touch.
		/// </summary>
		public int Identifier {
			get;
			internal set;
		}
		
		/// <summary>
		/// The touch's initial position, in pixels.
		/// </summary>
		public Vector2 InitialPosition {
			get;
			protected set;
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Began phase?
		/// </summary>
		public bool IsBegan {
			get { return this.Phase == TouchPhase.Began; }
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Began or TouchPhase.Moved phase?
		/// </summary>
		public bool IsBeganOrMoved {
			get { return this.IsBegan || this.IsMoved; }
		}
		
		/// <summary>
		/// Is the touch bound to an observer?
		/// </summary>
		public bool IsBound {
			get;
			internal set;
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Cancelled phase?
		/// </summary>
		public bool IsCancelled {
			get { return this.Phase == TouchPhase.Cancelled; }
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Cancelled or TouchPhase.Ended phase?
		/// </summary>
		public bool IsCancelledOrEnded {
			get { return this.IsCancelled || this.IsEnded; }
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Ended phase?
		/// </summary>
		public bool IsEnded {
			get { return this.Phase == TouchPhase.Ended; }
		}
		
		/// <summary>
		/// Has the touch been initialized?
		/// </summary>
		public bool IsInitialized {
			get;
			protected set;
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Moved touch phase?
		/// </summary>
		public bool IsMoved {
			get { return this.Phase == TouchPhase.Moved; }
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Moved or TouchPhase.Stationary phase?
		/// </summary>
		public bool IsMovedOrStationary {
			get { return this.IsMoved || this.IsStationary; }
		}
		
		/// <summary>
		/// Is the touch in the TouchPhase.Stationary phase?
		/// </summary>
		public bool IsStationary {
			get { return this.Phase == TouchPhase.Stationary; }
		}
		
		/// <summary>
		/// Is the touch a tap? A touch is considered a tap if it has moved 
		/// less than .25 inches and lasted less than .333 seconds.
		/// </summary>
		public bool IsTap {
			get { return (this.DistanceMovedInInches < 0.25f && this.Duration < 0.333f); }
		}
		
		[System.Obsolete("MaxDistanceFromInitialPosition is deprecated, use DistanceMovedInPixels instead.")]
		public float MaxDistanceFromInitialPosition {
			get { return this.DistanceMovedInPixels; }
		}
		
		/// <summary>
		/// The touch's current phase.
		/// </summary>
		public TouchPhase Phase {
			get;
			internal set;
		}
		
		/// <summary>
		/// The touch's current position, in pixels.
		/// </summary>
		public Vector2 Position {
			get { return m_Position; }
			internal set {
				value.x = Mathf.Clamp(value.x, 0f, Screen.width);
				value.y = Mathf.Clamp(value.y, 0f, Screen.height);
				if (this.IsInitialized) {
					this.PreviousPosition = m_Position;
					m_Position = value;
					this.DistanceMovedInPixels = Mathf.Max(this.DistanceMovedInPixels, (m_Position - this.InitialPosition).magnitude);
				} else {
					this.InitialPosition = value;
					this.IsInitialized = true;
					this.DistanceMovedInPixels = 0f;
					this.PreviousPosition = value;
					m_Position = value;
				}
			}
		}
		
		/// <summary>
		/// The touch's previous position, in pixels.
		/// </summary>
		public Vector2 PreviousPosition {
			get;
			protected set;
		}
		
		/// <summary>
		/// The time the touch began.
		/// </summary>
		public float Timestamp {
			get;
			protected set;
		}
		
		/// <summary>
		/// The velocity of the touch, in pixels per second.
		/// </summary>
		public Vector2 Velocity {
			get;
			internal set;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Creates a new touch.
		/// </summary>
		public Touch() {
			this.Timestamp = Time.time;
		}
		
		#endregion
		
		
	}
	
}