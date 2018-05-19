namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class SpringVector : MonoBehaviour {


		#region Fields

		[SerializeField]
		private Vector3 m_Value = Vector3.one;

		[TweakerFloatRangeAttribute(0f, 100f)]
		[Range(0f, 100f)]
		[SerializeField]
		private float m_Damper = 20f;

		[TweakerFloatRangeAttribute(0f, 5000f)]
		[Range(0f, 5000)]
		[SerializeField]
		private float m_Acceleration = 430;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		public float Acceleration {
			get { return m_Acceleration; }
			set { m_Acceleration = value; }
		}

		public float Damper {
			get { return m_Damper; }
			set { m_Damper = value; }
		}

		public Vector3 Velocity {
			get;
			set;
		}

		public Vector3 GoToVector {
			get;
			set;
		}

		public Vector3 Value {
			get { return m_Value; }
			set { m_Value = value; } 
		}

		#endregion


		#region Methods

		void Start() {
			
			this.GoToVector = this.Value;

		}

		public void UpdateVector() {

			float deltaTime = Mathf.Clamp(Time.deltaTime, 0, 0.016f);

			this.Velocity -= this.Velocity * this.Damper * deltaTime;

			Vector3 dif = this.GoToVector - this.Value; 

			this.Velocity += dif * this.Acceleration * deltaTime;

			this.Value += this.Velocity * deltaTime;

		}

		#endregion


	}
}