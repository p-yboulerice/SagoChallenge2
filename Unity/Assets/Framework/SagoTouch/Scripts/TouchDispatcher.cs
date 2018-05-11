namespace SagoTouch {
	
	using System.Collections.Generic;
	
	public class TouchDispatcher : UnityEngine.MonoBehaviour {
		
		
		#region Types
		
		public enum TouchDispatcherState {
			Unknown,
			Quit
		}

		/// <summary>
		/// Prevents boxing of enum in dictionary lookups to 
		/// eliminate memory allocations.
		/// </summary>
		public class TouchPhaseComparer : IEqualityComparer<TouchPhase> {

			public bool Equals(TouchPhase x, TouchPhase y) {
				return x == y;
			}

			public int GetHashCode(TouchPhase x) {
				return (int)x;
			}

		}

		#endregion
		
		
		#region Properties
		
		public static TouchDispatcher Instance {
			get {
				#if !UNITY_TVOS
					if (!UnityEngine.Application.isPlaying) {
						return null;
					}
					if (TouchDispatcher.State == TouchDispatcherState.Quit) {
						return s_Instance;
					}
					if (s_Instance == null) {
						s_Instance = UnityEngine.GameObject.FindObjectOfType(typeof(TouchDispatcher)) as TouchDispatcher;
					}
					if (s_Instance == null) {
						s_Instance = new UnityEngine.GameObject().AddComponent<TouchDispatcher>();
						s_Instance.name = "TouchDispatcher";
						DontDestroyOnLoad(s_Instance.gameObject);
					}
					return s_Instance;
				#else
					return null;
				#endif
			}
		}
		
		public static TouchDispatcherState State {
			get; private set;
		}
		
		public int TouchCount {
			get { return this.Touches.Count; }
		}
		
		#endregion
		
		
		#region Methods
		
		public void Add(IMultiTouchObserver observer, int priority = 0) {
			this.MultiTouchHandler.Add(observer, priority);
		}
		
		public void Add(ISingleTouchObserver observer, int priority = 0, bool swallowsTouches = true) {
			this.SingleTouchHandler.Add(observer, priority, swallowsTouches);
		}
		
		public void Clear() {
			this.MultiTouchHandler.Clear();
			this.Phases.Clear();
			this.SingleTouchHandler.Clear();
			this.Touches.Clear();
		}
		
		public void Remove(IMultiTouchObserver observer) {
			this.MultiTouchHandler.Remove(observer);
		}
		
		public void Remove(ISingleTouchObserver observer) {
			this.SingleTouchHandler.Remove(observer);
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		private static TouchDispatcher s_Instance;
		
		#endregion
		
		
		#region Internal Properties
		
		private MultiTouchHandler MultiTouchHandler {
			get; set;
		}
		
		private Dictionary<TouchPhase,List<Touch>> Phases {
			get; set;
		}
		
		private SingleTouchHandler SingleTouchHandler {
			get; set;
		}
		
		private List<Touch> Touches {
			get; set;
		}
		
		#endregion
		
		
		#region Internal Methods
		
		private void Awake() {
			
			if (this != TouchDispatcher.Instance) {
				DestroyObject(gameObject);
				return;
			}
			
			this.MultiTouchHandler = new MultiTouchHandler();
			this.Phases = new Dictionary<TouchPhase, List<Touch>>(new TouchPhaseComparer());
			this.Phases[TouchPhase.Began] = new List<Touch>();
			this.Phases[TouchPhase.Moved] = new List<Touch>();
			this.Phases[TouchPhase.Stationary] = new List<Touch>();
			this.Phases[TouchPhase.Ended] = new List<Touch>();
			this.Phases[TouchPhase.Cancelled] = new List<Touch>();
			this.SingleTouchHandler = new SingleTouchHandler();
			this.Touches = new List<Touch>();
			
		}
		
		private void Dispatch(TouchPhase phase, List<Touch> touches) {
			touches = this.SingleTouchHandler.Dispatch(phase, touches);
			touches = this.MultiTouchHandler.Dispatch(phase, touches);
		}
		
		private void OnApplicationQuit() {
			TouchDispatcher.State = TouchDispatcherState.Quit;
		}
		
		private void OnDisable() {
			List<Touch> touches = new List<Touch>(this.Touches);
			foreach (Touch touch in touches) {
				touch.Phase = TouchPhase.Cancelled;
			}
			this.Dispatch(TouchPhase.Cancelled, touches);
			this.Touches.Clear();
		}
		
		private void Update() {
			
			// existing touches
			this.Touches.RemoveAll(touch => {
				if (touch.Identifier < 0) {
					
					#if UNITY_EDITOR || (UNITY_METRO && !UNITY_WP_8_1) || UNITY_ANDROID
					if (UnityEngine.Input.GetMouseButton(0)) { // mouse move
						touch.Phase = TouchPhase.Moved;
						touch.Position = UnityEngine.Input.mousePosition;
						touch.Velocity = (touch.Position - touch.PreviousPosition) / UnityEngine.Time.deltaTime;
						return false;
					} else if (touch.Phase != TouchPhase.Ended && touch.Phase != TouchPhase.Cancelled) { // mouse up
						touch.Phase = TouchPhase.Ended;
						touch.Position = UnityEngine.Input.mousePosition;
						touch.Velocity = (touch.Position - touch.PreviousPosition) / UnityEngine.Time.deltaTime;
						return false;
					}
					#endif
					
				} else {
					
					foreach (UnityEngine.Touch other in UnityEngine.Input.touches) {
						if (touch.Identifier == other.fingerId) {
							touch.Phase = (TouchPhase)other.phase;
							touch.Position = other.position;
							touch.Velocity = (touch.Position - touch.PreviousPosition) / UnityEngine.Time.deltaTime;
							return false;
						}
					}
					
					// make sure the touch gets cancelled instead of just being 
					// removed if the underlying touch disappears in another phase. 
					
					// this happens on some android devices when you press the home 
					// button or receive a phone call while touching. when the app 
					// becomes inactive, the touch is in the Moved or Stationary state. 
					// when the app becomes active, the touch doesn't exist anymore 
					// and never went through the cancelled or ended phase, which can 
					// leave the game in an inconsistent state.
					if (touch.Phase != TouchPhase.Cancelled && touch.Phase != TouchPhase.Ended) {
						touch.Phase = TouchPhase.Cancelled;
						return false;
					}
					
				}
				return true;
			});
			
			// new touches
			foreach (UnityEngine.Touch other in UnityEngine.Input.touches) {
				if (this.Touches.Find(touch => touch.Identifier == other.fingerId) == null) {
					Touch touch = new Touch();
					touch.Identifier = other.fingerId;
					touch.Phase = TouchPhase.Began;
					touch.Position = other.position;
					touch.Velocity = UnityEngine.Vector2.zero;
					this.Touches.Add(touch);
				}
			}
			
			// mouse down
			#if UNITY_EDITOR || (UNITY_METRO && !UNITY_WP_8_1) || UNITY_ANDROID
			if (UnityEngine.Input.GetMouseButtonDown(0) && UnityEngine.Input.touchCount < 1) {
				if (this.Touches.Find(touch => touch.Identifier == -1) == null) {
					Touch touch = new Touch();
					touch.Identifier = -1;
					touch.Phase = TouchPhase.Began;
					touch.Position = UnityEngine.Input.mousePosition;
					touch.Velocity = UnityEngine.Vector2.zero;
					this.Touches.Add(touch);
				}
			}
			#endif
			
			// sort touches by phase
			this.Phases[TouchPhase.Began].Clear();
			this.Phases[TouchPhase.Moved].Clear();
			this.Phases[TouchPhase.Stationary].Clear();
			this.Phases[TouchPhase.Ended].Clear();
			this.Phases[TouchPhase.Cancelled].Clear();
			foreach (Touch touch in this.Touches) {
				this.Phases[touch.Phase].Add(touch);
			}
			
			// handle touches by phase
			this.Dispatch(TouchPhase.Cancelled, this.Phases[TouchPhase.Cancelled]);
			this.Dispatch(TouchPhase.Ended, this.Phases[TouchPhase.Ended]);
			this.Dispatch(TouchPhase.Stationary, this.Phases[TouchPhase.Stationary]);
			this.Dispatch(TouchPhase.Moved, this.Phases[TouchPhase.Moved]);
			this.Dispatch(TouchPhase.Began, this.Phases[TouchPhase.Began]);
			
		}
		
		#endregion
		
		
	}
	
}