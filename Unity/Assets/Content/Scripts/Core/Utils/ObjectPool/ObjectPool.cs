namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class ObjectPool {


		#region Fields

		[System.NonSerialized]
		private List<ObjectPoolItem> Items;

		[System.NonSerialized]
		private float m_TotalProbabilityWeight;

		#endregion


		#region Properties

		private float TotalProbabilityWeight {
			get {
				if (m_TotalProbabilityWeight == 0) {
					foreach (ObjectPoolItem i in this.Items) {
						m_TotalProbabilityWeight += i.PriorityWeight;
					}
				}
				return m_TotalProbabilityWeight;
			}

			set { m_TotalProbabilityWeight = value; }
		}

		#endregion


		#region Methods

		public ObjectPool(ObjectPoolItem[] items) {
			this.Items = new List<ObjectPoolItem>(items);
		}

		public ObjectPoolItem GetItem() {

			if (this.Items.Count == 0) {
				return null;
			}

			float randomWeight = this.TotalProbabilityWeight * Random.value;
			float previous = 0;
			float current = 0;
			int i;
			
			for (i = 0; i < this.Items.Count; i++) {
				previous = current;
				current += this.Items[i].PriorityWeight;

				if (previous <= randomWeight && current >= randomWeight) {
					break;
				}
			}
			
			return this.Items[i];
		}

		public ObjectPoolItem TakeItem() {
			ObjectPoolItem item = this.GetItem();
			if (item != null) {
				this.Items.Remove((ObjectPoolItem)item);
				this.TotalProbabilityWeight = 0;
			}
			return item;
		}

		#endregion


	}

}