namespace SagoUtilsEditor {
	
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
	
	public class CallbackOrderAttribute : System.Attribute {
		
		
		#region Static Methods
		
		public static void Invoke<T>(object[] parameters = null) where T : CallbackOrderAttribute {
			
			BindingFlags flags = (
				BindingFlags.DeclaredOnly | 
				BindingFlags.NonPublic | 
				BindingFlags.Public | 
				BindingFlags.Static
			);
			
			MethodInfo[] methods = System.AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.SelectMany(type => {
					return type
						.GetMethods(flags)
						.Where(method => {
							return method
								.GetCustomAttributes(typeof(T), false)
								.Count() > 0;
						});
				})
				.OrderBy(method => {
					return method
						.GetCustomAttributes(typeof(T), false)
						.Cast<T>()
						.Max( a => a.Priority );
				})
				.ToArray();
				
			foreach (MethodInfo method in methods) {
				try {
					method.Invoke(null, parameters);
				}
				catch (System.Exception e) {
					Debug.LogException(e);
					throw e;
				}
			}
			
		}
		
		#endregion
		
		
		#region Constructor
		
		public CallbackOrderAttribute(int priority = 0) {
			this.Priority = priority;
		}
		
		#endregion
		
		
		#region Properties
		
		public int Priority {
			get; protected set;
		}
		
		#endregion
		
		
	}
	
}