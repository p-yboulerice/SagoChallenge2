namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Extension methods for Unity Components.
	/// </summary>
	public static class ComponentExtensions {
		
		/// <summary>
		/// Like GetComponentsInChildren, but just returns the first component found so
		/// we can use the includeInactive flag.
		/// </summary>
		/// <returns>The component in children.</returns>
		/// <param name="comp">Comp.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInChildren<T>(this Component comp, bool includeInactive) where T : Component {
			// simple implementation just so we have this available
			T[] all = comp.GetComponentsInChildren<T>(includeInactive);
			if (all == null || all.Length == 0) {
				return null;
			} else {
				return all[0];
			}
		}
		
		/// <summary>
		/// The normal GetComponentInChildren does a depth first search of the tree.
		/// This gives the option of doing a breadth first search, i.e. it will check
		/// all children in order before checking any grandchildren.
		/// </summary>
		/// <returns>The component in children.</returns>
		/// <param name="comp">Component to search though</param>
		/// <param name="includeInactive">If set to <c>true</c> include results from inactive GameObjects</param>
		/// <param name="breadthFirst">If set to <c>true</c> do a breadth-first search, otherwise do depth-first.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInChildren<T>(this Component comp, bool includeInactive, bool breadthFirst) where T : Component {
			
			if (breadthFirst) {
				
				T result = null;
				
				if (comp.gameObject.activeInHierarchy || includeInactive) {
					
					result = comp.GetComponent<T>();
					
					if (!result) {
						Queue<Transform> checkQueue = new Queue<Transform>();
						EnqueueChildren(comp.transform, checkQueue);
						
						while (checkQueue.Count > 0) {
							Transform child = checkQueue.Dequeue();
							if (child.gameObject.activeInHierarchy || includeInactive) {
								result = child.GetComponent<T>();
								if (result) {
									break;
								} else {
									EnqueueChildren(child, checkQueue);
								}
							}
						}
					}
				}
				
				return result;
				
			} else {
				
				return comp.GetComponentInChildren<T>(includeInactive);
				
			}
			
		}
		
		private static void EnqueueChildren(Transform parent, Queue<Transform> queue) {
			for (int i = 0; i < parent.childCount; ++i) {
				queue.Enqueue(parent.GetChild(i));
			}
		}
		
	}
	
}
