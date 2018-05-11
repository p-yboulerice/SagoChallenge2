/// <summary>
/// Defines various structs for serializing/deserializing for JSON
/// </summary>
namespace SagoDebug.Json {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEngine;
	using System.Reflection;
	using UnityEngine.Networking;
	using SagoUtils;


	[System.Serializable]
	public struct ExportUploadResponse {
		public int status;
		public string errorTitle;
		public string errorDescription;
	}

	[System.Serializable]
	public struct DevUI {

		public static DevUI Create(SagoDebug.DevUI devUI) {
			var json = new DevUI();

			json.productInfo = ProductInfo.Default;
			json.systemInfo = SystemInfo.Default;

			json.dateTime = System.DateTime.UtcNow.ToString("u");

			TextAsset manifest = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
			if (manifest != null) {
				json.cloudBuildManifest = JsonUtility.FromJson<CloudBuildManifest>(manifest.text);
			}

			json.consoleMessages = devUI.ConsoleMessagesCopy.Select(cm => new ConsoleMessage(cm)).ToArray();

			return json;
		}

		public ProductInfo productInfo;
		public string dateTime;
		public CloudBuildManifest cloudBuildManifest;
		public SystemInfo systemInfo;
		public ConsoleMessage[] consoleMessages;

	}

	[System.Serializable]
	public struct ProductInfo {

		public static ProductInfo Default {
			get {
				var json = new ProductInfo();
				json.applicationVersion = Application.version;
				json.productName = Application.productName;
				json.bundleIdentifier = Application.identifier;
				json.platform = Application.platform.ToString();
				return json;
			}
		}

		public string productName;
		public string platform;
		public string bundleIdentifier;
		public string applicationVersion;

	}

	[System.Serializable]
	public struct SystemInfo {

		public static SystemInfo Default {
			get {
				var json = new SystemInfo();
				json.deviceType = UnityEngine.SystemInfo.deviceType.ToString();
				json.deviceName = UnityEngine.SystemInfo.deviceName;
				json.deviceModel = UnityEngine.SystemInfo.deviceModel;
				json.operatingSystem = UnityEngine.SystemInfo.operatingSystem;
				json.deviceUniqueIdentifier = UnityEngine.SystemInfo.deviceUniqueIdentifier;

#if UNITY_IPHONE && !UNITY_EDITOR
				json.deviceSystemVersion = UnityEngine.iOS.Device.systemVersion;
				json.deviceGeneration = UnityEngine.iOS.Device.generation.ToString();
#endif
				json.graphicsDeviceName = UnityEngine.SystemInfo.graphicsDeviceName;
				json.graphicsDeviceID = UnityEngine.SystemInfo.graphicsDeviceID;
				json.graphicsDeviceType = UnityEngine.SystemInfo.graphicsDeviceType.ToString();
				json.graphicsDeviceVendor = UnityEngine.SystemInfo.graphicsDeviceVendor;
				json.graphicsDeviceVendorID = UnityEngine.SystemInfo.graphicsDeviceVendorID;
				json.graphicsDeviceVersion = UnityEngine.SystemInfo.graphicsDeviceVersion;
				json.graphicsMemorySize = UnityEngine.SystemInfo.graphicsMemorySize;
				json.graphicsMultiThreaded = UnityEngine.SystemInfo.graphicsMultiThreaded;
				json.graphicsShaderLevel = UnityEngine.SystemInfo.graphicsShaderLevel;

				json.supportedRenderTargetCount = UnityEngine.SystemInfo.supportedRenderTargetCount;
				json.supports2DArrayTextures = UnityEngine.SystemInfo.supports2DArrayTextures;
				json.supports3DTextures = UnityEngine.SystemInfo.supports3DTextures;
				json.supportsAccelerometer = UnityEngine.SystemInfo.supportsAccelerometer;
				json.supportsAudio = UnityEngine.SystemInfo.supportsAudio;
				json.supportsComputeShaders = UnityEngine.SystemInfo.supportsComputeShaders;
				json.supports2DArrayTextures = UnityEngine.SystemInfo.supportsGyroscope;
				json.supportsImageEffects = UnityEngine.SystemInfo.supportsImageEffects;
				json.supportsInstancing = UnityEngine.SystemInfo.supportsInstancing;
				json.supportsLocationService = UnityEngine.SystemInfo.supportsLocationService;
				json.supportsMotionVectors = UnityEngine.SystemInfo.supportsMotionVectors;
				json.supportsRawShadowDepthSampling = UnityEngine.SystemInfo.supportsRawShadowDepthSampling;
				json.supportsRenderToCubemap = UnityEngine.SystemInfo.supportsRenderToCubemap;
				json.supportsSparseTextures = UnityEngine.SystemInfo.supportsSparseTextures;
				json.supportsVibration = UnityEngine.SystemInfo.supportsVibration;
				return json;
			}
		}

		public string GetFields() {
			FieldInfo[] fields = this.GetType().GetFields();
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("System Info:");
			foreach (var field in fields) {
				sb.AppendFormat("\n{0}:\n\t{1}", field.Name, field.GetValue(this));
			}

			return sb.ToString();
		}

		public string deviceType;
		public string deviceName;
		public string deviceModel;
		public string deviceUniqueIdentifier;
		public string operatingSystem;

		public string deviceSystemVersion;
		public string deviceGeneration;

		public string graphicsDeviceName;
		public int graphicsDeviceID;
		public string graphicsDeviceType;
		public string graphicsDeviceVendor;
		public int graphicsDeviceVendorID;
		public string graphicsDeviceVersion;
		public int graphicsMemorySize;
		public bool graphicsMultiThreaded;
		public int graphicsShaderLevel;

		public int supportedRenderTargetCount;
		public bool supports2DArrayTextures;
		public bool supports3DTextures;
		public bool supportsAccelerometer;
		public bool supportsAudio;
		public bool supportsComputeShaders;
		public bool supportsGyroscope;
		public bool supportsImageEffects;
		public bool supportsInstancing;
		public bool supportsLocationService;
		public bool supportsMotionVectors;
		public bool supportsRawShadowDepthSampling;
		public bool supportsRenderToCubemap;
		public bool supportsSparseTextures;
		public bool supportsVibration;

	}

	[System.Serializable]
	public struct ConsoleMessage {

		public ConsoleMessage(SagoDebug.DevUI.ConsoleMessage consoleMessage) {
			this.message = consoleMessage.Message;
			this.logType = consoleMessage.LogType.ToString();
		}

		public string message;
		public string logType;

	}

	[System.Serializable]
	public struct CloudBuildManifest {

		public string projectId;
		public string bundleId;
		public string cloudBuildTargetName;
		public string buildNumber;
		public string buildStartTime;
		public string scmBranch;
		public string scmCommitId;
		public string unityVersion;
		public string xcodeVersion;

	}

	
}
