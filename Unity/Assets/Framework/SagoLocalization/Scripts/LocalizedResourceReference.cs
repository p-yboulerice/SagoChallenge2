namespace SagoLocalization {
	
	using SagoCore.Resources;
	using System.Collections.Generic;
	using UnityEngine;
	
	public abstract class LocalizedResourceReferenceBase : ScriptableObject, IResourceReference, ISerializationCallbackReceiver {
		
		
		#region Types
		
		public struct Info {
			
			
			#region Constructors
			
			public Info(string identifier, string guid) {
				Identifier = identifier;
				Guid = guid;
				ResourceReference = null;
			}
			
			#endregion
			
			
			#region Fields
			
			public string Identifier;
			
			public string Guid;
			
			public ResourceReference ResourceReference;
			
			#endregion
			
			
			#region Properties
			
			public bool IsValid {
				get {
					return (
						!string.IsNullOrEmpty(Identifier) && 
						!string.IsNullOrEmpty(Guid)
					);
				}
			}
			
			#endregion
			
			
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private Dictionary<string,Info> m_Dictionary;
		
		[SerializeField]
		private List<string> m_Guids;
		
		[SerializeField]
		private List<string> m_Identifiers;
		
		#endregion
		
		
		#region Properties
		
		public string[] Guids {
			get {
				return (
					m_Guids != null ? 
					m_Guids.ToArray() : 
					new string[0]
				);
			}
		}
		
		public string[] Identifiers {
			get {
				return (
					m_Identifiers != null ? 
					m_Identifiers.ToArray() : 
					new string[0]
				);
			}
		}
		
		#endregion
		
		
		#region IResourceReference Properties
		
		public string Guid {
			get { return GetInfo(Locale.Current).Guid; }
		}
		
		public string AssetBundleName {
			get { return ResourceMap.GetAssetBundleName(Guid); }
		}
		
		public string AssetPath {
			get { return ResourceMap.GetAssetPath(Guid); }
		}
		
		public string ResourcePath {
			get { return ResourceMap.GetResourcePath(Guid); }
		}
		
		#endregion
		
		
		#region ISerializationCallbackReceiver Methods
		
		public void OnBeforeSerialize() {
			
		}
		
		public void OnAfterDeserialize() {
			
			m_Guids = m_Guids ?? new List<string>();
			m_Identifiers = m_Identifiers ?? new List<string>();
			m_Dictionary = m_Dictionary ?? new Dictionary<string,Info>();
			
			m_Dictionary.Clear();
			for (int index = 0; index < Mathf.Min(m_Identifiers.Count, m_Guids.Count); index++) {
				var info = new Info(m_Identifiers[index], m_Guids[index]);
				m_Dictionary[info.Identifier] = info;
			}
			
		}
		
		#endregion
		
		
		#region ScriptableObject Methods
		
		virtual public void Reset() {
			
			m_Dictionary = m_Dictionary ?? new Dictionary<string,Info>();
			m_Dictionary.Clear();
			
			m_Guids = m_Guids ?? new List<string>();
			m_Guids.Clear();
			
			m_Identifiers = m_Identifiers ?? new List<string>();
			m_Identifiers.Clear();
			
			for (int index = 0; index < Language.DefaultLanguages.Length; index++) {
				var info = new Info(Language.DefaultLanguages[index].Identifier, null);
				m_Dictionary.Add(info.Identifier, info);
				m_Identifiers.Add(info.Identifier);
				m_Guids.Add(info.Guid);
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		public Info GetInfo(string identifier) {
			var info = default(Info);
			if (m_Dictionary != null && !string.IsNullOrEmpty(identifier)) {
				m_Dictionary.TryGetValue(identifier, out info);
			}
			return info;
		}
		
		public Info GetInfo(Language language) {
			var info = default(Info);
			info = GetInfo(language.Identifier);
			return info;
		}
		
		public Info GetInfo(Locale locale) {
			var info = default(Info);
			for (int index = 0; index < locale.PreferredLanguages.Length && !info.IsValid; index++) {
				info = GetInfo(locale.PreferredLanguages[index]);
			}
			return info;
		}
		
		public ResourceReference GetResourceReference(string identifier) {
			return GetResourceReference(GetInfo(identifier));
		}
		
		public ResourceReference GetResourceReference(Info info) {
			if (info.IsValid && info.ResourceReference == null) {
				info.ResourceReference = ScriptableObject.CreateInstance<ResourceReference>();
				info.ResourceReference.Guid = info.Guid;
				m_Dictionary[info.Identifier] = info;
			}
			return info.ResourceReference;
		}
		
		public ResourceReference GetResourceReference(Language language) {
			return GetResourceReference(GetInfo(language));
		}
		
		public ResourceReference GetResourceReference(Locale locale) {
			return GetResourceReference(GetInfo(locale));
		}
		
		virtual public System.Type GetResourceType() {
			return typeof(Object);
		}
		
		#endregion
		
		
	}
	
	public abstract class LocalizedResourceReference<T> : LocalizedResourceReferenceBase where T : Object {
		
		
		#region Methods
		
		public LocalizedResourceReferenceLoaderRequest<T> Load() {
			return Load(Locale.Current);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> Load(string identifier) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(identifier), false);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> Load(Language language) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(language), false);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> Load(Locale locale) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(locale), false);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> LoadAsync() {
			return LoadAsync(Locale.Current);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> LoadAsync(string identifier) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(identifier), true);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> LoadAsync(Language language) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(language), true);
		}
		
		public LocalizedResourceReferenceLoaderRequest<T> LoadAsync(Locale locale) {
			return new LocalizedResourceReferenceLoaderRequest<T>(GetResourceReference(locale), true);
		}
		
		override public System.Type GetResourceType() {
			return typeof(T);
		}
		
		#endregion
		
		
	}
	
	[CreateAssetMenu]
	public class LocalizedResourceReference : LocalizedResourceReference<Object> {
		
	}
	
}