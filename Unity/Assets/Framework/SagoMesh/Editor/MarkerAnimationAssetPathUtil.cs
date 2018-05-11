namespace SagoMeshEditor {
	
	using SagoMesh;
	using SagoMeshEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	
	public interface IMarkerAnimationAssetPathProvider {
		
		string GetMarkerAnimationAssetPath(string directoryPath, string animationId, string labelId, string markerId);
		
	}
	
	public class MarkerAnimationAssetPathUtil {
		
		#region Types
		
		private class DefaultMarkerAnimationAssetPathProvider : IMarkerAnimationAssetPathProvider {
			
			public string GetMarkerAnimationAssetPath(string directoryPath, string animationId, string labelId, string markerId) {
				
				string legacyPath = string.Format(
					"{0}/{1}_{2}_{3}_marker.asset", 
					directoryPath,
					animationId, 
					labelId, 
					markerId
				);
				
				string fixedPath = string.Format(
					"{0}/{1}_{2}_{3}_marker.asset",
					directoryPath,
					animationId,
					markerId,
					labelId
				);
				
				return !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(legacyPath)) ? legacyPath : fixedPath;
				
			}
			
		}
		
		#endregion
		
		
		#region Static Fields
		
		private static Dictionary<string,IMarkerAnimationAssetPathProvider> _MarkerAnimationAssetPathProviders;
		
		private static Dictionary<string,IMarkerAnimationAssetPathProvider> MarkerAnimationAssetPathProviders {
			get {
				if (_MarkerAnimationAssetPathProviders == null) {
					_MarkerAnimationAssetPathProviders = new Dictionary<string,IMarkerAnimationAssetPathProvider>();
					_MarkerAnimationAssetPathProviders.Add("*", new DefaultMarkerAnimationAssetPathProvider());
				}
				return _MarkerAnimationAssetPathProviders;
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void AddMarkerAnimationAssetPathProvider(string path, IMarkerAnimationAssetPathProvider provider) {
			if (!string.IsNullOrEmpty(path) && provider != null) {
				MarkerAnimationAssetPathProviders[path] = provider;
			}
		}
		
		public static void RemoveMarkerAnimationAssetPathProvider(string path) {
			if (!string.IsNullOrEmpty(path)) {
				MarkerAnimationAssetPathProviders.Remove(path);
			}
		}
		
		public static string GetMarkerAnimationAssetPath(string xmlPath, string animationId, string labelId, string markerId) {
			
			IMarkerAnimationAssetPathProvider provider;
			provider = MarkerAnimationAssetPathProviders["*"];
			
			foreach (var kvp in MarkerAnimationAssetPathProviders) {
				if (xmlPath.StartsWith(kvp.Key)) {
					provider = kvp.Value;
					break;
				}
			}
			
			return provider.GetMarkerAnimationAssetPath(xmlPath, animationId, labelId, markerId);
			
		}
		
		#endregion
		
	}
	
}