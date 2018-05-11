namespace SagoDebug {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class BugReport {

		#region Internal Fields

		[System.NonSerialized]
		private Dictionary<string, string> m_Sections;

		#endregion

		#region Internal Properties

		private Dictionary<string, string> Sections {
			get {
				m_Sections = m_Sections ?? new Dictionary<string, string>();
				return m_Sections;
			}
		}

		#endregion

		#region Public Methods

		public void Add(string key, object json) {
			this.Sections[key] = JsonUtility.ToJson(json);
		}

		public string ToJsonString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("{");

			KeyValuePair<string, string>[] sections = this.Sections.ToArray();
			int sectionsCount = sections.Length;
			for (int i = 0; i < sectionsCount; i++) {
				KeyValuePair<string, string> section = sections[i];
				sb.AppendFormat("\"{0}\" : {1}\n", section.Key, section.Value);

				if (i < sectionsCount - 1) {
					sb.Append(",");
					}
				}


			sb.AppendLine("}");

			return sb.ToString();
		}

		#endregion
	}

}