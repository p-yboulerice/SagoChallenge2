namespace SagoCore.AssetBundles {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// TODO: Not sure this class will stay like this, need to add documentation when it's stable.
	/// </summary>
	public class AssetBundleOptions {
		
		
		#region Constants
		
		public const string UseAssetBundlesInEditorKey = "SagoCore.AssetBundles.AssetBundleOptions.UseAssetBundlesInEditor";
		
		#endregion
		
		
		#region Static Properties
		
		public static bool UseAssetBundlesInEditor {
			get {
				#if UNITY_EDITOR
					return UnityEditor.EditorPrefs.GetBool(UseAssetBundlesInEditorKey, false);
				#else
					return true;
				#endif
			}
			set {
				#if UNITY_EDITOR
					UnityEditor.EditorPrefs.SetBool(UseAssetBundlesInEditorKey, value);
				#endif
			}
		}
		
		public static bool UseAssetDatabaseInEditor {
			get { return !UseAssetBundlesInEditor; }
			set { UseAssetBundlesInEditor = !value; }
		}
		
		#endregion
		
		
	}
	
}