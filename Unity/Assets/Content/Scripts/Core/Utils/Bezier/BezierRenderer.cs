namespace Juice.Utils {
	
	using System.Collections;
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using SagoUtils;
	
	/// <summary>
	/// Renders a cubic Bezier defined by 4 control Transforms.  Can
	/// specify number of sample points, variable width, colour
	/// gradients.  Updates automatically or manually (UpdateMode).
	/// </summary>
	public class BezierRenderer : MonoBehaviour {
		
		
		#region Types
		
		[System.Serializable]
		public enum UpdateMode {
			Manual,
			Once,
			Continuous
		}
		
		[System.Serializable]
		public enum ThicknessMode {
			Constant,
			Linear,
			Curve
		}
		
		[System.Serializable]
		public enum ColourMode {
			None,
			Constant,
			Gradient
		}
		
		/// A convenience for stepping through a curve's points
		public class PathEnumerator : IEnumerator {
			
			#region Properties
			
			public int Count {
				get;
				protected set;
			}
			
			public int Index {
				get;
				protected set;
			}
			
			public BezierRenderer Path {
				get;
				protected set;
			}
			
			public Vector2 Range {
				get;
				protected set;
			}
			
			#endregion
			
			#region Constructors
			
			public PathEnumerator(BezierRenderer bezier, int sampleCount) :
				this(bezier, sampleCount, new Vector2(0.0f, 1.0f)) {
			}
			
			public PathEnumerator(BezierRenderer bezier, int sampleCount, Vector2 range) {
				
				this.Range = new Vector2(Mathf.Clamp01(range[0]), Mathf.Clamp01(range[1]));
				if (this.Range[1] < this.Range[0]) {
					this.Range = new Vector2(this.Range[1], this.Range[0]);
				}
				this.Count = sampleCount;
				this.Index = -1;
				this.Path = bezier;
			}
			
			#endregion
			
			#region IEnumerator Implementation
			
			virtual public object Current {
				get {
					float index = (float)(this.Index);
					float count = (float)(this.Count - 1);
					float t = Mathf.Lerp(this.Range[0], this.Range[1], Mathf.Clamp01(index / count));
					return this.Path.PointAt(t);
				}
			}
			
			public bool MoveNext() {
				this.Index++;
				if (this.Index < this.Count) {
					return true;
				}
				return false;
			}
			
			public void Reset() {
				this.Index = -1;
			}
			
			#endregion
			
		}
		
		#endregion
		
		
		#region Serialized Fields
		
		[Header("Update")]
		
		[SerializeField]
		protected UpdateMode m_UpdateMode;
		
		
		[Header("Sampling")]
		
		[SerializeField]
		protected int m_SampleCount;

		[SerializeField]
		protected bool m_FlattenZ;
		
		[Header("Control Points")]
		
		[SerializeField]
		protected Transform m_StartPoint;
		
		[SerializeField]
		protected Transform m_StartControl;
		
		[SerializeField]
		protected Transform m_EndControl;
		
		[SerializeField]
		protected Transform m_EndPoint;
		
		
		[Header("Thickness")]
		
		[SerializeField]
		protected ThicknessMode m_ThicknessMode;
		
		[Disable(typeof(BezierRenderer), "DisableConstantThickness", 0, true)]
		[SerializeField]
		protected float m_ConstantThickness;
		
		[Disable(typeof(BezierRenderer), "DisableLinearThickness", 0, true)]
		[SerializeField]
		protected Vector2 m_LinearThickness;
		
		[Disable(typeof(BezierRenderer), "DisableCurveThickness", 0, true)]
		[SerializeField]
		protected AnimationCurve m_ThicknessCurve;
		
		
		[Header("Material")]
		
		[ContextMenuItem("Set Default Material", "MenuSetDefaultMaterial")]
		[SerializeField]
		protected Material m_SharedMaterial;
		
		[SerializeField]
		protected ColourMode m_ColourMode;
		
		[Disable(typeof(BezierRenderer), "DisableConstantColour", 0, true)]
		[SerializeField]
		protected Color m_ConstantColour;
		
		[Disable(typeof(BezierRenderer), "DisableGradientColour", 0, true)]
		[SerializeField]
		protected Gradient m_ColourGradient;
		
		[Disable(typeof(BezierRenderer), "DisableColourIsDynamic", 0, true)]
		[SerializeField]
		protected bool m_ColourIsDynamic;
		
		#endregion
		
		
		#region Public Properties
		
		/// <summary>
		/// Modifying this requires a call to RebuildMesh(), if changing
		/// to or from ColourMode.None.
		/// </summary>
		/// <value>The colour.</value>
		public ColourMode Colour {
			get { return m_ColourMode; }
			protected set { m_ColourMode = value; }
		}
		
		/// <summary>
		/// Only used if Colour is ColourMode.Gradient.
		/// </summary>
		/// <value>The colour gradient.</value>
		public Gradient ColourGradient {
			get { return m_ColourGradient; }
		}
		
		/// <summary>
		/// If set, colours are updated with each update, otherwise
		/// they are only updated when the mesh is rebuilt.
		/// </summary>
		/// <value><c>true</c> if colour is fixed; otherwise, <c>false</c>.</value>
		public bool ColourIsDynamic {
			get { return m_ColourIsDynamic; }
			set { m_ColourIsDynamic = value; }
		}
		
		/// <summary>
		/// Only used if Colour is ColourMode.Constant.
		/// </summary>
		/// <value>The constant colour.</value>
		public Color ConstantColour {
			get { return m_ConstantColour; }
		}
		
		/// <summary>
		/// Only used if Thickness is ThicknessMode.Constant.
		/// </summary>
		/// <value>The constant thickness.</value>
		public float ConstantThickness {
			get { return m_ConstantThickness; }
		}
		
		/// <summary>
		/// Cache of the calculated points along the curve.
		/// </summary>
		/// <value>The curve points.</value>
		public Vector3[] CurvePoints {
			get {
				m_CurvePoints = m_CurvePoints ?? new Vector3[this.SampleCount];
				return m_CurvePoints;
			}
		}
		
		/// <summary>
		/// Cache of the calculated tangents along the curve.
		/// </summary>
		/// <value>The curve tangents.</value>
		public Vector3[] CurveTangents {
			get {
				m_CurveTangents = m_CurveTangents ?? new Vector3[this.SampleCount];
				return m_CurveTangents;
			}
		}
		
		/// <summary>
		/// Gets or sets the end control point Transform.
		/// </summary>
		/// <value>The end control.</value>
		virtual public Transform EndControl {
			get { return m_EndControl; }
			set { m_EndControl = value; }
		}
		
		/// <summary>
		/// Gets or sets the end point Transform.
		/// </summary>
		/// <value>The end point.</value>
		virtual public Transform EndPoint {
			get { return m_EndPoint; }
			set { m_EndPoint = value; }
		}

		/// <summary>
		/// If set, will zero-out the local space z coordinate of all control points.
		/// </summary>
		virtual public bool FlattenZ {
			get { return m_FlattenZ; }
			set { m_FlattenZ = value; }
		}

		/// <summary>
		/// Only used if Thickness is ThicknessMode.Linear.
		/// </summary>
		/// <value>The linear thickness.</value>
		public Vector2 LinearThickness {
			get { return m_LinearThickness; }
		}
		
		/// <summary>
		/// The mesh renderer that was created for this.
		/// </summary>
		/// <value>The mesh renderer.</value>
		public MeshRenderer MeshRenderer {
			get { return m_MeshRenderer; }
			protected set { m_MeshRenderer = value; }
		}
		
		/// <summary>
		/// Modifying this requires a call to RebuildMesh().
		/// </summary>
		/// <value>The sample count.</value>
		public int SampleCount {
			get { return m_SampleCount;	}
			set { m_SampleCount = value; }
		}
		
		/// <summary>
		/// Modifying this requires a call to RebuildMesh().  Alternatively,
		/// get the current one via this.MeshRenderer.
		/// </summary>
		/// <value>The shared material.</value>
		public Material SharedMaterial {
			get { return m_SharedMaterial; }
			set { m_SharedMaterial = value; }
		}
		
		/// <summary>
		/// Gets or sets the start control point Transform.
		/// </summary>
		/// <value>The start control.</value>
		virtual public Transform StartControl {
			get { return m_StartControl; }
			set { m_StartControl = value; }
		}
		
		/// <summary>
		/// Gets or sets the start point Transform.
		/// </summary>
		/// <value>The start point.</value>
		virtual public Transform StartPoint {
			get { return m_StartPoint; }
			set { m_StartPoint = value; }
		}
		
		/// <summary>
		/// Changes how the thickness (width) of the mesh along the curve is determined.
		/// </summary>
		/// <value>The thickness.</value>
		public ThicknessMode Thickness {
			get { return m_ThicknessMode; }
			set { m_ThicknessMode = value; }
		}
		
		/// <summary>
		/// Only used if Thickness is ThicknessMode.Curve.
		/// </summary>
		/// <value>The thickness curve.</value>
		public AnimationCurve ThicknessCurve {
			get { return m_ThicknessCurve; }
		}
		
		/// <summary>
		/// This object's Transform (lazy loaded, cached).
		/// </summary>
		/// <value>The transform.</value>
		virtual public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		/// <summary>
		/// The number of vertices used in the mesh.
		/// </summary>
		/// <value>The vertex count.</value>
		virtual public int VertexCount {
			get {
				return 2 * this.CurvePoints.Length;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// The point on the Bezier curve at a normalized parameter, t.
		/// </summary>
		/// <returns>The <see cref="UnityEngine.Vector3"/>.</returns>
		/// <param name="t">T from [0, 1]</param>
		virtual public Vector3 PointAt(float t) {
			
			return MathUtil.Bezier(
				this.Transform.InverseTransformPoint(this.StartPoint.position),
				this.Transform.InverseTransformPoint(this.StartControl.position),
				this.Transform.InverseTransformPoint(this.EndControl.position),
				this.Transform.InverseTransformPoint(this.EndPoint.position),
				t);
			
		}
		
		/// <summary>
		/// Gets an enumerator of our Bezier curve with our sample count.
		/// </summary>
		/// <returns>The points enumerator.</returns>
		virtual public PathEnumerator GetPointsEnumerator() {
			return new PathEnumerator(this, this.SampleCount);
		}
		
		/// <summary>
		/// Gets an enumerator of our Bezier curve with the given sample count.
		/// </summary>
		/// <returns>The points enumerator.</returns>
		/// <param name="count">Count.</param>
		virtual public PathEnumerator GetPointsEnumerator(int count) {
			return new PathEnumerator(this, count);
		}
		
		/// <summary>
		/// Gets an enumerator of our Bezier curve with the given sample count
		/// between the given range (normalized, [0..1]).
		/// </summary>
		/// <returns>The points enumerator.</returns>
		/// <param name="count">Count.</param>
		/// <param name="range">Range.</param>
		virtual public PathEnumerator GetPointsEnumerator(int count, Vector2 range) {
			return new PathEnumerator(this, count, range);
		}
		
		/// <summary>
		/// Rebuilds our mesh.  
		/// Destroys existing mesh and mesh GameObject first.
		/// You only need to call this if you are using
		/// <see cref="UpdateMode.Manual"/>, in which case you
		/// must call it once before calling ManualUpdate.
		/// </summary>
		[ContextMenu("Rebuild Mesh")]
		virtual public void RebuildMesh() {
			
			// Clean up any existing
			if (this.Mesh) {
				if (Application.isPlaying) {
					Destroy(this.Mesh);
				} else {
					DestroyImmediate(this.Mesh);
				}
				this.Mesh = null;
			}
			if (this.MeshGameObject) {
				if (Application.isPlaying) {
					Destroy(this.MeshGameObject);
				} else {
					DestroyImmediate(this.MeshGameObject);
				}
				this.MeshGameObject = null;
			}
			this.MeshRenderer = null;
			
			m_CurvePoints = null;
			m_CurveTangents = null;
			this.MeshVertices = null;
			this.MeshColours = null;
			
			
			// Make new
			UpdateCurveData();
			
			// Create the mesh
			this.Mesh = new Mesh();
			this.Mesh.name = "Dynamic Curve Mesh";
			this.MeshVertices = new Vector3[this.VertexCount];
			if (this.m_ColourMode != ColourMode.None) {
				this.MeshColours = new Color32[this.MeshVertices.Length];
			}
			
			UpdateMeshVertices();
			UpdateMeshColours();
			
			// build triangles (once)
			int[] triangles = BuildTriangles();
			
			this.Mesh.Clear();
			this.Mesh.vertices = this.MeshVertices;
			this.Mesh.triangles = triangles;
			this.Mesh.colors32 = this.MeshColours;
			
			this.Mesh.RecalculateBounds();
			
			GameObject goMesh = new GameObject("Mesh");
			Transform goMeshT = goMesh.transform;
			goMeshT.parent = this.Transform;
			goMeshT.localPosition = Vector3.zero;
			goMeshT.localScale = new Vector3(1.0f, 1.0f, 1.001f);  // force non-uniform for batching
			MeshRenderer rend = goMesh.AddComponent<MeshRenderer>();
			rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			rend.receiveShadows = false;
			rend.sharedMaterial = this.SharedMaterial;
			MeshFilter mf = goMesh.AddComponent<MeshFilter>();
			mf.sharedMesh = this.Mesh;
			
			this.MeshRenderer = rend;
			this.MeshGameObject = goMesh;
		}
		
		/// <summary>
		/// The Update loop, which you can call on your own if the UpdateMode is
		/// set to Manual.  You must call RebuildMesh the first time.
		/// Internally this is called automatically if UpdateMode is Continous.
		/// </summary>
		virtual public void ManualUpdate() {
			
			if (!this.IsCurveInputValid) {
				return;
			}
			
			UpdateCurveData();
			UpdateMeshVertices();
			if (this.ColourIsDynamic) {
				UpdateMeshColours();
			}
			UpdateMesh();
			
		}
		
		#endregion
		
		
		#region MonoBehaviour
		
		virtual public void Reset() {
			MenuAddControlPoints();
			MenuSetDefaultMaterial();
			m_UpdateMode = UpdateMode.Continuous;
			m_SampleCount = 24;
			m_ConstantThickness = 0.25f;
			m_LinearThickness = new Vector2(0.1f, 0.2f);
			m_ThicknessCurve = AnimationCurve.Linear(0.0f, 0.1f, 1.0f, 0.2f);
			m_ColourMode = ColourMode.Constant;
			m_ConstantColour = Color.red;
		}
		
		virtual protected void Start() {
			
			switch (this.m_UpdateMode) {
			
			case UpdateMode.Manual:
				this.enabled = false;
				break;
			
			case UpdateMode.Once:
				RebuildMesh();
				ManualUpdate();
				this.enabled = false;
				break;
			
			case UpdateMode.Continuous:
				RebuildMesh();
				break;
			
			}
		}
		
		virtual protected void Update() {
			ManualUpdate();
		}
		
		virtual protected void OnDrawGizmos() {
			
			if (!this.IsCurveInputValid) {
				return;
			}
			
			const float radius = 0.1f;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.StartPoint.position, radius);
			Gizmos.color = new Color(0.0f, 0.5f, 0.0f, 1.0f);
			Gizmos.DrawWireSphere(this.StartControl.position, radius);
			Gizmos.color = new Color(0.5f, 0.0f, 0.0f, 1.0f);
			Gizmos.DrawWireSphere(this.EndControl.position, radius);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.EndPoint.position, radius);
			
			PathEnumerator points = this.GetPointsEnumerator();
			
			if (points.MoveNext()) {
				
				Gizmos.color = Color.yellow;
				Gizmos.matrix = this.Transform.localToWorldMatrix;
				
				Vector3 previous;
				Vector3 current = (Vector3)points.Current;
				
				while (points.MoveNext()) {
					
					previous = current;
					current = (Vector3)points.Current;
					
					Gizmos.DrawLine(previous, current);
				}
			}
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		protected Vector3[] m_CurvePoints;
		
		[System.NonSerialized]
		protected Vector3[] m_CurveTangents;
		
		[System.NonSerialized]
		protected MeshRenderer m_MeshRenderer;
		
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Internal Properties

		virtual protected bool IsCurveInputValid {
			get {
				return this.SampleCount > 1 && this.StartPoint && this.StartControl && this.EndControl && this.EndPoint;
			}
		}
		
		protected Mesh Mesh {
			get; set;
		}
		
		protected Color32[] MeshColours {
			get; set;
		}
		
		protected GameObject MeshGameObject {
			get; set;
		}
		
		protected Vector3[] MeshVertices {
			get; set;
		}
		
		#endregion
		
		
		#region Internal Methods
		
		#region Inspector Helpers
		
		private static bool DisableConstantThickness(object obj) {
			return (obj as BezierRenderer).Thickness != ThicknessMode.Constant;
		}
		private static bool DisableLinearThickness(object obj) {
			return (obj as BezierRenderer).Thickness != ThicknessMode.Linear;
		}
		private static bool DisableCurveThickness(object obj) {
			return (obj as BezierRenderer).Thickness != ThicknessMode.Curve;
		}
		private static bool DisableConstantColour(object obj) {
			return (obj as BezierRenderer).Colour != ColourMode.Constant;
		}
		private static bool DisableGradientColour(object obj) {
			return (obj as BezierRenderer).Colour != ColourMode.Gradient;
		}
		private static bool DisableColourIsDynamic(object obj) {
			return (obj as BezierRenderer).Colour == ColourMode.None;
		}
		
		#endregion
		
		/// <summary>
		/// Refresh CurvePoints and CurveTangents based on current position of control points.
		/// </summary>
		virtual protected void UpdateCurveData() {
			
			Vector3 p0 = this.Transform.InverseTransformPoint(this.StartPoint.position);
			Vector3 p1 = this.Transform.InverseTransformPoint(this.StartControl.position);
			Vector3 p2 = this.Transform.InverseTransformPoint(this.EndControl.position);
			Vector3 p3 = this.Transform.InverseTransformPoint(this.EndPoint.position);

			if (this.FlattenZ) {
				p0.z = p1.z = p2.z = p3.z = 0f;
			}

			float deltaT = 1.0f / (this.CurvePoints.Length - 1);
			float t = 0.0f;
			
			for (int i = 0; i < this.CurvePoints.Length; i++) {
				
				this.CurvePoints[i] = MathUtil.Bezier(p0, p1, p2, p3, t);
				
				this.CurveTangents[i] = MathUtil.BezierTangent(p0, p1, p2, p3, t).normalized;
				
				t += deltaT;
			}
		}
		
		/// <summary>
		/// Builds the triangles array that can be applied directly to the mesh.
		/// </summary>
		/// <returns>The triangles.</returns>
		virtual protected int[] BuildTriangles() {
			int[] triangles = new int[3 * 2 * (this.CurvePoints.Length - 1)];
			for (int i = 1; i < this.CurvePoints.Length; ++i) {
				int tIdx = 3 * 2 * (i-1);
				triangles[tIdx] = i * 2 - 2;
				triangles[tIdx+1] = i * 2 - 1;
				triangles[tIdx+2] = i * 2;
				
				triangles[tIdx+3] = triangles[tIdx+2];
				triangles[tIdx+4] = triangles[tIdx+1];
				triangles[tIdx+5] = i * 2 + 1;
			}
			return triangles;
		}
		
		/// <summary>
		/// Updates the internal MeshVertices array.  The mesh is not updated
		/// until UpdateMesh is called.
		/// </summary>
		virtual protected void UpdateMeshVertices() {
			if (this.MeshVertices != null) {
				float size = this.ConstantThickness * 0.5f;
				float deltaT = 1.0f / (float)(this.CurvePoints.Length - 1);
				
				for (int i = 0; i < this.CurvePoints.Length; ++i) {
					
					switch (this.Thickness) {
						case ThicknessMode.Linear:
							size = Mathf.Lerp(this.LinearThickness[0], this.LinearThickness[1], (float)i * deltaT) * 0.5f;
							break;
						case ThicknessMode.Curve:
							float t = (float)i * deltaT;
							size = this.ThicknessCurve.Evaluate(t);
							break;
					}
					
					Vector3 normal = Vector3.Cross(Vector3.back, this.CurveTangents[i]); // assuming pointing back at camera
					this.MeshVertices[i * 2] = this.CurvePoints[i] + size * normal;
					this.MeshVertices[i * 2 + 1] = this.CurvePoints[i] - size * normal;
				}
			}
		}
		
		/// <summary>
		/// Updates the internal MeshColours array.  The mesh is not updated
		/// until UpdateMesh is called.
		/// </summary>
		virtual protected void UpdateMeshColours() {
			if (this.MeshColours != null) {
				
				Color32 color;
				
				switch (this.Colour) {
				
				case ColourMode.None:
					break;
				
				case ColourMode.Constant:
					color = this.ConstantColour;
					for (int i = 0; i < this.CurvePoints.Length; ++i) {
						this.MeshColours[i * 2] = color;
						this.MeshColours[i * 2 + 1] = color;
					}
					break;
				
				case ColourMode.Gradient:
					float deltaT = 1.0f / (float)(this.CurvePoints.Length - 1);
					for (int i = 0; i < this.CurvePoints.Length; ++i) {
						float t = (float)i * deltaT;
						color = this.ColourGradient.Evaluate(t);
						
						this.MeshColours[i * 2] = color;
						this.MeshColours[i * 2 + 1] = color;
					}	
					break;
				
				}
			}
		}
		
		/// <summary>
		/// Updates the mesh with the current values of MeshVertices and MeshColours.
		/// </summary>
		virtual protected void UpdateMesh() {
			if (!this.Mesh) {
				return;
			}
			
			// update verts - triangles unchanged
			this.Mesh.vertices = this.MeshVertices;
			this.Mesh.colors32 = this.MeshColours;
			this.Mesh.RecalculateBounds();
			
		}
		
		[ContextMenu("Add Control Points")]
		protected void MenuAddControlPoints() {
			
			if (this.StartPoint == null) {
				this.StartPoint = AddControlPoint("Start", new Vector3(-2.0f, 0.0f, 0.0f));
			}
			if (this.StartControl == null) {
				this.StartControl = AddControlPoint("Start Control", new Vector3(-1.0f, 1.0f, 0.0f));
			}
			if (this.EndControl == null) {
				this.EndControl = AddControlPoint("End Control", new Vector3(1.0f, 1.0f, 0.0f));
			}
			if (this.EndPoint == null) {
				this.EndPoint = AddControlPoint("End", new Vector3(2.0f, 0.0f, 0.0f));
			}
			
		}
		
		#if UNITY_EDITOR
		[ContextMenu("Default Material")]
		protected void MenuSetDefaultMaterial() {
			
			if (!this.SharedMaterial) {
				
				const string path = "Assets/External/SagoMesh/Materials/OpaqueMesh.mat";
				
				Material mat;
				mat = AssetDatabase.LoadAssetAtPath<Material>(path);
				
				if (mat) {
					this.SharedMaterial = mat;
				} else {
					Debug.LogWarningFormat("Could not find default material at {0}", path);
				}
				
			}
		}
		#else
		protected void MenuSetDefaultMaterial() { }
		#endif
		
		private Transform AddControlPoint(string name, Vector3 offset) {
			GameObject go = new GameObject(name);
			Transform t = go.transform;
			t.parent = this.Transform;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			t.localPosition = offset;
			return t;
		}
		
		#endregion
		
		
	}
	
}
