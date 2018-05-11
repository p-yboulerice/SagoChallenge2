namespace SagoMeshEditor {
	
	using SagoMesh;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Stopwatch = System.Diagnostics.Stopwatch;
	using UnityEditor;
	using UnityEngine;
	
	
	#region Editor Window
	
	public class MeshAnimationWindow : EditorWindow {
		
		
		#region Menu Methods
		
		[MenuItem("Window/Sago/Mesh Animation")]
		public static void GetWindow() {
			EditorWindow.GetWindow<MeshAnimationWindow>();
		}
		
		#endregion
		
		
		#region Constants
		
		private static string AnimationKey = "SagoMeshEditor.MeshAnimationWindow.Animation";
		private static string FrameIndexKey = "SagoMeshEditor.MeshAnimationWindow.FrameIndex";
		private static string IsLoopKey = "SagoMeshEditor.MeshAnimationWindow.IsLoop";
		
		#endregion
		
		
		#region Properties
		
		private MeshAnimation MeshAnimation {
			get; set;
		}
		
		private MeshAnimationView MeshAnimationView {
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
				if (MeshAnimation) {
					return MeshAnimation.Frames.Length;
				}
				return -1;
			}
		}
		
		private int FrameRate {
			get {
				if (MeshAnimation) {
					return MeshAnimation.FramesPerSecond;
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
			
			MeshAnimation = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(AnimationKey)), typeof(MeshAnimation)) as MeshAnimation;
			
			Timer = ScriptableObject.CreateInstance<Timer>();
			Timer.hideFlags = HideFlags.HideAndDontSave;
			Timer.FrameCount = FrameCount;
			Timer.FrameRate = FrameRate;
			Timer.IsLoop = EditorPrefs.GetBool(IsLoopKey);
			Timer.GoToAndStop(EditorPrefs.GetInt(FrameIndexKey));
			Timer.OnFrameIndexChanged += OnTimerFrameIndexChanged;
			Timer.OnIsPlayingChanged += OnTimerIsPlayingChanged;
			
			MeshAnimationView = ScriptableObject.CreateInstance<MeshAnimationView>();
			MeshAnimationView.hideFlags = HideFlags.HideAndDontSave;
			MeshAnimationView.RepaintLaterImpl = RepaintLater;
			MeshAnimationView.RepaintNowImpl = RepaintNow;
			MeshAnimationView.Timer = Timer;
			
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
						MeshAnimation = EditorGUILayout.ObjectField("Mesh Animation", MeshAnimation, typeof(MeshAnimation), false) as MeshAnimation;
						if (EditorGUI.EndChangeCheck()) {
							RepaintLater();
						}
					}
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				
				{
					EditorPrefs.SetString(AnimationKey, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MeshAnimation)));
					Timer.FrameCount = FrameCount;
					Timer.FrameRate = FrameRate;
				}
				
				// timer view
				if (MeshAnimation != null) {
					EditorGUILayout.BeginVertical(sectionStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
						TimerView.OnGUI();
						EditorPrefs.SetBool(IsLoopKey, Timer.IsLoop);
					EditorGUILayout.EndVertical();
				}
				
				// mesh animation view
				MeshAnimationView.Animation = MeshAnimation;
				if (MeshAnimation != null) {
					EditorGUILayout.BeginVertical(sectionStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
						MeshAnimationView.OnGUI();
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
		
	enum MeshAnimationViewMatteType {
		Light,
		Medium,
		Dark
	}
	
	class MeshAnimationView : ScriptableObject {
		
		
		#region Properties
		
		public MeshAnimation Animation {
			get; set;
		}
		
		public Timer Timer {
			get; set;
		}
		
		#endregion
		
		
		#region Scriptable Object
		
		public void OnEnable() {
			MatteType = (MeshAnimationViewMatteType)EditorPrefs.GetInt(MatteTypeKey, (int)MeshAnimationViewMatteType.Dark);
		}
		
		public void OnDisable() {
			ReleaseRenderTexture();
		}
		
		#endregion
		
		
		#region User Interface
		
		public void OnGUI() {
			
			Rect absoluteRect = GUILayoutUtility.GetRect(
				GUIContent.none, 
				GUIStyle.none, 
				GUILayout.ExpandWidth(true), 
				GUILayout.ExpandHeight(true),
				GUILayout.MinHeight(240)
			);
			
			OnGUI_Matte(absoluteRect);
			OnGUI_RenderTexture(absoluteRect);
			OnGUI_Buttons(absoluteRect);
			
		}
		
		private void OnGUI_Matte(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				EditorGUI.DrawRect(relativeRect, MatteColor);
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_RenderTexture(Rect absoluteRect) {
			GUI.BeginGroup(absoluteRect);
			{
				Rect relativeRect = new Rect(0,0,absoluteRect.width,absoluteRect.height);
				Rect textureRect = GetRenderTextureRect(relativeRect);
				if (Event.current.type == EventType.Repaint) {
					int width = (int)textureRect.width;
					int height = (int)textureRect.height;
					if (RenderTexture == null || RenderTexture.width != width || RenderTexture.height != height) {
						ReleaseRenderTexture();
						CreateRenderTexture(width, height);
					}
					DrawRenderTexture(Animation, Timer.FrameIndex, width, height);
				}
				GUI.Label(textureRect, new GUIContent(RenderTexture));
			}
			GUI.EndGroup();
		}
		
		private void OnGUI_Buttons(Rect absoluteRect) {
			
			float buttonWidth = 24;
			float buttonHeight = 12;
			float buttonSpacing = 4;
			
			Rect controlsRect = new Rect();
			controlsRect.width = (buttonWidth * 3) + (buttonSpacing * 4);
			controlsRect.height = (buttonHeight * 1) + (buttonSpacing * 2);
			controlsRect.x = absoluteRect.xMax - controlsRect.width;
			controlsRect.y = absoluteRect.yMax - controlsRect.height;
			
			GUI.BeginGroup(controlsRect);
			{
				Color highlightColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
				Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
				
				RectOffset highlightOffset = new RectOffset(-1,-1,-1,-1);
				RectOffset shadowOffset = new RectOffset(1,1,1,1);
				
				Rect relativeRect = new Rect(0, 0, controlsRect.width, controlsRect.height);
				Rect lightRect = new Rect(relativeRect.x + buttonSpacing * 1 + buttonWidth * 0, relativeRect.y + buttonSpacing * 1, buttonWidth, buttonHeight);
				Rect mediumRect = new Rect(relativeRect.x + buttonSpacing * 2 + buttonWidth * 1, relativeRect.y + buttonSpacing * 1, buttonWidth, buttonHeight);
				Rect darkRect = new Rect(relativeRect.x + buttonSpacing * 3 + buttonWidth * 2, relativeRect.y + buttonSpacing * 1, buttonWidth, buttonHeight);
				
				EditorGUI.DrawRect(shadowOffset.Add(lightRect), shadowColor);
				EditorGUI.DrawRect(lightRect, MatteColorLight);
				EditorGUI.DrawRect(lightRect, highlightColor);
				EditorGUI.DrawRect(highlightOffset.Add(lightRect), MatteColorLight);
				
				EditorGUI.DrawRect(shadowOffset.Add(mediumRect), shadowColor);
				EditorGUI.DrawRect(mediumRect, MatteColorMedium);
				EditorGUI.DrawRect(mediumRect, highlightColor);
				EditorGUI.DrawRect(highlightOffset.Add(mediumRect), MatteColorMedium);
				
				EditorGUI.DrawRect(shadowOffset.Add(darkRect), shadowColor);
				EditorGUI.DrawRect(darkRect, MatteColorDark);
				EditorGUI.DrawRect(darkRect, highlightColor);
				EditorGUI.DrawRect(highlightOffset.Add(darkRect), MatteColorDark);
				
				if (Event.current.type == EventType.MouseUp && lightRect.Contains(Event.current.mousePosition)) {
					MatteType = MeshAnimationViewMatteType.Light;
					EditorPrefs.SetInt(MatteTypeKey, (int)MatteType);
					RepaintLater();
				}
				if (Event.current.type == EventType.MouseUp && mediumRect.Contains(Event.current.mousePosition)) {
					MatteType = MeshAnimationViewMatteType.Medium;
					EditorPrefs.SetInt(MatteTypeKey, (int)MatteType);
					RepaintLater();
				}
				if (Event.current.type == EventType.MouseUp && darkRect.Contains(Event.current.mousePosition)) {
					MatteType = MeshAnimationViewMatteType.Dark;
					EditorPrefs.SetInt(MatteTypeKey, (int)MatteType);
					RepaintLater();
				}
			}
			GUI.EndGroup();
		}
		
		#endregion
		
		
		#region Matte
		
		private readonly Color MatteColorLight = new Color(0.9f,0.9f,0.9f,1.0f);
		
		private readonly Color MatteColorMedium = new Color(0.5f,0.5f,0.5f,1.0f);
		
		private readonly Color MatteColorDark = new Color(0.1f,0.1f,0.1f,1.0f);
		
		private static string MatteTypeKey = "MeshAnimationView.Matte";
		
		
		private Color MatteColor {
			get {
				switch (MatteType) {
					case MeshAnimationViewMatteType.Light:
						return MatteColorLight;
					case MeshAnimationViewMatteType.Medium:
						return MatteColorMedium;
					default:
						return MatteColorDark;
				}
			}
		}
		
		private MeshAnimationViewMatteType MatteType {
			get; set;
		}
		
		#endregion
		
		
		#region Render Texture
		
		private RenderTexture RenderTexture {
			get; set;
		}
		
		
		private Rect GetRenderTextureRect(Rect rect) {
			
			float contentWidth = Animation ? Animation.ContentSize.x * Animation.PixelsPerMeter : 1600;
			float contentHeight = Animation ? Animation.ContentSize.y * Animation.PixelsPerMeter : 900;
			float contentRatio = contentWidth / contentHeight;
			
			float availableWidth = rect.width;
			float availableHeight = rect.height;
			float availableRatio = availableWidth / availableHeight;
			
			float renderTextureWidth = 0;
			float renderTextureHeight = 0;
			
			if (availableRatio < contentRatio) {
				renderTextureWidth =  Mathf.Max(availableWidth, 32);
				renderTextureHeight = renderTextureWidth / contentRatio;
			} else {
				renderTextureHeight = Mathf.Max(availableHeight, 32);
				renderTextureWidth = renderTextureHeight * contentRatio;
			}
			if (renderTextureWidth > contentWidth || renderTextureHeight > contentHeight) {
				float scale = Mathf.Min(contentWidth / renderTextureWidth, contentHeight / renderTextureHeight);
				renderTextureWidth *= scale;
				renderTextureHeight *= scale;
			}
			
			float x = rect.xMin + (rect.width - renderTextureWidth) * 0.5f;
			float y = rect.yMin + (rect.height - renderTextureHeight) * 0.5f;
			return new Rect(x, y, renderTextureWidth, renderTextureHeight);
			
		}
		
		private void CreateRenderTexture(int width, int height) {
			ReleaseRenderTexture();
			RenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
			RenderTexture.antiAliasing = 8;
			RenderTexture.hideFlags = HideFlags.HideAndDontSave;
			RenderTexture.Create();
		}
		
		private void ReleaseRenderTexture() {
			if (RenderTexture != null) {
				RenderTexture.Release();
				Object.DestroyImmediate(RenderTexture, true);
				RenderTexture = null;
			}
		}
		
		private void DrawRenderTexture(MeshAnimation animation, int index, int width, int height) {
			if (animation && index >= 0 && index < animation.Frames.Length && width > 0 && height > 0) {
				RenderTexture.active = RenderTexture;
				Graphics.SetRenderTarget(RenderTexture);
				SagoMeshEditor.MeshAnimationEditor.DrawPreviewNow(animation, width, height, index);
				RenderTexture.active = null;
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
		
		
		private void RepaintLater() {
			if (RepaintLaterImpl != null) {
				RepaintLaterImpl();
			}
		}
		
		private void RepaintNow(bool flag = false) {
			if (RepaintNowImpl != null) {
				RepaintNowImpl(flag);
			}
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