namespace SagoCore.Submodules {
	
	using UnityEngine;
	
	/// <summary>
	/// The SubmoduleInfo class allows the <see cref="SubmoduleMap" /> 
	/// to discover and store metadata about submodules.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each submodule must implement it's own subclass of <see cref="SubmoduleInfo" />. 
	/// The script for the subclass must be located at <c>{submodule}/Scripts/SubmoduleInfo.cs</c> 
	/// (the <see cref="SubmoduleMap" /> uses the location of the <c>.cs</c> file to determine 
	/// the submodule path).
	/// </para>
	/// <code>
	/// namespace MySubmodule {
	/// 	sealed public class SubmoduleInfo : SagoCore.Submodules.SubmoduleInfo {
	/// 		
	/// 	}
	/// }
	/// </code>
	/// </remarks>
	public abstract class SubmoduleInfo : ScriptableObject {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the name of the submodule.
		/// </summary>
		public string SubmoduleName {
			get { return SubmoduleMap.GetSubmoduleName(GetType()); }
		}
		
		/// <summary>
		/// Gets the path of the submodule.
		/// </summary>
		public string SubmodulePath {
			get { return SubmoduleMap.GetSubmodulePath(GetType()); }
		}
		
		#endregion
		
		
	}
	
}