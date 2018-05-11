namespace SagoDebug {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Linq;
	using SagoLayout;
	using SagoTouch;
	using SagoUtils;
	using Touch = SagoTouch.Touch;

	/// <summary>
	/// <para>Developer UI</para>
	/// <para>Typical usage: <see cref="DevUI.AddDebugOnGUI()"/> and <see cref="DevUI.AddPage()"/>.</para>
	/// <remarks><para>Requires Resources/DevUISkin</para></remarks>
	/// </summary>
	public class DevUI : MonoBehaviour, ISingleTouchObserver, ILogHandler {
		

		#region Types
		
		/// Function to be called as your replacement to OnGUI()
		public delegate void DebugOnGUI();
		
		/// Function to be called when your page is being displayed in the DevUI
		public delegate void DrawPage(DevUIPage page);
		
		/// Function to be called after your page is rendered in the DevUI so you can do popups
		public delegate void PostDrawPage(DevUIPage page);

		/// Internal state of the DevUI
		public enum DevUIState {
			Hidden,
			Button,
			Visible,
			Minimized
		}
		
		/// <summary>
		/// Holds info for drawing different pages on the DevUI.
		/// </summary>
		public class DevUIPage {
			/// <summary>
			/// Gets or sets the context object.  When the context object is destroyed the page will be removed.
			/// </summary>
			/// <value>The context object.</value>
			public UnityEngine.Object ContextObject { get; set; }
			
			/// <summary>
			/// Gets or sets the context GameObjects.  After adding a page you can continue to modify the list.
			/// Note that these are not UnityEngine.Objects so we can watch the same list with complicating things with generics.
			/// </summary>
			/// <value>The context objects.</value>
			public List<GameObject> ContextGroup { get; set; }
			
			/// <summary>
			/// Gets or sets the name that appears on the page popup.
			/// </summary>
			/// <value>The name.</value>
			public string Name {
				get {
					return this.MenuContent.text;
				}
				set {
					this.MenuContent.text = value;
				}
			}
			
			/// <summary>
			/// Used in the popup menus.  Can be modified directly or via Name.
			/// </summary>
			/// <value>The content of the menu.</value>
			public GUIContent MenuContent {
				get;
				protected set;
			}
			
			/// <summary>
			/// Gets or sets the draw callback.  This is called when your page is active, inside a scrollview.
			/// </summary>
			/// <value>The draw callback.</value>
			public DrawPage OnPageDraw { get; protected set; }
			
			/// <summary>
			/// Gets or sets the post draw callback.  This is called when your page is active, after the scrollview is complete.
			/// </summary>
			/// <value>The post draw callback.</value>
			public PostDrawPage OnPostPageDraw { get; protected set; }
			
			/// <summary>
			/// Gets or sets the scroll value of the scrollview that your page is drawn inside.
			/// </summary>
			/// <value>The scroller.</value>
			public Vector2 Scroller { get; set; }
			
			/// <summary>
			/// The last drawn rectangle for the page/scrollview.
			/// </summary>
			public Rect LastRect;
			
			/// <summary>
			/// Gets a value indicating whether this instance is invalid.
			/// </summary>
			/// <value><c>true</c> if this instance is invalid; otherwise, <c>false</c>.</value>
			public bool IsInvalid {
				get {
					return (this.OnPageDraw == null || (this.ContextObject == null && this.ContextGroup == null));
				}
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="SagoDebug.DevUI+DevUIPage"/> class.
			/// </summary>
			/// <param name="pageName">Page name appears on the page selector popup</param>
			/// <param name="context">When the context object is destroyed, the page will be removed</param>
			/// <param name="drawCallback">Main function to be called when your page is being displayed in the DevUI.</param>
			/// <param name="postDrawCallback">(Optional) Function to be called when your page is being displayed in the DevUI, after/outside the scrollview.</param>
			public DevUIPage(string pageName, UnityEngine.Object context, DrawPage drawCallback, PostDrawPage postDrawCallback = null) {
				this.ContextObject = context;
				this.MenuContent = new GUIContent(pageName);
				this.OnPageDraw = drawCallback;
				this.OnPostPageDraw = postDrawCallback;
			}
			
			public DevUIPage(string pageName, List<GameObject> contextGroup, DrawPage drawCallback, PostDrawPage postDrawCallback = null) {
				this.ContextGroup = contextGroup;
				this.MenuContent = new GUIContent(pageName);
				this.OnPageDraw = drawCallback;
				this.OnPostPageDraw = postDrawCallback;
			}
		}
		
		/// Hold the info regarding external OnGUI callbacks
		public class DebugOnGUIClient {
			
			/// The content of the toggle on the External GUI button list
			public GUIContent ToggleContent { get; protected set; }
			
			/// The callback for the OnGUI, called when this IsEnabled
			public DebugOnGUI OnDebugGUI { get; set; }
			
			/// Whether or not this DebugOnGUIClient is drawn.
			public bool IsEnabled { get; set; }
			
			/// When the ContextObject is destroyed/null this DebugOnGUIClient will be removed.
			public UnityEngine.Object ContextObject { get; set; }
			
			/// The name displayed on the toggle button on the External GUI button list
			public string Name {
				get{ return this.ToggleContent.text; } 
				set{ this.ToggleContent.text = value; }
			}
			
			public DebugOnGUIClient(string name, UnityEngine.Object context, DebugOnGUI onDebugGUI) {
				this.ContextObject = context;
				this.ToggleContent = new GUIContent(name);
				this.OnDebugGUI = onDebugGUI;
			}
		}
		
		/// Holds info around the Debug.Log console messages
		public struct ConsoleMessage {
			public string Message {
				get {
					return this.Content.text;
				}
				set {
					this.Content.text = value;
				}
			}
			public GUIContent Content { get; private set; }
			public LogType LogType { get; set; }
			public UnityEngine.Object Context {
				get {
					return m_ContextWeak.Target as UnityEngine.Object;
				}
				set {
					m_ContextWeak = new System.WeakReference(value, false);
				}
			}
			private System.WeakReference m_ContextWeak;
			
			public float Time { get; private set; }

			public ConsoleMessage(LogType logType, string message, UnityEngine.Object context) {
				m_ContextWeak = null;
				this.Content = new GUIContent();
				this.LogType = logType;
				this.Time = UnityEngine.Time.time;
				this.Message = message;
				this.Context = context;
			}

			public float Age {
				get { return UnityEngine.Time.time - this.Time; }
			}
		}

		/// <summary>
		/// For grouping console messages together based on their context object
		/// and then persisting the selected configuration to PlayerPrefs.
		/// </summary>
		[System.Serializable]
		private class ConsoleMessageGroup {
			
			[SerializeField]
			private string m_Name;

			[SerializeField]
			private ConsoleMessageLevel m_ToastLevel;

			[SerializeField]
			private int m_Order;

			[System.NonSerialized]
			private List<System.WeakReference> m_Contexts;
			

			public string Name {
				get { return m_Name; }
				set { m_Name = value; }
			}

			public ConsoleMessageLevel ToastLevel {
				get { return m_ToastLevel; }
				set { m_ToastLevel = value; }
			}

			public int Order {
				get { return m_Order; }
				set { m_Order = value; }
			}


			public ConsoleMessageGroup() {
				m_Contexts = new List<System.WeakReference>();
			}

			public ConsoleMessageGroup(string name) : this() {
				this.Name = name;
			}

			public void AddContext(UnityEngine.Object context) {
				System.WeakReference contextRef = new System.WeakReference(context, false);
				m_Contexts.Add(contextRef);
			}

			public bool IncludesContext(UnityEngine.Object context) {
				bool result = false;
				for (int i = m_Contexts.Count - 1; i >= 0; --i) {
					UnityEngine.Object ob = m_Contexts[i].Target as UnityEngine.Object;
					if (!ob) {
						m_Contexts.RemoveAt(i);
						continue;
					}

					if (ob == context) {
						result = true;
					}
				}
				return result;
			}

		}

		/// Serializable container for creating JSON for console message groups.
		[System.Serializable]
		private struct SerializableConsoleMessageGroups {
			[SerializeField]
			public List<ConsoleMessageGroup> Groups;
		}

		/// Internal enum for filtering which messages to show
		private enum ConsoleMessageLevel {
			None,
			Exceptions,
			Errors,
			Warnings,
			Messages
		}
		
		/// Internal enum for docking state of window
		private enum WindowLocation {
			Top,
			Bottom,
			Left,
			Right
		}
		
		#endregion
		
		
		#region Serialized Fields

		[Tooltip("The GUISkin to use when displaying the DevUI")]
		[SerializeField]
		protected GUISkin m_skin;
		
		[Tooltip("The most console messages to keep in memory")]
		[SerializeField]
		protected int m_maxConsoleMessages = 500;
		
		#endregion
		

		#region ILogHandler implementation

		/// <summary>
		/// Implementation of ILogHandler.  Logs to the DevUI,
		/// and forwards it on to the default handler as well.
		/// </summary>
		/// <param name="logType">Log type.</param>
		/// <param name="context">Context.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void LogFormat(LogType logType, Object context, string format, params object[] args) {
			string message = string.Format(format, args).Trim();
			AddToConsole(logType, message, context);
			if (m_UnityDefaultLogHandler != null) {
				m_UnityDefaultLogHandler.LogFormat(logType, context, format, args);
			}
		}

		/// <summary>
		/// Implementation of ILogHandler.  Logs to the DevUI,
		/// and forwards it on to the default handler as well.
		/// </summary>
		/// <param name="exception">Exception.</param>
		/// <param name="context">Context.</param>
		public void LogException(System.Exception exception, Object context) {
			AddToConsole(LogType.Exception, exception.ToString(), context);
			if (m_UnityDefaultLogHandler != null) {
				m_UnityDefaultLogHandler.LogException(exception, context);
			}
		}

		#endregion


		#region Public Properties
		
		/// <summary>
		/// The most recently enabled instance of DevUI.  Is null if none exists.
		/// </summary>
		public static DevUI Current {
			get;
			protected set;
		}
		
		/// <summary>
		/// This will create a DevUI instance if none exists yet.
		/// Note: An instance will be created even if SAGO_DEBUG is off.
		/// </summary>
		/// <value>The instance.</value>
		public static DevUI Instance {
			get {
				if (Current == null) {
					GameObject go = new GameObject("DevUI");
					Current = go.AddComponent<DevUI>();
					DontDestroyOnLoad(go);
				}
				return Current;
			}
		}
		
		/// <summary>
		/// If this build was created in CloudBuild, this will contain the information it supplies.
		/// </summary>
		/// <value>The cloud build info.</value>
		public string CloudBuildInfo {
			get;
			protected set;
		}

		public ConsoleMessage[] ConsoleMessagesCopy {
			get {
				return this.ConsoleMessages.ToArray();
			}
		}

		/// <summary>
		/// Gets or sets the maximum number console messages to keep in memory.
		/// </summary>
		/// <value>The max console message count.</value>
		public int MaxConsoleMessages {
			get { return m_maxConsoleMessages; }
			protected set { m_maxConsoleMessages = value; }
		}
		
		/// <summary>
		/// Serialized field access.
		/// </summary>
		/// <value>The skin.</value>
		public GUISkin Skin {
			get { return m_skin; }
			protected set { m_skin = value; }
		}
		
		/// <summary>
		/// Gets or sets the state of DevUI.
		/// </summary>
		/// <value>The state.</value>
		virtual public DevUIState State {
			get { return m_State; }
			protected set {
				DevUIState oldState = m_State;
				DevUIState newState = value;
				if (oldState != newState) {
					this.OnStateWillChange(oldState, newState);
					m_State = value;
					this.StateStartTime = Time.time;
					this.OnStateDidChange(oldState, newState);
				}
			}
		}
		
		virtual public float StateStartTime {
			get;
			protected set;
		}
		
		virtual public float StateTime {
			get {
				return Time.time - this.StateStartTime;
			}
		}
		
		/// <summary>
		/// Cached, delayed initializer for Transform component
		/// </summary>
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		#endregion
		

		#region Public Methods
		
		/// <summary>
		/// Add a custom OnGUI that can be toggled on and off in the DevUI.  Rename your OnGUI (so
		/// it isn't automatically called by Unity), and pass that renamed function to this method.
		/// <para>Note: has no effect if SAGO_DEBUG is not set, unless availableInRelease is set.</para>
		/// </summary>
		/// <param name="name">How this will appear in the DevUI.</param>
		/// <param name="context">When the context object is destroyed, the item will be removed (e.g. pass 'this').</param>
		/// <param name="onDebugGUI">The OnGUI function that will be called.  Make sure you rename your OnGUI or it will always be called by Unity.</param>
		/// <param name="silent">Whether or not to print a message to the log when this is added.</param>
		public static DebugOnGUIClient AddDebugOnGUI(string name, UnityEngine.Object context, DebugOnGUI onDebugGUI, bool silent = false, bool availableInRelease = false) {
		
			bool add = availableInRelease;

			#if SAGO_DEBUG
			add = true;
			#endif

			if (Instance && add) {
				DebugOnGUIClient client = new DebugOnGUIClient(name, context, onDebugGUI);
				Instance.AddDebugClient(client, silent);
				return client;
			}

			return null;
		}
		
		/// <summary>
		/// <para>Adds a new page to the DevUI.  Your draw function will be called when the page is selected in the DevUI page popup.</para>
		/// <para>Note: has no effect if SAGO_DEBUG is not set, unless availableInRelease is set.</para>
		/// </summary>
		/// <param name="pageName">Page name.</param>
		/// <param name="context">Context.</param>
		/// <param name="onDrawPage">Called when your page is the active page in the DevUI, from inside the scrollview.</param>
		/// <param name="onPostDrawPage">(Optional) Called when your page is the active page in the DevUI, outside the scrollview.</param>
		/// <param name="silent">Whether or not to print a message to the log when this is added.</param>
		public static DevUIPage AddPage(string pageName, UnityEngine.Object context, DrawPage onDrawPage, PostDrawPage onPostDrawPage = null, bool silent = false, bool availableInRelease = false) {

			bool add = availableInRelease;

			#if SAGO_DEBUG
			add = true;
			#endif

			if (Instance && add) {
				DevUIPage page = new DevUIPage(pageName, context, onDrawPage, onPostDrawPage);
				Instance.AddPage(page, silent);
				return page;
			}

			return null;
		}
		
		/// <summary>
		/// <para>Adds a new page to the DevUI for a group of objects.  Your draw function will be called when the page is selected in the DevUI page popup.</para>
		/// <para>Note: has no effect if SAGO_DEBUG is not set, unless availableInRelease is set.</para>
		/// </summary>
		/// <param name="pageName">Page name.</param>
		/// <param name="contexts">List of all the instances that are involved in this page</param>
		/// <param name="onDrawPage">Called when your page is the active page in the DevUI, from inside the scrollview.</param>
		/// <param name="onPostDrawPage">(Optional) Called when your page is the active page in the DevUI, outside the scrollview.</param>
		/// <param name="silent">Whether or not to print a message to the log when this is added.</param>
		public static DevUIPage AddGroupPage(string pageName, List<GameObject> contexts, DrawPage onDrawPage, PostDrawPage onPostDrawPage = null, bool silent = false, bool availableInRelease = false) {

			bool add = availableInRelease;

			#if SAGO_DEBUG
			add = true;
			#endif

			if (Instance && add) {
				DevUIPage page = new DevUIPage(pageName, contexts, onDrawPage, onPostDrawPage);
				Instance.AddPage(page, silent);
				return page;
			}

			return null;
		}
		
		/// <summary>
		/// A draw routine that can be passed to <see cref="AddGroupPage()"/> as the onDrawPage parameter 
		/// that displays general GameObject info, if you don't want to make your own, or as an example.
		/// </summary>
		/// <param name="page">Page.</param>
		public static void DefaultDrawGroupPage(DevUI.DevUIPage page) {
			
			if (page.ContextGroup == null) {
				
				GUILayout.Label("Invalid page group; list of objects destroyed.");
				
			} else if (page.ContextGroup.Count == 0) {
			
				GUILayout.Label(string.Format("No objects in page group.", page.ContextGroup.Count));
				
			} else {
				
				GUILayoutOption[] columnWidths = { GUILayout.Width(32), GUILayout.MinWidth(250), GUILayout.Width(200), GUILayout.Width(120) };
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("On", columnWidths[0]);
				GUILayout.Label(string.Format("Name ({0} total objects)", page.ContextGroup.Count), columnWidths[1]);
				GUILayout.Label("Position", columnWidths[2]);
				GUILayout.Label("Layer", columnWidths[3]);
				GUILayout.EndHorizontal();
				
				Color originalColor = GUI.color;
				
				int idx = 0;
				foreach(GameObject go in page.ContextGroup) {
					
					GUI.color = go.activeInHierarchy ? Color.white : Color.grey;
					
					GUILayout.BeginHorizontal();
					
					bool active = GUILayout.Toggle(go.activeSelf, GUIContent.none, columnWidths[0]);
					if (active != go.activeSelf) {
						go.SetActive(active);
					}
					GUILayout.Label(go.name, columnWidths[1]);
					GUILayout.Label(go.transform.position.ToString("F2"), columnWidths[2]);
					GUILayout.Label(LayerMask.LayerToName(go.layer), columnWidths[3]);
					
					GUILayout.EndHorizontal();
					GUI.color = Color.white;
					
					bool isTarget = Instance.DebugCameraTarget && Instance.DebugCameraTarget == go.transform;
					bool toggle = GUI.Toggle (GUILayoutUtility.GetLastRect(), isTarget, GUIContent.none, Instance.ConsoleMessageStyles[0]);
					if (toggle != isTarget) {
						if (isTarget) {
							Instance.DebugCameraTarget = null;
						} else {
							Instance.DebugCameraTarget = go.transform;
						}
					}
					
					idx++;
				}
				
				GUI.color = originalColor;
			}
			
		}
		
		/// <summary>
		/// <para>Log a message to the DevUI console.  This will
		/// automatically be called when logging to the regular console (Debug.Log).</para>
		/// </summary>
		/// <param name="logLevel">Log level.</param>
		/// <param name="message">Message.</param>
		/// <param name="context">Context.</param>
		public void AddToConsole(LogType logLevel, string message, UnityEngine.Object context) {
			
			ConsoleMessage cm = new ConsoleMessage(logLevel, message, context);
			this.ConsoleMessages.Enqueue(cm);

			if (IsToastable(cm)) {
				if (this.ToastMessages == null) {
					this.ToastMessages = new List<ConsoleMessage>();
				}
				this.ToastMessages.Add(cm);
			}

			while (this.ConsoleMessages.Count > this.MaxConsoleMessages) {
				this.ConsoleMessages.Dequeue();
			}
			
		}
		
		/// <summary>
		/// <para>Make the debug camera look at/follow the target transform.  To disable
		/// the camera, LookAt(null).</para>
		/// </summary>
		/// <param name="targetTransform">Target transform.</param>
		public static void LookAt(Transform targetTransform) {
			if (Instance) {
				Instance.DebugCameraTarget = targetTransform;
			}
		}

		/// <summary>
		/// Make the DevUI visible.
		/// </summary>
		public void Show() {
			this.State = DevUIState.Visible;
		}

		/// <summary>
		/// Hide the DevUI.
		/// </summary>
		public void Hide() {
			this.State = DevUIState.Hidden;
		}

		/// <summary>
		/// Add a group for console messages, and assign a context object.
		/// All Debug.Log messages that use the given context object will be considered
		/// to be in this group.  A popup and with message level filters
		/// will be shown in Settings, and its value will be saved to PlayerPrefs.
		/// </summary>
		/// <param name="name">The group's name/ID.</param>
		/// <param name="context">A context object to associate with this group.</param>
		/// <param name="order">(Optional) Sort order.</param>
		public void AddConsoleMessageGroup(string name, UnityEngine.Object context, int order = 0) {

			ConsoleMessageGroup cat = this.ConsoleMessageGroups.FirstOrDefault(x => x.Name == name);
			if (cat == null) {
				cat = new ConsoleMessageGroup(name);
				cat.Order = order;
				AddConsoleMessageGroup(cat);
				SaveConsoleMessageGroups();
			}

			if (context && this.ConsoleMessageGroups[0].Name != name) {  // never add context for Unassigned/null group
				cat.AddContext(context);
			}

		}

		/// <summary>
		/// Add a group for console messages, and create a context object.
		/// The returned context object can be used for subsequent Debug.Log
		/// calls.
		/// </summary>
		/// <returns>The console message group.</returns>
		/// <param name="name">Name.</param>
		/// <param name="order">Order.</param>
		public UnityEngine.Object AddConsoleMessageGroup(string name, int order = 0) {
			
			GameObject go = new GameObject(string.Format("AutoGroupContext-{0}", name));
			go.transform.parent = this.transform;

			AddConsoleMessageGroup(name, go, order);

			return go;
		}

		/// <summary>
		/// Logs the given JSON text to the console.
		/// </summary>
		/// <param name="logType">Log type.</param>
		/// <param name="context">Context.</param>
		/// <param name="format">Format.</param>
		/// <param name="json">Json.</param>
		public void LogJson(LogType logType, UnityEngine.Object context, string format, string json) {
			string msg = json.Replace("{}", "{ }").Replace("{", "{{").Replace("}", "}}");
			LogFormat(logType, context, format, msg);
		}

		#endregion

		
		#region ISingleTouchObserver Methods
		
		/// <summary>
		/// Prevents pass-through of touches over the DevUI.
		/// Otherwise, returns false.
		/// </summary>
		/// <param name="touch">Touch.</param>
		public bool OnTouchBegan(Touch touch) {

			if (this.State == DevUIState.Hidden) {
				return false;
			}

			Vector2 guiTouch = new Vector2(touch.Position.x, Screen.height - touch.Position.y);

			if (this.State == DevUIState.Button) {
				
				return this.ButtonRectScreenspace.Contains(guiTouch);
				
			} else if (this.State == DevUIState.Visible) {
				
				return this.WindowRectScreenspace.Contains(guiTouch);
				
			} else if (this.State == DevUIState.Minimized) {
				
				return this.WindowRectMinimizedScreenspace.Contains(guiTouch);
				
			}
			
			return false;
		}
		
		/// <summary>
		/// No-op
		/// </summary>
		/// <param name="touch">Touch.</param>
		public void OnTouchMoved(Touch touch) {
		}
		
		/// <summary>
		/// No-op
		/// </summary>
		/// <param name="touch">Touch.</param>
		public void OnTouchEnded(Touch touch) {
		}
		
		/// <summary>
		/// No-op
		/// </summary>
		/// <param name="touch">Touch.</param>
		public void OnTouchCancelled(Touch touch) {
		}
		
		#endregion


		#region MonoBehaviour
		
		/// <summary>
		/// Reset this instance to reasonable default values.
		/// </summary>
		virtual public void Reset() {
			m_maxConsoleMessages = 500;
		}

		virtual protected void Awake() {

			#if !UNITY_EDITOR || SAGO_DEBUG_HOOK_LOG_IN_EDITOR
			m_UnityDefaultLogHandler = UnityEngine.Debug.unityLogger.logHandler;
			UnityEngine.Debug.unityLogger.logHandler = this;
			#endif

			Current = this;
			if (this.Skin == null) {
				this.Skin = Resources.Load<GUISkin>("DevUISkin");
			}
			this.Pages = new List<DevUIPage>();
			this.Clients = new List<DebugOnGUIClient>();
			this.ConsoleMessages = new Queue<ConsoleMessage>();
			this.ConsoleAutoScroll = true;
			this.AddPage(new DevUIPage("Settings", this, DrawDevUISettingsPage, DrawDevUISettingsPagePost), true);
			this.AddPage(new DevUIPage("External", this, DrawDevUIClientsPage), true);
			this.AddPage(new DevUIPage("Console", this, DrawDevUIConsolePage), true);
			this.FirstRemovablePageIndex = this.Pages.Count;
			#if SAGO_DEBUG
			this.RequiredButtonDownTime = 2.5f;
			#else
			this.RequiredButtonDownTime = 10f;
			#endif
			
			string[] enumNames = System.Enum.GetNames(typeof(LogType));
			this.ConsoleMessageStyles = new GUIStyle[enumNames.Length];
			for (int i = 0; i < enumNames.Length; ++i) {
				GUIStyle style = this.Skin.FindStyle(string.Format("Console{0}", enumNames[i]));
				style = style ?? this.Skin.label;
				this.ConsoleMessageStyles[i] = style;
			}

			LoadConsoleMessageGroups();
			AddConsoleMessageGroup("Ungrouped", null, int.MinValue);
			AddConsoleMessageGroup("DevUI", this, int.MinValue + 1);

		}

		virtual protected void Start() {
			gameObject.AddComponent<MemoryStatsWindow>();
			InitializeCloudBuildInfo();

			this.IsFullScreen = ScreenUtility.IsPhone;

			if (BugReporter.Instance) {
				BugReporter.Instance.OnCreateBugReport += this.OnCreateBugReport;
			}
		}
		
		virtual protected void OnDestroy() {
			if (Current == this) {
				Current = null;

				if (m_UnityDefaultLogHandler != null) {
					UnityEngine.Debug.unityLogger.logHandler = m_UnityDefaultLogHandler;
				}
			}

			if (BugReporter.Instance) {
				BugReporter.Instance.OnCreateBugReport -= this.OnCreateBugReport;
			}
		}
		
		virtual protected void OnEnable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Add(this, 10000);
			}
		}
		
		virtual protected void OnDisable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}

		protected void Update() {
			if (this.DebugCameraTarget) {
				this.DebugCamera.enabled = true;
				Vector3 targetPosition = this.DebugCameraTarget.position + new Vector3(0.0f, 0.0f, -5.0f);
				this.DebugCameraTransform.position = Vector3.SmoothDamp(this.DebugCameraTransform.position, targetPosition, ref m_DebugCameraVelocity, 0.25f );
			} else {
				this.DebugCamera.enabled = false;
			}
		}
		
		protected void OnGUI() {
			
			// external OnGUIs get first crack at the can, regardless of whether or not I am drawing
			for (int i = this.Clients.Count - 1; i >= 0; --i) {
				DebugOnGUIClient client = this.Clients[i];
				if (client.ContextObject == null || client.OnDebugGUI == null) {
					RemoveClient(i);
				} else if (client.IsEnabled) {
					client.OnDebugGUI();
				}
			}
			
			if (Event.current.type == EventType.Layout) {
				for (int i = this.Pages.Count - 1; i >= this.FirstRemovablePageIndex; --i) {
					DevUIPage page = this.Pages[i];
					if (page == null || page.IsInvalid) {
						RemovePage(i);
					}
				}
			}

			if (this.Skin != null) {
				GUI.skin = this.Skin;
			}
			
			float scale = this.Scale;
			float invScale = 1.0f / scale;
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));

			DrawToastWindow();

			float w = Mathf.Clamp(Screen.height * invScale * 0.125f, 48.0f, 200.0f);
			this.ButtonRect = new Rect(0.0f, 0.0f, w, w);
			this.WindowRect = GetBaseWindowRect(this.WindowDockLocation, scale, this.IsFullScreen);
			
			switch (State) {
				
			case DevUIState.Hidden:
				if (GUI.RepeatButton(this.ButtonRect, GUIContent.none, GUIStyle.none)) {
					this.ButtonDownThisFrame = true;
				}
				
				if (Event.current.type == EventType.Repaint) {
					if (this.ButtonDownThisFrame) {
						this.ButtonDownThisFrame = false;
						this.ButtonDownTime += Time.deltaTime;
						if (this.ButtonDownTime > this.RequiredButtonDownTime) {
							this.State = DevUIState.Button;
						}
					} else {
						this.ButtonDownTime = 0f;
					}
				}
				
				break;
				
			case DevUIState.Button:
				
				if (GUI.Button(ButtonRect, ":-)")) {
					Show();
				}
				
				if (this.StateTime > 3.0f) {
					Hide();
				}
				
				break;
				
			case DevUIState.Visible:
				
				GUILayout.BeginArea(this.WindowRect, GUI.skin.box);
				
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				Rect popupRect = GUILayoutUtility.GetRect(this.PagesPopup.DrawRect.width, this.PagesPopup.DrawRect.height, this.PagesPopup.ButtonStyle);
				// draw popup here, so it gets clicks before things that are under it
				this.PagesPopup.DrawInsideContainer(popupRect, this.WindowRect);
				
//				if (GUILayout.Button("^")) {
//					this.WindowRect.y -= 5.0f;
//				}
//				if (GUILayout.Button("v")) {
//					this.WindowRect.y += 5.0f;
//				}
//				if (GUILayout.Button("<")) {
//					this.WindowRect.x -= 5.0f;
//				}
//				if (GUILayout.Button(">")) {
//					this.WindowRect.x += 5.0f;
//				}
				
				DevUIPage page = this.Pages[this.CurrentPageIndex];
				bool invalidPage = (page == null || page.IsInvalid);
				
				if (!invalidPage && this.CurrentPageIndex >= this.FirstRemovablePageIndex) {
					if (page.ContextGroup == null) {
						Transform pageTransform = GetObjectTransform(page.ContextObject);
					
						bool origEnabled = GUI.enabled;
						GUI.enabled = (pageTransform);
						if (GUILayout.Button("Look At", this.ButtonStyle)) {
							this.DebugCameraTarget = pageTransform;
						}
						GUI.enabled = origEnabled;
					}
					
					if (GUILayout.Button("Remove", this.ButtonStyle)) {
						RemovePage(this.CurrentPageIndex);
					}
				}
				
				GUILayout.FlexibleSpace();
				DrawFullScreenToggle();
				if (GUILayout.Button("Minimize", this.ButtonStyle)) {
					this.State = DevUIState.Minimized;
				}
				DrawCloseButton();

				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				
				if (!invalidPage) {
					page.Scroller = GUILayout.BeginScrollView(page.Scroller);
					page.OnPageDraw(page);
					GUILayout.EndScrollView();
					if (Event.current.type == EventType.Repaint) {
						page.LastRect = GUILayoutUtility.GetLastRect();
					}
				}
			
				GUILayout.EndArea();
				
				if (!invalidPage) {
					if (Event.current.type == EventType.Repaint) {
						page.LastRect.position += this.WindowRect.position;
					}
					if (page.OnPostPageDraw != null) {
						page.OnPostPageDraw(page);
					}
				}
				
				// draw popup again, last, so it is visually on top, and not clipped by the area
				this.PagesPopup.DrawOutsideContainer(this.WindowRect);
				
				if (invalidPage) {
					if (Event.current.type == EventType.Repaint) {
						RemovePage(this.CurrentPageIndex);
					}
				}

				break;

			case DevUIState.Minimized:

				const int messageCount = 2;
				const float maxAge = 15f;
				const float fadeAge = maxAge - 1.5f;

				float lineHeight = this.ConsoleMessageStyles[0].CalcHeight(new GUIContent("X"), 100) + this.ConsoleMessageStyles[0].margin.vertical;
				float height = Mathf.Max(this.ButtonStyle.fixedHeight + this.ButtonStyle.margin.vertical,
					messageCount * lineHeight)
					+ GUI.skin.box.padding.vertical;
				this.WindowRectMinimized = new Rect(this.WindowRect.position.x, 0, this.WindowRect.width, height);

				GUILayout.BeginArea(this.WindowRectMinimized, GUI.skin.box);
			
				GUILayout.BeginVertical(GUI.skin.box);

				GUILayout.BeginHorizontal(GUILayout.MaxHeight(height));

				IEnumerable<ConsoleMessage> messages = this.ConsoleMessages.
					Skip(Mathf.Max(0, this.ConsoleMessages.Count - messageCount)).
					Where(m => m.Age < maxAge);

				if (messages.Count() > 0) {
					float maxWidth = this.WindowRect.width - 180f;
					GUILayout.BeginVertical();
					Color color = GUI.color;
					foreach (ConsoleMessage message in messages) {
						float alpha = 1.0f - Mathf.InverseLerp(fadeAge, maxAge, message.Age);
						GUI.color = new Color(color.r, color.g, color.b, alpha);
						DrawToastConsoleMessage(message, maxWidth);
					}
					GUI.color = color;
					GUILayout.EndVertical();
				}

				GUILayout.FlexibleSpace();

				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Restore", this.ButtonStyle)) {
					this.State = DevUIState.Visible;
				}
				DrawCloseButton();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
				GUILayout.EndArea();

				break;

			}
		}
		
		#endregion
		

		#region Internal Fields

		private const string PrefKeyConsoleMessageGroups = "DevUI.ConsoleMessageGroups";
		private const string AppInfoBugReportKey = "appInfo";

		private static DevUI m_Instance;


		/// <summary>
		/// The first value will be replaced with the calculated value from <see cref="GetScreenScale()"/>.
		/// </summary>
		private static float[] ScalePresets = { 1.0f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 3.0f, 4.0f };
		
		[System.NonSerialized]
		private GUIStyle m_ButtonStyle;

		[System.NonSerialized]
		private Rect m_ButtonRect;

		[System.NonSerialized]
		private Camera m_DebugCamera;
		
		[System.NonSerialized]
		private Transform m_DebugCameraTransform;
		
		[System.NonSerialized]
		private Vector3 m_DebugCameraVelocity;
		
		[System.NonSerialized]
		private GUIStyle m_PopupListStyle;
		
		[System.NonSerialized]
		private Popup m_PagesPopup;
		
		[System.NonSerialized]
		private Popup m_ScalePopup;
		
		[System.NonSerialized]
		private DevUIState m_State;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private ILogHandler m_UnityDefaultLogHandler;

		[System.NonSerialized]
		private WindowLocation m_WindowDockLocation;
		
		[System.NonSerialized]
		private Popup m_WindowDockPopup;

		[System.NonSerialized]
		private Rect m_WindowRect;

		
		#endregion

		
		#region Internal Properties
		
		private Rect ButtonRect {
			get {
				return m_ButtonRect;
			}
			set {
				m_ButtonRect = value;
				this.ButtonRectScreenspace = new Rect(
					GUIUtility.GUIToScreenPoint(m_ButtonRect.position), 
					GUIUtility.GUIToScreenPoint(m_ButtonRect.size));
			}
		}

		private Rect ButtonRectScreenspace {
			get; set;
		}
		
		private bool ButtonDownThisFrame {
			get; set;
		}
		
		private float ButtonDownTime {
			get; set;
		}

		private GUIStyle ButtonStyle {
			get {
				if (m_ButtonStyle == null) {
					m_ButtonStyle = new GUIStyle(this.Skin.button);
					m_ButtonStyle.fixedHeight = m_ButtonStyle.fixedHeight == 0 ? 32.0f : m_ButtonStyle.fixedHeight;
				}
				return m_ButtonStyle;
			}
		}
		
		private List<DebugOnGUIClient> Clients {
			get; set;
		}

		private bool ConsoleAutoScroll {
			get; set;
		}

		private List<ConsoleMessageGroup> ConsoleMessageGroups {
			get; set;
		}

		private Queue<ConsoleMessage> ConsoleMessages {
			get; set;
		}
		
		private GUIStyle[] ConsoleMessageStyles {
			get; set;
		}

		private Vector2 ConsoleScroller {
			get; set;
		}

		private int CurrentPageIndex {
			get {
				return this.PagesPopup.SelectedIndex;
			}
			set {
				this.PagesPopup.SelectedIndex = value;
			}
		}
		
		private Camera DebugCamera {
			get {
				if (m_DebugCamera == null) {
					GameObject go = new GameObject("DevUI Debug Camera");
					Transform goT = go.GetComponent<Transform>();
					goT.parent = this.Transform;
					m_DebugCamera = go.AddComponent<Camera>();
					m_DebugCamera.rect = GetDebugCameraRect(this.WindowDockLocation);
					if (this.DebugCameraTarget) {
						goT.position = this.DebugCameraTarget.position + Vector3.back;
						m_DebugCamera.enabled = true;
					} else {
						m_DebugCamera.enabled = false;
					}
				}
				return m_DebugCamera;
			}
		}
		
		private Transform DebugCameraTarget {
			get; set;
		}
		
		private Transform DebugCameraTransform {
			get {
				m_DebugCameraTransform = m_DebugCameraTransform ?? this.DebugCamera.GetComponent<Transform>();
				return m_DebugCameraTransform;
			}
		}
		
		private int FirstRemovablePageIndex {
			get; set;
		}

		private bool IsFullScreen {
			get; set;
		}

		private List<DevUIPage> Pages {
			get; set;
		}
		
		private Popup PagesPopup {
			get {
				if (m_PagesPopup == null) {
					GUIContent[] pageNames = RebuildPageNames();
					m_PagesPopup = Popup.Create(
						new Rect(0.0f, 0.0f, 300.0f, this.ButtonStyle.fixedHeight),
						pageNames[0],
						pageNames,
						this.ButtonStyle,
						this.Skin.box,
						this.PopupListStyle
						);
					m_PagesPopup.SelectedIndex = 0;
				}
				return m_PagesPopup;
			}
		}
		
		private GUIStyle PopupListStyle {
			get {
				if (m_PopupListStyle == null) {
					m_PopupListStyle = new GUIStyle(this.Skin.button);
					m_PopupListStyle.margin = new RectOffset(0,0,0,0);
					m_PopupListStyle.fixedHeight = m_ButtonStyle.fixedHeight == 0 ? 32.0f : m_ButtonStyle.fixedHeight;
				}
				return m_PopupListStyle;
			}
		}

		private float RequiredButtonDownTime {
			get; set;
		}

		public float Scale {
			get {
				if (this.ScalePopup.SelectedIndex == 0) {
					return GetScreenScale();
				} else {
					return ScalePresets[this.ScalePopup.SelectedIndex];
				}
			}
		}
		
		private Popup ScalePopup {
			get {
				if (m_ScalePopup == null) {
					GUIContent[] pageNames = new GUIContent[ScalePresets.Length];
					pageNames[0] = new GUIContent("Default");
					for (int i = 1; i < pageNames.Length; ++i) {
						pageNames[i] = new GUIContent(string.Format("{0:F1}x", ScalePresets[i]));
					}
					m_ScalePopup = Popup.Create(
						new Rect(0.0f, 0.0f, 100.0f, this.ButtonStyle.fixedHeight),
						pageNames[0],
						pageNames,
						this.ButtonStyle,
						this.Skin.box,
						this.PopupListStyle
						);
					m_ScalePopup.SelectedIndex = 0;
				}
				return m_ScalePopup;
			}
		}

		private Dictionary<string, Popup> ToastGroupPopups {
			get; set;
		}

		private List<ConsoleMessage> ToastMessages {
			get;
			set;
		}

		private Popup WindowDockPopup {
			get {
				if (m_WindowDockPopup == null) {
					m_WindowDockPopup = Popup.Create<WindowLocation>(
						new Rect(0.0f, 0.0f, 100.0f, this.ButtonStyle.fixedHeight),
						new GUIContent("Dock"),
						this.ButtonStyle,
						this.Skin.box,
						this.PopupListStyle
						);
					m_WindowDockPopup.SelectedIndex = 0;
					OnWindowDockLocationDidChange(-1, 0);
					m_WindowDockPopup.OnSelectionDidChange += OnWindowDockLocationDidChange;
				}
				return m_WindowDockPopup;
			}
		}
		
		private WindowLocation WindowDockLocation  {
			get { 
				return (WindowLocation)this.WindowDockPopup.SelectedIndex;
			}
			set { 
				this.WindowDockPopup.SelectedIndex = (int)value;
			}
		}
		
		private Rect WindowRect {
			get {
				return m_WindowRect;
			}
			set {
				m_WindowRect = value;
				this.WindowRectScreenspace = new Rect(
					GUIUtility.GUIToScreenPoint(m_WindowRect.position), 
					GUIUtility.GUIToScreenPoint(m_WindowRect.size));
			}
				
		}

		private Rect WindowRectScreenspace {
			get; set;
		}

		private Rect m_WindowRectMinimized;
		private Rect WindowRectMinimized {
			get {
				return m_WindowRectMinimized;
			}
			set {
				m_WindowRectMinimized = value;
				this.WindowRectMinimizedScreenspace = new Rect(
					GUIUtility.GUIToScreenPoint(m_WindowRectMinimized.position),
					GUIUtility.GUIToScreenPoint(m_WindowRectMinimized.size));
			}
		}

		private Rect WindowRectMinimizedScreenspace {
			get; set;
		}
		
		#endregion
		

		#region Internal Methods

		[RuntimeInitializeOnLoadMethod]
		private static void CreateDevUI() {
			if (DevUI.Instance) {
			}
		}

		private static Rect GetBaseWindowRect(WindowLocation windowDockLocation, float scale, bool fullScreen) {
			
			float invScale = 1.0f / scale;

			if (fullScreen) {
				return new Rect(0.0f, 0.0f, Screen.width * invScale, Screen.height * invScale);
			}

			switch (windowDockLocation) {
			case WindowLocation.Top:
				return new Rect(0.0f, 0.0f, Screen.width * invScale, Screen.height * 0.5f * invScale);
			case WindowLocation.Bottom:
				return new Rect(0.0f, Screen.height * 0.5f * invScale, Screen.width * invScale, Screen.height * 0.5f * invScale);
			case WindowLocation.Left:
				return new Rect(0.0f, 0.0f, Screen.width * 0.5f * invScale, Screen.height * invScale);
			case WindowLocation.Right:
			default:
				return new Rect(Screen.width * 0.5f * invScale, 0.0f, Screen.width * 0.5f * invScale, Screen.height * invScale);
			}
		}
		
		private static Rect GetDebugCameraRect(WindowLocation windowDockLocation) {
			const float size = 0.25f;
			switch (windowDockLocation) {
			case WindowLocation.Top:
				return new Rect(0.0f, 0.0f, size, size);
			case WindowLocation.Bottom:
				return new Rect(1.0f - size, 1.0f - size, size, size);
			case WindowLocation.Left:
				return new Rect(1.0f - size, 0.0f, size, size);
			case WindowLocation.Right:
			default:
				return new Rect(0.0f, 1.0f - size, size, size);
			}
		}
		
		private static Transform GetObjectTransform(UnityEngine.Object unityObject) {
			if (unityObject == null) {
				return null;
			} else if (unityObject is Component) {
				return (unityObject as Component).transform;
			} else if (unityObject is GameObject) {
				return (unityObject as GameObject).transform;
			} else {
				return null;
			}
		}
		
		private static float GetScreenScale() {
			// if there are too many pixels on the device, then our pixel-sized buttons are going to be too small
			return ScreenUtility.PointsToPixelsRatio;
		}
		
		protected void OnStateDidChange(DevUIState oldState, DevUIState newState) {
			this.ButtonDownTime = 0f;
			this.ButtonDownThisFrame = false;

			switch (newState) {

			case DevUIState.Hidden:
				break;

			case DevUIState.Button:
				this.RequiredButtonDownTime = 0.75f;
				break;

			case DevUIState.Visible:
				break;

			case DevUIState.Minimized:
				break;
			
			}
		}
		
		protected void OnStateWillChange(DevUIState oldState, DevUIState newState) {
			
		}
		
		private void AddDebugClient(DebugOnGUIClient client, bool silent) {
			this.Clients.Add(client);
			if (!silent) {
				LogFormat(LogType.Log, client.ContextObject, "DevUI: Added Client: {0}", client.Name);
			}
		}
		
		private void AddPage(DevUIPage page, bool silent = false) {
			this.Pages.Add(page);
			this.PagesPopup.ListContent = RebuildPageNames();
			if (!silent) {
				if (page.ContextObject) {
					LogFormat(LogType.Log, page.ContextObject, "DevUI: Added Page: {0}", page.Name);
				} else if (page.ContextGroup != null) {
					GameObject context = page.ContextGroup.Count > 0 ? page.ContextGroup[0] : null;
					LogFormat(LogType.Log, context, "DevUI: Added Group Page: {0} ({1} items)", page.Name, page.ContextGroup.Count);
				}
			}
		}

		private void DrawDevUIConsolePage(DevUIPage page) {

			if (Event.current.type == EventType.MouseDrag) {
				Rect rect = GUIUtility.ScreenToGUIRect(page.LastRect);
				if (rect.Contains(Event.current.mousePosition)) {
					this.ConsoleAutoScroll = false;
				}
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("System Info", GUILayout.Width(130))) {
				UnityEngine.Debug.Log(GetSystemInfoDump());
			}

			string exportText;
			if (BugReporter.Instance && BugReporter.Instance.IsSendingBugReport) {
				exportText = "Exporting...";
				GUI.enabled = false;
			} else {
				exportText = "Export";
			}
			if (GUILayout.Button(exportText, GUILayout.Width(130))) {
				if (BugReporter.Instance) {
					BugReporter.Instance.SendBugReport();
				}
			}
			GUI.enabled = true;

			bool isWrapped = this.ConsoleMessageStyles[0].wordWrap;
			bool newIsWrapped = GUILayout.Toggle(isWrapped, "Wrap", GUILayout.MinWidth(50));
			if (newIsWrapped != isWrapped) {
				AdjustConsoleWordWrap(newIsWrapped);
			}

			GUILayout.Space(10);

			if (GUILayout.Button("f-", GUILayout.MinWidth(50))) {
				AdjustConsoleFontSize(-1);
			}
			if (GUILayout.Button("F+", GUILayout.MinWidth(50))) {
				AdjustConsoleFontSize(1);
			}

			GUILayout.Space(10);

			this.ConsoleAutoScroll = GUILayout.Toggle(this.ConsoleAutoScroll, "\u02C7", GUILayout.MinWidth(50));

			GUILayout.EndHorizontal();

			GUILayout.BeginVertical(GUI.skin.box);
			this.ConsoleScroller = GUILayout.BeginScrollView(this.ConsoleScroller);

			#if UNITY_EDITOR && !SAGO_DEBUG_HOOK_LOG_IN_EDITOR
			GUILayout.Label("Console messages are not being captured by DevUI.");
			GUILayout.Label("If you want messages here in the editor, set the console flag:");
			GUILayout.Label("SAGO_DEBUG_HOOK_LOG_IN_EDITOR");
			#endif

			Rect lastMessage = new Rect();
			foreach (ConsoleMessage message in this.ConsoleMessages) {
				DrawDevUIConsoleMessage(message);
				if (Event.current.type == EventType.Repaint) {
					lastMessage = GUILayoutUtility.GetLastRect();
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();

			if (Event.current.type == EventType.Repaint && this.ConsoleAutoScroll) {
				this.ConsoleScroller = new Vector2(0f, lastMessage.max.y);
			}
		}

		private void DrawDevUIConsoleMessage(ConsoleMessage message) {
			
			Transform messageTransform = GetObjectTransform(message.Context);
			
			bool selected = messageTransform != null && messageTransform == this.DebugCameraTarget;
			
			bool newSelected = GUILayout.Toggle(selected, message.Content, this.ConsoleMessageStyles[(int)message.LogType]);
			if (selected != newSelected) {
				if (newSelected) {
					this.DebugCameraTarget = messageTransform;
				} else {
					this.DebugCameraTarget = null;
					this.DebugCamera.enabled = false;
				}
			}
		}

		private void AdjustConsoleFontSize(int delta) {
			foreach (var style in this.ConsoleMessageStyles) {
				style.fontSize = Mathf.Clamp(style.fontSize + delta, 6, 30);
			}
		}

		private void AdjustConsoleWordWrap(bool wrap) {
			foreach (var style in this.ConsoleMessageStyles) {
				style.wordWrap = wrap;
			}
		}
		
		private void DrawDevUICloudBuildPage(DevUIPage page) {
			GUILayout.Label(this.CloudBuildInfo);
		}
		
		private void DrawDevUIClientsPage(DevUIPage page) {
			if (this.Clients.Count == 0) {
				GUILayout.Label("There are no debug OnGUI clients added");
			} else {
				Color orig = GUI.color;
				
				foreach (DebugOnGUIClient client in this.Clients) {
					GUI.color = client.IsEnabled ? Color.green : Color.red;
					client.IsEnabled = GUILayout.Toggle(client.IsEnabled, client.ToggleContent, GUI.skin.button);
				}
				
				GUI.color = orig;
			}
		}
		
		private void DrawDevUISettingsPage(DevUIPage page) {
			
			GUILayoutOption[] labelOptions = { GUILayout.Width(200) };
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("GUI Scale:", labelOptions);
			Rect scaleRect = GUILayoutUtility.GetRect(this.ScalePopup.DrawRect.width, this.ScalePopup.DrawRect.height, this.ScalePopup.ButtonStyle);
			this.ScalePopup.DrawInsideContainer(scaleRect, page.LastRect, page.Scroller);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("Max Console Messages ({0}):", this.MaxConsoleMessages), labelOptions);
			this.MaxConsoleMessages = Mathf.RoundToInt(GUILayout.HorizontalSlider(this.MaxConsoleMessages, 10, 5000));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Window Docking:", labelOptions);
			Rect dockRect = GUILayoutUtility.GetRect(this.WindowDockPopup.DrawRect.width, this.WindowDockPopup.DrawRect.height, this.WindowDockPopup.ButtonStyle);
			this.WindowDockPopup.DrawInsideContainer(dockRect, page.LastRect, page.Scroller);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("Show Toast Message Groups:");
			GUILayout.BeginHorizontal();
			GUILayout.Space(50);
			GUILayout.BeginVertical();
			foreach (var item in this.ConsoleMessageGroups) {
				GUILayout.BeginHorizontal();

				string groupName = item.Name;
				Popup popup = this.ToastGroupPopups[groupName];

				GUILayout.Label(groupName, labelOptions);
				Rect rect = GUILayoutUtility.GetRect(popup.DrawRect.width, popup.DrawRect.height, popup.ButtonStyle);
				popup.DrawInsideContainer(rect, page.LastRect, page.Scroller);

				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			if (GUILayout.Button("Erase\nSaved\nSettings")) {
				EraseConsoleMessageGroups();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		
		private void DrawDevUISettingsPagePost(DevUIPage page) {
			this.ScalePopup.DrawOutsideContainer(page.LastRect, page.Scroller);
			this.WindowDockPopup.DrawOutsideContainer(page.LastRect, page.Scroller);

			foreach (var item in this.ToastGroupPopups) {
				Popup popup = item.Value;
				popup.DrawOutsideContainer(page.LastRect, page.Scroller);
			}
		}

		private void DrawCloseButton() {
			if (GUILayout.Button("Close", this.ButtonStyle)) {
				Hide();
			}
		}

		private void DrawFullScreenToggle() {
			this.IsFullScreen = GUILayout.Toggle(this.IsFullScreen, "Full", this.ButtonStyle);
		}

		private static ConsoleMessageLevel ConvertLogType(LogType logType) {
			ConsoleMessageLevel messageLevel;
			switch (logType) {
			case LogType.Error:
				messageLevel = ConsoleMessageLevel.Errors;
				break;
			case LogType.Assert:
				messageLevel = ConsoleMessageLevel.Exceptions;
				break;
			case LogType.Warning:
				messageLevel = ConsoleMessageLevel.Warnings;
				break;
			case LogType.Log:
				messageLevel = ConsoleMessageLevel.Messages;
				break;
			case LogType.Exception:
				messageLevel = ConsoleMessageLevel.Exceptions;
				break;
			default:
				messageLevel = ConsoleMessageLevel.None;
				break;
			}
			return messageLevel;
		}

		private bool IsToastable(ConsoleMessage message) {
			ConsoleMessageLevel messageLevel = ConvertLogType(message.LogType);
			UnityEngine.Object context = message.Context;
			if (!context) {
				return messageLevel <= this.ConsoleMessageGroups[0].ToastLevel;
			} else {
				bool hasGroup = false;
				for (int i = 1; i < this.ConsoleMessageGroups.Count; ++i) {
					ConsoleMessageGroup cat = this.ConsoleMessageGroups[i];
					if (cat.IncludesContext(context)) {
						if (messageLevel <= cat.ToastLevel) {
							return true;
						}
						hasGroup = true;
					}
				}

				// it isn't null, but it isn't in an active group either
				if (!hasGroup) {
					return messageLevel <= this.ConsoleMessageGroups[0].ToastLevel;
				} else {
					return false;
				}
			}
		}

		private void DrawToastWindow() {

			if (this.ToastMessages == null || this.ToastMessages.Count == 0) {
				return;
			}

			const int maxMessages = 10;
			const float maxAge = 10f;
			const float fadeAge = maxAge - 1.5f;

			for (int i = this.ToastMessages.Count - 1; i >= 0; --i) {
				var m = this.ToastMessages[i];
				if (m.Age > maxAge) {
					this.ToastMessages.RemoveAt(i);
				}
			}

			if (this.ToastMessages.Count > 0) {
				Vector2 scale = new Vector2(GUI.matrix.m00, GUI.matrix.m11);
				Vector2 invScale = new Vector2(1.0f / scale.x, 1.0f / scale.y);
				Vector2 screenDim = new Vector2(Screen.width * 0.5f, Screen.height * 0.25f);
				Vector2 screenPos = new Vector2(0.5f * Screen.width, Screen.height * 0.75f);
				Vector2 dim = Vector2.Scale(screenDim, invScale);
				Vector2 pos = GUIUtility.ScreenToGUIPoint(screenPos) - 0.5f * dim;
				Rect r = new Rect(pos, dim);

				GUILayout.BeginArea(r);
				GUILayout.BeginVertical(GUI.skin.box);
				Color color = GUI.color;
				int startIndex = Mathf.Max(0, this.ToastMessages.Count - maxMessages);
				int endIndex = this.ToastMessages.Count - 1;
				for (int i = startIndex; i <= endIndex; ++i) {
					var message = this.ToastMessages[i];
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					float alpha = 1.0f - Mathf.InverseLerp(fadeAge, maxAge, message.Age);
					GUI.color = new Color(color.r, color.g, color.b, alpha);
					DrawToastConsoleMessage(message, r.width - 20f);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				GUI.color = color;
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndArea();
			}
		}

		private void DrawToastConsoleMessage(ConsoleMessage message, float maxWidth) {

			// only first line
			bool addEllipsis = false;

			string text;
			int pos = message.Message.IndexOf('\n');
	        if (pos < 0) {
	            text = message.Message;
	        } else {
	        	text = message.Message.Substring(0, pos);
	        	addEllipsis = true;
	        }
			
			GUIStyle style = this.ConsoleMessageStyles[(int)message.LogType];
			GUIContent gc = new GUIContent(text);
			Vector2 size = style.CalcSize(gc);
			if (size.x > maxWidth) {
				// assume monospace font
				int maxLength = Mathf.Max(4, Mathf.RoundToInt(maxWidth / (size.x / text.Length)));
				if (text.Length > maxLength) {
					text = text.Substring(0, maxLength - 3);
					addEllipsis = true;
				}
			}

			if (addEllipsis) {
				text += "...";
			}

			if (GUILayout.Button(text, style, GUILayout.MaxWidth(maxWidth))) {
				this.State = DevUIState.Visible;
				this.PagesPopup.SelectedIndex = this.Pages.FindIndex(p => p.Name == "Console");
				this.ConsoleAutoScroll = true;
			}
		}

		private void OnWindowDockLocationDidChange(int oldValue, int newValue) {
			if (m_DebugCamera) {
				this.DebugCamera.rect = GetDebugCameraRect(this.WindowDockLocation);
			}
		}
		
		private GUIContent[] RebuildPageNames() {
			List<GUIContent> gcs = new List<GUIContent>();
			foreach (DevUIPage page in Pages) {
				gcs.Add(page.MenuContent);
			}
			return gcs.ToArray();
		}
		
		private void RemoveClient(int clientIndex) {
			LogFormat(LogType.Warning, this.Clients[clientIndex].ContextObject, "DevUI: Removed Client: {0}", this.Clients[clientIndex].Name);
			this.Clients.RemoveAt(clientIndex);
		}
		
		private void RemovePage(int pageIndex) {
			if (this.CurrentPageIndex >= pageIndex) {
				this.CurrentPageIndex = Mathf.Max(0, this.Pages.Count - 2);
			}
			LogFormat(LogType.Warning, this.Pages[pageIndex].ContextObject, "DevUI: Removed Page: {0}", this.Pages[pageIndex].Name);
			this.Pages.RemoveAt(pageIndex);
			this.PagesPopup.ListContent = RebuildPageNames();
		}
		
		private void InitializeCloudBuildInfo() {
			
			// CloudBuild insert info into builds it makes
			TextAsset manifest = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
			if (manifest != null) {
				System.Text.StringBuilder sb = new System.Text.StringBuilder("CloudBuild Info:\n");
				Dictionary<string,string> manifestDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(manifest.text);
				foreach(var kvp in manifestDict) {
					sb.AppendFormat("{0}: {1}\n", kvp.Key, kvp.Value);
				}
				this.CloudBuildInfo = sb.ToString();
				this.AddPage(new DevUIPage("CloudBuild", this, DrawDevUICloudBuildPage), true);
			} else {
				this.CloudBuildInfo = "Build not created on CloudBuild";
			}
			
		}

		private Popup CreatePopup<EnumType>() where EnumType : struct, System.IConvertible {
			Popup popup = Popup.Create<EnumType>(
				new Rect(0.0f, 0.0f, 100.0f, this.ButtonStyle.fixedHeight),
				new GUIContent(),
				this.ButtonStyle,
				this.Skin.box,
				this.PopupListStyle
				);
			popup.SelectedIndex = 0;
			return popup;
		}

		private void AddConsoleMessageGroup(ConsoleMessageGroup group) {
			if (!this.ConsoleMessageGroups.Exists(x => x.Name == group.Name)) {
				this.ConsoleMessageGroups.Add(group);
				this.ConsoleMessageGroups.Sort((a, b) => { 
					int order = a.Order.CompareTo(b.Order);
					if (order == 0) order = a.Name.CompareTo(b.Name);
					return order; } );

				Popup popup = CreatePopup<ConsoleMessageLevel>();
				popup.SelectedIndex = (int)group.ToastLevel;
				popup.OnSelectionDidChange += (oldV, newV) => 
					{ group.ToastLevel = (ConsoleMessageLevel)newV; SaveConsoleMessageGroups(); };

				this.ToastGroupPopups = this.ToastGroupPopups ?? new Dictionary<string, Popup>();
				this.ToastGroupPopups.Add(group.Name, popup);
			}
		}

		private void SaveConsoleMessageGroups() {
			if (this.ConsoleMessageGroups != null) {

				var temp = new SerializableConsoleMessageGroups();
				temp.Groups = this.ConsoleMessageGroups;

				var json = JsonUtility.ToJson(temp);
				//LogJson(LogType.Log, this, "Saving Groups: {0}", json);

				PlayerPrefs.SetString(PrefKeyConsoleMessageGroups, json);
			}
		}

		private void LoadConsoleMessageGroups() {
			
			if (this.ConsoleMessageGroups == null) {
				this.ConsoleMessageGroups = new List<ConsoleMessageGroup>();	
			}

			if (PlayerPrefs.HasKey(PrefKeyConsoleMessageGroups)) {

				string json = PlayerPrefs.GetString(PrefKeyConsoleMessageGroups);
				//LogJson(LogType.Log, this, "Loading Groups: {0}", json);

				var temp = JsonUtility.FromJson<SerializableConsoleMessageGroups>(json);

				List<ConsoleMessageGroup> loaded = temp.Groups;
				if (loaded != null) {
					foreach (ConsoleMessageGroup group in loaded) {
						AddConsoleMessageGroup(group);
					}
				}
			}
		}

		private void EraseConsoleMessageGroups() {
			PlayerPrefs.DeleteKey(PrefKeyConsoleMessageGroups);
		}

		private static string GetSystemInfoDump() {
			return Json.SystemInfo.Default.GetFields();
		}

		protected void OnCreateBugReport(BugReport bugReport) {
			bugReport.Add(AppInfoBugReportKey, Json.DevUI.Create(this));
		}

		#endregion

	}

}
