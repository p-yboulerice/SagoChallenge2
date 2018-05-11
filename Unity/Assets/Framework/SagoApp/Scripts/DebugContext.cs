namespace SagoApp {
	
	using SagoDebug;
	using UnityEngine;
	
	public static class DebugContext {
		
		[RuntimeInitializeOnLoadMethod]
		private static void Init() {
			SagoApp.hideFlags = HideFlags.HideAndDontSave;
		}
		
		private static Object _SagoApp;
		
		public static Object SagoApp {
			get { return _SagoApp = _SagoApp ?? DevUI.Instance.AddConsoleMessageGroup("SagoApp"); }
		}
		
	}
	
}