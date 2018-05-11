namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// Defines methods used to observe single touch events 
	/// dispatched by the <see cref="TouchDispatcher" />.
	/// </summary>
	public interface ISingleTouchObserver {
		
		
		#region Methods
		
		/// <summary>
		/// Called whenever a touch begins. If this method returns true, the touch 
		/// will be bound to the observer. If the observer swallows touches, other 
		/// observers with lower priority will not receive any events for the touch. 
		/// If this method returns false, the touch will not be bound to the observer
		/// and the observer will not receive subsequent events for the touch.
		/// </summary>
		/// <param name="touch">The touch that began.</param>
		/// <returns><c>true</c> if the touch should be bound to the observer, otherwise <c>false</c>.</returns>
		bool OnTouchBegan(Touch touch);
		
		/// <summary>Called whenever bound touch bound to this observer moves.</summary>
		/// <param name="touch">The touch that moved.</param>
		void OnTouchMoved(Touch touch);
		
		/// <summary>Called whenever bound touch bound to this observer ends.</summary>
		/// <param name="touch">The touch that ended.</param>
		void OnTouchEnded(Touch touch);
		
		/// <summary>Called whenever bound touch bound to this observer is cancelled.</summary>
		/// <param name="touch">The touch that was cancelled.</param>
		void OnTouchCancelled(Touch touch);
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The interface used to observe unbound single touch events from the <see cref="TouchDispatcher" />.
	/// </summary>
	public interface IUnboundSingleTouchObserver : ISingleTouchObserver {
		
		
		#region Methods
		
		/// <summary>
		/// Called whenever an unbound touch begins. If this method returns true, 
		/// the touch will be bound to the observer. If the observer swallows 
		/// touches, other observers with lower priority will not receive any 
		/// events for the touch. If this method returns false, the touch will 
		/// not be bound to the observer.
		/// </summary>
		/// <param name="touch">The touch that began.</param>
		/// <returns><c>true</c> if the touch should be bound to the observer, otherwise <c>false</c>.</returns>
		bool OnUnboundTouchBegan(Touch touch);
		
		/// <summary>
		/// Called whenever an unbound touch moves. If this method returns true, 
		/// the touch will be bound to the observer. If the observer swallows 
		/// touches, other observers with lower priority will not receive any 
		/// events for the touch. If this method returns false, the touch will 
		/// not be bound to the observer.
		/// </summary>
		/// <param name="touch">The touch that moved.</param>
		/// <returns><c>true</c> if the touch should be bound to the observer, otherwise <c>false</c>.</returns>
		bool OnUnboundTouchMoved(Touch touch);
		
		/// <summary>
		/// Called whenever an unbound touch ends. If this method returns true, 
		/// the touch will be bound to the observer. If the observer swallows 
		/// touches, other observers with lower priority will not receive any 
		/// events for the touch. If this method returns false, the touch will 
		/// not be bound to the observer.
		/// </summary>
		/// <param name="touch">The touch that ended.</param>
		/// <returns><c>true</c> if the touch should be bound to the observer, otherwise <c>false</c>.</returns>
		bool OnUnboundTouchEnded(Touch touch);
		
		/// <summary>
		/// Called whenever an unbound touch is cancelled. If this method returns 
		/// true, the touch will be bound to the observer. If the observer swallows 
		/// touches, other observers with lower priority will not receive any 
		/// events for the touch. If this method returns false, the touch will 
		/// not be bound to the observer.
		/// </summary>
		/// <param name="touch">The touch that was cancelled.</param>
		/// <returns><c>true</c> if the touch should be bound to the observer, otherwise <c>false</c>.</returns>
		bool OnUnboundTouchCancelled(Touch touch);
		
		#endregion
		
		
	}
	
}