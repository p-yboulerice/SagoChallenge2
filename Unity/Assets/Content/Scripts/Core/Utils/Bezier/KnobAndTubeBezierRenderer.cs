namespace Juice.Utils {
	
	using UnityEngine;
	using SagoUtils;
	
	/// <summary>
	/// Makes circles at the sample points and connects them with rectangles.
	/// Uses more verts than the standard <see cref="BezierRenderer"/>, but
	/// maintains thickness around sharp corners.
	/// Note:  Does not handle gradient colouring.
	/// </summary>
	public class KnobAndTubeBezierRenderer : BezierRenderer {
		
		
		[Header("Knob and Tube Specific")]
		
		[Tooltip("The number of verts around the circle of each knob")]
		[Range(4, 24)]
		[SerializeField]
		protected int m_KnobVertCount;
		
		
		/// <summary>
		/// The number of verts around the circle of each knob.
		/// </summary>
		/// <value>The knob vert count.</value>
		public int KnobVertCount {
			get {
				return m_KnobVertCount;
			}
		}
		
		
		public override void Reset() {
			base.Reset();
			m_KnobVertCount = 12;
		}
		
		/// <summary>
		/// The number of vertices used in the mesh.
		/// </summary>
		/// <value>The vertex count.</value>
		public override int VertexCount {
			get {
				int ends = (2 + this.KnobVertCount + 1) * 2;
				if (this.CurvePoints.Length > 2) {
					return ends + (4 + this.KnobVertCount + 1) * (this.CurvePoints.Length - 2);
				} else {
					return ends;
				}
			}
		}
		
		protected override int[] BuildTriangles() {
			
			int triangleCount = (this.CurvePoints.Length - 1) * 2 +
				(this.CurvePoints.Length * this.KnobVertCount);
			
			int[] triangles = new int[3 * triangleCount];
			int tIdx = 0;
			int vIdx = 0;
			for (int i = 0; i < this.CurvePoints.Length; ++i) {
				
				if (i < this.CurvePoints.Length - 1) {
					
					triangles[tIdx] = vIdx;
					triangles[tIdx+1] = vIdx + 1;
					triangles[tIdx+2] = vIdx + 2;
					
					triangles[tIdx+3] = triangles[tIdx+2];
					triangles[tIdx+4] = triangles[tIdx+1];
					triangles[tIdx+5] = vIdx + 3;
					
					tIdx += 6;
					vIdx += 4;
				}
				
				int knobCenterVIdx = vIdx;
				for (int k = 0; k < this.KnobVertCount - 1; ++k) {
					triangles[tIdx++] = knobCenterVIdx + k + 1;
					triangles[tIdx++] = knobCenterVIdx;
					triangles[tIdx++] = knobCenterVIdx + k + 2;
				}
				triangles[tIdx++] = knobCenterVIdx + this.KnobVertCount;
				triangles[tIdx++] = knobCenterVIdx;
				triangles[tIdx++] = knobCenterVIdx + 1;
				
				vIdx += this.KnobVertCount + 1;
			}
			
			return triangles;
		}
		
		protected override void UpdateMeshColours() {
			if (this.MeshColours != null) {
				
				Color32 color;
				
				switch (this.Colour) {
					
				case ColourMode.None:
					break;
					
				case ColourMode.Constant:
					color = this.ConstantColour;
					for (int i = 0; i < this.MeshColours.Length; ++i) {
						this.MeshColours[i] = color;
					}
					break;
					
				case ColourMode.Gradient:
					// No gradient
					color = this.ColourGradient.Evaluate(0f);
					for (int i = 0; i < this.MeshColours.Length; ++i) {
						this.MeshColours[i] = color;
					}
					break;
					
				}
			}
		}
		
		protected override void UpdateMeshVertices() {
			
			if (this.MeshVertices != null) {
				float deltaT = 1.0f / (float)(this.CurvePoints.Length - 1);
				
				int vIdx = 0;

				float size;
				
				switch (this.Thickness) {
				case ThicknessMode.Constant:
					size = this.ConstantThickness * 0.5f;
					break;
				case ThicknessMode.Linear:
					size = this.LinearThickness[0] * 0.5f;
					break;
				case ThicknessMode.Curve:
					size = this.ThicknessCurve.Evaluate(0f);
					break;
				default:
					size = this.ConstantThickness * 0.5f;
					break;
				}
				
				float knobSideAngle = (Mathf.PI * 2.0f) / (float)this.KnobVertCount;
				
				float nextSize = size;
				Vector3 normal = Vector3.up;
				
				for (int i = 0; i < this.CurvePoints.Length; ++i) {
					
					switch (this.Thickness) {
					case ThicknessMode.Linear:
						nextSize = Mathf.Lerp(this.LinearThickness[0], this.LinearThickness[1], (float)(i + 1) * deltaT) * 0.5f;
						break;
					case ThicknessMode.Curve:
						float t = (float)(i + 1) * deltaT;
						nextSize = this.ThicknessCurve.Evaluate(t);
						break;
					}
					
					Vector3 point = this.CurvePoints[i];
					
					if (i < this.CurvePoints.Length - 1) {
						Vector3 nextPoint = this.CurvePoints[i + 1];
						Vector3 dir = (nextPoint - point).normalized;
						normal = Vector3.Cross(Vector3.back, dir);
						
						// tube from here to next point
						this.MeshVertices[vIdx++] = point + size * normal;
						this.MeshVertices[vIdx++] = point - size * normal;
						this.MeshVertices[vIdx++] = nextPoint + nextSize * normal;
						this.MeshVertices[vIdx++] = nextPoint - nextSize * normal;
					}
					
					this.MeshVertices[vIdx++] = point;  // center of knob
					
					float offsetAngle = Mathf.Atan2(normal.y, normal.x);
					for (int k = 0; k < this.KnobVertCount; ++k) {
						float theta = k * knobSideAngle + offsetAngle;
						
						this.MeshVertices[vIdx++] = point + new Vector3(size * Mathf.Cos(theta), size * Mathf.Sin(theta), 0.0f);
					}
					
					size = nextSize;
					
				}
			}
			
		}
		
	}
	
}
