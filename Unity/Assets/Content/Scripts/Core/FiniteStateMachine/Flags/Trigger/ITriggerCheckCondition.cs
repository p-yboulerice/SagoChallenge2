namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public interface ITriggerCheckCondition {

		bool CheckTrigger(Collider2D trigger);

	}
}