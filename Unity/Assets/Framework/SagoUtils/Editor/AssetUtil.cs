namespace SagoUtilsEditor {

	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	public static class AssetUtil {
		
		
		#region Public Methods
		
		public static string GetAbsolutePath(string relativePath) {
			return Path.GetFullPath(relativePath).TrimEnd(new char[]{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			});
		}
		
		public static string GetRelativePath(string absolutePath) {
			return GetRelativePath(absolutePath, Application.dataPath);
		}
		
		public static string GetRelativePath(string absolutePath, string basePath) {
			System.Uri absoluteUri = new System.Uri(GetAbsolutePath(absolutePath));
			System.Uri baseUri = new System.Uri(GetAbsolutePath(basePath));
			return System.Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).ToString());
		}
		
		public static bool CopyAsset(string srcPath, string dstPath, bool replace = false) {
			if (Directory.Exists(srcPath)) {
				if (replace || !Directory.Exists(dstPath)) {
					try {
						CopyDirectory(srcPath, dstPath);
					} catch (System.Exception) {
						Debug.LogError(string.Format("Cannot copy asset: {0} -> {1}", srcPath, dstPath));
						return false;
					}
				}
				return true;
			}
			if (File.Exists(srcPath)) {
				if (replace || !File.Exists(dstPath)) {
					try {
						CopyFile(srcPath, dstPath);
					} catch (System.Exception e) {
						Debug.LogError(string.Format("Cannot copy asset: {0} -> {1}", srcPath, dstPath));
						Debug.LogException(e);
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		public static bool SymlinkAsset(string srcPath, string dstPath, bool replace = false) {
			#if UNITY_EDITOR_OSX
				
				// check for valid paths
				if (string.IsNullOrEmpty(srcPath)) {
					Debug.LogError("srcPath is null or empty");
					return false;
				}
				if (string.IsNullOrEmpty(dstPath)) {
					Debug.Log("dstPath is null or empty");
					return false;
				}
				if (!File.Exists(srcPath) && !Directory.Exists(srcPath)) {
					Debug.LogError(string.Format("srcPath does not exist: {0}", srcPath));
					return false;
				}
				
				// check for existing symlinks
				if (!replace && File.Exists(srcPath) && File.Exists(dstPath)) {
					return true;
				}
				if (!replace && Directory.Exists(srcPath) && Directory.Exists(dstPath)) {
					return true;
				}
				
				// create parent directory
				Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
				
				// remove existing symlinks
				if (File.Exists(dstPath)) {
					File.Delete(dstPath);
				}
				if (Directory.Exists(dstPath)) {
					Directory.Delete(dstPath, false);
				}
				
				// create the symlink via the command line
				System.Diagnostics.Process p = new System.Diagnostics.Process();
				p.StartInfo.Arguments = string.Format(
					"-c 'cd \"{0}\"; ln -s \"{1}\" \"./{2}\"'",
					Path.GetDirectoryName(dstPath),
					GetRelativePath(srcPath, dstPath),
					Path.GetFileName(dstPath)
				);
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				p.StartInfo.FileName = "sh";
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();
				p.WaitForExit();
				
				// check for errors
				if (!string.IsNullOrEmpty(p.StandardError.ReadToEnd())) {
					Debug.LogError(string.Format(
						"Cannot symlink asset: {0} -> {1}", 
						srcPath, 
						dstPath
					));
					return false;
				}
				
				return true;
				
			#else
				
				Debug.LogError(string.Format(
					"Cannot symlink asset: {0} -> {1}", 
					srcPath, 
					dstPath
				));
				return false;
				
			#endif
		}
		
		public static bool UnlinkAsset(string dstPath) {
			if (File.Exists(dstPath + ".meta")) {
				File.Delete(dstPath + ".meta");
			}
			if (File.Exists(dstPath)) {
				File.Delete(dstPath);
				return true;
			}
			if (Directory.Exists(dstPath)) {
				Directory.Delete(dstPath, false);
				return true;
			}
			return true;
		}

		public static long GetAssetsSize(string[] guids) {
			long totalBytes = 0;
			if (guids != null) {
				for (int i = 0, count = guids.Length; i < count; ++i) {
					totalBytes += GetAssetSize(guids[i]);
				}
			}
			return totalBytes;
		}

		/// <summary>
		/// Gets the size of the asset on disk.  Note that the size
		/// in the build will likely be different.
		/// </summary>
		public static long GetAssetSize(string guid) {
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (!string.IsNullOrEmpty(assetPath) && File.Exists(assetPath)) {
				return new FileInfo(assetPath).Length;
			} else {
				return 0;
			}
		}

		/// <summary>
		/// Tries to determine the compressed size of the asset by
		/// looking in the project's Library folder.  Most assets will
		/// not have a compressed version, and will return 0.  If
		/// there is a compressed version (e.g. an audio file) this
		/// will return that size in bytes.
		/// </summary>
		public static long GetCompressedFileSize(string guid) {
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (!string.IsNullOrEmpty(assetPath)) {
				string p = Path.GetFullPath(string.Format("{0}../../Library/metadata/{1}/{2}.resource", 
					Application.dataPath, guid.Substring(0, 2), guid));
				if (File.Exists(p)) {
					return new FileInfo(p).Length;
				}
			}
			return 0;
		}

		/// <summary>
		/// Express a number of bytes in an appropriate number of B/KB/MB, etc.
		/// </summary>
		public static string ByteFormat(long numBytes) {
			string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
			if (numBytes == 0)
				return "0 " + units[0];
			int place = System.Convert.ToInt32(System.Math.Floor(System.Math.Log(numBytes, 1024)));
			double num = System.Math.Round(numBytes / System.Math.Pow(1024, place), 1);
			return string.Format("{0:0.##} {1}", System.Math.Sign(numBytes) * num, units[place]);
		}

		#endregion
		
		
		#region Helper Methods
		
		private static void CopyFile(string srcPath, string dstPath) {
			
			// create the directory
			Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
			
			// copy the file
			File.Copy(srcPath, dstPath, true);
			
		}
		
		private static void CopyDirectory(string srcPath, string dstPath) {
			
			// replace the directory
			if (Directory.Exists(dstPath)) {
				Directory.Delete(dstPath, true);
			}
			Directory.CreateDirectory(dstPath);
			
			// copy files
			foreach (var file in Directory.GetFiles(srcPath)) {
				File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
			}
			
			// copy subdirectories
			foreach (var dir in Directory.GetDirectories(srcPath)) {
				CopyDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
			}
			
		}

		private struct AssetInfo {
			public string Path;
			public string Guid;
			public long AssetSize;
			public long CompressedSize;
		}

		[MenuItem("Sago/Utils/Selected Asset Size")]
		private static void OutputSelectedFileSize() {
			var guids = Selection.assetGUIDs;
			if (guids != null) {
				var list = new List<AssetInfo>(guids.Length);
				long totalBytes = 0, totalCompressedBytes = 0;
				for (int i = 0, count = guids.Length; i < count; ++i) {
					var assetInfo = new AssetInfo();
					assetInfo.Path = AssetDatabase.GUIDToAssetPath(guids[i]);
					assetInfo.Guid = guids[i];
					assetInfo.AssetSize = GetAssetSize(guids[i]);
					assetInfo.CompressedSize = GetCompressedFileSize(guids[i]);
					list.Add(assetInfo);

					totalBytes += assetInfo.AssetSize;
					totalCompressedBytes += (assetInfo.CompressedSize == 0) ? assetInfo.AssetSize : assetInfo.CompressedSize;
				}

				list.Sort((a, b) => {
					var B = b.CompressedSize == 0 ? b.AssetSize : b.CompressedSize;
					var A = a.CompressedSize == 0 ? a.AssetSize : a.CompressedSize;
					return B.CompareTo(A);
				});

				var report = new System.Text.StringBuilder();
				report.AppendFormat("Total Asset Size: {0} ({1} bytes, {2} items)\nTotal using estimated compressed sizes: {3}\n\n", 
					ByteFormat(totalBytes), totalBytes, guids.Length, ByteFormat(totalCompressedBytes));
				for (int i = 0, count = list.Count; i < count; ++i) {
					var item = list[i];
					if (item.CompressedSize == 0) {
						report.AppendFormat("{0}: {1}\n", 
							ByteFormat(item.AssetSize), item.Path);
					} else {
						report.AppendFormat("{0} ({1}): {2}\n", 
							ByteFormat(item.CompressedSize), ByteFormat(item.AssetSize), item.Path);
					}
				}

				Debug.Log(report.ToString());
			}
		}

		#endregion
		
		
	}
	
}