namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class SF_Timer : StateFlag {

		#region Fields

		[SerializeField]
		private float m_TimeDelay = 0;

		#endregion


		#region Properties

		private float TimeDelay {
			get { return m_TimeDelay; }
		}

		private float TimeWhenEnabled {
			get;
			set;
		}

		public override bool IsActive {
			get {
				return Time.time - this.TimeWhenEnabled >= this.TimeDelay;
			}
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.TimeWhenEnabled = Time.time;
		}

		#endregion

	
	}

}