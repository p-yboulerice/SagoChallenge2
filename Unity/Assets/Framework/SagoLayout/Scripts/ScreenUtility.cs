namespace SagoLayout {

	using UnityEngine;

	public class ScreenUtility {


		//
		// Constants
		//
		public const int KINDLE_HEIGHT_IN_POINTS = 800;
		public const int KINDLE_WIDTH_IN_POINTS = 1280;
		public const int PAD_HEIGHT_IN_POINTS = 768;
		public const int PAD_WIDTH_IN_POINTS = 1024;
		public const int PHONE_HEIGHT_IN_POINTS = 320;
		public const int PHONE_WIDTH_IN_POINTS = 480;
		public const int STANDARD_KINDLE_DPI = 216;
		public const int STANDARD_DPI = 167;


		//
		// Properties
		//
		public static float Aspect {
			get { return AspectFromSize(Size); }
		}

		public static float HeightInPoints {
			get { return CalculateHeightInPoints(); }
		}

		public static bool IsKindle {
			get { return Application.platform == RuntimePlatform.Android; }
		}

		public static bool IsEditor {
			get { return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor; }
		}

		public static bool IsLandscape {
			get { return Screen.width > Screen.height; }
		}

		public static bool IsPhone {
			get { return !IsKindle && IsPhoneSize(Size); }
		}

		public static bool IsRetina {
			get { return Screen.dpi > (IsKindle ? STANDARD_KINDLE_DPI : STANDARD_DPI); }
		}

		public static float PointsToPixelsRatio {
			get {
				if (IsRetina) {

					float landscapeHeight;
					landscapeHeight = IsLandscape ? Screen.height : Screen.width;

					if (IsKindle) return landscapeHeight / (1.0f * KINDLE_HEIGHT_IN_POINTS);
					if (IsPhone) return landscapeHeight / (1.0f * PHONE_HEIGHT_IN_POINTS);
					return landscapeHeight / (1.0f * PAD_HEIGHT_IN_POINTS);
				
				}
				return 1;
			}
		}

		public static Vector2 Size {
			get { return new Vector2(Screen.width, Screen.height); }	
		}

		public static Vector2 SizeInPoints {
			get { return new Vector2(WidthInPoints, HeightInPoints); }
		}

		public static float WidthInPoints {
			get { return Aspect * HeightInPoints; }
		}


		//
		// Public Methods
		//
		public static float AspectFromSize(Vector2 size) {
			return size.x / size.y;
		}

		public static bool IsPhoneSize(Vector2 size) {

			float aspect;
			aspect = IsLandscape ? AspectFromSize(size) : 1 / AspectFromSize(size);
			
			float thresholdWidth;
			thresholdWidth = 0.5f * (PAD_WIDTH_IN_POINTS + PHONE_WIDTH_IN_POINTS);
			
			float thresholdHeight;
			thresholdHeight = 0.5f * (PAD_HEIGHT_IN_POINTS + PHONE_HEIGHT_IN_POINTS);
			
			float thresholdAspect;
			thresholdAspect = thresholdWidth / (thresholdHeight);
			
			return aspect > thresholdAspect;
		
		}


		//
		// Functions
		//
		private static float CalculateHeightInPoints() {
			if (IsEditor) {
				return (IsPhone ? PHONE_HEIGHT_IN_POINTS : PAD_HEIGHT_IN_POINTS) / (IsLandscape ? 1 : Aspect);
			}
			return Screen.height / PointsToPixelsRatio;
		}


	}

}
