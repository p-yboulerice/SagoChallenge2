namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public interface ITriggerController {
		
		void TriggerEnter2D(Collider2D collider, Collider2D colliderAgainst);
		void TriggerStay2D(Collider2D collider, Collider2D colliderAgainst);
		void TriggerExit2D(Collider2D collider, Collider2D colliderAgainst);

	}

}