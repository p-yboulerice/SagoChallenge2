namespace SagoTouch {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Used by the <see cref="TouchDispatcher" /> to manage <see cref="MultiTouchBinding"> 
	/// objects and notify <see cref="IMultiTouchObserver" /> objects about touch events.
	/// </summary>
	public class MultiTouchHandler {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new MultiTouchHandler object.
		/// </summary>
		public MultiTouchHandler() {
			this.Bindings = new List<MultiTouchBinding>();
			this.BindingsToAdd = new List<MultiTouchBinding>();
			this.BindingsToRemove = new List<MultiTouchBinding>();
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Adds a binding for the observer, unless one already exists.
		/// </summary>
		/// <param name="observer">The observer to add.</param>
		/// <param name="priority">
		/// The priority of the observer. Observers with higher priority will 
		/// receive touch events before observers with lower priority.The 
		/// default value is zero.
		/// </param>
		public void Add(IMultiTouchObserver observer, int priority = 0) {
			
			MultiTouchBinding binding;
			binding = this.GetBinding(observer);
			
			if (binding == null) {
				binding = new MultiTouchBinding();
				binding.Observer = observer;
				binding.Priority = priority;
				this.AddBinding(binding);
			}
			
		}
		
		/// <summary>
		/// Clears all bindings.
		/// </summary>
		public void Clear() {
			
			if (this.Locked) {
				throw new System.InvalidOperationException("Handler is locked.");
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
			
			// return immediately if there are no touches
			if (touches.Count == 0) {
				return touches;
			}
			
			// lock the handler so the bindings array isn't modified while being enumerated
			this.Locked = true;
			
			// check if the bindings need to be sorted
			if (this.Dirty) {
				this.Bindings.Sort(MultiTouchBinding.Compare);
				this.Dirty = false;
			}
			
			// dispatch touches
			foreach (MultiTouchBinding binding in this.Bindings) {
				
				bool isActive;
				isActive = (binding.Observer is UnityEngine.Component) ? (binding.Observer as UnityEngine.Component).gameObject.activeInHierarchy : (binding.Observer != null);

				if (isActive) {
					switch (phase) {
						case TouchPhase.Began:
							binding.Observer.OnTouchesBegan(touches.ToArray());
						break;
						case TouchPhase.Moved:
						case TouchPhase.Stationary:
							binding.Observer.OnTouchesMoved(touches.ToArray());
						break;
						case TouchPhase.Ended:
							binding.Observer.OnTouchesEnded(touches.ToArray());
						break;
						case TouchPhase.Cancelled:
							binding.Observer.OnTouchesCancelled(touches.ToArray());
						break;
					}
				}
				
			}
			
			// unlock the handler to the bindings can be modified again
			this.Locked = false;
			
			// process bindings that were added while the handler was locked
			foreach (MultiTouchBinding binding in this.BindingsToAdd) {
				this.AddBinding(binding);
			}
			this.BindingsToAdd.Clear();
			
			// process bindings that were removed while the handler was locked
			foreach (MultiTouchBinding binding in this.BindingsToRemove) {
				this.RemoveBinding(binding);
			}
			this.BindingsToRemove.Clear();
			
			// return the touches
			return touches;
			
		}
		
		/// <summary>
		/// Removes the binding for the observer.
		/// </summary>
		/// <param name="observer">The observer to remove.</param>
		public void Remove(IMultiTouchObserver observer) {
			
			MultiTouchBinding binding;
			binding = this.GetBinding(observer);
			
			if (binding != null) {
				this.RemoveBinding(binding);
			}
			
		}
		
		#endregion
		
		
		#region Internal Properties
		
		private List<MultiTouchBinding> Bindings {
			get; set;
		}
		
		private List<MultiTouchBinding> BindingsToAdd {
			get; set;
		}
		
		private List<MultiTouchBinding> BindingsToRemove {
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
		
		private void AddBinding(MultiTouchBinding binding) {
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
		
		private MultiTouchBinding GetBinding(IMultiTouchObserver observer) {
			
			foreach (MultiTouchBinding binding in this.Bindings) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			foreach (MultiTouchBinding binding in this.BindingsToAdd) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			foreach (MultiTouchBinding binding in this.BindingsToRemove) {
				if (binding.Observer == observer) {
					return binding;
				}
			}
			
			return null;
			
		}
		
		private void RemoveBinding(MultiTouchBinding binding) {
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
		
		#endregion
		
		
	}
	
}