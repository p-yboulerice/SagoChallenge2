namespace SagoPlatformEditor {
	
	using SagoPlatform;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	
	public static class PlatformExtensionsEditor {
		
		/// <summary>
		/// The dictionary used to map platforms to build target groups.
		/// </summary>
		private static readonly Dictionary<Platform,BuildTargetGroup> BuildTargetGroupMap = new Dictionary<Platform,BuildTargetGroup> {
			{ Platform.Unknown, BuildTargetGroup.Standalone },
			{ Platform.iOS, BuildTargetGroup.iOS },
			{ Platform.GooglePlay, BuildTargetGroup.Android },
			{ Platform.GooglePlayFree, BuildTargetGroup.Android },
			{ Platform.Kindle, BuildTargetGroup.Android },
			{ Platform.KindleFreeTime, BuildTargetGroup.Android },
			{ Platform.WindowsStore, BuildTargetGroup.WSA },
			{ Platform.Nabi, BuildTargetGroup.Android },
			{ Platform.AmazonTeal, BuildTargetGroup.Android },
			{ Platform.SamsungMilk, BuildTargetGroup.Android },
			{ Platform.SmartEducation, BuildTargetGroup.Android },
			{ Platform.tvOS, BuildTargetGroup.tvOS },
			{ Platform.Bemobi, BuildTargetGroup.Android },
			{ Platform.Thales, BuildTargetGroup.Android },
			{ Platform.PanasonicEx, BuildTargetGroup.Android }
		};
		
		/// <summary>
		/// Gets build target group for the specified platform (or the default 
		/// build target group if no mapping exists).
		/// </summary>
		public static BuildTargetGroup ToBuildTargetGroup(this Platform platform) {
			return (
				BuildTargetGroupMap.ContainsKey(platform) ? 
				BuildTargetGroupMap[platform] : 
				BuildTargetGroupMap[Platform.Unknown]
			);
		}
		
		/// <summary>
		/// The dictionary used to map platforms to build targets.
		/// </summary>
		private static readonly Dictionary<Platform,BuildTarget> BuildTargetMap = new Dictionary<Platform,BuildTarget> {
			{ Platform.Unknown, BuildTarget.StandaloneOSX },
			{ Platform.iOS, BuildTarget.iOS },
			{ Platform.GooglePlay, BuildTarget.Android },
			{ Platform.GooglePlayFree, BuildTarget.Android },
			{ Platform.Kindle, BuildTarget.Android },
			{ Platform.KindleFreeTime, BuildTarget.Android },
			{ Platform.WindowsStore, BuildTarget.WSAPlayer },
			{ Platform.Nabi, BuildTarget.Android },
			{ Platform.AmazonTeal, BuildTarget.Android },
			{ Platform.SamsungMilk, BuildTarget.Android },
			{ Platform.SmartEducation, BuildTarget.Android },
			{ Platform.tvOS, BuildTarget.tvOS },
			{ Platform.Bemobi, BuildTarget.Android },
			{ Platform.Thales, BuildTarget.Android },
			{ Platform.PanasonicEx, BuildTarget.Android }
		};
		
		/// <summary>
		/// Gets build target for the specified platform (or the default build 
		/// target if no mapping exists).
		/// </summary>
		public static BuildTarget ToBuildTarget(this Platform platform) {
			return (
				BuildTargetMap.ContainsKey(platform) ? 
				BuildTargetMap[platform] : 
				BuildTargetMap[Platform.Unknown]
			);
		}
		
		
	}
	
}