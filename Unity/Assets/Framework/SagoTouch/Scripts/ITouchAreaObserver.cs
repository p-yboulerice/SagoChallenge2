namespace SagoTouch {
	
	/// <summary>The interface used to observe events from <see cref="TouchArea" /> objects.</summary>
	public interface ITouchAreaObserver {
		
		
		#region Methods
		
		/// <summary>Called when a touch bound to the <see cref="TouchArea" /> is cancelled.</summary>
		/// <param name="touchArea">The touch area.</param>
		/// <param name="touch">The touch.</param>
		void OnTouchCancelled(TouchArea touchArea, Touch touch);
		
		/// <summary>Called when a touch begins inside the <see cref="TouchArea" />.</summary>
		/// <param name="touchArea">The touch area.</param>
		/// <param name="touch">The touch.</param>
		void OnTouchDown(TouchArea touchArea, Touch touch);
		
		/// <summary>Called when a touch moves into the <see cref="TouchArea"> (i.e. the user drags into the touch area).</summary>
		/// <param name="touchArea">The touch area.</param>
		/// <param name="touch">The touch.</param>
		void OnTouchEnter(TouchArea touchArea, Touch touch);
		
		/// <summary>Called when a touch moves out of the <see cref="TouchArea" /> (i.e. the user drags out of the touch area).</summary>
		/// <param name="touchArea">The touch area.</param>
		/// <param name="touch">The touch.</param>
		void OnTouchExit(TouchArea touchArea, Touch touch);
		
		/// <summary>Called when a touch bound to the <see cref="TouchArea"> ends.</summary>
		/// <param name="touchArea">The touch area.</param>
		/// <param name="touch">The touch.</param>
		void OnTouchUp(TouchArea touchArea, Touch touch);
		
		#endregion
		
		
	}

}
