using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Juice.FSM;

public class SA_Explosion : StateAction {

	#region Properties

	private ShipManager m_ShipManager;

	private ShipManager ShipManager {
		get { return m_ShipManager = m_ShipManager ?? this.GetComponentInParent<ShipManager>(); }
	}

	private ShipController m_ShipController;

	private ShipController ShipController {
		get { return m_ShipController = m_ShipController ?? this.GetComponentInParent<ShipController>(); }
	}

	#endregion

	#region Fields

	[SerializeField]
	private GameObject explosionPrefab;

	#endregion

	#region Methods

	public override void Run() {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		StartCoroutine(Explosion());
    }

    private IEnumerator Explosion() {
        yield return this.ShipController.ScaleDown();
		this.ShipManager.RemoveShip(this.ShipController);      
    }

	#endregion   

}
