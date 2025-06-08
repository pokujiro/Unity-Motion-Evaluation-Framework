using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// key input system

public class AnimationSystemController : MonoBehaviour
{

    //  �L�[���͂Ő���@�f�[�^�̕ۑ��Ȃ�


    public Animator animator; // �A�j���[�V�������Đ�����A�o�^�[
    public Text similarityText; // �]���X�R�A��\������UI

    private AnimationRecorder recorder;
    private AnimationDataSaver saver;
    private AnimationEvaluator evaluator;

    void Start()
    {
        // �e�X�N���v�g�̎Q�Ƃ��擾
        recorder = animator.GetComponent<AnimationRecorder>();
        saver = animator.GetComponent<AnimationDataSaver>();
        evaluator = animator.GetComponent<AnimationEvaluator>();
    }
}
