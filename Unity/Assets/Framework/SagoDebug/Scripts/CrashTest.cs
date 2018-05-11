namespace SagoDebug {

	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class CrashTest : MonoBehaviour {


		#region Public Methods

		public static void AddCrashTestItem(string name, Action crashMethod) {

			#if SAGO_DEBUG

			if (!string.IsNullOrEmpty(name) && crashMethod != null) {
				CustomCrashes[name] = new CrashTestItem(name, crashMethod);
			}

			#endif

		}

		public static void Create() {
			
			#if SAGO_DEBUG

			GameObject go = new GameObject("CrashTest");
			DontDestroyOnLoad(go);
			go.AddComponent<CrashTest>();

			#endif

		}

		#endregion


		#region Internals

		#if SAGO_DEBUG

		private struct CrashTestItem {
			public string Name;
			public Action CrashMethod;

			public CrashTestItem(string name, Action crashMethod) {
				this.Name = name;
				this.CrashMethod = crashMethod;
			}
		}


		private static Dictionary<string, CrashTestItem> CustomCrashes = new Dictionary<string, CrashTestItem>();


		void Start() {
			DevUI.AddPage("Crash Test", this, OnDrawDevUIPage);
		}

		void OnDrawDevUIPage(DevUI.DevUIPage page) {

			GUILayoutOption[] opts = { GUILayout.Width(300f) };

			if (GUILayout.Button("Application Exception", opts)) {
				throw new ApplicationException();
			}
			if (GUILayout.Button("Null Reference", opts)) {
				GameObject go = null;
				UnityEngine.Debug.Log(go.name);
			}
			if (GUILayout.Button("Float Divide By Zero", opts)) {
				float x = 3.14f;
				float y = 0.0f;
				float z = x / y;
				UnityEngine.Debug.Log(z.ToString());
			}
			if (GUILayout.Button("Integer Divide By Zero", opts)) {
				int x = 42;
				int y = 0;
				int z = x / y;
				UnityEngine.Debug.Log(z.ToString());
			}
			if (GUILayout.Button("Stack Overflow", opts)) {
				OverflowStack(1, 2, 3);
			}
			if (GUILayout.Button("Duplicates in Dictionary", opts)) {
				var dict = new Dictionary<string, string>();
				dict.Add("key1", "value1");
				dict.Add("key1", "value1");
			}
			if (GUILayout.Button("Out of Range in Array", opts)) {
				string[] collectionOfStrings = new string[3];
				UnityEngine.Debug.Log(collectionOfStrings[10]);
			}
			foreach (var item in CustomCrashes.Values) {
				if (GUILayout.Button(item.Name, opts)) {
					if (item.CrashMethod != null) {
						item.CrashMethod();
					}
				}
			}
		}

		private int OverflowStack(int a,int b,int c){
			return OverflowStack(c, b, a) + OverflowStack(b, c, a + 1);
		}

		#endif

		#endregion


	}
}