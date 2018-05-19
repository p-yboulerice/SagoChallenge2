namespace Juice.Utils {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class UpdateRenderTextureObjectAndResolution : MonoBehaviour {

		#region Fields

		[SerializeField]
		private Camera Camera;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		void Start() {
			this.Camera.targetTexture.width = Screen.width;
			this.Camera.targetTexture.height = Screen.height;

			Vector3 a = this.Camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
			Vector3 b = this.Camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

			Vector3 dif = b - a;
			//dif *= 0.5f;
			dif.z = 1; 

			this.Transform.localScale = dif;
		}

		#endregion

	}
}