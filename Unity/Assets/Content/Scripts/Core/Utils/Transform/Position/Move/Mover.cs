namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public enum MoverUpdateMode {
		Update,
		LateUpdate,
		FixedUpdate
	}

	public enum MoveMode {
		Transform,
		Rigidbody
	}

	public class Mover : MonoBehaviour {

		#region Fields

		[SerializeField]
		private Transform m_MoveTransform;

		[SerializeField]
		private Transform m_GoToTarget;

		[SerializeField]
		private Space m_MoveSpace = Space.World;
		
		[SerializeField]
		private MoveMode m_MoveMode;

		[SerializeField]
		private MoverUpdateMode m_UpdateMode;
	
		[TweakerFloatRangeAttribute(0f, 100f)]
		[Range(0f, 100f)]
		[SerializeField]
		private float m_Damper = 20f;

		[TweakerFloatRangeAttribute(0f, 5000f)]
		[Range(0f, 5000)]
		[SerializeField]
		private float m_Acceleration = 430;

		[TweakerAttribute]
		[SerializeField]
		private bool UseMaximunVelocity = false;

		[TweakerAttribute]
		[SerializeField]
		private float MaximunVelocity;

		[SerializeField]
		public float AtPositionDistance = 0.1f;

		[SerializeField]
		public float AtPositionVelocity = 0.1f;

		[SerializeField]
		public bool LockZAxis = true;

		public event System.Action<Mover> OnBeforeMoverIsUpdated;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private Vector3 m_GoToPosition;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public Transform MoveTransform {
			get { return m_MoveTransform = m_MoveTransform ?? this.Transform; }
			private set { m_MoveTransform = value; }
		}

		public Vector3 Velocity {
			get;
			set;
		}

		public Vector3 GoToPosition {
			get { 
				if (this.GoToTarget != null) {
					m_GoToPosition = this.MoveSpace == Space.Self ? m_GoToTarget.localPosition : m_GoToTarget.position;
				}
				return m_GoToPosition; 
			}
			set {
				m_GoToPosition = value;
			}
		}

		public float Acceleration {
			get { return m_Acceleration; }
			set { m_Acceleration = value; }
		}

		public float Damper {
			get { return m_Damper; }
			set { m_Damper = value; }
		}

		public Space MoveSpace {
			get { return m_MoveSpace; }
			set { m_MoveSpace = value; }
		}

		public MoverUpdateMode UpdateMode {
			get { return m_UpdateMode; }
		}

		public bool AtPosition {
			get {
				Vector3 dif;
				if (this.MoveSpace == Space.Self) {
					dif = this.Transform.localPosition - this.GoToPosition;
				} else {
					dif = this.Transform.position - this.GoToPosition;
				}
				dif.z = 0;
				return this.Velocity.magnitude <= this.AtPositionVelocity && dif.magnitude <= this.AtPositionDistance;
			}
		}

		public Rigidbody2D Rigidbody {
			get { return m_Rigidbody = m_Rigidbody ?? this.GetComponent<Rigidbody2D>(); }
		}

		public Transform GoToTarget {
			get { return m_GoToTarget; }
			set { m_GoToTarget = value; }
		}

		public Vector3 Position {
			get { return this.MoveTransform.position; }
			set { this.MoveTransform.position = value; }
		}

		public MoveMode MoveMode {
			get { return m_MoveMode; }
			set { m_MoveMode = value; }
		}

		#endregion


		#region Methods

		void Reset() {
			this.MoveTransform = this.MoveTransform;
		}

		void Awake() {
			if (this.MoveSpace == Space.Self) {
				this.ForcePosition(this.MoveTransform.localPosition);
			} else {
				this.ForcePosition(this.MoveTransform.position);
			}
		}

		void Update() {
			if (this.UpdateMode == MoverUpdateMode.Update) {
				this.UpdateMover();
			}
		}

		void LateUpdate() {
			if (this.UpdateMode == MoverUpdateMode.LateUpdate) {
				this.UpdateMover();
			}
		}

		void FixedUpdate() {
			if (this.UpdateMode == MoverUpdateMode.FixedUpdate) {
				this.UpdateMover();
			}
		}

		public void UpdateGoToPosition(Vector3 position) {
			m_GoToPosition = position;
		}

		public void UpdateMover() {

			if (this.OnBeforeMoverIsUpdated != null) {
				this.OnBeforeMoverIsUpdated(this);
			}

			float deltaTime = Mathf.Clamp(Time.deltaTime, 0, 0.016f);
			this.Velocity -= this.Velocity * this.Damper * deltaTime;
			Vector3 dif; 

			if (this.MoveSpace == Space.Self) {
				dif = this.GoToPosition - this.MoveTransform.localPosition;
			} else {
				dif = this.GoToPosition - this.MoveTransform.position;
			}

			if (this.LockZAxis) {
				dif.z = 0;
			}

			this.Velocity += dif * this.Acceleration * deltaTime;

			if (this.UseMaximunVelocity) {
				if (this.Velocity.magnitude > this.MaximunVelocity) {
					this.Velocity = this.Velocity.normalized * this.MaximunVelocity;
				}
			}

			if (this.MoveMode == MoveMode.Rigidbody) {
				this.Rigidbody.MovePosition(this.MoveTransform.position + this.Velocity * deltaTime);
				this.Rigidbody.velocity = Vector3.zero;
			} else {
				this.MoveTransform.Translate(this.Velocity * deltaTime, this.MoveSpace);
			}

		}

		public void ForcePosition(Vector3 position) {
			if (this.MoveSpace == Space.Self) {
				this.MoveTransform.localPosition = position;
			} else {
				this.MoveTransform.position = position;
			}
			this.GoToPosition = position;
			this.Velocity = Vector3.zero;
		}

		public void SetDepth(float depth) {
			if (this.MoveSpace == Space.Self) {
				this.MoveTransform.localPosition = new Vector3(this.MoveTransform.localPosition.x, this.MoveTransform.localPosition.y, depth);
			} else {
				this.MoveTransform.position = new Vector3(this.MoveTransform.position.x, this.MoveTransform.position.y, depth);
			}
		}

		#endregion

	
	}

}