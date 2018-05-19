namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class MoverRadialBounds : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float Size = 1;

		[SerializeField]
		private float HeghtOffset;

		[System.NonSerialized]
		private Mover m_Mover;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponent<Mover>(); }
		}

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private Vector3 OriginalPosition {
			get;
			set;
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.Mover.OnBeforeMoverIsUpdated += this.ApplyBounds;
		}

		void Start() {
			this.OriginalPosition = this.Mover.MoveTransform.localPosition;
		}

		void OnDisable() {
			this.Mover.OnBeforeMoverIsUpdated -= this.ApplyBounds;
		}

		public void ApplyBounds(Mover mover) {
			Vector3 dif = this.Mover.GoToPosition - this.OriginalPosition;

			if (dif.magnitude > this.Size) {
				if (this.Mover.GoToPosition.y <= this.OriginalPosition.y + this.HeghtOffset) {
					this.Mover.GoToPosition = this.OriginalPosition + dif.normalized * this.Size;
				}
			}
		}

		void OnDrawGizmos() {
			Gizmos.DrawWireSphere(this.Transform.position, this.Size);
		}

		#endregion


	}
}