namespace Mechanic {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SagoTouch;
    using Touch = SagoTouch.Touch;

    /// <summary>
    /// Handles a wheel that can be picked up and bounces on surfaces
    /// </summary>
    public class Wheel : MonoBehaviour, ISingleTouchObserver {

        #region Properties

        // Visible
        [SerializeField]
        private Camera Camera;

        [SerializeField]
        private Renderer Renderer;

        [Space]
        [Header("Animation")]
        [SerializeField]
        [Range(1, 20)]
        private float respawnAnimationSpeed;
        [SerializeField]
        private AnimationCurve respawnAnimationCurve;

        [Space]
        [Header("Interaction")]
        [SerializeField]
        private float draggingSpeed;
        
        // ---

        // Defaults
        private PhysicsMaterial2D defaultPhysicsMaterial;
        private Vector2 defaultPosition;

        private Touch Touch;

        // Components
        private Rigidbody2D rb2d;

        // States
        private enum ObjectState {
            Active,
            Respawning,
            Inactive
        }

        private ObjectState state;

        #endregion

        // ---

        // Unity Defaults

        private void Awake() {
            rb2d = GetComponent<Rigidbody2D>();
            defaultPhysicsMaterial = rb2d.sharedMaterial;
            defaultPosition = transform.position;
        }

        private void OnEnable() {

            TouchDispatcher.Instance.Add(this, 0);
        }

        private void OnDisable() {

            TouchDispatcher.Instance.Remove(this);
        }

        private void FixedUpdate() {
            if (state == ObjectState.Active) {
                DefaultDragging();
                CheckBounds();
            }
        }


        // Dragging Functionality

        private void DefaultDragging() {
            if (this.Touch != null) {

                Vector2 currentVelocity;
                Vector2 currentPosition = transform.position;
                Vector2 touchPosition = Camera.ScreenToWorldPoint(this.Touch.Position);

                Vector2 targetPosition = touchPosition - currentPosition;

                currentVelocity = targetPosition * draggingSpeed / Time.fixedDeltaTime;

                Vector2 targetVelocity = currentVelocity - rb2d.velocity;

                Vector2 force = rb2d.mass * targetVelocity / Time.fixedDeltaTime;

                rb2d.AddForce(force);
            }
        }


        // Out of bounds Functionality

        private void CheckBounds() {
            if (!this.Renderer.isVisible) {
                state = ObjectState.Respawning;
                StartCoroutine(Respawning());
            }
        }

        private IEnumerator Respawning() {
            float timeElapsed = 0;

            // ---

            transform.localScale = Vector3.zero;

            yield return new WaitForSeconds(1);

            // Reset velocity
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
            rb2d.angularVelocity = 0;
            transform.position = defaultPosition;

            while (timeElapsed <= 1) {
                timeElapsed += Time.deltaTime * respawnAnimationSpeed;

                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, respawnAnimationCurve.Evaluate(timeElapsed));
                yield return 0;
            }

            rb2d.isKinematic = false;
            state = ObjectState.Active;
        }


        // ISingleTouchObserver Implementation

        public bool OnTouchBegan(Touch touch) {

            // Actually check if the touch is within this object
            RaycastHit2D hit2D;
            Vector2 ray = Camera.ScreenToWorldPoint(touch.Position);

            hit2D = Physics2D.Raycast(ray, Vector3.back);

            if (hit2D.transform == transform) {
                rb2d.sharedMaterial = null;
                this.Touch = touch;
                return true;
            }
            else {
                return false;
            }
        }

        public void OnTouchMoved(Touch touch) {

        }

        public void OnTouchEnded(Touch touch) {
            this.Touch = null;
            rb2d.sharedMaterial = defaultPhysicsMaterial;
        }

        public void OnTouchCancelled(Touch touch) {

        }


    }

}
