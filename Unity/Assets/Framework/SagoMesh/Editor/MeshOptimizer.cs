namespace SagoMeshEditor {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	/// <summary>
	/// Used to set the optimal compression level on a Mesh or array of Meshes. All Meshes must pass two
	/// tests after attempting to set the compression level: a check against the original Mesh for
	/// any difference between vert positions (can be tweaked with the precision value) and a check
	/// against the original for any colour difference (with zero fault tolerance).
	/// </summary> 
	public static class MeshOptimizer {


		#region Constants

		const string tempMeshPath = "Assets/mesh_optimizer_temp.asset";

		#endregion


		#region Public Methods

		/// <summary>
		/// Optimizes the mesh to the maximum level of compression safetly allowed within the precision parameter.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="maxCompressionLevel">Max compression level.</param>
		/// <param name="precision">How much tolerance for fault in the vert positions is allowed. Defaults to 1/10th of a pixel.</param>
		public static void OptimizeMesh(Mesh mesh, ModelImporterMeshCompression maxCompressionLevel, float precision) {
			if (mesh != null) {

				StartEditing();
				var compressionLevels = GetCompressionLevels(maxCompressionLevel);
				var level = Optimize(mesh, compressionLevels, precision);
				Debug.Log("Set compression level to: " + level);
				StopEditing();

			}				
		}

		/// <summary>
		/// Optimizes the mesh array to the maximum level of compression safetly allowed within the precision parameter.
		/// </summary>
		/// <param name="meshes">Meshes.</param>
		/// <param name="maxCompressionLevel">Max compression level.</param>
		/// <param name="precision">How much tolerance for fault in the vert positions is allowed. Defaults to 1/10th of a pixel.</param>
		public static void OptimizeMeshes(Mesh[] meshes, ModelImporterMeshCompression maxCompressionLevel = ModelImporterMeshCompression.High, float precision = 0.001f,  bool logBreakdown = false) {
			if (meshes != null && meshes.Length > 0) {

				StartEditing();
				var compressionLevels = GetCompressionLevels(maxCompressionLevel);

				var breakdown = new Dictionary<ModelImporterMeshCompression, int>();
				foreach (var level in compressionLevels) {
					breakdown.Add(level, 0);
				}

				for (int i = 0; i < meshes.Length; i++) {

					var mesh = meshes[i];
					var level = Optimize(mesh, compressionLevels, precision);
					breakdown[level]++;
					float progress = (float)i / (float)meshes.Length;
					string progressLabel = i + " of " + meshes.Length + " completed";
					EditorUtility.DisplayProgressBar("Compressing Meshes", progressLabel, progress);

				}

				if (logBreakdown) {
					LogMeshBreakdown(breakdown);
				}
				EditorUtility.ClearProgressBar();
				StopEditing();

			}
		}

		#endregion


		#region Private Methods

		private static void StartEditing() {
			AssetDatabase.StartAssetEditing();
		}

		private static void StopEditing() {
			AssetDatabase.SaveAssets();
			AssetDatabase.StopAssetEditing();
			AssetDatabase.DeleteAsset(tempMeshPath);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}

		private static ModelImporterMeshCompression Optimize(Mesh mesh, ModelImporterMeshCompression[] compressionLevels, float precision) {

			foreach (var level in compressionLevels) {

				if (level != ModelImporterMeshCompression.Off) {

					var meshCopy = new Mesh();
					EditorUtility.CopySerialized(mesh, meshCopy);
					MeshUtility.SetMeshCompression(meshCopy, level);

					AssetDatabase.CreateAsset(meshCopy, tempMeshPath);
					EditorUtility.UnloadUnusedAssetsImmediate();

					if (!AreMeshColoursDifferent(meshCopy, mesh) && !AreMeshVertsDifferent(meshCopy, mesh, precision)) {

						MeshUtility.SetMeshCompression(mesh, level);
						EditorUtility.SetDirty(mesh);
						return level;

					}

				}

			}

			var lastCompressionLevel = compressionLevels[compressionLevels.Length - 1];
			return lastCompressionLevel;

		}

		private static ModelImporterMeshCompression[] GetCompressionLevels(ModelImporterMeshCompression maxCompressionLevel) {

			var compressionLevels = System.Enum.GetValues(typeof(ModelImporterMeshCompression));
			var compressionLevelsList = new List<ModelImporterMeshCompression>();

			// Only trying to compress to the max specified by the user
			foreach (var compressionLevel in compressionLevels) {
				var level = (ModelImporterMeshCompression)compressionLevel;
				compressionLevelsList.Add(level);
				if (level == maxCompressionLevel) {
					break;
				}
			}

			compressionLevelsList.Reverse();
			return compressionLevelsList.ToArray();

		}

		private static bool AreMeshVertsDifferent(Mesh meshA, Mesh meshB, float precision) {

			if (meshA != null && meshB != null) {

				Vector3[] vertsA = meshA.vertices;
				Vector3[] vertsB = meshB.vertices;

				for (int meshIndex = 0; meshIndex < vertsA.Length; meshIndex++) {

					var meshAVert = vertsA[meshIndex];
					var meshBVert = vertsB[meshIndex];

					if (Vector3.Distance(meshAVert, meshBVert) > precision) {
						return true;
					}

				}

			}

			return false;

		}

		private static bool AreMeshColoursDifferent(Mesh meshA, Mesh meshB) {

			if (meshA != null && meshB != null) {

				for (int i = 0; i < meshA.colors.Length; i++) {

					var meshAColor = meshA.colors[i];
					var meshBColor = meshB.colors[i];

					if (!Color.Equals(meshAColor, meshBColor)) {
						return true;
					}

				}
			}

			return false;

		}

		private static void LogMeshBreakdown(Dictionary<ModelImporterMeshCompression, int> breakdown) {

			var sum = breakdown.Sum(b => b.Value);

			Debug.Log("Compression Level Breakdown");
			Debug.Log("---------------------------");
			foreach (var level in breakdown.Keys) {
				float percent = ((float)breakdown[level] / (float)sum) * 100f;
				Debug.Log(level + " - " + percent + " %");
			}

		}

		#endregion

	}

}