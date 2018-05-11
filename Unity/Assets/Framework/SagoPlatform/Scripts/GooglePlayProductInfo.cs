namespace SagoPlatform {

	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using SagoPlatform;

	public class GooglePlayProductInfo : AndroidProductInfo {

		#region Serialized Fields

		[HideInInspector]
		[SerializeField]
		public string LicensePublicKey;

		[HideInInspector]
		[SerializeField]
		public bool GooglePlayDownloaderDebugMode;

		#endregion
	}
	
}