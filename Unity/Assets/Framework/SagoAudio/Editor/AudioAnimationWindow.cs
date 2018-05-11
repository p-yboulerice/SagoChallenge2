namespace SagoAudioEditor {
	
	using SagoAudio;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Stopwatch = System.Diagnostics.Stopwatch;
	using UnityEditor;
	using UnityEngine;
	
	
	#region Editor Window
	
	public class AudioAnimationWindow : EditorWindow {
		
		
		#region Menu Methods
		
		[MenuItem("Window/Sago/Audio Animation")]
		public static void GetWindow() {
			EditorWindow.GetWindow<AudioAnimationWindow>();
		}
		
		#endregion
		
		
		#region Constants
		
		private static string AnimationKey = "SagoAudioEditor.AudioAnimationWindow.Animation";
		private static string FrameIndexKey = "SagoAudioEditor.AudioAnimationWindow.FrameIndex";
		private static string IsLoopKey = "SagoAudioEditor.AudioAnimationWindow.IsLoop";
		
		#endregion
		
		
		#region Properties
		
		private AudioAnimation AudioAnimation {
			get; set;
		}
		
		private AudioAnimationView AudioAnimationView {
			get; set;
		}
		
		private Vector2 ScrollPosition {
			get; set;
		}
		
		private Timer Timer {
			get; set;
		}
		
		private TimerView TimerView {
			get; set;
		}
		
		#endregion
		
		
		#region Other
		
		private int FrameCount {
			get {
				if (AudioAnimation) {
					return AudioAnimation.FrameCount;
				}
				return -1;
			}
		}
		
		private int FrameRate {
			get {
				if (AudioAnimation) {
					return AudioAnimation.FramesPerSecond;
				}
				return -1;
			}
		}
		
		#endregion
		
		
		#region Editor Window Methods
		
		private void OnDisable() {
			
		}
		
		private void OnEnable() {
			
			autoRepaintOnSceneChange = true;
			maxSize = new Vector2(1024,9999);
			minSize = new Vector2(640,640);
			wantsMouseMove = true;
			
			AudioAnimation = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(AnimationKey)), typeof(AudioAnimation)) as AudioAnimation;
			
			Timer = ScriptableObject.CreateInstance<Timer>();
			Timer.hideFlags = HideFlags.HideAndDontSave;
			Timer.FrameCount = FrameCount;
			Timer.FrameRate = FrameRate;
			Timer.IsLoop = EditorPrefs.GetBool(IsLoopKey);
			Timer.GoToAndStop(EditorPrefs.GetInt(FrameIndexKey));
			Timer.OnFrameIndexChanged += OnTimerFrameIndexChanged;
			Timer.OnIsPlayingChanged += OnTimerIsPlayingChanged;
			
			AudioAnimationView = ScriptableObject.CreateInstance<AudioAnimationView>();
			AudioAnimationView.hideFlags = HideFlags.HideAndDontSave;
			AudioAnimationView.RepaintLaterImpl = RepaintLater;
			AudioAnimationView.RepaintNowImpl = RepaintNow;
			AudioAnimationView.Timer = Timer;
			
			TimerView = ScriptableObject.CreateInstance<TimerView>();
			TimerView.hideFlags = HideFlags.HideAndDontSave;
			TimerView.RepaintLaterImpl = RepaintLater;
			TimerView.RepaintNowImpl = RepaintNow;
			TimerView.Timer = Timer;
			
			RepaintNow(true);
			
		}
		
		private void OnGUI() {
			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
			{
				GUIStyle buttonStyle;
				buttonStyle = new GUIStyle(GUI.skin.button);
				buttonStyle.margin = new RectOffset(0,0,2,2);
				
				GUIStyle sectionStyle;
				sectionStyle = new GUIStyle(GUI.skin.box);
				sectionStyle.border = new RectOffset(1,1,1,1);
				sectionStyle.padding = new RectOffset(10,10,10,10);
				sectionStyle.margin = new RectOffset(10,10,10,10);
				
				GUILayout.Space(10);
				
				EditorGUILayout.BeginVertical(sectionStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
				{
					GUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						AudioAnimation = EditorGUILayout.ObjectField("Audio Animation", AudioAnimation, typeof(AudioAnimation), false) as AudioAnimation;
						if (EditorGUI.EndChangeCheck()) {
							RepaintLater();
						}
						if (GUILayout.Button("New", buttonStyle, GUILayout.ExpandWidth(false))) {
							AudioAnimation = AudioAnimationEditor.CreateAudioAnimation();
							RepaintLater();
						}
					}
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				
				{
					EditorPrefs.SetString(AnimationKey, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(AudioAnimation)));
					Timer.FrameCount = FrameCount;
					Timer.FrameRate = FrameRate;
				}
				
				// timer view
				if (AudioAnimation != null) {
					EditorGUILayout.BeginVertical(sectionStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
						TimerView.OnGUI();
						EditorPrefs.SetBool(IsLoopKey, Timer.IsLoop);
					EditorGUILayout.EndVertical();
				}
				
				// audio animation view
				AudioAnimationView.Animation = AudioAnimation;
				if (AudioAnimation != null) {
					EditorGUILayout.BeginVertical(sectionStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
						AudioAnimationView.OnGUI();
					EditorGUILayout.EndVertical();
				}
				
			}
			EditorGUILayout.EndScrollView();
			RepaintNow();
		}
		
		private void OnLostFocus() {
			Timer.Stop();
		}
		
		#endregion
		
		
		#region Timer Events
		
		private void OnTimerFrameIndexChanged(Timer timer) {
			EditorPrefs.SetInt(FrameIndexKey, timer.FrameIndex);
			RepaintNow(true);
		}
		
		private void OnTimerIsPlayingChanged(Timer timer) {
			RepaintNow(true);
		}
		
		#endregion
		
		
		#region Repaint
		
		private bool m_RepaintFlag;
		
		private void RepaintLater() {
			m_RepaintFlag = true;
		}
		
		private void RepaintNow(bool flag = false) {
			if (m_RepaintFlag || flag) {
				Repaint();
				m_RepaintFlag = false;
			}
		}
		
		#endregion
		
		
	}
	
	#endregion
	
	
	#region Editor Window Support
	
	class AudioAnimationView : ScriptableObject {
		
		
		#region Fields
		
		private AudioAnimation m_Animation;
		
		private Timer m_Timer;
		
		#endregion
		
		
		#region Properties
		
		public AudioAnimation Animation {
			get { return m_Animation; }
			set {
				if (m_Animation != value) {
					m_Animation = value;
					Selection.Clear();
				}
			}
		}
		
		public Timer Timer {
			get { return m_Timer; }
			set {
				if (m_Timer != value) {
					if (m_Timer != null) {
						m_Timer.OnFrameIndexChanged -= OnTimerFrameIndexChanged;
						m_Timer.OnIsPlayingChanged -= OnTimerIsPlayingChanged;
					}
					m_Timer = value;
					if (m_Timer != null) {
						m_Timer.OnFrameIndexChanged += OnTimerFrameIndexChanged;
						m_Timer.OnIsPlayingChanged += OnTimerIsPlayingChanged;
					}
				}
			}
		}
		
		#endregion
		
		
		#region Color
		
		private readonly Color GridColor = new Color(0f,0f,0f,0.15f);
		
		private readonly Color HighlightColor = new Color(0f,0f,0f,0.4f);
		
		private readonly Color PlayheadColor = new Color(1f,0.5f,0f,0.35f);
		
		private readonly Color SelectionColor = new Color(1f,0.5f,0f,0.2f);
		
		#endregion
		
		
		#region Drag
		
		private Vector2 DragPosition {
			get; set;
		}
		
		private bool IsDragging {
			get; set;
		}
		
		#endregion
		
		
		#region Drag And Drop
		
		private long DragAndDropElapsed {
			get { return GetTimestamp() - DragAndDropTimestamp; }
		}
		
		private long DragAndDropTimestamp {
			get; set;
		}
		
		#endregion
		
		
		#region Lasso
		
		private bool IsLassoing {
			get; set;
		}
		
		private Vector2 LassoBegin {
			get; set;
		}
		
		private Vector2 LassoEnd {
			get; set;
		}
		
		private Rect LassoRect {
			get { 
				return Rect.MinMaxRect(
					Mathf.Min(LassoBegin.x, LassoEnd.x),
					Mathf.Min(LassoBegin.y, LassoEnd.y),
					Mathf.Max(LassoBegin.x, LassoEnd.x),
					Mathf.Max(LassoBegin.y, LassoEnd.y)
				);
			}
		}
		
		#endregion
		
		
		#region Layout
		
		private int ClampElementIndex(int elementIndex) {
			return ClampRowIndex(elementIndex);
		}
		
		private int GetElementCount() {
			return GetRowCount();
		}
		
		private int GetElementIndex(AudioAnimationElement element) {
			return GetRowIndex(element);
		}
		
		private int GetElementIndex(Rect timelineRect, Vector2 mousePosition) {
			return GetRowIndex(timelineRect, mousePosition);
		}
		
		private Rect GetElementRect(Rect timelineRect, AudioAnimationElement element) {
			
			float cellWidth = GetCellWidth(timelineRect);
			float cellHeight = GetCellHeight(timelineRect);
			
			int columnCount = GetColumnCount(element);
			int columnIndex = GetColumnIndex(element);
			int rowIndex = GetRowIndex(element);
			
			int lastColumnIndex = GetColumnCount() - 1;
			int lastRowIndex = GetRowCount() - 1;
			
			Rect rect = new Rect();
			rect.x = timelineRect.x + columnIndex * cellWidth;
			rect.y = timelineRect.y + rowIndex * cellHeight;
			rect.width = cellWidth * Mathf.Max(columnCount, 1);
			rect.height = cellHeight;
			
			rect.xMin += 1;
			if (columnIndex + columnCount >= lastColumnIndex) {
				rect.xMax -= 1;
			}
			rect.yMin += 1;
			if (rowIndex == lastRowIndex) {
				rect.yMax -= 1;
			}
			
			return rect;
			
		}
		
		
		private int ClampFrameIndex(int frameIndex) {
			return Mathf.Clamp(frameIndex, 0, GetFrameCount() - 1);
		}
		
		private int GetFrameCount() {
			return Animation ? Animation.FrameCount : -1;
		}
		
		private int GetFrameIndex(AudioAnimationElement element) {
			return GetColumnIndex(element);
		}
		
		private int GetFrameIndex(Rect timelineRect, Vector2 mousePosition) {
			float t = (mousePosition.x - timelineRect.xMin) / timelineRect.width;
			return Mathf.FloorToInt(t * GetColumnCount());
		}
		
		
		private int ClampColumnIndex(int columnIndex) {
			return Mathf.Clamp(columnIndex, 0, GetColumnCount() - 1);
		}
		
		private int ClampRowIndex(int rowIndex) {
			return Mathf.Clamp(rowIndex, 0, GetRowCount() - 1);
		}
		
		private float GetCellHeight(Rect timelineRect) {
			return timelineRect.height / GetRowCount();
		}
		
		private float GetCellWidth(Rect timelineRect) {
			return timelineRect.width / GetColumnCount();
		}
		
		private int GetColumnCount() {
			return Timer != null ? Timer.FrameCount : -1;
			// return (Animation != null) ? Animation.FrameCount : -1;
		}
		
		private int GetColumnCount(AudioAnimationElement element) {
			if (Animation != null && element.AudioClip != null) {
				return Mathf.CeilToInt(element.AudioClip.length * Timer.FrameRate / Mathf.Max(element.Pitch, 0.1f));
			}
			return -1;
		}
		
		private int GetColumnIndex(AudioAnimationElement element) {
			if (Animation != null && Animation.Elements != null && Animation.Elements.Contains(element)) {
				return element.FrameIndex;
			}
			return -1;
		}
		
		private int GetColumnIndex(Rect timelineRect, Vector2 mousePosition) {
			float t = (mousePosition.x - timelineRect.xMin) / timelineRect.width;
			return Mathf.FloorToInt(t * GetColumnCount());
		}
		
		private int GetRowCount() {
			return (Animation != null && Animation.Elements != null) ? Animation.Elements.Length : -1;
		}
		
		private int GetRowIndex(AudioAnimationElement element) {
			if (Animation != null && Animation.Elements != null && Animation.Elements.Contains(element)) {
				return System.Array.IndexOf(Animation.Elements, element);
			}
			return -1;
		}
		
		private int GetRowIndex(Rect timelineRect, Vector2 mousePosition) {
			float t = (mousePosition.y - timelineRect.yMin) / timelineRect.height;
			return Mathf.FloorToInt(t * GetRowCount());
		}
		
		#endregion
		
		
		#region Preview
		
		private Dictionary<int, AudioPreview> m_Previews;
		
		private Dictionary<int,AudioPreview> Previews {
			get {
				m_Previews = m_Previews ?? new Dictionary<int,AudioPreview>();
				return m_Previews;
			}
		}
		
		private void OnTimerFrameIndexChanged(Timer timer) {
			if (Timer.IsPlaying && Animation != null) {
				foreach (AudioAnimationElement element in Animation.ElementsAt(timer.FrameIndex)) {
					if (element.AudioClip) {
						
						int elementIndex = GetRowIndex(element);
						if (Previews.ContainsKey(elementIndex)) {
							AudioPreview preview;
							preview = Previews[elementIndex];
							Previews.Remove(elementIndex);
							if (preview) {
								preview.GetComponent<AudioSource>().Stop();
								DestroyImmediate(preview);
							}
						}
						
						Previews[elementIndex] = AudioPreviewHelper.Play(
							element.AudioClip, 
							element.Pitch, 
							element.Volume, 
							element.IsLoop
						);
						
					}
				}
				RepaintLater();
			}
		}
		
		private void OnTimerIsPlayingChanged(Timer timer) {
			if (Timer.IsPlaying) {
				OnTimerFrameIndexChanged(timer);
			} else {
				AudioPreviewHelper.Stop();
				Previews.Clear();
			}
		}
		
		#endregion
		
		
		#region Repaint
		
		public System.Action RepaintLaterImpl {
			get; set;
		}
		
		public System.Action<bool> RepaintNowImpl {
			get; set;
		}
		
		public void RepaintLater() {
			if (RepaintLaterImpl != null) {
				RepaintLaterImpl();
			}
		}
		
		public void RepaintNow(bool flag = false) {
			if (RepaintNowImpl != null) {
				RepaintNowImpl(flag);
			}
		}
		
		#endregion
		
		
		#region Selection
		
		private List<int> m_Selection;
		
		private List<int> Selection {
			get {
				m_Selection = m_Selection ?? new List<int>();
				return m_Selection;
			}
		}
		
		private void DuplicateSelection() {
			if (Animation != null && Animation.Elements != null && Animation.Elements.Length != 0 && Selection != null && Selection.Count != 0) {
				
				List<AudioAnimationElement> elements = new List<AudioAnimationElement>(Animation.Elements);
				List<int> indexes = Selection.OrderByDescending(i => i).ToList();
				List<string> guids = new List<string>();
				
				foreach (int index in indexes) {
					AudioAnimationElement clone = elements[index];
					clone.GUID = System.Guid.NewGuid().ToString();
					if (index == elements.Count - 1) {
						elements.Add(clone);
						guids.Add(clone.GUID);
					} else {
						elements.Insert(index + 1, clone);
						guids.Add(clone.GUID);
					}
				}
				
				Animation.Elements = elements.ToArray();
				EditorUtility.SetDirty(Animation);
				
				Selection.Clear();
				Selection.AddRange(Animation.Elements.Where(e => guids.Contains(e.GUID)).Select(e => GetElementIndex(e)));
				
			}
		}
		
		private bool IsSelected(AudioAnimationElement element) {
			return Selection.Contains(GetRowIndex(element));
		}
		
		#endregion
		
		
		#region Sync
		
		public System.Func<bool> FrameCountSyncedImpl {
			get; set;
		}
		
		public System.Func<bool> FrameRateSyncedImpl {
			get; set;
		}
		
		public System.Action SyncFrameCountImpl {
			get; set;
		}
		
		public System.Action SyncFrameRateImpl {
			get; set;
		}
		
		#endregion
		
		
		#region Methods
		
		public void OnGUI() {
			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			{
				if (Animation != null) {
					Rect headerRect = GUILayoutUtility.GetRect(
						GUIContent.none,
						GUIStyle.none, 
						GUILayout.ExpandWidth(true),
						GUILayout.MinHeight(EditorGUIUtility.singleLineHeight)
					);
					OnGUI_TimelineHeader(headerRect);
					
					Rect timelineRect = GUILayoutUtility.GetRect(
						GUIContent.none, 
						GUIStyle.none, 
						GUILayout.ExpandWidth(true),
						GUILayout.ExpandHeight(true),
						GUILayout.MinHeight(Mathf.Max(GetRowCount(), 1) * EditorGUIUtility.singleLineHeight * 2)
					);
					OnGUI_TimelineSelection(timelineRect);
					OnGUI_TimelineDrag(timelineRect);
					OnGUI_TimelineDragAndDrop(timelineRect);
					OnGUI_TimelineKeyboard(timelineRect);
					OnGUI_TimelineGrid(timelineRect);
					OnGUI_TimelineElements(timelineRect);
					OnGUI_TimelineLasso(timelineRect);
					OnGUI_TimelineOutline(timelineRect);
					
					GUILayout.BeginHorizontal();
					{
						OnGUI_AudioAnimationOptions();
						OnGUI_AudioAnimationElementOptions();
					}
					GUILayout.EndHorizontal();
					
				} else {
					GUILayout.Label(GUIContent.none);
				}
			}
			GUILayout.EndVertical();
		}
		
		private void OnGUI_AudioAnimationOptions() {
			
			GUIStyle boxStyle = new GUIStyle(GUIStyle.none);
			boxStyle.margin = new RectOffset(0,0,10,0);
			boxStyle.padding = new RectOffset(0,20,0,10);
			
			GUIStyle buttonStyle;
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontStyle = FontStyle.Bold;
			buttonStyle.margin = new RectOffset(0,0,0,2);
			
			GUILayout.BeginVertical(boxStyle, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.5f));
			{
				GUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					int frameCount = EditorGUILayout.IntField("Frame Count", Animation ? Animation.FrameCount : 0);
					if (Animation && EditorGUI.EndChangeCheck()) {
						Animation.FrameCount = frameCount;
						EditorUtility.SetDirty(Animation);
					}
					
					if (SyncFrameCountImpl != null && FrameCountSyncedImpl != null) {
						EditorGUI.BeginDisabledGroup(FrameCountSyncedImpl());
						GUI.color = FrameCountSyncedImpl() ? Color.white : Color.yellow;
						if (GUILayout.Button("Sync", buttonStyle, GUILayout.Width(60))) {
							SyncFrameCountImpl();
						}
						GUI.color = Color.white;
						EditorGUI.EndDisabledGroup();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					int frameRate = EditorGUILayout.IntField("Frame Rate", Animation ? Animation.FramesPerSecond : 0);
					if (Animation && EditorGUI.EndChangeCheck()) {
						Animation.FramesPerSecond = frameRate;
						EditorUtility.SetDirty(Animation);
					}
					if (SyncFrameRateImpl != null && FrameRateSyncedImpl != null) {
						EditorGUI.BeginDisabledGroup(FrameRateSyncedImpl());
						GUI.color = FrameRateSyncedImpl() ? Color.white : Color.yellow;
						if (GUILayout.Button("Sync", buttonStyle, GUILayout.Width(60))) {
							SyncFrameRateImpl();
						}
						GUI.color = Color.white;
						EditorGUI.EndDisabledGroup();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				{
					List<int> trim = (Animation != null && Animation.Elements != null ? Animation.Elements.Where(e => e.FrameIndex >= Animation.FrameCount).Select(e => GetElementIndex(e)).OrderByDescending(i => i).ToList() : null);
					bool disabled = (trim == null || trim.Count == 0);
					
					EditorGUI.BeginDisabledGroup(disabled);
					GUI.color = (disabled ? Color.white : Color.yellow);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Trim", buttonStyle, GUILayout.Width(60))) {
						List<AudioAnimationElement> elements = Animation.Elements.ToList();
						foreach (int index in trim) {
							elements.RemoveAt(index);
						}
						Animation.Elements = elements.ToArray();
						EditorUtility.SetDirty(Animation);
					}
					GUI.color = Color.white;
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			
		}
		
		private void OnGUI_AudioAnimationElementOptions() {
			
			GUIStyle boxStyle = new GUIStyle(GUIStyle.none);
			boxStyle.margin = new RectOffset(0,0,10,0);
			boxStyle.padding = new RectOffset(20,0,0,10);
			
			GUIStyle buttonStyle;
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontStyle = FontStyle.Bold;
			buttonStyle.margin = new RectOffset(0,0,0,2);
			
			GUILayout.BeginVertical(boxStyle, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.5f));
			{
				// audio clip
				GUILayout.BeginHorizontal();
				{
					bool disabled = (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0 || Selection == null || Selection.Count == 0);
					AudioClip audioClip = !disabled ? Animation.Elements[Selection[0]].AudioClip : null;
					
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUI.BeginChangeCheck();
					audioClip = EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false) as AudioClip;
					if (EditorGUI.EndChangeCheck()) {
						AudioAnimationElement[] elements = Animation.Elements;
						foreach (int index in Selection) {
							elements[index].AudioClip = audioClip;
						}
						Animation.Elements = elements;
						EditorUtility.SetDirty(Animation);
					}
					EditorGUI.EndDisabledGroup();
					
					EditorGUI.BeginDisabledGroup(disabled || !audioClip);
					if (GUILayout.Button("▶︎", buttonStyle, GUILayout.Width(55))) {
						Timer.Stop();
						AudioAnimationElement element = Animation.Elements[Selection[0]];
						AudioPreviewHelper.Play(element.AudioClip, element.Pitch, element.Volume, false);
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				
				// frame index
				GUILayout.BeginHorizontal();
				{
					bool disabled = (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0 || Selection == null || Selection.Count != 1);
					int frameIndex = !disabled ? Animation.Elements[Selection[0]].FrameIndex : 0;
					int lastIndex = !disabled ? Animation.FrameCount - 1 : 0;
					
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUI.BeginChangeCheck();
					frameIndex = EditorGUILayout.IntSlider("Frame", frameIndex, 0, lastIndex);
					if (EditorGUI.EndChangeCheck() && !disabled) {
						AudioAnimationElement[] elements = Animation.Elements;
						foreach (int index in Selection) {
							elements[index].FrameIndex = frameIndex;
						}
						Animation.Elements = elements;
						EditorUtility.SetDirty(Animation);
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				
				// volume
				GUILayout.BeginHorizontal();
				{
					bool disabled = (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0 || Selection == null || Selection.Count == 0);
					float volume = !disabled ? Animation.Elements[Selection[0]].Volume : 0;
					
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUI.BeginChangeCheck();
					volume = EditorGUILayout.Slider("Volume", volume, 0, 1);
					if (EditorGUI.EndChangeCheck() && !disabled) {
						AudioAnimationElement[] elements = Animation.Elements;
						foreach (int index in Selection) {
							elements[index].Volume = volume;
						}
						Animation.Elements = elements;
						EditorUtility.SetDirty(Animation);
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				
				// pitch
				GUILayout.BeginHorizontal();
				{
					bool disabled = (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0 || Selection == null || Selection.Count == 0);
					float pitch = !disabled ? Animation.Elements[Selection[0]].Pitch : 0;
					
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUI.BeginChangeCheck();
					pitch = EditorGUILayout.Slider("Pitch", pitch, 0.1f, 1);
					if (EditorGUI.EndChangeCheck() && !disabled) {
						AudioAnimationElement[] elements = Animation.Elements;
						foreach (int index in Selection) {
							elements[index].Pitch = pitch;
						}
						Animation.Elements = elements;
						EditorUtility.SetDirty(Animation);
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				
				// loop
				GUILayout.BeginHorizontal();
				{
					bool disabled = (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0 || Selection == null || Selection.Count == 0);
					bool isLoop = !disabled ? Animation.Elements[Selection[0]].IsLoop : false;
					
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUI.BeginChangeCheck();
					isLoop = EditorGUILayout.Toggle("Loop", isLoop);
					if (EditorGUI.EndChangeCheck() && !disabled) {
						AudioAnimationElement[] elements = Animation.Elements;
						foreach (int index in Selection) {
							elements[index].IsLoop = isLoop;
						}
						Animation.Elements = elements;
						EditorUtility.SetDirty(Animation);
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				
			}
			GUILayout.EndVertical();
			
			Rect borderRect = GUILayoutUtility.GetLastRect();
			borderRect.xMin = borderRect.xMin - 1;
			borderRect.xMax = borderRect.xMin + 1;
			EditorGUI.DrawRect(borderRect, new Color(1,1,1,0.08f));
			
		}
		
		private void OnGUI_TimelineDrag(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				if (Animation != null && Animation.Elements != null && Animation.Elements.Length != 0) {
					if (IsDragging && Event.current.type == EventType.MouseDrag) {
					
						int oldColumnIndex = ClampColumnIndex(GetColumnIndex(relativeRect, DragPosition));
						int newColumnIndex = ClampColumnIndex(GetColumnIndex(relativeRect, Event.current.mousePosition));
						int columnDelta = newColumnIndex - oldColumnIndex;
					
						int oldRowIndex = ClampRowIndex(GetRowIndex(relativeRect, DragPosition));
						int newRowIndex = ClampRowIndex(GetRowIndex(relativeRect, Event.current.mousePosition));
						int rowDelta = newRowIndex - oldRowIndex;
					
						if (rowDelta != 0 || columnDelta != 0) {
							foreach (AudioAnimationElement element in Selection.Select(selectedIndex => Animation.Elements[selectedIndex])) {
								int frameIndex = ClampFrameIndex(GetFrameIndex(element));
								columnDelta = Mathf.Clamp(
									columnDelta,
									0 - frameIndex,
									GetFrameCount() - 1 - frameIndex
								);
								int elementIndex = ClampElementIndex(GetElementIndex(element));
								rowDelta = Mathf.Clamp(
									rowDelta,
									0 - elementIndex,
									GetElementCount() - 1 - elementIndex
								);
							}
						}
					
						if (rowDelta != 0 || columnDelta != 0) {
						
							AudioAnimationElement[] elements = Animation.Elements;
						
							if (rowDelta != 0) {
							
								int count = Mathf.Abs(rowDelta);
								List<int> all = Enumerable.Range(0, GetElementCount()).ToList();
								List<int> selected = Selection.OrderBy(n => n).ToList();
								List<int> head;
								List<int> tail;
							
								if (rowDelta < 0) {
								
									head = all.Where(i => i < selected.Min() && !selected.Contains(i)).ToList();
									tail = all.Where(i => !head.Contains(i) && !selected.Contains(i)).ToList();
								
									tail.InsertRange(0, head.GetRange(head.Count - count, count));
									head.RemoveRange(head.Count - count, count);
								
								} else {
								
									head = all.Where(i => i < selected.Max() && !selected.Contains(i)).ToList();
									tail = all.Where(i => !head.Contains(i) && !selected.Contains(i)).ToList();
								
									head.AddRange(tail.GetRange(0, count));
									tail.RemoveRange(0, count);
								
								}
							
								List<int> result = new List<int>();
								result.AddRange(head);
								result.AddRange(selected);
								result.AddRange(tail);
							
								elements = result.Select(index => Animation.Elements[index]).ToArray();
							
								Selection.Clear();
								Selection.AddRange(Enumerable.Range(head.Count, selected.Count));
							
							}
						
							if (columnDelta != 0) {
								foreach (int selectedIndex in Selection) {
									elements[selectedIndex].FrameIndex += columnDelta;
								}
							}
						
							Animation.Elements = elements;
							EditorUtility.SetDirty(Animation);
						
						}
					
						DragPosition = Event.current.mousePosition;
						RepaintLater();
					
					}
				}
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_TimelineDragAndDrop(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				if (Animation != null && Animation.Elements != null && Event.current.type == EventType.DragExited && DragAndDropElapsed > 1000 && relativeRect.Contains(Event.current.mousePosition)) {
				
					AudioClip[] audioClips = (
						DragAndDrop.objectReferences.Length > 0 ? 
						DragAndDrop.objectReferences.OfType<AudioClip>().ToArray() : 
						null
					);
				
					if (audioClips != null) {
					
						int frameIndex;
						frameIndex = ClampFrameIndex(GetColumnIndex(relativeRect, Event.current.mousePosition));
					
						Selection.Clear();
						List<AudioAnimationElement> elements = Animation.Elements.ToList();
						foreach (AudioClip audioClip in audioClips) {
							AudioAnimationElement element = new AudioAnimationElement();
							element.AudioClip = audioClip;
							element.FrameIndex = frameIndex;
							element.GUID = System.Guid.NewGuid().ToString();
							element.Pitch = 1;
							element.Volume = 1;
							elements.Add(element);
							Selection.Add(elements.Count - 1);
						}
					
						Animation.Elements = elements.ToArray();
						EditorUtility.SetDirty(Animation);
					
					}
				
					Event.current.Use();
					DragAndDropTimestamp = GetTimestamp();
					RepaintLater();
				
				}
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_TimelineElements(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				if (Animation != null) {
					foreach (AudioAnimationElement element in Animation.Elements) {
						
						Rect elementRect = GetElementRect(relativeRect, element);
						
						// rect
						EditorGUI.DrawRect(elementRect, Color.grey);
						
						// waveform
						if (element.AudioClip && Event.current.type == EventType.Repaint) {
							Texture2D texture = AssetPreview.GetAssetPreview(element.AudioClip);
							if (texture) {
								EditorGUI.DrawTextureTransparent(elementRect, texture);
							}
						}
						
						// selected
						if (IsSelected(element)) {
							EditorGUI.DrawRect(elementRect, SelectionColor);
						}
						
						// label
						if (element.AudioClip) {
							
							GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
							labelStyle.clipping = TextClipping.Clip;
							labelStyle.fontStyle = FontStyle.Bold;
							labelStyle.normal.textColor = Color.white;
							labelStyle.padding = new RectOffset(5,0,0,0);
							
							string labelText;
							labelText = element.AudioClip.name;
							
							EditorGUI.DropShadowLabel(elementRect, labelText, labelStyle);
							
						}
						
					}
				}
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_TimelineGrid(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				EditorGUI.DrawRect(relativeRect, new Color(0f,0f,0f,0.1f));
			
				if (GetColumnCount() > 0) {
				
					float progress;
					progress = (float)Timer.FrameIndex / (float)Timer.FrameCount;
				
					Rect rect;
					rect = new Rect(progress * relativeRect.width + 1, 0, GetCellWidth(relativeRect) - 1, relativeRect.height);
				
					EditorGUI.DrawRect(rect, PlayheadColor);
					EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, 1, rect.height), PlayheadColor);
					EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.yMin, 1, rect.height), PlayheadColor);
				
				}
			
				float cellHeight = GetCellHeight(relativeRect);
				for (int rowIndex = 0; rowIndex <= GetRowCount(); rowIndex++) {
					Rect line = new Rect(0, Mathf.Min(rowIndex * cellHeight, relativeRect.height - 1), relativeRect.width, 1);
					EditorGUI.DrawRect(line, GridColor);
				}
			
				float cellWidth = GetCellWidth(relativeRect);
				for (int columnIndex = 0; columnIndex <= Mathf.Min(GetColumnCount(), GetFrameCount()); columnIndex++) {
					Rect line = new Rect(Mathf.Min(columnIndex * cellWidth, relativeRect.width - 1), 0, 1, relativeRect.height);
					EditorGUI.DrawRect(line, columnIndex > 0 && columnIndex % 10 == 0 ? HighlightColor : GridColor);
				}
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_TimelineHeader(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0, 0, absoluteRect.width, absoluteRect.height);
				EditorGUI.DrawRect(new RectOffset(-1,-1,-1,0).Add(relativeRect), ScaleAlpha(Color.white, 0.1f));
				EditorGUI.DrawRect(new Rect(relativeRect.xMin, relativeRect.yMin, relativeRect.width, 1), HighlightColor);
				EditorGUI.DrawRect(new Rect(relativeRect.xMin, relativeRect.yMin, 1, relativeRect.height), HighlightColor);
				EditorGUI.DrawRect(new Rect(relativeRect.xMax - 1, relativeRect.yMin, 1, relativeRect.height), HighlightColor);
				
				float cellWidth = GetCellWidth(relativeRect);
				int columnCount = GetColumnCount();
				for (int columnIndex = 0; columnIndex <= columnCount; columnIndex++) {
					
					Rect majorHashRect = new Rect();
					majorHashRect.x = Mathf.Min(columnIndex * cellWidth, relativeRect.width - 1);
					majorHashRect.width = 1;
					majorHashRect.height = relativeRect.height;
					
					Rect minorHashRect = majorHashRect;
					minorHashRect.yMin = minorHashRect.yMin + minorHashRect.height * 0.5f;
					
					Rect labelRect = new Rect();
					labelRect.x = columnIndex * cellWidth;
					labelRect.width = 30;
					labelRect.height = relativeRect.height;
					
					GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
					labelStyle.alignment = TextAnchor.MiddleLeft;
					labelStyle.fontStyle = FontStyle.Bold;
					labelStyle.margin = new RectOffset(0,0,0,0);
					labelStyle.normal.textColor = ScaleAlpha(Color.white, 0.4f);
					labelStyle.padding = new RectOffset(4,0,0,0);
					
					string labelText;
					labelText = columnIndex.ToString();
					
					if (columnCount < 10) {
						EditorGUI.DrawRect(majorHashRect, GridColor);
						EditorGUI.LabelField(labelRect, labelText, labelStyle);
					} else if (columnCount < 100) {
						if (columnIndex % 5 == 0 && columnIndex % 10 != 0) {
							EditorGUI.DrawRect(minorHashRect, GridColor);
						}
						if (columnIndex % 10 == 0) {
							EditorGUI.DrawRect(majorHashRect, GridColor);
							EditorGUI.LabelField(labelRect, labelText, labelStyle);
						}
					} else {
						if (columnIndex % 50 == 0) {
							EditorGUI.DrawRect(minorHashRect, GridColor);
						}
						if (columnIndex % 100 == 0) {
							EditorGUI.DrawRect(majorHashRect, GridColor);
							EditorGUI.LabelField(labelRect, labelText, labelStyle);
						}
					}
					
				}
				
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_TimelineKeyboard(Rect absoluteRect) {
			if (Animation != null && Animation.Elements != null && Animation.Elements.Length != 0) {
				if (Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == 0 && Selection.Count > 0) {
					switch (Event.current.keyCode) {
						case KeyCode.Backspace:
						case KeyCode.Delete:
							List<AudioAnimationElement> elements = new List<AudioAnimationElement>(Animation.Elements);
							foreach (int selectedIndex in Selection.OrderByDescending(n => n)) {
								elements.RemoveAt(selectedIndex);
							}
							Animation.Elements = elements.ToArray();
							EditorUtility.SetDirty(Animation);
							Selection.Clear();
							Event.current.Use();
							RepaintLater();
						break;
						case KeyCode.D:
							if (Event.current.command) {
								DuplicateSelection();
								Event.current.Use();
							}
						break;
					}
				}
			}
		}
		
		private void OnGUI_TimelineLasso(Rect absoluteRect) {
			
			if (Event.current.type == EventType.MouseDown) {
				bool clickedAnyElement = false;
				foreach (AudioAnimationElement element in Animation.Elements) {
					Rect elementRect = GetElementRect(absoluteRect, element);
					if (elementRect.Contains(Event.current.mousePosition)) {
						clickedAnyElement = true;
						break;
					}
				}
				if (!clickedAnyElement && absoluteRect.Contains(Event.current.mousePosition)) {
					Selection.Clear();
					IsLassoing = true;
					LassoBegin = Event.current.mousePosition - absoluteRect.min;
					LassoEnd = Event.current.mousePosition - absoluteRect.min;
					RepaintLater();
				}
			} else if (Event.current.type == EventType.MouseDrag) {
				if (IsLassoing) {
					LassoEnd = Event.current.mousePosition - absoluteRect.min;
					RepaintLater();
				}
			} else if (Event.current.type == EventType.MouseUp) {
				if (IsLassoing) {
					Rect relativeRect = new Rect(0,0,absoluteRect.width, absoluteRect.height);
					foreach (AudioAnimationElement element in Animation.Elements) {
						Rect elementRect = GetElementRect(relativeRect, element);
						if (LassoRect.Overlaps(elementRect, true)) {
							Selection.Add(GetElementIndex(element));
						}
					}
					IsLassoing = false;
					RepaintLater();
				}
			}
			
			GUI.BeginGroup(absoluteRect);
			{
				if (IsLassoing) {
					EditorGUI.DrawRect(LassoRect, SelectionColor);
				}
			}
			GUI.EndGroup();
			
		}
		
		private void OnGUI_TimelineOutline(Rect absoluteRect) {
			
			Color outlineColor;
			outlineColor = Color.black;
			outlineColor.a = 0.4f;
			
			Color highlightColor;
			highlightColor = Color.white;
			highlightColor.a = 0.05f;
			
			Color shadowColor;
			shadowColor = Color.black;
			shadowColor.a = 0.1f;
			
			// bottom
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMax - 3, absoluteRect.width, 1), ScaleAlpha(highlightColor, 0.5f));
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMax - 2, absoluteRect.width, 1), highlightColor);
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMax - 1, absoluteRect.width, 1), outlineColor);
			
			// top
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMin + 2, absoluteRect.width, 1), ScaleAlpha(shadowColor, 0.5f));
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMin + 1, absoluteRect.width, 1), shadowColor);
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMin, absoluteRect.width, 1), outlineColor);
			
			// left
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin + 2, absoluteRect.yMin, 1, absoluteRect.height), ScaleAlpha(shadowColor, 0.5f));
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin + 1, absoluteRect.yMin, 1, absoluteRect.height), shadowColor);
			EditorGUI.DrawRect(new Rect(absoluteRect.xMin, absoluteRect.yMin, 1, absoluteRect.height), outlineColor);
			
			// right
			EditorGUI.DrawRect(new Rect(absoluteRect.xMax - 3, absoluteRect.yMin, 1, absoluteRect.height), ScaleAlpha(highlightColor, 0.5f));
			EditorGUI.DrawRect(new Rect(absoluteRect.xMax - 2, absoluteRect.yMin, 1, absoluteRect.height), highlightColor);
			EditorGUI.DrawRect(new Rect(absoluteRect.xMax - 1, absoluteRect.yMin, 1, absoluteRect.height), outlineColor);
			
		}
		
		private void OnGUI_TimelineSelection(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				if (Animation == null || Animation.Elements == null || Animation.Elements.Length == 0) {
					Selection.Clear();
				} else if (Event.current.type == EventType.MouseDown) {
					
					bool clickedAnyElement = false;
					foreach (AudioAnimationElement element in Animation.Elements) {
						
						int index = GetRowIndex(element);
						bool selected = IsSelected(element);
						Rect rect = GetElementRect(relativeRect, element);
						
						if (rect.Contains(Event.current.mousePosition)) {
							if (selected && Event.current.shift) {
								Selection.Remove(index);
								RepaintLater();
							}
							if (!selected && Event.current.shift) {
								Selection.Add(index);
								RepaintLater();
							}
							if (!selected && !Event.current.shift) {
								Selection.Clear();
								Selection.Add(index);
								RepaintLater();
							}
							clickedAnyElement = true;
							break;
						}
					
					}
					
					if (clickedAnyElement) {
						IsDragging = true;
						DragPosition = Event.current.mousePosition;
					} else {
						IsDragging = false;
						DragPosition = Vector2.zero;
						if (relativeRect.Contains(Event.current.mousePosition)) {
							Selection.Clear();
							RepaintLater();
						}
					}
					
					if (relativeRect.Contains(Event.current.mousePosition)) {
						GUI.FocusControl(null);
					}
				} else if (Event.current.type == EventType.MouseUp) {
					IsDragging = false;
					DragPosition = Vector2.zero;
				}
			}
			GUI.EndGroup();
		}
		
		#endregion
		
		
		#region Helper Methods
		
		private long GetTimestamp() {
			return Stopwatch.GetTimestamp() / System.TimeSpan.TicksPerMillisecond;
		}
		
		private Color ScaleAlpha(Color color, float scale) {
			color.a *= scale;
			return color;
		}
		
		#endregion
		
		
	}
	
	class Timer : ScriptableObject {
		
		
		#region Fields
		
		private int m_FrameCount;
		
		private float m_FrameElapsed;
		
		private int m_FrameIndex;
		
		private int m_FrameRate;
		
		private string m_Identifier;
		
		private bool m_IsLoop;
		
		private bool m_IsPlaying;
		
		private Stopwatch m_Stopwatch;
		
		#endregion
		
		
		#region Properties
		
		public float Duration {
			get { return FrameDuration * FrameCount; }
		}
		
		public float Elapsed {
			get { return (FrameIndex * FrameDuration) + FrameElapsed; }
		}
		
		public int FrameCount {
			get { return m_FrameCount; }
			set {
				if (m_FrameCount != value) {
					
					m_FrameCount = value;
					
					if (IsPlaying) {
						StopPlaying();
						NotifyIsPlayingChanged();
					}
					
					if (FrameIndex > LastIndex) {
						FrameIndex = LastIndex;
						NotifyFrameChanged();
					}
					
				}
			}
		}
		
		public float FrameDuration {
			get { return 1f / (float)FrameRate; }
		}
		
		public float FrameElapsed {
			get { return m_FrameElapsed; }
			private set { m_FrameElapsed = value; }
		}
		
		public int FrameIndex {
			get { return m_FrameIndex; }
			private set {
				if (m_FrameIndex != value) {
					m_FrameIndex = value;
				}
			}
		}
		
		public int FrameRate {
			get { return m_FrameRate; }
			set {
				if (m_FrameRate != value) {
					m_FrameRate = value;
				}
			}
		}
		
		public bool IsLoop {
			get { return m_IsLoop; }
			set {
				if (m_IsLoop != value) {
					m_IsLoop = value;
				}
			}
		}
		
		public bool IsPlaying {
			get { return m_IsPlaying; }
			private set { m_IsPlaying = value; }
		}
		
		public int LastIndex {
			get { return FrameCount > 0 ? FrameCount - 1 : -1; }
		}
		
		public int NextIndex {
			get { return (IsLoop && FrameIndex == LastIndex) ? 0 : Mathf.Min(FrameIndex + 1, LastIndex); }
		}
		
		public int PreviousIndex {
			get { return (IsLoop && FrameIndex == 0) ? LastIndex : Mathf.Max(FrameIndex - 1, 0); }
		}
		
		private Stopwatch Stopwatch {
			get {
				if (m_Stopwatch == null) {
					m_Stopwatch = new Stopwatch();
				}
				return m_Stopwatch;
			}
		}
		
		#endregion
		
		
		#region Scriptable Object
		
		private void OnDisable() {
			Stop();
		}
		
		#endregion
		
		
		#region Events
		
		public event System.Action<Timer> OnFrameIndexChanged;
		
		public event System.Action<Timer> OnIsPlayingChanged;
		
		
		private void NotifyFrameChanged() {
			if (OnFrameIndexChanged != null) {
				OnFrameIndexChanged(this);
			}
		}
		
		private void NotifyIsPlayingChanged() {
			if (OnIsPlayingChanged != null) {
				OnIsPlayingChanged(this);
			}
		}
		
		#endregion
		
		
		#region Methods
		
		public void GoToAndStop(int frameIndex) {
			
			if (IsPlaying) {
				StopPlaying();
				NotifyIsPlayingChanged();
			}
			
			if (FrameIndex != frameIndex) {
				FrameIndex = frameIndex;
				NotifyFrameChanged();
			}
			
		}
		
		public void GoToFirstFrame() {
			GoToAndStop(0);
		}
		
		public void GoToPreviousFrame() {
			GoToAndStop(PreviousIndex);
		}
		
		public void GoToNextFrame() {
			GoToAndStop(NextIndex);
		}
		
		public void GoToLastFrame() {
			GoToAndStop(LastIndex);
		}
		
		public void Play() {
			if (!IsPlaying) {
				
				if (FrameIndex == LastIndex) {
					FrameIndex = 0;
					NotifyFrameChanged();
				}
				
				StartPlaying();
				NotifyIsPlayingChanged();
				
			}
		}
		
		public void Stop() {
			if (IsPlaying) {
				StopPlaying();
				NotifyIsPlayingChanged();
			}
		}
		
		#endregion
		
		
		#region Helper Methods
		
		private void StartPlaying() {
			IsPlaying = true;
			FrameElapsed = 0;
			Stopwatch.Reset();
			Stopwatch.Start();
			EditorApplication.update -= Tick;
			EditorApplication.update += Tick;
		}
		
		private void StopPlaying() {
			IsPlaying = false;
			FrameElapsed = 0;
			Stopwatch.Reset();
			EditorApplication.update -= Tick;
		}
		
		private void Tick() {
			if (IsPlaying) {
				if (FrameCount == -1 || FrameRate == -1) {
					
					StopPlaying();
					NotifyIsPlayingChanged();
					
				} else {
					
					FrameElapsed += Stopwatch.ElapsedMilliseconds / 1000f;
					Stopwatch.Reset();
					Stopwatch.Start();
					
					while (IsPlaying && FrameElapsed >= FrameDuration) {
						if (IsLoop) {
							FrameIndex = (FrameIndex + 1) % FrameCount;
							FrameElapsed -= FrameDuration;
							NotifyFrameChanged();
						} else if (FrameIndex < LastIndex) {
							FrameIndex++;
							FrameElapsed -= FrameDuration;
							NotifyFrameChanged();
						} else {
							StopPlaying();
							FrameIndex = LastIndex;
							FrameElapsed = FrameDuration;
							NotifyIsPlayingChanged();
						}
					}
					
				}
			}
		}
		
		#endregion
		
		
	}
	
	class TimerView : ScriptableObject {
		
		
		#region Properties 
		
		public Timer Timer {
			get; set;
		}
		
		#endregion
		
		
		#region User Interface
		
		public void OnGUI() {
			
			// info
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginHorizontal();
				{
					string content = string.Format(
						"{0}/{1}", 
						Timer.FrameCount > 0 ? Timer.FrameIndex + 1 : 0,
						Timer.FrameCount > 0 ? Timer.LastIndex + 1 : 0
					);
				
					GUIStyle style;
					style = new GUIStyle(GUI.skin.label);
					style.alignment = TextAnchor.MiddleLeft;
					style.fontStyle = FontStyle.Bold;
					style.normal.textColor = Color.white;
				
					GUILayout.Label(content, style, GUILayout.MinWidth(100));
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				{
					string content = string.Format(
						"{0:0.0}/{1:0.0}",
						Timer.FrameCount > 0 ? Timer.Elapsed : 0, 
						Timer.FrameCount > 0 ? Timer.Duration : 0
					);
					
					GUIStyle style;
					style = new GUIStyle(GUI.skin.label);
					style.alignment = TextAnchor.MiddleRight;
					style.fontStyle = FontStyle.Bold;
					style.normal.textColor = Color.white;
					
					GUILayout.Label(content, style, GUILayout.MinWidth(100));
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndHorizontal();
			
			// slider
			GUILayout.BeginHorizontal();
			{
				EditorGUI.BeginDisabledGroup(Timer.FrameCount <= 0);
				{	
					GUIStyle sliderStyle;
					sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
					sliderStyle.margin = new RectOffset(0,0,10,10);
					sliderStyle.padding = new RectOffset(0,0,0,0);
					
					GUIStyle sliderThumbStyle;
					sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
					
					Rect sliderRect;
					sliderRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, sliderStyle);
					
					int lastIndex;
					lastIndex = Mathf.Max(Timer.LastIndex, 0);
					
					int frameIndex;
					frameIndex = Mathf.Max(Timer.FrameIndex, 0);
					frameIndex = (int)GUI.HorizontalSlider(sliderRect, frameIndex, 0, lastIndex, sliderStyle, sliderThumbStyle);
					
					if (Timer.FrameIndex != frameIndex) {
						Timer.GoToAndStop(frameIndex);
					}
				}
				EditorGUI.EndDisabledGroup();
				
			}
			GUILayout.EndHorizontal();
			
			// buttons
			GUILayout.BeginHorizontal();
			{
				float buttonSize;
				buttonSize = 60;
				
				GUIStyle buttonStyle;
				buttonStyle = new GUIStyle(GUI.skin.button);
				buttonStyle.fontStyle = FontStyle.Bold;
				
				GUIStyle fpsStyle;
				fpsStyle = new GUIStyle(buttonStyle);
				
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Label(string.Format("{0} FPS", Timer.FrameRate > 0 ? Timer.FrameRate : 0), fpsStyle, GUILayout.Width(buttonSize));
				EditorGUI.EndDisabledGroup();
				
				GUILayout.FlexibleSpace();
				
				EditorGUI.BeginDisabledGroup(Timer.FrameCount == -1 || Timer.FrameRate == -1);
				{
					if (GUILayout.Button("First", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.GoToFirstFrame();
					}
					if (GUILayout.Button("Prev", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.GoToPreviousFrame();
					}
					if (Timer.IsPlaying && GUILayout.Button("Stop", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.Stop();
					}
					if (!Timer.IsPlaying && GUILayout.Button("Play", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.Play();
					}
					if (GUILayout.Button("Next", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.GoToNextFrame();
					}
					if (GUILayout.Button("Last", buttonStyle, GUILayout.Width(buttonSize))) {
						Timer.GoToLastFrame();
					}
				}
				EditorGUI.EndDisabledGroup();
				
				GUILayout.FlexibleSpace();
				
				Timer.IsLoop = GUILayout.Toggle(Timer.IsLoop, "Loop", buttonStyle, GUILayout.Width(buttonSize));
				
			}
			GUILayout.EndHorizontal();
			
		}
		
		#endregion
		
		
		#region Repaint
		
		public System.Action RepaintLaterImpl {
			get; set;
		}
		
		public System.Action<bool> RepaintNowImpl {
			get; set;
		}
		
		
		public void RepaintLater() {
			if (RepaintLaterImpl != null) {
				RepaintLaterImpl();
			}
		}
		
		public void RepaintNow(bool flag = false) {
			if (RepaintNowImpl != null) {
				RepaintNowImpl(flag);
			}
		}
		
		#endregion
		
		
	}
	
	#endregion
	
	
}