using UnityEngine;

public class MeshAnimation : CustomScriptableObject {

    [SerializeField]
    private AnimationType type;
    public AnimationType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Material material;

    private Material sharedMaterial;

    [SerializeField]
    private Texture overrideTexture;
    public Texture OverrideTexture {
        private get {
            return this.overrideTexture;
        }
        set {
            this.texture = this.overrideTexture = value == null ? this.material.mainTexture : value;
            this.RefreshTexture();
        }
    }

    private Texture texture;

    [SerializeField]
    private Mesh mesh;
    public Mesh Mesh {
        get { return this.mesh; }
    }

    [SerializeField]
    private Vector2 atlasSize;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private Vector2 margin;

    [SerializeField]
    private int[] frames;

    [SerializeField]
    private AnimationType nextAnimation;

    private Vector2 relativeTileSize;
    private Vector2 relativeOffset;
    private Vector2 relativeMargin;
    private Vector2 tileSize;

    private Vector2[] originalUV;

    private int lastTriggeredFrameIndex;

    private int currentFrame;
    public int CurrentFrame {
        get { return this.currentFrame; }
        set {
            if (this.currentFrame == value) {
                return;
            }

            this.currentFrame = value;

            this.OnFrameChanged();

            if (!this.Inverted) {
                foreach (MeshAnimationTrigger trigger in this.triggers) {
                    if (trigger.FrameIndex == value) {
                        this.owner.MeshAnimator.OnAnimationTrigger(this.Type, trigger.Type);
                    }
                }
            }
        }
    }

    [SerializeField]
    private float delay = 0.05f;
    public float Delay {
        get { return this.delay; }
    }

    [SerializeField]
    private bool pingPong = false;

    [SerializeField]
    private bool loop = true;

    [SerializeField]
    private MeshAnimationTrigger[] triggers;

    private Direction direction;
    public Direction Direction {
        private get { return this.direction; }
        set {
            bool update = (value != this.direction) && (this.direction != Direction.None);

            this.direction = value;

            if (update) {
                this.Iterate();
            }
        }
    }

    public bool Inverted { get; set; }

    private SceneObject owner;
    private Vector2 paddedRelativeSize;

    public delegate void FrameChangedHandler(object sender, FrameChangedArgs args);
    private event FrameChangedHandler FrameChanged;

    public override void Activate() {
        if (!this.Initialized) {
            Debug.LogWarning("Tried to play " + this + " on " + this.owner + " before initialization.");
            return;
        }

        base.Activate();

        this.CurrentFrame = this.Inverted ? this.frames.Length - 1 : 0;

        this.sharedMaterial = this.owner.Renderer.sharedMaterial = this.material;
        this.sharedMaterial.mainTextureScale = this.relativeTileSize;
        //        this.sharedMaterial.mainTextureScale = this.paddedRelativeSize;
        this.sharedMaterial.mainTexture = this.texture;

        this.owner.MeshFilter.mesh = this.mesh;
    }

    public void Initialize(SceneObject owner) {
        this.Initialize();

        this.owner = owner;

        this.RefreshTexture();
        this.InitializeMesh();
    }

    private void InitializeMesh() {
        this.mesh = Mesh.Instantiate(this.mesh);
        this.originalUV = this.mesh.uv;

        Color32[] colors = new Color32[this.mesh.vertexCount];

        for (int i = 0; i < colors.Length; i++) {
            colors[i].r = colors[i].g = colors[i].b = colors[i].a = 255;
        }

        this.mesh.colors32 = colors;
    }

    public bool Iterate() {
        if (this.frames.Length == 0) {
            return false;
        }

        bool ended = false;

        if (this.loop) {
            this.CurrentFrame = this.CurrentFrame.RoundClamp(0, this.frames.Length - 1);
        } else if (!this.Inverted && this.CurrentFrame >= this.frames.Length) {
            if (this.pingPong) {
                this.Inverted = true;
                this.CurrentFrame--;
            } else {
                ended = true;
            }
        } else if (this.Inverted && this.CurrentFrame < 0) {
            ended = true;
        }

        if (ended) {
            this.owner.MeshAnimator.OnAnimationTrigger(this.Type, AnimationTriggerType.OnFinished);

            if (this.nextAnimation != AnimationType.None) {
                this.owner.Play(this.nextAnimation);
            }

            return false;
        }

        this.UpdateUV();

        this.CurrentFrame += this.Inverted ? -1 : 1;

        return true;
    }

    private void OnFrameChanged() {
        if (this.FrameChanged != null) {
            this.FrameChanged(this, new FrameChangedArgs(this.CurrentFrame));
        }
    }

    private void RefreshTexture() {
        this.texture = this.overrideTexture == null ? this.material.mainTexture : this.overrideTexture;

        if (this.texture == null) {
            Debug.LogError(string.Format("No texture found for {0} on {1}", this.owner.name, this.Type));
        }

        this.relativeTileSize = new Vector2(
            (this.texture.width / this.atlasSize.x) / this.texture.width,
            (this.texture.height / this.atlasSize.y) / this.texture.height
        );

        this.relativeOffset = new Vector2(
            (this.offset.x / this.texture.width) / this.relativeTileSize.x,
            (this.offset.y / this.texture.height) / this.relativeTileSize.y
        );

        this.relativeMargin = new Vector2(
            (this.margin.x / this.texture.width) / this.relativeTileSize.x,
            (this.margin.y / this.texture.height) / this.relativeTileSize.y
        );
    }

    public void RegisterFrameChangedListener(FrameChangedHandler handler) {
        this.FrameChanged += handler;
    }

    public void SetFrameValue(int frameIndex, int atlasIndex) {
        this.frames[frameIndex] = atlasIndex;
    }

    private void UpdateUV() {
        if (this.atlasSize == Vector2.zero) {
            Debug.LogError($"Atlas size not set at {this.owner?.transform.parent?.name}:{this}");
            return;
        }

        Vector2[] uvs = new Vector2[this.originalUV.Length];
        int i = 0;

        DirectionData data = this.Direction.GetData();
        int currentFrame = this.frames[this.CurrentFrame] + data.SpriteOffset;

        this.owner.Transform.SetLocalScaleX(this.owner.DefaultTransformData.LocalScale.x * data.SpriteMultiplier);

        int x = currentFrame % (int)this.atlasSize.x;
        int y = currentFrame / (int)this.atlasSize.x;

        while (i < uvs.Length) {
            uvs[i] = new Vector2(
                this.originalUV[i].x + x + x * this.relativeMargin.x + this.relativeOffset.x,
                this.originalUV[i].y + (this.atlasSize.y - y - 1) - y * this.relativeMargin.y - this.relativeOffset.y
            );

            i++;
        }

        this.mesh.uv = uvs;
    }
}
