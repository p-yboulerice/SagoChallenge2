namespace Juice.Utils {

	using UnityEngine;

	public class ImplementsAttribute : PropertyAttribute {


		#region Properties

		public System.Type ObjectType {
			get;
			protected set;
		}

		public System.Type InterfaceType {
			get;
			protected set;
		}

		#endregion


		#region Constructor

		public ImplementsAttribute(System.Type objectType, System.Type interfaceType) {
			this.ObjectType = objectType;
			this.InterfaceType = interfaceType;
		}

		#endregion


	}

}
