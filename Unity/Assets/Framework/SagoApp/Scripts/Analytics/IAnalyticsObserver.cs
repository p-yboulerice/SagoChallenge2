namespace SagoApp.Analytics {
	
	using System.Collections.Generic;

	public interface IAnalyticsObserver {

		int Priority { get; }

		void OnTrackEvent(string eventName, Dictionary<string,object> eventInfo);

	}
}
