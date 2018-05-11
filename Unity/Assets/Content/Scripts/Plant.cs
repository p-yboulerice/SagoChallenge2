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
       
		private TouchAreaObserver touchAreaObserver;
		private LineRenderer lineRenderer;
		private Touch Touch;

        // States
		private enum PlantState
		{
			Idle,
            Growing,
            Pulling
		}

		private PlantState m_state;

		private PlantState state
		{
			get{
				return m_state;
			}
			set{
				m_state = value;
				if (state == PlantState.Growing)
				{
					StartCoroutine(Growing());
				}
			}
		}

		// ---

		// Visible in Inspector

		[SerializeField]
		private Camera Camera;

		[SerializeField]
		[Range (1, 20)]
		private float growSpeed;

		[SerializeField]
		private AnimationCurve growAnimationCurve;

        

        // Unity Defaults

		private void Awake() {
         
            // Components
			touchAreaObserver = GetComponent<TouchAreaObserver>();
			lineRenderer = GetComponent<LineRenderer>();                  

            // Line Renderer
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, transform.position);

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

		private void Update() {

			switch (state){
				case PlantState.Pulling :
					
					if (this.Touch != null) {
                        lineRenderer.SetPosition(1, new Vector3(this.Camera.ScreenToWorldPoint(this.Touch.Position).x, this.Camera.ScreenToWorldPoint(this.Touch.Position).y, transform.position.z));
                    }

					break;
			}         
		}


		// Growing

		private IEnumerator Growing()
		{
			float timeElapsed = 0;

			// ---
			yield return new WaitForSeconds(2);

			while(timeElapsed <= 1)
			{
				timeElapsed += Time.deltaTime * growSpeed;
				lineRenderer.SetPosition(1, Vector2.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), growAnimationCurve.Evaluate(timeElapsed)));
				yield return 0;
			}

			state = PlantState.Idle;
		}

		// Touches

		public void OnTouchDown(TouchArea touchArea, Touch touch){
			this.Touch = touch;
		}

		public void OnTouchUp(TouchArea touchArea, Touch touch){
			this.Touch = null;
		}

	}

}
