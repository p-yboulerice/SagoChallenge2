namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class RectangleGraphic : MaskableGraphic {
		
		
		#region Fields
		
		[SerializeField]
		private float m_Radius;
		
		[SerializeField]
		private FillType m_Type;
		
		#endregion
		
		
		#region Properties
		
		public float Radius {
			get { return m_Radius; }
			set {
				if (m_Radius != value) {
					m_Radius = value;
					SetVerticesDirty();
				}
			}
		}
		
		public FillType Type {
			get { return m_Type; }
			set {
				if (m_Type != value) {
					m_Type = value;
					SetVerticesDirty();
				}
			}
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		override protected void OnPopulateMesh(VertexHelper helper) {
			
			Rect rect;
			rect = rectTransform.rect;
			
			if (GraphicUtil.IsEmpty(rect) || Type == FillType.None) {
				helper.Clear();
				return;
			}
			
			float radius = Radius;
			radius = Mathf.Min(radius, rect.width * 0.5f);
			radius = Mathf.Min(radius, rect.height * 0.5f);
			
			float size;
			size = radius * 2;
			
			List<UIVertex> verts;
			verts = ListPool<UIVertex>.Get();
			
			GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMin, rect.yMin, size, size), color, Type, 180, 90);
			GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMin, rect.yMax - size, size, size), color, Type, 90, 90);
			GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMax - size, rect.yMax - size, size, size), color, Type, 0, 90);
			GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMax - size, rect.yMin, size, size), color, Type, 270, 90);
			
			
			GraphicUtil.AddRectFill(verts, new Rect(rect.xMin, rect.yMin + radius, radius, rect.height - size), color, Type);
			GraphicUtil.AddRectFill(verts, new Rect(rect.xMin + radius, rect.yMin, rect.width - size, rect.height), color, Type);
			GraphicUtil.AddRectFill(verts, new Rect(rect.xMax - radius, rect.yMin + radius, radius, rect.height - size), color, Type);
			
			helper.Clear();
			helper.AddUIVertexTriangleStream(verts);
			ListPool<UIVertex>.Put(verts);
			
		}
		
		#if UNITY_EDITOR
		override protected void Reset() {
			base.Reset();
			Radius = 0;
			Type = FillType.Fill;
		}
		#endif
		
		#endregion
		
		
	}
	
}
