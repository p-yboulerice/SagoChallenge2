namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public abstract class TouchShape : MonoBehaviour {

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

		virtual public bool PositionOverShape(Vector3 position){
			return false;
		}
		#endregion

	
	}

}