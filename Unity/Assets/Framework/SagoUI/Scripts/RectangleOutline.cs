namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class RectangleOutline : BaseMeshEffect {
		
		
		#region Fields
		
		[SerializeField]
		private Color m_Color;
		
		[System.NonSerialized]
		private Graphic m_Graphic;
		
		[SerializeField]
		private int m_Size;
		
		[SerializeField]
		private OutlineType m_Type;
		
		#endregion
		
		
		#region Properties
		
		public Color Color {
			get { return m_Color; }
			set {
				if (m_Color != value) {
					m_Color = value;
					Graphic.SetVerticesDirty();
				}
			}
		}
		
		public Graphic Graphic {
			get { return m_Graphic = m_Graphic ?? GetComponent<Graphic>(); }
		}
		
		public int Size {
			get { return m_Size; }
			set {
				if (m_Size != value) {
					m_Size = value;
					Graphic.SetVerticesDirty();
				}
			}
		}
		
		public OutlineType Type {
			get { return m_Type; }
			set {
				if (m_Type != value) {
					m_Type = value;
					Graphic.SetVerticesDirty();
				}
			}
		}
		
		#endregion
		
		
		#region Methods
		
		override public void ModifyMesh(VertexHelper helper) {
			
			if (!Graphic || !(Graphic is RectangleGraphic) || Type == OutlineType.None) {
				return;
			}
			
			Rect rect;
			rect = Graphic.rectTransform.rect;
			
			if (GraphicUtil.IsEmpty(rect)) {
				return;
			}
			
			float radius = (Graphic as RectangleGraphic).Radius;
			radius = Mathf.Min(radius, rect.width * 0.5f);
			radius = Mathf.Min(radius, rect.height * 0.5f);
			
			float size;
			size = radius * 2;
			
			List<UIVertex> verts;
			verts = ListPool<UIVertex>.Get();
			helper.GetUIVertexStream(verts);
			
			GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMin, rect.yMin, size, size), Color, Size, Type, 180, 90);
			GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMin, rect.yMax - size, size, size), Color, Size, Type, 90, 90);
			GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMax - size, rect.yMax - size, size, size), Color, Size, Type, 0, 90);
			GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMax - size, rect.yMin, size, size), Color, Size, Type, 270, 90);
			
			Rect horizontal = new Rect(rect.xMin, rect.yMin + radius, rect.width, rect.height - size);
			GraphicUtil.AddRectOutline(verts, horizontal, Color, Size, Type, EdgeType.Left);
			GraphicUtil.AddRectOutline(verts, horizontal, Color, Size, Type, EdgeType.Right);
			
			Rect vertical = new Rect(rect.xMin + radius, rect.yMin, rect.width - size, rect.height);
			GraphicUtil.AddRectOutline(verts, vertical, Color, Size, Type, EdgeType.Top);
			GraphicUtil.AddRectOutline(verts, vertical, Color, Size, Type, EdgeType.Bottom);
			
			helper.Clear();
			helper.AddUIVertexTriangleStream(verts);
			ListPool<UIVertex>.Put(verts);
			
		}
		
		#if UNITY_EDITOR
		override protected void Reset() {
			base.Reset();
			Color = Color.black;
			Type = OutlineType.Outer;
			Size = 1;
		}
		#endif
		
		#endregion
		
		
	}
	
}