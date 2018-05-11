namespace SagoDebug {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Draws 3D text labels for debugging purposes.  Does
	/// nothing unless SAGO_DEBUG is set.
	/// </summary>
	public static class Label {


		#region Public

		/// <summary>
		/// Draw the specified text at worldPosition, in the given color, and 
		/// at the given world size.  Does nothing unless SAGO_DEBUG is set.
		/// </summary>
		public static void Draw(string text, Vector3 worldPosition) {
			#if SAGO_DEBUG
			Draw(text, worldPosition, Color.white);
			#endif
		}

		/// <summary>
		/// Draw the specified text at worldPosition, in the given color, and 
		/// at the given world size.  Does nothing unless SAGO_DEBUG is set.
		/// </summary>
		public static void Draw(string text, Vector3 worldPosition, Color color, float size = 1f) {
			#if SAGO_DEBUG
			DebugTextRenderer.Draw(text, worldPosition, color, size);
			#endif
		}

		/// <summary>
		/// Draw the specified text using the given transformation matrix,
		/// in the given color.  Does nothing unless SAGO_DEBUG is set.
		/// </summary>
		public static void Draw(string text, Matrix4x4 mat, Color color) {
			#if SAGO_DEBUG
			DebugTextRenderer.Draw(text, mat, color);
			#endif
		}

		#endregion


		#region Fields

		static TextRenderer s_DebugTextRenderer;

		#endregion


		#region Properties

		static TextRenderer DebugTextRenderer {
			get {
				s_DebugTextRenderer = s_DebugTextRenderer ?? new TextRenderer();
				return s_DebugTextRenderer;
			}
		}

		#endregion


	}
}