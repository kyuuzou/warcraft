using UnityEngine;
using System.Collections;

public class SceneObject : MonoBehaviour {

    public Animator Animator { get; private set; }
    public Camera Camera { get; private set; }
    public Collider2D Collider { get; private set; }
    public GameObject GameObject { get; private set; }
    public MeshAnimator MeshAnimator { get; protected set; }
    public MeshFilter MeshFilter { get; protected set; }
    public MeshRenderer MeshRenderer { get; private set; }
    public Renderer Renderer { get; protected set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public SpriteRenderer SpriteRenderer { get; protected set; }
    public Transform Transform { get; private set; }
   
    private Mesh mesh;
    public Mesh Mesh {
        get { return this.mesh; }
        set {
            if (this.MeshFilter != null) {
                this.MeshFilter.mesh = value;
            }

            this.mesh = value;
        }
    }

    public bool Active { get; protected set; }
    public MeshData DefaultMeshData { get; private set; }
    public TransformData DefaultTransformData { get; private set; }

    protected bool LockedAnimations { get; set; }
    protected bool MouseIsOver { get; private set; }

    public bool InitializedExternals { get; private set; }
    protected bool InitializedInternals { get; private set; }

    protected virtual void Awake () {
        this.InitializeInternals ();
    }

    public virtual void Activate () {
        this.Active = true;

        this.InitializeInternals ();

        this.GameObject.SetActive (true);
        this.LockedAnimations = false;
    }

    public virtual void Deactivate () {
        this.Active = false;

        this.GameObject.SetActive (false);
    }

    public T GetCollider<T> () where T : Collider2D {
        return (T) this.Collider;
    }

    public virtual void InitializeExternals () {
        this.InitializeInternals ();

        this.InitializedExternals = true;
    }

    protected virtual void InitializeInternals () {
        if (this.InitializedInternals) {
            return;
        }

        this.InitializedInternals = true;

        this.Animator = this.GetComponent<Animator> ();
        this.Camera = this.GetComponent<Camera>();
        this.Collider = this.GetComponent<Collider2D>();
        this.GameObject = this.gameObject;
        this.MeshAnimator = this.GetComponent<MeshAnimator> ();
        this.MeshFilter = this.GetComponent<MeshFilter> ();
        this.MeshRenderer = this.GetComponent<MeshRenderer> ();
        this.Renderer = this.GetComponent<Renderer>();
        this.Rigidbody = this.GetComponent<Rigidbody2D>();
        this.SpriteRenderer = this.GetComponent<SpriteRenderer> ();
        this.Transform = this.transform;

        this.DefaultTransformData = new TransformData (this.Transform);

        this.Mesh = this.MeshFilter == null ? null : this.MeshFilter.sharedMesh;
        this.DefaultMeshData = this.Mesh == null ? null : new MeshData (this.Mesh);
    }

    public virtual void ManualReset () {

    }

    public virtual bool OnManualMouseDown () {
        return true;
    }

    public virtual void OnManualMouseEnter () {
        this.MouseIsOver = true;
    }
    
    public virtual void OnManualMouseExit () {
        this.MouseIsOver = false;
    }
    
    public virtual void OnManualMouseUp () {
        
    }
    
    public virtual void Pause () {
        
    }

    public virtual void Play (AnimationType animationType, bool inverted = false) {
        if (this.LockedAnimations) {
            return;
        }

        if (this.Animator != null) {
            this.Animator.Play (animationType.ToString ());
        } else if (this.MeshAnimator != null) {
            this.MeshAnimator.Play (animationType, inverted);
        } else {
            Debug.LogWarning (this + " has no animator, but is trying to play animation: " + animationType);
        }
    }

    protected void ResetLocalTransform () {
        this.Transform.localPosition = this.DefaultTransformData.LocalPosition;
        this.Transform.localEulerAngles = this.DefaultTransformData.LocalEulerAngles;
        this.Transform.localScale = this.DefaultTransformData.LocalScale;
    }

    public void ResetMesh () {
        this.Mesh.vertices = this.DefaultMeshData.Vertices;
        this.Mesh.uv = this.DefaultMeshData.TextureCoordinates;
    }

    public void ResetTransform () {
        this.Transform.position = this.DefaultTransformData.Position;
        this.Transform.eulerAngles = this.DefaultTransformData.EulerAngles;
        this.Transform.localScale = this.DefaultTransformData.LocalScale;
    }

    public virtual void Resume () {
        
    }

    public void SeparateMesh () {
        this.Mesh = GameObject.Instantiate (this.MeshFilter.sharedMesh);
        this.MeshFilter.mesh = this.Mesh;
    }

    public virtual void SetOpacity (float opacity) {
        Color32[] colors = this.Mesh.colors32;
        byte opacityByte = (byte) (Mathf.Clamp (opacity, 0.0f, 1.0f) * 255.0f);

        if (colors.Length == 0) {
            Debug.LogWarning ("Color array is empty, on : " + this);
            return;
        }
        
        for (int i = 0 ; i < this.DefaultMeshData.VertexCount; i ++) {
            colors[i].a = opacityByte;
        }
        
        this.Mesh.colors32 = colors;
    }

    public virtual void SetVisible (bool visible) {
        this.Renderer.enabled = visible;
    }

    protected virtual void Start () {
        this.InitializeExternals ();
    }

    public virtual IEnumerator YieldPlay (AnimationType animationType, bool inverted = false) {
        if (this.LockedAnimations) {
            yield break;
        }
        
        if (this.MeshAnimator != null) {
            yield return this.StartCoroutine (this.MeshAnimator.YieldPlay (animationType, inverted));
        } else {
            Debug.LogWarning (this + " has no animator, but is trying to play animation: " + animationType);
        }
    }
}
