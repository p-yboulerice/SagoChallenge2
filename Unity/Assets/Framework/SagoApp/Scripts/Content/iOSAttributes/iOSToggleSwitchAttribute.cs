namespace SagoApp.Content {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	
	public class iOSToggleSwitchAttribute : System.Attribute {
		
		
		#region Fields
		
		[System.NonSerialized]
		private iOSToggleSwitchConfig m_Config;
		
		#endregion
		
		
		#region Properties
		
		public iOSToggleSwitchConfig Config {
			get {
				if (m_Config.Equals(default(iOSToggleSwitchConfig))) {
					MethodInfo method = this.Type.GetMethod(this.ConfigMethodName, BindingFlags.Static | BindingFlags.NonPublic);
					m_Config = (iOSToggleSwitchConfig)method.Invoke(null, null);
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
		
		public iOSToggleSwitchAttribute(System.Type type, string configMethodName) {
			this.ConfigMethodName = configMethodName;
			this.Type = type;
		}
		
		#endregion
		
		
	}
	
	public struct iOSToggleSwitchConfig {
		
		#region Fields
		
		[System.NonSerialized]
		public bool DefaultValue;
		
		[System.NonSerialized]
		public string TitleKey;
		
		[System.NonSerialized]
		public Dictionary<string,string> TitleLocalization;
		
		#endregion
		
	}
	
}