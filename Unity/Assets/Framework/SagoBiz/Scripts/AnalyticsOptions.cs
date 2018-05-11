namespace SagoBiz {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// AnalyticsOptions defines configurable options for the Analytics component.
	/// </summary>
	public class AnalyticsOptions : MonoBehaviour {
		
		/// <summary>
		/// Gets and sets the <see cref="AnalyticsMode" />.
		/// </summary>
		[FormerlySerializedAs("Mode")]
		[SerializeField]
		private AnalyticsMode m_Mode;

		public AnalyticsMode Mode {
			get {
				// Adding these define symbols overrides m_Mode serialized field on target platform prefab.
				#if SAGO_ANALYTICS_DISABLED
					return AnalyticsMode.Disabled;
				#elif SAGO_ANALYTICS_DEVELOPMENT
					return AnalyticsMode.Development;
				#elif SAGO_ANALYTICS_PRODUCTION
					return AnalyticsMode.Production;
				#endif

				return m_Mode;
			}
		}
		
		/// <summary>
		/// Resets the AnalyticsOptions component to the default values.
		/// </summary>
		void Reset() {
			this.m_Mode = AnalyticsMode.Development;
		}
		
	}
	
}