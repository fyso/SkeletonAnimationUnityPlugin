using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DrawLine : MonoBehaviour
{
    private List<BoneDefinition> m_BoneDefinitionList = new List<BoneDefinition>();
    private List<List<Matrix4x4>> m_AllLocalToWorld = new List<List<Matrix4x4>>();

    private string[] m_fileStr;
    private int FrameCount;
    private const int BONE_NR = 55;

    void Start()
    {
        FrameCount = 0;

        string FilePath = @"D:\My Project\SkeletonAnimationDataTransfer\captureData.csv";
        m_fileStr = File.ReadAllLines(FilePath);

        createBones();
    }

    void OnDrawGizmos()
    {
        ////按层级相连--绝对偏移(保存世界坐标位置)
        //for (int i = 0; i < m_BoneDefinitionList.Count; i++)
        //{
        //    Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //    int ParentIndex = m_BoneDefinitionList[i].ParentID;
        //    if (ParentIndex < 0)
        //        continue;
        //    Vector3 Start = m_BoneDefinitionList[ParentIndex].Offset;
        //    Vector3 End = m_BoneDefinitionList[i].Offset;
        //    Gizmos.DrawLine(Start, End);
        //}

        //按层级相连--从矩阵获取世界空间位置
        List<Vector3> InitialWorldPosList = new List<Vector3>(m_BoneDefinitionList.Count);
        List<Matrix4x4> InitialRTSList = m_AllLocalToWorld[0];
        if (FrameCount < m_AllLocalToWorld.Count)
        {
            InitialRTSList = m_AllLocalToWorld[FrameCount];
            FrameCount = (FrameCount + 1) % m_AllLocalToWorld.Count;
        }

        for (int i = 0; i < m_BoneDefinitionList.Count; i++)
        {
            Vector3 WorldPos = new Vector3(InitialRTSList[i].m03, InitialRTSList[i].m13, InitialRTSList[i].m23);
            InitialWorldPosList.Add(WorldPos);
        }

        for (int i = 0; i < m_BoneDefinitionList.Count; i++)
        {
            Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            int ParentIndex = m_BoneDefinitionList[i].ParentID;
            if (ParentIndex < 0)
                continue;
            Vector3 Start = InitialWorldPosList[ParentIndex];
            Vector3 End = InitialWorldPosList[i];
            Gizmos.DrawLine(Start, End);
        }
    }

    private void createBones()
    {
        for (int i = 0; i < m_fileStr.Length; i++)
        {
            string[] LineStr = m_fileStr[i].Split(',');
            if (LineStr[0].Equals("BONEEND"))
            {
                break;
            }
            else
            {
                BoneDefinition Definition = new BoneDefinition();
                Definition.ParentID = int.Parse(LineStr[0]);
                Definition.EnumerationValue = int.Parse(LineStr[1]);
                Definition.Offset = new Vector3();
                Definition.Offset.x = float.Parse(LineStr[2]);
                Definition.Offset.y = float.Parse(LineStr[3]);
                Definition.Offset.z = float.Parse(LineStr[4]);

                m_BoneDefinitionList.Add(Definition);
            }
        }

        getBonePose(m_BoneDefinitionList.Count+1, m_BoneDefinitionList.Count);
    }
    
    private void getBonePose(int vStartLine, int vCount)
    {
        for (int n = vStartLine; n < m_fileStr.Length;)
        {
            List<Matrix4x4> localToWorldList = new List<Matrix4x4>(vCount);
            int m = n;
            for (int i = 0; i < vCount; i++)
            {
                string[] str = m_fileStr[m + i].Split(',');
                Matrix4x4 TransferMatrix = new Matrix4x4();
                TransferMatrix.m00 = float.Parse(str[0]);
                TransferMatrix.m01 = float.Parse(str[1]);
                TransferMatrix.m02 = float.Parse(str[2]);
                TransferMatrix.m03 = float.Parse(str[3]);
                TransferMatrix.m10 = float.Parse(str[4]);
                TransferMatrix.m11 = float.Parse(str[5]);
                TransferMatrix.m12 = float.Parse(str[6]);
                TransferMatrix.m13 = float.Parse(str[7]);
                TransferMatrix.m20 = float.Parse(str[8]);
                TransferMatrix.m21 = float.Parse(str[9]);
                TransferMatrix.m22 = float.Parse(str[10]);
                TransferMatrix.m23 = float.Parse(str[11]);
                TransferMatrix.m30 = float.Parse(str[12]);
                TransferMatrix.m31 = float.Parse(str[13]);
                TransferMatrix.m32 = float.Parse(str[14]);
                TransferMatrix.m33 = float.Parse(str[15]);

                localToWorldList.Add(TransferMatrix);
            }
            n += vCount;
            m_AllLocalToWorld.Add(localToWorldList);
        }
    }
}
