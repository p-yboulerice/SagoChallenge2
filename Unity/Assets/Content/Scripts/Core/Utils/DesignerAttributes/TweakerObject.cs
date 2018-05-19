namespace Juice.Utils {
	
	using UnityEngine;
	using System.Collections;

	public class TweakerObject : MonoBehaviour {


		#region Fields

		public string Group;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


	}
}