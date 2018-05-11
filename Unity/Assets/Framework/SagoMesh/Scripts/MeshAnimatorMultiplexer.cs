namespace SagoMesh {
    
    using SagoMesh;
    using SagoMesh.Internal;
    using System.Collections.Generic;
    using UnityEngine;
    
    public enum MeshAnimatorMultiplexerEditMode {
        Animator,
        Everything 
    }
    
    public enum MeshAnimatorMultiplexerState {
        Unknown,
        Generate,
        Ready
    }
    
    public class MeshAnimatorMultiplexer : MonoBehaviour {
        
        // ================================================================= //
        // Delegates
        // ================================================================= //
        
        public delegate void Block(MeshAnimator animator);
        
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected MeshAnimator m_Animator;
        
        [System.NonSerialized]
        protected MeshAnimator[] m_Animators;
        
        [System.NonSerialized]
        protected Dictionary<string,MeshAnimator> m_AnimatorsByKey;
        
        [SerializeField]
        protected MeshAnimatorMultiplexerEditMode m_EditMode;
        
        [System.NonSerialized]
        protected string[] m_Keys;
        
        [System.NonSerialized]
        protected Dictionary<MeshAnimator,string> m_KeysByAnimator;
        
        [SerializeField]
        protected LayerMask m_LayerMask;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimator Animator {
            get {
                if (m_Animator && !this.Contains(m_Animator)) {
                    m_Animator = null;
                    AssetUtil.SetDirty(this);
                }
                return m_Animator;
            }
            protected set {
                if (m_Animator != value) {
                    m_Animator = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public MeshAnimator[] Animators {
            get {
                this.Check();
                return m_Animators;
            }
        }
        
        protected Dictionary<string,MeshAnimator> AnimatorsByKey {
            get {
                this.Check();
                return m_AnimatorsByKey;
            }
        }
        
        public MeshAnimatorMultiplexerEditMode EditMode {
            get { return Application.isPlaying ? MeshAnimatorMultiplexerEditMode.Animator : m_EditMode; }
            set {
                if (m_EditMode != value) {
                    m_EditMode = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public string[] Keys {
            get {
                this.Check();
                return m_Keys;
            }
        }
        
        protected Dictionary<MeshAnimator,string> KeysByAnimator {
            get {
                this.Check();
                return m_KeysByAnimator;
            }
        }
        
        public LayerMask LayerMask {
            get { return m_LayerMask; }
            set {
                if (m_LayerMask != value) {
                    m_LayerMask = value;
                    this.Invalidate();
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public MeshAnimatorMultiplexerState State {
            get; protected set;
        }
        
        
        // ================================================================= //
        // Collection Methods
        // ================================================================= //
        
        public bool Contains(MeshAnimator animator) {
            this.Check();
            return this.KeysByAnimator.ContainsKey(animator);
        }
        
        public bool Contains(string key) {
            this.Check();
            return this.AnimatorsByKey.ContainsKey(key);
        }
        
        public MeshAnimator GetAnimator(string key) {
            return this.Contains(key) ? this.AnimatorsByKey[key] : null;
        }
        
        public string GetKey(MeshAnimator animator) {
            return this.Contains(animator) ? this.KeysByAnimator[animator] : null;
        }
        
        
        // ================================================================= //
        // Control Methods
        // ================================================================= //
        
        public void Jump(MeshAnimator animator) {
            this.Apply(
                animator,
                a => {
                    this.Toggle(a, true);
                    a.Jump();
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        public void Jump(MeshAnimator animator, int frame) {
            this.Apply(
                animator, 
                a => {
                    this.Toggle(a, true);
                    a.Jump(frame);
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        public void Play(MeshAnimator animator) {
            this.Apply(
                animator, 
                a => {
                    this.Toggle(a, true);
                    a.Play();
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        public void Play(MeshAnimator animator, int frame) {
            this.Apply(
                animator, 
                a => {
                    this.Toggle(a, true);
                    a.Play(frame);
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        public void Solo(MeshAnimator animator) {
            this.Apply(
                animator, 
                a => { 
                    this.Toggle(a, true);
                }, 
                a => { 
                    this.Toggle(a, false);
                }
            );
        }
        
        public void Stop(MeshAnimator animator) {
            this.Apply(
                animator, 
                a => {
                    this.Toggle(a, true);
                    a.Stop();
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        public void Stop(MeshAnimator animator, int frame) {
            this.Apply(
                animator, 
                a => {
                    this.Toggle(a, true);
                    a.Stop(frame);
                },
                a => {
                    this.Toggle(a, false);
                    a.Stop();
                }
            );
        }
        
        
        // ================================================================= //
        // MonoBehaviour Methods
        // ================================================================= //
        
        public void Awake() {
            this.Solo(this.Animator);
        }
        
        public void Reset() {
            this.LayerMask = LayerMask.NameToLayer("Everything");
            this.Invalidate();
        }
        
        
        // ================================================================= //
        // State Methods
        // ================================================================= //
        
        protected void Check() {
            if (this.State == MeshAnimatorMultiplexerState.Unknown) {
                this.Generate();
            }
        }
        
        protected void Generate() {
            this.State = MeshAnimatorMultiplexerState.Generate;
            {
                
                Dictionary<string,MeshAnimator> animators;
                animators = new Dictionary<string,MeshAnimator>();
                
                Dictionary<MeshAnimator,string> keys;
                keys = new Dictionary<MeshAnimator,string>();
                
                foreach (MeshAnimator animator in this.GetComponentsInChildren<MeshAnimator>(true)) {
                    if (this.LayerMask.Contains(animator) == true) {
                        string key = this.GenerateKey(animator);
                        if (!string.IsNullOrEmpty(key)) {
                            animators.Add(key, animator);
                            keys.Add(animator, key);
                        }
                    }
                }
                
                m_AnimatorsByKey = animators;
                m_Animators = new MeshAnimator[animators.Count];
                animators.Values.CopyTo(m_Animators, 0);
                
                m_KeysByAnimator = keys;
                m_Keys = new string[keys.Count];
                keys.Values.CopyTo(m_Keys, 0);
                
            }
            this.State = MeshAnimatorMultiplexerState.Ready;
            this.Solo(this.Animator);
        }
        
        protected string GenerateKey(MeshAnimator animator) {
            
            string key;
            key = null;
            
            if (animator) {
                
                Transform transform;
                transform = animator.GetComponent<Transform>();
                
                if (transform != this.GetComponent<Transform>() && transform.IsChildOf(this.GetComponent<Transform>())) {
                    while (transform != this.GetComponent<Transform>()) {
                        if (key == null) {
                            key = transform.name;
                        } else {
                            key = transform.name + "." + key;
                        }
                        transform = transform.parent;
                    }
                }
                
            }
            
            return key;
            
        }
        
        public void Invalidate() {
            this.State = MeshAnimatorMultiplexerState.Unknown;
        }
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        protected void Apply(MeshAnimator animator, Block activate, Block deactivate) {
            
            if (animator && !this.Contains(animator)) {
                animator = null;
            }
            
            foreach (MeshAnimator other in this.Animators) {
                if (other != null && other != animator) {
                    if (deactivate != null) {
                        deactivate(other);
                    }
                    this.Toggle(other, false);
                }
            }
            
            if (animator != null) {
                this.Toggle(animator, true);
                if (activate != null) {
                    activate(animator);
                }
            }
            
            this.Animator = animator;
            
        }
        
        protected void Toggle(MeshAnimator animator, bool enabled) {
            if (animator != null) {
                if (this.EditMode == MeshAnimatorMultiplexerEditMode.Everything) {
                    animator.gameObject.SetActive(true);
                } else {
                    animator.gameObject.SetActive(enabled);
                }
            }
        }
        
        
    }
    
}