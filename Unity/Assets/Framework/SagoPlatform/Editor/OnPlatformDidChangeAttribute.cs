namespace SagoPlatformEditor {
	
	using SagoUtilsEditor;
	using SagoPlatform;
	
	public class OnPlatformDidChangeAttribute : CallbackOrderAttribute {
		
		
		#region Static Methods
		
		public static void Invoke(Platform oldPlatform, Platform newPlatform) {
			CallbackOrderAttribute.Invoke<OnPlatformDidChangeAttribute>(
				new object[]{ oldPlatform, newPlatform }
			);
		}
		
		#endregion
		
		
		#region Constructor
		
		public OnPlatformDidChangeAttribute(int priority = 0) : base(priority) {
			
		}
		
		#endregion
		
	}
	
}