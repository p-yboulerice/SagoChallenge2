namespace SagoApp {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	 
	public class FramesPerSecond : MonoBehaviour  {
		
		
		#region Fields
		
		[SerializeField]
		protected Color m_Color;
		
		[System.NonSerialized]
		List<float> m_Data;
		
		[System.NonSerialized]
		protected int m_Interval;
		
		[System.NonSerialized]
		protected GUIContent m_Label;
		
		[System.NonSerialized]
		protected GUIStyle m_Style;
		
		#endregion
		
		
		#region MonoBehaviour Methods
		#if SAGO_DEBUG
		
		void OnGUI() {
			
			float scale;
			scale = Mathf.Max(Screen.dpi / 160f, 1f) / 3f;
			
			Vector2 size;
			size = m_Style.CalcSize(m_Label);
			
			Vector2 position;
			position = new Vector2(Screen.width - size.x, 0);
			
			Rect rect = new Rect();
			rect.x = position.x / scale;
			rect.y = position.y / scale;
			rect.width = size.x / scale;
			rect.height = size.y / scale;
			
			GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
			m_Style.normal.textColor = m_Color;
			GUI.Label(rect, m_Label, m_Style);
			
		}
		
		void Reset() {
			m_Color = Color.white;
		}
		
		void Start() {
			m_Data = new List<float>();
			m_Interval = 15;
			m_Label = new GUIContent(string.Empty);
			m_Style = new GUIStyle();
			m_Style.alignment = TextAnchor.UpperRight;
			m_Style.font = Resources.Load("SagoApp/FramesPerSecond", typeof(Font)) as Font;
			m_Style.normal.textColor = m_Color;
			m_Style.padding = new RectOffset(20, 20, 18, 18);

			useGUILayout = false;
		}
		
		void Update() {
			m_Data.Add(Time.deltaTime);
			if (m_Data.Count == m_Interval) {
				float target;
				string targetText;
				if (Application.targetFrameRate != -1) {
					target = Application.targetFrameRate;
					targetText = target.ToString("F0");
				} else {
					target = 60;
					targetText = "-";
				}
				float current = Mathf.Min(1f / m_Data.Average(), target);
				m_Label.text = string.Format("{0:f0} / {1}", current, targetText);
				m_Data.Clear();
			}
		}
		
		#endif
		#endregion
		
		
		#region Menu Items
		#if UNITY_EDITOR
		
		[MenuItem("CONTEXT/FramesPerSecond/Enable SAGO_DEBUG")]
		static void EnableSagoDebug(MenuCommand command) {
			SetDefineSymbol("SAGO_DEBUG", true);
		}
		
		[MenuItem("CONTEXT/FramesPerSecond/Enable SAGO_DEBUG", true)]
		static bool ValidateEnableSagoDebug(MenuCommand command) {
			#if SAGO_DEBUG
			return false;
			#else
			return true;
			#endif
		}
		
		[MenuItem("CONTEXT/FramesPerSecond/Disable SAGO_DEBUG")]
		static void DisableSagoDebug(MenuCommand command) {
			SetDefineSymbol("SAGO_DEBUG", false);
		}
		
		[MenuItem("CONTEXT/FramesPerSecond/Disable SAGO_DEBUG", true)]
		static bool ValidateDisableSagoDebug(MenuCommand command) {
			#if !SAGO_DEBUG
			return false;
			#else
			return true;
			#endif
		}
		
		static void SetDefineSymbol(string defineSymbol, bool enabled) {
			
			string delimiter;
			delimiter = ";";
				
			BuildTargetGroup buildTargetGroup;
			buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			
			string value;
			value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			
			HashSet<string> hash;
			hash = new HashSet<string>(value.Split(delimiter.ToCharArray()));
			
			if (enabled) {
				hash.Add(defineSymbol.ToString());
			} else {
				hash.Remove(defineSymbol.ToString());
			}
			
			value = string.Join(delimiter, hash.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, value);
			
		}
		
		#endif
		#endregion
		
		
	}
	
}
