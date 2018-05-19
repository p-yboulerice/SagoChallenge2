namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SF_PathComplete : StateFlag {


		#region Fields

		[SerializeField]
		private SA_ThrowPathUpdater m_ThrowPath;


		#endregion


		#region Properties

		public SA_ThrowPathUpdater ThrowPath {
			get { return m_ThrowPath; }
		}

		public override bool IsActive {
			get {
				return this.ThrowPath.PathCompleted; 
			}
		}

		#endregion


	}
}