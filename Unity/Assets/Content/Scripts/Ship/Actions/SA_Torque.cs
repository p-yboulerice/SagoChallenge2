using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Torque : StateAction {

	#region Properties

	private ShipManager m_ShipManager;

	private ShipManager ShipManager {
		get { return m_ShipManager = m_ShipManager ?? this.GetComponentInParent<ShipManager>(); }
	}

	#endregion

	#region Fields

	private enum Direction {
		Right = -1,
		Left = 1
	}

	[SerializeField]
	private Direction direction;

	#endregion

	#region Methods

	public override void Run() {
		this.ShipManager.RotateShips((int)direction);
	}

	#endregion

}
