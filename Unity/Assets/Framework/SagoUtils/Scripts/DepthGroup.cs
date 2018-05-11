namespace SagoUtils {

	using System.Collections.Generic;
	using UnityEngine;

	public interface IDepthGroupElement {
		DepthGroup DepthGroup { get; set; }
		Transform Transform { get; }
	}

	public class DepthGroup : MonoBehaviour {


		#region Serialized Properties

		[SerializeField]
		public bool IsElementContainer;

		[SerializeField]
		public bool CompactOnRemove;

		[Range(0, 1)]
		[SerializeField]
		public float Spacing;

		#endregion


		#region Events

		public event System.Action<DepthGroup, IDepthGroupElement> OnAddElement;
		public event System.Action<DepthGroup, IDepthGroupElement> OnRemoveElement;
		public event System.Action<DepthGroup, IDepthGroupElement> OnSetElementDepth;

		#endregion


		#region Properties

		public List<IDepthGroupElement> Elements {
			get {
				if (m_Elements == null) {
					m_Elements = new List<IDepthGroupElement>();
				}
				return m_Elements;
			}
		}

		public Transform Transform {
			get {
				if (!m_Transform) {
					m_Transform = GetComponent<Transform>();
				}
				return m_Transform;
			}
		}

		#endregion


		#region Methods

		public void AddElement(IDepthGroupElement element) {

			if (element.DepthGroup) {
				element.DepthGroup.RemoveElement(element);
			}

			float front;
			front = this.Front;

			this.Elements.Add(element);
			element.DepthGroup = this;

			SetElementDepth(element, front);
			ParentElement(element);

			if (this.OnAddElement != null) {
				this.OnAddElement(this, element);
			}

		}

		public void RemoveElement(IDepthGroupElement element) {
			if (this.Elements.Contains(element)) {

				DeparentElement(element);
				this.Elements.Remove(element);
				element.DepthGroup = null;

				if (this.CompactOnRemove) {
					CompactElements();
				}

				if (this.OnRemoveElement != null) {
					this.OnRemoveElement(this, element);
				}

			}
		}

		public void CompactElements() {

			float depth;
			depth = this.Back;

			foreach (IDepthGroupElement element in this.Elements) {
				SetElementDepth(element, depth);
				depth -= this.Spacing;
			}

		}

		#endregion


		#region Fields

		private List<IDepthGroupElement> m_Elements;
		private Transform m_Transform;

		#endregion


		#region Functions

		private float GetElementDepth(IDepthGroupElement element) {
			return element.Transform.position.z;
		}

		private void SetElementDepth(IDepthGroupElement element, float depth) {

			element.Transform.position = (Vector3)(Vector2)element.Transform.position + depth * Vector3.forward;

			if (this.OnSetElementDepth != null) {
				this.OnSetElementDepth(this, element);
			}

		}

		private void ParentElement(IDepthGroupElement element) {
			element.Transform.parent = this.IsElementContainer ? this.Transform : element.Transform.parent;
		}

		private void DeparentElement(IDepthGroupElement element) {
			if (this.IsElementContainer && element.Transform.parent == this.Transform) {
				element.Transform.parent = this.Transform.parent;
			}
		}

		#endregion


		#region Helper

		private float Back {
			get { return this.Transform.position.z; }
		}

		private float Front {
			get { return (this.LastElement == null) ? this.Back : GetElementDepth(this.LastElement) - this.Spacing; }
		}

		private IDepthGroupElement LastElement {
			get { return (this.Elements.Count > 0) ? this.Elements[this.Elements.Count - 1] : null; }
		}

		#endregion


	}

}