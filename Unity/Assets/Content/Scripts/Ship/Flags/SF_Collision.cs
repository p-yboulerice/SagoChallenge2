using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SF_Collision : StateFlag {

	#region Properties

	[SerializeField]
	private Collider2D m_Collider;

	private Collider2D Collider {
		get { return m_Collider = m_Collider ?? GetComponentInParent<PolygonCollider2D>(); }
	}

	#endregion

	#region Fields

	[SerializeField]
	private ContactFilter2D filter;

	private Collider2D[] collisions = new Collider2D[4];

	#endregion

	#region Properties

	public override bool IsActive {
		get {
			Physics2D.OverlapCollider(this.Collider, filter, collisions);

			for (int i = 0; i < collisions.Length; i++) {
				if (collisions[i] != null) {
					collisions[i].GetComponent<ShipController>();
					return true;
				}
			}

			return false;
		}
	}

	#endregion

}
