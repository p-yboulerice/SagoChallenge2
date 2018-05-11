namespace SagoAudioEditor {
	
	using SagoAudio;
	using UnityEditor;
	using UnityEngine;
	using System.IO;
	
	public class AudioAnimationEditor : Editor {
		
		
		#region Menu Methods
		
		[MenuItem("Assets/Create/Audio Animation")]
		public static void CreateAudioAnimationMenuItem() {
			CreateAudioAnimation();
		}
		
		#endregion

		
		#region Static Methods
		
		public static AudioAnimation CreateAudioAnimation() {

			string path = EditorUtility.SaveFilePanel(
				"New Audio Animation",
				LastSaveFolder,
				"Untitled",
				"asset"
			);
		
			if (!string.IsNullOrEmpty(path)) {

				path = GetRelativePath(path, Application.dataPath);

				int folderEndIndex = path.LastIndexOf("/");
				LastSaveFolder = path.Remove(folderEndIndex, path.Length - folderEndIndex);
		
				AudioAnimation animation = ScriptableObject.CreateInstance<AudioAnimation>();
				animation.Elements = new AudioAnimationElement[0];
				animation.FrameCount = 30;
				animation.FramesPerSecond = 12;
				
				AssetDatabase.CreateAsset(animation, path);
				AssetDatabase.SaveAssets();
				
				return AssetDatabase.LoadAssetAtPath(path, typeof(AudioAnimation)) as AudioAnimation;
				
			}
			
			return null;
			
		}

		private static string GetRelativePath(string absolutePath, string basePath) {

			System.Uri absoluteUri = new System.Uri(Path.GetFullPath(absolutePath));
			System.Uri baseUri = new System.Uri(Path.GetFullPath(basePath));
			return System.Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).ToString());

		}
		
		#endregion


		#region Static Properties

		private static string LastSaveFolder {
			get {
				return EditorPrefs.GetString("AudioAnimationLastSaveFolder", "Assets");
			}
			set {
				EditorPrefs.SetString("AudioAnimationLastSaveFolder", value);
			}
		}

		#endregion
		
		
	}
	
}