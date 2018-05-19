namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;
	using System;

	public class SA_ToggleGameObject : StateAction {


		#region Fields

		[SerializeField]
		private ToggleGameObjectConfig[] ToggleObjects;


		#endregion


		#region Properties

		public override void Run() {
			foreach (ToggleGameObjectConfig t in this.ToggleObjects) {
				t.GameObject.SetActive(t.ToggleOn);
			}
		}

		#endregion


		#region Methods
		#endregion


	}

	[Serializable]
	public struct ToggleGameObjectConfig {
		public GameObject GameObject;
		public bool ToggleOn;
	}

}