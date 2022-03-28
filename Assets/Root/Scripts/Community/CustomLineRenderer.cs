using UnityEngine;
using System.Collections;
using System.Collections.Generic;
     
public class CustomLineVertex
{
    public Vector3 position = Vector3.zero;
    public Color color = Color.white;
    public float width = 1.0f;
}
     
[RequireComponent (typeof (MeshFilter),typeof (MeshRenderer))]
public class CustomLineRenderer : MonoBehaviour
{
    private Mesh m_Mesh;
    private int[] m_Indices;
    private Vector2[] m_UVs;
    private List<CustomLineVertex> m_Points;
     
    void Awake ()
    {
        m_Mesh = new Mesh ();
        m_Points = new List<CustomLineVertex> ();
        GetComponent<MeshFilter> ().sharedMesh = m_Mesh;
    }
     
    void Start ()
    {
        // To calculate the initial bounds
        UpdateMesh (Camera.main);
    }
     
    void OnWillRenderObject ()
    {
        UpdateMesh (Camera.current);
    }
     
    void UpdateMesh (Camera aCamera)
    {
        Vector3 localViewPos = transform.InverseTransformPoint (aCamera.transform.position);
        Vector3[] vertices = m_Mesh.vertices;
        Vector3[] normals = m_Mesh.normals;
        Color[] colors = m_Mesh.colors;
        Vector3 oldTangent = Vector3.zero;
        for (int i = 0; i < m_Points.Count-1; i++) {
            Vector3 faceNormal = (localViewPos - m_Points [i].position).normalized;
            Vector3 dir = (m_Points [i + 1].position - m_Points [i].position);
            Vector3 tangent = Vector3.Cross (dir, faceNormal).normalized;
            Vector3 offset = (oldTangent + tangent).normalized * m_Points [i].width / 2.0f;
     
            vertices [i * 2] = m_Points [i].position - offset;
            vertices [i * 2 + 1] = m_Points [i].position + offset;
            normals [i * 2] = normals [i * 2 + 1] = faceNormal;
            colors [i * 2] = colors [i * 2 + 1] = m_Points [i].color;
            if (i == m_Points.Count - 2) {
                // last two points
                vertices [i * 2 + 2] = m_Points [i + 1].position - tangent * m_Points [i + 1].width / 2.0f;
                vertices [i * 2 + 3] = m_Points [i + 1].position + tangent * m_Points [i + 1].width / 2.0f;
                normals [i * 2 + 2] = normals [i * 2 + 3] = faceNormal;
                colors [i * 2 + 2] = colors [i * 2 + 3] = m_Points [i + 1].color;
            }
            oldTangent = tangent;
        }
        m_Mesh.vertices = vertices;
        m_Mesh.normals = normals;
        m_Mesh.colors = colors;
        m_Mesh.uv = m_UVs;
        m_Mesh.SetTriangles (m_Indices, 0);
        m_Mesh.RecalculateBounds ();
    }
     
    public void SetVertexCount (int aCount)
    {
        aCount = Mathf.Clamp (aCount, 0, 0xFFFF / 2);
        if (m_Points.Count > aCount)
            m_Points.RemoveRange (aCount, m_Points.Count - aCount);
        while (m_Points.Count < aCount)
            m_Points.Add (new CustomLineVertex ());
     
        m_Mesh.vertices = new Vector3[m_Points.Count * 2];
        m_Mesh.normals = new Vector3[m_Points.Count * 2];
        m_Mesh.colors = new Color[m_Points.Count * 2];
        m_Indices = new int[m_Points.Count * 2];
        m_UVs = new Vector2[m_Points.Count * 2];
        for (int i = 0; i < m_Points.Count; i++) {
            m_Indices [i * 2] = i * 2;
            m_Indices [i * 2 + 1] = i * 2 + 1;
            m_UVs [i * 2] = m_UVs [i * 2 + 1] = new Vector2 ((float)i / (m_Points.Count - 1), 0);
            m_UVs [i * 2 + 1].y = 1.0f;
        }
    }
     
    public void SetPosition (int aIndex, Vector3 aPosition)
    {
        if (aIndex < 0 || aIndex >= m_Points.Count)
            return;
        m_Points [aIndex].position = aPosition;
    }
     
    public void SetWidth (int aIndex, float aWidth)
    {
        if (aIndex < 0 || aIndex >= m_Points.Count)
            return;
        m_Points [aIndex].width = aWidth;
    }
     
    public void SetColor (int aIndex, Color aColor)
    {
        if (aIndex < 0 || aIndex >= m_Points.Count)
            return;
        m_Points [aIndex].color = aColor;
    }
     
    public void SetWidth (float aStartWidth, float aEndWidth)
    {
        for (int i = 0; i < m_Points.Count; i++) {
            m_Points [i].width = Mathf.Lerp (aStartWidth, aEndWidth, (float)i / (m_Points.Count - 1));
        }
    }
     
    public void SetColor (Color aStart, Color aEnd)
    {
        for (int i = 0; i < m_Points.Count; i++) {
            m_Points [i].color = Color.Lerp (aStart, aEnd, (float)i / (m_Points.Count - 1));
        }
    }
     
}