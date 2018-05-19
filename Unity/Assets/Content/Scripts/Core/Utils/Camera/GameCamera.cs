namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public class GameCamera : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private Camera m_Camera;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public Camera Camera {
			get { return m_Camera = m_Camera ?? this.GetComponent<Camera>(); }
		}

		#endregion


		#region Methods

		void Start() {
			Application.targetFrameRate = 60;
		}

		#endregion

	
	}

}