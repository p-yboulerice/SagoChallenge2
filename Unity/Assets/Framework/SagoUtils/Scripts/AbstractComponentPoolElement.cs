namespace SagoUtils {
	
	using UnityEngine;
	
	public interface IComponentPoolElement {
		
		void OnDepool(IComponentPool pool);
		
		void OnRepool(IComponentPool pool);
		
	}
	
	abstract public class AbstractComponentPoolElement<T> : MonoBehaviour, IComponentPoolElement where T : Component, IComponentPool {
		
		
		#region Fields
		
		[System.NonSerialized]
		private T m_Pool;
		
		[System.NonSerialized]
		private Transform m_Transform;
		
		#endregion
		
		
		#region Properties
		
		public T Pool {
			get { return m_Pool; }
			protected set { m_Pool = value; }
		}
		
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		virtual public void OnDepool(T pool) {
			this.Pool = pool;
			// Debug.Log(string.Format("OnDepool: {0}", this), this);
		}
		
		virtual public void OnRepool(T pool) {
			this.Pool = null;
			// Debug.Log(string.Format("OnRepool: {0}", this), this);
		}
		
		virtual public void OnDestroy() {
			if (this.Pool != null) {
				this.Pool.Cleanup(this);
			}
		}
		
		#endregion
		
		
		#region Private Methods
		
		void IComponentPoolElement.OnDepool(IComponentPool pool) {
			OnDepool(pool as T);
		}
		
		void IComponentPoolElement.OnRepool(IComponentPool pool) {
			OnRepool(pool as T);
		}
		
		#endregion
		
		
	}
	
}