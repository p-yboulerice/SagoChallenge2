using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Thrust : StateAction {

	private ShipManager m_ShipManager;

    private ShipManager ShipManager {
        get { return m_ShipManager = m_ShipManager ?? this.GetComponentInParent<ShipManager>(); }
    }

	public override void Run() {
		this.ShipManager.ThrustShips();
	}

}
