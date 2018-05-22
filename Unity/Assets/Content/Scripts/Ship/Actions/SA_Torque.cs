using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Torque : StateAction {

	private ShipManager shipManager;

	private enum Direction {
		Right = -1,
        Left = 1
	}

	[SerializeField]
	private Direction direction;
    
    private void OnEnable() {
        shipManager = GetComponentInParent<ShipManager>();
    }

	public override void Run() {
		shipManager.RotateShips((int)direction);
	}

}
