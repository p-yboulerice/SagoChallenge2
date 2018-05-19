namespace Juice.FSM {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Destroy a GameObject
	/// </summary>
	public class SA_DestroyGameObject : StateAction {

		[SerializeField]
		protected GameObject m_Target;

		public override void Run() {
			base.Run();
			if (m_Target) {
				Destroy(m_Target);
			}
		}
		
	}
	
}
