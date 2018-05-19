namespace Juice.Utils {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Drives a BezierBend shader from Transforms representing control
	/// point positions.
	/// </summary>
	[ExecuteInEditMode]
	public class BezierBendable : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		protected Transform[] m_ControlPoints;

		[SerializeField]
		protected bool m_SetHeightFromBounds;

		[SerializeField]
		protected bool m_SetOffsetFromBounds;

		#endregion


		#region Public Properties

		public bool AreControlPointsValid {
			get {
				
				if (this.ControlPoints == null || this.ControlPoints.Length < 4) {
					return false;
				}

				for (int i = 0; i < 4; ++i) {
					if (!this.ControlPoints[i]) {
						return false;
					}
				}

				return true;
			}
		}

		public Transform[] ControlPoints {
			get {
				return m_ControlPoints;
			}
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// If you want to disable this component, you can
		/// call this method to do what is in Update().
		/// </summary>
		virtual public void ManualUpdate() {

			if (!this.AreControlPointsValid) {
				return;
			}

			Vector3[] controlPoints = this.ControlPointLocalPositions;

			for (int i = 0; i < 4; ++i) {
				this.MaterialPropertyBlock.SetVector(this.ControlPointPropertyIds[i], controlPoints[i]);
			}

			this.Renderer.SetPropertyBlock(this.MaterialPropertyBlock);

		}

		/// <summary>
		/// Updates the values based on the base mesh bounds.
		/// This happens automatically when the object is enabled,
		/// but if you alter the mesh, you can call this to
		/// update the values afterward.
		/// </summary>
		virtual public void UpdateMeshBoundsValues() {
			if (m_SetHeightFromBounds || m_SetOffsetFromBounds) {
				MeshFilter mf = this.Renderer.GetComponent<MeshFilter>();
				if (mf && mf.sharedMesh) {
					Bounds b = mf.sharedMesh.bounds;
					if (m_SetHeightFromBounds) {
						this.MaterialPropertyBlock.SetFloat(this.MeshHeightPropertyId, b.size.y);
					}
					if (m_SetOffsetFromBounds) {
						float offset = this.transform.position.y - this.transform.TransformPoint(b.min).y;
						this.MaterialPropertyBlock.SetFloat(this.MeshOffsetPropertyId, offset);
					}
				}
			}
		}

		#endregion


		#region MonoBehaviour

		virtual public void Reset() {

			m_SetHeightFromBounds = true;
			m_SetOffsetFromBounds = true;

			if (m_ControlPoints == null) {
				m_ControlPoints = new Transform[4];
			}
			if (m_ControlPoints.Length != 4) {
				System.Array.Resize(ref m_ControlPoints, 4);
			}
			for (int i = 0; i < 4; ++i) {
				if (!m_ControlPoints[i]) {
					string cpName = string.Format("Control Point {0}", i);
					Transform t = this.transform.Find(cpName);
					if (!t) {
						GameObject go = new GameObject(cpName);
						t = go.transform;
						go.transform.parent = this.transform;
					}
					t.localPosition = new Vector3(0f, i / 3f, 0f);
					m_ControlPoints[i] = t;
				}
			}

			RepositionControlPointsAtBounds();
		}

		virtual protected void OnEnable() {
			if (!this.Renderer || !this.Material || 
				!this.Material.HasProperty("MeshHeight") || !this.Material.HasProperty("MeshOffset") ||
				!this.Material.HasProperty("P0") || !this.Material.HasProperty("P1") || 
				!this.Material.HasProperty("P2") || !this.Material.HasProperty("P3") ) {
				Debug.LogWarning("BezierBendable not set up correctly; needs renderer and material with 'MeshHeight', 'P0'-'P3' properties, and 4 control point transforms");
				this.enabled = false;
			} else if (this.AreControlPointsValid) {
				UpdateMeshBoundsValues();
			}
		}

		virtual protected void OnDisable() {
			if (this.MaterialPropertyBlock != null) {
				this.MaterialPropertyBlock.Clear();
			}
		}

		virtual protected void Update() {
			ManualUpdate();
		}

		virtual protected void OnDrawGizmosSelected() {

			if (!this.AreControlPointsValid) {
				return;
			}

			Vector3[] controlPoints = this.ControlPointLocalPositions;

			Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			Gizmos.matrix = this.transform.localToWorldMatrix;

			const float step = 1.0f / 20f;
			float t = 0f;
			Vector3 pos = Bezier(controlPoints, t);
			while (t < 1.0f) {
				float nextT = Mathf.Clamp01(t + step);
				Vector3 nextPos = Bezier(controlPoints, nextT);

				Gizmos.DrawLine(pos, nextPos);

				pos = nextPos;
				t = nextT;
			}

		}

		#endregion


		#region Internal Fields

		protected int[] m_ControlPointPropertyIds;
		protected Vector3[] m_ControlPointLocalPositions;
		protected Material m_Material;
		protected MaterialPropertyBlock m_MaterialPropertyBlock;
		protected int m_MeshHeightPropertyId;
		protected int m_MeshOffsetPropertyId;
		protected Renderer m_Renderer;

		#endregion


		#region Internal Properties

		protected Vector3[] ControlPointLocalPositions {
			get {
				m_ControlPointLocalPositions = m_ControlPointLocalPositions ?? new Vector3[4];
				if (this.AreControlPointsValid) {
					for (int i = 0; i < 4; ++i) {
						m_ControlPointLocalPositions[i] = this.transform.InverseTransformPoint(this.ControlPoints[i].position);
					}
				}
				return m_ControlPointLocalPositions;
			}
		}

		protected int[] ControlPointPropertyIds {
			get {
				if (m_ControlPointPropertyIds == null) {
					m_ControlPointPropertyIds = new int[4];
					for (int i = 0; i < 4; ++i) {
						m_ControlPointPropertyIds[i] = Shader.PropertyToID(string.Format("P{0}", i));
					}
				}
				return m_ControlPointPropertyIds;
			}
		}

		protected Material Material {
			get {
				if (m_Material == null && this.Renderer) {
					m_Material = this.Renderer.sharedMaterial;
				}
				return m_Material;
			}
		}

		protected MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if (m_MaterialPropertyBlock == null && this.Renderer) {
					m_MaterialPropertyBlock = new MaterialPropertyBlock();
					this.Renderer.GetPropertyBlock(m_MaterialPropertyBlock);
				}
				return m_MaterialPropertyBlock;
			}
		}

		protected int MeshHeightPropertyId {
			get {
				if (m_MeshHeightPropertyId == 0) {
					m_MeshHeightPropertyId = Shader.PropertyToID("MeshHeight");
				}
				return m_MeshHeightPropertyId;
			}
		}

		protected int MeshOffsetPropertyId {
			get {
				if (m_MeshOffsetPropertyId == 0) {
					m_MeshOffsetPropertyId = Shader.PropertyToID("MeshOffset");
				}
				return m_MeshOffsetPropertyId;
			}
		}

		protected Renderer Renderer {
			get {
				if (!m_Renderer) {
					m_Renderer = GetComponentInChildren<Renderer>();
				}
				return m_Renderer;
			}
		}

		#endregion


		#region Internal Methods

		private static Vector3 Bezier(Vector3[] controlPoints, float t) {
			return SagoUtils.MathUtil.Bezier(
				controlPoints[0],
				controlPoints[1],
				controlPoints[2],
				controlPoints[3],
				t
			);
		}

		[ContextMenu("Space 1 and 2 between 0 and 3")]
		private void MenuSpaceEvenly() {
			if (this.AreControlPointsValid) {
				this.ControlPoints[1].position = (2f * this.ControlPoints[0].position + this.ControlPoints[3].position) / 3f;
				this.ControlPoints[2].position = (this.ControlPoints[0].position + 2f * this.ControlPoints[3].position) / 3f;
			}
		}

		[ContextMenu("Set Control Points From Bounds")]
		private void RepositionControlPointsAtBounds() {
			if (this.AreControlPointsValid) {
				Bounds b = this.Renderer.bounds;
				Transform[] cp = this.ControlPoints;
				float x = b.center.x;
				float z = b.center.z;
				cp[0].position = new Vector3(x, b.min.y, z);
				cp[1].position = new Vector3(x, b.min.y + b.size.y * 0.333f, z);
				cp[2].position = new Vector3(x, b.min.y + b.size.y * 0.667f, z);
				cp[3].position = new Vector3(x, b.max.y, z);

				UpdateMeshBoundsValues();
				ManualUpdate();
			}
		}

		#endregion


	}

}
