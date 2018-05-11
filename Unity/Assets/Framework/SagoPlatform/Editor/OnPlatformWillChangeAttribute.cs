namespace SagoPlatformEditor {
	
	using SagoUtilsEditor;
	using SagoPlatform;
	
	public class OnPlatformWillChangeAttribute : CallbackOrderAttribute {
		
		
		#region Static Methods
		
		public static void Invoke(Platform oldPlatform, Platform newPlatform) {
			CallbackOrderAttribute.Invoke<OnPlatformWillChangeAttribute>(
				new object[]{ oldPlatform, newPlatform }
			);
		}
		
		#endregion
		
		
		#region Constructor
		
		public OnPlatformWillChangeAttribute(int priority = 0) : base(priority) {
			
		}
		
		#endregion
		
	}
	
}

