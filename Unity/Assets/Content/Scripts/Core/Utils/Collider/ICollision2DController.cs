namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public interface ICollision2DController {
		void OnCollision2DEnter(Collision2D collision);
		void OnCollision2DStay(Collision2D collision);
		void OnCollision2DExit(Collision2D collision);
		void Terminate();

	}
}