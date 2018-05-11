namespace SagoDebug {
		
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Displays simple graphs for debugging purposes.  
	/// Use Create(), and either supply a function that will
	/// be called each frame, or add points manually
	/// with AddPoint().
	/// </summary>
	public class Graph : MonoBehaviour {


		#region Types

		public enum GraphStyle {
			Line,
			Bar
		}

		public class GraphFormatParams {
			
			public string Title { get; set; }
			public AxisFormatParams XAxis { get; protected set; }
			public AxisFormatParams YAxis { get; protected set; }

			public GraphFormatParams(string title, string yAxisLabel) :
				this(title, "Time", yAxisLabel) { }

			public GraphFormatParams(string title, string xAxisLabel, string yAxisLabel) {
				this.Title = title;
				this.XAxis = new AxisFormatParams(xAxisLabel);
				this.YAxis = new AxisFormatParams(yAxisLabel);
			}


			private static Color32[] AutoColors = {
				new Color32(255, 0, 0, 192),
				new Color32(0, 0, 255, 192),
				new Color32(255, 255, 0, 192),
				new Color32(0, 255, 0, 192),
				new Color32(255, 0, 255, 192),
				new Color32(0, 255, 255, 192),
				new Color32(255, 128, 0, 192),
				new Color32(0, 128, 255, 192),
				new Color32(255, 255, 128, 192),
				new Color32(128, 255, 0, 192),
				new Color32(255, 128, 255, 192),
				new Color32(128, 255, 255, 192)
			};
			private int m_NextAutoColorIndex = 0;

			public Color32 NextAutoColor {
				get {
					Color32 next = AutoColors[m_NextAutoColorIndex];
					m_NextAutoColorIndex = (m_NextAutoColorIndex + 1) % AutoColors.Length;
					return next;
				}
			}
			public Vector2 ManualYAxisRange;
			public bool UseManualYAxisRange;
		}

		public struct AxisFormatParams {

			public Color32 LineColor;
			public string Name;

			public AxisFormatParams(string label) : 
				this(label, Color.grey) { }

			public AxisFormatParams(string label, Color32 color) {
				this.Name = label;
				this.LineColor = color;
			}

		}

		public struct SeriesFormatParams {

			public string Name;
			public Color32 PrimaryColor;
			public GraphStyle Style;

			public SeriesFormatParams(string name, Color32 color) :
				this(name, GraphStyle.Line, color) { }

			public SeriesFormatParams(string name, GraphStyle style, Color32 color) {
				this.Name = name;
				this.Style = style;
				this.PrimaryColor = color;
			}

		}

		protected class Series {

			public Series() {
				this.Data = new List<Vector2>();
			}

			public Series(string name, Color32 color) : this() {
				this.Format = new SeriesFormatParams(name, color);
			}

			public Series(string name, GraphStyle style, Color32 color) : this() {
				this.Format = new SeriesFormatParams(name, style, color);
			}

			public SeriesFormatParams Format {
				get;
				set;
			}

			public List<Vector2> Data {
				get;
				set;
			}

			public System.Func<float> AutoSampleCallback {
				get;
				set;
			}

		}

		#endregion


		#region Public Properties

		public int MaxSamples {
			get; set;
		}

		public int SeriesCount {
			get {
				return this.SeriesList.Count;
			}
		}

		#endregion


		#region Factory Methods

		/// <summary>
		/// Creates a graph object.  Use the AddPoint() method on the
		/// returned value to add points.  Returns null if SAGO_DEBUG
		/// is not set.
		/// </summary>
		/// <param name="seriesName">Series name.</param>
		/// <param name="style">Style.</param>
		/// <param name="maxSamples">Max samples.</param>
		public static Graph Create(string seriesName, GraphStyle style = GraphStyle.Line, int maxSamples = DefaultMaxSamples) {
			return Create(seriesName, null, style, maxSamples);
		}

		/// <summary>
		/// Creates a graph object, which will call the given sampleCallback
		/// for data each frame.  Returns null if SAGO_DEBUG is not
		/// set.
		/// </summary>
		/// <param name="seriesName">Series name.</param>
		/// <param name="sampleCallback">Sample callback.</param>
		/// <param name="style">Style.</param>
		/// <param name="maxSamples">Max samples.</param>
		public static Graph Create(string seriesName, System.Func<float> sampleCallback, GraphStyle style = GraphStyle.Line, int maxSamples = DefaultMaxSamples) {

			#if SAGO_DEBUG

			GameObject go = new GameObject(string.Format("Graph {0}", seriesName));
			Graph newDebugGraph = go.AddComponent<Graph>();

			string xAxisName, title;
			if (sampleCallback == null) {
				xAxisName = "";
				title = seriesName;
			} else {
				xAxisName = "Time";
				title = string.Format("{0} vs {1}", seriesName, xAxisName);
			}

			newDebugGraph.Init(title, xAxisName, seriesName, maxSamples);
			newDebugGraph.AddSeries(seriesName, sampleCallback, style);

			return newDebugGraph;

			#else

			return null;

			#endif
		}

		/// <summary>
		/// Creates a graph with a specific color.  Returns null if
		/// SAGO_DEBUG is not set.
		/// </summary>
		/// <param name="seriesName">Series name.</param>
		/// <param name="sampleCallback">Sample callback.</param>
		/// <param name="color">Color.</param>
		/// <param name="style">Style.</param>
		/// <param name="maxSamples">Max samples.</param>
		public static Graph Create(string seriesName, System.Func<float> sampleCallback, Color color, GraphStyle style = GraphStyle.Line, int maxSamples = DefaultMaxSamples) {

			#if SAGO_DEBUG

			Graph newDebugGraph = Create(seriesName, sampleCallback, style, maxSamples);
			newDebugGraph.SetSeriesColor(0, color);

			return newDebugGraph;

			#else

			return null;

			#endif
		}

		#endregion


		#region Public Methods

		public int AddSeries(string name, GraphStyle style = GraphStyle.Line) {
			return AddSeries(name, null, style);
		}

		public int AddSeries(string name, System.Func<float> sampleCallback, Color color) {
			return AddSeries(name, sampleCallback, color, GraphStyle.Line);
		}

		public int AddSeries(string name, System.Func<float> sampleCallback, GraphStyle style = GraphStyle.Line) {
			return AddSeries(name, sampleCallback, this.Format.NextAutoColor, style);
		}

		public int AddSeries(string name, System.Func<float> sampleCallback, Color color, GraphStyle style) {
			Series series = new Series(name, style, color);
			series.AutoSampleCallback = sampleCallback;
			int index = this.SeriesList.Count;
			this.SeriesList.Add(series);
			UnityEngine.Debug.Assert(index == this.SeriesList.Count - 1);
			return index;
		}

		public void RemoveSeries(int seriesIndex) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				this.SeriesList.RemoveAt(seriesIndex);
			}
		}

		public void AddPoint(int seriesIndex, Vector2 point) {

			if (seriesIndex < 0 || seriesIndex >= this.SeriesList.Count) {
				return;
			}

			if (this.MaxSamples > 0) {
				if (this.SeriesList[seriesIndex].Data.Count >= this.MaxSamples) {
					this.SeriesList[seriesIndex].Data.RemoveRange(0, this.SeriesList[seriesIndex].Data.Count - this.MaxSamples);
				}
			}

			this.SeriesList[seriesIndex].Data.Add(point);
		}

		public void AddPoint(int seriesIndex, float yt) {
			AddPoint(seriesIndex, new Vector2(Time.time, yt));
		}

		public void AddPoint(int seriesIndex, float x, float y) {
			AddPoint(seriesIndex, new Vector2(x, y));
		}

		public void AddPoint(Vector2 point) {
			AddPoint(0, point);
		}

		public void AddPoint(float x, float y) {
			AddPoint(0, new Vector2(x, y));
		}

		public void AddPoint(float yt) {
			AddPoint(0, new Vector2(Time.time, yt));
		}

		public void SetYAxisRange(float minY, float maxY) {
			this.Format.UseManualYAxisRange = true;
			this.Format.ManualYAxisRange = new Vector2(minY, maxY);
		}

		public void SetAutoYAxisRange() {
			this.Format.UseManualYAxisRange = false;
		}

		public SeriesFormatParams GetSeriesFormat(int seriesIndex) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				return this.SeriesList[seriesIndex].Format;
			} else {
				return default(SeriesFormatParams);
			}
		}

		public void SetSeriesFormat(int seriesIndex, SeriesFormatParams format) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				this.SeriesList[seriesIndex].Format = format;
			}
		}

		public void SetSeriesName(int seriesIndex, string name) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				SeriesFormatParams format = this.SeriesList[seriesIndex].Format;
				format.Name = name;
				this.SeriesList[seriesIndex].Format = format;
			}
		}

		public void SetSeriesStyle(int seriesIndex, GraphStyle style) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				SeriesFormatParams format = this.SeriesList[seriesIndex].Format;
				format.Style = style;
				this.SeriesList[seriesIndex].Format = format;
			}
		}

		public void SetSeriesColor(int seriesIndex, Color color) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				SeriesFormatParams format = this.SeriesList[seriesIndex].Format;
				format.PrimaryColor = color;
				this.SeriesList[seriesIndex].Format = format;
			}
		}

		public void SetSeriesSampleCallback(int seriesIndex, System.Func<float> callback) {
			if (seriesIndex >= 0 && seriesIndex < this.SeriesList.Count) {
				this.SeriesList[seriesIndex].AutoSampleCallback = callback;
			}
		}

		#endregion


		#region MonoBehaviour

		protected void Start() {
			if (!this.IsInitialized) {
				Init(gameObject.name, "X", gameObject.name, DefaultMaxSamples);
			}
		}

		protected void OnGUI() {

			if (this.GuiRect.width == 0) {
				const float DefaultHeight = 100f;
				this.GuiRect = new Rect(0f, NextWindowStartY, Screen.width, DefaultHeight);
				NextWindowStartY = Mathf.Repeat(NextWindowStartY + DefaultHeight, Screen.height);
			}

			GUI.depth = (int)this.Z;

			this.GuiRect = GUI.Window(this.WindowId, this.GuiRect, DrawWindow, this.Format.Title, this.WindowStyle);

			// Convert Gui Window to screen space, with padding inside window
			RectOffset padding = new RectOffset(8, 8, 18, 8);
			Vector2 pos = GUIUtility.GUIToScreenPoint(new Vector2(this.GuiRect.xMin + padding.left, this.GuiRect.yMax - padding.bottom));
			pos.y = Screen.height - pos.y;
			Vector2 dim = GUIUtility.GUIToScreenPoint(this.GuiRect.size - new Vector2(padding.horizontal, padding.vertical));
			this.GraphRect = new Rect(pos, dim);

		}

		protected void LateUpdate() {
			for (int i = 0; i < this.SeriesList.Count; ++i) {
				if (this.SeriesList[i].AutoSampleCallback != null) {
					AddPoint(i, this.SeriesList[i].AutoSampleCallback());
				}
			}
		}

		#endregion


		#region Internal Fields

		protected const int DefaultMaxSamples = 500;

		protected static Material s_LineMaterial;

		protected GUIStyle m_WindowStyle;

		#endregion


		#region Internal Properties

		protected static int FocusedWindowId {
			get; set;
		}

		protected static Material LineMaterial {
			get {
				if (!s_LineMaterial) {
					Shader shader = Shader.Find("Hidden/Internal-Colored");
					Material mat = new Material(shader);
					mat.hideFlags = HideFlags.HideAndDontSave;
					// Turn on alpha blending
					mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					// Turn backface culling off
					mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
					s_LineMaterial = mat;
				}
				return s_LineMaterial;
			}
		}

		protected static float NextWindowStartY {
			get;
			set;
		}


		protected GraphFormatParams Format {
			get; 
			set;
		}

		protected Rect GraphRect {
			get; 
			set;
		}

		protected Rect GuiRect {
			get;
			set;
		}

		protected bool IsInitialized {
			get;
			set;
		}

		protected List<Series> SeriesList {
			get; set;
		}

		protected int WindowId {
			get {
				return GetHashCode();
			}
		}

		protected GUIStyle WindowStyle {
			get {
				if (m_WindowStyle == null) {
					GUIStyle style = new GUIStyle();
					style.normal.background = CreatePixelTexture(new Color32(128, 128, 128, 64));
					style.hover.background = CreatePixelTexture(new Color32(128, 128, 128, 128));
					style.active.background = style.hover.background;
					style.focused.background = style.active.background;

					m_WindowStyle = style;
				}
				return m_WindowStyle;
			}
		}

		protected float Z {
			get {
				return (this.WindowId == FocusedWindowId) ? 1f : 0f;
			}
		}

		#endregion


		#region Drawing

		protected void DrawWindow(int id) {

			// TODO: draw labels, etc
			GUI.DragWindow();

			if (Event.current.GetTypeForControl(id) == EventType.Used) {
				FocusedWindowId = id;
			}

			if (Event.current.type == EventType.Repaint) {
				Render();
			}
		}

		private int BarSeriesCount {
			get {
				int numBarSeries = 0;
				for (int i = 0; i < this.SeriesList.Count; ++i) {
					if (this.SeriesList[i].Format.Style == GraphStyle.Bar) {
						numBarSeries++;
					}
				}
				return numBarSeries;
			}
		}

		protected void Render() {

			// figure out size/scale for all series

			int pixelsPerDataPoint = Mathf.FloorToInt(this.GraphRect.width / this.MaxSamples);

			int barSeriesCount = this.BarSeriesCount;
			int minimumPixelWidth = 6 * (barSeriesCount + 1);

			pixelsPerDataPoint = Mathf.Max(pixelsPerDataPoint, minimumPixelWidth);

			int maxSamples = Mathf.FloorToInt(this.GraphRect.width / pixelsPerDataPoint);

			Vector2 yRange = CalcYRange(maxSamples);
			if (yRange[0] > 0f) {
				yRange[0] = 0f;
			} else if (yRange[1] < 0f) {
				yRange[1] = 0f;
			}

			Graph.LineMaterial.SetPass(0);

			GL.PushMatrix();

			GL.LoadPixelMatrix();

			float z = this.Z;

			float y = Mathf.Lerp(this.GraphRect.yMin, this.GraphRect.yMax, Mathf.InverseLerp(yRange[0], yRange[1], 0f));
			RenderAxis(this.Format.XAxis, new Vector3(this.GraphRect.xMin, y, z), new Vector3(this.GraphRect.xMax, y, z));

			RenderAxis(this.Format.YAxis, new Vector3(this.GraphRect.xMin, this.GraphRect.yMin, z), new Vector3(this.GraphRect.xMin, this.GraphRect.yMax, z));

			int barSeriesIndex = -1;
			float pixelsPerBar = pixelsPerDataPoint / (barSeriesCount + 1);
			Vector2 xPointPixelRange = new Vector2(0f, pixelsPerDataPoint);

			for (int i = 0; i < this.SeriesList.Count; ++i) {
				Series series = this.SeriesList[i];
				SeriesFormatParams format = this.SeriesList[i].Format;

				if (format.Style == GraphStyle.Bar) {
					barSeriesIndex++;
					xPointPixelRange = new Vector2(barSeriesIndex + 0.5f, barSeriesIndex + 1.5f) * pixelsPerBar;
				} else {
					xPointPixelRange = new Vector2(0f, pixelsPerDataPoint);
				}

				RenderSeries(series, format, maxSamples, pixelsPerDataPoint, xPointPixelRange, yRange);
			}

			GL.PopMatrix();
		}

		private void RenderAxis(AxisFormatParams format, Vector3 from, Vector3 to) {
			GL.Begin(GL.LINES);
			GL.Color(format.LineColor);
			GL.Vertex(from);
			GL.Vertex(to);
			GL.End();
		}

		private void RenderSeries(Series series, SeriesFormatParams format, int maxSamples, int pixelsPerDataPoint, Vector2 xPointPixelRange, Vector2 yRange) {

			int count = series.Data.Count;
			if (count == 0) {
				return;
			}

			int drawSamples = Mathf.Min(maxSamples, count);

			int baseScreenX = (int)this.GraphRect.xMax - drawSamples * pixelsPerDataPoint;

			float z = this.Z;

			switch (format.Style) {

			case GraphStyle.Line: {
					GL.Begin(GL.LINES);
					GL.Color(format.PrimaryColor);

					int startIndex = Mathf.Max(1, count - maxSamples);

					Vector3 prevPoint = new Vector3(baseScreenX, this.GraphRect.yMin + this.GraphRect.height * Mathf.InverseLerp(yRange[0], yRange[1], series.Data[startIndex - 1].y), z);

					for (int i = startIndex; i < count; ++i) {
						Vector3 point = new Vector3(prevPoint.x + pixelsPerDataPoint, 
							this.GraphRect.yMin + this.GraphRect.height * Mathf.InverseLerp(yRange[0], yRange[1], series.Data[i].y), z);
						GL.Vertex(prevPoint);
						GL.Vertex(point);
						prevPoint = point;
					}

					GL.End();
				}
				break;

			case GraphStyle.Bar: {

					GL.Begin(GL.QUADS);
					GL.Color(format.PrimaryColor);

					float y0 = this.GraphRect.yMin + this.GraphRect.height * Mathf.InverseLerp(yRange[0], yRange[1], 0f);

					int startIndex = Mathf.Max(0, count - maxSamples);
					for (int i = startIndex, drawSample = 0; i < count; ++i, ++drawSample) {
						Vector3 p0 = new Vector3(baseScreenX + drawSample * pixelsPerDataPoint + xPointPixelRange[0], 
							this.GraphRect.yMin + this.GraphRect.height * Mathf.InverseLerp(yRange[0], yRange[1], series.Data[i].y), z);

						Vector3 p1 = new Vector3(p0.x + xPointPixelRange[1] - xPointPixelRange[0], y0, z);

						GL.Vertex(p0);
						GL.Vertex3(p0.x, p1.y, z);
						GL.Vertex(p1);
						GL.Vertex3(p1.x, p0.y, z);
					}

					GL.End();
				}
				break;

			}
		}

		private Vector2 CalcYRange(int maxSamples) {
			if (this.Format.UseManualYAxisRange) {
				return this.Format.ManualYAxisRange;
			} else {
				Vector2 range = new Vector2(float.MaxValue, float.MinValue);
				for (int i = 0; i < this.SeriesList.Count; ++i) {
					Vector2 seriesRange = CalcSeriesYRange(this.SeriesList[i], maxSamples);
					range[0] = Mathf.Min(seriesRange[0], range[0]);
					range[1] = Mathf.Max(seriesRange[1], range[1]);
				}
				return range;
			}
		}

		private Vector2 CalcSeriesYRange(Series series, int maxSamples) {
			Vector2 range = new Vector2(float.MaxValue, float.MinValue);
			int count = series.Data.Count;
			int startIndex = Mathf.Max(0, count - maxSamples);
			for (int i = startIndex; i < count; ++i) {
				float y = series.Data[i].y;
				range[0] = Mathf.Min(y, range[0]);
				range[1] = Mathf.Max(y, range[1]);
			}
			return range;
		}

		#endregion


		#region Other Internal Methods

		protected static Texture2D CreatePixelTexture(Color32 color) {
			Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tex.SetPixels32(new Color32[]{ color });
			tex.Apply();
			return tex;
		}


		protected void Init(string title, string xAxisName, string yAxisName, int maxSamples) {

			if (this.IsInitialized) {
				return;
			}

			this.SeriesList = new List<Series>(maxSamples);

			this.Format = new GraphFormatParams(title, xAxisName, yAxisName);
			this.MaxSamples = maxSamples;
			
			this.IsInitialized = true;
		}

		#endregion


	}

}