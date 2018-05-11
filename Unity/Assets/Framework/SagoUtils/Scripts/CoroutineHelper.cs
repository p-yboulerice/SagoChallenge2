namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	/// <summary>
	/// The CoroutineHelper class provides a simple, easy way to start, stop and wait for coroutines.
	/// </summary>
	/// <example>
	/// This example shows how to use a CoroutineHelper object to implement show and hide coroutines. 
	/// The show and hide coroutines will never run concurrently. Calling show or hide multiple times 
	/// won't interrupt or restart the coroutine if it's already running.
	/// <code>
	/// private CoroutineHelper m_Helper;
	/// 
	/// private CoroutineHelper Helper {
	/// 	get { m_Helper = m_Helper ?? new CoroutineHelper(this); }
	/// }
	/// 
	/// public bool IsHiding {
	/// 	get { return Helper.IsRunningCoroutine("HideAsync"); }
	/// }
	/// 
	/// public bool IsShowing {
	/// 	get { return Helper.IsRunningCoroutine("ShowAsync"); }
	/// }
	/// 
	/// private Coroutine Hide() {
	/// 	Helper.StopAllCoroutinesExcept("HideAsync");
	/// 	return Helper.StartCoroutine("HideAsync", HideAsync());
	/// }
	/// 
	/// private IEnumerator HideAsync() {
	/// 	yield return new WaitForSeconds(1);
	/// }
	/// 
	/// public Coroutine Show() {
	/// 	Helper.StopAllCoroutinesExcept("ShowAsync");
	/// 	return Helper.StartCoroutine("ShowAsync", ShowAsync());
	/// }
	/// 
	/// private IEnumerator ShowAsync() {
	/// 	yield return new WaitForSeconds(1);
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// This example shows how to use a CoroutineHelper object to run multiple coroutines at the same time and wait until they have finished.
	/// <code>
	/// private CoroutineHelper m_Helper;
	/// 
	/// private CoroutineHelper Helper {
	/// 	get { m_Helper = m_Helper ?? new CoroutineHelper(this); }
	/// }
	/// 
	/// public IEnumerator DoTasks() {
	/// 	yield return Helper.StartCoroutine("DoTasks", new IEnumerator[]{ 
	/// 		Task1(), Task2(), Task3() 
	/// 	});
	/// 	Debug.Log("Done!");
	/// }
	/// 
	/// private IEnumerator Task1() {
	/// 	yield return new WaitForSeconds(1);
	///	}
	/// 
	/// private IEnumerator Task2() {
	/// 	yield return new WaitForSeconds(2);
	///	}
	/// 
	/// private IEnumerator Task3() {
	/// 	yield return new WaitForSeconds(3);
	///	}
	/// 
	/// </code>
	/// </example>
	/// <example>
	/// This example shows how to use a CoroutineHelper object to let multiple methods to wait for the same coroutine to finish.
	/// <code>
	/// private CoroutineHelper m_Helper;
	/// 
	/// private CoroutineHelper Helper {
	/// 	get { m_Helper = m_Helper ?? new CoroutineHelper(this); }
	/// }
	/// 	
	/// private void Start() {
	/// 	Helper.StartCoroutine("Load", Load());
	/// 	Helper.StartCoroutine("PlayAnimationAfterLoad", PlayAnimationAfterLoad());
	/// 	Helper.StartCoroutine("PlayAudioAfterLoad", PlayAudioAfterLoad());
	/// }
	/// 
	/// private IEnumerator Load() {
	/// 	yield return new WaitForSeconds(1);
	/// }
	/// 
	/// private IEnumerator PlayAnimationAfterLoad() {
	/// 	while (Helper.IsRunningCoroutine("Load")) {
	/// 		yield return null;
	/// 	}
	/// 	Debug.Log("Play animation...");
	/// }
	/// 
	/// private IEnumerator PlayAudioAfterLoad() {
	/// 	yield return Helper.WaitForCoroutine("Load");
	/// 	Debug.Log("Play audio...");
	/// }
	/// </code>
	/// </example>
	public class CoroutineHelper {
		
		
		#region Fields
		
		[System.NonSerialized]
		private MonoBehaviour m_Behaviour;
		
		[System.NonSerialized]
		private Dictionary<string,List<IEnumerator>> m_Dictionary;
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a CoroutineHelper instance.
		/// </summary>
		/// <param name="Behaviour">
		/// The MonoBehaviour instance that will run the coroutines.
		/// </param>
		public CoroutineHelper(MonoBehaviour behaviour) {
			m_Behaviour = behaviour;
			m_Dictionary = new Dictionary<string,List<IEnumerator>>();
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>Is a coroutine with the specified key running?</summary>
		/// <param name="Key">The key for the coroutine.</param>
		public bool IsRunningCoroutine(string key) {
			return m_Dictionary.ContainsKey(key);
		}
		
		/// <summary>Starts a coroutine with the specified key.</summary>
		/// <param name="Key">The key for the coroutine.</param>
		/// <param name="Routine">The routine to use when starting the coroutine.</param>
		public Coroutine StartCoroutine(string key, IEnumerator routine) {
			if (!IsRunningCoroutine(key)) {
				Run(key, WaitAndRemoveKeyAsync(key, routine));
			}
			return WaitForCoroutine(key);
		}
		
		/// <summary>Starts the coroutines with the specified key.</summary>
		/// <param name="Key">The key for the coroutines.</param>
		/// <param name="Routine">The routines to use when starting the coroutine.</param>
		public Coroutine StartCoroutine(string key, IEnumerator[] routines) {
			if (!IsRunningCoroutine(key)) {
				Run(key, WaitAndRemoveKeyAsync(key, Multiplex(key, routines)));
			}
			return WaitForCoroutine(key);
		}
		
		/// <summary>Stops all coroutines.</summary>
		public void StopAllCoroutines() {
			foreach (string key in m_Dictionary.Keys.ToArray()) {
				StopCoroutine(key);
			}
		}
		
		/// <summary>Stops all coroutines except the coroutine for the specified key.</summary>
		/// <param name="Key">The key for the coroutine.</param>
		public void StopAllCoroutinesExcept(string key) {
			foreach (string otherKey in m_Dictionary.Keys.ToArray()) {
				if (otherKey != key) {
					StopCoroutine(otherKey);
				}
			}
		}
		
		/// <summary>Stops the coroutine for the specified key.</summary>
		/// <param name="Key">The key for the coroutine.</param>
		public void StopCoroutine(string key) {
			if (IsRunningCoroutine(key)) {
				foreach (IEnumerator routine in m_Dictionary[key]) {
					m_Behaviour.StopCoroutine(routine);
				}
				m_Dictionary.Remove(key);
			}
		}
		
		/// <summary>Waits until the coroutine with the specified key stops running.</summary>
		/// <param name="Key">The key for the coroutine.</param>
		public Coroutine WaitForCoroutine(string key) {
			return IsRunningCoroutine(key) ? m_Behaviour.StartCoroutine(WaitAsync(key)) : null;
		}
		
		#endregion
		
		
		#region Methods
		
		private Coroutine Run(string key, IEnumerator routine) {
			
			if (!m_Dictionary.ContainsKey(key)) {
				m_Dictionary.Add(key, new List<IEnumerator>());
			}
			
			Coroutine coroutine;
			coroutine = m_Behaviour.StartCoroutine(routine);
			
			if (m_Dictionary.ContainsKey(key)) {
				m_Dictionary[key].Add(routine);
				return coroutine;
			}
			
			return null;
			
		}
		
		private IEnumerator Multiplex(string key, IEnumerator[] routines) {
			int count = routines.Length;
			System.Action action = () => { count--; };
			foreach (IEnumerator routine in routines) {
				Run(key, WaitAndDoActionAsync(key, routine, action));
			}
			while (count > 0) {
				yield return null;
			}
		}
		
		private IEnumerator WaitAsync(string key) {
			while (IsRunningCoroutine(key)) {
				yield return null;
			}
		}
		
		private IEnumerator WaitAndDoActionAsync(string key, IEnumerator routine, System.Action action) {
			yield return Run(key, routine);
			if (action != null) {
				action();
			}
		}
		
		private IEnumerator WaitAndRemoveKeyAsync(string key, IEnumerator routine) {
			yield return Run(key, routine);
			m_Dictionary.Remove(key);
		}
		
		#endregion
		
		
	}

}
