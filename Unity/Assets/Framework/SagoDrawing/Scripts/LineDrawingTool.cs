namespace SagoDrawing {

	using System.Collections.Generic;
	using UnityEngine;
	using Touch = SagoTouch.Touch;
	
		
	/// <summary>
	/// The LineDrawingTool class implements functionality for drawing smooth lines to a <see cref="DrawingSurface" />.
	/// </summary>
	public class LineDrawingTool : MonoBehaviour, IDrawingTool {
		
		
		#region Fields
		
		[SerializeField]
		private Color m_Color;
		
		[System.NonSerialized]
		private LineGeometry m_Geometry;
		
		[System.NonSerialized]
		private Dictionary<int,Line> m_Lines;
		
		[SerializeField]
		private Material m_Material;
		
		[System.NonSerialized]
		private Mesh m_Mesh;
		
		[SerializeField]
		private LineOptions m_Options;
		
		#endregion
		
		
		#region Properties
		
		public Color Color {
			get { return m_Color; }
			set { m_Color = value; }
		}
		
		private LineGeometry Geometry {
			get { return m_Geometry = m_Geometry ?? new LineGeometry(); }
		}
		
		private Dictionary<int,Line> Lines {
			get { return m_Lines = m_Lines ?? new Dictionary<int,Line>(); }
		}
		
		public Material Material {
			get { return m_Material; }
		}
		
		private Mesh Mesh {
			get {
				if (m_Mesh == null) {
					m_Mesh = new Mesh();
					m_Mesh.MarkDynamic();
				}
				return m_Mesh;
			}
		}
		
		#endregion
		
		
		#region IDrawingTool
		
		public void OnRender(DrawingSurface surface) {
			
			Debug.AssertFormat(surface != null, "Cannot render to surface: {0}", "surface is null");
			Debug.AssertFormat(surface.RenderTexture != null, "Cannot render to surface: {0}", "render texture is null");
			Debug.AssertFormat(Material != null, "Cannot render to surface: {0}", "material is null");
			
			Geometry.Reset();
			foreach (Line line in Lines.Values) {
				line.OnRender(m_Geometry, m_Options);
			}
			Geometry.CopyToMesh(Mesh);

			Matrix4x4 matrix = surface.Camera.transform.localToWorldMatrix;
			matrix.m23 = surface.transform.position.z;

			surface.Invoke(() => {
				surface.RenderTexture.MarkRestoreExpected();
				Material.SetPass(0);
				Graphics.DrawMeshNow(Mesh, matrix, 0);
			});
			
		}
		
		public bool OnTouchBegan(DrawingSurface surface, Touch touch) {
			Line line = FindOrCreateLine(touch.Identifier);
			line.OnTouchBegan(surface, m_Color, touch, m_Options);
			return true;
		}
		
		public void OnTouchMoved(DrawingSurface surface, Touch touch) {
			Line line = FindLine(touch.Identifier);
			line.OnTouchMoved(surface, m_Color, touch, m_Options);
		}
		
		public void OnTouchEnded(DrawingSurface surface, Touch touch) {
			Line line = FindLine(touch.Identifier);
			line.OnTouchEnded(surface, m_Color, touch, m_Options);
		}
		
		public void OnTouchCancelled(DrawingSurface surface, Touch touch) {
			OnTouchEnded(surface, touch);
		}
		
		#endregion
		
		
		#region Lines
		
		private Line CreateLine(int key) {
			Debug.AssertFormat(!Lines.ContainsKey(key), "Could not create line: {0}", key);
			Line value = new Line();
			Lines.Add(key, value);
			return value;
		}
		
		private Line FindLine(int key) {
			return Lines.ContainsKey(key) ? Lines[key] : null;
		}
		
		private Line FindOrCreateLine(int key) {
			return FindLine(key) ?? CreateLine(key);
		}
		
		private void SetLine(int key, Line value) {
			Debug.AssertFormat(Lines.ContainsKey(key), "Could not set line: {0}", key);
			if (value == null) {
				Lines.Remove(key);
			} else {
				Lines[key] = value;
			}
		}
		
		private void RemoveLine(int key) {
			SetLine(key, null);
		}
		
		#endregion
		
		
		#region MonoBehaviour
		
		private void Reset() {
			m_Color = Color.black;
			m_Options.Reset();
		}
		
		#endregion
		
		
	}
	
}

namespace SagoDrawing {
	
	using SagoTouch;
	using UnityEngine;
	using Touch = SagoTouch.Touch;
	
	/// <summary>
	/// The Line class represents one line in the <see cref="LineDrawingTool" />.
	/// </summary>
	[System.Serializable]
	internal class Line {
		
		
		#region Fields
		
		/// <summary>
		/// The line cap (updated each time the line is renderered).
		/// </summary>
		[System.NonSerialized]
		private LineCap m_Cap;
		
		/// <summary>
		/// The flag that indicates whether or not to render the line cap.
		/// </summary>
		[System.NonSerialized]
		private bool m_CapRequired;
		
		/// <summary>
		/// The flag that indicates whether or not any points have been added to the line since the last time it was rendered.
		/// </summary>
		[System.NonSerialized]
		private bool m_IsDirty;
		
		/// <summary>
		/// The array of points that have been added since the last time the line was rendered.
		/// </summary>
		[System.NonSerialized]
		private LinePoint[] m_Points;
		
		/// <summary>
		/// The number of elements used in the m_Points array.
		/// NOTE: m_PointCount isn't the same as m_Points.Length (the points array is a fixed length).
		/// </summary>
		[System.NonSerialized]
		private int m_PointCount;
		
		/// <summary>
		/// The line segment (updated each time the line is rendered).
		/// </summary>
		[System.NonSerialized]
		private LineSegment m_Segment;
		
		/// <summary>
		/// The array of points with bezier smoothing applied.
		/// </summary>
		[System.NonSerialized]
		private LinePoint[] m_SmoothPoints;
		
		/// <summary>
		/// The number of elements used in the m_SmoothPoints array.
		/// </summary>
		[System.NonSerialized]
		private int m_SmoothPointCount;
		
		/// <summary>
		/// The line velocity (updated each time the touch begins, moves or ends).
		/// </summary>
		[System.NonSerialized]
		private LineVelocity m_Velocity;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Transforms the array of points into verts, colors and triangles and adds them to 
		/// the geometry object (called by the <see cref="LineDrawingTool" /> each time it 
		/// renders to the <see cref="DrawingSurface" />.
		/// </summary>
		public void OnRender(LineGeometry geometry, LineOptions options) {
			if (m_IsDirty && m_PointCount > 0) {
				
				// apply bezier smoothing
				GenerateSmoothPoints();
				int count = (m_SmoothPointCount > 0 ? m_SmoothPointCount : m_PointCount);
				LinePoint[] points = (m_SmoothPointCount > 0 ? m_SmoothPoints : m_Points);
				
				// add cap
				if (m_CapRequired) {
					m_Cap.Update(points[count - 1]);
					m_Cap.Apply(geometry);
				}
				
				// add points
				for (int index = 1; index < count; index++) {
					
					// add segement
					m_Segment.Update(points[index - 1], points[index - 0]);
					m_Segment.Apply(geometry);
					
					// add a cap when the direction changes
					if (Vector2.Dot(m_Segment.NormalA, m_Segment.NormalB) < 0) {
						m_Cap.Update(points[index - 1]);
						m_Cap.Apply(geometry);
					}
					
				}
				
				// consume points
				if (m_PointCount > 2) {
					m_Points[0] = m_Points[m_PointCount - 2];
					m_Points[1] = m_Points[m_PointCount - 1];
					m_PointCount = 2;
				}
				
				// clear flags
				m_CapRequired = false;
				m_IsDirty = false;
				
			}
		}
		
		/// <summary>
		/// Called by the <see cref="LineDrawingTool" /> when the touch associated with this line begins on the <see cref="DrawingSurface" />.
		/// </summary>
		public void OnTouchBegan(DrawingSurface surface, Color color, Touch touch, LineOptions options) {
			
			// get position
			Vector2 position = CameraUtils.TouchToWorldPoint(touch, surface.Transform, surface.Camera);
			position = surface.Transform.parent.InverseTransformPoint(position);
			
			// reset points
			m_Points = m_Points ?? new LinePoint[16];
			m_PointCount = 0;
			m_SmoothPoints = m_SmoothPoints ?? new LinePoint[1024];
			m_SmoothPointCount = 0;
			
			// reset
			m_Cap.Reset(options.Overdraw);
			m_Segment.Reset(options.Overdraw);
			m_Velocity.Reset(position, options.VelocityMin, options.VelocityMax, options.VelocitySmoothing);
			
			// add point
			LinePoint point = default(LinePoint);
			point.Color = color;
			point.Position = position;
			point.Weight = Mathf.Lerp(options.WeightMin, options.WeightMax, m_Velocity.Factor);
			AddPoint(point);
			
			// set flags
			m_IsDirty = true;
			m_CapRequired = true;
			
		}
		
		/// <summary>
		/// Called by the <see cref="LineDrawingTool" /> when the touch associated with this line moves on the <see cref="DrawingSurface" />.
		/// </summary>
		public void OnTouchMoved(DrawingSurface surface, Color color, Touch touch, LineOptions options) {
			
			// get position
			Vector2 position = CameraUtils.TouchToWorldPoint(touch, surface.Transform, surface.Camera);
			position = surface.Transform.parent.InverseTransformPoint(position);
			
			// update
			m_Velocity.Update(position);
			
			// add point
			LinePoint point = default(LinePoint);
			point.Color = color;
			point.Position = position;
			point.Weight = Mathf.Lerp(options.WeightMin, options.WeightMax, m_Velocity.Factor);
			AddPoint(point);
			
		}
		
		/// <summary>
		/// Called by the <see cref="LineDrawingTool" /> when the touch associated with this line ends on the <see cref="DrawingSurface" />.
		/// </summary>
		public void OnTouchEnded(DrawingSurface surface, Color color, Touch touch, LineOptions options) {
			
			// get position
			Vector2 position = CameraUtils.TouchToWorldPoint(touch, surface.Transform, surface.Camera);
			position = surface.Transform.parent.InverseTransformPoint(position);
			
			// update
			m_Velocity.Update(position);
			
			// add point
			LinePoint point = default(LinePoint);
			point.Color = color;
			point.Position = position;
			point.Weight = Mathf.Lerp(options.WeightMin, options.WeightMax, m_Velocity.Factor);
			AddPoint(point);
			
			// set flags
			m_IsDirty = true;
			m_CapRequired = true;
			
		}
		
		#endregion
		
		
		#region Points
		
		private void AddPoint(LinePoint point) {
			
			bool flag = false;
			
			if (m_PointCount == 0) {
				flag = true;
			} else if ((point.Position - m_Points[m_PointCount - 1].Position).magnitude > 0.01f) {
				flag = true;
			}
			
			if (flag) {
				m_Points[m_PointCount] = point;
				m_PointCount++;
				m_IsDirty = true;
			}
			
		}
		
		#endregion
		
		
		#region Smooth Points
		
		private void GenerateSmoothPoints() {
			
			m_SmoothPointCount = 0;
			
			if (m_PointCount > 2) {
				
				float resolution = 1;
				float step = 0;
				float time = 0;
				
				int countMin = 1;
				int countMax = 8;
				int count = 0;
				
				LinePoint pointA;
				LinePoint pointB;
				LinePoint pointC;
				
				Vector2 positionA;
				Vector2 positionB;
				Vector2 positionC;
				
				float weightA;
				float weightB;
				float weightC;
				
				for (int i = 2; i < m_PointCount; i++) {
					
					pointA = m_Points[i - 2];
					pointB = m_Points[i - 1];
					pointC = m_Points[i - 0];
					
					positionA = Vector2.Lerp(pointA.Position, pointB.Position, 0.5f);
					positionB = pointB.Position;
					positionC = Vector2.Lerp(pointB.Position, pointC.Position, 0.5f);
					
					weightA = Mathf.Lerp(pointA.Weight, pointB.Weight, 0.5f);
					weightB = pointB.Weight;
					weightC = Mathf.Lerp(pointB.Weight, pointC.Weight, 0.5f);
					
					count = (int)(Mathf.Clamp((positionA - positionC).magnitude / resolution, countMin, countMax));
					time = 0.0f;
					step = 1.0f / (float)count;
					
					for(int j = 0; j <= count; j++, time += step) {
						LinePoint point = m_SmoothPoints[m_SmoothPointCount];
						point.Color = Color.Lerp(pointA.Color, pointC.Color, time);
						point.Position = GetBezierPosition(positionA, positionB, positionC, time);
						point.Weight = GetBezierWeight(weightA, weightB, weightC, time);
						m_SmoothPoints[m_SmoothPointCount] = point;
						m_SmoothPointCount++;
					}
					
				}
				
			}
			
		}
		
		private static Vector2 GetBezierPosition(Vector2 a, Vector2 b, Vector2 c, float t) {
		
			// Use quadratic bezier curves to smooth points
			// http://en.wikipedia.org/wiki/B%C3%A9zier_curve#Quadratic_B.C3.A9zier_curves
		
			float i;
			i = 1.0f - t;
		
			Vector2 p;
			p.x = (i * i * a.x) + (2.0f * t * i * b.x) + (t * t * c.x);
			p.y = (i * i * a.y) + (2.0f * t * i * b.y) + (t * t * c.y);
			return p;
		
		}
	
		private static float GetBezierWeight(float a, float b, float c, float t) {
		
			// Use quadratic bezier curves to smooth weights
			// http://en.wikipedia.org/wiki/B%C3%A9zier_curve#Quadratic_B.C3.A9zier_curves
		
			float i;
			i = 1.0f - t;
		
			float weight;
			weight = (i * i * a) + (2.0f * t * i * b) + (t * t * c);
			return weight;
		
		}
		
		#endregion
		
		
	}
	
		
	/// <summary>
	/// A LineGeometry object stores all the vertices, colors and triangles used to draw a line. All of 
	/// the lines reuse one LineGeometry object so that the lines don't have to allocate/deallocate any
	/// arrays, and so that all of the lines can be drawn in a single draw call.
	/// </summary>
	[System.Serializable]
	internal class LineGeometry {
		
		
		#region Fields
		
		/// <summary>
		/// The array of colors.
		/// </summary>
		[SerializeField]
		public Color[] Colors;
		
		/// <summary>
		/// The number of elements in the <see cref="Colors" /> array that have been used.
		/// </summary>
		[SerializeField]
		public int ColorCount;
		
		/// <summary>
		/// The array of triangles.
		/// </summary>
		[SerializeField]
		public int[] Triangles;
		
		/// <summary>
		/// The number of elements in the <see cref="Triangles" /> array that have been used.
		/// </summary>
		[SerializeField]
		public int TriangleCount;
		
		/// <summary>
		/// The array of vertices.
		/// </summary>
		[SerializeField]
		public Vector3[] Vertices;
		
		/// <summary>
		/// The number of elements in the <see cref="Vertices" /> array that have been used.
		/// </summary>
		[SerializeField]
		public int VertexCount;
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new LineGeometry object.
		/// </summary>
		public LineGeometry() {
			Colors = new Color[0];
			ColorCount = 0;
			Triangles = new int[0];
			TriangleCount = 0;
			Vertices = new Vector3[0];
			VertexCount = 0;
		}
		
		#endregion
		
		
		#region Methods
		
		public void AddColorCapacity(int additionalCapacity) {
			if (ColorCount + additionalCapacity > Colors.Length) {
				System.Array.Resize(ref Colors, Colors.Length + additionalCapacity);
			}
		}
		
		public void AddTriangleCapacity(int additionalCapacity) {
			if (TriangleCount + additionalCapacity > Triangles.Length) {
				System.Array.Resize(ref Triangles, Triangles.Length + additionalCapacity);
			}
		}
		
		public void AddVertexCapacity(int additionalCapacity) {
			if (VertexCount + additionalCapacity > Vertices.Length) {
				System.Array.Resize(ref Vertices, Vertices.Length + additionalCapacity);
			}
		}
		
		/// <summary>
		/// Copies the vertices, colors and triangles to the mesh.
		/// </summary>
		public void CopyToMesh(Mesh mesh) {
			
			Color[] colors = new Color[ColorCount];
			System.Array.Copy(Colors, colors, ColorCount);
			
			int[] triangles = new int[TriangleCount];
			System.Array.Copy(Triangles, triangles, TriangleCount);
			
			Vector3[] vertices = new Vector3[VertexCount];
			System.Array.Copy(Vertices, vertices, VertexCount);
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.colors = colors;
			mesh.RecalculateBounds();
			
		}
		
		/// <summary>
		/// Resets the LineGeometry object to it's default values. Reset needs to be called at 
		/// the beginning of each frame (before adding any new geometry) to clear out everything
		/// that was added last frame.
		/// </summary>
		public void Reset() {
			ColorCount = 0;
			TriangleCount = 0;
			VertexCount = 0;
		}
		
		#endregion
		
		
	}
	
		
	/// <summary>
	/// The LineCap struct represents the circular cap at the start and end of the line.
	/// </summary>
	[System.Serializable]
	internal struct LineCap {
		
		
		#region Fields
		
		/// <summary>
		/// How much to overdraw the cap, in meters.
		/// </summary>
		[SerializeField]
		private float Overdraw;
		
		/// <summary>
		/// The point at the center of the cap.
		/// </summary>
		[SerializeField]
		private LinePoint Point;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the cap to the default values.
		/// </summary>
		public void Reset(float overdraw) {
			Overdraw = overdraw;
			Point = default(LinePoint);
		}
		
		/// <summary>
		/// Updates the cap.
		/// </summary>
		public void Update(LinePoint point) {
			Point = point;
		}
		
		/// <summary>
		/// Converts the cap into vertices and adds them to the geometry.
		/// </summary>
		public void Apply(LineGeometry geometry) {
			
			int steps = 30;
			float increment = 360f / (steps - 1);
			
			int colorsPerStep = 5;
			int trianglesPerStep = 9;
			int verticesPerStep = 5;
			
			geometry.AddColorCapacity(steps * colorsPerStep);
			geometry.AddTriangleCapacity(steps * trianglesPerStep);
			geometry.AddVertexCapacity(steps * verticesPerStep);
			
			for (int i = 0; i < steps; i++) {
				
				// F ----------- G
				//  \     /     /
				//   B ------- C
				//    \       /
				//     \     /
				//      \   /
				//       \ /
				//        A
				
				Color colorA = Point.Color;
				Color colorB = new Color(Point.Color.r, Point.Color.g, Point.Color.b, 0);
				
				Vector2 normalA = Quaternion.Euler(0,0,increment * (i - 1)) * Vector2.right;
				Vector2 normalB = Quaternion.Euler(0,0,increment * (i - 0)) * Vector2.right;
				
				float radiusA = Point.Weight;
				float radiusB = Point.Weight + Overdraw;
				
				LineVertex vertexA = new LineVertex();
				vertexA.Color = colorA;
				vertexA.Position = Point.Position;
				
				LineVertex vertexB = new LineVertex();
				vertexB.Color = colorA;
				vertexB.Position = Point.Position + normalA * radiusA;
				
				LineVertex vertexC = new LineVertex();
				vertexC.Color = colorA;
				vertexC.Position = Point.Position + normalB * radiusA;
				
				LineVertex vertexF = new LineVertex();
				vertexF.Color = colorB;
				vertexF.Position = Point.Position + normalA * radiusB;
				
				LineVertex vertexG = new LineVertex();
				vertexG.Color = colorB;
				vertexG.Position = Point.Position + normalB * radiusB;
				
				// add colors
				geometry.Colors[geometry.ColorCount+0] = vertexA.Color;
				geometry.Colors[geometry.ColorCount+1] = vertexB.Color;
				geometry.Colors[geometry.ColorCount+2] = vertexC.Color;
				geometry.Colors[geometry.ColorCount+3] = vertexF.Color;
				geometry.Colors[geometry.ColorCount+4] = vertexG.Color;
				
				// add vertices
				geometry.Vertices[geometry.VertexCount+0] = vertexA.Position;
				geometry.Vertices[geometry.VertexCount+1] = vertexB.Position;
				geometry.Vertices[geometry.VertexCount+2] = vertexC.Position;
				geometry.Vertices[geometry.VertexCount+3] = vertexF.Position;
				geometry.Vertices[geometry.VertexCount+4] = vertexG.Position;
				
				// add triangle - ABC
				geometry.Triangles[geometry.TriangleCount+0] = geometry.VertexCount + 0;
				geometry.Triangles[geometry.TriangleCount+1] = geometry.VertexCount + 1;
				geometry.Triangles[geometry.TriangleCount+2] = geometry.VertexCount + 2;
				
				// add triangle - FGB
				geometry.Triangles[geometry.TriangleCount+3] = geometry.VertexCount + 3;
				geometry.Triangles[geometry.TriangleCount+4] = geometry.VertexCount + 4;
				geometry.Triangles[geometry.TriangleCount+5] = geometry.VertexCount + 1;
				
				// add triangle - BGC
				geometry.Triangles[geometry.TriangleCount+6] = geometry.VertexCount + 1;
				geometry.Triangles[geometry.TriangleCount+7] = geometry.VertexCount + 4;
				geometry.Triangles[geometry.TriangleCount+8] = geometry.VertexCount + 2;
				
				// update stepss
				geometry.ColorCount += colorsPerStep;
				geometry.VertexCount += verticesPerStep;
				geometry.TriangleCount += trianglesPerStep;
				
			}
			
		}
		
		#endregion
		
		
	}
		
	/// <summary>
	/// The LineSegment struct represents a line segment between two points on the line.
	/// </summary>
	[System.Serializable]
	internal struct LineSegment {
		
		
		#region Fields
		
		/// <summary>
		/// The normal of the first point.
		/// </summary>
		[SerializeField]
		public Vector2 NormalA;
		
		/// <summary>
		/// The normal of the second point.
		/// </summary>
		[SerializeField]
		public Vector2 NormalB;
		
		/// <summary>
		/// How much to overdraw the edges of the line segment, in meters.
		/// </summary>
		[SerializeField]
		public float Overdraw;
		
		/// <summary>
		/// The point at the start of the line segment.
		/// </summary>
		[SerializeField]
		public LinePoint PointA;
		
		/// <summary>
		/// The point at the end of the line segment.
		/// </summary>
		[SerializeField]
		public LinePoint PointB;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the line segement to the default values.
		/// </summary>
		public void Reset(float overdraw) {
			NormalA = default(Vector2);
			NormalB = default(Vector2);
			Overdraw = overdraw;
			PointA = default(LinePoint);
			PointB = default(LinePoint);
		}
		
		/// <summary>
		/// Updates the line segment.
		/// </summary>
		public void Update(LinePoint pointA, LinePoint pointB) {
			
			bool empty = PointA.Equals(default(LinePoint)) && PointB.Equals(default(LinePoint));
			Vector2 normalA = new Vector2(PointA.Position.y - PointB.Position.y, PointB.Position.x - PointA.Position.x).normalized;
			Vector2 normalB = new Vector2(pointA.Position.y - pointB.Position.y, pointB.Position.x - pointA.Position.x).normalized;
			
			NormalA = empty ? normalB : normalA;
			NormalB = normalB;
			
			PointA = pointA;
			PointB = pointB;
			
		}
		
		/// <summary>
		/// Converts the line segment into vertices and adds them to the geometry.
		/// </summary>
		public void Apply(LineGeometry geometry) {
			
			// F ------------------- G
			// |          /          |
			// A ------------------- C
			// |           /         |
			// |          /          |
			// |         /           |
			// B ------------------- D
			// |          /          |
			// H ------------------- I
			
			int steps = 1;
			int colorsPerStep = 8;
			int trianglesPerStep = 18;
			int verticesPerStep = 8;
			
			geometry.AddColorCapacity(steps * colorsPerStep);
			geometry.AddTriangleCapacity(steps * trianglesPerStep);
			geometry.AddVertexCapacity(steps * verticesPerStep);
			
			LineVertex vertexA = new LineVertex();
			vertexA.Color = PointA.Color;
			vertexA.Position = PointA.Position + NormalA * PointA.Weight;
			
			LineVertex vertexB = new LineVertex();
			vertexB.Color = PointA.Color;
			vertexB.Position = PointA.Position - NormalA * PointA.Weight;
			
			LineVertex vertexC = new LineVertex();
			vertexC.Color = PointB.Color;
			vertexC.Position = PointB.Position + NormalB * PointB.Weight;
			
			LineVertex vertexD = new LineVertex();
			vertexD.Color = PointB.Color;
			vertexD.Position = PointB.Position - NormalB * PointB.Weight;
			
			LineVertex vertexF = new LineVertex();
			vertexF.Color = PointA.Transparent;
			vertexF.Position = PointA.Position + NormalA * (PointA.Weight + Overdraw);
			
			LineVertex vertexG = new LineVertex();
			vertexG.Color = PointB.Transparent;
			vertexG.Position = PointB.Position + NormalB * (PointB.Weight + Overdraw);
			
			LineVertex vertexH = new LineVertex();
			vertexH.Color = PointA.Transparent;
			vertexH.Position = PointA.Position - NormalA * (PointA.Weight + Overdraw);
			
			LineVertex vertexI = new LineVertex();
			vertexI.Color = PointB.Transparent;
			vertexI.Position = PointB.Position - NormalB * (PointB.Weight + Overdraw);
			
			// add colors
			geometry.Colors[geometry.ColorCount+0] = vertexA.Color;
			geometry.Colors[geometry.ColorCount+1] = vertexB.Color;
			geometry.Colors[geometry.ColorCount+2] = vertexC.Color;
			geometry.Colors[geometry.ColorCount+3] = vertexD.Color;
			geometry.Colors[geometry.ColorCount+4] = vertexF.Color;
			geometry.Colors[geometry.ColorCount+5] = vertexG.Color;
			geometry.Colors[geometry.ColorCount+6] = vertexH.Color;
			geometry.Colors[geometry.ColorCount+7] = vertexI.Color;
			
			// add vertices
			geometry.Vertices[geometry.VertexCount+0] = vertexA.Position;
			geometry.Vertices[geometry.VertexCount+1] = vertexB.Position;
			geometry.Vertices[geometry.VertexCount+2] = vertexC.Position;
			geometry.Vertices[geometry.VertexCount+3] = vertexD.Position;
			geometry.Vertices[geometry.VertexCount+4] = vertexF.Position;
			geometry.Vertices[geometry.VertexCount+5] = vertexG.Position;
			geometry.Vertices[geometry.VertexCount+6] = vertexH.Position;
			geometry.Vertices[geometry.VertexCount+7] = vertexI.Position;
			
			// add triangle - ACB
			geometry.Triangles[geometry.TriangleCount+0] = geometry.VertexCount + 0;
			geometry.Triangles[geometry.TriangleCount+1] = geometry.VertexCount + 2;
			geometry.Triangles[geometry.TriangleCount+2] = geometry.VertexCount + 1;
			
			// add triangle - BCD
			geometry.Triangles[geometry.TriangleCount+3] = geometry.VertexCount + 1;
			geometry.Triangles[geometry.TriangleCount+4] = geometry.VertexCount + 2;
			geometry.Triangles[geometry.TriangleCount+5] = geometry.VertexCount + 3;
			
			// add triangle - FGA
			geometry.Triangles[geometry.TriangleCount+6] = geometry.VertexCount + 4;
			geometry.Triangles[geometry.TriangleCount+7] = geometry.VertexCount + 5;
			geometry.Triangles[geometry.TriangleCount+8] = geometry.VertexCount + 0;
			
			// add triangle - AGC
			geometry.Triangles[geometry.TriangleCount+9] = geometry.VertexCount + 0;
			geometry.Triangles[geometry.TriangleCount+10] = geometry.VertexCount + 5;
			geometry.Triangles[geometry.TriangleCount+11] = geometry.VertexCount + 2;
			
			// add triangle - BDH
			geometry.Triangles[geometry.TriangleCount+12] = geometry.VertexCount + 1;
			geometry.Triangles[geometry.TriangleCount+13] = geometry.VertexCount + 3;
			geometry.Triangles[geometry.TriangleCount+14] = geometry.VertexCount + 6;
			
			// add triangle - HDI
			geometry.Triangles[geometry.TriangleCount+15] = geometry.VertexCount + 6;
			geometry.Triangles[geometry.TriangleCount+16] = geometry.VertexCount + 3;
			geometry.Triangles[geometry.TriangleCount+17] = geometry.VertexCount + 7;
			
			// update counts
			geometry.ColorCount += colorsPerStep;
			geometry.VertexCount += verticesPerStep;
			geometry.TriangleCount += trianglesPerStep;
			
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The LineVelocity represents the velocity of last point on the line (the touch) as 
	/// the line is being drawn. It uses an exponential moving average to smooth the values.
	/// </summary>
	[System.Serializable]
	internal struct LineVelocity {
		
		
		#region Fields
		
		/// <summary>
		/// The average velocity, in meters per second.
		/// </summary>
		[SerializeField]
		public float Average;
		
		/// <summary>
		/// The current velocity, in meters per second.
		/// </summary>
		[SerializeField]
		public float Current;
		
		/// <summary>
		/// The maximum velocity, in meters per second.
		/// </summary>
		[SerializeField]
		public float Max;
		
		/// <summary>
		/// The minimum velocity, in meters per second.
		/// </summary>
		[SerializeField]
		public float Min;
		
		/// <summary>
		/// The position of the touch in world space.
		/// </summary>
		[SerializeField]
		public Vector2 Position;
		
		/// <summary>
		/// How much smoothing to use when calculating the average velocity.
		/// </summary>
		[SerializeField]
		public float Smoothing;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// The position of the average velocity between the min and max.
		/// </summary>
		public float Factor {
			get { return Mathf.Clamp((Average - Min) / (Max - Min), 0.0f, 1.0f); }
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the velocity to the default values.
		/// </summary>
		public void Reset(Vector2 position, float min, float max, float smoothing) {
			Average = 0;
			Current = 0;
			Max = max;
			Position = position;
			Min = min;
			Smoothing = smoothing;
		}
		
		/// <summary>
		/// Updates the velocity.
		/// </summary>
		public void Update(Vector2 position) {
			// http://en.wikipedia.org/wiki/Exponential_smoothing#The_exponential_moving_average
			Average = (Smoothing * Current) + ((1f - Smoothing) * Average);
			Current = (position - Position).magnitude / Time.deltaTime;
			Position = position;
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The LineOptions struct stores the configurable options for the <see cref="LineDrawingTool" />. 
	/// The values may be different from project to project, depending on the scene setup and the 
	/// how the designer wants the line to look and feel.
	/// </summary>
	[System.Serializable]
	internal struct LineOptions {
		
		
		#region Fields
		
		/// <summary>
		/// How much to overdraw the edges of the line, in meters. Overdraw helps to smooth the 
		/// edges of the line, especially when antialiasing is disabled. For the best result, 
		/// use a value that is about 1-2 pixels when converted to screen space.
		/// </summary>
		[Range(0.01f, 1f)]
		[SerializeField]
		public float Overdraw;
		
		/// <summary>
		/// How fast a touch has to move (in meters per second) until the line weight reaches the maximum.
		/// </summary>
		[SerializeField]
		public float VelocityMax;
		
		/// <summary>
		/// How fast a touch has to move (in meters per second) before the line weight will go above the minimum.
		/// </summary>
		[SerializeField]
		public float VelocityMin;
		
		/// <summary>
		/// How much to smooth the touch velocity. Higher values will make the line weight change 
		/// more smoothly, lower values will make the line weight change more quickly.
		/// </summary>
		[Range(0f, 1f)]
		[SerializeField]
		public float VelocitySmoothing;
		
		/// <summary>
		/// The maximum line weight, in meters.
		/// </summary>
		[SerializeField]
		public float WeightMax;
		
		/// <summary>
		/// The minimum line weight, in meters.
		/// </summary>
		[SerializeField]
		public float WeightMin;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the options to their default values.
		/// </summary>
		public void Reset() {
			Overdraw = 0.01f;
			VelocityMax = 150f;
			VelocityMin = 10f;
			VelocitySmoothing = 0.2f;
			WeightMax = 0.2f;
			WeightMin = 0.1f;
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The LinePoint struct represents a single point on a <see cref="Line">.
	/// </summary>
	[System.Serializable]
	internal struct LinePoint {
		
		
		#region Fields
		
		[SerializeField]
		public Color Color;
		
		[SerializeField]
		public Vector2 Position;
		
		[SerializeField]
		public float Weight;
		
		#endregion
		
		
		#region Properties
		
		public Color Transparent {
			get { return new Color(Color.r, Color.g, Color.b, 0); }
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The LineVertex struct represents a single vertex in a <see cref="LineCap" /> or <see cref="LineSegment">.
	/// </summary>
	[System.Serializable]
	internal struct LineVertex {
		
		
		#region Fields
		
		[SerializeField]
		public Color Color;
		
		[SerializeField]
		public Vector2 Position;
		
		#endregion
		
		
	}
	
	
}

