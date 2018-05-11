namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class EllipseGraphic : MaskableGraphic {
		
		
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
			
			GraphicUtil.AddEllipseFill(verts, rect, color, Type, 0, 360);
			
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