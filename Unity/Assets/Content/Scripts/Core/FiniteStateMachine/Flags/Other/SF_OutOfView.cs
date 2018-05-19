namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public enum OutOfViewDirection : int {
		Horizontal,
		Vertical,
		Any,
		Down
	}

	public class SF_OutOfView : StateFlag {

		#region Fields

		[SerializeField]
		private Transform m_Target;

		[SerializeField]
		private OutOfViewDirection m_OutOfViewDirection;

		[SerializeField]
		private float m_Tolerance = 0;

		[System.NonSerialized]
		private GameCamera m_GameCamera;

		#endregion


		#region Properties

		private Transform Target {
			get { return m_Target; }
		}

		private Camera Camera {
			get { return this.GameCamera ? this.GameCamera.Camera : null; }
		}

		private GameCamera GameCamera {
			get { return m_GameCamera = m_GameCamera ?? FindObjectOfType<GameCamera>(); }
		}

		private float Tolerance {
			get { return m_Tolerance; }
			set { m_Tolerance = value; }
		}

		public override bool IsActive {
			get {
				if (this.Camera) {
					Vector3 pos = this.Camera.WorldToViewportPoint(this.Target.position);

					switch (this.OutOfViewDiretion) {
					case OutOfViewDirection.Horizontal:
						return pos.x < -this.Tolerance || pos.x > 1 + this.Tolerance;
					case OutOfViewDirection.Vertical:
						return pos.y < -this.Tolerance || pos.y > 1 + this.Tolerance;
					case OutOfViewDirection.Any:	
						return pos.x < -this.Tolerance || pos.x > 1 + this.Tolerance || pos.y < -this.Tolerance || pos.y > 1 + this.Tolerance;
					case OutOfViewDirection.Down:
						return pos.y < -this.Tolerance;
					}
		
				}
				return false;
			}
		}

		private OutOfViewDirection OutOfViewDiretion {
			get { return m_OutOfViewDirection; }
		}

		#endregion

	
	}

}