using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionEvaluator : MonoBehaviour
{
    public float leftPositionError { get; private set; }
    public float rightPositionError { get; private set; }
    public float leftRotationError { get; private set; }
    public float rightRotationError { get; private set; }

    public Animator targetAnimator; // �A�j���[�V�������A�^�b�`���ꂽAnimator
    public Animator animator;
    public Transform referenceLeftHand; // ��ƂȂ鍶��
    public Transform referenceRightHand; // ��ƂȂ�E��

    private Transform leftHandBone;
    private Transform rightHandBone;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        // �A�o�^�[�̍���ƉE��̃{�[����Animator����擾
        referenceLeftHand = targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        referenceRightHand = targetAnimator.GetBoneTransform(HumanBodyBones.RightHand);

        leftHandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);

        // �m�F�p���O
        Debug.Log("Reference Left Hand: " + referenceLeftHand.name);
        Debug.Log("Reference Right Hand: " + referenceRightHand.name);
    }

    void Update()
    {
        // ��{�[���i�A�j���[�V�������̃{�[���j�̈ʒu���]���g�p���ĕ]���\
        //if (referenceLeftHand != null)
        //{
        //    Debug.Log($"Reference Left Hand Position: {referenceLeftHand.position}");
        //}
        //if (referenceRightHand != null)
        //{
        //    Debug.Log($"Reference Right Hand Position: {referenceRightHand.position}");
        //}


        // �]���̍ۂɎg�p

        if (referenceLeftHand != null && leftHandBone != null)
        {
            // �ʒu�̌덷���v�Z
            leftPositionError = Vector3.Distance(referenceLeftHand.position, leftHandBone.position);

            // ��]�̌덷���v�Z
            leftRotationError = Quaternion.Angle(referenceLeftHand.rotation, leftHandBone.rotation);

            Debug.Log($"Left�@Position Error: {leftPositionError}, Rotation Error: {leftRotationError}");
        }

        if (referenceLeftHand != null && leftHandBone != null)
        {
            // �ʒu�̌덷���v�Z
            rightPositionError = Vector3.Distance(referenceRightHand.position, rightHandBone.position);

            // ��]�̌덷���v�Z
            rightRotationError = Quaternion.Angle(referenceRightHand.rotation, rightHandBone.rotation);

            Debug.Log($"Left�@Position Error: {rightPositionError}, Rotation Error: {rightRotationError}");
        }
    }
}


