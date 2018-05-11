namespace SagoAnimationEditor {
	
	using SagoAnimation;
	using SagoAudio;
	using UnityEditor;
	using UnityEngine;
	using System.IO;

	public class CompositeAnimationEditor : Editor {
		
		
		#region Menu Methods
		
		[MenuItem("Assets/Create/Composite Animation")]
		public static void CreateCompositeAnimationMenuItem() {
			CreateCompositeAnimation();
		}
		
		#endregion
				
		
		#region Static Methods
		
		public static CompositeAnimation CreateCompositeAnimation() {
			
			string path = EditorUtility.SaveFilePanel(
				"New Composite Animation",
				LastSaveFolder,
				"Untitled",
				"asset"
			);
		
			if (!string.IsNullOrEmpty(path)) {

				path = GetRelativePath(path, Application.dataPath);
		
				CompositeAnimation animation = ScriptableObject.CreateInstance<CompositeAnimation>();
				AudioAnimation audioAnimation = ScriptableObject.CreateInstance<AudioAnimation>();

				audioAnimation.Elements = new AudioAnimationElement[0];
				audioAnimation.FrameCount = 30;
				audioAnimation.FramesPerSecond = 12;
			
				animation.AudioAnimation = audioAnimation;

				var animationPath = path.Replace(".asset", "_comp.asset");
				AssetDatabase.CreateAsset(animation, animationPath);

				var audioAnimationPath = path.Replace(".asset", "_audio.asset");
				AssetDatabase.CreateAsset(audioAnimation, audioAnimationPath);

				AssetDatabase.SaveAssets();
				
				return AssetDatabase.LoadAssetAtPath(animationPath, typeof(CompositeAnimation)) as CompositeAnimation;
				
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
				return EditorPrefs.GetString("CompositeAnimationLastSaveFolder", "Assets");
			}
			set {
				EditorPrefs.SetString("CompositeAnimationLastSaveFolder", value);
			}
		}

		#endregion

		
	}
	
}