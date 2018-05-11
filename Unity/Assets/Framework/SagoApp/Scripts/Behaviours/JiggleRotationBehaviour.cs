namespace SagoApp {
	
	using System.Collections;
	using UnityEngine;
	using SagoUtils;
	
	/// <summary>
	/// Jiggles the rotation of this object when Trigger() is called.
	/// </summary>
	public class JiggleRotationBehaviour : MonoBehaviour {
		
		
		#region Serialized Fields
		
		[Range(0.01f, 4f)]
		[SerializeField]
		protected float m_Duration;
		
		[Range(0f, 90f)]
		[SerializeField]
		protected float m_SwingAngle;
		
		[Range(1, 12)]
		[SerializeField]
		protected int m_Oscillations;
		
		#endregion
		
		
		#region Public Properties
		
		virtual public float Duration {
			get {
				return m_Duration;
			}
		}
		
		virtual public bool IsJiggling {
			get; 
			protected set;
		}
		
		virtual public int Oscillations {
			get {
				return m_Oscillations;
			}
		}
		
		virtual public float Rotation {
			get {
				return this.Transform.localEulerAngles.z;
			}
			set {
				Vector3 euler = this.Transform.localEulerAngles;
				euler.z = value;
				this.Transform.localEulerAngles = euler;
			}
		}
		
		virtual public float SwingAngle {
			get {
				return m_SwingAngle;
			}
		}
		
		/// <summary>
		/// The Transform component (lazy loaded, cached).
		/// </summary>
		virtual public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Start jiggling.  Stops automatically after Duration.
		/// Has no effect if already jiggling.
		/// </summary>
		[ContextMenu("Trigger Jiggle")]
		virtual public void Trigger() {
			if (!this.IsJiggling) {
				this.StartCoroutine(this.JiggleAsync());
			}
		}
		
		/// <summary>
		/// Stop the jiggling and resets the rotation to where it started.
		/// </summary>
		virtual public void Stop() {
			if (this.IsJiggling) {
				StopAllCoroutines();
				this.Rotation = this.JiggleStartRotation;
				this.IsJiggling = false;
			}
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		/// <summary>
		/// Reset to default values.
		/// </summary>
		virtual public void Reset() {
			m_Duration = 0.75f;
			m_SwingAngle = 5f;
			m_Oscillations = 6;
		}
		
		virtual protected void OnDisable() {
			Stop();
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Internal Properties
		
		protected float JiggleStartRotation {
			get; set;
		}
		
		#endregion
		
		
		#region Internal Methods
		
		protected IEnumerator JiggleAsync() {
			
			this.IsJiggling = true;
			this.JiggleStartRotation = this.Rotation;
			
			float startTime = Time.time;
			float endTime = startTime + this.Duration;
			float invDuration = 1.0f / this.Duration;
			
			while (Time.time < endTime) {
				
				float t = (Time.time - startTime) * invDuration;
				
				this.Rotation = MathUtil.WrappedAngle(
					this.SwingAngle * Mathf.Sin(Mathf.PI * this.Oscillations * t) + this.JiggleStartRotation);
				
				yield return null;
			}
			
			this.Rotation = this.JiggleStartRotation;
			this.IsJiggling = false;
		}
		
		#endregion
		
		
	}
	
}
