namespace SagoApp.Content {
	using System.Reflection;
	using UnityEngine;
	using System.Collections.Generic;
	using SagoApp.Project;

	public class iOSStandaloneOnlyAttribute : System.Attribute {

		public bool IsWorldProject {
			get { return ProjectInfo.Instance.IsStandaloneProject; }
		}

	}

}
