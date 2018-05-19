namespace Juice.FSM{

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_Bounce : StateAction {

		#region Fields

		[SerializeField]
		private float m_BounceForce = 0.1f;

		[SerializeField]
		private BounceScale m_Bounce;

		#endregion


		#region Properties

		private BounceScale Bounce {
			get { return m_Bounce;}
		}

		private float BounceForce {
			get { return m_BounceForce; }
		}

		#endregion


		#region Methods
		public override void Run() {
			this.Bounce.Bounce(this.BounceForce);
		}
		#endregion

	
	}

}