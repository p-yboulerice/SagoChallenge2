namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>The inteface that can be used by code outside of <c>SagoTouch</c> to decouple touches.</summary>
	public interface ITouch {
		
		
		#region Properties
		
		/// <summary>Gets the touch phase.</summary>
		TouchPhase Phase { get; }
		
		/// <summary>Gets the touch position, in pixels.</summary>
		Vector2 Position { get; }
		
		#endregion
		
		
	}
	
}