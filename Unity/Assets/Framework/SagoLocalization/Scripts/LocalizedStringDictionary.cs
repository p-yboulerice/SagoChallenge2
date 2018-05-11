namespace SagoLocalization {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	public class LocalizedStringDictionary : Dictionary<string,LocalizedString> {
		
		
		#region Types
		
		[System.Serializable]
		public struct Json {
			
			
			#region Constructors
			
			public Json(LocalizedString.Json[] localizedStrings) {
				this.localizedStrings = localizedStrings;
			}
			
			#endregion
			
			
			#region Fields
			
			[SerializeField]
			public LocalizedString.Json[] localizedStrings;
			
			#endregion
			
			
		}
		
		#endregion
		
		
		#region Constructors
		
		public LocalizedStringDictionary() : base() {
			
		}
		
		public LocalizedStringDictionary(Json json) : base() {
			if (json.localizedStrings != null) {
				for (int index = 0; index < json.localizedStrings.Length; index++) {
					var localizedString = new LocalizedString(json.localizedStrings[index]);
					if (!string.IsNullOrEmpty(localizedString.Key)) {
						this[localizedString.Key] = localizedString;
					}
				}
			}
		}
		
		public LocalizedStringDictionary(LocalizedString[] localizedStrings) : base() {
			if (localizedStrings != null) {
				for (int index = 0; index < localizedStrings.Length; index++) {
					var localizedString = localizedStrings[index];
					if (!string.IsNullOrEmpty(localizedString.Key)) {
						this[localizedString.Key] = localizedString;
					}
				}
			}
		}
		
		#endregion
		
		
	}
	
}