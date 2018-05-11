namespace SagoUtils {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
	
	/// <summary>
	/// GameObjectPool will set the GameObjectPool property of items that are in it so they can
	/// destroy themselves properly (i.e. by calling the pool's Destroy).
	/// </summary>
	public interface IPooledObject {
		
		/// <summary>
		/// Sets the game object pool.  This is set by the GameObjectPool when this is created.
		/// </summary>
		/// <value>The game object pool.</value>
		GameObjectPool GameObjectPool { set; }
		
		/// <summary>
		/// Disable this instance and return it to the pool.  Typically just:
		/// this.GameObjectPool.Destroy(this.gameObject);
		/// </summary>
		void Destroy();
	}
	
	
	/// <summary>
	/// Maintains a pool of disabled GameObjects that can be "spawned" (i.e. reactivated)
	/// and "destroyed" (i.e. deactivated) without having to re-Instantiate.  Usage:
	/// just call GameObjectPool.Create().
	/// Can also pass a list of prefabs to get random ones.
	/// </summary>
	public class GameObjectPool : MonoBehaviour {
		
		
		#region Types
		
		/// How to handle a Spawn call from the pool when it is empty/exhausted
		[System.Serializable]
		public enum ExhaustBehaviour {
			
			/// If pool is exhausted, ignore Spawn calls
			Ignore,
			
			/// If pool is exhausted, despawn the oldest and use that (Default)
			UseOldest,
			
			/// If pool is exhausted, increase pool size
			Grow
		}
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SagoUtils.GameObjectPool"/> apply random scaling.
		/// <see cref="ScaleRange"/>.
		/// </summary>
		/// <value><c>true</c> if apply random scaling; otherwise, <c>false</c>.</value>
		public bool ApplyRandomScaling {
			get;
			set;
		}
		
		/// <summary>
		/// How to handle spawns when the pool is empty/exhausted.
		/// </summary>
		/// <value>The exhaustion behaviour.</value>
		public ExhaustBehaviour ExhaustionBehaviour {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the scale range (lower and upper scale factors, e.g. 0.8f, 1.2f).
		/// Only applies if <see cref="ApplyRandomScaling"/> is set.
		/// </summary>
		/// <value>The scale range.</value>
		public Vector2 ScaleRange {
			get;
			set;
		}
		
		/// <summary>
		/// The current number of items in the pool.
		/// </summary>
		/// <value>The size of the pool.</value>
		public int PoolSize {
			get;
			protected set;
		}
		
		/// <summary>
		/// ActivePool is a list so that the spawned objects can be re-parented freely.
		/// </summary>
		/// <value>The active pool.</value>
		public LinkedList<Transform> ActivePool {
			get;
			protected set;
		}
		
		/// <summary>
		/// The parent transform of all the inactive objects in the pool.
		/// </summary>
		/// <value>The inactive pool.</value>
		public Transform InactivePool {
			get;
			protected set;
		}
		
		public Transform Transform {
			get {
				if (!m_Transform) {
					m_Transform = GetComponent<Transform>();
				}
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Interface
		
		/// <summary>
		/// Create a GameObjectPool for the specified prefab, with the given poolSize, and put it on the given parent.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="poolSize">Pool size.</param>
		/// <param name="parent">Parent.</param>
		public static GameObjectPool Create(GameObject prefab, int poolSize, Transform parent = null) {
			GameObject[] prefabs = new GameObject[1];
			prefabs[0] = prefab;
			return Create(prefabs, poolSize, parent);
		}
		
		/// <summary>
		/// Create a GameObjectPool for the specified prefabs, with the given poolSize, and put it on the given parent.
		/// </summary>
		/// <param name="prefabs">Prefabs.</param>
		/// <param name="poolSize">Pool size.</param>
		/// <param name="parent">Parent.</param>
		public static GameObjectPool Create(GameObject[] prefabs, int poolSize, Transform parent) {
			
			if (parent == null) {
				throw new System.ArgumentException("Could not create GameObjectPool: parent cannot be null");
			}
			
			string poolName;
			if (prefabs == null || prefabs.Length == 0) {
				Debug.LogError(string.Format("Failed to create {0}: No prefab", typeof(GameObjectPool)));
				return null;
			}
			else if (prefabs.Length == 1) {
				poolName = string.Format("Pool - {0}", prefabs[0].name);
			} else {
				poolName = string.Format("Random Pool - {0} (+{1} more)", prefabs[0].name, prefabs.Length - 1);
			}
			
			GameObject goPool = new GameObject(poolName);
			Transform tPool = goPool.GetComponent<Transform>();
			
			tPool.parent = parent;
			tPool.localPosition = Vector3.zero;
			
			GameObjectPool newPool = goPool.AddComponent<GameObjectPool>();
			newPool.Initialize(prefabs, poolSize);
			
			return newPool;
		}
		
		/// <summary>
		/// Spawns an object from the pool.
		/// </summary>
		/// <returns>The spawned (depooled) object.</returns>
		/// <param name="position">Position.</param>
		public GameObject Spawn(Vector3 position) {
			
			// handle exhausted pool
			if (this.InactivePool.childCount == 0) {
				if (this.ExhaustionBehaviour == ExhaustBehaviour.UseOldest) {
					Transform deactivate = this.ActivePool.First.Value;
					ActivePool.RemoveFirst();
					deactivate.parent = this.InactivePool;
				} else if (this.ExhaustionBehaviour == ExhaustBehaviour.Grow) {
					int rndIdx = Random.Range(0, this.ActivePool.Count);
					LinkedListNode<Transform> current = this.ActivePool.First;
					int counter = 0;
					while (current != null && current.Next != null && counter < rndIdx) {
						current = current.Next;
						++counter;
					}
					if (current != null && current.Value) {
						AddPrefabToPool(current.Value.gameObject, this.PoolSize);
						++this.PoolSize;
						//#if SAGO_DEBUG
						//Debug.Log(string.Format("Growing Pool {0} to {1}", this.name, this.PoolSize), this);
						//#endif
					}
					
				} else {
					return null;
				}
			}
			
			if (this.InactivePool.childCount > 0) {
				Transform activated = this.InactivePool.GetChild(this.InactivePool.childCount - 1);
				activated.parent = this.Transform;
				activated.position = position;
				ActivePool.AddLast(activated);
				
				if (this.ApplyRandomScaling) {
					float scale = Random.Range(this.ScaleRange[0], this.ScaleRange[1]);
					activated.localScale = new Vector3(scale, scale, scale);
				}
				
				return activated.gameObject;
			}
			return null;
		}
		
		/// <summary>
		/// Spawns an object from the pool.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="eulerAngles">Euler angles.</param>
		public GameObject Spawn(Vector3 position, Vector3 eulerAngles) {
			GameObject go = Spawn(position);
			if (go) {
				go.GetComponent<Transform>().eulerAngles = eulerAngles;
			}
			return go;
		}
		
		/// <summary>
		/// Spawned objects must call this to destroy themselves (i.e. re-enter the inactive pool).
		/// </summary>
		/// <param name="bubble">GameObject to "destroy" (return to inactive pool).</param>
		public void Destroy(GameObject go) {
			Transform goT = go.GetComponent<Transform>();
			ActivePool.Remove(goT);
			goT.parent = this.InactivePool;
		}
		
		/// <summary>
		/// Destroys (returns to inactive pool) all objects.
		/// </summary>
		public void DestroyAll() {
			if (this.ActivePool != null && this.InactivePool) {
				while (ActivePool.Last != null) {
					ActivePool.Last.Value.parent = this.InactivePool;
					ActivePool.RemoveLast();
				}
			}
		}
		
		/// <summary>
		/// Makes exactly activeCount objects active by activating if lower
		/// and deactivating if higher.
		/// </summary>
		/// <param name="activeCount">Active count.</param>
		public void SetActiveCount(int activeCount) {
			if (this.ActivePool != null && this.InactivePool) {
				activeCount = Mathf.Min(activeCount, this.PoolSize);
				while (this.ActivePool.Count < activeCount) {
					Spawn(Vector3.zero);
				}
				while (this.ActivePool.Count > activeCount) {
					Destroy(this.ActivePool.Last.Value.gameObject);
				}
			}
		}
		
		/// <summary>
		/// To access active objects by index.
		/// </summary>
		/// <returns>The <see cref="UnityEngine.Transform"/>.</returns>
		/// <param name="idx">Index.</param>
		public Transform GetActiveAt(int idx) {
			if (idx < 0 || ActivePool == null) {
				return null;
			}
			LinkedListNode<Transform> current = ActivePool.First;
			while (current != null) {
				if (idx == 0) {
					return current.Value;
				}
				--idx;
				current = current.Next;
			}
			return null;
		}
		
		#endregion
		
		
		#region MonoBehaviour
		
		virtual protected void Awake() {
			this.ExhaustionBehaviour = ExhaustBehaviour.UseOldest;
			this.ApplyRandomScaling = false;
			this.ScaleRange = new Vector2(1.0f, 1.0f);
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Internal Methods
		
		protected void Initialize(GameObject[] prefabs, int poolSize) {
			
			if (this.ActivePool != null && this.InactivePool != null) {
				return;
			}
			
			this.PoolSize = poolSize;
			
			// inactive pool will just be handled by making inactive objects children to an inactive parent
			GameObject goInactivePool = new GameObject("Inactive Pool");
			this.InactivePool = goInactivePool.GetComponent<Transform>();
			this.InactivePool.parent = this.Transform;
			this.InactivePool.gameObject.SetActive(false);
			
			this.ActivePool = new LinkedList<Transform>();
			
			for (int i = 0; i < this.PoolSize; ++i) {
				GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
				AddPrefabToPool(prefab, i);
			}
		}
		
		protected void AddPrefabToPool(GameObject prefab, int id) {
			GameObject go = Instantiate(prefab) as GameObject;
			
			string name = prefab.name;
			int baseIndex = name.IndexOf(" (Pooled)");
			if (baseIndex >= 0) {
				name = name.Substring(0, baseIndex);
			}
			go.name = string.Format("{0} (Pooled) #{1:D3}", name, id);
			
			Transform t = go.GetComponent<Transform>();
			t.parent = this.InactivePool;
			
			foreach(IPooledObject iPool in go.GetComponentsInChildren(typeof(IPooledObject), true)) {
				iPool.GameObjectPool = this;
			}
		}
		
		#endregion
		
		
	}
}