namespace SagoDebug {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// <para>Now largely deprecated.  Was a wrapper/replacement for 
	/// <see cref="UnityEngine.Debug"/>, largely for the Log() methods.</para>
	/// <para>The Log() methods are now directly passed through to UnityEngine.Debug
	/// and the hook into DevUI is handled directly.</para>
	/// <para>For the remaining methods, e.g. DrawBounds(), use them directly
	/// with SagoDebug.Debug.DrawBounds, instead of aliasing Debug with a using command.</para>
	/// <para>Only enabled if SAGO_DEBUG is defined in the player settings when the build is made.</para>
	/// </summary>
	public static class Debug {
		
		
		#region Public Properties (UnityEngine.Debug Wrappers)
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.developerConsoleVisible</para>
		/// Opens or closes developer console.
		/// </summary>
		/// <value><c>true</c> if developer console visible; otherwise, <c>false</c>.</value>
		public static bool developerConsoleVisible {
			get {
				return UnityEngine.Debug.developerConsoleVisible;
			}
			set {
				UnityEngine.Debug.developerConsoleVisible = value;
			}
		}

		/// <summary>
		/// <para>Wraps UnityEngine.Debug.isDebugBuild</para>
		/// In the Build Settings dialog there is a check box called "Development Build".
		/// If it is checked isDebugBuild will be true. In the editor isDebugBuild always returns true. 
		/// It is recommended to remove all calls to Debug.Log when deploying a game, 
		/// this way you can easily deploy beta builds with debug prints and final builds without.
		/// </summary>
		/// <value><c>true</c> if is debug build; otherwise, <c>false</c>.</value>
		public static bool isDebugBuild {
			get {
				return UnityEngine.Debug.isDebugBuild;
			}
		}

		#endregion
		
		
		#region Public Methods (UnityEngine.Debug Wrappers)
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.AssertFormat()</para>
		/// Assert the specified condition.  Message of type LogType.Assert is logged.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		[System.Obsolete("Use UnityEngine.Debug.AssertFormat instead of Debug.Assert")]
		public static void Assert(bool condition) {
			#if UNITY_5
				UnityEngine.Debug.AssertFormat(condition, "");
			#else
				if (!condition) {
					LogError("Failed assertion");
				}
			#endif
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.Assert()</para>
		/// Assert the specified condition.  Message of type LogType.Assert is logged.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		/// <param name="message">Message.</param>
		[System.Obsolete("Use UnityEngine.Debug.AssertFormat instead of Debug.Assert")]
		public static void Assert(bool condition, string message) {
			#if UNITY_5
				UnityEngine.Debug.AssertFormat(condition, message);
			#else
				if (!condition) {
					LogError("Failed assertion: " + message);
				}
			#endif
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.Assert()</para>
		/// Assert the specified condition.  Message of type LogType.Assert is logged.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void AssertFormat(bool condition, string message, params object[] args) {
			#if UNITY_5
				UnityEngine.Debug.AssertFormat(condition, message, args);
			#else
				if (!condition) {
					LogError(string.Format("Failed assertion: " + message, args));
				}
			#endif
		}

		/// <summary>
		/// <para>Wraps UnityEngine.Debug.Assert()</para>
		/// Assert the specified condition.  Message of type LogType.Assert is logged.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		/// <param name="context">Context.</param>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void AssertFormat(bool condition, Object context, string message, params object[] args) {
			#if UNITY_5
				UnityEngine.Debug.AssertFormat(condition, context, message, args);
			#else
				if (!condition) {
					LogError(string.Format("Failed assertion: " + message, args));
				}
			#endif
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.Break()</para>
		/// Pauses the editor.		
		/// This is useful when you want to check certain values on the inspector and you are not able to pause it manually.
		/// </summary>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void Break() {
			UnityEngine.Debug.Break();
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.ClearDeveloperConsole()</para>
		/// Clears the developer console.
		/// </summary>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void ClearDeveloperConsole() {
			UnityEngine.Debug.ClearDeveloperConsole();
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.DrawLine()</para>
		/// Draws a line between specified start and end points.
		/// The line will be drawn in the scene view of the editor. 
		/// If gizmo drawing is enabled in the game view, the line will also be drawn there. 
		/// The duration is the time (in seconds) for which the line will be visible after it is first displayed. 
		/// A duration of zero shows the line for just one frame.
		/// <remarks>Note: This is for debugging playmode only. Editor gizmos should be drawn with Gizmos.Drawline or Handles.DrawLine instead.</remarks>
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true) {
			UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
		}
		
		/// <summary>
		/// <para>Wraps UnityEngine.Debug.DrawRay()</para>
		/// Draws a line from start to start + dir in world coordinates.
		/// The duration parameter determines how long the line will be visible after the frame it is drawn. 
		/// If duration is 0 (the default) then the line is rendered 1 frame.
		/// If depthTest is set to true then the line will be obscured by other objects in the scene that are nearer to the camera.
		/// The line will be drawn in the scene view of the editor. 
		/// If gizmo drawing is enabled in the game view, the line will also be drawn there.
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true) {
			UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
		}
		
		/// <summary>
		/// <para>This wraps UnityEngine.Debug.Log(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// Logs message to the Unity Console.
		/// When you select the message in the console a connection to the context object will be drawn. 
		/// This can be useful for locating the object on which an error occurs.
		/// When the message is a string, rich text markup can be used to add emphasis.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void Log(object message, UnityEngine.Object context = null) {
			UnityEngine.Debug.Log(message, context);
		}

		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogAssertion(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// Logs message to the Unity Console.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="context">Context.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogAssertion(object message, UnityEngine.Object context = null) {
			UnityEngine.Debug.LogAssertion(message, context);
		}

		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogAssertion(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// Logs message to the Unity Console.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogAssertionFormat(string format, params object[] args) {
			LogAssertionFormat(null, format, args);
		}

		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogAssertion(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// Logs message to the Unity Console.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogAssertionFormat(Object context, string format, params object[] args) {
			UnityEngine.Debug.LogAssertionFormat(context, format, args);
		}

		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogError(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// A variant of Debug.Log that logs an error message to the Unity Console.
		/// When you select the message in the console a connection to the context object will be drawn. 
		/// This can be useful for locating the object on which an error occurs.
		/// When the message is a string, rich text markup can be used to add emphasis.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogError(object message, UnityEngine.Object context = null) {
			UnityEngine.Debug.LogError(message, context);
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogErrorFormat (behaves like string.Format())
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogErrorFormat(object message, params object[] args) {
			LogError(string.Format(message.ToString(), args));
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogErrorFormat (behaves like string.Format())
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogErrorFormat(UnityEngine.Object context, object message, params object[] args) {
			LogError(string.Format(message.ToString(), args), context);
		}
		
		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogException(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// A variant of Debug.Log that logs an exception message to the Unity Console.
		/// When you select the message in the console a connection to the context object will be drawn. 
		/// This can be useful for locating the object on which an error occurs.
		/// When the message is a string, rich text markup can be used to add emphasis.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogException(System.Exception exception, Object context = null) {
			UnityEngine.Debug.LogException(exception, context);
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogFormat (behaves like string.Format())
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogFormat(object message, params object[] args) {
			Log(string.Format(message.ToString(), args));
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogFormat (behaves like string.Format())
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogFormat(UnityEngine.Object context, object message, params object[] args) {
			Log(string.Format(message.ToString(), args), context);
		}
		
		/// <summary>
		/// <para>This wraps UnityEngine.Debug.LogWarning(), and adds the message to the <see cref="DevUI"/> log.</para>
		/// A variant of Debug.Log that logs a warning message to the Unity Console.
		/// When you select the message in the console a connection to the context object will be drawn. 
		/// This can be useful for locating the object on which an error occurs.
		/// When the message is a string, rich text markup can be used to add emphasis.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogWarning(object message, UnityEngine.Object context = null) {
			UnityEngine.Debug.LogWarning(message, context);
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogWarningFormat (behaves like string.Format())
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogWarningFormat(object message, params object[] args) {
			LogWarning(string.Format(message.ToString(), args));
		}
		
		/// <summary>
		/// Emulates UnityEngine.LogWarningFormat (behaves like string.Format())
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		[System.Obsolete("This is now equivalent to UnityEngine.Debug; use that instead")]
		public static void LogWarningFormat(UnityEngine.Object context, object message, params object[] args) {
			LogWarning(string.Format(message.ToString(), args), context);
		}
		
		#endregion
		
		
		#region Public Methods (Beyond UnityEngine.Debug)
		
		/// <summary>
		/// Draws a '+' in the XY plane over the given location.  Useful for visualizing positions.
		/// The duration parameter determines how long the cross will be visible after the frame it is drawn. 
		/// If duration is 0 (the default) then the cross is rendered 1 frame.
		/// If depthTest is set to true then the cross will be obscured by other objects in the scene that are nearer to the camera.
		/// The cross will be drawn in the scene view of the editor. 
		/// If gizmo drawing is enabled in the game view, the cross will also be drawn there.
		/// </summary>
		/// <param name="center">Point in world space that the cross is centered over.</param>
		/// <param name="color">Color of the cross.</param>
		/// <param name="size">Size.</param>
		/// <param name="duration">How long the cross will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the cross be obscured by objects closer to the camera?</param>
		public static void DrawCross(Vector3 center, Color color = default(Color), float size = 0.1f, float duration = 0.0f, bool depthTest = true) {
			UnityEngine.Debug.DrawLine(center + new Vector3(0.0f, size, 0.0f), center - new Vector3(0.0f, size, 0.0f), color, duration, depthTest);
			UnityEngine.Debug.DrawLine(center + new Vector3(size, 0.0f, 0.0f), center - new Vector3(size, 0.0f, 0.0f), color, duration, depthTest);
		}
		
		/// <summary>
		/// Draws the bounds provided.
		/// The duration parameter determines how long the bounds will be visible after the frame they are drawn. 
		/// If duration is 0 (the default) then the bounds are rendered 1 frame.
		/// If depthTest is set to true then the bounds will be obscured by other objects in the scene that are nearer to the camera.
		/// </summary>
		/// <param name="b">The bounds to draw (world space).</param>
		/// <param name="color">Color of the bounds.</param>
		/// <param name="duration">How long the bounds will be visible for (in seconds).</param>
		/// <param name="depthTest">If set to <c>true</c>, the bounds will be obscured by objects closer to the camera.</param>
		public static void DrawBounds(Bounds b, Color color = default(Color), float duration = 0.0f, bool depthTest = true) {
		
			Vector3 p0 = new Vector3(b.min.x, b.min.y, b.min.z);
			Vector3 p1 = new Vector3(b.min.x, b.max.y, b.min.z);
			Vector3 p2 = new Vector3(b.max.x, b.max.y, b.min.z);
			Vector3 p3 = new Vector3(b.max.x, b.min.y, b.min.z);
			Vector3 p4 = new Vector3(b.min.x, b.min.y, b.max.z);
			Vector3 p5 = new Vector3(b.min.x, b.max.y, b.max.z);
			Vector3 p6 = new Vector3(b.max.x, b.max.y, b.max.z);
			Vector3 p7 = new Vector3(b.max.x, b.min.y, b.max.z);
			
			UnityEngine.Debug.DrawLine(p0, p1, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p1, p2, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p2, p3, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p3, p0, color, duration, depthTest);
				
			UnityEngine.Debug.DrawLine(p4, p5, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p5, p6, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p6, p7, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p7, p4, color, duration, depthTest);
			
			UnityEngine.Debug.DrawLine(p0, p4, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p1, p5, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p2, p6, color, duration, depthTest);
			UnityEngine.Debug.DrawLine(p3, p7, color, duration, depthTest);
		}
		
		/// <summary>
		/// Returns a string representation of a color as RGBA in hex, e.g. "ffc30aff"
		/// </summary>
		/// <returns>The color as a string of the concatenated RGBA components in hex.</returns>
		/// <param name="color">The color to translate</param>
		public static string ColorToHex(Color color) {
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}",
			                     Mathf.FloorToInt(color.r * 255),
			                     Mathf.FloorToInt(color.g * 255),
			                     Mathf.FloorToInt(color.b * 255),
			                     Mathf.FloorToInt(color.a * 255));
		}
		
		/// <summary>
		/// An alternative to Log, which sets the color of the output text.
		/// Otherwise it behaves the same as the uncolored Log (e.g. wraps 
		/// UnityEngine.Debug.Log, and adds the message to the DevUI).
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="color">Color used to display the text.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void Log(object message, Color color, UnityEngine.Object context = null) {
			string msg = string.Format("<color=#{0}>{1}</color>", ColorToHex(color), message);
			UnityEngine.Debug.Log(msg, context);
		}
		
		#endregion
		
		
	}
}
