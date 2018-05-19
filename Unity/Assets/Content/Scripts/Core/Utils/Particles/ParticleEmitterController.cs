namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoUtils;

	public class ParticleEmitterController : MonoBehaviour {


		#region Fields


		[SerializeField]
		private int PoolAmount = 33;

		[System.NonSerialized]
		private List<ParticleController> m_Prefab_Particle;

		[System.NonSerialized]
		private RandomArrayIndex m_RAI_Prefab_Particle;

		[SerializeField]
		private Transform[] SpawnPositions;

		[SerializeField]
		private int EmitOnStartAmount;

		[SerializeField]
		private bool Emit;

		[SerializeField]
		private float EmitRate;

		[SerializeField]
		private Vector3 SpawnVelocity_Min;

		[SerializeField]
		private Vector3 SpawnVelocity_Max;

		[SerializeField]
		private bool UseRadialForce = false;

		[SerializeField]
		private Transform RadialForceCenter;

		[System.NonSerialized]
		private RandomArrayIndex m_RandomSpawnPositionArrayIndex;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<ParticleController> m_Particles;

		[System.NonSerialized]
		private int m_CurrentIndex;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<ParticleController> Particles {
			get { 
				if (m_Particles == null) {
					m_Particles = new List<ParticleController>();
					for (int i = 0; i < this.PoolAmount; i++) {
						ParticleController particle = GameObject.Instantiate(this.Prefab_Particle[this.RAI_Prefab_Particle.Advance].gameObject).GetComponent<ParticleController>();
						particle.gameObject.SetActive(false);
						this.Particles.Add(particle);
						particle.transform.parent = this.Transform;
					}
				} 

				return m_Particles;
			}
		}

		private List<ParticleController> Prefab_Particle {
			get { 
				if (m_Prefab_Particle == null) {
					m_Prefab_Particle = new List<ParticleController>(this.GetComponentsInChildren<ParticleController>(true));
				}
				return m_Prefab_Particle;
			}
		}

		private RandomArrayIndex RAI_Prefab_Particle {
			get { return m_RAI_Prefab_Particle = m_RAI_Prefab_Particle ?? new RandomArrayIndex(this.Prefab_Particle.Count); }
		}

		private int CurrentIndex {
			get {
				return m_CurrentIndex;
			}
			set { 
				m_CurrentIndex = value;
				if (m_CurrentIndex >= this.Particles.Count) {
					m_CurrentIndex = 0;
				}
			}
		}

		private RandomArrayIndex RandomSpawnArrayIndex {
			get { return m_RandomSpawnPositionArrayIndex = m_RandomSpawnPositionArrayIndex ?? new RandomArrayIndex(this.SpawnPositions.Length); }
		}

		#endregion


		#region Methods

		void Start() {
			this.EmitParticles(this.EmitOnStartAmount);
			this.StartCoroutine(this.Coroutine_Spawn());
		}

		private IEnumerator Coroutine_Spawn() {
		
			while (true) {
				if (this.Emit) {
					yield return new WaitForSeconds(1 / this.EmitRate);
					this.EmitParticles(1);
				} else {
					yield return null;
				}
			}

		}

		public void EmitParticles(int amount) {
			for (int i = 0; i < amount; i++) {
				if (this.UseRadialForce) {
					Vector3 spawnPosition = this.SpawnPositions[this.RandomSpawnArrayIndex.Advance].position;
					Vector3 dir = spawnPosition - this.RadialForceCenter.position;
					this.Particles[this.CurrentIndex].Emit(spawnPosition, Vector3.Lerp(this.SpawnVelocity_Min, this.SpawnVelocity_Max, Random.Range(0f, 1f)).magnitude * dir.normalized);

				} else {
					float x = this.SpawnVelocity_Min.x + (this.SpawnVelocity_Max.x - this.SpawnVelocity_Min.x) * Random.value;
					float y = this.SpawnVelocity_Min.y + (this.SpawnVelocity_Max.y - this.SpawnVelocity_Min.y) * Random.value;
					this.Particles[this.CurrentIndex].Emit(this.SpawnPositions[this.RandomSpawnArrayIndex.Advance].position, 
						new Vector3(x, y, 0));
				}
				this.CurrentIndex++;
			}
		}

		#endregion


	}
}