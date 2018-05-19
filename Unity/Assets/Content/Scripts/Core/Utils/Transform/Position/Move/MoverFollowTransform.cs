namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class MoverFollowTransform : MonoBehaviour {


		#region Fields

		[SerializeField]
		private Transform m_Target;

		[System.NonSerialized]
		private Mover m_MoverVelocity;

		#endregion


		#region Properties

		public Mover MoverVelocity {
			get { return m_MoverVelocity = m_MoverVelocity ?? GetComponent<Mover>(); }
		}

		public Transform Target {
			get { return m_Target; }
			set {
				m_Target = value;
				this.MoverVelocity.GoToPosition = this.m_Target.position;
			}
		}

		#endregion


		#region Methods

		void Update() {
			if (this.Target != null) {
				this.MoverVelocity.GoToPosition = this.Target.position;
			}

		}

		#endregion


	}
}