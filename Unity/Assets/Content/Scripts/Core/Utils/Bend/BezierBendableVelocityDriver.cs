namespace Juice.Utils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Juice.Utils;
	
	/// <summary>
	/// Drives the control point transforms of a BezierBendable
	/// based on velocity, with and idle sway.  This, combined with
	/// a BezierBendable, is a bit like a normal Bendable.
	/// </summary>
	[RequireComponent(typeof(BezierBendable))]
	public class BezierBendableVelocityDriver : MonoBehaviour {


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

		[SerializeField]
		protected float[] m_ControlPointScales;

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
			m_ControlPointScales = new float[]{ 0f, 0f, 0.25f, 1f };
			m_IdleFrequency = 0.1f;
			m_MaxIdleBend = 3f;
			m_MaxVelocity = 2f;
			m_MaxVelocityBend = 10f;
			m_MotionOffset = new Vector2(0f, 1f);
			m_Springyness = 30f;
			m_Damping = 0.0625f;
		}

		void Start() {
			this.StartTime = Time.time;
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

			UpdateControlPoints(this.Bend);
		}

		#endregion


		#region Internal Fields

		Vector3[] m_BaseControlPointLocalPositions;
		BezierBendable m_BezierBendable;

		#endregion


		#region Internal Properties

		bool AreControlPointScalesValid {
			get {
				return this.ControlPointScales != null && this.ControlPointScales.Length >= 4;
			}
		}

		float Bend {
			get;
			set;
		}

		float BendVelocity {
			get;
			set;
		}

		Vector3[] BaseControlPointLocalPositions {
			get {
				if (m_BaseControlPointLocalPositions == null && this.BezierBendable && this.BezierBendable.AreControlPointsValid) {
					m_BaseControlPointLocalPositions = new Vector3[4];
					for (int i = 0; i < 4; ++i) {
						m_BaseControlPointLocalPositions[i] = this.transform.InverseTransformPoint(this.BezierBendable.ControlPoints[i].position);
					}
				}
				return m_BaseControlPointLocalPositions;
			}
		}

		BezierBendable BezierBendable {
			get {
				if (!m_BezierBendable) {
					m_BezierBendable = GetComponent<BezierBendable>();
				}
				return m_BezierBendable;
			}
		}

		float[] ControlPointScales {
			get { return m_ControlPointScales; }
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

		Vector3 MotionOffsetPosition {
			get {
				return this.transform.TransformPoint(m_MotionOffset);
			}
		}

		float RandomPhase {
			get;
			set;
		}

		float StartTime {
			get;
			set;
		}

		#endregion


		#region Internal Methods

		virtual protected void UpdateControlPoints(float bend) {
			if (!this.BezierBendable.AreControlPointsValid || !this.AreControlPointScalesValid) {
				return;
			}

			Vector3 prevLocal = Vector3.zero;
			for (int i = 0; i < this.ControlPointScales.Length; ++i) {
				float scale = this.ControlPointScales[i];
				if (scale != 0f) {
					Vector3 localPos = this.BaseControlPointLocalPositions[i].RotateAroundZ(prevLocal, scale * bend * this.BaseControlPointLocalPositions[i].y);
					Vector3 pos = this.transform.TransformPoint(localPos);
					this.BezierBendable.ControlPoints[i].position = pos;
					prevLocal = localPos;
				}
			}
		}

		#endregion
		
		
	}
	
}
