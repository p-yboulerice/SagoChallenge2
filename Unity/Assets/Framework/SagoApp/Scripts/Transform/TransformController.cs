namespace SagoApp {

	using System.Collections.Generic;
	using UnityEngine;

	public class TransformController : MonoBehaviour {


		//
		// Properties
		//
		public HashSet<ITransformEffect> Effects {
			get {
				m_Effects = m_Effects ?? FindEffects();
				return m_Effects;
			}
		}

		public Vector3 LocalPosition {
			get { return m_LocalPosition; }
			set { SetLocalPosition(value); }
		}

		public Vector3 LocalScale {
			get { return m_LocalScale; }
			set { SetLocalScale(value); }
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}


		//
		// Methods
		//
		public void RefreshEffects() {
			m_Effects = FindEffects();
		}


		//
		// Member Fields
		//
		private HashSet<ITransformEffect> m_Effects;
		private Vector3 m_LocalPosition;
		private Vector3 m_LocalScale;
		private Transform m_Transform;


		//
		// Find Effects
		//
		private HashSet<ITransformEffect> FindEffects() {

			HashSet<ITransformEffect> result;
			result = new HashSet<ITransformEffect>();

			foreach (ITransformEffect effect in GetComponents(typeof(ITransformEffect))) {
				result.Add(effect);
			}

			return result;

		}


		//
		// Transform
		//
		private void SetLocalPosition(Vector3 value) {
			if (m_LocalPosition != value) {
				m_LocalPosition = value;
				ApplyEffects();
			}
		}

		private void SetLocalScale(Vector3 value) {
			if (m_LocalScale != value) {
				m_LocalScale = value;
				ApplyEffects();
			}
		}

		private void ApplyEffects() {

			Vector3 localPosition;
			localPosition = this.LocalPosition;

			Vector3 localScale;
			localScale = this.LocalScale;

			foreach (ITransformEffect effect in this.Effects) {
				localPosition += effect.LocalPositionOffset;
				localScale = Vector3.Scale(localScale, effect.LocalScaleFactor);
			}

			this.Transform.localPosition = localPosition;
			this.Transform.localScale = localScale;

		}


	}

}
