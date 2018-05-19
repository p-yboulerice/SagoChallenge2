namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;

	public class SF_IsNextFrame : StateFlag {

		#region Fields

		#endregion


		#region Properties

		private bool IsNextFrame {
			get;
			set;
		}

		public override bool IsActive {
			get {
				return this.IsNextFrame;
			}
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.IsNextFrame = false;
		}

		void Update() {
			this.IsNextFrame = true;
		}

		#endregion

	
	}

}