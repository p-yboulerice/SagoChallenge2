namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SA_KnobAndTubeBezierRenderBuildMesh : StateAction {


		#region Fields

		[SerializeField]
		private KnobAndTubeBezierRenderer Bezier;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			if (this.Bezier != null) {
				this.Bezier.RebuildMesh();
			}
		}

		#endregion


	}
}