namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class CapsuleOutline : BaseMeshEffect {
		
		
		#region Fields
		
		[SerializeField]
		private Color m_Color;
		
		[System.NonSerialized]
		private CapsuleGraphic m_Graphic;
		
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
		
		public CapsuleGraphic Graphic {
			get { return m_Graphic = m_Graphic ?? GetComponent<CapsuleGraphic>(); }
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
			
			if (!Graphic || !(Graphic is CapsuleGraphic) || Type == OutlineType.None) {
				return;
			}
			
			Rect rect;
			rect = Graphic.rectTransform.rect;
			
			if (GraphicUtil.IsEmpty(rect)) {
				return;
			}
				
			List<UIVertex> verts;
			verts = ListPool<UIVertex>.Get();
			helper.GetUIVertexStream(verts);
			
			if (rect.height > rect.width) {
				GraphicUtil.AddEllipseOutline(verts, new Rect(rect.x, rect.yMax - rect.width, rect.width, rect.width), Color, Size, Type, 0, 180);
				GraphicUtil.AddRectOutline(verts, new Rect(rect.x, rect.y + rect.width * 0.5f, rect.width, rect.height - rect.width), Color, Size, Type, EdgeType.Left);
				GraphicUtil.AddEllipseOutline(verts, new Rect(rect.x, rect.yMin, rect.width, rect.width), Color, Size, Type, 180, 180);
				GraphicUtil.AddRectOutline(verts, new Rect(rect.x, rect.y + rect.width * 0.5f, rect.width, rect.height - rect.width), Color, Size, Type, EdgeType.Right);
			} else {
				GraphicUtil.AddRectOutline(verts, new Rect(rect.xMin + rect.height * 0.5f, rect.y, rect.width - rect.height, rect.height), Color, Size, Type, EdgeType.Top);
				GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMin, rect.y, rect.height, rect.height), Color, Size, Type, 90, 180);
				GraphicUtil.AddRectOutline(verts, new Rect(rect.xMin + rect.height * 0.5f, rect.y, rect.width - rect.height, rect.height), Color, Size, Type, EdgeType.Bottom);
				GraphicUtil.AddEllipseOutline(verts, new Rect(rect.xMax - rect.height, rect.y, rect.height, rect.height), Color, Size, Type, 270, 180);
			}
			
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