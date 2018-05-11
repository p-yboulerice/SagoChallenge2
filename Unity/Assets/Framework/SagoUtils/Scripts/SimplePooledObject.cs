namespace SagoUtils {
	
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Destroys itself (by repooling) after a set time.
	/// </summary>
	public class SimplePooledObject : MonoBehaviour, IPooledObject {
		
		
		#region Serialized Fields

		[SerializeField]
		protected float m_MaxDuration;
		
		#endregion
		
		
		#region Properties
		
		public float MaxDuration {
			get { return m_MaxDuration; }
			protected set { m_MaxDuration = value; }
		}
		
		#endregion
		
		
		#region IPooledObject Implementation
		
		public GameObjectPool GameObjectPool {
			get;
			set;
		}
		
		public void Destroy() {
			this.GameObjectPool.Destroy(gameObject);
		}
		
		#endregion
		
		
		#region MonoBehaviour
		
		virtual public void Reset() {
			m_MaxDuration = 1.0f;
		}
		
		virtual protected void OnEnable() {
			if (Application.isPlaying) {
				Invoke("Destroy", this.MaxDuration);
			}
		}

		#endregion
		
		
	}
	
}