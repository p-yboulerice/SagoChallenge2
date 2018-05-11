namespace SagoApp.Project {
	
	using SagoCore.AssetBundles;
	using SagoCore.Scenes;
	using SagoNavigation;
	using UnityEngine;
	
	public class NavigateToContentSceneController : SceneController {

		#if SAGO_DEBUG

		private void OnGUI() {
			
			ProgressReport report;
			report = ProjectNavigator.Instance.ProgressReport;
			
			if (report.Count == 0 || report.Item == null) {
				return;
			}
			
			string text;
			text = null;
			
			if (report.Item is LoadAssetBundleProgressReportItem) {
				var item = report.Item as LoadAssetBundleProgressReportItem;
				if (item.AssetBundleReference != null && item.AssetBundleReference.adaptor != null) {
					text = string.Format(
						"Loading asset bundle: {0:0}% ({1}/{2}) ({3},{4})",
						item.Progress * 100f,
						report.Index + 1,
						report.Count,
						item.AssetBundleReference.assetBundleName,
						item.AssetBundleReference.adaptor.GetType().Name
					);
				}
			} else if (report.Item is LoadSceneProgressReportItem) {
				var item = report.Item as LoadSceneProgressReportItem;
				text = string.Format(
					"Loading scene: {0:0}% ({1}/{2}) ({3})",
					item.Progress * 100f,
					report.Index + 1,
					report.Count,
					item.SceneReference.ScenePath
				);
			}
			
			GUIStyle style;
			style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.black;
			style.fontSize = 18;
			
			GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
			GUILayout.FlexibleSpace();
			if (text != null) GUILayout.Label(text, style);
			GUILayout.EndVertical();
			
		}

		#endif
		
	}
	
}
