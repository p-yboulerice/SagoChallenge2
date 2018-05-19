namespace Juice.Utils {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Drives a BoxBend shader from 8 Transforms representing control
	/// point positions.
	/// </summary>
	[ExecuteInEditMode]
	public class BoxBendable : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		protected Transform[] m_ControlPoints;

		[SerializeField]
		protected bool m_SetSizeFromBounds;

		[SerializeField]
		protected bool m_SetOffsetFromBounds;

		#endregion


		#region Public Properties

		public bool AreControlPointsValid {
			get {

				if (this.ControlPoints == null || this.ControlPoints.Length < 8) {
					return false;
				}

				for (int i = 0; i < 8; ++i) {
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
				int idx0 = 2 * i;
				int idx1 = (idx0 + 1) % 8;
				Vector4 v = new Vector4(
					controlPoints[idx0].x,
					controlPoints[idx0].y,
					controlPoints[idx1].x,
					controlPoints[idx1].y);
				this.MaterialPropertyBlock.SetVector(this.ControlPointPropertyIds[i], v);
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
			if (m_SetSizeFromBounds || m_SetOffsetFromBounds) {
				MeshFilter mf = this.Renderer.GetComponent<MeshFilter>();
				if (mf && mf.sharedMesh) {
					Bounds b = mf.sharedMesh.bounds;
					Vector4 v = this.MaterialPropertyBlock.GetVector(this.MeshRectPropertyId);
					if (m_SetSizeFromBounds) {
						v.z = b.size.x;
						v.w = b.size.y;
					}
					if (m_SetOffsetFromBounds) {
						Vector2 offset = this.transform.position - this.transform.TransformPoint(b.center);
						v.x = offset.x;
						v.y = offset.y;
					}
					this.MaterialPropertyBlock.SetVector(this.MeshRectPropertyId, v);
				}
			}
		}

		#endregion


		#region MonoBehaviour

		virtual public void Reset() {

			m_SetSizeFromBounds = true;
			m_SetOffsetFromBounds = true;

			if (m_ControlPoints == null) {
				m_ControlPoints = new Transform[8];
			}
			if (m_ControlPoints.Length != 8) {
				System.Array.Resize(ref m_ControlPoints, 8);
			}

			for (int i = 0; i < 8; ++i) {
				if (!m_ControlPoints[i]) {
					string cpName = string.Format("Control Point {0}", i);
					Transform t = this.transform.Find(cpName);
					if (!t) {
						GameObject go = new GameObject(cpName);
						t = go.transform;
						go.transform.parent = this.transform;
					}
					m_ControlPoints[i] = t;
				}
			}

			RepositionControlPointsAtBounds();
		}

		virtual protected void OnEnable() {
			m_Material = null;
			m_Renderer = null;

			if (!this.Renderer || !this.Material || 
				!this.Material.HasProperty("MeshRect") ||
				!this.Material.HasProperty("P01") || !this.Material.HasProperty("P23") || 
				!this.Material.HasProperty("P45") || !this.Material.HasProperty("P67") ) {
				Debug.LogWarning("BoxBendable not set up correctly; needs renderer and material with 'MeshRect', 'P01', 'P23', 'P45', 'P67' properties, and 8 control point transforms");
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

		virtual protected void OnDrawGizmos() {
			OnDrawGizmosSelected();
		}

		virtual protected void OnDrawGizmosSelected() {

			if (!this.AreControlPointsValid) {
				return;
			}

			Vector3[] controlPoints = this.ControlPointLocalPositions;

			Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			Gizmos.matrix = this.transform.localToWorldMatrix;

			const float step = 1.0f / 20f;

			for (int i = 0; i < 4; ++i) {
				float t = 0f;
				int cpIdx = 2 * i;
				Vector3 pos = Bezier(t, controlPoints[cpIdx], controlPoints[cpIdx + 1], controlPoints[(cpIdx + 2) % 8]);
				while (t < 1.0f) {
					float nextT = Mathf.Clamp01(t + step);
					Vector3 nextPos = Bezier(nextT, controlPoints[cpIdx], controlPoints[cpIdx + 1], controlPoints[(cpIdx + 2) % 8]);

					Gizmos.DrawLine(pos, nextPos);

					pos = nextPos;
					t = nextT;
				}
			}
		}

		#endregion


		#region Internal Fields

		protected int[] m_ControlPointPropertyIds;
		protected Vector3[] m_ControlPointLocalPositions;
		protected Material m_Material;
		protected MaterialPropertyBlock m_MaterialPropertyBlock;
		protected int m_MeshRectPropertyId;
		protected Renderer m_Renderer;

		#endregion


		#region Internal Properties

		protected Vector3[] ControlPointLocalPositions {
			get {
				m_ControlPointLocalPositions = m_ControlPointLocalPositions ?? new Vector3[8];
				if (this.AreControlPointsValid) {
					for (int i = 0; i < 8; ++i) {
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
						m_ControlPointPropertyIds[i] = Shader.PropertyToID(string.Format("P{0}{1}", 2 * i, 2 * i + 1));
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

		protected int MeshRectPropertyId {
			get {
				if (m_MeshRectPropertyId == 0) {
					m_MeshRectPropertyId = Shader.PropertyToID("MeshRect");
				}
				return m_MeshRectPropertyId;
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

		static public Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
			return SagoUtils.MathUtil.Bezier(p0, p1, p2, t);
		}

		[ContextMenu("Set Control Points From Bounds")]
		private void RepositionControlPointsAtBounds() {
			if (this.AreControlPointsValid) {
				Bounds b = this.Renderer.bounds;
				Transform[] cp = this.ControlPoints;
				float z = b.center.z;
				cp[0].position = new Vector3(b.min.x, b.max.y, z);
				cp[1].position = new Vector3(b.center.x, b.max.y, z);
				cp[2].position = new Vector3(b.max.x, b.max.y, z);
				cp[3].position = new Vector3(b.max.x, b.center.y, z);
				cp[4].position = new Vector3(b.max.x, b.min.y, z);
				cp[5].position = new Vector3(b.center.x, b.min.y, z);
				cp[6].position = new Vector3(b.min.x, b.min.y, z);
				cp[7].position = new Vector3(b.min.x, b.center.y, z);

				UpdateMeshBoundsValues();
				ManualUpdate();
			}
		}

		#endregion


	}

}
