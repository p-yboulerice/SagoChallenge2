using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroy : MonoBehaviour {

	[SerializeField]
	private float time;

	private void Awake() {
		Destroy(gameObject, time);
	}

}
