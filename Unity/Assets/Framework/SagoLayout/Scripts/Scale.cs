namespace SagoLayout {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	public enum ScaleDevice {
		Unknown,
		Phone,
		Tablet
	}
	
	public enum ScalePreset {
		None,
		Phone,
		Tablet,
		Custom
	}
	
	public class Scale : LayoutComponent {
		
		
		#region Editor Constants
		#if UNITY_EDITOR
		
		private static readonly string DeviceKey = "SagoLayout.Scale.Device";
		
		#endif
		#endregion
		
		
		#region Editor Methods
		#if UNITY_EDITOR
		
		private static void ApplyAll() {
			foreach (var layout in FindObjectsOfType<LayoutComponent>()) {
				layout.Apply();
			}
		}
		
		[MenuItem("CONTEXT/Scale/Editor Is Phone")]
		private static void EditorIsPhone(MenuCommand command) {
			EditorPrefs.SetInt(DeviceKey, (int)ScaleDevice.Phone);
			ApplyAll();
		}
		
		[MenuItem("CONTEXT/Scale/Editor Is Tablet")]
		private static void EditorIsTablet(MenuCommand command) {
			EditorPrefs.SetInt(DeviceKey, (int)ScaleDevice.Tablet);
			ApplyAll();
		}
		
		#endif
		#endregion
		
		
		#region Static Properties
		
		public static ScaleDevice Device {
			get {
				#if UNITY_EDITOR
					return (ScaleDevice)EditorPrefs.GetInt(DeviceKey, (int)ScaleDevice.Phone);
				#else
					if (DeviceSizeInInches.y <= PhoneSizeInInches.y) {
						return ScaleDevice.Phone;
					} else if (DeviceSizeInInches.y <= TabletSizeInInches.y) {
						return ScaleDevice.Tablet;
					}
					return ScaleDevice.Tablet;
				#endif
			}
		}
		
		public static Vector2 DeviceSizeInInches {
			get {
				float height = Mathf.Min(Screen.width, Screen.height);
				float width = Mathf.Max(Screen.width, Screen.height);
				float dpi = Screen.dpi;
				return new Vector2(width, height) / dpi;
			}
		}
		
		public static Vector2 PhoneSizeInInches {
			get {
				// iPhone 6 Plus
				float height = 1080;
				float width = 1980;
				float dpi = 401;
				return new Vector2(width, height) / dpi;
			}
		}
		
		public static Vector2 TabletSizeInInches {
			get {
				// iPad Air
				float height = 1536;
				float width = 2048;
				float dpi = 264;
				return new Vector2(width, height) / dpi;
			}
		}
		
		#endregion
		
		
		#region Fields
		
		[System.NonSerialized]
		protected Align m_Align;
		
		[SerializeField]
		protected float[] m_Factors;
		
		[SerializeField]
		protected ScalePreset m_Preset;
		
		#endregion
		
		
		#region Properties
		
		public Align Align {
			get {
				m_Align = m_Align ?? GetComponent<Align>();
				return m_Align;
			}
			set {
				m_Align = value;
			}
		}
		
		public float Factor {
			get {
				if (m_Factors != null && m_Factors.Length == System.Enum.GetValues(typeof(ScaleDevice)).Length) {
					return m_Factors[(int)Device];
				}
				return 1f;
			}
		}
		
		#endregion
		
		
		#region LayoutComponent Methods
		
		override public void Apply() {
			
			Transform.localScale = Vector3.one * Factor;
			
			if (Align) {
				Align.Apply();
			}
			
		}
		
		override protected void Reset() {
			base.Reset();
			m_Preset = ScalePreset.None;
			m_Factors = new float[] { 1f, 1f, 1f };
		}
		
		#endregion
		
		
	}
	
}
