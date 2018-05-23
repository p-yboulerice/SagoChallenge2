using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;
using Juice.Utils;

public class ShipController : MonoBehaviour {

	#region Properties

	private Rigidbody2D m_Rigidbody2D;

	private Rigidbody2D Rigidbody2D {
		get { return m_Rigidbody2D = m_Rigidbody2D ?? this.GetComponentInParent<Rigidbody2D>(); }
	}

	private FiniteStateMachine m_FiniteStateMachine;

	private FiniteStateMachine FiniteStateMachine {
		get { return m_FiniteStateMachine = m_FiniteStateMachine ?? this.GetComponentInChildren<FiniteStateMachine>(); }
	}

	private Mover m_Mover;

	private Mover Mover {
		get { return m_Mover = m_Mover ?? this.GetComponent<Mover>(); }
	}

	#endregion

	#region Fields

	private const float SCALEDOWNSPEED = 4;

	[SerializeField]
	private float thrustForce;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private State fallingState;

	#endregion

	#region Methods

	public void Thrust() {
		if (this.FiniteStateMachine.CurrentState == fallingState) {
			this.Rigidbody2D.AddForce(transform.up * thrustForce);
		}
		else {
			this.Mover.Velocity = this.Mover.Velocity + (transform.up * thrustForce);
		}
	}

	public void Rotate(float torque) {
		this.Rigidbody2D.AddTorque(torque * rotationSpeed);
	}

	public IEnumerator ScaleDown() {
		float timeElapsed = 0;
		while (timeElapsed <= 1) {

			timeElapsed += Time.deltaTime * SCALEDOWNSPEED;

			transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, timeElapsed);

			yield return 0;
		}
	}

	#endregion

}