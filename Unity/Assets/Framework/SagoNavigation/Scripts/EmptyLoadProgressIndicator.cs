namespace SagoNavigation {

	using UnityEngine;

	public class EmptyLoadProgressIndicator : LoadingIndicator {
		
		
		//
		// Factory
		//
		static public EmptyLoadProgressIndicator Create() {
			return new GameObject("EmptyLoadProgressIndicator").AddComponent<EmptyLoadProgressIndicator>();
		}


	}
}
