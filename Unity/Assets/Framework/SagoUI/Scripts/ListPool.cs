namespace SagoUI {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	public static class ListPool<T> {
		
		
		#region Static Fields
		
		private static readonly ObjectPool<List<T>> _Pool = new ObjectPool<List<T>>(null, list => list.Clear());
		
		#endregion
		
		
		#region Static Methods

		public static List<T> Get() {
			return _Pool.Get();
		}
		
		public static void Put(List<T> list) {
			_Pool.Put(list);
		}
		
		#endregion
		
		
	}
}
