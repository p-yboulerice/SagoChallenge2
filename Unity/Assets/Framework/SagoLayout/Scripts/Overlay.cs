namespace SagoLayout {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public enum OverlayType {
		None,
		iPhoneX_Notch_Left,
		iPhoneX_Notch_Left_SafeArea,
		iPhoneX_Notch_Right,
		iPhoneX_Notch_Right_SafeArea,
	}

	public class Overlay : MonoBehaviour {

		#region Serialized Fields

		[SerializeField]
		private OverlayType m_ActiveOverlay;

		#endregion


		#region Public Properties

		public OverlayType ActiveOverlay {
			get {
				return m_ActiveOverlay;
			}
		}

		#endregion


		#region Private Methods

		public void OnOverlayTypeChanged() {
				
			foreach (IOverlayElement element in GetComponentsInChildren<IOverlayElement>(true)) {
				if (element.OverlayType == this.ActiveOverlay) {
					element.Activate();
				} else {
					element.Deactivate();
				}
			}
		}

		#endregion
	}
}