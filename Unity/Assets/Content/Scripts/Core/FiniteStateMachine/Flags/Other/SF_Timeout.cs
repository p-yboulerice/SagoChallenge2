namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class SF_Timeout : StateFlag {

		#region Fields

		[SerializeField]
		private float m_MinTimeDelay = 0;

		[SerializeField]
		private float m_MaxTimeDelay = 0;

		#endregion


		#region Properties

		private float MinTimeDelay {
			get { return m_MinTimeDelay; }
		}

		private float MaxTimeDelay {
			get { return m_MaxTimeDelay; }
		}

		private float TimeWhenEnabled {
			get;
			set;
		}

		public override bool IsActive {
			get {
				return Time.time - this.TimeWhenEnabled >= this.CurrentRandomDelay;
			}
		}

		private float CurrentRandomDelay {
			get;
			set;
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.TimeWhenEnabled = Time.time;
			this.CurrentRandomDelay = Random.Range(this.MinTimeDelay, this.MaxTimeDelay);
		}

		#endregion


	}

}