namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoNavigation;

	public class SF_SceneIsLoaded : StateFlag {

		public override bool IsActive {
			get { return !SceneNavigator.Instance.IsBusy; }
		}


	}
}