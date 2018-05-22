using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {

	#region Fields

	[SerializeField]
	private GameObject shipPrefab;

	[SerializeField]
	private Transform shipSpawnAnchor;

	[Space]
	[SerializeField]
	private int maxShips;

	private List<ShipController> shipControllers = new List<ShipController>();

	#endregion

	#region Methods

	private void Awake() {
		SpawnShip();
	}

	public void SpawnShip() {

		// Instantiates Ship Prefab and adds it's ShipController component to shipController list
		if (shipControllers.Count < maxShips) {
			shipControllers.Add(Instantiate(shipPrefab, shipSpawnAnchor.position, Quaternion.identity, shipSpawnAnchor).GetComponent<ShipController>());
		}
	}

	public void ThrustShips() {
		for (int i = 0; i < shipControllers.Count; i++) {
			shipControllers[i].Thrust();
		}
	}

	public void RotateShips(float direction) {
		for (int i = 0; i < shipControllers.Count; i++) {
			shipControllers[i].Rotate(direction);
        }
	}

	public void RemoveShip(ShipController ship) {
		shipControllers.Remove(ship);
		Destroy(ship.gameObject);
	}

	#endregion
}
