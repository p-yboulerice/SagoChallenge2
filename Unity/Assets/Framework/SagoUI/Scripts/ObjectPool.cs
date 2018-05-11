namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	public class ObjectPool<T> where T : new() {
		
		
		#region Fields
		
		private readonly Stack<T> m_Stack = new Stack<T>();
		
		private readonly System.Action<T> m_ActionOnGet;
		
		private readonly System.Action<T> m_ActionOnPut;
		
		#endregion
		
		
		#region Constructors
		
		public ObjectPool(System.Action<T> actionOnGet, System.Action<T> actionOnPut) {
			m_ActionOnGet = actionOnGet;
			m_ActionOnPut = actionOnPut;
		}
		
		#endregion
		
		
		#region Methods
		
		public T Get() {
			T element;
			if (m_Stack.Count == 0) {
				element = new T();
			} else {
				element = m_Stack.Pop();
			}
			if (m_ActionOnGet != null) {
				m_ActionOnGet(element);
			}
			return element;
		}
		
		public void Put(T element) {
			if (m_ActionOnPut != null) {
				m_ActionOnPut(element);
			}
			if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element)) {
				return;
			}
			m_Stack.Push(element);
		}
		
		#endregion
		
		
	}
}
