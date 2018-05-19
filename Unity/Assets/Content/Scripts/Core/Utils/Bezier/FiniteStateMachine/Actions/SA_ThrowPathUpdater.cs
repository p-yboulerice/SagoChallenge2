namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SA_ThrowPathUpdater : StateAction {


		#region Fields

		[SerializeField]
		private Transform MoveTransform;

		[SerializeField]
		private float Speed = 1;

		[SerializeField]
		private ThrowPath m_ThrowPath;

		[SerializeField]
		private bool AimAtMovementDirection;

		[System.NonSerialized]
		private float CurrentTime;

		#endregion


		#region Properties

		public ThrowPath ThrowPath {
			get { return m_ThrowPath; }
			set { m_ThrowPath = value; }
		}

		public bool PathCompleted {
			get { return this.CurrentTime >= 1; }
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.CurrentTime = 0;
		}

		public override void Run() {
			this.CurrentTime += Time.deltaTime * this.Speed;
			if (this.CurrentTime > 1) {
				this.CurrentTime = 1;
			}
			Vector3 pos = this.ThrowPath.GetPositionAtPercent(this.CurrentTime);
			pos.z = this.MoveTransform.position.z;
			this.MoveTransform.position = pos;

			if (this.AimAtMovementDirection) {
				Vector3 direction = this.ThrowPath.GetPositionAtPercent(this.CurrentTime + 0.01f) - this.ThrowPath.GetPositionAtPercent(this.CurrentTime);
				direction.z = 0;
				this.MoveTransform.up = -direction;
			}

		}

		#endregion


	}
}