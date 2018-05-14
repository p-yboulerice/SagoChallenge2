namespace Mechanic {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	/// <summary>
	/// Handles a plant that can be pulled out of the ground and regrows
	/// </summary>
	public class Plant : MonoBehaviour {

		#region Fields

		private TouchAreaObserver touchAreaObserver;
		private LineRenderer lineRenderer;
		private Touch Touch;

		// States
		private enum PlantState {
			Idle,
			Growing,
			Pulling,
			Plucked,
			Wither
		}

		private PlantState m_state;

		private PlantState state {
			get {
				return m_state;
			}
			set {
				m_state = value;
				if (state == PlantState.Growing) {
					StartCoroutine(Growing());
				}
				else if (state == PlantState.Plucked) {
					StartCoroutine(Plucked());
				}
				else if (state == PlantState.Wither) {
					StartCoroutine(Withering());
				}
			}
		}

		// ---

		// Visible in Inspector

		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private Transform flower;

		[Space]
		[Header("Interactions")]
		[SerializeField]
		private float touchSpeed;

		[SerializeField]
		[Range(1, 20)]
		private float growSpeed;

		[SerializeField]
		private AnimationCurve growAnimationCurve;

		[SerializeField]
		private float pluckDistance;

		#endregion

		#region Unity Defaults

		// Unity Defaults

		private void OnDrawGizmos() {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, pluckDistance);
		}

		private void Awake() {

			// Components
			touchAreaObserver = GetComponent<TouchAreaObserver>();
			lineRenderer = GetComponent<LineRenderer>();

			// Coroutines
			state = PlantState.Growing;
		}

		private void OnEnable() {
			// Touches
			touchAreaObserver.TouchUpDelegate += OnTouchUp;
			touchAreaObserver.TouchDownDelegate += OnTouchDown;
		}

		private void OnDisable() {
			// Touches
			touchAreaObserver.TouchUpDelegate -= OnTouchUp;
			touchAreaObserver.TouchDownDelegate -= OnTouchDown;
		}

		// FLower target positions for lerp
		private Vector2 touchPosition = Vector2.zero;

		private Vector2 targetFlowerPosition;
		private Vector2 targetStemPosition;
		private Vector2 targetRootPosition;


		private void Update() {

			if (Touch != null) {
				// Get touch position without z axis
				touchPosition = GetTouchPosition();

				// Get touch position in local space for line renderer
				touchPosition = transform.InverseTransformPoint(touchPosition);
			}

			switch (state) {

				case PlantState.Pulling:

					if (this.Touch != null) {
						// Stem Movement
						if (Vector2.Distance(touchPosition, lineRenderer.GetPosition(0)) > 1) {
							Vector2 direction = touchPosition - (Vector2)lineRenderer.GetPosition(0);
							direction = direction.normalized;
							targetStemPosition = direction + (direction * Vector2.Distance(lineRenderer.GetPosition(0), touchPosition) * 0.2f);
						}
						else {
							// Follow mouse position
							targetStemPosition = touchPosition;
						}

						// Check if plucked
						if (Vector2.Distance(lineRenderer.GetPosition(1), touchPosition) > pluckDistance) {
							state = PlantState.Plucked;
						}
					}
					else {
						state = PlantState.Idle;
					}

					flower.localPosition = Vector2.Lerp(lineRenderer.GetPosition(1), targetStemPosition, touchSpeed * Time.deltaTime);
					lineRenderer.SetPosition(1, Vector2.Lerp(lineRenderer.GetPosition(1), targetStemPosition, touchSpeed * Time.deltaTime));

					break;

				case PlantState.Idle:

					flower.localPosition = Vector2.Lerp(lineRenderer.GetPosition(1), Vector2.up, touchSpeed * Time.deltaTime);
					lineRenderer.SetPosition(1, Vector2.Lerp(lineRenderer.GetPosition(1), Vector2.up, touchSpeed * Time.deltaTime));

					if (this.Touch != null) {
						state = PlantState.Pulling;
					}

					break;

				case PlantState.Plucked:
					if (this.Touch != null) {
						targetFlowerPosition = touchPosition;
						flower.localPosition = Vector2.Lerp(flower.localPosition, targetFlowerPosition, touchSpeed * 2 * Time.deltaTime);
					}
					else {
						state = PlantState.Wither;
					}

					break;
			}
		}

		#endregion


		// Growing

		private IEnumerator Growing() {
			float timeElapsed = 0;

			yield return new WaitForSeconds(2);

			while (timeElapsed <= 1) {
				timeElapsed += Time.deltaTime * growSpeed;
				lineRenderer.SetPosition(1, Vector2.Lerp(Vector2.zero, Vector2.up, growAnimationCurve.Evaluate(timeElapsed)));
				flower.localPosition = Vector2.Lerp(Vector2.zero, Vector2.up, growAnimationCurve.Evaluate(timeElapsed));
				flower.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, growAnimationCurve.Evaluate(timeElapsed));
				yield return 0;
			}

			state = PlantState.Idle;
		}

		// Plucked

		private IEnumerator Plucked() {
			float timeElapsed = 0;
			Vector2 startingPosition = lineRenderer.GetPosition(1);

			while (timeElapsed <= 1) {
				timeElapsed += Time.deltaTime * growSpeed;
				lineRenderer.SetPosition(1, Vector2.Lerp(startingPosition, Vector2.zero, growAnimationCurve.Evaluate(timeElapsed)));
				yield return 0;
			}
		}

		// Withering

		private IEnumerator Withering() {
			float timeElapsed = 0;
			Vector2 startingPosition = lineRenderer.GetPosition(1);

			while (timeElapsed <= 1) {
				timeElapsed += Time.deltaTime * 5;
				flower.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, growAnimationCurve.Evaluate(timeElapsed));
				yield return 0;
			}

			state = PlantState.Growing;
		}

		#region Touch

		// Touches

		public void OnTouchDown(TouchArea touchArea, Touch touch) {
			this.Touch = touch;
		}

		public void OnTouchUp(TouchArea touchArea, Touch touch) {
			this.Touch = null;
		}

		private Vector2 GetTouchPosition() {
			return new Vector2(this.Camera.ScreenToWorldPoint(this.Touch.Position).x, this.Camera.ScreenToWorldPoint(this.Touch.Position).y);
		}

		#endregion

	}

}
