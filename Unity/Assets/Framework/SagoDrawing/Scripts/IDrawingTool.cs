namespace SagoDrawing {

	using Touch = SagoTouch.Touch;
		
	/// <summary>
	/// The IDrawingTool interface defines the methods required implement a drawing tool.
	/// </summary>
	public interface IDrawingTool {
		
		
		/// <summary>
		/// Called when the drawing tool should render to the drawing surface. The 
		/// drawing tool should wait for this method before drawing (it should not 
		/// do any rendering directly from any of the touch methods).
		/// </summary>
		void OnRender(DrawingSurface surface);
		
		/// <summary>
		/// Called when a touch on the drawing surface begins.
		/// </summary>
		bool OnTouchBegan(DrawingSurface surface, Touch touch);
		
		/// <summary>
		/// Called when a touch on the drawing surface moves.
		/// </summary>
		void OnTouchMoved(DrawingSurface surface, Touch touch);
		
		/// <summary>
		/// Called when a touch on the drawing surface ends.
		/// </summary>
		void OnTouchEnded(DrawingSurface surface, Touch touch);
		
		/// <summary>
		/// Called when a touch on the drawing surface is cancelled.
		/// </summary>
		void OnTouchCancelled(DrawingSurface surface, Touch touch);
		
		
	}
	
}