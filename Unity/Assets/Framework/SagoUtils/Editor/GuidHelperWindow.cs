namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	
	/// <summary>
	/// Simple UI to find an Asset by GUID, or the GUID of an Asset.
	/// </summary>
	public class GuidHelperWindow : EditorWindow {
		

		#region Properties

		protected string GUID { get; set; }

		protected Object Asset { get; set; }

		protected string InfoOutput { get; set; }

		#endregion


		#region EditorWindow

		[MenuItem ("Sago/Utils/GUID Helper")]
		public static void CreateWindow() {
			GuidHelperWindow win = EditorWindow.GetWindow<GuidHelperWindow>(true, "GUID Helper", true);
			if (Selection.objects != null && Selection.objects.Length > 0) {
				win.Asset = Selection.objects[0];
				win.CheckAsset();
			}
		}

		protected void Awake() {
			this.GUID = "";
			this.Asset = null;
			this.InfoOutput = DefaultMessage;
		}

		protected void OnGUI() {
			
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			this.GUID = EditorGUILayout.TextField("GUID", this.GUID);
			if (EditorGUI.EndChangeCheck()) {
				CheckGUID();
			}

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			this.Asset = EditorGUILayout.ObjectField("Asset", this.Asset, typeof(Object), false);
			if (EditorGUI.EndChangeCheck()) {
				CheckAsset();
			}

			EditorGUILayout.Space();

			GUILayout.TextArea(this.InfoOutput);

		}

		#endregion


		#region Internal Methods

		const string DefaultMessage = "Enter a GUID, or select an asset";


		protected void CheckGUID() {
			string path = AssetDatabase.GUIDToAssetPath(this.GUID);
			if (!string.IsNullOrEmpty(path)) {
				this.Asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				this.InfoOutput = string.Format("Object: {0} at {1} has GUID: {2}",
					this.Asset.name, path, this.GUID);
			} else {
				this.Asset = null;
				this.InfoOutput = DefaultMessage;
			}
		}

		protected void CheckAsset() {
			if (this.Asset) {
				string path = AssetDatabase.GetAssetPath(this.Asset);
				if (!string.IsNullOrEmpty(path)) {
					string guid = AssetDatabase.AssetPathToGUID(path);
					this.GUID = guid;
					this.InfoOutput = string.Format("GUID: {0} for '{1}'", this.GUID, this.Asset.name);
				} else {
					this.InfoOutput = string.Format("No GUID for '{0}' (not an asset?)", this.Asset.name);
				}
			} else {
				this.GUID = "";
				this.InfoOutput = DefaultMessage;
			}
		}

		#endregion


	}
	
}
