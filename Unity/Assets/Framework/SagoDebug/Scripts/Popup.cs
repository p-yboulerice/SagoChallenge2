namespace SagoDebug {
	using UnityEngine;
	using System.Collections.Generic;
	
	/// <summary>
	/// <para>UnityGUI (OnGUI) popup control - a button, that, when clicked, shows a drops down list.</para>
	/// <remarks>Heavily modified version of "http://wiki.unity3d.com/index.php?title=PopupList"</remarks>
	/// </summary>
	public class Popup {
		
		#region Types
		
		/// Callback method prototype for when the Popup selection value changes.
		public delegate void SelectionChange(int oldValue, int newValue);
		
		#endregion
		
		
		#region Public Properties
		
		/// <summary>
		/// Gets or sets the box style.
		/// </summary>
		/// <value>The box style.</value>
		public GUIStyle BoxStyle {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the content of the button.
		/// </summary>
		/// <value>The content of the button.</value>
		public GUIContent ButtonContent {
			get; 
			protected set;
		}
		
		/// <summary>
		/// Gets or sets the button style.
		/// </summary>
		/// <value>The button style.</value>
		public GUIStyle ButtonStyle {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the rectangle in which the base button draws.
		/// </summary>
		/// <value>The draw rect.</value>
		public Rect DrawRect {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is open.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen {
			get; 
			set;
		}
		
		/// <summary>
		/// The last rectangle that this drew in (changes depending on whether or not list is open).
		/// Main use is so you can test touches against it to prevent pass-through.
		/// </summary>
		/// <value>The last rect.</value>
		public Rect LastRect {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the content of the list.  Normally this will be set once when you create the
		/// list, but if the list changes afterward you can modify it.
		/// </summary>
		/// <value>The content of the list.</value>
		public GUIContent[] ListContent {
			get {
				return m_ListContent;
			}
			set {
				m_ListContent = value;
				if (m_ListContent != null) {
					this.SelectedIndex = Mathf.Clamp(this.SelectedIndex, 0, m_ListContent.Length - 1);
					this.ButtonContent = m_ListContent[this.SelectedIndex];
				} else {
					this.SelectedIndex = 0;
					this.ButtonContent = GUIContent.none;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the list style.
		/// </summary>
		/// <value>The list style.</value>
		public GUIStyle ListStyle {
			get; set;
		}
		
		/// <summary>
		/// Add/remove yourself to this to get a callback when the value changes.
		/// </summary>
		/// <value>The on selection did change.</value>
		public SelectionChange OnSelectionDidChange {
			get;
			set;
		}
		
		/// <summary>
		/// The currently selected item from the list.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex{
			get { return m_SelectedIndex; }
			set {
				int oldState = m_SelectedIndex;
				int newState = value;
				if (oldState != newState) {
					this.OnSelectedWillChange(oldState, newState);
					m_SelectedIndex = value;
					this.OnSelectedDidChange(oldState, newState);
				}
			}
		}
		
		#endregion
		
		
		#region Public Methods

		/// <summary>
		/// Create a new popup.  If you do not provide the styles, the defaults will be applied.
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="buttonContent">Button content.</param>
		/// <param name="listContent">List content.</param>
		/// <param name="buttonStyle">Button style.</param>
		/// <param name="boxStyle">Box style.</param>
		/// <param name="listStyle">List style.</param>
		public static Popup Create(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle = null, GUIStyle boxStyle = null, GUIStyle listStyle = null) {
			
			return new Popup(rect, buttonContent, listContent, buttonStyle, boxStyle, listStyle);
		}
		
		/// <summary>
		/// Creates a new popup list using an Enum type to populate the list.
		/// <remarks>Note that you cannot constrain a generic type to System.Enum, so this will throw an exception if you use a non-Enum type.</remarks>
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="buttonContent">Button content.</param>
		/// <param name="buttonStyle">Button style.</param>
		/// <param name="boxStyle">Box style.</param>
		/// <param name="listStyle">List style.</param>
		/// <typeparam name="EnumType">A System.Enum type</typeparam>
		public static Popup Create<EnumType>(Rect rect, GUIContent buttonContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
			where EnumType : struct, System.IConvertible {
			
			if (!typeof(EnumType).IsEnum) {
				throw new System.ArgumentException("EnumType must be an enumerated type");
			}
			GUIContent[] listContent = EnumToGUIContent<EnumType>();
			return Create(rect, buttonContent, listContent, buttonStyle, boxStyle, listStyle);
		}
		
		/// <summary>
		/// <para>Draw this Popup at its DrawRect (call from your OnGUI or similar callback).</para>
		/// <para>Note that due to the nature of Popups (i.e. drawing on top of other controls) regular drawing 
		/// is only effective in the simplest circumstances.  See <see cref="DrawInsideContainer()"/> and
		/// <see cref="DrawOutsideContainer()"/>.</para>
		/// </summary>
		public int Draw() {
			return this.Draw(this.DrawRect);
		}
		
		/// <summary>
		/// <para>Draw this Popup at the given location (call from your OnGUI or similar callback).</para>
		/// <para>Note that due to the nature of Popups (i.e. drawing on top of other controls) regular drawing 
		/// is only effective in the simplest circumstances.  See <see cref="DrawInsideContainer()"/> and
		/// <see cref="DrawOutsideContainer()"/>.</para>
		/// </summary>
		/// <param name="drawRect">Draw rect.</param>
		public int Draw(Rect drawRect) {
			return Draw (drawRect, false, false);
		}
		
		/// <summary>
		/// <para><see cref="DrawInsideContainer()"/> and <see cref="DrawOutsideContainer()"/> are an alternative to <see cref="Draw()"/>
		/// which allows the open Popup to go outside of a window/scrollarea without getting clipped.</para>
		/// <para>Call DrawInsideContainer from inside the window/area, and call DrawOutsideContainer from outside them.
		/// Both are necessary to function properly.</para>
		/// </summary>
		/// <returns>The current selection index</returns>
		/// <param name="drawRect">Draw rect.</param>
		/// <param name="containerRect">The rectangle of the container you are inside.</param>
		/// <param name="scroller">If in a scrollview, pass that, otherwise pass Vector2.zero.</param>
		public int DrawInsideContainer(Rect drawRect, Rect containerRect, Vector2 scroller = default(Vector2)) {

			int result = Draw (drawRect, false, true);
			
			containerRect.position = Vector2.zero;
			bool visible = containerRect.Contains(drawRect.center - scroller);
			if (!visible && Event.current.type == EventType.Repaint) {
				this.IsOpen = false;
			}
			
			return result;
		}
		
		/// <summary>
		/// <para><see cref="DrawInsideContainer()"/> and <see cref="DrawOutsideContainer()"/> are an alternative to <see cref="Draw()"/></para>
		/// <para>Call this from outside the window/scrollarea so it can draw on top.  Both are necessary to function properly.</para>
		/// </summary>
		/// <returns>The current selection index.</returns>
		/// <param name="containerRect">The rectangle of the container you are inside.</param>
		/// <param name="scroller">If in a scrollview, pass that, otherwise pass Vector2.zero.</param>
		public int DrawOutsideContainer(Rect containerRect, Vector2 scroller = default(Vector2)) {
		
			Rect lastRect = this.LastRect;
			Rect drawRect = lastRect;
			drawRect.height = this.DrawRect.height;
			drawRect.position += containerRect.position;
			drawRect.position -= scroller;
			
			int result = Draw(drawRect, true, false);
			
			this.LastRect = lastRect;

			return result;
		}
		
		#endregion
		
	
		#region Internal Fields
		
		private static int useControlID = -1;
		
		private static bool forceClose = false;
		
		private GUIContent[] m_ListContent;
		
		private int m_SelectedIndex;
		
		#endregion
		
		
		#region Internal Methods
		
		/// <summary>
		/// Converts the names of an Enum to an array of GUIContent.
		/// </summary>
		/// <returns>The to GUI content.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected static GUIContent[] EnumToGUIContent<T>() {
			List<GUIContent> content = new List<GUIContent>();
			foreach (string name in System.Enum.GetNames(typeof(T))) {
				content.Add(new GUIContent(name));
			}
			return content.ToArray();
		}
		
		/// <summary>
		/// Popups by nature don't work well in UnityGUI, since they want to draw on top of
		/// things.  This is a modified version of the draw which allows the components of
		/// the popup to draw invisibly so they still function/get clicks in the right order.
		/// </summary>
		/// <param name="drawRect">Draw rect.</param>
		/// <param name="invisibleButton">If set to <c>true</c> invisible button.</param>
		/// <param name="invisibleList">If set to <c>true</c> invisible list.</param>
		protected int Draw(Rect drawRect, bool invisibleButton, bool invisibleList) {
			
			this.ButtonStyle = this.ButtonStyle ?? GUI.skin.button;
			this.BoxStyle = this.BoxStyle ?? GUI.skin.box;
			this.ListStyle = this.ListStyle ?? GUI.skin.box;
			
			if (forceClose) {
				forceClose = false;
				this.IsOpen = false;
			}
			
			bool done = false;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			
			switch (Event.current.GetTypeForControl(controlID)) {
			case EventType.MouseUp: {
				if (this.IsOpen) {
					done = true;
				}
			}
				break;
			}
			
			Color orig = GUI.color;
			GUI.color = invisibleButton ? new Color(0,0,0,0) : orig;
			
			if (GUI.Button(drawRect, this.ButtonContent, this.ButtonStyle)) {
				if (useControlID == -1) {
					useControlID = controlID;
					//this.IsOpen = false;
				}
				if (useControlID != controlID) {
					forceClose = true;
					useControlID = controlID;
				}
				this.IsOpen = true;
			}
			LastRect = drawRect;
			
			if (this.IsOpen) {
				Rect listRect = new Rect(
					drawRect.x,
					drawRect.y + this.ListStyle.CalcHeight(this.ListContent[0], 1.0f),
					drawRect.width,
					this.ListStyle.CalcHeight(this.ListContent[0], 1.0f) * this.ListContent.Length
					);
				
				
				GUI.color = invisibleList ? new Color(0,0,0,0) : orig;
				
				GUI.Box(listRect, string.Empty, this.BoxStyle);
				this.SelectedIndex = GUI.SelectionGrid(listRect, this.SelectedIndex, this.ListContent, 1, this.ListStyle);
				LastRect = new Rect(drawRect.x, drawRect.y, drawRect.width, drawRect.height + listRect.height);
			}
			GUI.color = orig;
			
			if (done) {
				this.IsOpen = false;
			}
			
			return this.SelectedIndex;
		}
		
		protected void OnSelectedDidChange(int oldState, int newState) {
			this.ButtonContent = this.ListContent[newState];
			
			if (this.OnSelectionDidChange != null) {
				this.OnSelectionDidChange(oldState, newState);
			}
		}
		
		protected void OnSelectedWillChange(int oldState, int newState) {
			
		}
		
		protected Popup(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle = null, GUIStyle boxStyle = null, GUIStyle listStyle = null) {
			this.DrawRect = rect;
			this.ButtonContent = buttonContent;
			this.ListContent = listContent;
			this.ButtonStyle = buttonStyle;
			this.BoxStyle = boxStyle;
			this.ListStyle = listStyle;
		}
		
		#endregion
		
	}
	
}

