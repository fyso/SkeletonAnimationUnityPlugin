using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RotateBones : MonoBehaviour
{
    public GameObject rootBone;

    private Animator m_Animator;

    private StreamWriter m_StreamWriter;
    private List<Transform> m_BoneTransformList = new List<Transform>();
    private List<List<Matrix4x4>> m_AllLocalToWorld = new List<List<Matrix4x4>>();
    private List<BoneDefinition> m_BoneDefinition = new List<BoneDefinition>();

    private string BONE_END = "BONEEND";
    private const int BONE_NR = 55;
    private bool WRITE = false;
    private int Count;
    private bool IsEnd = false;
    private int i = 0;

    void Start()
    {
        Count = 0;

        m_Animator = rootBone.GetComponent<Animator>();

        string FilePath = @"D:\My Project\SkeletonAnimationDataTransfer\captureData.csv";
        if (!File.Exists(FilePath))
        {
            m_StreamWriter = File.CreateText(FilePath);
            Debug.Log("File created successfully!");
        }
        else
        {
            File.WriteAllText(FilePath, "");
            m_StreamWriter = File.AppendText(FilePath);
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals(60);

        initBoneList();
    }

    void Update()
    {
        var animator = GetComponent<Animator>();
        var animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        //normalizedTime的值为0~1，0为开始，1为结束。

        if(!animator.enabled)
        {
            Debug.Log("pause");
        }
        if (animatorInfo.normalizedTime >= 1.0f && !WRITE && animatorInfo.IsTag("exit"))
        {
            AnimationEnd();
        }
    }

    //在动画中设置的结束标志
    void AnimationEnd()
    {
        for (int i = 0; i < m_AllLocalToWorld.Count; i++)
        {
            List<Matrix4x4> localToWorldList = m_AllLocalToWorld[i];
            for (int t = 0; t < localToWorldList.Count; t++)
            {
                Matrix4x4 matrix4X4 = localToWorldList[t];
                string matrixstr = matrix4X4.m00.ToString("f8") + "," + matrix4X4.m01.ToString("f8") + "," + matrix4X4.m02.ToString("f8") + "," + matrix4X4.m03.ToString("f8") + "," +
                    matrix4X4.m10.ToString("f8") + "," + matrix4X4.m11.ToString("f8") + "," + matrix4X4.m12.ToString("f8") + "," + matrix4X4.m13.ToString("f8") + "," +
                    matrix4X4.m20.ToString("f8") + "," + matrix4X4.m21.ToString("f8") + "," + matrix4X4.m22.ToString("f8") + "," + matrix4X4.m23.ToString("f8") + "," +
                    matrix4X4.m30.ToString("f8") + "," + matrix4X4.m31.ToString("f8") + "," + matrix4X4.m32.ToString("f8") + "," + matrix4X4.m33.ToString("f8");
                m_StreamWriter.WriteLine(matrixstr);
            }
        }
        WRITE = true;
        m_StreamWriter.Close();
        m_StreamWriter.Dispose();
        Debug.Log("Animation end!");
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (WRITE == false)
        {
            updateBoneTransform();
        }
    }

    private void initBoneList()
    {
        for (int i = 0; i < BONE_NR; i++)
        {
            Transform Current = m_Animator.GetBoneTransform(HumanBodyBones.Hips + i);
            if (Current != null)
            {
                BoneDefinition Definition = new BoneDefinition();
                Definition.EnumerationValue = i;
                Definition.Offset = Current.localPosition;
                //Definition.Offset = Current.position;
                m_BoneDefinition.Add(Definition);

                m_BoneTransformList.Add(Current);
            }
        }

        m_BoneDefinition[0].ParentID = -1;
        for (int i = 1; i < m_BoneDefinition.Count; i++)
        {
            Transform Current = m_BoneTransformList[i];
            m_BoneDefinition[i].ParentID = fetchParentIndex(i);
        }
        
        writeBoneListToFile();
        m_StreamWriter.WriteLine(BONE_END);
        updateBoneTransform();
    }

    private int fetchParentIndex(int vCurrentIndex)
    {
        Transform Current = m_BoneTransformList[vCurrentIndex];
        int RootInstanceID = m_BoneTransformList[0].GetInstanceID();
        int ParentInstanceID;
        int Index = -1;
        do
        {
            ParentInstanceID = Current.parent.GetInstanceID();
            for (int i = 0; i < m_BoneTransformList.Count; i++)
            {
                if (m_BoneTransformList[i].GetInstanceID() == ParentInstanceID)
                {
                    Index = i;
                    break;
                }
            }
            if (Index == -1)
                Current = Current.parent;
        } while (Index == -1 && ParentInstanceID != RootInstanceID);
        
        return Index;
    }

    private void writeBoneListToFile()
    {
        for (int i = 0; i < m_BoneDefinition.Count; i++)
        {
            BoneDefinition bone = m_BoneDefinition[i];
            string boneStr = bone.ParentID + "," + bone.EnumerationValue + "," + bone.Offset.x.ToString("f10")
                + "," + bone.Offset.y.ToString("f10") + "," + bone.Offset.z.ToString("f10");
            m_StreamWriter.WriteLine(boneStr);
        }
    }

    private void updateBoneTransform()
    {
        List<Matrix4x4> LocalToWorld = new List<Matrix4x4>(m_BoneTransformList.Count);
        for (int i = 0; i < m_BoneTransformList.Count; i++)
        {
            Matrix4x4 TransferMatrix = m_BoneTransformList[i].localToWorldMatrix;
            LocalToWorld.Add(TransferMatrix);
        }
        m_AllLocalToWorld.Add(LocalToWorld);
    }
}