namespace SagoApp.Content {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	
	public class iOSSettingsGroupAttribute : System.Attribute {
		
		
		#region Fields
		
		[System.NonSerialized]
		private iOSSettingsGroupConfig m_Config;
		
		#endregion
		
		
		#region Properties
		
		public iOSSettingsGroupConfig Config {
			get {
				if (m_Config.Equals(default(iOSSettingsGroupConfig))) {
					MethodInfo method = this.Type.GetMethod(this.ConfigMethodName, BindingFlags.Static | BindingFlags.NonPublic);
					m_Config = (iOSSettingsGroupConfig)method.Invoke(null, null);
				}
				return m_Config;
			}
		}
		
		private string ConfigMethodName {
			get;
			set;
		}
		
		private System.Type Type {
			get;
			set;
		}
		
		#endregion
		
		
		#region Constructor
		
		public iOSSettingsGroupAttribute(System.Type type, string configMethodName) {
			this.ConfigMethodName = configMethodName;
			this.Type = type;
		}
		
		#endregion
		
	}

	public struct iOSSettingsGroupConfig {
		
		#region Fields
		
		[System.NonSerialized]
		public int Priority;
		
		[System.NonSerialized]
		public string TitleKey;
		
		[System.NonSerialized]
		public Dictionary<string,string> TitleLocalization;
		
		#endregion
		
	}

}