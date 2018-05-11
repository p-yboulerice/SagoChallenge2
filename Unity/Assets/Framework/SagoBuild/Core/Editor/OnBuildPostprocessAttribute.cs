namespace SagoBuildEditor.Core {
	
	using SagoUtilsEditor;
	using UnityEngine;
	
	public class OnBuildPostprocessAttribute : CallbackOrderAttribute {
		
		public static void Invoke(IBuildProcessor processor) {
			CallbackOrderAttribute.Invoke<OnBuildPostprocessAttribute>(new object[]{ processor });
		}
		
		public OnBuildPostprocessAttribute(int priority = 0) : base(priority) {
			
		}
		
	}
	
}