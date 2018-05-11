using SagoLayout;namespace SagoBiz {
	
	using System.Collections;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// ParentsState defines the possible states for the <see cref="Parents" /> component.
	/// </summary>
	public enum ParentsState {
		Unknown,
		Loading,
		Complete,
		Error
	}
	
	/// <summary>
	/// The Parents class provides functionality for loading the parents content and turning it on and off.
	/// </summary>
	public class Parents : MonoBehaviour {
		
		
		#region Types
		
		/// <summary>
		/// The method signature for event handlers for the <see cref="Parents" /> component.
		/// </summary>
		public delegate void ParentsCallback(Parents parents);
		
		#endregion
		
		
		#region Events
		
		/// <summary>
		/// Adds and removes event handlers to call when the component finishes loading.
		/// </summary>
		public event ParentsCallback OnComplete;
		
		/// <summary>
		/// Adds and removes event handlers to call when the component has an error while loading.
		/// </summary>
		public event ParentsCallback OnError;
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// Cached reference to the <see cref="Content" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Content m_Content;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets a reference to the <see cref="Content" /> component.
		/// </summary>
		public Content Content {
			get {
				m_Content = m_Content ?? this.GetComponentInChildren<Content>();
				return m_Content;
			}
		}
		
		/// <summary>
		/// Gets and sets the error that occured while loading.
		/// </summary>
		public string Error {
			get; protected set;
		}
		
		/// <summary>
		/// Gets and sets the current state.
		/// </summary>
		public ParentsState State {
			get; protected set;
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Clears the component and resets it to the default state.
		/// </summary>
		public void Clear() {
			this.StopAllCoroutines();
			this.Content.Clear();
			this.Error = null;
			this.State = ParentsState.Unknown;
		}
		
		/// <summary>
		/// Loads the component.
		/// </summary>
		public void Load() {
			if (this.State == ParentsState.Unknown) {
				this.StartCoroutine(this.LoadAsync());
			}
		}
		
		/// <summary>
		/// Turns the component's <see cref="Content" /> on.
		/// </summary>
		public void TurnContentOn(bool animated) {
			this.Content.TurnOn(animated);
		}
		
		/// <summary>
		/// Turns the component's <see cref="Content" /> off.
		/// </summary>
		public void TurnContentOff(bool animated) {
			this.Content.TurnOff(animated);
		}
		
		#endregion
		
		
		#region Coroutine Methods
		
		/// <summary>
		/// Loads the component asynchronously.
		/// </summary>
		protected IEnumerator LoadAsync() {
			this.State = ParentsState.Complete;
			if (this.OnComplete != null) {
				this.OnComplete(this);
			}
			yield break;
		}
		
		#endregion
		
		
	}

}