using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Thrust : StateAction {

	private ShipManager shipManager;

	private void OnEnable() {
		shipManager = GetComponentInParent<ShipManager>();
	}

	public override void Run() {
		shipManager.ThrustShips();
	}

}
