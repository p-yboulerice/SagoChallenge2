namespace SagoBiz {

	using UnityEngine;
	using System.Runtime.InteropServices;

	public class AppStoreReviewRequest {

		#region Native binding

		#if SAGO_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		private static extern void _RequestReview();

		#endif

		#endregion


		#region Public methods

		public static void RequestReview() {
			#if SAGO_IOS && !UNITY_EDITOR

			_RequestReview();

			#endif
		}

		#endregion

	}

}
