namespace SagoMeshEditor {

	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using SagoEngine;

	/// <summary>
	/// Used to set the optimal compression level on a folder containing Meshes or other files
	/// that are containers for Meshes ie MeshAnimationAtlas & MeshAsset. All Meshes must pass two
	/// tests after attempting to set the compression level: a check against the original Mesh for
	/// any difference between vert positions (can be tweaked with the precision value) and a check
	/// against the original for any colour difference (with zero fault tolerance).
	/// </summary> 
	public class MeshOptimizerWindow : EditorWindow {


		#region Editor Window Methods

		[MenuItem("Window/Mesh Optimizer")]
		private static void Init() {
			
			MeshOptimizerWindow window = EditorWindow.GetWindow<MeshOptimizerWindow>("Mesh Optimizer", true);
			window.Show();

		}

		private void OnGUI() {

			// Settings label
			GUILayout.Label("Settings", EditorStyles.boldLabel);

			// Compression Level
			m_MaxCompressionLevel = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Max Compression Level", m_MaxCompressionLevel);

			// Target root toggle
			m_IsTargetingRootFolder = EditorGUILayout.Toggle("Target Root Folder", m_IsTargetingRootFolder);

			// Target folder
			this.UpdateTargetFolderGUI();

			// Precision
			m_Precision = EditorGUILayout.FloatField("Precision Check", m_Precision);

			// Info box
			this.UpdateInfoBoxGUI();

			// Optimize button
			this.UpdateOptimizeButtonGUI();

		}

		#endregion


		#region GUI Methods

		private void UpdateTargetFolderGUI() {

			// This is a hack as Unity doesn't have a method for folder fields
			UnityEngine.Object targetFolder = null;

			if (!m_IsTargetingRootFolder) {
				targetFolder = EditorGUILayout.ObjectField("Target folder", m_TargetFolder, typeof(UnityEngine.Object), false);
			}

			m_TargetPath = AssetDatabase.GetAssetPath(targetFolder);
			if (AssetDatabase.IsValidFolder(m_TargetPath)) {
				m_TargetFolder = targetFolder;
			}

			m_TargetPath = (m_IsTargetingRootFolder) ? "Assets" : AssetDatabase.GetAssetPath(m_TargetFolder);

			if (m_TargetPath != m_PreviousTargetPath) {
				m_PreviousTargetPath = m_TargetPath;
				this.FindMeshes();
			}

		}

		private void UpdateInfoBoxGUI() {

			EditorGUILayout.Space();

			var infoStyle = new GUIStyle(GUI.skin.box);
			infoStyle.border = new RectOffset(1,1,1,1);
			infoStyle.padding = new RectOffset(10,10,10,10);
			infoStyle.margin = new RectOffset(10,10,10,10);

			var reportStyle = new GUIStyle(GUI.skin.label);
			reportStyle.richText = true;

			GUILayout.BeginVertical(infoStyle);

			if (string.IsNullOrEmpty(m_TargetPath)) {

				EditorGUILayout.LabelField("Please select a target folder or toggle the 'Target Root Folder' checkbox");

			} else {

				float fileSizeInMegabytes = (float)m_TotalFileSize / (float)1000000;
				fileSizeInMegabytes = Mathf.Round(fileSizeInMegabytes * 1000f) / 1000f;

				EditorGUILayout.LabelField("Total Size", "<b><color=white>" + fileSizeInMegabytes + " MB" + "</color></b>", reportStyle);
				EditorGUILayout.LabelField("MeshAnimationAtlases", "<b><color=white>" + m_MeshAtlasFiles.Length.ToString() + "</color></b>", reportStyle);
				EditorGUILayout.LabelField("Meshes", "<b><color=white>" + m_TotalNumberOfMeshes.ToString() + "</color></b>", reportStyle);
				EditorGUILayout.LabelField("MeshAssets", "<b><color=white>" + m_MeshAssetFiles.Length.ToString() + "</color></b>", reportStyle);

			}

			GUILayout.EndVertical();
			EditorGUILayout.Space();

		}

		private void UpdateOptimizeButtonGUI() {

			EditorGUI.BeginDisabledGroup(m_MeshAtlasFiles.Length == 0);

			var buttonText = new GUIContent("Optimize Meshes");
			var rect = GUILayoutUtility.GetRect(buttonText, GUI.skin.button, GUILayout.Width(220), GUILayout.Height(25));
			rect.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rect.center.y);

			if (GUI.Button(rect, buttonText) && m_MeshAtlasFiles.Length != 0) {

				this.CompressMeshes();
				this.FindMeshes();

			}

			EditorGUI.EndDisabledGroup();

		}

		#endregion


		#region Methods

		private void CompressMeshes() {

			m_Meshes = new List<Mesh>();

			GatherMeshesFromMeshAtlases(m_MeshAtlasFiles);
			GatherMeshesFromMeshAssets(m_MeshAssetFiles);
			GatherMeshesFromMeshFiles(m_MeshFiles);

			MeshOptimizer.OptimizeMeshes(m_Meshes.ToArray(), m_MaxCompressionLevel, m_Precision, true);

			m_Meshes = null;
			EditorUtility.UnloadUnusedAssetsImmediate();

		}

		private void GatherMeshesFromMeshAtlases(string[] files) {

			for (int i = 0; i < files.Length; i++) {
				
				var file = files[i];
				var path = AssetDatabase.GUIDToAssetPath(file);

				foreach (Object atlasAsset in AssetDatabase.LoadAllAssetsAtPath(path)) {
					
					if (atlasAsset is Mesh) {						
						var mesh = atlasAsset as Mesh;
						m_Meshes.Add(mesh);
					}

				}

			}

		}

		private void GatherMeshesFromMeshAssets(string[] files) {

			foreach (var file in files) {

				var path = AssetDatabase.GUIDToAssetPath(file);
				var meshAsset = AssetDatabase.LoadAssetAtPath<MeshAsset>(path);
				var mesh = meshAsset.Mesh;

				if (mesh != null) {
					m_Meshes.Add(mesh);
				}

			}

		}

		private void GatherMeshesFromMeshFiles(string[] files) {

			foreach (var file in files) {

				var path = AssetDatabase.GUIDToAssetPath(file);
				var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

				if (mesh != null) {
					m_Meshes.Add(mesh);
				}

			}

		}

		private void FindMeshes() {

			m_TotalNumberOfMeshes = 0;
			m_TotalFileSize = 0;
			m_MeshAtlasFiles = new string[0];
			m_MeshAssetFiles = new string[0];
			m_MeshFiles = new string[0];
			
			if (AssetDatabase.IsValidFolder(m_TargetPath)) {
			
				var searchFolder = new string[] { m_TargetPath };
				m_MeshAtlasFiles = AssetDatabase.FindAssets("t:MeshAnimationAtlas", searchFolder);
				m_MeshAssetFiles = AssetDatabase.FindAssets("t:MeshAsset", searchFolder);
				m_MeshFiles = AssetDatabase.FindAssets("t:Mesh", searchFolder).Distinct().ToArray();
				m_MeshFiles = m_MeshFiles.Where(x => !m_MeshAtlasFiles.Contains(x)).ToArray();
			
				// Adding together the number of meshes and the total file size
			
				foreach (var file in m_MeshAtlasFiles) {
			
					var path = AssetDatabase.GUIDToAssetPath(file);
					var fullPath = Path.GetFullPath(path);
					var fileInfo = new System.IO.FileInfo(fullPath);
			
					m_TotalFileSize += fileInfo.Length;
					// Minus 1 because the container shouldn't be counted 
					m_TotalNumberOfMeshes += AssetDatabase.LoadAllAssetsAtPath(path).Length - 1;
			
				}
			
				// Adding on the MeshAsset files to the file size
			
				foreach (var file in m_MeshAssetFiles) {
			
					var path = AssetDatabase.GUIDToAssetPath(file);
					var fullPath = Path.GetFullPath(path);
					var fileInfo = new System.IO.FileInfo(fullPath);
			
					m_TotalFileSize += fileInfo.Length;
					m_TotalNumberOfMeshes++;
			
				}

				// Adding on the Mesh files to the file size

				foreach (var file in m_MeshFiles) {

					var path = AssetDatabase.GUIDToAssetPath(file);
					var fullPath = Path.GetFullPath(path);
					var fileInfo = new System.IO.FileInfo(fullPath);

					m_TotalFileSize += fileInfo.Length;
					m_TotalNumberOfMeshes++;

				}

			}

		}

		#endregion


		#region Fields

		private ModelImporterMeshCompression m_MaxCompressionLevel = ModelImporterMeshCompression.High;

		private UnityEngine.Object m_TargetFolder;

		private bool m_IsTargetingRootFolder = false;

		private float m_Precision = 0.001f; // Defaults to 1/10th of a pixel

		private int m_TotalNumberOfMeshes = 0;

		private long m_TotalFileSize = 0;

		private string m_TargetPath;

		private string m_PreviousTargetPath;

		private string[] m_MeshAtlasFiles = new string[0];

		private string[] m_MeshAssetFiles = new string[0];

		private string[] m_MeshFiles = new string[0];

		private List<Mesh> m_Meshes = new List<Mesh>();

		#endregion


	}

}