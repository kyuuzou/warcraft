using System;
using UnityEditor;
using UnityEngine;

public class MeshViewer : EditorWindow {
    private const int MAX_DISPLAY = 100;
    private const int DEFAULT_MIN = 20;
    private Mesh mesh;
    private GameObject obj;
    private bool linkScroll, showVerts, showNorm, showTangents, showIndices, showUVs, showUV2s, showColors, showBoneWeights, showBindPoses, showBones;
    private Vector2 vertScroll, normScroll, tangentScroll, triScroll, UVScroll, UV2Scroll, colorScroll, boneWeightScroll, bindPoseScroll, boneScroll;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector4[] tangents;
    private Vector2[] uv, uv2;
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	Color[] colors;
#else
    private Color32[] colors;
#endif
    private BoneWeight[] boneWeights;
    private Matrix4x4[] bindPoses;
    private Transform[] bones;
    private int[] triangles;
    private int subMesh, oldSubMesh;
    private string[] subMeshStrings;
    private int[] subMeshIndices;
    private SkinnedMeshRenderer SMR;

    [MenuItem("Window/MeshViewer")]
    private static void Init() {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(MeshViewer));
    }

    private GameObject oldObject;

#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
    private MeshTopology topo;
#endif

    private int min, max;
    private float minSlider, maxSlider;
    private int idxMin, idxMax;
    private float idxMinSlider, idxMaxSlider;

    private void OnGUI() {
        GUILayout.BeginHorizontal();
        {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject));
#else
            this.obj = (GameObject)EditorGUILayout.ObjectField(this.obj, typeof(GameObject), true);
#endif
            if (this.obj == null) {
                this.mesh = null;
                this.SMR = null;
            } else if (this.obj.GetComponent<MeshFilter>() && (this.mesh == null || this.obj != this.oldObject)) {
                this.SMR = null;
                this.InitMesh(this.obj.GetComponent<MeshFilter>().sharedMesh);
            } else if (this.obj.GetComponent<SkinnedMeshRenderer>() && (this.mesh == null || this.obj != this.oldObject)) {
                this.SMR = this.obj.GetComponent<SkinnedMeshRenderer>();
                this.InitMesh(this.SMR.sharedMesh);
            } else if (this.obj.GetComponent<MeshCollider>() && (this.mesh == null || this.obj != this.oldObject)) {
                this.SMR = null;
                this.InitMesh(this.obj.GetComponent<MeshCollider>().sharedMesh);
            }
            GUI.enabled = this.mesh;
            this.oldObject = this.obj;
            this.linkScroll = GUILayout.Toggle(this.linkScroll, "Link scroll bars");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
            this.showVerts = GUILayout.Toggle(this.showVerts, "Show Vertices");
            this.showNorm = GUILayout.Toggle(this.showNorm, "Show Normals");
            this.showTangents = GUILayout.Toggle(this.showTangents, "Show Tangents");
            this.showIndices = GUILayout.Toggle(this.showIndices, "Show Indices");
            this.showUVs = GUILayout.Toggle(this.showUVs, "Show UVs");
            if (this.mesh && this.mesh.uv2.Length > 0) {
                this.showUV2s = GUILayout.Toggle(this.showUV2s, "Show UV2s");
            }
            if (this.mesh && this.mesh.colors.Length > 0) {
                this.showColors = GUILayout.Toggle(this.showColors, "Show Colors");
            }
            this.showBoneWeights = GUILayout.Toggle(this.showBoneWeights, "Show BoneWeights");
            this.showBindPoses = GUILayout.Toggle(this.showBindPoses, "Show BindPoses");

            if (this.SMR) {
                this.showBones = GUILayout.Toggle(this.showBones, "Show Bones");
            }
        }
        GUILayout.EndHorizontal();
        if (this.mesh) {
            //Vertex range slider
            GUILayout.Label("Vertex Range: " + this.min + " - " + this.max + " (" + (this.max - this.min) + ")");
            if (this.maxSlider >= this.mesh.vertexCount) {
                this.maxSlider = this.mesh.vertexCount;
            }

            if (this.minSlider >= this.maxSlider) {
                this.minSlider = 0;
            }

            EditorGUILayout.MinMaxSlider(ref this.minSlider, ref this.maxSlider, 0, this.mesh.vertexCount);
            this.minSlider = this.min = (int)this.minSlider;
            this.maxSlider = this.max = (int)this.maxSlider;
            //Index range slider
            GUILayout.Label("Face Range: " + this.idxMin + " - " + this.idxMax + " (" + (this.idxMax - this.idxMin) + ")");
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if(idxMaxSlider >= mesh.GetTriangles(subMesh).Length / 3)
				idxMaxSlider = mesh.GetTriangles(subMesh).Length / 3;
			if(idxMinSlider >= idxMaxSlider) {
				idxMinSlider = 0;
			}
			EditorGUILayout.MinMaxSlider(ref idxMinSlider, ref idxMaxSlider, 0, mesh.GetTriangles(subMesh).Length / 3);
#else
            if (this.idxMaxSlider >= this.mesh.GetIndices(this.subMesh).Length / TopologyToNum(this.topo)) {
                this.idxMaxSlider = this.mesh.GetIndices(this.subMesh).Length / TopologyToNum(this.topo);
            }

            if (this.idxMinSlider >= this.idxMaxSlider) {
                this.idxMinSlider = 0;
            }

            EditorGUILayout.MinMaxSlider(ref this.idxMinSlider, ref this.idxMaxSlider, 0, this.mesh.GetIndices(this.subMesh).Length / TopologyToNum(this.topo));
#endif
            this.idxMinSlider = this.idxMin = (int)this.idxMinSlider;
            this.idxMaxSlider = this.idxMax = (int)this.idxMaxSlider;
            //startOffset = (int)GUILayout.HorizontalSlider(startOffset, 0, mesh.vertexCount - MAX_DISPLAY > 0 ? mesh.vertexCount - MAX_DISPLAY : 0);
            GUILayout.BeginHorizontal();
            {
                if (this.showVerts) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Vertices");
                    this.BeginScroll(ref this.vertScroll);
                    this.vertices = this.mesh.vertices;
                    for (int i = this.min; i < this.max && i < this.min + MAX_DISPLAY; i++) {
                        this.vertices[i] = EditorGUILayout.Vector3Field(i + ":", this.vertices[i]);
                    }
                    this.mesh.vertices = this.vertices;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showNorm) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Normals");
                    this.BeginScroll(ref this.normScroll);
                    this.normals = this.mesh.normals;
                    for (int i = this.min; i < this.max && i < this.min + MAX_DISPLAY; i++) {
                        this.normals[i] = EditorGUILayout.Vector3Field(i + ":", this.normals[i]);
                    }
                    this.mesh.normals = this.normals;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showTangents) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Tangents");
                    this.BeginScroll(ref this.tangentScroll);
                    this.tangents = this.mesh.tangents;

                    for (int i = this.min; i < this.max && i < this.min + MAX_DISPLAY && i < this.tangents.Length - 1; i++) {
                        this.tangents[i] = EditorGUILayout.Vector3Field(i + ":", this.tangents[i]);
                    }

                    this.mesh.tangents = this.tangents;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showIndices) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Indices (Faces)");
                    GUILayout.BeginHorizontal();
                    this.subMesh = EditorGUILayout.IntPopup("SubMesh:", this.subMesh, this.subMeshStrings, this.subMeshIndices);
#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
                    if (this.subMesh != this.oldSubMesh) {
                        this.topo = this.mesh.GetTopology(this.subMesh);
                    }

                    this.oldSubMesh = this.subMesh;
                    GUILayout.Label("Mesh Topology: ", GUILayout.Width(92));
                    this.topo = (MeshTopology)EditorGUILayout.EnumPopup(this.topo);
#endif
                    GUILayout.EndHorizontal();
                    this.triScroll = GUILayout.BeginScrollView(this.triScroll);
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					triangles = mesh.GetTriangles(subMesh);
					for(int i = idxMin; i < idxMax; i++) {
						GUILayout.BeginHorizontal();
						{
							int start = i * 3;
							triangles[start] = EditorGUILayout.IntField(start + ":", triangles[start]);
							triangles[start + 1] = EditorGUILayout.IntField(start + 1 + ":", triangles[start + 1]);
							triangles[start + 2] = EditorGUILayout.IntField(start + 2 + ":", triangles[start + 2]);
						} GUILayout.EndHorizontal();

					}
					mesh.SetTriangles(triangles, subMesh);
#else
                    this.triangles = this.mesh.GetIndices(this.subMesh);
                    for (int i = this.idxMin; i < this.idxMax; i++) {
                        GUILayout.BeginHorizontal();
                        {
                            int start = i * TopologyToNum(this.topo);
                            this.triangles[start] = EditorGUILayout.IntField(start + ":", this.triangles[start]);

                            if (this.topo == MeshTopology.LineStrip || this.topo == MeshTopology.Lines || this.topo == MeshTopology.Triangles || this.topo == MeshTopology.Quads) {
                                this.triangles[start + 1] = EditorGUILayout.IntField(start + 1 + ":", this.triangles[start + 1]);
                            }

                            if (this.topo == MeshTopology.Triangles || this.topo == MeshTopology.Quads) {
                                this.triangles[start + 2] = EditorGUILayout.IntField(start + 2 + ":", this.triangles[start + 2]);
                            }

                            if (this.topo == MeshTopology.Quads) {
                                this.triangles[start + 3] = EditorGUILayout.IntField(start + 3 + ":", this.triangles[start + 3]);
                            }
                        }
                        GUILayout.EndHorizontal();

                    }
                    this.mesh.SetIndices(this.triangles, this.topo, this.subMesh);
#endif
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showUVs) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("UVs");
                    this.BeginScroll(ref this.UVScroll);
                    this.uv = this.mesh.uv;
                    for (int start = this.min; start < this.max && start < this.min + MAX_DISPLAY; start++) {
                        this.uv[start] = EditorGUILayout.Vector2Field(start + ":", this.uv[start]);
                    }
                    this.mesh.uv = this.uv;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showUV2s && this.mesh.uv2.Length > 0) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("UV2s");
                    this.BeginScroll(ref this.UV2Scroll);
                    this.uv2 = this.mesh.uv2;
                    for (int start = this.min; start < this.max && start < this.min + MAX_DISPLAY; start++) {
                        this.uv2[start] = EditorGUILayout.Vector2Field(start + ":", this.uv2[start]);
                    }
                    this.mesh.uv2 = this.uv2;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showColors && this.mesh.colors.Length > 0) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Vertex Colors");
                    this.BeginScroll(ref this.colorScroll);
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					colors = mesh.colors;
#else
                    this.colors = this.mesh.colors32;
#endif
                    for (int start = this.min; start < this.max && start < this.min + MAX_DISPLAY; start++) {
                        this.colors[start] = EditorGUILayout.ColorField(start + ":", this.colors[start]);
                    }
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					mesh.colors = colors;
#else
                    this.mesh.colors32 = this.colors;
#endif
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showBoneWeights) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bone Weights");
                    this.boneWeightScroll = GUILayout.BeginScrollView(this.boneWeightScroll);
                    this.boneWeights = this.mesh.boneWeights;
                    for (int start = this.min; start < this.boneWeights.Length; start++) {
                        GUILayout.Label(start + ":");
                        this.boneWeights[start].boneIndex0 = EditorGUILayout.IntField("Bone 0 idx: ", this.boneWeights[start].boneIndex0);
                        this.boneWeights[start].boneIndex1 = EditorGUILayout.IntField("Bone 1 idx: ", this.boneWeights[start].boneIndex1);
                        this.boneWeights[start].boneIndex2 = EditorGUILayout.IntField("Bone 2 idx: ", this.boneWeights[start].boneIndex2);
                        this.boneWeights[start].boneIndex3 = EditorGUILayout.IntField("Bone 3 idx: ", this.boneWeights[start].boneIndex3);
                        this.boneWeights[start].weight0 = EditorGUILayout.FloatField("Bone 0 Weight: ", this.boneWeights[start].weight0);
                        this.boneWeights[start].weight1 = EditorGUILayout.FloatField("Bone 1 Weight: ", this.boneWeights[start].weight1);
                        this.boneWeights[start].weight2 = EditorGUILayout.FloatField("Bone 2 Weight: ", this.boneWeights[start].weight2);
                        this.boneWeights[start].weight3 = EditorGUILayout.FloatField("Bone 3 Weight: ", this.boneWeights[start].weight3);
                    }
                    this.mesh.boneWeights = this.boneWeights;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.showBindPoses) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bind Poses");
                    this.bindPoseScroll = GUILayout.BeginScrollView(this.bindPoseScroll);
                    this.bindPoses = this.mesh.bindposes;
                    for (int start = 0; start < this.bindPoses.Length; start++) {
                        Matrix4x4Field(start + ":", ref this.bindPoses[start]);
                    }
                    this.mesh.bindposes = this.bindPoses;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (this.SMR && this.showBones) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bones");
                    this.boneScroll = GUILayout.BeginScrollView(this.boneScroll);
                    this.bones = this.SMR.bones;
                    for (int start = 0; start < this.bones.Length; start++) {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
						bones[start] = EditorGUILayout.ObjectField(bones[start], typeof(Transform)) as Transform;
#else
                        this.bones[start] = EditorGUILayout.ObjectField(this.bones[start], typeof(Transform), true) as Transform;
#endif
                    }
                    this.SMR.bones = this.bones;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void InitMesh(Mesh mesh) {
        this.mesh = mesh;
        if (mesh) {
            this.subMesh = 0;
#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
			
			idxMaxSlider = Mathf.Min((DEFAULT_MIN - 2) * 2, mesh.GetTriangles(subMesh).Length / 3);
#else
            this.topo = mesh.GetTopology(this.subMesh);
            this.idxMaxSlider = Mathf.Min(DEFAULT_MIN * 2 - 1, mesh.GetIndices(this.subMesh).Length / TopologyToNum(this.topo));
#endif
            this.maxSlider = Mathf.Min(DEFAULT_MIN, mesh.vertexCount);
            this.subMeshIndices = new int[mesh.subMeshCount];
            this.subMeshStrings = new string[mesh.subMeshCount];
            for (int start = 0; start < mesh.subMeshCount; start++) {
                this.subMeshIndices[start] = start;
                this.subMeshStrings[start] = start + "";
            }

            this.showVerts = true;
            this.showIndices = true;
        }
    }

    private void BeginScroll(ref Vector2 input) {
        if (this.linkScroll) {
            this.vertScroll = GUILayout.BeginScrollView(this.vertScroll);
        } else {
            input = GUILayout.BeginScrollView(input);
        }
    }
#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
    private static int TopologyToNum(MeshTopology topo) {
        switch (topo) {
            case MeshTopology.Lines:
                return 2;
            case MeshTopology.LineStrip:
                return 2;
            case MeshTopology.Points:
                return 1;
            case MeshTopology.Quads:
                return 4;
            case MeshTopology.Triangles:
                return 3;
            default:
                // do nothing
                break;
        }
        
        return 1;
    }
#endif
    private static void Matrix4x4Field(string label, ref Matrix4x4 mat) {
        GUILayout.Label(label);
        mat.SetRow(0, EditorGUILayout.Vector4Field("0", mat.GetRow(0)));
        mat.SetRow(1, EditorGUILayout.Vector4Field("1", mat.GetRow(1)));
        mat.SetRow(2, EditorGUILayout.Vector4Field("2", mat.GetRow(2)));
        mat.SetRow(3, EditorGUILayout.Vector4Field("3", mat.GetRow(3)));
    }
}