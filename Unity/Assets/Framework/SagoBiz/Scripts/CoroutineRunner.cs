namespace SagoBiz {

	using System.Collections;
	using UnityEngine;

	public class CoroutineRunner : MonoBehaviour {


		#region Singleton

		private static CoroutineRunner _Instance;

		public static CoroutineRunner Instance {
			get {
				#if UNITY_EDITOR

				if (UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
					if (_Instance == null) {
						_Instance = new GameObject().AddComponent<CoroutineRunner>();
						DontDestroyOnLoad(_Instance);
						if (SagoBiz.Controller.Instance) {
							_Instance.transform.parent = SagoBiz.Controller.Instance.transform;
						}
						_Instance.gameObject.name = _Instance.GetType().Name;
					}
				}

				#else
									
					if (_Instance == null) {
						_Instance = new GameObject().AddComponent<CoroutineRunner>();
						_Instance.gameObject.name = _Instance.GetType().Name;
						DontDestroyOnLoad(_Instance);
						if (SagoBiz.Controller.Instance) {
							_Instance.transform.parent = SagoBiz.Controller.Instance.transform;
						}
					}
					
				#endif
				return _Instance;
			}
		}

		#endregion


	}

}
