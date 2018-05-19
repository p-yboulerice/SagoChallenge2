namespace Juice.Utils {
	
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	/// <summary>
	/// Useful when picking repeatedly from a list of objects that
	/// may change from pick to pick.  This keeps track of recent
	/// picks and makes them less likely to come up next time.
	/// 
	/// Unlike RandomArrayIndex, this does not care if the size
	/// of the source list changes or is reordered; it refers to
	/// the items themselves.
	/// </summary>
	public class RandomListTracker<T> where T : class {


		#region Public Interface

		public RandomListTracker() {
			this.History = new List<T>();
		}

		public T PickFrom(List<T> sourceList) {

			T result;

			if (sourceList == null || sourceList.Count == 0) {
				result = null;
			} else if (sourceList.Count == 1) {
				result = sourceList[0];
			} else {
				List<T> eligible = sourceList.Except(this.History).ToList();
				if (eligible.Count == 0) {

					if (this.History.Count > 0) {
						T last = this.History[this.History.Count - 1];
						eligible = sourceList.Where(t => t != last).ToList();
					} else {
						eligible = sourceList;
					}

					this.History.Clear();
				
				}
				result = eligible[Random.Range(0, eligible.Count)];
			}

			AddToHistory(result);
			return result;
		}

		#endregion


		#region Internal

		private void AddToHistory(T item) {
			if (!this.History.Contains(item)) {
				this.History.Add(item);
			}
		}

		private List<T> History;

		#endregion

		
	}
	
}
