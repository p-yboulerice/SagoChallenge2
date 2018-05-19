namespace Juice.Utils {
	
	using UnityEngine;

	public class SubclassAttribute : PropertyAttribute {


		#region Properties

		public System.Type BaseType {
			get;
			protected set;
		}

		#endregion


		#region Constructor

		public SubclassAttribute(System.Type baseType) {
			this.BaseType = baseType;
		}

		#endregion


		#region Methods

		public static string GetSerializedName(System.Type type) {
			return (type == null) ? "" : type.AssemblyQualifiedName;
		}

		public static System.Type GetKeyType(string key) {
			return string.IsNullOrEmpty(key) ? null : System.Type.GetType(key);
		}

		#endregion


	}

}
