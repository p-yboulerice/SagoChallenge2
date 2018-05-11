namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// Defines methods used to observe multi touch events 
	/// dispatched by the <see cref="TouchDispatcher" />.
	/// </summary>
	public interface IMultiTouchObserver {
		
		
		#region Methods
		
		/// <summary>Called when one or more touches begin.</summary>
		/// <param name="touches">The array of touches that begin.</param>
		void OnTouchesBegan(Touch[] touches);
		
		/// <summary>Called when one or more touches move.</summary>
		/// <param name="touches">The array of touches that moved.</param>
		void OnTouchesMoved(Touch[] touches);
		
		/// <summary>Called when one or more touches end.</summary>
		/// <param name="touches">The array of touches that ended.</param>
		void OnTouchesEnded(Touch[] touches);
		
		/// <summary>Called when one or more touches are cancelled.</summary>
		/// <param name="touches">The array of touches that were cancelled.</param>
		void OnTouchesCancelled(Touch[] touches);
		
		#endregion
		
		
	}
	
}