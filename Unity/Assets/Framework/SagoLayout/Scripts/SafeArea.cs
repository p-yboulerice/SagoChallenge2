namespace SagoLayout {

	using System;
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoPlatform;
	using SagoUtils;
	using System.Linq;
	using System.Runtime.InteropServices;

	public class SafeArea {

		#region iOS Native Binder

			#if SAGO_IOS && !UNITY_EDITOR

			[DllImport ("__Internal")]
			private static extern void _GetSafeArea(out float x, out float y, out float w, out float h);

			[DllImport ("__Internal")]
			private static extern void _GetSafeAreaInsets(out int x, out int y, out int w, out int h);

			#endif

		#endregion


		#region Public Methods

			public static Rect GetSafeArea() {

				float originX = 0;
				float originY = 0;
				float width = Screen.width;
				float height = Screen.height;

				#if SAGO_IOS && !UNITY_EDITOR

				_GetSafeArea(out originX, out originY, out width, out height);
				
				#endif

				Rect screenRect = new Rect(originX, originY, width, height);
				return screenRect;
			}

			public static RectOffset GetSafeAreaInsets() {

				int left = 0;
				int right = 0;
				int top = 0;
				int bottom = 0;

				#if SAGO_IOS && !UNITY_EDITOR

				_GetSafeAreaInsets(out left, out bottom, out right, out top);
				
				#endif

				RectOffset screenRect = new RectOffset(left, bottom, right, top);
				return screenRect;
			}

		#endregion
	}
}