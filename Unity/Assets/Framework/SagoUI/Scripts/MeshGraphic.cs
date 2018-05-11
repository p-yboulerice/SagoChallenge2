namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Stopwatch = System.Diagnostics.Stopwatch;
	
	public enum MeshGraphicAlignMode {
		None,
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight
	}
	
	public enum MeshGraphicColorMode {
		Color,
		VertexColor
	}
	
	public enum MeshGraphicScaleMode {
		None,
		AspectFill,
		AspectFit,
		Fill
	}
	
	public class MeshGraphic : MaskableGraphic, ILayoutElement {
		
		
		#region Fields
		
		[SerializeField]
		private Mesh m_Mesh;
		
		[SerializeField]
		private int m_PixelsPerUnit;
		
		[SerializeField]
		private MeshGraphicAlignMode m_AlignMode;
		
		[SerializeField]
		private MeshGraphicColorMode m_ColorMode;
		
		[SerializeField]
		private MeshGraphicScaleMode m_ScaleMode;
		
		#endregion
		
		
		#region Properties
		
		public MeshGraphicAlignMode AlignMode {
			get { return m_AlignMode; }
			set {
				if (m_AlignMode != value) {
					m_AlignMode = value;
					SetVerticesDirty();
				}
			}
		}
		
		public MeshGraphicColorMode ColorMode {
			get { return m_ColorMode; }
			set {
				if (m_ColorMode != value) {
					m_ColorMode = value;
					SetVerticesDirty();
				}
			}
		}
		
		public Mesh Mesh {
			get { return m_Mesh; }
			set {
				if (m_Mesh != value) {
					m_Mesh = value;
					SetVerticesDirty();
				}
			}
		}
		
		public int PixelsPerUnit {
			get { return m_PixelsPerUnit; }
			set {
				if (m_PixelsPerUnit != value) {
					m_PixelsPerUnit = value;
					SetVerticesDirty();
				}
			}
		}
		
		public MeshGraphicScaleMode ScaleMode {
			get { return m_ScaleMode; }
			set {
				if (m_ScaleMode != value) {
					m_ScaleMode = value;
					SetVerticesDirty();
				}
			}
		}
		
		#endregion
		
		
		#region Methods
		
		override protected void OnPopulateMesh(VertexHelper helper) {
			
			// var total = new Stopwatch();
			// total.Start();
			
			if (!Mesh || !Mesh.isReadable) {
				helper.Clear();
				return;
			}
			
			int[] triangles = Mesh.triangles;
			Color[] colors = Mesh.colors;
			Vector3[] positions = Mesh.vertices;
			
			List<int> indices;
			indices = ListPool<int>.Get();
			for (int index = 0; index < triangles.Length; index++) {
				indices.Add(triangles[index]);
			}
			
			Rect rect;
			rect = rectTransform.rect;
			
			Vector2 scale;
			scale = Vector2.zero;
			
			switch (ScaleMode) {
				case MeshGraphicScaleMode.None:
					scale = Vector2.one * PixelsPerUnit;
				break;
				case MeshGraphicScaleMode.AspectFill:
					scale.x = scale.y = Mathf.Max(rect.width / Mesh.bounds.size.x, rect.height / Mesh.bounds.size.y);
				break;
				case MeshGraphicScaleMode.AspectFit:
					scale.x = scale.y = Mathf.Min(rect.width / Mesh.bounds.size.x, rect.height / Mesh.bounds.size.y);
				break;
				case MeshGraphicScaleMode.Fill:
					scale.x = rect.width / Mesh.bounds.size.x;
					scale.y = rect.height / Mesh.bounds.size.y;
				break;
			}
			
			Bounds bounds;
			bounds = Mesh.bounds;
			bounds.SetMinMax(Vector2.Scale(bounds.min, scale), Vector2.Scale(bounds.max, scale));
			
			Vector2 offset;
			offset = (Vector2)rect.center;
			
			switch (AlignMode) {
				case MeshGraphicAlignMode.UpperLeft:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * 0.5f;
					offset.y += (bounds.size.y - rect.height) * -0.5f;
				break;
				case MeshGraphicAlignMode.UpperCenter:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.y += (bounds.size.y - rect.height) * -0.5f;
				break;
				case MeshGraphicAlignMode.UpperRight:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * -0.5f;
					offset.y += (bounds.size.y - rect.height) * -0.5f;
				break;
				case MeshGraphicAlignMode.MiddleLeft:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * 0.5f;
				break;
				case MeshGraphicAlignMode.MiddleCenter:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
				break;
				case MeshGraphicAlignMode.MiddleRight:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * -0.5f;
				break;
				case MeshGraphicAlignMode.LowerLeft:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * 0.5f;
					offset.y += (bounds.size.y - rect.height) * 0.5f;
				break;
				case MeshGraphicAlignMode.LowerCenter:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.y += (bounds.size.y - rect.height) * 0.5f;
				break;
				case MeshGraphicAlignMode.LowerRight:
					offset = (Vector2)rect.center - (Vector2)bounds.center;
					offset.x += (bounds.size.x - rect.width) * -0.5f;
					offset.y += (bounds.size.y - rect.height) * 0.5f;
				break;
			}
			
			List<UIVertex> vertices;
			vertices = ListPool<UIVertex>.Get();
			for (int index = 0; index < positions.Length; index++) {
				UIVertex vert = UIVertex.simpleVert;
				vert.position = Vector2.Scale(positions[index], scale) + offset;
				vert.color = ColorMode == MeshGraphicColorMode.VertexColor ? colors[index] : color;
				vertices.Add(vert);
			}
			
			helper.Clear();
			helper.AddUIVertexStream(vertices, indices);
			
			ListPool<int>.Put(indices);
			ListPool<UIVertex>.Put(vertices);
			
			// total.Stop();
			// Debug.LogFormat(this, "MeshGraphic.OnPopulateMesh(): {0}ms", total.ElapsedMilliseconds);
			
		}
		
		#if UNITY_EDITOR
		override protected void Reset() {
			base.Reset();
			AlignMode = MeshGraphicAlignMode.MiddleCenter;
			ColorMode = MeshGraphicColorMode.VertexColor;
			Mesh = null;
			PixelsPerUnit = 100;
			ScaleMode = MeshGraphicScaleMode.None;
			SetVerticesDirty();
		}
		#endif
		
		#endregion
		
		
		#region Properties – ILayoutElement
		
		public float flexibleHeight {
			get;
			private set;
		}
		
		public float flexibleWidth {
			get;
			private set;
		}
		
		public int layoutPriority {
			get;
			private set;
		}
		
		public float minHeight {
			get;
			private set;
		}
		
		public float minWidth {
			get;
			private set;
		}
		
		public float preferredHeight {
			get;
			private set;
		}
		
		public float preferredWidth {
			get;
			private set;
		}
		
		#endregion
		
		
		#region Methods – ILayoutElement 
		
		public void CalculateLayoutInputHorizontal() {
			
			flexibleWidth = 1;
			minWidth = -1;
			preferredWidth = -1;
			
			if (Mesh != null && Mesh.bounds.size.x > 0) {
				preferredWidth = Mesh.bounds.size.x * PixelsPerUnit;
			}
			
			if (ScaleMode == MeshGraphicScaleMode.None) {
				minWidth = preferredWidth;
			}
			
		}
		
		public void CalculateLayoutInputVertical() {
			
			flexibleHeight = 1;
			minHeight = -1;
			preferredHeight = -1;
			
			if (Mesh != null && Mesh.bounds.size.y > 0) {
				preferredHeight = Mesh.bounds.size.y * PixelsPerUnit;
			}
			
			if (ScaleMode == MeshGraphicScaleMode.None) {
				minHeight = preferredHeight;
			}
			
		}
		
		#endregion
		
		
	}
	
}