namespace SagoApp.Content {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;	
	using System.Reflection;
	
	public class iOSMultiValueAttribute : System.Attribute {
		
		#region Fields
		
		[System.NonSerialized]
		private iOSMultiValueConfig m_Config;
		
		#endregion
		
		
		#region Properties
		
		public iOSMultiValueConfig Config {
			get {
				if (m_Config.Equals(default(iOSMultiValueConfig))) {
					MethodInfo method = this.Type.GetMethod(this.ConfigMethodName, BindingFlags.Static | BindingFlags.NonPublic);
					m_Config = (iOSMultiValueConfig)method.Invoke(null, null);
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
		
		public iOSMultiValueAttribute(System.Type type, string configMethodName) {
			this.ConfigMethodName = configMethodName;
			this.Type = type;
		}
		
		#endregion
		
	}
	
	public struct iOSMultiValueConfig {
		
		#region Fields
		
		[System.NonSerialized]
		public string TitleKey;
		
		[System.NonSerialized]
		public Dictionary<string,string> TitleLocalization;
		
		[System.NonSerialized]
		public Dictionary<string, Dictionary<string, string>>  Values;
		
		#endregion
		
	}
	
}