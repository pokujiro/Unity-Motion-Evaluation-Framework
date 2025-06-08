using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
///  �R���[�`���@PlayBack�֐���
/// </summary>
public class TrackerDataPlayer1 : MonoBehaviour
{
    [Header("Playback Settings (Objects to be moved)")] // �������Ώۂ̃I�u�W�F�N�g
    [SerializeField] private Transform headTransform; // ���̃g���b�J�[��Transform�@
    [SerializeField] private Transform leftHandTransform; // ����̃g���b�J�[��Transform
    [SerializeField] private Transform rightHandTransform; // �E��̃g���b�J�[��Transform
    [SerializeField] private Transform leftFootTransform; // �����̃g���b�J�[��Transform
    [SerializeField] private Transform rightFootTransform; // �E���̃g���b�J�[��Transform
    [SerializeField] private Transform waistTransform; // ���̃g���b�J�[��Transform

    [SerializeField] private float playbackInterval = 0.1f; // �f�[�^���Đ�����Ԋu�i�b�j�@���R�[�_�[�̃C���^�[�o���Ɠ����łȂ���΂Ȃ�Ȃ�
    private List<List<TrackerDataRecorder.TrackerData>> recordedData; // �L�^���ꂽ�g���b�J�[�f�[�^
    private bool isPlaying = false; // �Đ������ǂ����������t���O

    /// <summary>
    /// �L�^���ꂽ�f�[�^��ݒ�
    /// </summary>
    /// <param name="data">�L�^���ꂽ�g���b�J�[�f�[�^�̃��X�g</param>
    public void SetRecordedData(List<List<TrackerDataRecorder.TrackerData>> data)
    {
        recordedData = data; // �L�^�f�[�^���Z�b�g
    }

    /// <summary>
    /// �f�[�^�̍Đ����J�n
    /// </summary>
    public void StartPlayback()
    {
        // �Đ��f�[�^�����݂��Ȃ��ꍇ�̓G���[��\��
        if (recordedData == null || recordedData.Count == 0)
        {
            Debug.LogError("No data to play."); // �f�[�^���Ȃ��ꍇ�̃G���[���O
            return;
        }

        // �Đ����J�n����Ă��Ȃ��ꍇ�̂ݍĐ����J�n
        if (!isPlaying)
        {
            isPlaying = true; // �Đ��t���O��ݒ�
            StartCoroutine(Playback()); // �Đ��R���[�`�����J�n
        }
    }

    /// <summary>
    /// �f�[�^�̍Đ����~
    /// </summary>
    public void StopPlayback()
    {
        isPlaying = false; // �Đ��t���O������
        StopAllCoroutines(); // �Đ����̃R���[�`����S�Ē�~

        // ���ׂẴR���[�`�����~����̂Ł@���Y���ƍ��킹���Ƃ��ɃG���[���N���邩������Ȃ�
        //
        //
        //
    }

    /// <summary>
    /// �L�^�f�[�^�����ԂɍĐ�����R���[�`��
    /// </summary>
    private IEnumerator Playback()
    {
        foreach (var frameData in recordedData) // �t���[�����Ƃ̃f�[�^�����[�v
        {
            if (!isPlaying) yield break; // �Đ��t���O����������Ă�����I��

            foreach (var tracker in frameData) // �e�g���b�J�[�̃f�[�^��K�p
            {
                ApplyTrackerData(tracker); // �g���b�J�[�̃f�[�^��K�p
            }

            yield return new WaitForSeconds(playbackInterval); // �Đ��Ԋu��ҋ@
        }

        Debug.Log("Playback finished."); // �Đ��������O
        isPlaying = false; // �Đ��t���O������
    }

    /// <summary>
    /// �g���b�J�[�f�[�^��K�p����Transform���X�V
    /// </summary>
    /// <param name="trackerData">�g���b�J�[�̈ʒu�Ɖ�]�f�[�^</param>
    private void ApplyTrackerData(TrackerDataRecorder.TrackerData trackerData)
    {
        switch (trackerData.name) // �g���b�J�[���ɉ����ēK�p���I��
        {
            case "Head": // ���̃g���b�J�[
                headTransform.position = trackerData.position;
                headTransform.rotation = trackerData.rotation;
                break;
            case "LeftHand": // ����̃g���b�J�[
                leftHandTransform.position = trackerData.position;
                leftHandTransform.rotation = trackerData.rotation;
                break;
            case "RightHand": // �E��̃g���b�J�[
                rightHandTransform.position = trackerData.position;
                rightHandTransform.rotation = trackerData.rotation;
                break;
            case "LeftFoot": // �����̃g���b�J�[
                leftFootTransform.position = trackerData.position;
                leftFootTransform.rotation = trackerData.rotation;
                break;
            case "RightFoot": // �E���̃g���b�J�[
                rightFootTransform.position = trackerData.position;
                rightFootTransform.rotation = trackerData.rotation;
                break;
            case "Waist": // ���̃g���b�J�[
                waistTransform.position = trackerData.position;
                waistTransform.rotation = trackerData.rotation;
                break;
        }
    }
}
