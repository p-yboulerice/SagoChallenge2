namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using SagoMesh;
	using SagoLocalization;

	public class LocalizedMeshAnimationAutoLoad : MonoBehaviour {


		#region Fields

		[SerializeField]
		private LocalizedMeshAnimation LocalizedMeshAnimation;

		[System.NonSerialized]
		private MeshAnimationSource m_MeshAnimationSource;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private MeshAnimationSource MeshAnimationSource {
			get { return m_MeshAnimationSource = m_MeshAnimationSource ?? this.GetComponent<MeshAnimationSource>(); }
		}

		#endregion


		#region Methods

		void Start() {
			this.StartCoroutine(this.Coroutine_LoadLocalizedMeshAnimation());
		}

		private IEnumerator Coroutine_LoadLocalizedMeshAnimation(){
			LocalizedResourceReferenceLoaderRequest<MeshAnimation> r =  this.LocalizedMeshAnimation.LoadAsync();

			while (!r.isDone) {
				yield return null;
			}

			this.MeshAnimationSource.Animation = r.asset;

			yield return null;
		}

		#endregion


	}
}