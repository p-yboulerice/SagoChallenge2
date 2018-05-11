namespace SagoMesh {
    
    using SagoMesh;
    using SagoMesh.Internal;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
	using UnityEngine.Rendering;
    
    public enum MeshAnimatorDirection {
        Forward,
        Reverse
    }
    
    /// <summary>
    /// Defines how the animator will adjust playback on slow devices by 
    /// changing how the <see cref="Step" /> method calculates the elapsed 
    /// time between steps.
    /// </summary>
    public enum MeshAnimatorFramerate {
        /// <summary>
        /// The elapsed time between steps is variable.
        /// The number of update calls required to play an animation depends on the device.
        /// The animator uses <see cref="Time#deltaTime" /> as the elapsed time.
        /// The animator will play at the animation's framerate.
        /// The animator may drop frames if necessary.
        /// The animator's framerate will not be dependent on Unity's framerate.
        /// </summary>
        Variable = 0,
        /// <summary>
        /// The elapsed time between steps is fixed.
        /// The number of update calls required to play an animation is the same on all devices.
        /// The animator uses 1f / <see cref="Application#targetFrameRate" /> as the elapsed time.
        /// The animator will play at less than the animation's framerate when Unity's actual framerate is lower than the target framerate.
        /// The animator will not drop frames.
        /// The animator's framerate is dependent on Unity's framerate.
        /// </summary>
        Fixed = 1
    }
    
    public class MeshAnimator : MonoBehaviour {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected bool m_AutoPlay;
        
        [SerializeField]
        protected int m_CurrentIndex;
        
        [SerializeField]
        protected MeshAnimatorDirection m_Direction;
        
        [SerializeField]
        [Tooltip(
            "How the animator will adjust playback on slow devices.\n\n" +
            "Variable: The number of update calls depends on the device, the animator will drop frames on slow devices.\n\n" + 
            "Fixed: The number of update calls is always the same, the animator will play slower on slow devices."
        )]
        protected MeshAnimatorFramerate m_Framerate;
        
        [SerializeField]
        protected bool m_IsLoop;
        
        [SerializeField]
        protected MeshAnimatorLayer[] m_Layers;
        
        [SerializeField]
        protected bool m_Locked;
        
        [SerializeField]
        protected string m_Version;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimation Animation {
            get { 
                IMeshAnimatorSource source;
                source = this.GetComponent(typeof(IMeshAnimatorSource)) as IMeshAnimatorSource;
                return source != null ? source.Animation : null;
            }
        }
        
        public bool AutoPlay {
            get { return m_AutoPlay; }
            protected set {
                if (m_AutoPlay != value) {
                    m_AutoPlay = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public Bounds Bounds {
            get {
                
                Transform transform;
                transform = this.GetComponent<Transform>();
                
                Bounds result;
                result = new Bounds();
                    
                Vector2 min;
                min = new Vector2(Mathf.Infinity, Mathf.Infinity);
                
                Vector2 max;
                max = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
                
                if (this.Layers != null) {
                    foreach (MeshAnimatorLayer layer in this.Layers) {
                        
                        Renderer renderer = layer.Renderer;
                        
                        if (renderer != null) {
                            
                            Bounds bounds;
                            bounds = renderer.bounds;
                            
							if (bounds.size == Vector3.zero) {
								bounds.center = renderer.GetComponent<Transform>().position;
							}
                            
                            min.x = Mathf.Min(min.x, bounds.min.x);
                            min.y = Mathf.Min(min.y, bounds.min.y);
                            max.x = Mathf.Max(max.x, bounds.max.x);
                            max.y = Mathf.Max(max.y, bounds.max.y);
                            
                        }
                        
                    }
                }
                
                min.x = (min.x != Mathf.Infinity) ? min.x : transform.position.x;
                min.y = (min.y != Mathf.Infinity) ? min.y : transform.position.y;
                max.x = (max.x != Mathf.NegativeInfinity) ? max.x : transform.position.x;
                max.y = (max.y != Mathf.NegativeInfinity) ? max.y : transform.position.y;
                
                result.center = (Vector3)(min + 0.5f * (max - min)) + transform.position.z * Vector3.forward;
                result.size = (Vector3)(max - min);
                
                return result;
                
            }
        }
        
        public int CurrentIndex {
            get { return m_CurrentIndex; }
            protected set {
                int index = this.Normalize(value);
                if (m_CurrentIndex != index) {
                    m_CurrentIndex = index;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public MeshAnimatorDirection Direction {
            get { return m_Direction; }
            set { 
                if (m_Direction != value) {
                    m_Direction = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public float Duration {
            get { return this.Animation != null ? this.Animation.Duration : -1f; }
        }
        
        protected float ElapsedTime {
            get; set;
        }
        
        public MeshAnimatorFramerate Framerate {
            get { return m_Framerate; }
            set {
                if (m_Framerate != value) {
                    m_Framerate = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public bool IsComplete {
            get;
            protected set;
        }
        
        public bool IsLoop {
            get { return m_IsLoop; }
            set { 
                if (m_IsLoop != value) {
                    m_IsLoop = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public bool IsPlaying {
            get;
            protected set;
        }
        
        public bool IsStepping {
            get; protected set;
        }
        
        public bool IsVisible {
            get {
                if (this.Layers != null) {
                    foreach (MeshAnimatorLayer layer in this.Layers) {
                        if (layer.Renderer.isVisible) {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        
        public int LastIndex {
            get { return Mathf.Max(0, this.Animation ? this.Animation.Frames.Length - 1 : 0); }
        }
        
        public MeshAnimatorLayer[] Layers {
            get { return m_Layers; }
            protected set {
                if (!ArrayUtil.Equal<MeshAnimatorLayer>(m_Layers, value)) {
                    m_Layers = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public float Progress {
            get {
                float progress = 0;
                if (this.Animation != null && this.Duration > 0) {
                    progress += (float)this.CurrentIndex / (float)this.LastIndex;
                    if (this.IsStepping) {
                        progress += this.ElapsedTime / this.Animation.Duration;
                    }
                }
                return Mathf.Clamp(progress, 0f, 1f);
            }
        }
        
        public string Version {
            get { return m_Version; }
            protected set { 
                if (m_Version != value) {
                    m_Version = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        
        // ================================================================= //
        // MonoBehaviour Methods
        // ================================================================= //
        
        void Awake() {
            this.Pull();
            if (this.AutoPlay) {
                this.Play();
            }
        }
        
        void OnEnable() {
            if (this.IsPlaying && !this.IsStepping) {
                this.StartCoroutine(this.Step());
            }
        }
        
        void OnDisable() {
            this.StopAllCoroutines();
            this.IsStepping = false;
        }
        
        
        // ================================================================= //
        // Playback Methods
        // ================================================================= //
        
        public void Forward() {
            this.Direction = MeshAnimatorDirection.Forward;
        }
        
        public void Jump() {
            this.Jump(this.CurrentIndex);
        }
        
        public void Jump(int index) {
            
            // get the previous index
            int previous = this.CurrentIndex;
            
            // set the current index
            this.CurrentIndex = index;
            
            // check if the index changed
            bool dirty = (this.CurrentIndex != previous);
            

            // update mesh filters
			if (this.Layers != null) {
	            for (int layerIndex = 0; layerIndex < this.Layers.Length; layerIndex++) {
	                MeshAnimatorLayer layer = null;
	                MeshFilter filter = null;
	                if ((layer = this.Layers[layerIndex]) && (filter = layer.MeshFilter)) {
	                    filter.sharedMesh = this.Animation ? this.Animation.Frames[this.CurrentIndex].Meshes[layerIndex] : null;
	                }
	            }
			}

            if (dirty) {
                this.OnJump();
            }
            
        }
        
        public void Play() {
            if (!this.IsPlaying) {
                if (this.Direction == MeshAnimatorDirection.Forward && this.CurrentIndex == this.LastIndex) {
                    this.Jump(0);
                }
                if (this.Direction == MeshAnimatorDirection.Reverse && this.CurrentIndex == 0) {
                    this.Jump(this.LastIndex);
                }
                this.IsComplete = false;
                this.IsPlaying = true;
                this.OnPlay();
            }
            if (!this.IsStepping) {
                this.StartCoroutine(this.Step());
            }
        }
        
        public void Play(int index) {
            this.Jump(index);
            this.Play();
        }
        
        public void Reverse() {
            this.Direction = MeshAnimatorDirection.Reverse;
        }
        
        public void Stop() {
            if (this.IsPlaying) {
                this.IsPlaying = false;
                this.OnStop();
            }
        }
        
        public void Stop(int frameIndex) {
            this.Stop();
            this.Jump(frameIndex);
        }
        
        
        // ================================================================= //
        // Step Methods
        // ================================================================= //
        
        protected IEnumerator Step() {
            
            this.ElapsedTime = 0;
            this.IsStepping = true;
            
            while (true) {
                
                if (this.Animation == null) {
                    this.Stop();
                }
                
                if (this.IsPlaying) {
                    yield return null;
                }

                // Prior to Sept-2016, if we returned from the yield and IsPlaying
                // was false, one more frame could Jump.  Now it checks and exits
                // immediately.  This makes intuitive sense, but there could be
                // some edge cases that relied on the old behaviour.

                if (!this.IsPlaying) {
                    break;
                }
                
				int targetFrameRate;
				targetFrameRate = (Application.targetFrameRate > -1) ? Application.targetFrameRate : 60;

                float fixedDeltaTime;
				fixedDeltaTime = (targetFrameRate > 0) ? 1f / targetFrameRate : 0f;
                
                float variableDeltaTime;
                variableDeltaTime = Time.deltaTime;
                
                float deltaTime;
                deltaTime = this.Framerate == MeshAnimatorFramerate.Variable ? variableDeltaTime : fixedDeltaTime;
                
                this.ElapsedTime += deltaTime;
                int elapsedFrames = Mathf.FloorToInt(this.ElapsedTime * this.Animation.FramesPerSecond);
                this.ElapsedTime -= Mathf.Floor(this.ElapsedTime * this.Animation.FramesPerSecond) / this.Animation.FramesPerSecond;
                
                if (this.Direction == MeshAnimatorDirection.Forward) {
                    this.StepForward(elapsedFrames);
                } else {
                    this.StepReverse(elapsedFrames);
                }
                
            }

			this.IsStepping = false;
            
        }
        
        protected void StepForward(int count) {
            for (int index = 0; index < count; index++) {
                if (this.IsLoop == false && this.CurrentIndex == this.LastIndex) {
                    this.IsComplete = true;
                    this.Stop();
                    break;
                }
                this.Jump(this.CurrentIndex + 1);
            }
        }
        
        protected void StepReverse(int count) {
            for (int index = 0; index < count; index++) {
                if (this.IsLoop == false && this.CurrentIndex == 0) {
                    this.IsComplete = true;
                    this.Stop();
                    break;
                }
                this.Jump(this.CurrentIndex - 1);
            }
        }
        
        
        // ================================================================= //
        // Event Methods
        // ================================================================= //
        
        protected void OnJump() {
            if (Application.isPlaying) {
                foreach (IMeshAnimatorObserver observer in this.GetComponents(typeof(IMeshAnimatorObserver))) {
                    observer.OnMeshAnimatorJump(this);
                }
            }
        }
        
        protected void OnPlay() {
            if (Application.isPlaying) {
                foreach (IMeshAnimatorObserver observer in this.GetComponents(typeof(IMeshAnimatorObserver))) {
                    observer.OnMeshAnimatorPlay(this);
                }
            }
        }
        
        protected void OnStop() {
            if (Application.isPlaying) {
                foreach (IMeshAnimatorObserver observer in this.GetComponents(typeof(IMeshAnimatorObserver))) {
                    observer.OnMeshAnimatorStop(this);
                }
            }
        }
        
        
        // ================================================================= //
        // Source Methods
        // ================================================================= //
        
        public bool Dirty {
            get {
                
                IMeshAnimatorSource source;
                source = this.GetComponent(typeof(IMeshAnimatorSource)) as IMeshAnimatorSource;
                
                string version;
                version = source != null && source.Animation != null ? source.Animation.Version : null;
                
                return this.Version != version;
                
            }
        }
        
        public bool Locked {
            get { return m_Locked; }
            set { 
                if (m_Locked != value) {
                    m_Locked = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public void Pull() {
            
            IMeshAnimatorSource source;
            source = this.GetComponent(typeof(IMeshAnimatorSource)) as IMeshAnimatorSource;
            
            string version;
            version = source != null && source.Animation != null ? source.Animation.Version : null;
            
            if (this.Version != version) {
                
                this.CurrentIndex = this.CurrentIndex;
                this.ElapsedTime = 0f;
                this.Version = version;

                if (!this.Locked) {
                	this.Build();
                }
                this.Jump();
                
            }
            
        }
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        protected void Build() {
            
            MeshAnimatorLayer[] layers;
            layers = new MeshAnimatorLayer[this.Animation ? this.Animation.Layers.Length : 0];
            
            // find existing layers, remove unused layers
            for (int childIndex = this.GetComponent<Transform>().childCount - 1; childIndex >= 0; childIndex--) {
                
                MeshAnimatorLayer layer;
                layer = this.GetComponent<Transform>().GetChild(childIndex).GetComponent<MeshAnimatorLayer>();
                
                if (layer != null) {
                    
                    int layerIndex;
                    layerIndex = MeshAnimatorLayer.NameToIndex(layer.name);
                    
                    if (layerIndex >= 0 && layerIndex < layers.Length && layers[layerIndex] == null) {
                        layers[layerIndex] = layer;
                    } else if (Application.isPlaying) {
                        Destroy(layer.gameObject);
                    } else {
                        DestroyImmediate(layer.gameObject, false);
                    }
                    
                }
                
            }
            
            // create missing layers
            for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++) {
                
                MeshAnimatorLayer layer;
                layer = layers[layerIndex];
                
                if (layer == null) {
                    
                    Transform layerTransform;
                    layerTransform = new GameObject().GetComponent<Transform>();
                    layerTransform.name = MeshAnimatorLayer.IndexToName(layerIndex);
                    layerTransform.parent = this.GetComponent<Transform>();
                    layerTransform.localEulerAngles = Vector3.zero;
                    layerTransform.localPosition = Vector3.zero;
                    layerTransform.localScale = Vector3.one;
                    
                    MeshRenderer layerRenderer;
                    layerRenderer = layerTransform.gameObject.AddComponent<MeshRenderer>();
					layerRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    layerRenderer.receiveShadows = false;
					layerRenderer.lightProbeUsage = LightProbeUsage.Off;
					layerRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                    layerRenderer.sharedMaterial = this.FindDefaultMaterial();
                    
                    layer = layerTransform.gameObject.AddComponent<MeshAnimatorLayer>();
                    layers[layerIndex] = layer;
                    
                }
                
            }
            
            this.Layers = layers;
            
        }
        
        protected Material FindDefaultMaterial() {
            #if UNITY_EDITOR
                
                string scriptPath;
                scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
                
                string materialPath;
                materialPath = scriptPath.Replace("Scripts/MeshAnimator.cs", "Materials/OpaqueMesh.mat");
                
                return UnityEditor.AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
                
            #else
                
                return null;
                
            #endif
        }
        
        public float IndexToRatio(int index) {
            
            float ratio = 0f;
            
            if (this.Animation != null) {
				ratio = Mathf.Clamp01((float)index / (float)this.Animation.Frames.Length);
            }
            
            return ratio;
            
        }
        
        protected int Normalize(int index) {
            int clamped = 0;
            if (this.Animation != null) {
                if (this.IsLoop) {
                    clamped = index;
                    clamped = clamped % (int)this.Animation.Frames.Length;
                    clamped = clamped < 0 ? clamped + this.Animation.Frames.Length : clamped;
                } else {
                    clamped = index;
                    clamped = Mathf.Clamp(clamped, 0, Mathf.Max(this.Animation.Frames.Length - 1, 0));
                }
            }
            return clamped;
        }
        
        public int RatioToIndex(float ratio) {
            
            int index = 0;
            
            if (this.Animation != null) {
                index = Mathf.FloorToInt(ratio * (this.Animation.Frames.Length));
                index = Mathf.Clamp(index, 0, this.Animation.Frames.Length - 1);
            }
            
            return index;
            
        }
        
        
    }
    
}