namespace SagoBuildEditor.Core {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The MenuItemUtil class provides common functionality for menu item methods.
	/// </summary>
	public static class MenuItemUtil {
		
		
		#region Static Methods
		
		/// <summary>
		/// Checks whether a menu item should be enabled for the specified build platform.
		/// </summary>
		/// <param name="buildPlatform">
		/// The build target.
		/// </param>
		public static bool IsMenuItemEnabledForBuildPlatform(Platform buildPlatform) {
			return (
				PlatformUtil.ActivePlatform == buildPlatform && 
				EditorApplication.isCompiling == false
			);
		}
		
		#endregion
		
		
	}
	
}