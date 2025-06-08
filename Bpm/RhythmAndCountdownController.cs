using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


//  �R���[�`���@�P
public class RhythmAndCountdownController : MonoBehaviour
{
    [Header("Settings")]
    public float bpm = 120f; // �e���|�iBPM�j
    public int beat = 1;

    [Header("UI Elements")]
    public GameObject rhythmIndicatorPrefab; // ���Y���̃C���W�P�[�^�[��Prefab
    public Vector3 displayPosition = new Vector3(0, 2, 5); // ���Y���C���W�P�[�^�[��\������ʒu
    public Vector3 scale = new Vector3(1, 1, 1); // �C���W�P�[�^�[�̃X�P�[��

    [Header("Audio")]
    public AudioSource headBeatSound; // ��������炷AudioSource

    private GameObject rhythmIndicator; // ���Y�������o������UI�i��: Image��Circle�Ȃǁj
    private double nextBeatTime; // ���̃r�[�g�̃^�C�~���O
    private double beatInterval; // 1���̎��ԁi�b�j
    public int beatCount = 0; // ���݂̔���
    private bool isPlayingRhythm = true; // ���Y���Đ������ǂ���

    void Start()
    {
        // 1���̊Ԋu���v�Z
        beatInterval = 60f / bpm;

        // �C���W�P�[�^�[�𐶐����A�ʒu��ݒ�
        if (rhythmIndicatorPrefab != null)
        {
            rhythmIndicator = Instantiate(rhythmIndicatorPrefab, displayPosition, Quaternion.identity);
            rhythmIndicator.transform.localScale = scale; // �X�P�[���𒲐�
            rhythmIndicator.SetActive(false); // ������ԂŔ�\��
        }
        else
        {
            Debug.LogError("Rhythm Indicator Prefab is not assigned!");
        }
        // **�ŏ��̃r�[�g�^�C�~���O��␳**
        double dspTime = AudioSettings.dspTime;
        nextBeatTime = dspTime + (beatInterval - (dspTime % beatInterval));
        // ���Y�����Đ�
        StartCoroutine(ShowRhythm());
    }

    /// <summary>
    /// ���Y���̍Đ����J�n
    /// </summary>
    void StartRhythm()
    {
        isPlayingRhythm = true;
        StartCoroutine(ShowRhythm());
    }

    /// <summary>
    /// ���Y�����e���|�ɍ��킹�Ď��o�I�ɕ\��
    /// </summary>
    /// <summary>
    /// ���Y�����e���|�ɍ��킹�Ď��o�I�ɕ\�����A�����ŉ���炷
    /// </summary>
    IEnumerator ShowRhythm()
    {
        nextBeatTime = AudioSettings.dspTime + beatInterval; // ����̃r�[�g���Ԃ�ݒ�

        while (isPlayingRhythm)
        {
            double currentTime = AudioSettings.dspTime;

            if (currentTime >= nextBeatTime)
            {
                if (headBeatSound != null)
                {
                    headBeatSound.PlayScheduled(nextBeatTime);
                }

                if (rhythmIndicator != null)
                {
                    StartCoroutine(FlashIndicator());
                }

                // **���̃r�[�g�̃^�C�~���O���m���ɍX�V**
                nextBeatTime += beatInterval;
                beatCount++;
            }

            yield return null; // **���t���[���`�F�b�N**
        }
    }

    /// <summary>
    /// ���Y���̃C���W�P�[�^�[����u�_��������
    /// </summary>
    IEnumerator FlashIndicator()
    {
        rhythmIndicator.SetActive(true);
        yield return new WaitForSeconds((float)(beatInterval / 2));
        rhythmIndicator.SetActive(false);
    }

    // ���̔��܂ł̒x�����Ԃ�ς����B
    public double GetDelayInterval()
    {
        double currentTime = AudioSettings.dspTime;
        return nextBeatTime - currentTime;
    }


    // 1���̎��ԁi�b�j��Ԃ��B
    public double GetBeatInterval()
    {
        return beatInterval;
    }
}
