namespace SagoApp.Content {

	using SagoApp.Project;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Portrait apps (e.g. Monsters, Hats) can use this in
	/// their custom ContentInfo and SceneController's to help 
	/// change the screen orientation consistently across apps.
	/// </summary>
	public class PortraitAppHelper {
		

		#region Public Methods

		/// <summary>
		/// Yield on this before setting m_IsReady in your app's SceneController.
		/// </summary>
		/// <remarks>
		/// TODO: patch Unity when they fix this issue.
		/// This is a temporary fix for this Unity bug: 
		/// https://issuetracker.unity3d.com/issues/ios-changing-the-screen-orientation-via-a-script-sometimes-results-in-corrupted-view-on-ios-10
		/// which happens sometimes when you set the screen orientation on iPhones running iOS 10.
		/// This fix works by rotating the screen out of the glitched half-black state, and back again to the desired
		/// orientation.
		/// </remarks>
		public static IEnumerator SceneControllerIsReadyAsync() {

			#if UNITY_IOS

			if (!ProjectInfo.Instance.IsStandaloneProject) {
				if (SystemInfo.deviceModel.Contains("iPhone") && SystemInfo.operatingSystem.Contains("iOS 10.")) {

					ScreenOrientation orientation;
					orientation = ScreenOrientation.Portrait;

					Screen.orientation = orientation;
					yield return new WaitForSeconds(1.0f);

					Screen.orientation = ScreenOrientation.LandscapeLeft;
					yield return new WaitForSeconds(1.0f);

					Screen.orientation = orientation;
					yield return new WaitForSeconds(1.0f);

					Screen.orientation = orientation;
				}
			}

			#endif

			yield break;
		}

		/// <summary>
		/// Call this from your app's ContentInfo.OnProjectNavigatorWillActivateContent.
		/// </summary>
		public void TransitionIntoPortraitApp() {
			if (!ProjectInfo.Instance.IsStandaloneProject) {
				SetScreenOrientation();
			}
		}

		/// <summary>
		/// Call this from your app's ContentInfo.OnProjectNavigatorDidDeactivateContent
		/// </summary>
		public void TransitionOutOfPortraitApp() {
			if (!ProjectInfo.Instance.IsStandaloneProject) {
				UndoScreenOrientation();
			}
		}

		#endregion


		#region Fields

		[System.NonSerialized]
		private ScreenOrientation m_WorldScreenOrientation;

		[System.NonSerialized]
		private Queue<bool> m_WorldAutoRotateQueue = new Queue<bool>(4);

		#endregion


		#region Screen Orientation

		private void SetScreenOrientation() {
			m_WorldScreenOrientation = Screen.orientation;

			Screen.autorotateToPortrait = SaveRotationSetting(Screen.autorotateToPortrait, false);
			Screen.autorotateToPortraitUpsideDown = SaveRotationSetting(Screen.autorotateToPortraitUpsideDown, false);
			Screen.autorotateToLandscapeLeft = SaveRotationSetting(Screen.autorotateToLandscapeLeft, false);
			Screen.autorotateToLandscapeRight = SaveRotationSetting(Screen.autorotateToLandscapeRight, false);
			Screen.orientation = ScreenOrientation.Portrait;
			ApplyAllLayout();
		}

		private void UndoScreenOrientation() {
			Screen.autorotateToPortrait = RestoreRotationSetting();
			Screen.autorotateToPortraitUpsideDown = RestoreRotationSetting();
			Screen.autorotateToLandscapeLeft = RestoreRotationSetting();
			Screen.autorotateToLandscapeRight = RestoreRotationSetting();
			Screen.orientation = m_WorldScreenOrientation;
			ApplyAllLayout();
		}

		private bool SaveRotationSetting(bool current, bool retVal) {
			m_WorldAutoRotateQueue.Enqueue(current);
			return retVal;
		}

		private bool RestoreRotationSetting() {
			return m_WorldAutoRotateQueue.Dequeue();
		}

		private void ApplyAllLayout() {
			LoadingSpinner loadingSpinner = Object.FindObjectOfType<LoadingSpinner>();
			if (loadingSpinner) {
				foreach (var layout in loadingSpinner.GetComponentsInChildren<SagoLayout.LayoutComponent>()) {
					layout.Apply();
				}
			}
		}

		#endregion


	}
	
}
