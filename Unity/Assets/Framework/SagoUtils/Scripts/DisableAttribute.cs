namespace SagoUtils {

	using UnityEngine;
	using System;
	using System.Reflection;

	/// <summary>
	/// <para>Conditionally enables/disables the inspector control for the decorated serialized field
	/// based on a function you define.  Make a static method that takes a <see cref="UnityEngine.Object"/> and
	/// returns a bool, pass your class and the method by name.</para>
	/// <para><code>[Disable(typeof(MyClass), "IsFieldDisabled")]</code></para>
	/// <para>and then implement it:</para>
	/// <para><code>
	/// static bool IsFieldDisabled(Object obj) {
	///    return (obj as MyClass).whatever;
	/// }
	/// </code></para>
	/// </summary>
	public class DisableAttribute : PropertyAttribute {

		public delegate bool DisableCallback(UnityEngine.Object obj);
		
		public DisableCallback IsDisabled { get; protected set; }
		
		public int Indent { get; protected set; }
		
		public bool HideWhenDisabled { get; protected set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DisableAttribute"/> class.
		/// </summary>
		/// <param name="type">Type of your script.</param>
		/// <param name="callbackMethodName">Callback method name.</param>
		/// <param name="indent">Indent amount, e.g. 1 to move this property in a bit 'under' another.</param>
		/// <param name="hide">If set to <c>true</c>, don't draw anything when disabled.</param>
		public DisableAttribute(Type type, string callbackMethodName, int indent = 0, bool hide = false) {

			#if UNITY_EDITOR

			BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo mi = type.GetMethod(callbackMethodName, flags);
			if (mi == null) {
				Debug.LogError(string.Format("Could not find method {0}.{1}.  Make sure it is static and spelled correctly in the string you passed.", type, callbackMethodName));
				this.IsDisabled = DefaultIsDisabled;
			} else {
				try {
					this.IsDisabled = Delegate.CreateDelegate(typeof(DisableCallback), mi) as DisableCallback;
				} catch (ArgumentException) {
					Debug.LogError(
						string.Format(
							"Method {0}.{1} must take a UnityEngine.Object parameter, and return a bool, i.e. static bool {1}(UnityEngine.Object obj) ...",
							type, 
							callbackMethodName));
					this.IsDisabled = DefaultIsDisabled;
				}
			}
			
			this.Indent = indent;
			
			this.HideWhenDisabled = hide;

			#endif

		}
		
		public DisableAttribute(Type type, string callbackMethodName, bool hide, int indent = 0) :
			this(type, callbackMethodName, indent, hide) { }

		protected static bool DefaultIsDisabled(UnityEngine.Object obj) { return false; }
		
	}

}
