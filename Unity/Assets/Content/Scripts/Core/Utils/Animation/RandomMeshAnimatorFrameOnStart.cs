namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using SagoMesh;

	public class RandomMeshAnimatorFrameOnStart : MonoBehaviour {


		#region Fields

		[System.NonSerialized]
		private MeshAnimator m_MeshAnimator;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private MeshAnimator MeshAnimator {
			get { return m_MeshAnimator = m_MeshAnimator ?? this.GetComponent<MeshAnimator>(); }
		}

		#endregion


		#region Methods

		void Start() {
			this.MeshAnimator.Stop(Random.Range(0, this.MeshAnimator.Animation.Frames.Length));
		}

		#endregion


	}
}