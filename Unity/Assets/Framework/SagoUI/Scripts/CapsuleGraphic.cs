namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class CapsuleGraphic : MaskableGraphic {
		
		
		#region Fields
		
		[SerializeField]
		private FillType m_Type;
		
		#endregion
		
		
		#region Properties
		
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
		
		
		#region Methods
		
		override protected void OnPopulateMesh(VertexHelper helper) {
			
			Rect rect;
			rect = rectTransform.rect;
			
			if (Type == FillType.None || GraphicUtil.IsEmpty(rect)) {
				helper.Clear();
				return;
			}
				
			List<UIVertex> verts;
			verts = ListPool<UIVertex>.Get();
			
			if (rect.height > rect.width) {
				GraphicUtil.AddEllipseFill(verts, new Rect(rect.x, rect.yMax - rect.width, rect.width, rect.width), color, Type, 0, 180);
				GraphicUtil.AddRectFill(verts, new Rect(rect.x, rect.y + rect.width * 0.5f, rect.width, rect.height - rect.width), color, Type);
				GraphicUtil.AddEllipseFill(verts, new Rect(rect.x, rect.yMin, rect.width, rect.width), color, Type, 180, 180);
			} else {
				GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMin, rect.y, rect.height, rect.height), color, Type, 90, 180);
				GraphicUtil.AddRectFill(verts, new Rect(rect.x + rect.height * 0.5f, rect.y, rect.width - rect.height, rect.height), color, Type);
				GraphicUtil.AddEllipseFill(verts, new Rect(rect.xMax - rect.height, rect.y, rect.height, rect.height), color, Type, 270, 180);
			}
			
			helper.Clear();
			helper.AddUIVertexTriangleStream(verts);
			ListPool<UIVertex>.Put(verts);
			
		}
		
		#if UNITY_EDITOR
		override protected void Reset() {
			base.Reset();
			Type = FillType.Fill;
		}
		#endif
		
		#endregion
		
		
	}
	
}
