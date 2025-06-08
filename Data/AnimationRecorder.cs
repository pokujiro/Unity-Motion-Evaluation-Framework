using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�j���[�V�����̃{�[���f�[�^���L�^����X�N���v�g�i�o�ߎ��ԂŋL�^�Ǘ��j
/// Update�i�j
/// </summary>
public class AnimationRecorder : MonoBehaviour
{
    [Header(" Setting ")]
    public Animator animator; // �A�j���[�V�������Đ����Ă���Animator
    public Animator replayAnimator;  // ���v���C����A�o�^�[
    public DisplayBonesAsObjects displayBoneAsObject; // Bone�̐F��ύX
    public BoneConnector boneConnector; // BoneConnector �ւ̎Q��


    [Header(" Interval ")]
    public float recordInterval = 0.1f; // �L�^�̊Ԋu�i�b�j


    private List<BoneData> recordedData = new List<BoneData>(); // �L�^���ꂽ�{�[���f�[�^�̃��X�g
    private List<BoneData> referenceRecordedData = new List<BoneData>(); // �L�^���ꂽ�{�[���f�[�^�̃��X�g
    private List<FrameSimilarityData> recordedSimilarity = new List<FrameSimilarityData>(); // �t���[�����Ƃ̈�v�x���L�^


    private bool isRecording = false; // ���݋L�^�����ǂ����������t���O
    private float timeSinceLastRecord = 0f; // �Ō�ɋL�^���Ă���̌o�ߎ���

    [Header("�����ʒu")]
    private Vector3 initialWaistPosition; // ���̏����ʒu
    private Vector3 initialReplayWaistPosition; // ���̏����ʒu�i1��ڂ̃��v���C���j
    private bool isInitialized = false; // �����ʒu���ݒ肳�ꂽ��

    [Header("臒l")]
    public float positionThreshold; // �ʒu�덷��臒l
    public float rotationThreshold;  // ��]�덷��臒l�i�x���j
    private Dictionary<HumanBodyBones, Color> boneColors = new Dictionary<HumanBodyBones, Color>();


    [Header("�]�����̏d��")]
    public float positionWeight; // �ʒu�덷��臒l
    public float rotationWeight;  // ��]�덷��臒l�i�x���j


    [Header("�t�B�[�h�o�b�N������")]
    [SerializeField] private bool isFeedBack = true;
    [SerializeField] private bool isFeedBackArrow = false;

    [Header("�F�̔Z�W")]
    public float maxPositionError;  // �ʒu�덷�̍ő�臒l
    public float maxRotationError;   // ��]�덷�̍ő�臒l

    [Header("���")]
    [SerializeField] private GameObject arrowPrefab; // ���̃v���n�u
    private Dictionary<HumanBodyBones, GameObject> arrows = new Dictionary<HumanBodyBones, GameObject>();�@// ���I�u�W�F�N�g��ۑ�


    // �e�{�[�����Ƃ̌덷�f�[�^���L�^����\����
    [System.Serializable]
    public class BoneErrorData
    {
        public float positionError; // �|�W�V�����덷
        public float rotationError; // ��]�덷
    }

    // �t���[�����Ƃ̌덷�f�[�^
    [System.Serializable]
    public class FrameSimilarityData
    {
        public int frame; // �t���[���ԍ�
        public Dictionary<HumanBodyBones, BoneErrorData> boneErrors = new Dictionary<HumanBodyBones, BoneErrorData>(); // �e�{�[�����Ƃ̌덷
    }



    private List<BoneData> referenceData; // ��f�[�^
    private int startFrame = 0; // �L�^�J�n���̃t���[����

    // ��vBone �ɑΉ�����@��̓I�ȁ@Bone�@�̑Ή��\
    private Dictionary<string, string> boneNameMapping = new Dictionary<string, string>();


    // �{�[���f�[�^�̍\����
    [System.Serializable]
    public class BoneData
    {
        public string boneName; // �{�[����
        public Vector3 position; // �{�[���̈ʒu
        public Quaternion rotation; // �{�[���̉�]
        // public float normalizedTime; // �A�j���[�V�����̐��K�����ꂽ����
        public int frame; // �t���[���ԍ��i�I�v�V�����j
    }

    // ��v�ȃ{�[�����i�[�����z��
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

    private void Start()
    {
        // ��f�[�^�̋L�^���Ԃ����[�h
        //recordingDuration = PlayerPrefs.GetFloat(RecordingDurationKey, 0f);  // PlayerPrefs.GetFloat() �͕ۑ�����Ă���f�[�^��ǂݍ��ރ��\�b�h�ł�
                                                                             // ��1���� RecordingDurationKey: �f�[�^�����ʂ��邽�߂̃L�[
                                                                             // ��2���� 0f: �f�[�^�����݂��Ȃ��ꍇ�̃f�t�H���g�l�i���̏ꍇ��0�b�j


        foreach (HumanBodyBones bone in importantBones)
        {
            boneColors[bone] = Color.white; // �f�t�H���g�͔�
        }

        //Debug.Log($"Loaded recording duration: {recordingDuration:F2} seconds");
    }

    public void ProcessRecording(float deltaTime)
    {
        // �L�^���ł���AAnimator���ݒ肳��Ă���ꍇ�̂݋L�^���s��
        if (isRecording && animator != null)
        {
            // �o�ߎ��Ԃ��L�^�Ԋu�𒴂����ꍇ�ɋL�^
            timeSinceLastRecord += deltaTime;

            if (timeSinceLastRecord >= recordInterval)
            {
                // Debug.Log($"Real Interval (���ۂ̃C���^�[�o�� BoneRecorder) :  {timeSinceLastRecord}");
                RecordFrame(); // �e Bone ���̎擾
                timeSinceLastRecord = 0f; // �o�ߎ��Ԃ����Z�b�g
            }
        }
    }


    public void ProcessReplayRecording(float deltaTime)
    {
        // �L�^���ł���AAnimator���ݒ肳��Ă���ꍇ�̂݋L�^���s��
        if (isRecording && replayAnimator != null)
        {
            // �o�ߎ��Ԃ��L�^�Ԋu�𒴂����ꍇ�ɋL�^
            timeSinceLastRecord += deltaTime;

            if (timeSinceLastRecord >= recordInterval)
            {
                // Debug.Log($"Real Interval (���ۂ̃C���^�[�o�� BoneRecorder (Replay)) :  {timeSinceLastRecord}");
                ReplayRecordFrame(); // �e Bone ���̎擾
                timeSinceLastRecord = 0f; // �o�ߎ��Ԃ����Z�b�g
            }
        }
    }

    /// <summary>
    /// 1�t���[�����̃{�[���f�[�^���L�^���A��v�x���v�Z����
    /// </summary>
    private void RecordFrame()
    {

        if (!isInitialized) return; // �����ʒu���ݒ肳���܂ŋL�^���Ȃ�

        float totalError = 0f;
        int boneCount = 0;

        FrameSimilarityData frameData = new FrameSimilarityData
        {
            frame = Time.frameCount - startFrame
        };

        foreach (HumanBodyBones bone in importantBones)
        {
            Transform boneTransform = animator.GetBoneTransform(bone);
            if (boneTransform != null)
            {
                // ��r����Ώۂ����̏����ʒu��e���Ă���ꍇ��������g�p
                //BoneData currentBone = new BoneData
                //{
                //    boneName = boneTransform.name,
                //    position = boneTransform.position - initialWaistPosition, // ����̑��Έʒu
                //    rotation = boneTransform.rotation,
                //    frame = Time.frameCount - startFrame
                //};
                BoneData currentBone = new BoneData
                {
                    boneName = boneTransform.name,
                    position = boneTransform.position, // ����̑��Έʒu
                    rotation = boneTransform.rotation,
                    frame = Time.frameCount - startFrame
                };

                recordedData.Add(currentBone);

                float positionError = Vector3.Distance(replayAnimator.GetBoneTransform(bone).position, currentBone.position);
                //Debug.Log($"Position Error: {positionError} | RefBone: {refBone.position}, CurrentBone: {currentBone.position}");
                float rotationError = Quaternion.Angle(replayAnimator.GetBoneTransform(bone).rotation, currentBone.rotation);
                //Debug.Log($"Rotation Error: {rotationError}�� | RefBone: {refBone.rotation.eulerAngles}, CurrentBone: {currentBone.rotation.eulerAngles}");

                // **�덷��臒l�𒴂����ꍇ�́A臒l���������l�ɒ���**
                if (positionError > positionThreshold)
                {
                    positionError -= positionThreshold;
                    Debug.Log($"Position Error: {positionWeight * positionError} |realError{positionError + positionThreshold}");
                }
                else
                {
                    positionError = 0;
                }

                if (rotationError > rotationThreshold)
                {
                    rotationError -= rotationThreshold;
                    Debug.Log($"Rotation Error: {rotationWeight * rotationError} |realError{rotationError + rotationThreshold}");
                }
                else
                {
                    rotationError = 0;

                    if (isFeedBackArrow)
                    {
                        HideCorrectionArrow(bone);
                    }

                }

                // **�덷�f�[�^���L�^**
                frameData.boneErrors[bone] = new BoneErrorData
                {
                    positionError = positionError,
                    rotationError = rotationError,
                };
                if (isFeedBack)
                {


                    // �ʒu�덷�Ɖ�]�덷�𐳋K���i0.0 �` 1.0�j
                    float positionIntensity = Mathf.Clamp01(positionError / maxPositionError);
                    float rotationIntensity = Mathf.Clamp01(rotationError / maxRotationError);

                    // �����F�͔��i�덷�Ȃ��j
                    Color baseColor = Color.white;
                    Color targetColor = Color.white;

                    if (positionError > 0 && rotationError > 0)
                    {
                        targetColor = Color.red; // �����̌덷������ꍇ
                    }
                    else if (positionError > 0)
                    {
                        targetColor = Color.blue; // �ʒu�덷������ꍇ
                    }
                    else if (rotationError > 0)
                    {
                        targetColor = Color.yellow; // ��]�덷������ꍇ
                    }

                    // �덷�̋��x�ɉ����ĐF��ω��i�� �� targetColor�j
                    float intensity = Mathf.Max(positionIntensity, rotationIntensity);
                    boneColors[bone] = Color.Lerp(baseColor, targetColor, intensity);
                }

                // **�{�[�����ƂɐF��ύX**
                displayBoneAsObject.ExchangeBoneColor(bone.ToString(), boneColors[bone]);

                // **���K��������v�x�X�R�A���v�Z**
                float similarity = 1.0f - (positionWeight * positionError + (rotationError * rotationWeight)); // ��]�덷�̃X�P�[������
                similarity = Mathf.Clamp(similarity, 0.0f, 1.0f); //Mathf.Clamp() �ŃX�R�A�� 0.0�`1.0 �͈̔͂ɐ����B

                totalError += similarity;
                boneCount++;
                //// **��v�x�̕]��**
                //if (referenceData != null && referenceData.Count > 0)
                //{
                //    // **���݂̃t���[���ɍł��߂���f�[�^������**
                //    BoneData refBone = referenceData.Find(b =>
                //        b.boneName == currentBone.boneName &&
                //        Mathf.Abs(b.frame - currentBone.frame) <= 120 // ���e�t���[���덷
                //    );

                //    if (refBone != null)
                //    {
                //        float positionError = Vector3.Distance(refBone.position, currentBone.position);
                //        //Debug.Log($"Position Error: {positionError} | RefBone: {refBone.position}, CurrentBone: {currentBone.position}");
                //        float rotationError = Quaternion.Angle(refBone.rotation, currentBone.rotation);
                //        //Debug.Log($"Rotation Error: {rotationError}�� | RefBone: {refBone.rotation.eulerAngles}, CurrentBone: {currentBone.rotation.eulerAngles}");

                //        // **�덷��臒l�𒴂����ꍇ�́A臒l���������l�ɒ���**
                //        if (positionError > positionThreshold)
                //        {
                //            positionError -= positionThreshold; 
                //            Debug.Log($"Position Error: {positionWeight * positionError} |realError{positionError + positionThreshold}");
                //        }
                //        else
                //        {
                //            positionError = 0;
                //        }

                //        if (rotationError > rotationThreshold)
                //        {
                //            rotationError -= rotationThreshold;
                //            Debug.Log($"Rotation Error: {rotationWeight * rotationError} |realError{rotationError + rotationThreshold}");

                //            if (isFeedBackArrow)
                //            {
                //                // **��]�̕␳�������C��**
                //                Quaternion rotationDifference = refBone.rotation * Quaternion.Inverse(currentBone.rotation);
                //                Vector3 correctionDirection = rotationDifference * Vector3.forward;

                //                // **X���̉�]�덷�ɑΉ�**
                //                if (Mathf.Abs(correctionDirection.x) > Mathf.Abs(correctionDirection.z))
                //                {
                //                    correctionDirection = rotationDifference * Vector3.up;
                //                }
                //                // **���I�u�W�F�N�g��\��**
                //                ShowCorrectionArrow(bone, boneTransform, correctionDirection);
                //            }



                //        }
                //        else
                //        {
                //            rotationError = 0;

                //            if (isFeedBackArrow)
                //            {
                //                HideCorrectionArrow(bone);
                //            }

                //        }

                //        // **�덷�f�[�^���L�^**
                //        frameData.boneErrors[bone] = new BoneErrorData
                //        {
                //            positionError = positionError,
                //            rotationError = rotationError,
                //        };
                //        if (isFeedBack)
                //        {


                //            // �ʒu�덷�Ɖ�]�덷�𐳋K���i0.0 �` 1.0�j
                //            float positionIntensity = Mathf.Clamp01(positionError / maxPositionError);
                //            float rotationIntensity = Mathf.Clamp01(rotationError / maxRotationError);

                //            // �����F�͔��i�덷�Ȃ��j
                //            Color baseColor = Color.white;
                //            Color targetColor = Color.white;

                //            if (positionError > 0 && rotationError > 0)
                //            {
                //                targetColor = Color.red; // �����̌덷������ꍇ
                //            }
                //            else if (positionError > 0)
                //            {
                //                targetColor = Color.blue; // �ʒu�덷������ꍇ
                //            }
                //            else if (rotationError > 0)
                //            {
                //                targetColor = Color.yellow; // ��]�덷������ꍇ
                //            }

                //            // �덷�̋��x�ɉ����ĐF��ω��i�� �� targetColor�j
                //            float intensity = Mathf.Max(positionIntensity, rotationIntensity);
                //            boneColors[bone] = Color.Lerp(baseColor, targetColor, intensity);
                //        }

                //        // **�{�[�����ƂɐF��ύX**
                //        displayBoneAsObject.ExchangeBoneColor(bone.ToString(), boneColors[bone]);

                //        // **���K��������v�x�X�R�A���v�Z**
                //        float similarity = 1.0f - (positionWeight*positionError + (rotationError * rotationWeight)); // ��]�덷�̃X�P�[������
                //        similarity = Mathf.Clamp(similarity, 0.0f, 1.0f);�@//Mathf.Clamp() �ŃX�R�A�� 0.0�`1.0 �͈̔͂ɐ����B

                //        totalError += similarity;
                //        boneCount++;
                //    }
                //    else
                //    {
                //        Debug.Log("��f�[�^��������܂���");
                //    }
                //}
            }
        }
        // **�t���[�����Ƃ̌덷�f�[�^���L�^**
        recordedSimilarity.Add(frameData);

        float finalSimilarity = totalError / boneCount;
        Debug.Log($"���ʁF{finalSimilarity}");


        if (isFeedBack)
        {
            if (boneConnector != null)
            {
                Dictionary<string, (float positionError, float rotationError)> errorData = new Dictionary<string, (float, float)>();

                foreach (var boneError in frameData.boneErrors)
                {
                    string boneName = boneError.Key.ToString();
                    errorData[boneName] = (boneError.Value.positionError, boneError.Value.rotationError);
                }

                boneConnector.SetBoneErrors(errorData);
                boneConnector.ConnectBones();
            }
        }
        // === BoneConnector �Ɍ덷�f�[�^��n�� ===


    }

    public (float, float) GetWeight()
    {
        return (positionWeight, rotationWeight);
    }

    private void ShowCorrectionArrow(HumanBodyBones bone, Transform boneTransform, Vector3 direction)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("arrowPrefab �� null �̂��߁A���𐶐��ł��܂���B");
            return;
        }

        float offsetDistance = 0.1f; // �����{�[������ǂꂾ���������i�P��: ���[�g���j

        if (!arrows.ContainsKey(bone))
        {
            GameObject arrow = Instantiate(arrowPrefab, boneTransform.position, Quaternion.identity);
            arrow.transform.SetParent(boneTransform, true);
            arrows[bone] = arrow;
        }

        GameObject arrowObject = arrows[bone];
        arrowObject.SetActive(true);

        // �����{�[�����班�����ꂽ�ʒu�ɔz�u
        Vector3 offsetPosition = boneTransform.position + direction.normalized * offsetDistance;
        arrowObject.transform.position = offsetPosition;
        // **X���̉�]�덷������ꍇ�̕␳**
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            arrowObject.transform.rotation = Quaternion.LookRotation(Vector3.Cross(direction, Vector3.right));
        }
        else
        {
            arrowObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// �����\���ɂ���
    /// </summary>
    private void HideCorrectionArrow(HumanBodyBones bone)
    {
        if (arrows.ContainsKey(bone))
        {
            arrows[bone].SetActive(false);
        }
    }


    /// <summary>
    /// 1�t���[�����̃{�[���f�[�^���L�^���A��v�x���v�Z���� (Replay�̂�)
    /// 
    /// </summary>
    private void ReplayRecordFrame()
    {

        if (!isInitialized) return; // �����ʒu���ݒ肳���܂ŋL�^���Ȃ�

        foreach (HumanBodyBones bone in importantBones)
        {
            Transform boneTransform = replayAnimator.GetBoneTransform(bone);
            if (boneTransform != null)
            {
                BoneData currentBone = new BoneData
                {
                    boneName = boneTransform.name,
                    position = boneTransform.position - initialReplayWaistPosition, // ����̑��Έʒu
                    rotation = boneTransform.rotation,
                    frame = Time.frameCount - startFrame
                };

                referenceRecordedData.Add(currentBone);
            }
        }
    }

    /// <summary>
    /// �L�^���J�n����
    /// </summary>
    public void StartRecording()
    {
        recordedData.Clear(); // �ȑO�̋L�^���N���A
        recordedSimilarity.Clear(); // �ȑO�̋L�^���N���A

        isRecording = true; // �L�^�t���O���I��
        timeSinceLastRecord = 0f; // �o�ߎ��Ԃ����Z�b�g
        startFrame = Time.frameCount; // ���݂̃t���[�����L�^�J�n�t���[���Ƃ��Đݒ�
        Transform waistTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
        initialWaistPosition = waistTransform.position; // �������ʒu��ۑ�
        isInitialized = true;
        Debug.Log("Recording started.");
    }

    public void StartReplayRecording()
    {
        referenceRecordedData.Clear(); // �ȑO�̋L�^���N���A
        recordedSimilarity.Clear(); // �ȑO�̋L�^���N���A

        isRecording = true; // �L�^�t���O���I��
        timeSinceLastRecord = 0f; // �o�ߎ��Ԃ����Z�b�g
        startFrame = Time.frameCount; // ���݂̃t���[�����L�^�J�n�t���[���Ƃ��Đݒ�
        Transform waistTransform = replayAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialReplayWaistPosition = waistTransform.position; // �������ʒu��ۑ�
        isInitialized = true;
        Debug.Log("Recording started.");
    }



    /// <summary>
    /// �L�^���~����
    /// </summary>
    public void StopRecording()
    {
        isRecording = false; // �L�^�t���O���I�t
    }

    /// <summary>
    /// ��f�[�^��ǂݍ���
    /// </summary>
    /// <param name="reference"></param>
    public void SetReferenceData(List<BoneData> reference)
    {
        // ��f�[�^��ǂݍ���
        referenceData = reference;
    }

    /// <summary>
    /// �L�^���ꂽ��v�x�f�[�^���擾����
    /// </summary>
    public List<FrameSimilarityData> GetSimilarityData()
    {
        return recordedSimilarity;
    }

    /// <summary>
    /// �L�^���ꂽ�f�[�^���擾����
    /// </summary>
    /// <returns>�L�^���ꂽ�{�[���f�[�^�̃��X�g</returns>
    public List<BoneData> GetReferenceRecordedData()
    {
        return referenceRecordedData; // �L�^���ꂽ�f�[�^��Ԃ�
    }

    public List<BoneData> GetRecordedData()
    {
        return recordedData;
    }

    /// <summary>
    /// �O���Ń}�b�s���O���g����悤�ɂ���
    /// </summary>
    /// <returns>��v�{�[���ɑ΂��Ă̋�̓I�ȃ{�[���̃}�b�s���O</returns>
    public Dictionary<string, string> GetBoneNameMapping()
    {
        return boneNameMapping;
    }
}
