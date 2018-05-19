namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using SagoUtils;

	public class BounceScale : MonoBehaviour {

		#region Fields

		[SerializeField]
		private Vector3 m_BounceVector = new Vector3(1, 1, 1);

		[TweakerFloatRange(0f,1f)]
		[Range(0f,1f)]
		[SerializeField]
		private float m_Damper = 0.3f;

		[TweakerFloatRange(0f,1f)]
		[Range(0f,1f)]
		[SerializeField]
		private float m_ForceStrength = 0.5f;

		[SerializeField]
		private Vector3 m_GoToScale = new Vector3(1, 1, 1);

		[SerializeField]
		private bool m_BounceDamperRelativeToCurrentVelocity;
	
		[Disable(typeof(BounceScale), "DisableMaximunVelocity", 0, true)]
		[SerializeField]
		private float m_MaximunVelocity = 1;

		[SerializeField]
		private bool LimitScale;

		[SerializeField]
		private Vector3 MinScale = Vector3.zero;

		[SerializeField]
		private Vector3 MaxScale = Vector3.zero;

		[System.NonSerialized]
		private Transform m_Transform;


		private static bool DisableMaximunVelocity(Object obj) {
			BounceScale ob = obj as BounceScale;
			return !ob.BounceDamperRelativeToCurrentVelocity;
		}

		#endregion


		#region Properties

		public Transform Transform {
			get { 
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		public Vector3 GoToScale {
			get { return m_GoToScale; }
			set { m_GoToScale = value; }
		}

		private Vector3 CurrentScale {
			get;
			set;
		}

		private float Damper {
			get { return m_Damper; }
			set { m_Damper = value; }
		}

		public Vector3 Velocity {
			get;
			set;
		}

		private float ForceStrength {
			get { return m_ForceStrength; }
			set { m_ForceStrength = value; }
		}

		private Vector3 BounceVector {
			get { return m_BounceVector; }
		}

		private bool BounceDamperRelativeToCurrentVelocity {
			get { return m_BounceDamperRelativeToCurrentVelocity; }
		}

		private float MaximunVelocity {
			get { return m_MaximunVelocity; }
		}

		#endregion


		#region Monobehaviour Methods

		void Awake() {
			CurrentScale = Transform.localScale;
		}

		void Update() {
			Velocity *= (1 - Damper);
			Velocity += ForceStrength * (GoToScale - CurrentScale);
			CurrentScale += Velocity;

			if (this.LimitScale) {

				Vector3 scale = this.CurrentScale;

				if (this.CurrentScale.x < this.MinScale.x) {
					scale.x = this.MinScale.x;				
				}

				if (this.CurrentScale.x > this.MaxScale.x) {
					scale.x = this.MaxScale.x;
				}
					
				if (this.CurrentScale.y < this.MinScale.y) {
					scale.y = this.MinScale.y;				
				}

				if (this.CurrentScale.y > this.MaxScale.y) {
					scale.y = this.MaxScale.y;
				}

				this.CurrentScale = scale;
			}

			Transform.localScale = new Vector3(Mathf.Abs(CurrentScale.x), Mathf.Abs(CurrentScale.y), 1);
		}

		#endregion


		#region Methods

		public void Bounce(float force) {
			if (this.BounceDamperRelativeToCurrentVelocity) {
				float percent = this.Velocity.magnitude / this.MaximunVelocity;
				if (percent > 1) {
					force = 0;
				} else {
					force *= 1 - percent;
				}
			}

			Velocity += BounceVector.normalized * force;
			this.Update();
		}

		public void NormalizeBounce() {
			Velocity = Vector3.zero;
			CurrentScale = Vector3.one;
		}

		public void BounceWithDelay(float force, float delay) {
			StopAllCoroutines();
			StartCoroutine(BounceDelayed(force, delay));
		}

		public void ForceScale(Vector3 scale) {
			this.Transform.localScale = scale;
			this.CurrentScale = scale;
			this.Velocity = Vector3.zero;
		}

		#endregion


		#region Coroutines

		private IEnumerator BounceDelayed(float force, float delay) {
			yield return new WaitForSeconds(delay);
			Bounce(force);
		}

		#endregion

	}
}
