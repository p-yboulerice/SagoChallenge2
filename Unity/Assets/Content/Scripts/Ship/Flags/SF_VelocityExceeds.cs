using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SF_VelocityExceeds : StateFlag {

	#region Properties

	private Rigidbody2D m_Rigidbody2D;

	private Rigidbody2D Rigidbody2D {
		get { return m_Rigidbody2D = m_Rigidbody2D ?? this.GetComponentInParent<Rigidbody2D>(); }
	}

	#endregion

	#region Fields

	[SerializeField]
	private float velocityThreshold;

	#endregion

	#region Methods

	public override bool IsActive {
		get {
			if (this.Rigidbody2D.velocity.magnitude > velocityThreshold) {
				return true;
			}

			return false;
		}
	}

	#endregion

}
