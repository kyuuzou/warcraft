using System.Collections.Generic;
using UnityEngine;

public class CustomLineVertex {
    public Color color = Color.white;
    public float width = 1.0f;
    public Vector3 Position { get; set; } = Vector3.zero;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomLineRenderer : MonoBehaviour {
    private Mesh m_Mesh;
    private int[] m_Indices;
    private Vector2[] m_UVs;
    private List<CustomLineVertex> m_Points;

    private void Awake() {
        this.m_Mesh = new Mesh();
        this.m_Points = new List<CustomLineVertex>();
        this.GetComponent<MeshFilter>().sharedMesh = this.m_Mesh;
    }

    private void Start() {
        // To calculate the initial bounds
        this.UpdateMesh(Camera.main);
    }

    private void OnWillRenderObject() {
        this.UpdateMesh(Camera.current);
    }

    private void UpdateMesh(Camera aCamera) {
        Vector3 localViewPos = this.transform.InverseTransformPoint(aCamera.transform.position);
        Vector3[] vertices = this.m_Mesh.vertices;
        Vector3[] normals = this.m_Mesh.normals;
        Color[] colors = this.m_Mesh.colors;
        Vector3 oldTangent = Vector3.zero;
        for (int i = 0; i < this.m_Points.Count - 1; i++) {
            Vector3 faceNormal = (localViewPos - this.m_Points[i].position).normalized;
            Vector3 dir = (this.m_Points[i + 1].position - this.m_Points[i].position);
            Vector3 tangent = Vector3.Cross(dir, faceNormal).normalized;
            Vector3 offset = (oldTangent + tangent).normalized * this.m_Points[i].width / 2.0f;

            vertices[i * 2] = this.m_Points[i].position - offset;
            vertices[i * 2 + 1] = this.m_Points[i].position + offset;
            normals[i * 2] = normals[i * 2 + 1] = faceNormal;
            colors[i * 2] = colors[i * 2 + 1] = this.m_Points[i].color;
            if (i == this.m_Points.Count - 2) {
                // last two points
                vertices[i * 2 + 2] = this.m_Points[i + 1].position - tangent * this.m_Points[i + 1].width / 2.0f;
                vertices[i * 2 + 3] = this.m_Points[i + 1].position + tangent * this.m_Points[i + 1].width / 2.0f;
                normals[i * 2 + 2] = normals[i * 2 + 3] = faceNormal;
                colors[i * 2 + 2] = colors[i * 2 + 3] = this.m_Points[i + 1].color;
            }
            oldTangent = tangent;
        }
        this.m_Mesh.vertices = vertices;
        this.m_Mesh.normals = normals;
        this.m_Mesh.colors = colors;
        this.m_Mesh.uv = this.m_UVs;
        this.m_Mesh.SetTriangles(this.m_Indices, 0);
        this.m_Mesh.RecalculateBounds();
    }

    public void SetVertexCount(int aCount) {
        aCount = Mathf.Clamp(aCount, 0, 0xFFFF / 2);
        if (this.m_Points.Count > aCount) {
            this.m_Points.RemoveRange(aCount, this.m_Points.Count - aCount);
        }

        while (this.m_Points.Count < aCount) {
            this.m_Points.Add(new CustomLineVertex());
        }

        this.m_Mesh.vertices = new Vector3[this.m_Points.Count * 2];
        this.m_Mesh.normals = new Vector3[this.m_Points.Count * 2];
        this.m_Mesh.colors = new Color[this.m_Points.Count * 2];
        this.m_Indices = new int[this.m_Points.Count * 2];
        this.m_UVs = new Vector2[this.m_Points.Count * 2];
        for (int i = 0; i < this.m_Points.Count; i++) {
            this.m_Indices[i * 2] = i * 2;
            this.m_Indices[i * 2 + 1] = i * 2 + 1;
            this.m_UVs[i * 2] = this.m_UVs[i * 2 + 1] = new Vector2((float)i / (this.m_Points.Count - 1), 0);
            this.m_UVs[i * 2 + 1].y = 1.0f;
        }
    }

    public void SetPosition(int aIndex, Vector3 aPosition) {
        if (aIndex < 0 || aIndex >= this.m_Points.Count) {
            return;
        }

        this.m_Points[aIndex].position = aPosition;
    }

    public void SetWidth(int aIndex, float aWidth) {
        if (aIndex < 0 || aIndex >= this.m_Points.Count) {
            return;
        }

        this.m_Points[aIndex].width = aWidth;
    }

    public void SetColor(int aIndex, Color aColor) {
        if (aIndex < 0 || aIndex >= this.m_Points.Count) {
            return;
        }

        this.m_Points[aIndex].color = aColor;
    }

    public void SetWidth(float aStartWidth, float aEndWidth) {
        for (int i = 0; i < this.m_Points.Count; i++) {
            this.m_Points[i].width = Mathf.Lerp(aStartWidth, aEndWidth, (float)i / (this.m_Points.Count - 1));
        }
    }

    public void SetColor(Color aStart, Color aEnd) {
        for (int i = 0; i < this.m_Points.Count; i++) {
            this.m_Points[i].color = Color.Lerp(aStart, aEnd, (float)i / (this.m_Points.Count - 1));
        }
    }

}