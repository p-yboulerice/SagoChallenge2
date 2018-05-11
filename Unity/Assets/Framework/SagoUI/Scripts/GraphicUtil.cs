namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	
	public enum EdgeType {
		Top,
		Left,
		Bottom,
		Right
	}
	
	public enum FillType {
		None,
		Fill
	}
	
	public enum OutlineType {
		None,
		Outer,
		Inner
	}
	
	public static class GraphicUtil {
		
		
		#region Static Methods
		
		public static void AddEllipseFill(List<UIVertex> verts, Rect rect, Color color, FillType fillType, int offset, int length) {
			
			if (IsEmpty(rect)|| fillType == FillType.None || length == 0) {
				return;
			}
			
			int count = length / 6;
			float degrees = offset;
			int increment = length / count;
			Vector2 radius = rect.size * 0.5f;
			
			UIVertex[] triangle = new UIVertex[3];
			for (int index = 0; index < triangle.Length; index++) {
				triangle[index] = UIVertex.simpleVert;
				triangle[index].color = color;
				triangle[index].position = rect.center;
			}
			
			for (int index = 0; index <= count; index++, degrees += increment) {
				
				float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
				float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
				
				triangle[1].position = triangle[2].position;
				triangle[2].position = rect.center + (Vector2.right * cos * radius.x) + (Vector2.up * sin * radius.y);
					
				if (index > 0) {
					verts.AddRange(triangle);
				}
				
			}
			
		}
		
		public static void AddRectFill(List<UIVertex> verts, Rect rect, Color color, FillType fillType) {
			
			if (IsEmpty(rect) || fillType == FillType.None) {
				return;
			}
			
			UIVertex[] triangles = new UIVertex[6];
			for (int index = 0; index < triangles.Length; index++) {
				triangles[index] = UIVertex.simpleVert;
				triangles[index].color = color;
			}
			
			triangles[0].position = new Vector2(rect.xMin, rect.yMin);
			triangles[1].position = new Vector2(rect.xMin, rect.yMax);
			triangles[2].position = new Vector2(rect.xMax, rect.yMax);
			triangles[3].position = triangles[2].position;
			triangles[4].position = new Vector2(rect.xMax, rect.yMin);
			triangles[5].position = triangles[0].position;
			
			verts.AddRange(triangles);
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void AddEllipseOutline(List<UIVertex> verts, Rect rect, Color color, int size, OutlineType outlineType, int offset, int length) {
			
			if (IsEmpty(rect) || outlineType == OutlineType.None || length == 0) {
				return;
			}
			
			int count = length / 6;
			float degrees = offset;
			int increment = length / count;
			Vector2 innerRadius = Vector2.zero;
			Vector2 outerRadius = Vector2.zero;
			
			switch (outlineType) {
				case OutlineType.Inner:
					innerRadius = rect.size * 0.5f - Vector2.one * size;
					outerRadius = rect.size * 0.5f;
				break;
				case OutlineType.Outer:
					innerRadius = rect.size * 0.5f;
					outerRadius = rect.size * 0.5f + Vector2.one * size;
				break;
			}
			
			UIVertex[] triangles = new UIVertex[6];
			for (int index = 0; index < triangles.Length; index++) {
				triangles[index] = UIVertex.simpleVert;
				triangles[index].color = color;
			}
			
			for (int index = 0; index <= count; index++, degrees += increment) {
				
				float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
				float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
				
				triangles[0].position = triangles[4].position;
				triangles[1].position = triangles[3].position;
				triangles[5].position = triangles[4].position;
				
				triangles[2].position = rect.center + (Vector2.right * cos * outerRadius.x) + (Vector2.up * sin * outerRadius.y);
				triangles[3].position = triangles[2].position;
				triangles[4].position = rect.center + (Vector2.right * cos * innerRadius.x) + (Vector2.up * sin * innerRadius.y);
				
				if (index > 0) {
					verts.AddRange(triangles);
				}
				
			}
			
		}
		
		public static void AddRectOutline(List<UIVertex> verts, Rect rect, Color color, int size, OutlineType outlineType, EdgeType edgeType) {
			
			if (IsEmpty(rect) || outlineType == OutlineType.None) {
				return;
			}
			
			UIVertex[] triangles = new UIVertex[6];
			for (int index = 0; index < triangles.Length; index++) {
				triangles[index] = UIVertex.simpleVert;
				triangles[index].color = color;
			}
			
			float innerRadius = 0;
			float outerRadius = 0;
			
			switch (outlineType) {
				case OutlineType.Inner:
					innerRadius = -size;
					outerRadius = 0;
				break;
				case OutlineType.Outer:
					innerRadius = 0;
					outerRadius = size;
				break;
			}
			
			switch (edgeType) {
				case EdgeType.Top: // top
					triangles[0].position = new Vector2(rect.xMin, rect.yMax + innerRadius);
					triangles[1].position = new Vector2(rect.xMin, rect.yMax + outerRadius);
					triangles[2].position = new Vector2(rect.xMax, rect.yMax + outerRadius);
					triangles[3].position = triangles[2].position;
					triangles[4].position = new Vector2(rect.xMax, rect.yMax + innerRadius);
					triangles[5].position = triangles[0].position;
				break;
				case EdgeType.Left: // left
					triangles[0].position = new Vector2(rect.xMin - outerRadius, rect.yMin);
					triangles[1].position = new Vector2(rect.xMin - outerRadius, rect.yMax);
					triangles[2].position = new Vector2(rect.xMin - innerRadius, rect.yMax);
					triangles[3].position = triangles[2].position;
					triangles[4].position = new Vector2(rect.xMin - innerRadius, rect.yMin);
					triangles[5].position = triangles[0].position;
				break;
				case EdgeType.Bottom: // bottom
					triangles[0].position = new Vector2(rect.xMin, rect.yMin - innerRadius);
					triangles[1].position = new Vector2(rect.xMin, rect.yMin - outerRadius);
					triangles[2].position = new Vector2(rect.xMax, rect.yMin - outerRadius);
					triangles[3].position = triangles[2].position;
					triangles[4].position = new Vector2(rect.xMax, rect.yMin - innerRadius);
					triangles[5].position = triangles[0].position;
				break;
				case EdgeType.Right: // right
					triangles[0].position = new Vector2(rect.xMax + outerRadius, rect.yMin);
					triangles[1].position = new Vector2(rect.xMax + outerRadius, rect.yMax);
					triangles[2].position = new Vector2(rect.xMax + innerRadius, rect.yMax);
					triangles[3].position = triangles[2].position;
					triangles[4].position = new Vector2(rect.xMax + innerRadius, rect.yMin);
					triangles[5].position = triangles[0].position;
				break;
			}
			
			verts.AddRange(triangles);
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static bool IsEmpty(Rect rect) {
			return rect.size.magnitude < 1;
		}
		
		#endregion
		
		
	}
	
}