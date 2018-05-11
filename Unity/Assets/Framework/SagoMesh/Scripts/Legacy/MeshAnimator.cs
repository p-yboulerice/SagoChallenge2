namespace SagoEngine {
    
    using System;
    using System.Collections;
    using System.IO;
    using UnityEngine;
	using UnityEngine.Rendering;
    
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshAnimator : MonoBehaviour {
        
        // ================================================================= //
        // Static Variables
        // ================================================================= //
        
        private static Material _defaultMaterial;
        
        
        // ================================================================= //
        // Static Properties
        // ================================================================= //
        
        public static Material DefaultMaterial {
            get {
                if (_defaultMaterial == null) {
                    _defaultMaterial = new Material(Shader.Find("Sago Sago/Core/Vertex Color Opaque"));
                    _defaultMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _defaultMaterial.name = "Default";
                }
                return _defaultMaterial;
            }
        }
        
        // ================================================================= //
        // Variables
        // ================================================================= //
                
        [NonSerialized]
        private Single _accumulatedTime;
        
        [NonSerialized]
        private MeshAnimationAsset _animation;
        
        [SerializeField]
        private MeshAnimationAsset _animationAsset;
        
        [SerializeField]
        private string _animationAssetPath;
		
		[SerializeField]
		private string _animationAssetGuid;
        
        [SerializeField]
        private Boolean _autoPlay;
        
        [NonSerialized]
        private Int32 _currentFrame;
        
        [SerializeField]
        private Int32 _defaultFrame;
        
        [SerializeField]
        private UnityEngine.Object _delegate;
        
        [SerializeField]
        private bool _isAsync;
        
        [NonSerialized]
        private Boolean _isPlaying;
        
        [SerializeField]
        private Int32 _loop;
        
        [NonSerialized]
        private MeshFilter _meshFilter;
        
        [NonSerialized]
        private MeshRenderer _meshRenderer;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        private Single AccumulatedTime {
            get { return this._accumulatedTime; }
            set { this._accumulatedTime = value; }
        }
        
        public MeshAnimationAsset Animation {
            get { return this._animation; }
            set { this._animation = value; }
        }
        
        public MeshAnimationAsset AnimationAsset {
            get { return _animationAsset; }
            set { _animationAsset = value; }
        }
        
        public string AnimationAssetPath {
            get { return _animationAssetPath; }
            set { _animationAssetPath = value; }
        }
		
		public string AnimationAssetGuid {
			get { return _animationAssetGuid; }
			set { _animationAssetGuid = value; }
		}
        
        public Boolean AutoPlay {
            get { return this._autoPlay; }
            set { this._autoPlay = value; }
        }
        
        public Bounds Bounds {
            get {
                return this.MeshRenderer.bounds;
            }
        }
        
        public Int32 CurrentFrame {
            get { return this._currentFrame; }
            private set { this._currentFrame = value; }
        }
        
        public Int32 CurrentIndex {
            get { 
                if (this.IsLoaded) {
                    return this.CurrentFrame % this.Animation.Meshes.Length;
                }
                return 0;
            }
        }
        
        public Int32 DefaultFrame {
            get { return _defaultFrame; }
            set { _defaultFrame = value; }
        }
        
        public IMeshAnimatorDelegate Delegate {
            get { return this._delegate as IMeshAnimatorDelegate; }
            set { this._delegate = value as UnityEngine.Object; }
        }
        
        public float Duration {
            get { return this.Animation != null ? this.Animation.Duration : -1f; }
        }
        
        public bool IsAsync {
            get { return _isAsync; }
            set { _isAsync = value; }
        }
        
        public Boolean IsComplete {
            get; private set;
        }
        
        public Boolean IsPlaying {
            get { return this._isPlaying; }
            private set { this._isPlaying = value; }
        }
        
        public Int32 LastFrame {
            get { 
                if (this.IsLoaded) {
                    if (this.Loop > 0) {
                        return (this.Animation.Meshes.Length - 1) + (this.Animation.Meshes.Length * this.Loop);
                    } else {
                        return (this.Animation.Meshes.Length - 1);
                    }
                }
                return 0;
            }
        }
        
        public Int32 LastIndex {
            get {
                if (this.IsLoaded) {
                    return this.Animation.Meshes.Length - 1;
                }
                return 0;
            }
        }
        
        public Int32 Loop {
            get { return this._loop; }
            set { this._loop = Math.Max(value, -1); }
        }
        
        public MeshFilter MeshFilter {
            get {
                if (_meshFilter == null) {
                    _meshFilter = this.GetComponent<MeshFilter>();
                }
                return _meshFilter;
            }
        }
        
        public MeshRenderer MeshRenderer {
            get {
                if (_meshRenderer == null) {
                    _meshRenderer = this.GetComponent<MeshRenderer>();
                }
                return _meshRenderer;
            }
        }
        
        /// <summary>
        /// TODO:  Luke please verify (stubbed in by RH)
        /// Gets the progress from [0..1].
        /// </summary>
        /// <value>The progress.</value>
        public float Progress {
            get {
                float progress = 0;
                if (this.Animation != null && this.Duration > 0) {
                    progress += (float)this.CurrentIndex / (float)this.LastIndex;
                    progress += this.AccumulatedTime / this.Duration;
                }
                return Mathf.Clamp(progress, 0f, 1f);
            }
        }
        
        public MeshAsset SharedMesh {
            get { return this.IsLoaded ? this.Animation.Meshes[this.CurrentIndex] : null; }
        }
        
        
        // ================================================================= //
        // Private Methods
        // ================================================================= //
        
        private Int32 ClampFrame(Int32 frame) {
            if (this.IsLoaded) {
                if (this.Loop < 0) {
                    return frame % this.Animation.Meshes.Length;
                } else {
                    return Math.Min(frame, this.LastFrame);
                }
            }
            return 0;
        }
                
        private void Step(Single time) {
            
            if (this.IsPlaying == false || this.IsLoaded == false || this.Animation.Meshes.Length == 1 || this.MeshRenderer.enabled == false) {
                this.AccumulatedTime = 0f;
                return;
            }
            
            this.AccumulatedTime = this.AccumulatedTime + time;
            
            Int32 accumulatedFrames;
            accumulatedFrames = (Int32)Math.Floor(this.AccumulatedTime / this.Animation.Interval);
            
            for (Int32 index = 0; index < accumulatedFrames; index++) {
                    
                // the loop is finite, it's already on the last frame
                if (this.Loop >= 0 && this.CurrentFrame == this.LastFrame) {
                    this.IsComplete = true;
                    this.Stop();
                    break;
                }
                
                this.CurrentFrame = this.ClampFrame(this.CurrentFrame + 1);
                this.MeshFilter.sharedMesh = this.SharedMesh;
                
                if (this.Delegate != null) {
                    this.Delegate.OnMeshAnimatorFrame(this);
                }
                
            }
            
            this.AccumulatedTime = this.AccumulatedTime - (accumulatedFrames * this.Animation.Interval);
            
        }
        
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public void Jump(Int32 frame) {
                        
            if (this.Animation == null) {
                this.MeshFilter.sharedMesh = null;
                return;
            }
            
            this.AccumulatedTime = 0f;
            this.CurrentFrame = this.ClampFrame(frame);
            this.MeshFilter.sharedMesh = this.SharedMesh;
            
        }
        
        public void Play() {
            
            if (!Application.isPlaying) {
                return;
            }
            
            if (!this.IsPlaying) {
                this.AccumulatedTime = 0f;
                this.IsComplete = false;
                this.IsPlaying = true;
                this.Jump(this.CurrentFrame == this.LastFrame ? 0 : this.CurrentFrame);
                if (this.Delegate != null) {
                    this.Delegate.OnMeshAnimatorPlay(this);
                    this.Delegate.OnMeshAnimatorFrame(this);
                }
            }
            
        }
        
        public void Play(Int32 frame) {
            this.Jump(frame);
            this.Play();
        }
        
        public void Stop() {
            
            if (!Application.isPlaying) {
                return;
            }
            
            if (this.IsPlaying) {
                this.AccumulatedTime = 0f;
                this.IsPlaying = false;
                if (this.Delegate != null) {
                    this.Delegate.OnMeshAnimatorStop(this);
                }
            }
            
        }
        
        public void Stop(Int32 frame) {
            this.Stop();
            this.Jump(frame);
        }
        
        
        // ================================================================= //
        // MonoBehaviour Methods
        // ================================================================= //
        
        public void Awake() {
            
            // setup renderer
            MeshRenderer meshRenderer = this.MeshRenderer;
            if (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial.shader == null) {
                meshRenderer.sharedMaterial = MeshAnimator.DefaultMaterial;
            }
            
            // setup delegate
            if (this.Delegate == null) {
                this.Delegate = GetComponent(typeof(IMeshAnimatorDelegate)) as IMeshAnimatorDelegate;
            }
            
            // setup animation
            if (this.Animation == null && this.AnimationAsset != null) {
                this.Animation = this.AnimationAsset;
                this.Animation.Path = this.AnimationAssetPath;
            }
            
            // setup mesh filter
            this.Jump(this.DefaultFrame);
            
        }
        
        public void Start() {
            if (this.AutoPlay) {
                 this.Play();
            }
        }
        
        public void Update() {
            
            #if UNITY_EDITOR
            if (Application.isPlaying == false) {
                
                // setup renderer
                MeshRenderer meshRenderer = this.MeshRenderer;
                if (meshRenderer != null && (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial.shader == null)) {
                    meshRenderer.receiveShadows = false;
                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    meshRenderer.sharedMaterial = MeshAnimator.DefaultMaterial;
                    meshRenderer.lightProbeUsage = LightProbeUsage.Off;
                    meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
                
                // animation added
                if (this.Animation == null && !string.IsNullOrEmpty(this.AnimationAssetGuid)) {
                    this.Animation = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(this.AnimationAssetGuid), typeof(MeshAnimationAsset)) as MeshAnimationAsset;
                    this.Jump(this.DefaultFrame);
                }
                
                // animation changed
                if (this.Animation != null && this.AnimationAssetGuid != AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this.Animation))) {
                    this.Animation = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(this.AnimationAssetGuid), typeof(MeshAnimationAsset)) as MeshAnimationAsset;
                    this.Jump(this.DefaultFrame);
                }
                
                // animation removed
                if (this.Animation != null && string.IsNullOrEmpty(this.AnimationAssetGuid)) {
                    this.Animation = null;
                    this.Jump(this.DefaultFrame);
				}

				// setup animation
				if (this.Animation == null && this.AnimationAsset != null) {
					this.Animation = this.AnimationAsset;
					this.Jump(this.DefaultFrame);
				}
                
            }
            #endif
            
            if (Application.isPlaying == true) {
                this.Step(Time.deltaTime);
            }
            
        }
        
        
        // ================================================================= //
        // Loading
        // ================================================================= //
        
        internal IEnumerator LoadCoroutine() {
            
            this.IsLoading = true;
            yield return this.LoadOperation.Wait();
            this.Animation = this.LoadOperation != null ? this.LoadOperation.Asset : null;
            this.IsLoading = false;
            
            #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
            #endif
                
        }
        
        internal MeshLoadOperation LoadOperation {
            get; set;
        }
        
        
        public bool IsLoaded {
            get { return this.Animation != null && this.Animation.Meshes != null && this.Animation.Meshes.Length > 0; }
        }
        
        public bool IsLoading {
            get; private set;
        }
        
        public Coroutine Load() {
            return this.Load(0);
        }
        
        public Coroutine Load(int priority) {
            if (!MeshLoader.Instance) {
                return null;
            }
            if (!this.IsAsync || this.IsLoaded || string.IsNullOrEmpty(this.AnimationAssetGuid)) {
                return null;
            }
            if (this.LoadOperation == null) {
                this.LoadOperation = MeshLoader.Instance.Load(this.AnimationAssetGuid, priority);
            } else {
                MeshLoader.Instance.Load(this.AnimationAssetGuid, priority);
            }
            if (this.IsLoading == false) {
                this.IsLoading = true;
                this.StartCoroutine(this.LoadCoroutine());
            }
            return this.LoadOperation.Wait();
        }
        
        public void Unload() {
            if (this.IsAsync) {
                this.Animation = null;
                this.MeshFilter.sharedMesh = null;
                this.LoadOperation = null;
                this.IsLoading = false;
                #if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
                #endif
            }
        }
        
        
        // ================================================================= //
        // Crop
        // ================================================================= //
        
        private MeshAnimatorCropMediator CropMediator {
            get; set;
        }
        
        public Bounds Crop {
            get {
            
                // setup 
                if (this.CropMediator == null) {
                    this.CropMediator = new MeshAnimatorCropMediator(this);
                }
                
                // update
                return this.CropMediator.Update();
                
            }
        }
        
    }
        
        
    // ================================================================= //
    // Helper Classes
    // ================================================================= //
        
    class MeshAnimatorCropMediator {
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimationAsset Animation {
            get { return this.Animator ? this.Animator.Animation : null; }
        }
        
        public Bounds AnimationCrop {
            get; protected set;
        }
        
        public MeshAnimator Animator {
            get; protected set;
        }
        
        public Bounds AnimatorCrop {
            get; protected set;
        }
        
        public Matrix4x4 AnimatorMatrix {
            get; protected set;
        }
        
        public bool IsValid {
            get {
                if (this.Animation == null) {
                    return false;
                }
                if (this.Animation.Crop != this.AnimationCrop) {
                    return false;
                }
                if (this.Transform.localToWorldMatrix != this.AnimatorMatrix) {
                    return false;
                }
                return true;
            }
        }
        
        public Transform Transform {
            get; protected set;
        }
        
        
        // ================================================================= //
        // Constructors
        // ================================================================= //
        
        public MeshAnimatorCropMediator(MeshAnimator animator) {
            this.Animator = animator;
            this.Transform = animator.GetComponent<Transform>();
        }
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        public Bounds Invalidate() {
            this.AnimationCrop = new Bounds();
            this.AnimatorCrop = new Bounds();
            this.AnimatorMatrix = Matrix4x4.identity;
            return this.AnimatorCrop;
        }
        
        public Bounds Update() {
            
            if (this.IsValid == false) {
            
                // invalidate
                this.Invalidate();
                
                // calculate
                if (this.Animation != null && this.Transform != null) {
                
                    Bounds animationCrop;
                    animationCrop = this.Animation.Crop;
                    
                    Bounds animatorCrop;
                    animatorCrop = new Bounds();
                    
                    Vector3 tl = Vector3.zero;
                    tl.x = animationCrop.center.x - animationCrop.extents.x;
                    tl.y = animationCrop.center.y + animationCrop.extents.y;
                    tl = this.Transform.TransformPoint(tl);
                    
                    Vector3 tr = Vector3.zero;
                    tr.x = animationCrop.center.x + animationCrop.extents.x;
                    tr.y = animationCrop.center.y + animationCrop.extents.y;
                    tr = this.Transform.TransformPoint(tr);
                    
                    Vector3 br = Vector3.zero;
                    br.x = animationCrop.center.x + animationCrop.extents.x;
                    br.y = animationCrop.center.y - animationCrop.extents.y;
                    br = this.Transform.TransformPoint(br);
                    
                    Vector3 bl = Vector3.zero;
                    bl.x = animationCrop.center.x - animationCrop.extents.x;
                    bl.y = animationCrop.center.y - animationCrop.extents.y;
                    bl = this.Transform.TransformPoint(bl);
                    
                    Vector3 min = new Vector3(Single.MaxValue, Single.MaxValue, 0f);
                    min.x = Math.Min(min.x, tl.x);
                    min.x = Math.Min(min.x, tr.x);
                    min.x = Math.Min(min.x, br.x);
                    min.x = Math.Min(min.x, bl.x);
                    min.y = Math.Min(min.y, tl.y);
                    min.y = Math.Min(min.y, tr.y);
                    min.y = Math.Min(min.y, br.y);
                    min.y = Math.Min(min.y, bl.y);
                    min.z = this.Transform.position.z;
                    
                    Vector3 max = new Vector3(Single.MinValue, Single.MinValue, 0f);
                    max.x = Math.Max(max.x, tl.x);
                    max.x = Math.Max(max.x, tr.x);
                    max.x = Math.Max(max.x, br.x);
                    max.x = Math.Max(max.x, bl.x);
                    max.y = Math.Max(max.y, tl.y);
                    max.y = Math.Max(max.y, tr.y);
                    max.y = Math.Max(max.y, br.y);
                    max.y = Math.Max(max.y, bl.y);
                    max.z = this.Transform.position.z;
                    animatorCrop.SetMinMax(min, max);
                    
                    this.AnimationCrop = animationCrop;
                    this.AnimatorCrop = animatorCrop;
                    this.AnimatorMatrix = this.Transform.localToWorldMatrix;
                    
                }
                
            }
            
            return this.AnimatorCrop;
            
        }
        
    }
    
}
        