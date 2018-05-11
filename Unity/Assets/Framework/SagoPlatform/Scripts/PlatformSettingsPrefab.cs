namespace SagoPlatform {
	
	using UnityEngine;
	
	public class PlatformSettingsPrefab : MonoBehaviour {
		
		
		#region Fields
		
		[SerializeField]
		protected Platform m_Platform;
		
		#endregion
		
		
		#region Properties
		
		public Platform Platform {
			get { return m_Platform; }
			set { m_Platform = value; }
		}
		
		#endregion
		
		
	}
	
}