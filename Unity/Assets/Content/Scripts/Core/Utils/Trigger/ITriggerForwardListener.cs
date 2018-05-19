namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public interface ITriggerForwardListener {

		void TriggerEnter2D(Collider2D collider);
		void TriggerStay2D(Collider2D collider);
		void TriggerExit2D(Collider2D collider);
		void Terminate();
	
	}
}