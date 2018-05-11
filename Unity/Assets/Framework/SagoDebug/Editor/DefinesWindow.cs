namespace SagoDebugEditor {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	
	/// <summary>
	/// For operating on a set of preprocess defines.
	/// Locates the defines, and allows for toggling them on
	/// and off.
	/// </summary>
	abstract public class DefinesWindow : EditorWindow {
		

		#region EditorWindow

		virtual protected void OnEnable() {
			this.DefineSymbols = this.DefineSymbols ?? new List<DefineSymbol>();
			FindDefineSymbols();
		}

		protected void OnGUI() {

			EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

			DrawHeader();
			DrawListButtons();
			DrawList();
			GUILayout.Space(25);
			DrawBatchButtons();

			EditorGUI.EndDisabledGroup();  // IsCompiling

		}

		#endregion


		#region Internal Types

		protected class DefineSymbol {
			public string Name;
			public bool IsEnabled;
			public bool IsSelected;

			public DefineSymbol(string name, bool isEnabled) {
				this.Name = name;
				this.IsEnabled = isEnabled;
			}
		}

		#endregion


		#region Internal Fields

		protected GUIStyle m_ButtonStyle;
		protected GUIStyle m_LabelStyle;
		protected GUIStyle[] m_RowStyles;

		#endregion


		#region Internal Properties

		protected GUIStyle ButtonStyle {
			get {
				if (m_ButtonStyle == null) {
					GUIStyle style = new GUIStyle(EditorStyles.miniButton);
					style.richText = true;
					m_ButtonStyle = style;
				}
				return m_ButtonStyle;
			}
		}

		protected List<DefineSymbol> DefineSymbols {
			get; set;
		}

		virtual protected string HeaderInfoText {
			get {
				return 
					"This window allows you to view, and set, the various Sago preprocessor defines.\n\n" +
					"Toggling items will trigger a recompile.  It is best to select multiple and change them at once with the buttons at the bottom.";
			}
		}

		protected GUIStyle LabelStyle {
			get {
				if (m_LabelStyle == null) {
					GUIStyle style = new GUIStyle(EditorStyles.label);
					style.richText = true;
					m_LabelStyle = style;
				}
				return m_LabelStyle;
			}
		}

		protected GUIStyle[] RowStyles {
	    	get {
	    		if (m_RowStyles == null || m_RowStyles.Length < 2) {
	    			m_RowStyles = new GUIStyle[2];
	    			m_RowStyles[0] = new GUIStyle();

	    			m_RowStyles[1] = new GUIStyle(m_RowStyles[0]);

	    			Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
	    			Color32 c = new Color32(128, 128, 128, 32);
	    			tex.SetPixels32( new Color32[] { c, c, c, c } );
	    			tex.Apply();
	    			m_RowStyles[1].normal.background = tex;
	    		}
	    		return m_RowStyles;
	    	}
	    }

	    protected Vector2 Scroller {
	    	get; set;
	    }

		virtual protected List<string> SymbolPatterns {
			get;
			set;
		}

		virtual protected GUIContent TitleContent {
			get {
				return EditorWindow.GetWindow<DefinesWindow>().titleContent;
			}
			set {
				EditorWindow.GetWindow<DefinesWindow>().titleContent = value;
			}
		}

		#endregion


		#region Internal Methods

		virtual protected void DrawHeader() {
			GUILayout.Space(10);
			GUILayout.Box(this.HeaderInfoText, EditorStyles.helpBox);
			GUILayout.Space(10);
		}

		virtual protected void DrawListButtons() {
			
			GUILayout.BeginHorizontal();

			GUILayout.Label("Select:", EditorStyles.miniLabel);

			if (GUILayout.Button("All", EditorStyles.miniButtonLeft)) {
				this.DefineSymbols.ForEach(s => s.IsSelected = true);
			}

			if (GUILayout.Button("Invert", EditorStyles.miniButtonMid)) {
				this.DefineSymbols.ForEach(s => s.IsSelected = !s.IsSelected);
			}

			if (GUILayout.Button("None", EditorStyles.miniButtonRight)) {
				this.DefineSymbols.ForEach(s => s.IsSelected = false);
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Refresh List", EditorStyles.miniButton)) {
				FindDefineSymbols();
			}

			GUILayout.EndHorizontal();
		}

		virtual protected void DrawList() {
			this.Scroller = GUILayout.BeginScrollView(this.Scroller);

			int row = -1;
			foreach (var symbol in this.DefineSymbols) {
				row++;
				GUILayout.BeginHorizontal(this.RowStyles[row % this.RowStyles.Length]);
				DrawSymbol(symbol);
				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}

		virtual protected void DrawSymbol(DefineSymbol symbol) {

			GUILayoutOption[] columnOpts = {
				GUILayout.Width(25),
				GUILayout.Width(300),
				GUILayout.Width(50),
			};

			GUILayout.BeginHorizontal();

			symbol.IsSelected = GUILayout.Toggle(symbol.IsSelected, GUIContent.none, columnOpts[0]);

			GUILayout.Label(symbol.Name, this.LabelStyle, columnOpts[1]);

			Color color = symbol.IsEnabled ? Color.green : Color.red;
			string enabledText = string.Format("<color=#{0}>{1}</color>", SagoDebug.Debug.ColorToHex(color), symbol.IsEnabled);

			EditorGUI.BeginChangeCheck();
			symbol.IsEnabled = GUILayout.Toggle(symbol.IsEnabled, enabledText, this.ButtonStyle, columnOpts[2]);
			if (EditorGUI.EndChangeCheck()) {
				DebugEditorUtils.UpdateDefineSymbol(symbol.Name, symbol.IsEnabled);
			}

			GUILayout.EndHorizontal();
		}

		virtual protected void DrawBatchButtons() {
			GUILayout.BeginHorizontal();

			IEnumerable<DefineSymbol> selected = this.DefineSymbols.Where(s => s.IsSelected);

			EditorGUI.BeginDisabledGroup(selected.Count() == 0);

			EditorGUI.BeginDisabledGroup(selected.All(e => e.IsEnabled));
			if (GUILayout.Button("Enable Selected")) {
				DebugEditorUtils.UpdateDefineSymbols(
					this.DefineSymbols.Where(s => s.IsSelected).Select(n => n.Name).ToArray(), 
					null);
			}
			EditorGUI.EndDisabledGroup();  // Nothing to enable

			if (GUILayout.Button("Toggle Selected")) {
				DebugEditorUtils.UpdateDefineSymbols(
					this.DefineSymbols.Where(s => s.IsSelected && !s.IsEnabled).Select(n => n.Name).ToArray(), 
					this.DefineSymbols.Where(s => s.IsSelected && s.IsEnabled).Select(n => n.Name).ToArray());
			}

			EditorGUI.BeginDisabledGroup(selected.All(e => !e.IsEnabled));
			if (GUILayout.Button("Disabled Selected")) {
				DebugEditorUtils.UpdateDefineSymbols(
					null,
					this.DefineSymbols.Where(s => s.IsSelected).Select(n => n.Name).ToArray());
			}
			EditorGUI.EndDisabledGroup();  // Nothing to disable

			EditorGUI.EndDisabledGroup();  // Nothing selected

			GUILayout.EndHorizontal();
		}

		virtual protected void FindDefineSymbols() {

			if (this.SymbolPatterns == null) {
				return;
			}

			this.DefineSymbols = new List<DefineSymbol>();
			foreach (var regex in this.SymbolPatterns) {
				this.DefineSymbols.AddRange(FindDefineSymbols(regex));
			}

			this.DefineSymbols = this.DefineSymbols.OrderBy(ds => ds.Name).Distinct().ToList();

		}

		protected static List<DefineSymbol> FindDefineSymbols(string regex) {
			// first search for symbols that match the given expressions and
			// are on lines that have #*, then filter out the #*
			string path = Application.dataPath;
			string arg = string.Format("-Roh --include=\"*.cs\" \"\\#.*[[:<:]]{0}[[:>:]]\" .", regex);
			string[] args = { arg };
			string error;
			string output = RunCommand("grep", path, out error, args);

			string[] matches = output.Split(new char[]{ '\n' }, System.StringSplitOptions.RemoveEmptyEntries).
				OrderBy(a => a).
				Distinct().
				ToArray();

			arg = string.Format("-ohw \"{0}\"", regex);
			string filteredOutput = RunCommand("grep", path, matches, out error, new string[] { arg } );

			List<string> symbolNames = filteredOutput.Split( new char[]{ '\n' }, System.StringSplitOptions.RemoveEmptyEntries).
				OrderBy(a => a).
				Distinct().
				ToList();

			List<DefineSymbol> symbols = new List<DefineSymbol>();

			foreach (var symbolName in symbolNames) {
				bool isDefined = DebugEditorUtils.IsSymbolDefined(symbolName);
				DefineSymbol symbol = new DefineSymbol(symbolName, isDefined);
				symbols.Add(symbol);
			}

			return symbols;
		}

		public static string RunCommand(string command, string workingFolder, out string error, params string[] args) {

			Process p = StartCommand(command, workingFolder, false, args);
			p.WaitForExit();

			error = p.StandardError.ReadToEnd().Trim();
			string stdout = p.StandardOutput.ReadToEnd();

			p.Close();

			return stdout;
		}

		public static string RunCommand(string command, string workingFolder, string[] stdin, out string error, params string[] args) {

			Process p = StartCommand(command, workingFolder, true, args);

			foreach (var line in stdin) {
				p.StandardInput.WriteLine(line);
			}
			p.StandardInput.Close();
			p.WaitForExit();

			error = p.StandardError.ReadToEnd().Trim();
			string stdout = p.StandardOutput.ReadToEnd();

			p.Close();

			return stdout;
		}

		public static Process StartCommand(string command, string workingDirectory, bool useInput, params string[] args) {
			Process p = new Process();
			p.StartInfo.FileName = command;
			p.StartInfo.WorkingDirectory = workingDirectory;
			p.StartInfo.Arguments = string.Join(" ", args);
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardInput = useInput;
			p.StartInfo.UseShellExecute = false;
			p.Start();
			return p;
		}

		#endregion


	}
	
}
