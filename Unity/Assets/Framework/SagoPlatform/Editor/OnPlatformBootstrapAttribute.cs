namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using SagoUtilsEditor;
	
	public class OnPlatformBootstrapAttribute : CallbackOrderAttribute {
		
		
		#region Static Methods
		
		public static void Invoke() {
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				CallbackOrderAttribute.Invoke<OnPlatformBootstrapAttribute>(null);
			#endif
		}
		
		#endregion
		
		
		#region Constructors
		
		public OnPlatformBootstrapAttribute(int priority = 0) : base(priority) {
			
		}
		
		#endregion
		
		
	}
	
}