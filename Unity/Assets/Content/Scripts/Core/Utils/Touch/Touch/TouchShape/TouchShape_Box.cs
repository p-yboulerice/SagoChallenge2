namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public class TouchShape_Box : TouchShape {

		#region Fields

		[SerializeField]
		private Vector2 Size = new Vector3(1, 1, 0);

		[SerializeField]
		private Vector2 Offset;

		//private Vector3 CurrentPos;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override bool PositionOverShape(Vector3 position) {
			//this.CurrentPos = position;

			float Xmin = this.Transform.position.x - this.Size.x / 2 + this.Offset.x;
			float Xmax = this.Transform.position.x + this.Size.x / 2 + this.Offset.x;

			float Ymin = this.Transform.position.y - this.Size.y / 2 + this.Offset.y;
			float Ymax = this.Transform.position.y + this.Size.y / 2 + this.Offset.y;

			return position.x >= Xmin & position.x <= Xmax & position.y >= Ymin & position.y <= Ymax;
		}


		#if UNITY_EDITOR
		void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(this.Transform.position + (Vector3)this.Offset, this.Size);
			//Gizmos.DrawSphere(this.CurrentPos, 0.143f);
		}
		#endif

		#endregion

	
	}

}