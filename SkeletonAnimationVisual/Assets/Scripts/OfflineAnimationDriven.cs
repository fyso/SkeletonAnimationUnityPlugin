using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineAnimationDriven : MonoBehaviour
{
    public string OfflineDataPath;
    public string OfflineVertexDataName;
    public string OfflineDataName;
    public uint OfflineDataFrameCount;

    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;
    private Mesh m_Mesh;
    void Start()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        if (m_MeshFilter == null)
            Debug.LogError("need MeshFilter component!");

        m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_MeshRenderer == null)
            Debug.LogError("need MeshRenderer component!");

        m_Mesh = new Mesh();
        m_MeshFilter.mesh = m_Mesh;

        UpdateMeshVertex(0);
        InitMeshIndices();
        InitMeshNormals(); 
        InitMeshUVs();

        m_Mesh.MarkDynamic();
        m_Mesh.Optimize();
        UpdateOtherPerporty();
    }

    void Update()
    {
        UpdateMeshVertex((uint)Time.frameCount % OfflineDataFrameCount);
        UpdateOtherPerporty();
    }

    private void UpdateOtherPerporty()
    {
        m_Mesh.RecalculateBounds();
        m_Mesh.RecalculateTangents();
        m_Mesh.MarkModified();
    }

    private void UpdateMeshVertex(uint vFrameCount)
    {
        TextAsset txt = Resources.Load(OfflineDataPath + "Vertex Frame\\" + OfflineVertexDataName + vFrameCount.ToString()) as TextAsset;

        string[] str = txt.text.Split('\n');
        List<Vector3> Positions = new List<Vector3>();
        int Count = 0;
        foreach (string strs in str)
        {
            if (Count >= str.Length - 2)
                break;

            string[] ss = strs.Split(' ');
            float X = float.Parse(ss[0]);
            float Y = float.Parse(ss[1]);
            float Z = float.Parse(ss[2].Substring(0, ss[2].Length - 1));
            Vector3 Position = new Vector3(X, Y, Z);
            Positions.Add(Position);
            Count++;
        }
        m_Mesh.SetVertices(Positions.ToArray());
    }

    private void InitMeshIndices()
    {
        TextAsset txt = Resources.Load(OfflineDataPath + OfflineDataName + "Indices") as TextAsset;
        string[] str = txt.text.Split('\n');
        List<int> Indices = new List<int>();
        int Count = 0;
        foreach (string strs in str)
        {
            if (Count >= str.Length - 2)
                break;

            int Index = int.Parse(strs.Substring(0, strs.Length - 1));
            Indices.Add(Index);
            Count++;
        }
        m_Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m_Mesh.SetIndices(Indices.ToArray(), MeshTopology.Triangles, 0);
    }

    private void InitMeshNormals()
    {
        TextAsset txt = Resources.Load(OfflineDataPath + OfflineDataName + "Normal") as TextAsset;

        string[] str = txt.text.Split('\n');
        List<Vector3> Normals = new List<Vector3>();
        int Count = 0;
        foreach (string strs in str)
        {
            if (Count >= str.Length - 2)
                break;

            string[] ss = strs.Split(' ');
            float X = float.Parse(ss[0]);
            float Y = float.Parse(ss[1]);
            float Z = float.Parse(ss[2].Substring(0, ss[2].Length - 1));
            Vector3 Normal = new Vector3(X, Y, Z);
            Normals.Add(Normal);
            Count++;
        }
        m_Mesh.SetNormals(Normals.ToArray());
    }

    private void InitMeshUVs()
    {
        TextAsset txt = Resources.Load(OfflineDataPath + OfflineDataName + "UV") as TextAsset;

        string[] str = txt.text.Split('\n');
        List<Vector2> UVs = new List<Vector2>();
        int Count = 0;
        foreach (string strs in str)
        {
            if (Count >= str.Length - 2)
                break;

            string[] ss = strs.Split(' ');
            float X = float.Parse(ss[0]);
            float Y = float.Parse(ss[1].Substring(0, ss[1].Length - 1));
            Vector2 UV = new Vector2(X, Y);
            UVs.Add(UV);
            Count++;
        }
        m_Mesh.SetUVs(0, UVs.ToArray());
    }

    private void ComputeRelativeParticleData()
    {

    }
}
