namespace SagoApp {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This class is responsible for controlling frame rate when the app get sent to background and resumes.
	/// This is a temporary fix for game crashing on certain Unity versions.
	/// Unity forum:
	/// https://forum.unity3d.com/threads/5-5-nsinternalinconsistencyexception-crash-on-resume.448023/
	/// https://forum.unity3d.com/threads/multiple-crashes-in-unityappcontroller-rendering-mm-line-249-built-with-unity-5-4-3p2-p4.447530/
	/// Unity Issue Tracker:
	/// https://issuetracker.unity3d.com/issues/ios-crash-in-unityrepaint-when-resuming-the-app-with-different-landscape-orientation-if-running-at-60-fps
	/// JIRA ticket for known crash;
	/// https://sagosago.atlassian.net/browse/SW-85
	/// This is a temporary fix for SW-85. This crash is tied to an app built using certain Unity versions and on handheld devices (ie. iPhones)
	/// with iOS >= 10.0. Basically the target frame rate is set to 30 before the app is sent to background and back to 60 on resume just before
	/// frame update happens.
	/// Perhaps in the future, any frame rate controlling logic could be implemented within this class.
	/// </summary>
	public class FrameRateCrashFix : MonoBehaviour {


		#region Singleton

		static private FrameRateCrashFix _Instance;

		static public FrameRateCrashFix Instance {
			get {

				#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					return _Instance;
				}
				#endif

				if (!_Instance) {
					_Instance = new GameObject("FrameRateCrashFix Singleton").AddComponent<FrameRateCrashFix>();
					DontDestroyOnLoad(_Instance);
				}

				return _Instance;

			}
		}

		#endregion


		#region MonoBehaviour Methods

		void OnApplicationPause(bool paused) {
			if (!paused) {
				// Check it frame rate is still 30 reset frame rate to whatever vaue we saved it to otherwise, do nothing.
				if (Application.targetFrameRate == 30) {
					Application.targetFrameRate = m_TargetFrameRate;
				}
			} else {
				// Save frame rate here
				m_TargetFrameRate = Application.targetFrameRate;
				Application.targetFrameRate = 30;
			}
		}

		#endregion


		#region Internal Methods

		[RuntimeInitializeOnLoadMethod]
		static void TryGettingInstance() {
			// Trying to get singleton instance here to make sure it gets instantiated when the game gets loaded.
			if (Instance) {
				
			}
		}

		#endregion


		#region Internal Fields

		[System.NonSerialized]
		protected int m_TargetFrameRate;

		#endregion


	}

}