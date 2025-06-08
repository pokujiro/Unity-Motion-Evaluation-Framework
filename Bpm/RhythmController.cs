using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmController : MonoBehaviour
{
    public GameObject rhythmIndicatorPrefab; // ���Y���̃C���W�P�[�^�[��Prefab
    public float beatInterval = 1.0f; // ���Y���̊Ԋu�i�b�j
    public Vector3 displayPosition = new Vector3(0, 2, 5); // ���Y���C���W�P�[�^�[��\������ʒu
    public Vector3 scale = new Vector3(1, 1, 1); // �C���W�P�[�^�[�̃X�P�[��



    private GameObject rhythmIndicator; // �C���W�P�[�^�[�̃C���X�^���X
    private bool isRhythmPlaying = true; // ���Y���̍Đ��t���O

    void Start()
    {
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

        // ���Y�����Đ�
        StartCoroutine(PlayRhythm());
    }

    IEnumerator PlayRhythm()
    {
        while (isRhythmPlaying)
        {
            if (rhythmIndicator != null)
            {
                rhythmIndicator.SetActive(true); // �C���W�P�[�^�[��\��
                yield return new WaitForSeconds(beatInterval / 2); // �_������

                rhythmIndicator.SetActive(false); // �C���W�P�[�^�[���\��
                yield return new WaitForSeconds(beatInterval / 2); // ��������
            }
        }
    }

    public void StopRhythm()
    {
        isRhythmPlaying = false; // ���Y�����~
        if (rhythmIndicator != null)
            rhythmIndicator.SetActive(false); // �C���W�P�[�^�[���\��
    }

    public void StartRhythm()
    {
        if (!isRhythmPlaying)
        {
            isRhythmPlaying = true;
            StartCoroutine(PlayRhythm()); // ���Y�����ĊJ
        }
    }
}

