namespace SagoApp.Content {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The DynamicsManager class allows us to store different 3D physics settings for each content submodule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class should have the same fields as <c>ProjectSettings/DynamicsManager.asset</c>. When 
	/// the fields in the <c>DynamicsManager.asset</a> change, this class should be updated to match.
	/// </para>
	/// </remarks>
	[CreateAssetMenu(menuName = "DynamicsManager", fileName = "DynamicsManager", order = 5000)]
	public class DynamicsManager : ScriptableObject {
		
		
		#region Default
		
		[RuntimeInitializeOnLoadMethod]
		public static void CreateDefaultDynamicsManager() {
			if (DefaultDynamicsManager == null) {
				DefaultDynamicsManager = ScriptableObject.CreateInstance<DynamicsManager>();
				DefaultDynamicsManager.hideFlags = HideFlags.HideAndDontSave;
				DefaultDynamicsManager.name = "DefaultDynamicsManager";
				DefaultDynamicsManager.Reset();
				DefaultDynamicsManager.Pull();
			}
		}
		
		public static DynamicsManager DefaultDynamicsManager {
			get; private set;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private Vector3 m_Gravity;
		
		// TODO: There is no runtime api for setting the default material.
		[HideInInspector]
		[SerializeField]
		private PhysicMaterial m_DefaultMaterial;
		
		[SerializeField]
		private float m_BounceThreshold;
		
		[SerializeField]
		private float m_SleepThreshold;
		
		[SerializeField]
		private float m_DefaultContactOffset;
		
		[SerializeField]
		private int m_SolverIterationCount;
		
		[SerializeField]
		private bool m_QueriesHitTriggers;
		
		// TODO: There is no runtime api for setting enable adaptive force.
		[HideInInspector]
		[SerializeField]
		private bool m_EnableAdaptiveForce;
		
		[SerializeField]
		private int[] m_LayerCollisionMatrix;
		
		#endregion
		
		
		#region Methods
		
		public void Pull() {
			
			m_Gravity = Physics.gravity;
			m_BounceThreshold = Physics.bounceThreshold;
			m_SleepThreshold = Physics.sleepThreshold;
			m_DefaultContactOffset = Physics.defaultContactOffset;
			m_SolverIterationCount = Physics.defaultSolverIterations;
			m_QueriesHitTriggers = Physics.queriesHitTriggers;
			
			for (int row = 0; row < 32; row++) {
				for (int col = 0; col < 32; col++) {
					IgnoreLayerCollision(row, col, Physics.GetIgnoreLayerCollision(row, col));
				}
			}
			
		}
		
		public void Push() {
			
			Physics.gravity = m_Gravity;
			Physics.bounceThreshold = m_BounceThreshold;
			Physics.sleepThreshold = m_SleepThreshold;
			Physics.defaultContactOffset = m_DefaultContactOffset;
			Physics.defaultSolverIterations = m_SolverIterationCount;
			Physics.queriesHitTriggers = m_QueriesHitTriggers;
			
			for (int row = 0; row < 32; row++) {
				for (int col = 0; col < 32; col++) {
					Physics.IgnoreLayerCollision(row, col, GetIgnoreLayerCollision(row, col));
				}
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		private bool GetIgnoreLayerCollision(int layer1, int layer2) {
			int row = Mathf.Min(layer1, layer2);
			int col = Mathf.Max(layer1, layer2);
			BitArray bitArray = new BitArray(new int[]{ m_LayerCollisionMatrix[row] });
			return !bitArray.Get(col);
		}
		
		private void IgnoreLayerCollision(int layer1, int layer2, bool ignore) {
			int row = Mathf.Min(layer1, layer2);
			int col = Mathf.Max(layer1, layer2);
			BitArray bitArray = new BitArray(new int[]{ m_LayerCollisionMatrix[row] });
			bitArray.Set(col, !ignore);
			bitArray.CopyTo(m_LayerCollisionMatrix, row);
		}
		
		private void Reset() {
			m_LayerCollisionMatrix = new int[32];
		}
		
		#endregion
		
		
	}
	
}