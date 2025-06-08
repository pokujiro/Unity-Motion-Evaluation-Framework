using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayAsObject : MonoBehaviour
{
    public Animator animator; // �A�o�^�[��Animator�R���|�[�l���g
    public GameObject bonePrefab; // �{�[����\�����邽�߂�Prefab�i��: �����ȋ��j
    public Material boneMaterial; // �{�[���̃}�e���A���i�ύX�p�j
    public Material lineMaterial; // ����`�悷�邽�߂̃}�e���A��

    // private List<GameObject> boneObjects = new List<GameObject>(); // ���������{�[���I�u�W�F�N�g
    public Dictionary<string, GameObject> boneObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, LineRenderer> lineRenderers = new Dictionary<string, LineRenderer>();

    void Start()
    {
        if (animator == null || bonePrefab == null || lineMaterial == null || boneMaterial == null)
        {
            Debug.LogError("Animator, Bone Prefab, Line Material, or Bone Material is not assigned!");
            return;
        }

        Debug.Log("Starting bone visualization...");

        foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (bone == HumanBodyBones.LastBone) continue;

            Transform boneTransform = animator.GetBoneTransform(bone);

            if (boneTransform != null)
            {
                // �{�[���̈ʒu��Prefab�𐶐�
                GameObject boneObject = Instantiate(bonePrefab, boneTransform.position, Quaternion.identity, boneTransform);
                // Debug.Log($" �I�u�W�F�N�g�̈ʒu�@{boneTransform.position}");
                boneObject.transform.SetParent(null, true); // ������ 'true' �Ń��[���h���W���ێ�

                boneObject.name = bone.ToString();
                boneObjects[boneObject.name] = boneObject;

                // �{�[���̃}�e���A����ݒ�
                MeshRenderer renderer = boneObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(boneMaterial); // �C���X�^���X�����ČʕύX�\��
                }

                // ����̃{�[���i��: Hips�j�𖳎�
                if (bone == HumanBodyBones.Hips) continue;

                // �e�����݂���ꍇ�ɐ���`��
                if (boneTransform.parent != null)
                {
                    // LineRenderer��ǉ����Đ����Ȃ�
                    GameObject lineObject = new GameObject($"{bone}_Line");
                    LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                    lineRenderer.material = new Material(lineMaterial); // �ʂ̃}�e���A����ݒ�
                    lineRenderer.startWidth = 0.02f;
                    lineRenderer.endWidth = 0.02f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, boneTransform.position);
                    lineRenderer.SetPosition(1, boneTransform.parent.position);
                    lineRenderers[boneObject.name] = lineRenderer;

                    // �X�V�p�X�N���v�g��ǉ����Đ���Ǐ]
                    ReplayBoneLineUpdater updater = lineObject.AddComponent<ReplayBoneLineUpdater>();
                    updater.startTransform = boneTransform;
                    updater.endTransform = boneTransform.parent;
                }
            }
            else
            {
                Debug.LogWarning($"Bone not found: {bone}");
            }
        }

        Debug.Log("Bone visualizer with lines initialized.");
    }

    /// <summary>
    /// **����̃{�[���̐F��ύX**
    /// </summary>
    public void ExchangeBoneColor(string boneName, Color newColor)
    {
        if (boneObjects.ContainsKey(boneName))
        {
            MeshRenderer renderer = boneObjects[boneName].GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = newColor;
            }
        }

        if (lineRenderers.ContainsKey(boneName))
        {
            LineRenderer lineRenderer = lineRenderers[boneName];
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
            if (lineRenderer.material != null)
            {
                lineRenderer.material.color = newColor;
            }
        }
    }

    void Update()
    {
        foreach (var bone in boneObjects)
        {
            if (animator != null)
            {
                Transform boneTransform = animator.GetBoneTransform((HumanBodyBones)System.Enum.Parse(typeof(HumanBodyBones), bone.Key));
                if (boneTransform != null)
                {
                    bone.Value.transform.position = boneTransform.position;
                }
            }
        }
    }

    /// <summary>
    /// �{�[���ƃ��C�����\���ɂ���
    /// </summary>
    public void HideBonesAndLines()
    {
        foreach (var bone in boneObjects.Values)
        {
            if (bone != null)
            {
                bone.SetActive(false);
            }
        }

        foreach (var line in lineRenderers.Values)
        {
            if (line != null && line.gameObject != null)
            {
                line.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// �{�[���ƃ��C����\������
    /// </summary>
    public void ShowBonesAndLines()
    {
        foreach (var bone in boneObjects.Values)
        {
            if (bone != null)
            {
                bone.SetActive(true);
            }
        }

        foreach (var line in lineRenderers.Values)
        {
            if (line != null && line.gameObject != null)
            {
                line.gameObject.SetActive(true);
            }
        }
    }


}



// LineRenderer���{�[���ɒǏ]������X�N���v�g
public class ReplayBoneLineUpdater : MonoBehaviour
{
    public DisplayBonesAsObjects displayBonesAsObjects;
    public Transform startTransform; // ���̎n�_
    public Transform endTransform;   // ���̏I�_
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (lineRenderer != null && startTransform != null && endTransform != null)
        {
            lineRenderer.SetPosition(0, startTransform.position);
            lineRenderer.SetPosition(1, endTransform.position);
        }

        
    }
}
