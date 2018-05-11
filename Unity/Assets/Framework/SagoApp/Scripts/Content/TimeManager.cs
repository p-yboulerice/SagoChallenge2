namespace SagoApp.Content {
	
	using UnityEngine;
	
	/// <summary>
	/// The TimeManager class allows us to store different time settings for each content submodule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class should have the same fields as <c>ProjectSettings/TimeManager.asset</c>. When 
	/// the fields in the <c>TimeManager.asset</a> change, this class should be updated to match.
	/// </para>
	/// </remarks>
	[CreateAssetMenu(menuName = "TimeManager", fileName = "TimeManager", order = 5000)]
	public class TimeManager : ScriptableObject {
		
		
		#region Default
		
		[RuntimeInitializeOnLoadMethod]
		public static void CreateDefaultTimeManager() {
			if (DefaultTimeManager == null) {
				DefaultTimeManager = ScriptableObject.CreateInstance<TimeManager>();
				DefaultTimeManager.hideFlags = HideFlags.HideAndDontSave;
				DefaultTimeManager.name = "DefaultTimeManager";
				DefaultTimeManager.Reset();
				DefaultTimeManager.Pull();
			}
		}
		
		public static TimeManager DefaultTimeManager {
			get; private set;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private float m_FixedTimestep;
		
		[SerializeField]
		private float m_MaximumAllowedTimestep;
		
		[SerializeField]
		private float m_TimeScale;
		
		#endregion
		
		
		#region Methods
		
		public void Push() {
			Time.fixedDeltaTime = m_FixedTimestep;
			Time.maximumDeltaTime = m_MaximumAllowedTimestep;
			Time.timeScale = m_TimeScale;
		}
		
		public void Pull() {
			m_FixedTimestep = Time.fixedDeltaTime;
			m_MaximumAllowedTimestep = Time.maximumDeltaTime;
			m_TimeScale = Time.timeScale;
		}
		
		private void Reset() {
			m_FixedTimestep = 0.02f;
			m_MaximumAllowedTimestep = 0.3333333f;
			m_TimeScale = 1f;
		}
		
		#endregion
		
		
	}
	
}