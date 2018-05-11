namespace World.Store {
	
	using UnityEngine;
	using UnityEngine.UI;
	
	public static class CanvasExtentions {
		
		public static Rect GetScreenRect(this Canvas canvas, RectTransform rectTransform) {
			
			Vector3[] rectCorners = new Vector3[4];
			rectTransform.GetWorldCorners(rectCorners);
			
			Vector3[] screenCorners = new Vector3[2];
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace) {
				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectCorners[1]);
				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectCorners[3]);
			} else {
				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, rectCorners[1]);
				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, rectCorners[3]);
			}
			screenCorners[0].y = Screen.height - screenCorners[0].y;
			screenCorners[1].y = Screen.height - screenCorners[1].y;
			
			return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
			
		}
		
	}
	
}