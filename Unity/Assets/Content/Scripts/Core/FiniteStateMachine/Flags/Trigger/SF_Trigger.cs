namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Juice.Utils;

	public class SF_Trigger : StateFlag, ITriggerController {
		

		#region Fields

		[System.NonSerialized]
		private List<ITriggerCheckCondition> m_TriggerCheckConditions;

		#endregion

		#region Properties

		public override bool IsActive {
			get {
				return this.TriggerAgainstGameObject != null;
			}
		}

		private List<ITriggerCheckCondition> TriggerCheckConditions {
			get { return m_TriggerCheckConditions = m_TriggerCheckConditions ?? new List<ITriggerCheckCondition>(this.GetComponents<ITriggerCheckCondition>()); }
		}

		public GameObject TriggerAgainstGameObject {
			get;
			private set;
		}

		#endregion


		#region Methods


		#endregion


		#region ITriggerController implementation

		public void TriggerEnter2D(Collider2D collider, Collider2D colliderAgainst) {
			this.CheckTrigger(colliderAgainst);	
		}

		private void CheckTrigger(Collider2D colliderAgainst) {
			if (this.TriggerAgainstGameObject == null) {
				
				bool rightTrigger = true;

				foreach (ITriggerCheckCondition c in this.TriggerCheckConditions) {
					if (!c.CheckTrigger(colliderAgainst)) {
						rightTrigger = false;
						break;
					}
				}

				if (rightTrigger) {
					this.TriggerAgainstGameObject = colliderAgainst.gameObject;
				}
			}
		}

		public void TriggerStay2D(Collider2D collider, Collider2D colliderAgainst) {
		}

		public void TriggerExit2D(Collider2D collider, Collider2D colliderAgainst) {
			if (this.TriggerAgainstGameObject != null && colliderAgainst!=null && this.TriggerAgainstGameObject == colliderAgainst.gameObject) {
				this.TriggerAgainstGameObject = null;
			}
		}

		#endregion


	}
}