namespace SagoUtils {

	using UnityEngine;

	public class DepthGroupElement : MonoBehaviour, IDepthGroupElement {


		#region Properties

		public DepthGroup DepthGroup {
			get;
			set;
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		#endregion


		#region Fields

		private Transform m_Transform;

		#endregion


	}

}
