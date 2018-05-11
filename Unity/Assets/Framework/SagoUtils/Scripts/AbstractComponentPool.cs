namespace SagoUtils {
	
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using WeakReference = System.WeakReference;
	
	public interface IComponentPool {
		
		void Cleanup(IComponentPoolElement element);
		
		IComponentPoolElement Depool();
		
		void Repool(IComponentPoolElement element);
		
	}
	
	public abstract class AbstractComponentPool<T> : MonoBehaviour, IComponentPool where T : Component, IComponentPoolElement {
		
		
		#region Fields
		
		[System.NonSerialized]
		private int m_Count = -1;
		
		[Range(0,100)]
		[SerializeField]
		[Tooltip("The maximum number of elements the pool can create. When the value is zero, the pool can create an unlimited number of elements.")]
		private int m_Maximum;
		
		[System.NonSerialized]
		private Transform m_Transform;
		
		#endregion
		
		
		#region Properties
		
		public int Count {
			get {
				m_Count = m_Count < 0 ? Mathf.Max(this.Transform.childCount - 1, 0) : m_Count;
				return m_Count;
			}
			protected set {
				m_Count = value;
			}
		}
		
		public int Maximum {
			get { return m_Maximum; }
			protected set { m_Maximum = value; }
		}
		
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		virtual public void Cleanup(T element) {
			this.Count = Mathf.Max(this.Count - 1, 0);
		}
		
		virtual public T Depool() {
			T element = (this.Transform.childCount > 1) ? this.GetFirstElement() : this.CloneFirstElement();
			if (element != null) {
				element.GetComponent<Transform>().parent = this.Transform.parent;
				element.OnDepool(this);
			}
			return element;
		}
		
		virtual public void Repool(T element) {
			if (element != null) {
				element.GetComponent<Transform>().parent = this.Transform;
				element.OnRepool(this);
			}
		}
		
		#endregion
		
		
		#region IComponentPool Methods
		
		void IComponentPool.Cleanup(IComponentPoolElement element) {
			Cleanup(element as T);
		}
		
		IComponentPoolElement IComponentPool.Depool() {
			return Depool() as T;
		}
		
		void IComponentPool.Repool(IComponentPoolElement element) {
			Repool(element as T);
		}
		
		#endregion
		
		
		#region Private Methods
		
		private T CloneFirstElement() {
			
			if (this.Maximum > 0 && this.Maximum < this.Count) {
				return null;
			}
			
			Transform sourceTransform;
			sourceTransform = this.Transform.GetChild(0);
			
			Transform cloneTransform;
			cloneTransform = (Instantiate(sourceTransform.gameObject) as GameObject).GetComponent<Transform>();
			cloneTransform.name = sourceTransform.name;
			cloneTransform.position = sourceTransform.position;
			
			T clone;
			clone = cloneTransform.GetComponent<T>();
			
			this.Count++;
			
			return clone;
			
		}
		
		private T GetFirstElement() {
			return this.Transform.GetChild(0).GetComponent<T>();
		}
		
		#endregion
	
	}
	
}