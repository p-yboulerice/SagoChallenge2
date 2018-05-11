namespace SagoDebugEditor {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	
	/// <summary>
	/// For operating on the various SAGO_DEBUG_* defines.
	/// Locates the defines, and allows for toggling them on
	/// and off.
	/// </summary>
	public class DebugDefinesWindow : DefinesWindow {


		#region Menu

		[MenuItem ("Sago/Debug/Debug Define Symbols...", false, 152)]
		private static void MenuShowWindow() {
			var window = EditorWindow.GetWindow<DebugDefinesWindow>("SAGO_DEBUG");
			window.Show();
		}

		#endregion


		#region Internal Properties

		override protected string HeaderInfoText {
			get {
				return 
					"This window allows you to view, and set, the various SAGO_DEBUG preprocessor defines.\n\n" +
					"Toggling items will trigger a recompile.  It is best to select multiple and change them at once with the buttons at the bottom.";
			}
		}

		protected override List<string> SymbolPatterns {
			get {
				return new List<string>() {
					"SAGO_DEBUG_[[:alnum:]][[:alnum:]_]*",    // anything starting with SAGO_DEBUG
				};
			}
			set {}
		}

		#endregion


		#region Internal Methods

		override protected void FindDefineSymbols() {

			base.FindDefineSymbols();

			const string baseName = "SAGO_DEBUG";
			if (this.DefineSymbols.FirstOrDefault(ds => ds.Name == baseName) == null) {
				bool isDefined = DebugEditorUtils.IsSymbolDefined(baseName);
				DefineSymbol symbol = new DefineSymbol(baseName, isDefined);
				this.DefineSymbols.Insert(0, symbol);
			}

		}

		#endregion


	}
	
}
