namespace SagoEngine {
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
        
    // ================================================================= //
    // Classes
    // ================================================================= //
    
    public class MeshSetElement {
        
        public MeshAnimator Animator {
            get; private set;
        }
        
        public String Key {
            get; set;
        }
        
        public MeshRenderer Renderer {
            get; private set;
        }
        
        public MeshSetElement(String key, MeshAnimator animator) {
            this.Animator = animator;
            this.Key = key;
            this.Renderer = animator != null ? animator.GetComponent<MeshRenderer>() : null;
        }
        
        public void Toggle(Boolean visible) {
            if (this.Renderer) {
                this.Renderer.enabled = visible;
            }
        }
        
    }
        
    
    // ================================================================= //
    // Component
    // ================================================================= //
    
    public class MeshSet : MonoBehaviour {
        
        // ================================================================= //
        // Delegates
        // ================================================================= //
        
        public delegate void Block(MeshSetElement element);
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
		public MeshSetElement ActiveElement {
			get; private set;
		}
		
        private Dictionary<String, MeshSetElement> Elements {
            get {
				mElements = (mElements != null) ? mElements : InitElements();
				return mElements;
			}
        }
        
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public void Activate(String key, Block activate) {
            this.Activate(key, activate, element => {
                if (element.Animator) {
                    element.Animator.Stop();
                }
            });
        }
        
        public void Activate(String key, Block activate, Block deactivate) {
            this.Activate(this.Find(key), activate, deactivate);
        }
        
        public void Activate(MeshSetElement element, Block activate) {
            this.Activate(element, activate, other => {
                if (other.Animator) {
                    other.Animator.Stop();
                }
            });
        }
        
        public void Activate(MeshSetElement element, Block activate, Block deactivate) {
            
            foreach (MeshSetElement other in this.Elements.Values) {
                if (other != element) {
                    deactivate(other);
                    other.Toggle(false);
                }
            }
            
            if (element != null) {
                element.Toggle(true);
                activate(element);
				ActiveElement = element;
            }
            
        }
        
        public void Each(Block action) {
            foreach (MeshSetElement element in this.Elements.Values) {
                action(element);
            }
        }
        
        public MeshSetElement Find(String key) {
            if (!string.IsNullOrEmpty(key) && this.Elements.ContainsKey(key)) {
                return this.Elements[key];
            }
            return null;
        }
        
        public MeshAnimator FindAnimator(String key) {
            MeshSetElement element;
            if ((element = this.Find(key)) != null) {
                return element.Animator;
            }
            return null;
        }
        
        public void Jump(String key, Int32 frame) {
            this.Activate(key, (element) => { 
                if (element.Animator) {
                    element.Animator.Jump(frame);
                }
            });
        }
        
        public void Play(String key) {
            this.Activate(key, (element) => { 
                if (element.Animator) {
                    element.Animator.Play();
                }
            });
        }
        
        public void Play(String key, Int32 frame) {
            this.Activate(key, (element) => { 
                if (element.Animator) {
                    element.Animator.Jump(frame);
                    element.Animator.Play();
                }
            });
        }
        
        public void Stop(String key) {
            this.Activate(key, (element) => { 
                if (element.Animator) {
                    element.Animator.Stop();
                }
            });
        }
        
		
		// ================================================================= //
        // Member Variables
        // ================================================================= //
		
		private Dictionary<String,MeshSetElement> mElements;
		
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        private String GetKey(Component component) {
            String key = null;
            while (component != null && component.GetComponent<Transform>() != this.GetComponent<Transform>()) {
                if (key == null) {
                    key = component.name;
                } else {
                    key = component.name + "/" + key;
                }
                component = component.GetComponent<Transform>().parent;
            }
            return key;
        }
        
		private Dictionary<String,MeshSetElement> InitElements() {
			
			MeshSetElement element;
            element = null;
            
            Dictionary<String,MeshSetElement> elements;
            elements = new Dictionary<String,MeshSetElement>();
            
            foreach (MeshAnimator animator in gameObject.GetComponentsInChildren<MeshAnimator>(true)) {
                
                MeshSetElement other;
                other = new MeshSetElement(this.GetKey(animator), animator);
                
                if (element == null && other.Animator.gameObject.activeSelf == true) {
                    element = other;
                } else {
                    if (other.Animator) {
                        other.Animator.Stop();
                    }
                    other.Toggle(false);
                }
                // try {
                    elements.Add(other.Key, other);
                // } catch(Exception e) {
                //     Debug.Log(e);
                //     Debug.Log(other.Animator.GetPathInHierarchy());
                // }
            }
            
            return elements;
		}
        
        // ================================================================= //
        // MonoBehaviour Methods
        // ================================================================= //
        
		public void Awake() {
            if (mElements == null) {
                mElements = InitElements();
            }
		}
		
    }
    
}