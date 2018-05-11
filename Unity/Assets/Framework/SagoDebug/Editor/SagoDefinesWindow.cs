namespace SagoDebugEditor {

	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// For operating on the various Sago defines.
	/// Add regular expression patterns to the SymbolPatterns
	/// property below.
	/// </summary>
	public class SagoDefinesWindow : DefinesWindow {


		#region Menu

		[MenuItem ("Sago/Utils/Sago Define Symbols...", false, 152)]
		private static void MenuShowWindow() {
			var window = EditorWindow.GetWindow<SagoDefinesWindow>("Defines");
			window.Show();
		}

		#endregion


		#region Internal Properties

		/// <summary>
		/// Gets or sets the symbol patterns.  Update these based on
		/// https://docs.google.com/document/d/1fW7F6rfcew4aauNEpj83zLGoi8QkG6oi-foG7Q7CZRc/edit
		/// </summary>
		/// <value>The symbol patterns.</value>
		protected override List<string> SymbolPatterns {
			get {
				return new List<string>() {
					"SAGO_[[:alnum:]][[:alnum:]_]*",    // anything starting with SAGO_
					"STORE_[[:alnum:]][[:alnum:]_]*",    // anything starting with STORE_
					"VIDEO_BUILD",
					"IS_ADHOC"
				};
			}
			set {}
		}

		#endregion


	}

}
