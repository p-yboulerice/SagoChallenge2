namespace SagoPhoto {
	
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using UnityEngine;
	
	/// <summary>
	/// <para>Encapsulates the platform-dependent behaviour to save a Texture
	/// to the platform's file system (e.g. for saving screenshots/photos).</para>
	/// <para>Adapted from Monsters</para>
	/// </summary>
	public static class CameraRoll {
		
		
		#region Public Methods
		
		public static void Save(Texture2D texture) {
			SaveScreenshot(texture, CameraRoll.DefaultFilename);
		}
		
		public static void Save(Texture2D texture, string pngName) {
			SaveScreenshot(texture, pngName);
		}
		
		public static string DefaultFilename {
			get {
				DateTime now = DateTime.Now;
				string filename = string.Format(
					"{0:D4}-{1:D2}-{2:D2}-{3:D2}{4:D2}{5:D2}.{6:D3}.png",
					now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
				return filename;
			}
		}
		
		#endregion
		
		
		#region Internal Methods
		
		#if UNITY_IPHONE && !UNITY_EDITOR
		
		[DllImport("__Internal")]
		private static extern void _savePhotoAtPath(string path, int deleteAfter);
		
		private static void SaveScreenshot(Texture2D texture, string pngName) {
			
			string path = string.Format("{0}/{1}", Application.persistentDataPath, pngName);
			
			byte[] data = texture.EncodeToPNG();
			File.WriteAllBytes(path, data);
			
			int deleteAfter = 1;
			
			_savePhotoAtPath(path, deleteAfter);
			
		}
		
		#elif UNITY_ANDROID && !UNITY_EDITOR


		static AndroidJavaObject i;
		static AndroidJavaObject currentActivity;

		/// <summary>
		/// Saves texture to the data folder.
		/// </summary>
		/// <param name="texture">Texture.</param>
		/// <param name="pngName">Png name.</param>
		private static void SaveScreenshot(Texture2D texture, string pngName) {
			
			string path = string.Format("{0}/{1}", Application.persistentDataPath, pngName);
			
			using (var p = new AndroidJavaClass("android.unity.plugin.SaveScreenshot") )
				i = p.CallStatic<AndroidJavaObject>("instance");
			
			byte[] data = texture.EncodeToPNG();
			File.WriteAllBytes(path, data);
			
			using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			
			object[] par = {path, currentActivity};
			i.Call("SaveImage", par);

		}
		
		#else

		private static void SaveScreenshot(Texture2D texture, string pngName) {
			byte[] data = texture.EncodeToPNG();
			File.WriteAllBytes("Assets/" + Path.GetFileName(pngName), data);
			Debug.Log("Wrote image to unity assets's path");
		}

		#endif
		
		#endregion
		
		
	}
	
}
