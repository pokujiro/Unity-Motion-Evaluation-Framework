using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using static AnimationRecorder;
using System.Collections;
using System.Linq;
using TMPro;

public class AnimationEvaluator : MonoBehaviour
{
    [Header(" Settings ")]
    public Animator animator; // ���A���^�C���A�j���[�V������Animator
    public AnimationRecorder recorder; // AnimationRecorder�𗘗p���ă}�b�s���O���擾
    public AnimationDataSaver animationDataSaver;
    public AnimationRecorder animationRecorder;

    [Header(" reference data bone List ")]
    public List<AnimationRecorder.BoneData> referenceData; // ��f�[�^

    private List<FrameSimilarityData> similarityData; // ��v�x�f�[�^

    [Header(" result ")]
    public float similarity; // ��v�x

    private Dictionary<string, string> boneNameMapping; // �{�[�����̃}�b�s���O
    private Dictionary<HumanBodyBones, string> japanesebBoneNameMapping = new Dictionary<HumanBodyBones, string>()
{
    { HumanBodyBones.Hips, "��" },
    { HumanBodyBones.Spine, "�w��" },
    { HumanBodyBones.Chest, "��" },
    { HumanBodyBones.Neck, "��" },
    { HumanBodyBones.Head, "��" },
    { HumanBodyBones.LeftUpperArm, "����r" },
    { HumanBodyBones.RightUpperArm, "�E��r" },
    { HumanBodyBones.LeftLowerArm, "���O�r" },
    { HumanBodyBones.RightLowerArm, "�E�O�r" },
    { HumanBodyBones.LeftHand, "����" },
    { HumanBodyBones.RightHand, "�E��" },
    { HumanBodyBones.LeftUpperLeg, "������" },
    { HumanBodyBones.RightUpperLeg, "�E����" },
    { HumanBodyBones.LeftLowerLeg, "���G��" },
    { HumanBodyBones.RightLowerLeg, "�E�G��" },
    { HumanBodyBones.LeftFoot, "����" },
    { HumanBodyBones.RightFoot, "�E��" }
};

    // ��v�ȃ{�[�����X�g�iAnimationRecorder�ƈ�v������j
    private readonly HumanBodyBones[] importantBones = new HumanBodyBones[]
    {
        HumanBodyBones.Hips,
        HumanBodyBones.Spine,
        HumanBodyBones.Chest,
        HumanBodyBones.Neck,
        HumanBodyBones.Head,
        HumanBodyBones.LeftUpperArm,
        HumanBodyBones.RightUpperArm,
        HumanBodyBones.LeftLowerArm,
        HumanBodyBones.RightLowerArm,
        HumanBodyBones.LeftHand,
        HumanBodyBones.RightHand,
        HumanBodyBones.LeftUpperLeg,
        HumanBodyBones.RightUpperLeg,
        HumanBodyBones.LeftLowerLeg,
        HumanBodyBones.RightLowerLeg,
        HumanBodyBones.LeftFoot,
        HumanBodyBones.RightFoot
    };

    void Start()
    {
        // �R���|�[�l���g�̊m�F
        if (!ValidateComponents()) return;
        boneNameMapping = recorder.GetBoneNameMapping();
        Debug.Log("Bone name mapping loaded.");
    }

    /// <summary>
    /// �e�t���[�����Ƃ̃{�[���̈�v�x��]���i�e�{�[�����Ƃ̌덷���o�́j
    /// </summary>
    public void EvaluateSimilarity(List<FrameSimilarityData> frameSimilarity)
    {
        similarityData = frameSimilarity;

        (float positionWeight, float rotationWeight) = animationRecorder.GetWeight();

        // �v�Z�J�n���Ԃ��L�^
        float startTimeEmu = Time.realtimeSinceStartup;

        // �W�v�p�̃��X�g�i�S�t���[���E�S�{�[���̌덷�f�[�^�j
        List<float> positionErrors = new List<float>();
        List<float> rotationErrors = new List<float>();

        // �e�{�[�����Ƃ̃G���[�f�[�^���i�[���鎫��
        Dictionary<HumanBodyBones, List<BoneErrorData>> boneErrorRecords = new Dictionary<HumanBodyBones, List<BoneErrorData>>();

        foreach (HumanBodyBones bone in importantBones)
        {
            boneErrorRecords[bone] = new List<BoneErrorData>();
        }

        // �e�t���[���̃f�[�^������
        foreach (var frameData in similarityData)
        {
            //Debug.Log($"[Frame {frameData.frame}]");

            foreach (var boneError in frameData.boneErrors)
            {
                positionErrors.Add(boneError.Value.positionError);
                rotationErrors.Add(boneError.Value.rotationError);

                // �{�[�����Ƃ̃G���[���L�^
                boneErrorRecords[boneError.Key].Add(boneError.Value);

                // �e�{�[���̌덷�����O�o��
                // Debug.Log($"{boneError.Key}: PosErr = {boneError.Value.positionError:F4}, RotErr = {boneError.Value.rotationError:F4}");
            }
        }

        // **���v�v�Z�i�|�W�V�����덷�j**
        float avgPositionError = positionErrors.Average();
        float maxPositionError = positionErrors.Max();
        float minPositionError = positionErrors.Min();
        float stdDevPosition = Mathf.Sqrt(positionErrors.Average(p => Mathf.Pow(p - avgPositionError, 2)));

        // **���v�v�Z�i��]�덷�j**
        float avgRotationError = rotationErrors.Average();
        float maxRotationError = rotationErrors.Max();
        float minRotationError = rotationErrors.Min();
        float stdDevRotation = Mathf.Sqrt(rotationErrors.Average(r => Mathf.Pow(r - avgRotationError, 2)));

        // **�{�[�����Ƃ̌덷���v**
        Debug.Log($"=== Per-Bone Error Statistics ===");
        foreach (var bone in importantBones)
        {
            if (boneErrorRecords[bone].Count > 0)
            {
                float boneAvgPosErr = boneErrorRecords[bone].Average(e => e.positionError);
                float boneAvgRotErr = boneErrorRecords[bone].Average(e => e.rotationError);
                // Debug.Log($"{bone}: AvgPosErr = {boneAvgPosErr:F4}, AvgRotErr = {boneAvgRotErr:F4}");
            }
        }
        // **UI �Ƀ{�[���덷��\��**
        UpdateBoneErrorUI(boneErrorRecords);

        // **�ŏI�I�ȃX�R�A (Similarity) �̌v�Z**
        similarity = 1.0f - (avgPositionError * positionWeight + avgRotationError * rotationWeight);
        similarity = Mathf.Clamp(similarity, 0.0f, 1.0f);

        Debug.Log($"=== Final Similarity Report ===\n" +
                  $" - Position Error: Avg = {avgPositionError:F4}, Max = {maxPositionError:F4}, Min = {minPositionError:F4}, StdDev = {stdDevPosition:F4}\n" +
                  $" - Rotation Error: Avg = {avgRotationError:F4}, Max = {maxRotationError:F4}, Min = {minRotationError:F4}, StdDev = {stdDevRotation:F4}\n" +
                  $" - Final Similarity Score: {similarity:F4}");

        // UI �ɕ\��
        UpdateSimilarityUI(avgPositionError, maxPositionError, minPositionError, stdDevPosition,
                           avgRotationError, maxRotationError, minRotationError, stdDevRotation, similarity);


        // �v�Z���Ԃ��L�^
        float endTimeEmu = Time.realtimeSinceStartup;
        float elapsedTimeEmu = (endTimeEmu - startTimeEmu) * 1000; // �~���b�P��
        Debug.Log($"Time to calculate similarity: {elapsedTimeEmu:F2} ms");
    }

    public TextMeshProUGUI boneErrorText; // �{�[���̌덷�f�[�^�\���p UI
    public TextMeshProUGUI similarityText; // UI �e�L�X�g


    private void UpdateBoneErrorUI(Dictionary<HumanBodyBones, List<BoneErrorData>> boneErrorRecords)
    {
        if (boneErrorText == null) return;

        float positionErrorThreshold = 0.15f; // �ʒu�덷��臒l
        float rotationErrorThreshold = 45f;   // ��]�덷��臒l

        string uiText = "<size=18><b>=== �{�[���덷���v ===</b></size>\n";

        foreach (var bone in boneErrorRecords.Keys)
        {
            if (boneErrorRecords[bone].Count > 0)
            {
                float boneAvgPosErr = boneErrorRecords[bone].Average(e => e.positionError);
                float boneAvgRotErr = boneErrorRecords[bone].Average(e => e.rotationError);

                // **�{�[���̓��{�ꖼ���擾**
                string boneName = japanesebBoneNameMapping.ContainsKey(bone) ? japanesebBoneNameMapping[bone] : bone.ToString();

                // **�덷�̔���**
                bool isPositionErrorHigh = boneAvgPosErr > positionErrorThreshold;
                bool isRotationErrorHigh = boneAvgRotErr > rotationErrorThreshold;

                // **�{�[�����̐F����i�ǂ��炩�̌덷���傫����ΐԂ�����j**
                string boneColorTag = (isPositionErrorHigh || isRotationErrorHigh) ? "<color=red>" : "<color=#90EE90>";

                // **�덷�̒l���Ƃ̐F����**
                string posErrColorTag = isPositionErrorHigh ? "<color=red>" : "<color=#90EE90>"; //���C�g��r�|��
                string rotErrColorTag = isRotationErrorHigh ? "<color=red>" : "<color=#90EE90>";

                // **UI�e�L�X�g�ɔ��f**
                uiText += $"{boneColorTag}{boneName}</color>: " +
                          $"�ʒu�덷 = {posErrColorTag}{boneAvgPosErr:F3}</color>, " +
                          $"��]�덷 = {rotErrColorTag}{boneAvgRotErr:F3}</color>\n";
            }
        }

        boneErrorText.text = uiText;
        Debug.Log($"{uiText}");
    }



    private void UpdateSimilarityUI(float avgPositionError, float maxPositionError, float minPositionError, float stdDevPosition,
                                    float avgRotationError, float maxRotationError, float minRotationError, float stdDevRotation,
                                    float similarity)
    {
        if (similarityText != null)
        {
            similarityText.text = $"=== Final Similarity Report ===\n" +
                                  $" - P_Error:\n   Avg = {avgPositionError:F3}, Max = {maxPositionError:F3}, Min = {minPositionError:F3}\n" +
                                  $" - R_Error:\n   Avg = {avgRotationError:F3}, Max = {maxRotationError:F3}, Min = {minRotationError:F3}\n" +
                                  $" - Final Similarity Score: {similarity:F3}";
        }
    }

    private void FinalizeSimilarityReport(float avgPositionError, float maxPositionError, float minPositionError, float stdDevPosition,
                                          float avgRotationError, float maxRotationError, float minRotationError, float stdDevRotation,
                                          float similarity)
    {
        Debug.Log($"=== Final Similarity Report ===\n" +
                  $" - Position Error: Avg = {avgPositionError:F4}, Max = {maxPositionError:F4}, Min = {minPositionError:F4}, StdDev = {stdDevPosition:F4}\n" +
                  $" - Rotation Error: Avg = {avgRotationError:F4}, Max = {maxRotationError:F4}, Min = {minRotationError:F4}, StdDev = {stdDevRotation:F4}\n" +
                  $" - Final Similarity Score: {similarity:F4}");

        // UI �ɕ\��
        UpdateSimilarityUI(avgPositionError, maxPositionError, minPositionError, stdDevPosition,
                           avgRotationError, maxRotationError, minRotationError, stdDevRotation, similarity);
    }



    /// <summary>
    /// ��v�x��]��
    /// </summary>
    public void OldEvaluateSimilarity()
    {
        // �����J�n�������L�^
        float startTime = Time.realtimeSinceStartup;

        // JSON�t�@�C�������f�[�^��ǂݍ���
        referenceData = animationDataSaver.LoadReferenceData();


        // �����I���������L�^
        float endTime = Time.realtimeSinceStartup;

        // �o�ߎ��Ԃ��v�Z���ă��O�ɏo��
        float elapsedTime = (endTime - startTime) * 1000; // �~���b�P��
        Debug.Log($"Time to load and parse reference data: {elapsedTime:F2} ms");

        // ��v�x�̌v�Z�������J�n
        float startTimeEmu = Time.realtimeSinceStartup;
        if (referenceData == null || referenceData.Count == 0)
        {
            Debug.LogError("Reference data is not set or empty."); // ��f�[�^���ݒ肳��Ă��Ȃ��ꍇ
            return;
        }

        float totalError = 0f;

        // 臒l�̐ݒ�
        float positionThreshold = 0.05f;  // 5cm
        float rotationThreshold = 10f;    // 10�x
        int validComparisons = 0;  // �L���Ȕ�r�����J�E���g

        // ���݂̃A�j���[�V�����f�[�^���擾
        var currentData = recorder.GetRecordedData();
        
        if (currentData == null || currentData.Count == 0)
        {
            Debug.LogError("No recorded data found for evaluation.");
            return;
        }

        if (referenceData == null || referenceData.Count == 0)
        {
            Debug.LogError("Reference data is not set or empty.");
            return;
        }

        for (int i = 0; i < referenceData.Count; i++)
        {
            var refBone = referenceData[i];

            // ���݂̃f�[�^���瓯���t���[���̃f�[�^������
            var currentBone = currentData.Find(b =>
                b.boneName == refBone.boneName &&
                Mathf.Abs(b.frame - refBone.frame) <= 120 // ���e�t���[���덷
            );

            // ��r�f�[�^���s�����Ă���ꍇ�ɃX�L�b�v
            if (currentBone == null)
            {
                Debug.LogWarning($"Skipped bone: {refBone.boneName}. No matching data found within the allowed range.");
                continue;
            }

            // �ʒu�덷���v�Z
            float positionError = Vector3.Distance(refBone.position, currentBone.position);
            Debug.Log($"Position ����{refBone.position} �{currentBone.position}");
            if (positionError < positionThreshold)
            {
                Debug.Log($"臒l�덷 Position�@{positionError}");
                positionError = 0f; // 臒l�����̌덷�𖳎�

            }

            // ��]�덷���v�Z
            float rotationError = Quaternion.Angle(refBone.rotation, currentBone.rotation);
            if (rotationError < rotationThreshold)
            {
                rotationError = 0f; // 臒l�����̌덷�𖳎�
                Debug.Log($"臒l�덷 Rotation�@{rotationError}");
            }

            // �G���[��ݐρi��]�̉e���𒲐��j
            totalError += positionError + (rotationError * 0.1f);

            // �L���Ȕ�r�񐔂��J�E���g
            validComparisons++;

            Debug.Log($"Bone: {refBone.boneName}, PositionError: {positionError:F4}, RotationError: {rotationError:F2}");
        }

        // �S�̂̈�v�x���v�Z
        if (validComparisons > 0)
        {
            similarity = 1.0f - (totalError / validComparisons); // �L���Ȕ�r���Ŋ���
        }
        else
        {
            similarity = 0f; // ��r�f�[�^���Ȃ����0
        }

        // �����I���������L�^
        float endTimeEmu = Time.realtimeSinceStartup;

        // �o�ߎ��Ԃ��v�Z���ă��O�ɏo��
        float elapsedTimeEmu = (endTimeEmu - startTimeEmu) * 1000; // �~���b�P��
        Debug.Log($"Time to calculate similarity: {elapsedTimeEmu:F2} ms");
    }
    private bool ValidateComponents()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return false;
        }

        if (recorder == null)
        {
            Debug.LogError("Recorder is not assigned!");
            return false;
        }
        if (animationDataSaver  == null)
        {
            Debug.LogError("animationDataSavar is not assigned!");
            return false;
        }
        return true;
    }

    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> bones;
    }
}


