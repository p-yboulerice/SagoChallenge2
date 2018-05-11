namespace SagoDebug {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SagoTouch;

	using Touch = SagoTouch.Touch;

	/// <summary>
	/// Adds a memory stats display window that is accessible
	/// via the DevUI.
	/// </summary>
	public class MemoryStatsWindow : MonoBehaviour, ISingleTouchObserver {


		#region ISingleTouchObserver Methods

		/// <summary>
		/// Prevents pass-through of touches over the DevUI.
		/// Otherwise, returns false.
		/// </summary>
		public bool OnTouchBegan(Touch touch) {
			if (this.DebugGUIClient == null || !this.DebugGUIClient.IsEnabled) {
				return false;
			} else {
				Vector2 guiTouch = GUIUtility.ScreenToGUIPoint(new Vector2(touch.Position.x, Screen.height - touch.Position.y));
				return this.WindowRect.Contains(guiTouch);
			}
		}

		public void OnTouchMoved(Touch touch) {
		}

		public void OnTouchEnded(Touch touch) {
		}

		public void OnTouchCancelled(Touch touch) {
		}

		#endregion


		#region MonoBehaviour

		void Start() {
			this.DebugGUIClient = DevUI.AddDebugOnGUI("Memory Stats", this, OnDebugGUI, true, true);
			this.WindowRect = new Rect(0f, 0f, 300f, 200f);
		}

		void OnDisable() {
			if (this.IsRegisteredForTouch && TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
				this.IsRegisteredForTouch = false;
			}
		}

		#endregion


		#region Fields

		private GUILayoutOption[] LabelOptions = { GUILayout.Width(120.0f) };
		private AsyncOperation UnloadingOp;

		#endregion


		#region Properties

		private Rect WindowRect {
			get; set;
		}

		DevUI.DebugOnGUIClient DebugGUIClient {
			get; set;
		}

		bool IsRegisteredForTouch {
			get; set;
		}

		List<Texture2D> MemoryTestBuffer {
			get; set;
		}

		#endregion


		#region Internal Methods

		void OnDebugGUI() {

			if (!this.IsRegisteredForTouch) {
				if (TouchDispatcher.Instance) {
					TouchDispatcher.Instance.Add(this, 99999);
					this.IsRegisteredForTouch = true;
				}
			}

			float scale = DevUI.Instance.Scale;
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
			this.WindowRect = GUILayout.Window(this.GetHashCode(), this.WindowRect, WindowFunc, "Memory");
			this.WindowRect = ClampToScreen(this.WindowRect, scale);
		}

		void WindowFunc(int id) {
			
			Rect buttonRect = new Rect(this.WindowRect.width - 20f, 0f, 20f, 20f);
			if (GUI.Button(buttonRect, "X", GUI.skin.label)) {
				this.DebugGUIClient.IsEnabled = false;
			}

			DrawStat("Total Allocated", UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
			DrawStat("Total Reserved", UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong());
			DrawStat("Total Unreserved", UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong());

			DrawStat("Used Heap Size", UnityEngine.Profiling.Profiler.usedHeapSizeLong);
			DrawStat("Mono Heap Size", UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong());
			DrawStat("Mono Used Size", UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong());

			DrawUnloadUnusedAssetsButton();

			DrawTestMemoryAllocationButtons();

			GUI.DragWindow();
		}

		void DrawStat(string label, long numBytes) {
			DrawStat(label, ByteFormat(numBytes));
		}

		void DrawStat(string label, string stat) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, LabelOptions);
			GUILayout.Label(stat);
			GUILayout.EndHorizontal();
		}

		void DrawTestMemoryAllocationButtons() {
			const int dim = 1024;
			GUILayout.BeginVertical(GUI.skin.box);
			DrawStat("Test Memory (approx)", (this.MemoryTestBuffer == null ? 0 : this.MemoryTestBuffer.Count * dim * dim));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Allocate")) {
				this.MemoryTestBuffer = this.MemoryTestBuffer ?? new List<Texture2D>();
				this.MemoryTestBuffer.Add(new Texture2D(dim, dim));
			}
			if (GUILayout.Button("Release")) {
				this.MemoryTestBuffer = null;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		void DrawUnloadUnusedAssetsButton() {
			bool GuiEnabled = GUI.enabled;
			GUI.enabled = UnloadingOp == null || UnloadingOp.isDone;
			if (GUILayout.Button("UnloadUnusedAssets")) {
				StartCoroutine(UnloadUnusedAssets());
			}
			GUI.enabled = GuiEnabled;
		}

		IEnumerator UnloadUnusedAssets() {
			
			UnityEngine.Debug.LogFormat("UnloadUnusedAssets: start: {0}", 
				ByteFormat(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()));
			
			UnloadingOp = Resources.UnloadUnusedAssets();
			yield return UnloadingOp;
			UnloadingOp = null;

			UnityEngine.Debug.LogFormat("UnloadUnusedAssets: complete: {0}", 
				ByteFormat(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()));
		}

		static Rect ClampToScreen(Rect windowRect, float scale) {
			Vector2 pos = windowRect.position;
			float invScale = 1.0f / scale;
			pos.x = Mathf.Clamp(pos.x, 0f, invScale * Screen.width - windowRect.width);
			pos.y = Mathf.Clamp(pos.y, 0f, invScale * Screen.height - windowRect.height);
			windowRect.position = pos;
			return windowRect;
		}

		static string ByteFormat(long numBytes) {
		    string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
		    if (numBytes == 0)
		        return "0 " + units[0];
		    int place = Convert.ToInt32(Math.Floor(Math.Log(numBytes, 1024)));
		    double num = Math.Round(numBytes / Math.Pow(1024, place), 1);
		    return string.Format("{0:0.##} {1}", Math.Sign(numBytes) * num, units[place]);
		}

		#endregion


	}
	
}
