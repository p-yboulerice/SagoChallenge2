namespace Juice.Utils {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	/// <summary>
	/// Drives a Bend shader using an idle sway and velocity tracking
	/// </summary>
	public class Bendable : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		protected float m_IdleFrequency;

		[SerializeField]
		protected float m_MaxIdleBend;

		[SerializeField]
		protected float m_MaxVelocity;

		[SerializeField]
		protected float m_MaxVelocityBend;

		[SerializeField]
		protected Vector2 m_MotionOffset;

		[SerializeField]
		protected float m_Springyness;

		[SerializeField]
		protected float m_Damping;

		#endregion


		#region Public Methods

		public void AddBendImpulse(float normalizedBendScale) {
			normalizedBendScale = Mathf.Clamp(normalizedBendScale, -1f, 1f);
			this.BendVelocity += normalizedBendScale * m_MaxVelocityBend;
			this.Bend += normalizedBendScale * m_MaxVelocityBend;
		}

		#endregion


		#region MonoBehaviour

		public void Reset() {
			m_IdleFrequency = 0.1f;
			m_MaxIdleBend = 3f;
			m_MaxVelocity = 2f;
			m_MaxVelocityBend = 10f;
			m_MotionOffset = new Vector2(0f, 1f);
			m_Springyness = 30f;
			m_Damping = 0.0625f;
		}

		void Start() {
			if (!this.Renderer || !this.Material || !this.Material.HasProperty("Bend")) {
				Debug.LogWarning("Bendable not set up correctly; needs renderer and material with 'Bend' property");
				this.enabled = false;
				return;
			}

			this.StartTime = Time.time;
			this.BendPropertyId = Shader.PropertyToID("Bend");
			this.RandomPhase = 0f;  // Random.Range(0f, 5f);
			this.LastMotionOffsetPosition = this.MotionOffsetPosition;
		}

		void Update() {

			Vector3 motionOffsetPosition = this.transform.TransformPoint(m_MotionOffset);
			Vector3 velocity = (motionOffsetPosition - this.LastMotionOffsetPosition) / Time.deltaTime;
			Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);

			this.LastMotionOffsetPosition = motionOffsetPosition;

			float velocityBend = localVelocity.x / m_MaxVelocity * m_MaxVelocityBend;

			float targetBend = velocityBend + this.IdleBend;
			targetBend = Mathf.Sign(targetBend) * Mathf.Min(Mathf.Abs(targetBend), m_MaxVelocityBend);

			this.BendVelocity += (targetBend - this.Bend) * Time.deltaTime * m_Springyness;
			this.BendVelocity *= Mathf.Pow(1.0f - m_Damping, Time.deltaTime * 60f);

			this.Bend += this.BendVelocity * Time.deltaTime;

			this.Material.SetFloat(BendPropertyId, this.Bend);
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = new Color(1f, 0.5f, 0.5f, 1f);
			Gizmos.DrawWireSphere(this.transform.TransformPoint(m_MotionOffset), 0.1f);
		}

		#endregion


		#region Internal Fields

		Material m_Material;
		Renderer m_Renderer;

		#endregion


		#region Internal Properties

		float Bend {
			get;
			set;
		}

		int BendPropertyId {
			get;
			set;
		}

		float BendVelocity {
			get;
			set;
		}

		float IdleBend {
			get {
				return m_MaxIdleBend * 
					Mathf.Sin((Time.time - this.StartTime) * Mathf.PI * 2f * m_IdleFrequency + this.RandomPhase);
			}
		}

		Vector3 LastMotionOffsetPosition {
			get;
			set;
		}

		Material Material {
			get {
				if (m_Material == null && this.Renderer) {
				    m_Material = this.Renderer.material;
				}
				return m_Material;
			}
		}

		Vector3 MotionOffsetPosition {
			get {
				return this.transform.TransformPoint(m_MotionOffset);
			}
		}

		float RandomPhase {
			get;
			set;
		}

		Renderer Renderer {
			get {
				m_Renderer = m_Renderer ?? GetComponentInChildren<Renderer>();
				return m_Renderer;
			}
		}

		float StartTime {
			get;
			set;
		}

		#endregion

	}
	
}
