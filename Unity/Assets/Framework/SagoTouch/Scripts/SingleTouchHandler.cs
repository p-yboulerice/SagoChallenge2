namespace SagoTouch {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Used by the <see cref="TouchDispatcher" /> to manage <see cref="SingleTouchBinding"> 
	/// objects and notify <see cref="ISingleTouchObserver" /> objects about touch events.
	/// </summary>
	public class SingleTouchHandler {
		
		
		#region Constructors
		
		/// <summary>
		/// Creates a new SingleTouchHandler object.
		/// </summary>
		public SingleTouchHandler() {
			this.Bindings = new List<SingleTouchBinding>();
			this.BindingsToAdd = new List<SingleTouchBinding>();
			this.BindingsToRemove = new List<SingleTouchBinding>();
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Adds a binding for the observer if one doesn't exist.
		/// Modifies existing binding otherwise.
		/// </summary>
		/// <param name="observer">The observer to add.</param>
		/// <param name="priority">
		/// The priority of the observer. Observers 
		/// with higher priority will receive touch events before observers with 
		/// lower priority. The default value is zero.
		/// </param>
		/// <param name="swallowsTouches">
		/// The value indicating if the binding should swallow touches. If the value 
		/// is <c>true</c> and the observer's <see cref="ISingleTouchObserver.OnTouchBegan" /> 
		/// method returns <c>true</c>, other observers with lower priority will not receive any 
		/// events for the touch.
		/// </param>
		public void Add(ISingleTouchObserver observer, int priority = 0, bool swallowsTouches = true) {
			
			SingleTouchBinding binding;
			binding = this.GetBinding(observer);
			
			if (binding == null) {
				binding = new SingleTouchBinding();
				binding.Observer = observer;
				binding.Priority = priority;
				binding.SwallowsTouches = swallowsTouches;
				this.AddBinding(binding);
			} else if (binding.Priority != priority || binding.SwallowsTouches != swallowsTouches) {
				binding.Priority = priority;
				binding.SwallowsTouches = swallowsTouches;
				this.Dirty = true;
			}
			
		}
		
		/// <summary>
		/// Clears all bindings.
		/// </summary>
		public void Clear() {
			
			if (this.Locked) {
				throw new System.InvalidOperationException("Dispatcher is locked.");
			}
			
			this.Bindings.Clear();
			this.BindingsToAdd.Clear();
			this.BindingsToRemove.Clear();
			this.Dirty = false;
			
		}
		
		/// <summary>
		/// Dispatches the event to the observer for each binding.
		/// <summary>
		/// <param name="phase">The event that occurred.</param>
		/// <param name="touches">The touches associated with the event.</param>
		public List<Touch> Dispatch(TouchPhase phase, List<Touch> touches) {
			
			m_UnswallowedTouches = m_UnswallowedTouches ?? new List<Touch>();
			m_UnswallowedTouches.Clear();
			for (int i = 0, count = touches.Count; i < count; ++i) {
				m_UnswallowedTouches.Add(touches[i]);
			}
			
			// lock the dispatcher so the bindings array isn't modified while being enumerated
			this.Locked = true;
			
			// check if the bindings need to be sorted
			if (this.Dirty) {
				this.Bindings.Sort(SingleTouchBinding.Compare);
				this.Dirty = false;
			}
			
			// dispatch touches
			foreach (Touch touch in touches) {
				
				foreach (SingleTouchBinding binding in this.Bindings) {
					
					UnityEngine.Component component;
					component = binding.Observer as UnityEngine.Component;
					
					bool isActive;
					isActive = component ? component.gameObject.activeInHierarchy : (binding.Observer != null);
					
					bool swallowTouch;
					swallowTouch = false;
					
					switch (phase) {
						case TouchPhase.Began:
							if (isActive && binding.Observer.OnTouchBegan(touch)) {
								binding.Touches.Add(touch);
								touch.HasEverBeenBound = true;
								touch.IsBound = true;
								swallowTouch = binding.SwallowsTouches;
							}
						break;
						case TouchPhase.Moved:
						case TouchPhase.Stationary:
							if (binding.Touches.Contains(touch)) {
								binding.Observer.OnTouchMoved(touch);
								swallowTouch = binding.SwallowsTouches;
							}
						break;
						case TouchPhase.Ended:
							if (binding.Touches.Contains(touch)) {
								binding.Observer.OnTouchEnded(touch);
								binding.Touches.Remove(touch);
								touch.IsBound = false;
								swallowTouch = binding.SwallowsTouches;
							}
						break;
						case TouchPhase.Cancelled:
							if (binding.Touches.Contains(touch)) {
								binding.Observer.OnTouchCancelled(touch);
								binding.Touches.Remove(touch);
								touch.IsBound = false;
								swallowTouch = binding.SwallowsTouches;
							}
						break;
					}
					
					if (swallowTouch) {
						m_UnswallowedTouches.Remove(touch);
						break;
					}
					
				}
				
				if (!touch.HasEverBeenBound) {
					foreach (SingleTouchBinding binding in this.Bindings) {
						
						IUnboundSingleTouchObserver observer;
						observer = binding.Observer as IUnboundSingleTouchObserver;
						
						UnityEngine.Component component;
						component = observer as UnityEngine.Component;
						
						bool isActive;
						isActive = component ? component.gameObject.activeInHierarchy : (observer != null);
						
						bool swallowTouch;
						swallowTouch = false;
						
						switch (phase) {
							case TouchPhase.Began:
								if (isActive && observer.OnUnboundTouchBegan(touch)) {
									binding.Touches.Add(touch);
									touch.HasEverBeenBound = true;
									touch.IsBound = true;
									swallowTouch = binding.SwallowsTouches;
								}
							break;
							case TouchPhase.Moved:
							case TouchPhase.Stationary:
								if (isActive && observer.OnUnboundTouchMoved(touch)) {
									binding.Touches.Add(touch);
									touch.HasEverBeenBound = true;
									touch.IsBound = true;
									swallowTouch = binding.SwallowsTouches;
								}
							break;
							case TouchPhase.Ended:
								if (isActive && observer.OnUnboundTouchEnded(touch)) {
									touch.HasEverBeenBound = true;
									touch.IsBound = false;
									swallowTouch = binding.SwallowsTouches;
								}
							break;
							case TouchPhase.Cancelled:
								if (isActive && observer.OnUnboundTouchCancelled(touch)) {
									touch.HasEverBeenBound = true;
									touch.IsBound = false;
									swallowTouch = binding.SwallowsTouches;
								}
							break;
						}
						
						if (swallowTouch) {
							m_UnswallowedTouches.Remove(touch);
							break;
						}
						
					}
				}
				
			}
			
			// unlock the dispatcher to the bindings can be modified again
			this.Locked = false;
			
			// process bindings that were added while the dispatcher was locked
			foreach (SingleTouchBinding binding in this.BindingsToAdd) {
				this.AddBinding(binding);
			}
			this.BindingsToAdd.Clear();
			
			// process bindings that were removed while the dispatcher was locked
			foreach (SingleTouchBinding binding in this.BindingsToRemove) {
				this.RemoveBinding(binding);
			}
			this.BindingsToRemove.Clear();
			
			// return the unswallowed touches
			if (m_UnswallowedTouches.Count > 0) {
				var copy = new List<Touch>(m_UnswallowedTouches.Count);
				for (int i = 0, count = m_UnswallowedTouches.Count; i < count; ++i) {
					copy.Add(m_UnswallowedTouches[i]);
				}
				return copy;
			} else {
				return m_UnswallowedTouches;
			}
			
		}
		
		/// <summary>
		/// Removes the binding for the observer.
		/// </summary>
		/// <param name="observer">The observer to remove.</param>
		public void Remove(ISingleTouchObserver observer) {
			
			SingleTouchBinding binding;
			binding = this.GetBinding(observer);
			
			if (binding != null) {
				this.RemoveBinding(binding);
			}
		
		}
		
		#endregion
		

		#region Internal Fields

		/// <summary>
		/// This is a member so that we prevent reallocating 
		/// each frame.  It is assumed that this list will be 
		/// allocated once and cleared for each Dispatch.  
		/// It is also returned to the caller of Dispatch, only
		/// if it is empty.  If there are unswallowed touches,
		/// a copy will be made in case subsequent uses of the
		/// list call Dispatch again (e.g. if TouchDispatcher is
		/// Disabled by one of those touches).
		/// </summary>
		private List<Touch> m_UnswallowedTouches;

		#endregion


		#region Internal Properties
		
		private List<SingleTouchBinding> Bindings {
			get; set;
		}
		
		private List<SingleTouchBinding> BindingsToAdd {
			get; set;
		}
		
		private List<SingleTouchBinding> BindingsToRemove {
			get; set;
		}
		
		private bool Dirty {
			get; set;
		}
		
		private bool Locked {
			get; set;
		}
		
		#endregion
		
		
		#region Internal Methods
		
		private void AddBinding(SingleTouchBinding binding) {
			if (this.Locked) {
				if (this.BindingsToRemove.Contains(binding) == true) {
					this.BindingsToRemove.Remove(binding);
				}
				if (this.BindingsToAdd.Contains(binding) == false) {
					this.BindingsToAdd.Add(binding);
				}
			} else {
				if (this.Bindings.Contains(binding) == false) {
					this.Bindings.Add(binding);
					this.Dirty = true;
				}
			}
		}
		
		private void RemoveBinding(SingleTouchBinding binding) {
			if (this.Locked) {
				if (this.BindingsToAdd.Contains(binding) == true) {
					this.BindingsToAdd.Remove(binding);
				}
				if (this.BindingsToRemove.Contains(binding) == false) {
					this.BindingsToRemove.Add(binding);
				}
			} else {
				if (this.Bindings.Contains(binding) == true) {
					this.Bindings.Remove(binding);
					this.Dirty = true;
				}
			}
		}
		
		private SingleTouchBinding GetBinding(ISingleTouchObserver observer) {
			
			foreach (SingleTouchBinding binding in this.Bindings) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			foreach (SingleTouchBinding binding in this.BindingsToAdd) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			foreach (SingleTouchBinding binding in this.BindingsToRemove) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			return null;
			
		}
		
		#endregion
		
		
	}
	
}