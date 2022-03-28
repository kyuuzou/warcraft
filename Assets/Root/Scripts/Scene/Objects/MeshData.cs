using UnityEngine;
using System.Collections;

public class MeshData {

    public int VertexCount { get; private set; }
    public Vector3[] Vertices { get; private set; }
    public Vector2[] TextureCoordinates { get; private set; }

    public MeshData (Mesh mesh) {
        this.VertexCount = mesh.vertexCount;
        this.Vertices = mesh.vertices;
        this.TextureCoordinates = mesh.uv;

        if (mesh.colors32.Length == 0) {
            this.InitializeColors (mesh);
        }
    }

    private void InitializeColors (Mesh mesh) {
        Color32[] colors = new Color32[mesh.vertexCount];
        
        for (int i = 0; i < colors.Length; i ++) {
            colors[i].r = colors[i].g = colors[i].b = colors[i].a = 255;
        }
        
        mesh.colors32 = colors;
    }
}
