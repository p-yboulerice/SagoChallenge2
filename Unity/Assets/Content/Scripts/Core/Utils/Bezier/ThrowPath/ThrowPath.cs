namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class ThrowPath : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float HeightToMin = 0.43f;

		[SerializeField]
		private float HeightToMax = 1f;

		[SerializeField]
		private BezierRenderer Bezier;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		public void Initialize(Vector3 from, Vector3 to) {

			this.Transform.position = Vector3.zero;

			Vector3 dif = to - from;

			this.Bezier.StartPoint.position = from;
			this.Bezier.EndPoint.position = to;


			float randomUp = Random.Range(this.HeightToMin, this.HeightToMax);

			this.Bezier.StartControl.position = from + dif * 0.25f + Vector3.up * randomUp;
			this.Bezier.EndControl.position = from + dif * 0.75f + Vector3.up * randomUp ;
		
		}

		public Vector3 GetPositionAtPercent(float percent) {
			return this.Bezier.PointAt(percent);
		}


		#endregion


	}
}