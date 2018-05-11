namespace SagoLayout {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public interface IOverlayElement {

		OverlayType OverlayType {
			get;
		}

		void Activate();
		void Deactivate();
	}
}