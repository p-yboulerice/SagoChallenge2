namespace Juice.Utils {

	using UnityEngine;

	public class MaterialColorer : MonoBehaviour {


		#region Methods

		public void ApplyColor(Color value) {
			if (this.IsEnabled) {
				this.MaterialPropertyBlock.SetColor(this.ColorPropertyID, value);
				this.Renderer.SetPropertyBlock(this.MaterialPropertyBlock);
			}
		}

		#endregion


		#region Properties

		public Color Color {
			get { return m_Color; }
			set {
				if (m_Color != value) {
					m_Color = value;
					ApplyColor(value);
				}
			}
		}

		#endregion


		#region Serialized Fields

		[SerializeField]
		private Renderer m_Renderer;

		[SerializeField]
		private string m_PropertyName;

		[SerializeField]
		private Color m_Color;

		#endregion


		#region Fields

		private MaterialPropertyBlock m_MaterialPropertyBlock;

		#endregion


		#region Internal Properties

		private int ColorPropertyID {
			get;
			set;
		}

		private bool IsEnabled {
			get { return this.Renderer && this.Renderer.sharedMaterial.HasProperty(this.ColorPropertyID); }
		}

		private MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if (m_MaterialPropertyBlock == null) {
					m_MaterialPropertyBlock = new MaterialPropertyBlock();
					this.Renderer.GetPropertyBlock(m_MaterialPropertyBlock);
				}
				return m_MaterialPropertyBlock;
			}
		}

		private Renderer Renderer {
			get { return m_Renderer; }
		}

		#endregion


		#region MonoBehaviour

		private void Awake() {
			this.ColorPropertyID = Shader.PropertyToID(m_PropertyName);
			ApplyColor(this.Color);
		}

		#endregion


	}

}
