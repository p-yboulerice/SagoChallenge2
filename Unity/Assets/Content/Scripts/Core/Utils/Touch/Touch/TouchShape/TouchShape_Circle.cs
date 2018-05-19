namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public class TouchShape_Circle : TouchShape {

		#region Fields

		[SerializeField]
		private float Radius = 1;

		[SerializeField]
		private Vector2 m_Offset;

		#endregion


		#region Properties

		private Vector2 Offset {
			get { return (Vector2)(this.Transform.right * m_Offset.x + this.Transform.up * m_Offset.y);}
		}

		#endregion


		#region Methods

		public override bool PositionOverShape(Vector3 position) {
			Vector3 dif = (this.Transform.position + (Vector3)this.Offset) - position;
			dif.z = 0;
			return (dif.magnitude < this.Radius);
		}


		#if UNITY_EDITOR
		void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Transform.position + (Vector3)this.Offset, this.Radius);
		}
		#endif

		#endregion

	
	}

}