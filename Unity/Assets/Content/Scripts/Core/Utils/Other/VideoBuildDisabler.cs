namespace Juice.Utils {
	
	using UnityEngine;
	
	/// <summary>
	/// Disables the game object it is in, only if VIDEO_BUILD is set.
	/// </summary>
	public class VideoBuildDisabler : MonoBehaviour {

		#if VIDEO_BUILD

		void OnEnable() {
			gameObject.SetActive(false);
		}

		#endif
		
	}
	
}
