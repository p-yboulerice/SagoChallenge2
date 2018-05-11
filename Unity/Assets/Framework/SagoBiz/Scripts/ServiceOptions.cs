namespace SagoBiz {
	
	using UnityEngine;
	using UnityEngine.Serialization;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// The ServiceOptions class provides options for the <see cref="Service" /> component.
	/// </summary>
	public class ServiceOptions : MonoBehaviour{
		
		/// <summary>
		/// The mode the <see cref="Service" /> component will use when loading data.
		/// </summary>
		[FormerlySerializedAs("Mode")]
		[SerializeField]
		private ServiceMode m_Mode;

		public ServiceMode Mode {
			get {
				// Adding these define symbols overrides m_Mode serialized field on target platform prefab.
				#if SAGO_SERVICE_DISABLED
					return ServiceMode.Disabled;
				#elif SAGO_SERVICE_DEVELOPMENT
					return ServiceMode.Development;
				#elif SAGO_SERVICE_STAGING
					return ServiceMode.Staging;
				#elif SAGO_SERVICE_PRODUCTION
					return ServiceMode.Production;
				#endif

				return m_Mode;
			}
		}
		
		/// <see cref="MonoBehaviour.Reset" />
		void Reset() {
			this.m_Mode = ServiceMode.Development;
		}
		
	}
	
}