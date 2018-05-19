namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	public interface ITouchForwarderController {

		void OnTouchBegan(Touch touch);

		void OnTouchMoved(Touch touch);

		void OnTouchEnded(Touch touch);

		void OnTouchCancelled(Touch touch);

	}

}