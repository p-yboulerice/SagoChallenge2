namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class ParticleController : MonoBehaviour {


		#region Fields

		[SerializeField]
		private Transform Asset;

		[SerializeField]
		private float LifeTimeMin = 1;

		[SerializeField]
		private float LifeTimeMax = 1;

		[System.NonSerialized]
		private float LifeTime;

		[SerializeField]
		private float ScaleMaxVelocity = 1;

		[SerializeField]
		private AnimationCurve ScalePercentAnimationCurve;

		[SerializeField]
		private bool AlignToMovementDirection;

		[SerializeField]
		private bool ScaleRelativeToVelocity;

		[SerializeField]
		private Vector3 Scale_Min = Vector3.one;

		[SerializeField]
		private Vector3 Scale_Max = Vector3.one;

		[SerializeField]
		private bool RandomRotation = false;

		[SerializeField]
		private float RandomRotation_Min;

		[SerializeField]
		private float RandomRotation_Max;

		[SerializeField]
		private float RotationSpeen_Min;

		[SerializeField]
		private float RotationSpeed_Max;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody2D;

		[System.NonSerialized]
		private float CurrentTime;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private Rigidbody2D Rigidbody2D {
			get { return m_Rigidbody2D = m_Rigidbody2D ?? this.GetComponent<Rigidbody2D>(); }
		}

		private float RotationSpeed {
			get;
			set;
		}

		#endregion


		#region Methods

		void Update() {

			this.CurrentTime += Time.deltaTime;

			if (this.CurrentTime >= this.LifeTime) {		
				this.gameObject.SetActive(false);
			}

			float timePercent = this.CurrentTime / this.LifeTime;


			if (this.AlignToMovementDirection) {
				this.Transform.up = Vector3.Lerp(this.Transform.up, this.Rigidbody2D.velocity, Time.deltaTime * 8.43f);
			}

			if (this.ScaleRelativeToVelocity) {
				
				float percent = this.Rigidbody2D.velocity.magnitude / this.ScaleMaxVelocity;

				if (percent > 1) {
					percent = 1;
				}

				this.Asset.localScale = Vector3.Lerp(this.Scale_Min, this.Scale_Max, percent) * this.ScalePercentAnimationCurve.Evaluate(timePercent);

			} else {
				
				this.Asset.localScale = Vector3.one * this.ScalePercentAnimationCurve.Evaluate(timePercent);

			}

			this.Transform.eulerAngles += Vector3.forward * this.RotationSpeed * Time.deltaTime;

		}

		public void Emit(Vector3 position, Vector3 velocity) {
			this.gameObject.SetActive(true);
			this.Transform.position = position;
			this.Rigidbody2D.velocity = velocity;
			this.CurrentTime = 0;
			this.LifeTime = Random.Range(this.LifeTimeMin, this.LifeTimeMax);
			this.RotationSpeed = Random.Range(this.RotationSpeen_Min, this.RotationSpeed_Max);
			if (this.RandomRotation) {
				this.Transform.eulerAngles = new Vector3(0,0,Random.Range(this.RandomRotation_Min, this.RandomRotation_Max));
			}
			this.Update();
		}

		#endregion


	}
}