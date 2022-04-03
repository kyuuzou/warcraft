using UnityEditor;
using UnityEngine;

public class MeshViewer : EditorWindow {
    const int MAX_DISPLAY = 100;
    const int DEFAULT_MIN = 20;
    Mesh mesh;
    GameObject obj;
    bool linkScroll, showVerts, showNorm, showTangents, showIndices, showUVs, showUV2s, showColors, showBoneWeights, showBindPoses, showBones;
    Vector2 vertScroll, normScroll, tangentScroll, triScroll, UVScroll, UV2Scroll, colorScroll, boneWeightScroll, bindPoseScroll, boneScroll;

    Vector3[] vertices;
    Vector3[] normals;
    Vector4[] tangents;
    Vector2[] uv, uv2;
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	Color[] colors;
#else
    Color32[] colors;
#endif
    BoneWeight[] boneWeights;
    Matrix4x4[] bindPoses;
    Transform[] bones;
    int[] triangles;
    int subMesh, oldSubMesh;
    string[] subMeshStrings;
    int[] subMeshIndices;

    SkinnedMeshRenderer SMR;

    [MenuItem("Window/MeshViewer")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(MeshViewer));
    }
    GameObject oldObject;

#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
    MeshTopology topo;
#endif

    int min, max;
    float minSlider, maxSlider;
    int idxMin, idxMax;
    float idxMinSlider, idxMaxSlider;

    void OnGUI() {
        GUILayout.BeginHorizontal();
        {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject));
#else
            obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
#endif
            if (obj == null) {
                mesh = null;
                SMR = null;
            } else if (obj.GetComponent<MeshFilter>() && (mesh == null || obj != oldObject)) {
                SMR = null;
                InitMesh(obj.GetComponent<MeshFilter>().sharedMesh);
            } else if (obj.GetComponent<SkinnedMeshRenderer>() && (mesh == null || obj != oldObject)) {
                SMR = obj.GetComponent<SkinnedMeshRenderer>();
                InitMesh(SMR.sharedMesh);
            } else if (obj.GetComponent<MeshCollider>() && (mesh == null || obj != oldObject)) {
                SMR = null;
                InitMesh(obj.GetComponent<MeshCollider>().sharedMesh);
            }
            GUI.enabled = mesh;
            oldObject = obj;
            linkScroll = GUILayout.Toggle(linkScroll, "Link scroll bars");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
            showVerts = GUILayout.Toggle(showVerts, "Show Vertices");
            showNorm = GUILayout.Toggle(showNorm, "Show Normals");
            showTangents = GUILayout.Toggle(showTangents, "Show Tangents");
            showIndices = GUILayout.Toggle(showIndices, "Show Indices");
            showUVs = GUILayout.Toggle(showUVs, "Show UVs");
            if (mesh && mesh.uv2.Length > 0) {
                showUV2s = GUILayout.Toggle(showUV2s, "Show UV2s");
            }
            if (mesh && mesh.colors.Length > 0) {
                showColors = GUILayout.Toggle(showColors, "Show Colors");
            }
            showBoneWeights = GUILayout.Toggle(showBoneWeights, "Show BoneWeights");
            showBindPoses = GUILayout.Toggle(showBindPoses, "Show BindPoses");

            if (SMR) {
                showBones = GUILayout.Toggle(showBones, "Show Bones");
            }
        }
        GUILayout.EndHorizontal();
        if (mesh) {
            //Vertex range slider
            GUILayout.Label("Vertex Range: " + min + " - " + max + " (" + (max - min) + ")");
            if (maxSlider >= mesh.vertexCount) {
                maxSlider = mesh.vertexCount;
            }

            if (minSlider >= maxSlider) {
                minSlider = 0;
            }

            EditorGUILayout.MinMaxSlider(ref minSlider, ref maxSlider, 0, mesh.vertexCount);
            minSlider = min = (int)minSlider;
            maxSlider = max = (int)maxSlider;
            //Index range slider
            GUILayout.Label("Face Range: " + idxMin + " - " + idxMax + " (" + (idxMax - idxMin) + ")");
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if(idxMaxSlider >= mesh.GetTriangles(subMesh).Length / 3)
				idxMaxSlider = mesh.GetTriangles(subMesh).Length / 3;
			if(idxMinSlider >= idxMaxSlider) {
				idxMinSlider = 0;
			}
			EditorGUILayout.MinMaxSlider(ref idxMinSlider, ref idxMaxSlider, 0, mesh.GetTriangles(subMesh).Length / 3);
#else
            if (idxMaxSlider >= mesh.GetIndices(subMesh).Length / TopologyToNum(topo)) {
                idxMaxSlider = mesh.GetIndices(subMesh).Length / TopologyToNum(topo);
            }

            if (idxMinSlider >= idxMaxSlider) {
                idxMinSlider = 0;
            }

            EditorGUILayout.MinMaxSlider(ref idxMinSlider, ref idxMaxSlider, 0, mesh.GetIndices(subMesh).Length / TopologyToNum(topo));
#endif
            idxMinSlider = idxMin = (int)idxMinSlider;
            idxMaxSlider = idxMax = (int)idxMaxSlider;
            //startOffset = (int)GUILayout.HorizontalSlider(startOffset, 0, mesh.vertexCount - MAX_DISPLAY > 0 ? mesh.vertexCount - MAX_DISPLAY : 0);
            GUILayout.BeginHorizontal();
            {
                if (showVerts) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Vertices");
                    BeginScroll(ref vertScroll);
                    vertices = mesh.vertices;
                    for (int i = min; i < max && i < min + MAX_DISPLAY; i++) {
                        vertices[i] = EditorGUILayout.Vector3Field(i + ":", vertices[i]);
                    }
                    mesh.vertices = vertices;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showNorm) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Normals");
                    BeginScroll(ref normScroll);
                    normals = mesh.normals;
                    for (int i = min; i < max && i < min + MAX_DISPLAY; i++) {
                        normals[i] = EditorGUILayout.Vector3Field(i + ":", normals[i]);
                    }
                    mesh.normals = normals;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showTangents) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Tangents");
                    BeginScroll(ref tangentScroll);
                    tangents = mesh.tangents;

                    for (int i = min; i < max && i < min + MAX_DISPLAY && i < tangents.Length - 1; i++) {
                        tangents[i] = EditorGUILayout.Vector3Field(i + ":", tangents[i]);
                    }

                    mesh.tangents = tangents;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showIndices) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Indices (Faces)");
                    GUILayout.BeginHorizontal();
                    subMesh = EditorGUILayout.IntPopup("SubMesh:", subMesh, subMeshStrings, subMeshIndices);
#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
                    if (subMesh != oldSubMesh) {
                        topo = mesh.GetTopology(subMesh);
                    }

                    oldSubMesh = subMesh;
                    GUILayout.Label("Mesh Topology: ", GUILayout.Width(92));
                    topo = (MeshTopology)EditorGUILayout.EnumPopup(topo);
#endif
                    GUILayout.EndHorizontal();
                    triScroll = GUILayout.BeginScrollView(triScroll);
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
                    triangles = mesh.GetIndices(subMesh);
                    for (int i = idxMin; i < idxMax; i++) {
                        GUILayout.BeginHorizontal();
                        {
                            int start = i * TopologyToNum(topo);
                            triangles[start] = EditorGUILayout.IntField(start + ":", triangles[start]);

                            if (topo == MeshTopology.LineStrip || topo == MeshTopology.Lines || topo == MeshTopology.Triangles || topo == MeshTopology.Quads) {
                                triangles[start + 1] = EditorGUILayout.IntField(start + 1 + ":", triangles[start + 1]);
                            }

                            if (topo == MeshTopology.Triangles || topo == MeshTopology.Quads) {
                                triangles[start + 2] = EditorGUILayout.IntField(start + 2 + ":", triangles[start + 2]);
                            }

                            if (topo == MeshTopology.Quads) {
                                triangles[start + 3] = EditorGUILayout.IntField(start + 3 + ":", triangles[start + 3]);
                            }
                        }
                        GUILayout.EndHorizontal();

                    }
                    mesh.SetIndices(triangles, topo, subMesh);
#endif
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showUVs) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("UVs");
                    BeginScroll(ref UVScroll);
                    uv = mesh.uv;
                    for (int start = min; start < max && start < min + MAX_DISPLAY; start++) {
                        uv[start] = EditorGUILayout.Vector2Field(start + ":", uv[start]);
                    }
                    mesh.uv = uv;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showUV2s && mesh.uv2.Length > 0) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("UV2s");
                    BeginScroll(ref UV2Scroll);
                    uv2 = mesh.uv2;
                    for (int start = min; start < max && start < min + MAX_DISPLAY; start++) {
                        uv2[start] = EditorGUILayout.Vector2Field(start + ":", uv2[start]);
                    }
                    mesh.uv2 = uv2;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showColors && mesh.colors.Length > 0) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Vertex Colors");
                    BeginScroll(ref colorScroll);
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					colors = mesh.colors;
#else
                    colors = mesh.colors32;
#endif
                    for (int start = min; start < max && start < min + MAX_DISPLAY; start++) {
                        colors[start] = EditorGUILayout.ColorField(start + ":", colors[start]);
                    }
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					mesh.colors = colors;
#else
                    mesh.colors32 = colors;
#endif
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showBoneWeights) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bone Weights");
                    boneWeightScroll = GUILayout.BeginScrollView(boneWeightScroll);
                    boneWeights = mesh.boneWeights;
                    for (int start = min; start < boneWeights.Length; start++) {
                        GUILayout.Label(start + ":");
                        boneWeights[start].boneIndex0 = EditorGUILayout.IntField("Bone 0 idx: ", boneWeights[start].boneIndex0);
                        boneWeights[start].boneIndex1 = EditorGUILayout.IntField("Bone 1 idx: ", boneWeights[start].boneIndex1);
                        boneWeights[start].boneIndex2 = EditorGUILayout.IntField("Bone 2 idx: ", boneWeights[start].boneIndex2);
                        boneWeights[start].boneIndex3 = EditorGUILayout.IntField("Bone 3 idx: ", boneWeights[start].boneIndex3);
                        boneWeights[start].weight0 = EditorGUILayout.FloatField("Bone 0 Weight: ", boneWeights[start].weight0);
                        boneWeights[start].weight1 = EditorGUILayout.FloatField("Bone 1 Weight: ", boneWeights[start].weight1);
                        boneWeights[start].weight2 = EditorGUILayout.FloatField("Bone 2 Weight: ", boneWeights[start].weight2);
                        boneWeights[start].weight3 = EditorGUILayout.FloatField("Bone 3 Weight: ", boneWeights[start].weight3);
                    }
                    mesh.boneWeights = boneWeights;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (showBindPoses) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bind Poses");
                    bindPoseScroll = GUILayout.BeginScrollView(bindPoseScroll);
                    bindPoses = mesh.bindposes;
                    for (int start = 0; start < bindPoses.Length; start++) {
                        Matrix4x4Field(start + ":", ref bindPoses[start]);
                    }
                    mesh.bindposes = bindPoses;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                if (SMR && showBones) {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Bones");
                    boneScroll = GUILayout.BeginScrollView(boneScroll);
                    bones = SMR.bones;
                    for (int start = 0; start < bones.Length; start++) {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
						bones[start] = EditorGUILayout.ObjectField(bones[start], typeof(Transform)) as Transform;
#else
                        bones[start] = EditorGUILayout.ObjectField(bones[start], typeof(Transform), true) as Transform;
#endif
                    }
                    SMR.bones = bones;
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
    void InitMesh(Mesh mesh) {
        this.mesh = mesh;
        if (mesh) {
            subMesh = 0;
#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
			
			idxMaxSlider = Mathf.Min((DEFAULT_MIN - 2) * 2, mesh.GetTriangles(subMesh).Length / 3);
#else
            topo = mesh.GetTopology(subMesh);
            idxMaxSlider = Mathf.Min(DEFAULT_MIN * 2 - 1, mesh.GetIndices(subMesh).Length / TopologyToNum(topo));
#endif
            maxSlider = Mathf.Min(DEFAULT_MIN, mesh.vertexCount);
            subMeshIndices = new int[mesh.subMeshCount];
            subMeshStrings = new string[mesh.subMeshCount];
            for (int start = 0; start < mesh.subMeshCount; start++) {
                subMeshIndices[start] = start;
                subMeshStrings[start] = start + "";
            }

            showVerts = true;
            showIndices = true;
        }
    }
    void BeginScroll(ref Vector2 input) {
        if (linkScroll) {
            vertScroll = GUILayout.BeginScrollView(vertScroll);
        } else { 
            input = GUILayout.BeginScrollView(input);
        }
    }
#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
    static int TopologyToNum(MeshTopology topo) {
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
        }
        return 1;
    }
#endif
    static void Matrix4x4Field(string label, ref Matrix4x4 mat) {
        GUILayout.Label(label);
        mat.SetRow(0, EditorGUILayout.Vector4Field("0", mat.GetRow(0)));
        mat.SetRow(1, EditorGUILayout.Vector4Field("1", mat.GetRow(1)));
        mat.SetRow(2, EditorGUILayout.Vector4Field("2", mat.GetRow(2)));
        mat.SetRow(3, EditorGUILayout.Vector4Field("3", mat.GetRow(3)));
    }
}