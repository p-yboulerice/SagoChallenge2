namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// <para>Return random array indices, ensuring each is returned
	/// once before repeating, and on looping don't return same
	/// again.  e.g.</para>
	/// <code>
	/// RandomArrayIndex myRandomIndex = new RandomArrayIndex(myArray.Length);
	/// newRandomObject = myArray[myRandomIndex.Advance];
	/// </code>
	/// </summary>
	public class RandomArrayIndex {
		
		
		#region Public
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SagoUtils.RandomArrayIndex"/> class
		/// using the number of items in your array.
		/// </summary>
		/// <param name="count">Count.</param>
		public RandomArrayIndex(int count) {
			this.CurrentIndex = 0;
			this.Indices = new List<int>(count);
			for (int i = 0; i < count; ++i) {
				this.Indices.Add(i);
			}
			Shuffle();
		}
		
		/// <param name="rai">Implicitly cast the RandomArrayIndex to its Current value</param>
		public static implicit operator int(RandomArrayIndex rai) {
			return rai.Current;
		}
		
		/// <summary>
		/// Gets the current random index, without moving ahead in the pool.
		/// </summary>
		/// <value>The current.</value>
		public int Current {
			get {
				return this.Indices[this.CurrentIndex];
			}
		}
		
		/// <summary>
		/// Gets the next random index in the pool.  Reshuffles if it reaches the end.
		/// </summary>
		/// <value>The next.</value>
		public int Next {
			get {
				Increment();
				return this.Current;
			}
		}
		
		/// <summary>
		/// Gets the current random index in the pool, then moves the pool ahead for the next call.
		/// Reshuffles if it reaches the end.
		/// </summary>
		/// <value>The advance.</value>
		public int Advance {
			get {
				int original = this.Current;
				Increment();
				return original;
			}
		}
		
		#endregion
		
		
		#region Internal
		
		protected int Count {
			get {
				return this.Indices.Count;
			}
		}
		
		protected int CurrentIndex {
			get; set;
		}
		
		protected List<int> Indices {
			get; set;
		}
		
		protected void Increment() {
			int previousCurrent = this.Current;
			
			this.CurrentIndex++;
			
			if (this.CurrentIndex >= this.Count) {
				this.CurrentIndex = 0;
				Shuffle();
				if (this.Current == previousCurrent && this.Count > 1) {
					int tmp = this.Indices[0];
					this.Indices[0] = this.Indices[this.Count - 1];
					this.Indices[this.Count - 1] = tmp;
				}
			}
		}
		
		protected void Shuffle() {
			for (int i = 0; i < this.Count; ++i) {
				int idx = Random.Range(0, this.Count);
				if (idx != i) {
					int tmp = this.Indices[i];
					this.Indices[i] = this.Indices[idx];
					this.Indices[idx] = tmp;
				}
			}
		}
		
		#endregion
		
		
	}
	
}
