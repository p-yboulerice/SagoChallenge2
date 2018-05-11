namespace SagoBuildEditor.Core {
	
	using SagoUtilsEditor;
	using UnityEngine;
	
	public class OnBuildPreprocessAttribute : CallbackOrderAttribute {
		
		public static void Invoke(IBuildProcessor processor) {
			CallbackOrderAttribute.Invoke<OnBuildPreprocessAttribute>(new object[]{ processor });
		}
		
		public OnBuildPreprocessAttribute(int priority = 0) : base(priority) {
			
		}
		
	}
	
}