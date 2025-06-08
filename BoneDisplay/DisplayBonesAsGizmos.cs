using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBonesAsGizmos : MonoBehaviour
{
    public Animator animator; // �A�o�^�[��Animator�R���|�[�l���g
    public Color boneColor = Color.green; // �{�[���̐F

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        // �A�o�^�[����SkinnedMeshRenderer�i���b�V���p�j�����𖳌���
        foreach (var skinnedRenderer in animator.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedRenderer.enabled = false;
        }

        Debug.Log("All skinned meshes are now hidden. Only bones are visible.");
    }

    void OnDrawGizmos()
    {
        if (animator == null) return;

        Gizmos.color = boneColor;

        // HumanBodyBones�̂��ׂẴ{�[����`��
        foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (bone == HumanBodyBones.LastBone) continue;

            Transform boneTransform = animator.GetBoneTransform(bone);

            if (boneTransform != null)
            {
                // �{�[���̈ʒu�������ȋ��Ƃ��ĕ`��
                Gizmos.DrawSphere(boneTransform.position, 0.02f);

                // �{�[���̐e�q�֌W�����C���ŕ`��
                if (boneTransform.parent != null)
                {
                    Gizmos.DrawLine(boneTransform.position, boneTransform.parent.position);
                }
            }
        }
    }
}

