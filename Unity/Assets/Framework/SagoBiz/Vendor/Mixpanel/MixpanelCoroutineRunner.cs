namespace SagoBiz {

	using UnityEngine;

	/// <summary>
	/// As of Unity 5.4, you can no longer AddComponent<MonoBehaviour>()
	/// so this class exists solely so the Mixpanel class can add
	/// a derived MonoBehaviour to run its coroutines.
	/// </summary>
	public class MixpanelCoroutineRunner : MonoBehaviour {

	}

}