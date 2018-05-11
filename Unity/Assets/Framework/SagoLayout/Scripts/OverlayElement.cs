namespace SagoLayout {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class OverlayElement : MonoBehaviour, IOverlayElement {

		#region Serialized Fields

		[SerializeField]
		private OverlayType m_OverlayType;

		#endregion


		#region Public Properties

		public OverlayType OverlayType {
			get {
				return m_OverlayType;
			}
		}

		#endregion


		#region Public Methods

		public void Activate() {
			gameObject.SetActive(true);
		}

		public void Deactivate() {
			gameObject.SetActive(false);
		}

		#endregion
	}
}