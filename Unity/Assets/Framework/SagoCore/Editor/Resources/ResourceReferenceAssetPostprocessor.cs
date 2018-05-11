namespace SagoCoreEditor.Resources {
	
	using SagoCore.Resources;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The ResourceReferenceAssetPostprocessor moves or deletes resource references when the asset they reference is moved or deleted.
	/// </summary>
	public class ResourceReferenceAssetPostprocessor : AssetPostprocessor {
		
		
		#region Event Handlers
		
		private static void OnPostprocessAllAssets(string[] importedAssetPaths, string[] deletedAssetPaths, string[] movedAssetPaths, string[] movedFromAssetPaths) {
			if (deletedAssetPaths.Length != 0 || movedFromAssetPaths.Length != 0) {
				
				var assetGuids = (
					AssetDatabase
					.FindAssets("t:ResourceReference")
					.Select(referenceGuid => ResourceReferenceEditor.ReferenceGuidToAssetGuid(referenceGuid))
				);
				
				try {
					AssetDatabase.StartAssetEditing();
					foreach (string assetGuid in deletedAssetPaths.Select(path => AssetDatabase.AssetPathToGUID(path)).Intersect(assetGuids)) {
						
						string referencePath;
						referencePath = AssetDatabase.GUIDToAssetPath(ResourceReferenceEditor.AssetGuidToReferenceGuid(assetGuid));
						
						if (!string.IsNullOrEmpty(referencePath)) {
							AssetDatabase.DeleteAsset(referencePath);
						}
						
					}
					foreach (string assetGuid in movedAssetPaths.Select(path => AssetDatabase.AssetPathToGUID(path)).Intersect(assetGuids)) {
						
						string referencePath;
						referencePath = AssetDatabase.GUIDToAssetPath(ResourceReferenceEditor.AssetGuidToReferenceGuid(assetGuid));
						
						string newPath;
						newPath = ResourceReferenceEditor.GetReferencePath(assetGuid);
						
						if (string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(referencePath, newPath))) {
							AssetDatabase.MoveAsset(referencePath, newPath);
						}
						
					}
				} finally {
					AssetDatabase.StopAssetEditing();
				}
				
			}
		}
		
		#endregion
		
		
	}
	
}
