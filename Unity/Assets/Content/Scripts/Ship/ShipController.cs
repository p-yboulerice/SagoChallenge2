using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

	private Rigidbody2D rigidbody2D;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public void Thrust(Vector2 force)
	{
		rigidbody2D.AddForce(force);
	}

	public void Rotate(float torque)
	{
		rigidbody2D.AddTorque(torque);
	}

	public IEnumerator ScaleDown() {
        float timeElapsed = 0;
        while (timeElapsed <= 1) {

			timeElapsed += Time.deltaTime;

			transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, timeElapsed);

            yield return 0;
        }
    }
}
