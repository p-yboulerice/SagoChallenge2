namespace Juice.Utils {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public enum MultiBoxBendableUpdateMode {
		Update,
		LateUpdate,
		Manual
	}

	/// <summary>
	/// Drives a BoxBendMulti shader from 8 Transforms representing control
	/// point positions.
	/// </summary>
	[ExecuteInEditMode]
	public class MultiBoxBendable : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		private MultiBoxBendableUpdateMode UpdateMode;

		[SerializeField]
		protected Transform[] m_ControlPoints;

		[SerializeField]
		protected bool m_ManuallySetControlBounds;

		[SerializeField]
		protected Vector2 m_ControlPointMin;

		[SerializeField]
		protected Vector2 m_ControlPointMax;

		[SerializeField]
		protected bool m_ManuallyAddRenderers;

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

		public bool ManuallyAddRenderers {
			get { return m_ManuallyAddRenderers; }
			set { m_ManuallyAddRenderers = value; }
		}

		public Vector2 BaseControlPointLocalMin {
			get { return m_ControlPointMin; }
		}

		public Vector2 BaseControlPointLocalMax {
			get { return m_ControlPointMax; }
		}

		public Vector4 ControlRect {
			get;
			private set;
		}

		public Vector3 [] ControlPointLocalPosition {
			get { return m_ControlPointLocalPositions; }
		}

		public Vector2 P0 {
			get { return this.ControlPointLocalPosition[0]; }
		}

		public Vector2 P1 {
			get { return this.ControlPointLocalPosition[1]; }
		}

		public Vector2 P2 {
			get { return this.ControlPointLocalPosition[2]; }
		}

		public Vector2 P3 {
			get { return this.ControlPointLocalPosition[3]; }
		}

		public Vector2 P4 {
			get { return this.ControlPointLocalPosition[4]; }
		}

		public Vector2 P5 {
			get { return this.ControlPointLocalPosition[5]; }
		}

		public Vector2 P6 {
			get { return this.ControlPointLocalPosition[6]; }
		}

		public Vector2 P7 {
			get { return this.ControlPointLocalPosition[7]; }
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// If you want to disable this component, you can
		/// call this method to do what is in Update().
		/// </summary>
		public void ManualUpdate() {

			if (!this.AreControlPointsValid) {
				return;
			}

			ValidateRenderersAndPropertyBlocks();

			Vector2 cpMinWorld = this.transform.TransformPoint(m_ControlPointMin);
			Vector2 cpMaxWorld = this.transform.TransformPoint(m_ControlPointMax);

			for (int j = 0; j < this.MaterialPropertyBlocks.Count; ++j) {
				Renderer rend = this.Renderers[j];

				for (int cp = 0; cp < 8; ++cp) {
					m_ControlPointLocalPositions[cp] = rend.transform.InverseTransformPoint(this.ControlPoints[cp].position);
				}

				for (int i = 0; i < 4; ++i) {
					int idx0 = 2 * i;
					int idx1 = (idx0 + 1) % 8;
					Vector4 v = new Vector4(
						            m_ControlPointLocalPositions[idx0].x,
						            m_ControlPointLocalPositions[idx0].y,
						            m_ControlPointLocalPositions[idx1].x,
						            m_ControlPointLocalPositions[idx1].y);
					this.MaterialPropertyBlocks[j].SetVector(this.ControlPointPropertyIds[i], v);
				}

				Vector2 min = rend.transform.InverseTransformPoint(cpMinWorld);
				Vector2 max = rend.transform.InverseTransformPoint(cpMaxWorld);
				Vector2 size = (max - min);
				this.ControlRect = new Vector4(min.x, min.y, size.x, size.y);
				this.MaterialPropertyBlocks[j].SetVector(this.ControlRectPropertyId, this.ControlRect);

				rend.SetPropertyBlock(this.MaterialPropertyBlocks[j]);
			}

		}

		public void AddRenderer(Renderer rend) {
			AddRenderer(rend, !m_ManuallySetControlBounds);
		}

		public void AddRenderers(Renderer[] renderers) {
			for (int i = 0; i < renderers.Length; ++i) {
				AddRenderer(renderers[i], false);
			}
			if (!m_ManuallySetControlBounds) {
				SetControlPointBaseFromBounds();
			}
		}

		[ContextMenu("Reset Renderers")]
		public void ResetRenderers() {
			ClearRenderers();
		}

		public void SetControlPointBaseFromBounds() {
			Bounds bounds = AllRenderersBounds();
			SetControlPointBaseFromBounds(bounds);
		}

		public void SetControlPointBaseFromBounds(Bounds bounds) {
			m_ControlPointMin = (Vector2)this.transform.InverseTransformPoint(bounds.min);
			m_ControlPointMax = (Vector2)this.transform.InverseTransformPoint(bounds.max);
		}

		[ContextMenu("Set Control Points From Bounds")]
		public void RepositionControlPointsAtBounds(bool validate = false) {

			if (validate) {
				this.ValidateRenderersAndPropertyBlocks();
			}

			if (this.AreControlPointsValid) {
				Bounds b = AllRenderersBounds();
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

				SetControlPointBaseFromBounds(b);

				ManualUpdate();
			}
		}

		#endregion


		#region MonoBehaviour

		public void Reset() {

			if (m_ControlPoints == null) {
				m_ControlPoints = new Transform[8];
			}
			if (m_ControlPoints.Length != 8) {
				System.Array.Resize(ref m_ControlPoints, 8);
			}

			string[] dirNames = { "NW", "N", "NE", "E", "SE", "S", "SW", "W" };

			for (int i = 0; i < 8; ++i) {
				if (!m_ControlPoints[i]) {
					string cpName = string.Format("Control Point {0} ({1})", i, dirNames[i]);
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

		//		protected void Awake() {
		//			m_ControlPointLocalPositions = new Vector3[8];
		//		}

		protected void OnDestroy() {
			ClearRenderers();
		}

		protected void Update() {
			if (this.UpdateMode == MultiBoxBendableUpdateMode.Update) {
				ManualUpdate();
			}
		}

		protected void LateUpdate() {
			if (this.UpdateMode == MultiBoxBendableUpdateMode.LateUpdate) {
				ManualUpdate();
			}
		}

		protected void OnDrawGizmos() {
			DrawGizmos(false);
		}

		protected void OnDrawGizmosSelected() {
			DrawGizmos(true);
		}

		#endregion


		#region Internal Fields

		protected int[] m_ControlPointPropertyIds;
		protected Vector3[] m_ControlPointLocalPositions = new Vector3[8];
		protected int m_ControlRectPropertyId;
		protected List<MaterialPropertyBlock> m_MaterialPropertyBlocks;
		protected List<Renderer> m_Renderers;

		#endregion


		#region Internal Properties

		protected Vector3[] ControlPointLocalPositions {
			get {
				if (m_ControlPointLocalPositions == null || m_ControlPointLocalPositions.Length < 8) {
					m_ControlPointLocalPositions = new Vector3[8];
				}
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
				if (m_ControlPointPropertyIds == null || m_ControlPointPropertyIds.Length < 4) {
					m_ControlPointPropertyIds = new int[4];
					for (int i = 0; i < 4; ++i) {
						m_ControlPointPropertyIds[i] = Shader.PropertyToID(string.Format("P{0}{1}", 2 * i, 2 * i + 1));
					}
				}
				return m_ControlPointPropertyIds;
			}
		}

		protected int ControlRectPropertyId {
			get {
				if (m_ControlRectPropertyId == 0) {
					m_ControlRectPropertyId = Shader.PropertyToID("ControlRect");
				}
				return m_ControlRectPropertyId;
			}
		}

		protected List<MaterialPropertyBlock> MaterialPropertyBlocks {
			get { 
				m_MaterialPropertyBlocks = m_MaterialPropertyBlocks ?? new List<MaterialPropertyBlock>();
				return m_MaterialPropertyBlocks;
			}
			set { m_MaterialPropertyBlocks = value; }
		}

		protected List<Renderer> Renderers {
			get {
				m_Renderers = m_Renderers ?? new List<Renderer>();
				if (m_Renderers.Count == 0 && !this.ManuallyAddRenderers) {
					AddRenderers(GetComponentsInChildren<Renderer>(true));
				}
				return m_Renderers;
			}
		}

		#endregion


		#region Other Methods

		static public Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
			return SagoUtils.MathUtil.Bezier(p0, p1, p2, t);
		}


		protected void AddRenderer(Renderer rend, bool updateBounds) {
			m_Renderers = m_Renderers ?? new List<Renderer>();
			m_MaterialPropertyBlocks = m_MaterialPropertyBlocks ?? new List<MaterialPropertyBlock>();

			if (rend && rend.sharedMaterial && rend.sharedMaterial.HasProperty(this.ControlRectPropertyId) &&
			    rend.sharedMaterial.HasProperty(this.ControlPointPropertyIds[0]) &&
			    rend.sharedMaterial.HasProperty(this.ControlPointPropertyIds[1]) &&
			    rend.sharedMaterial.HasProperty(this.ControlPointPropertyIds[2]) &&
			    rend.sharedMaterial.HasProperty(this.ControlPointPropertyIds[3])) {
				if (!m_Renderers.Contains(rend)) {
					m_Renderers.Add(rend);
					MaterialPropertyBlock block = new MaterialPropertyBlock();
					rend.GetPropertyBlock(block);
					this.MaterialPropertyBlocks.Add(block);

					if (updateBounds) {
						SetControlPointBaseFromBounds();
					}
				}
			} else {
				Debug.LogWarning("Invalid renderer; needs renderer and material with 'ControlRect', 'P01', 'P23', 'P45', 'P67' properties", rend);
			}
		}

		protected Bounds AllRenderersBounds() {
			if (m_Renderers == null || m_Renderers.Count == 0) {
				return new Bounds(this.transform.position, Vector3.zero);
			}
			Bounds bounds = m_Renderers[0].bounds;
			for (int i = 1; i < m_Renderers.Count; ++i) {
				bounds.Encapsulate(m_Renderers[i].bounds);
			}
			return bounds;
		}

		protected void ValidateRenderersAndPropertyBlocks() {

			for (int i = this.Renderers.Count - 1; i >= 0; --i) {
				if (!this.Renderers[i]) {
					this.Renderers.RemoveAt(i);
				}
			}

			if (this.Renderers.Count != this.MaterialPropertyBlocks.Count) {
				for (int i = 0; i < this.MaterialPropertyBlocks.Count; ++i) {
					this.MaterialPropertyBlocks[i].Clear();
				}
				this.MaterialPropertyBlocks.Clear();
				for (int i = 0; i < this.Renderers.Count; ++i) {
					MaterialPropertyBlock block = new MaterialPropertyBlock();
					this.Renderers[i].GetPropertyBlock(block);
					this.MaterialPropertyBlocks.Add(block);
				}
			}
		}

		protected void ClearRenderers() {
			if (this.MaterialPropertyBlocks != null) {
				for (int i = 0; i < this.MaterialPropertyBlocks.Count; ++i) {
					this.MaterialPropertyBlocks[i].Clear();
				}
				this.MaterialPropertyBlocks.Clear();
				this.MaterialPropertyBlocks = null;
			}
			if (this.Renderers != null) {
				this.Renderers.Clear();
				m_Renderers = null;
			}
		}

		protected void DrawGizmos(bool selected) {

			if (!this.AreControlPointsValid) {
				return;
			}

			ValidateRenderersAndPropertyBlocks();

			Gizmos.matrix = this.transform.localToWorldMatrix;

			{	// undistorted control point bounds
				Gizmos.color = new Color(0.5f, 0.5f, 0.1f, 0.5f);
				Vector2 pos = (m_ControlPointMin + m_ControlPointMax) * 0.5f;
				Vector2 size = (m_ControlPointMax - m_ControlPointMin);
				Gizmos.DrawWireCube(pos, size);
			}

			Vector3[] controlPoints = this.ControlPointLocalPositions;

			Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 1f);

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

			if (selected) {
				Gizmos.color = Color.white;
				const float radius = 0.1f;
				for (int i = 0; i < 8; ++i) {
					Gizmos.DrawWireSphere(controlPoints[i], radius);
				}
			}
		}

		#endregion


	}

}