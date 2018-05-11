namespace SagoApp.Project {
	
	using SagoCore.AssetBundles;
	using SagoCore.Scenes;
	using SagoLocalization;
	using SagoNavigation;
	using UnityEngine;
	using UnityEngine.UI;
	
	/// <summary>
	/// SceneController for error handling.
	/// Both Content and Project scenes can transition into Error scene.
	/// </summary>
	public class NavigateToErrorSceneController : SceneController {
		
		
		#region Serialized Fields
		
		[SerializeField]
		private Button m_CloseButton;
		
		[SerializeField]
		private Text m_ErrorText;
		
		[SerializeField]
		private LocalizedStringReference m_LocalizedStringReference;
		
		#endregion
		
		
		#region Monobehaviour methods
		
		private void Close() {
			if (ProjectNavigator.Instance && ProjectNavigator.Instance.IsReady) {
				if (m_CloseButton) {
					m_CloseButton.onClick.RemoveAllListeners();
				}
				ProjectNavigator.Instance.NavigateToProject();
			}
		}
		
		private void Start() {
			if (ProjectNavigator.Instance) {
				
				// close button
				if (m_CloseButton) {
					m_CloseButton.onClick.AddListener(Close);
				}
				
				// error text
				if (m_ErrorText) {
					
					ProjectNavigatorError error;
					error = ProjectNavigator.Instance.Error;
					
					// error = ProjectNavigatorError.OdrErrorNoWiFi;
					// error = ProjectNavigatorError.OdrErrorNoInternet;
					// error = ProjectNavigatorError.LowDiskSpace;
					
					switch (error) {
						case ProjectNavigatorError.OdrErrorNoWiFi:
							m_ErrorText.text = m_LocalizedStringReference.GetString(
								"NavigateToErrorSceneController.Error.Text.OdrErrorNoWiFi", 
								"Please turn on WiFi to continue downloading!"
							);
						break;
						case ProjectNavigatorError.OdrErrorNoInternet:
							m_ErrorText.text = m_LocalizedStringReference.GetString(
								"NavigateToErrorSceneController.Error.Text.OdrErrorNoInternet", 
								"Oops. No internet connection!"
							);
						break;
						case ProjectNavigatorError.LowDiskSpace:
							m_ErrorText.text = m_LocalizedStringReference.GetString(
								"NavigateToErrorSceneController.Error.Text.LowDiskSpace", 
								"No space left. Try clearing some unused apps!"
							);
						break;
						default:
							m_ErrorText.text = m_LocalizedStringReference.GetString(
								"NavigateToErrorSceneController.Error.Text.Default", 
								"Something went wrong. Please try again!"
							);
						break;
					}
				}
				
			}
		}
		
		#endregion
		
		
	}
	
}