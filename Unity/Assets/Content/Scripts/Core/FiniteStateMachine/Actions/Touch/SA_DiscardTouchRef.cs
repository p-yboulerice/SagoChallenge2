namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class SA_DiscardTouchRef : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods
		#endregion

	
	}

}