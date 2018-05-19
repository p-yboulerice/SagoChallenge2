namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoTouch;

	public class SF_PlayerInactive : StateFlag, ISingleTouchObserver {


		#region Fields

		[SerializeField]
		private float Delay = 1.43f;

		#endregion


		#region Properties

		public override bool IsActive {
			get {
				return (Time.time - this.TimeAtLastTouch) >= this.Delay;
			}
		}

		private float TimeAtLastTouch {
			get;
			set;
		}

		#endregion


		#region Methods

		void OnEnable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Add(this, 99999, false);
				this.TimeAtLastTouch = Time.time;
			}
		}

		void OnDisable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}

		#endregion


		#region ISingleTouchObserver implementation

		public bool OnTouchBegan(SagoTouch.Touch touch) {
			this.TimeAtLastTouch = Time.time;
			return false;
		}

		public void OnTouchMoved(SagoTouch.Touch touch) {
			this.TimeAtLastTouch = Time.time;
		}

		public void OnTouchEnded(SagoTouch.Touch touch) {
		}

		public void OnTouchCancelled(SagoTouch.Touch touch) {
		}

		#endregion


	}
}