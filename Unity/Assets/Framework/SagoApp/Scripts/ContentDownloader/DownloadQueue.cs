namespace SagoApp.ContentDownloader {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A double ended queue with random access.
	/// ToDo : Will have to keep track of total file size.
	/// </summary>
	public class DownloadQueue<T> {
		private List<T> m_Items;

		private List<T> Items {
			get { return m_Items = m_Items ?? new List<T>(); }
		}

		public void AddFirst(T item) {
			if (!this.Items.Contains(item)) {
				this.Items.Insert(0, item);
			}
		}

		public void AddLast(T item) {
			if (!this.Items.Contains(item)) {
				this.Items.Add(item);
			}
		}

		public T RemoveFirst() {
			if (this.Items.Count > 0) {
				var item = this.Items[0];
				this.Items.RemoveAt(0);
				return item;
			}

			return default(T);
		}

		public T RemoveLast() {
			if (this.Items.Count > 0) {
				var item = this.Items[this.Items.Count - 1];
				this.Items.RemoveAt(this.Items.Count - 1);
				return item;
			}

			return default(T);
		}

		public T GetFirst() {
			if (this.Items.Count > 0) {
				var item = this.Items[0];
				return item;
			}

			return default(T);
		}

		public T GetLast() {
			if (this.Items.Count > 0) {
				var item = this.Items[this.Items.Count - 1];
				return item;
			}

			return default(T);
		}

		public void Remove(T item) {
			if (this.Items.Contains(item)) {
				this.Items.Remove(item);
			}
		}

		public void Clear() {
			this.Items.Clear();
		}

		public T[] ToArray() {
			return this.Items.ToArray();
		}

		public bool IsEmpty() {
			return this.Items.Count == 0;
		}
	}
}
