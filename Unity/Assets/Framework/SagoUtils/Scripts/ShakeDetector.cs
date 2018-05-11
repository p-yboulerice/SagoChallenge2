namespace SagoUtils {

	using UnityEngine;

	/// <summary>
	/// Calls a ShakeDetected event if the device is shaken and surpasses the ShakeAllowance property.
	/// </summary>
	public class ShakeDetector : MonoBehaviour {


		#region Events

		public event System.Action<ShakeDetector> ShakeDetected;

		#endregion


		#region Monobehavior Methods

		private void Start() {

			float accelerometerUpdateInterval = 1.0f / 60.0f;
			float lowPassKernelWidthInSeconds = 1.0f;

			LowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;    
			LowPassValue = Input.acceleration;

		}

		private void FixedUpdate() {

			Vector3 acceleration = Input.acceleration;
			Vector3 deltaAcc = acceleration - LowPassFilter();      
			float sqrMag = Vector3.SqrMagnitude(deltaAcc);
			 
			if (sqrMag >= ShakeAllowance && ShakeDetected != null){
				ShakeDetected(this);
			}    

		}

		#endregion


		#region Fields

		[SerializeField]
		private float m_ShakeAllowance = 5f;

		#endregion


		#region Internal Properties

		private float ShakeAllowance {
			get { return m_ShakeAllowance; }
			set { m_ShakeAllowance = value; }
		}

		private float LowPassFilterFactor {
			get;
			set;
		}

		private Vector3 LowPassValue {
			get;
			set;
		}

		#endregion


		#region Internal Methods

		private Vector3 LowPassFilter() {       
			LowPassValue = Vector3.Lerp(LowPassValue, Input.acceleration, LowPassFilterFactor);                     
			return LowPassValue;
		}

		#endregion


	}

}