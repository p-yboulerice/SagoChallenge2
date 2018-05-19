namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class RotatorBounds : MonoBehaviour {


		#region Fields

		[TweakerAttribute]
		[SerializeField]
		private float Min = -90f;

		[TweakerAttribute]
		[SerializeField]
		private float Max = 90f;

		[SerializeField]
		private bool SolidBound;

		[System.NonSerialized]
		private Rotator m_Rotator;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		private Rotator Rotator {
			get { return m_Rotator = m_Rotator ?? this.GetComponent<Rotator>(); }
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.Rotator.OnBeforeRotatorIsUpdated += this.ApplyBounds;
		}

		void OnDisable() {
			this.Rotator.OnBeforeRotatorIsUpdated -= this.ApplyBounds;
		}

		private void ApplyBounds(Rotator rotator) {

			if (Mathf.Abs(this.Rotator.GoToAngle) < Mathf.Abs(this.Min)) {
				this.Rotator.GoToAngle = this.Min;
			} else if (Mathf.Abs(this.Rotator.GoToAngle) > Mathf.Abs(this.Max)) {
				this.Rotator.GoToAngle = this.Max;
			}

			if (this.SolidBound) {
				if (this.Rotator.CurrentAngle < this.Min) {
					this.Rotator.CurrentAngle = this.Min;
				} else if (this.Rotator.CurrentAngle > this.Max) {
					this.Rotator.CurrentAngle = this.Max;
				}
			}
		}

		void OnDrawGizmos() {

			Gizmos.color = Color.magenta;

			Vector3 boundVector;
			boundVector = new Vector3(Mathf.Cos(Mathf.PI * ((this.Min + 90) / 360f) * 2), Mathf.Sin(Mathf.PI * ((this.Min + 90) / 360f) * 2), 0);
			Gizmos.DrawLine(this.Transform.position, this.Transform.position + boundVector);

			boundVector = new Vector3(Mathf.Cos(Mathf.PI * ((this.Max + 90) / 360f) * 2), Mathf.Sin(Mathf.PI * ((this.Max + 90) / 360f) * 2), 0);
			Gizmos.DrawLine(this.Transform.position, this.Transform.position + boundVector);

		}

		#endregion


	}
}