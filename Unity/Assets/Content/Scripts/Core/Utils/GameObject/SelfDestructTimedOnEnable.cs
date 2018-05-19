namespace Juice.Utils {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SelfDestructTimedOnEnable : MonoBehaviour {

		public float Delay = 5;

		void Start() {
			this.transform.parent = null;
			DontDestroyOnLoad(this.gameObject);
			Destroy(this.gameObject, this.Delay);
		}
	}
}