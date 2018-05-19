namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;
	using Juice.Depth;

	public class SA_SpawnGameObject : StateAction {


		#region Fields

		[SerializeField]
		private GameObject Prefab;

		[SerializeField]
		private Transform At;

		[SerializeField]
		private bool UseParentLayer;

		[SerializeField]
		private Layer Layer;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			GameObject go = GameObject.Instantiate(this.Prefab);
			go.transform.position = this.At.position;

			if (this.UseParentLayer) {
				Layer layer = go.GetComponent<Layer>();
				this.Layer.Add(layer);
			}
		}

		#endregion


	}
}