namespace SagoApp.Content {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The Physics2DSettings class allows us to store different 2D physics settings for each content submodule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class should have the same fields as <c>ProjectSettings/Physics2DSettings.asset</c>. When 
	/// the fields in the <c>Physics2DSettings.asset</a> change, this class should be updated to match.
	/// </para>
	/// </remarks>
	[CreateAssetMenu(menuName = "Physics2DSettings", fileName = "Physics2DSettings", order = 5000)]
	public class Physics2DSettings : ScriptableObject {
		
		
		#region Default
		
		[RuntimeInitializeOnLoadMethod]
		public static void CreateDefaultPhysics2DSettings() {
			if (DefaultPhysics2DSettings == null) {
				DefaultPhysics2DSettings = ScriptableObject.CreateInstance<Physics2DSettings>();
				DefaultPhysics2DSettings.hideFlags = HideFlags.HideAndDontSave;
				DefaultPhysics2DSettings.name = "DefaultPhysics2DSettings";
				DefaultPhysics2DSettings.Reset();
				DefaultPhysics2DSettings.Pull();
			}
		}
		
		public static Physics2DSettings DefaultPhysics2DSettings {
			get; private set;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private ContentInfo m_ContentInfo;
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		private Vector2 m_Gravity;
		
		// TODO: There is no runtime api for setting the default material.
		[HideInInspector]
		[SerializeField]
		private PhysicsMaterial2D m_DefaultMaterial;
		
		[SerializeField]
		private int m_VelocityIterations;
		
		[SerializeField]
		private int m_PositionIterations;
		
		[SerializeField]
		private float m_VelocityThreshold;
		
		[SerializeField]
		private float m_MaxLinearCorrection;
		
		[SerializeField]
		private float m_MaxAngularCorrection;
		
		[SerializeField]
		private float m_MaxTranslationSpeed;
		
		[SerializeField]
		private float m_MaxRotationSpeed;
		
		[SerializeField]
		private float m_MinPenetrationForPenalty;
		
		[SerializeField]
		private float m_BaumgarteScale;
		
		[SerializeField]
		private float m_BaumgarteTimeOfImpactScale;
		
		[SerializeField]
		private float m_TimeToSleep;
		
		[SerializeField]
		private float m_LinearSleepTolerance;
		
		[SerializeField]
		private float m_AngularSleepTolerance;
		
		[SerializeField]
		private bool m_QueriesHitTriggers;
		
		[SerializeField]
		private bool m_QueriesStartInColliders;
		
		[SerializeField]
		private bool m_ChangeStopsCallbacks;
		
		[SerializeField]
		private int[] m_LayerCollisionMatrix;
		
		#endregion
		
		
		#region Methods
		
		public void Pull() {
			
			m_Gravity = Physics2D.gravity;
			m_VelocityIterations = Physics2D.velocityIterations;
			m_PositionIterations = Physics2D.positionIterations;
			m_VelocityThreshold = Physics2D.velocityThreshold;
			m_MaxLinearCorrection = Physics2D.maxLinearCorrection;
			m_MaxAngularCorrection = Physics2D.maxAngularCorrection;
			m_MaxTranslationSpeed = Physics2D.maxTranslationSpeed;
			m_MaxRotationSpeed = Physics2D.maxRotationSpeed;
			m_MinPenetrationForPenalty = Physics2D.defaultContactOffset;
			m_BaumgarteScale = Physics2D.baumgarteScale;
			m_BaumgarteTimeOfImpactScale = Physics2D.baumgarteTOIScale;
			m_TimeToSleep = Physics2D.timeToSleep;
			m_LinearSleepTolerance = Physics2D.linearSleepTolerance;
			m_AngularSleepTolerance = Physics2D.angularSleepTolerance;
			m_QueriesHitTriggers = Physics2D.queriesHitTriggers;
			m_QueriesStartInColliders = Physics2D.queriesStartInColliders;
			m_ChangeStopsCallbacks = Physics2D.changeStopsCallbacks;
			
			for (int row = 0; row < 32; row++) {
				for (int col = 0; col < 32; col++) {
					IgnoreLayerCollision(row, col, Physics2D.GetIgnoreLayerCollision(row, col));
				}
			}
			
		}
		
		public void Push() {
			
			Physics2D.gravity = m_Gravity;
			Physics2D.velocityIterations = m_VelocityIterations;
			Physics2D.positionIterations = m_PositionIterations;
			Physics2D.velocityThreshold = m_VelocityThreshold;
			Physics2D.maxLinearCorrection = m_MaxLinearCorrection;
			Physics2D.maxAngularCorrection = m_MaxAngularCorrection;
			Physics2D.maxTranslationSpeed = m_MaxTranslationSpeed;
			Physics2D.maxRotationSpeed = m_MaxRotationSpeed;
			Physics2D.defaultContactOffset = m_MinPenetrationForPenalty;
			Physics2D.baumgarteScale = m_BaumgarteScale;
			Physics2D.baumgarteTOIScale = m_BaumgarteTimeOfImpactScale;
			Physics2D.timeToSleep = m_TimeToSleep;
			Physics2D.linearSleepTolerance = m_LinearSleepTolerance;
			Physics2D.angularSleepTolerance = m_AngularSleepTolerance;
			Physics2D.queriesHitTriggers = m_QueriesHitTriggers;
			Physics2D.queriesStartInColliders = m_QueriesStartInColliders;
			Physics2D.changeStopsCallbacks = m_ChangeStopsCallbacks;
			
			for (int row = 0; row < 32; row++) {
				for (int col = 0; col < 32; col++) {
					Physics2D.IgnoreLayerCollision(row, col, GetIgnoreLayerCollision(row, col));
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