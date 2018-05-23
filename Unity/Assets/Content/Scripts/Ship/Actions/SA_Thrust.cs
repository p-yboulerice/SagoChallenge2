using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Thrust : StateAction {

	#region Properties

	private ShipManager m_ShipManager;

	private ShipManager ShipManager {
		get { return m_ShipManager = m_ShipManager ?? this.GetComponentInParent<ShipManager>(); }
	}

	#endregion

	#region Methods

	public override void Run() {
		this.ShipManager.ThrustShips();
	}

	#endregion

}
