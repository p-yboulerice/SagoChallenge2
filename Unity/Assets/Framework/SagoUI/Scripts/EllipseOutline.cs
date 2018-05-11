namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class EllipseOutline : BaseMeshEffect {
		
		
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
			
			if (!Graphic || !(Graphic is EllipseGraphic) || Type == OutlineType.None) {
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
			
			GraphicUtil.AddEllipseOutline(verts, rect, Color, Size, Type, 0, 360);
			
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