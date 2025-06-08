using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// �A�j���[�V�������Đ����A�L�^�ƕ]�����Ǘ�����R���g���[��
/// void Update�@�P
/// </summary>
public class AnimationController : MonoBehaviour
{
    [Header("Settings")]
    public Animator animator; // �A�j���[�V�����Đ��pAnimator
    public AnimationRecorder recorder; // �A�j���[�V�����L�^�p
    public AnimationDataSaver saver; // �f�[�^�ۑ��p
    public AnimationEvaluator evaluator; // �]���p
    public RhythmAndCountdownController rhythmController; // ���Y���R���g���[���[

    [Header("UI Elements")]
    public Text countdownText; // �J�E���g�_�E���\���p�̃e�L�X�g
    public Button startButton; // �X�^�[�g�{�^��
    public Button stopButton; // �X�g�b�v�{�^��

    [Header("is Evaluating")]
    public bool shouldEvaluate = false; // �]�����s�����ǂ����̃t���O

    //private bool isRecording = false; // �L�^�����ǂ���
    //private bool animationFinished = false; // �A�j���[�V�����I�����
    //private float recordingDuration; // ��f�[�^�̋L�^����
    //private float recordingStartTime; // �L�^�J�n����

    void Start()
    {
        //// �R���|�[�l���g�̊m�F
        //if (!ValidateComponents()) return;
        //// �{�^���Ƀ��X�i�[��ǉ�
        ////if (startButton != null)
        ////    startButton.onClick.AddListener(OnStartButtonPressed);

        //if (stopButton != null)
        //    stopButton.onClick.AddListener(OnStopButtonPressed);

        //// �ŏ��̓X�^�[�g�{�^���̂ݕ\��
        //ToggleButtons(true);
    }

    //void Update()
    //{
    //    // �L�^�J�n�I
    //    if (isRecording && shouldEvaluate)
    //    {
    //        // ��f�[�^�̋L�^���ԂɒB������L�^���~
    //        if (Time.time - recordingStartTime >= recordingDuration)
    //        {
    //            StopRecording();
    //            animationFinished = true; // �A�j���[�V�����I���t���O��ݒ�
    //            Debug.Log($"Time.time: {Time.time}, recordingStartTime: {recordingStartTime}, recordingDuration: {recordingDuration}");
    //            Debug.Log(" StopRecord by  Update");
    //        }
    //    }

    //    if (animationFinished)
    //    {
    //        Debug.Log("Animation has finished!");

    //        // �A�j���[�V�����I����̏���
    //        animationFinished = false; // ������Ƀ��Z�b�g
    //    }
    //}

    /// <summary>
    /// �{�^���̕\����؂�ւ���
    /// </summary>
    /// <param name="showStart">�X�^�[�g�{�^����\�����邩�ǂ���</param>
    void ToggleButtons(bool showStart)
    {
        if (startButton != null)
            startButton.gameObject.SetActive(showStart);

        if (stopButton != null)
            stopButton.gameObject.SetActive(!showStart);
    }

    /// <summary>
    /// �X�^�[�g�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    //void OnStartButtonPressed()
    //{
    //    if (rhythmController != null)
    //    {
    //        if (shouldEvaluate)
    //        {
    //            // �]������Ƃ������A��f�[�^�̋L�^���Ԃ��擾
    //            recordingDuration = recorder.GetRecordingDuration(); // ��f�[�^�̋L�^���Ԃ��擾
    //        }
    //        float delay = rhythmController.GetDelayInterval();  // ���Y���𓯊����邽�߂̎��Ԃ��擾
    //        StartCoroutine(StartRecordingWithDelay(delay));
    //    }
    //    else
    //    {
    //        Debug.LogError("RhythmController is not assigned!");
    //    }

    //    // �{�^����؂�ւ��i�X�g�b�v�{�^����\���j
    //    ToggleButtons(false);
    //}

    /// <summary>
    /// �L�^��x�����ĊJ�n
    /// </summary>
    //IEnumerator StartRecordingWithDelay(float delay)
    //{
    //    string[] countdown = { "3", "2", "1", "�X�^�[�g!" }; // �J�E���g�_�E���e�L�X�g
    //    float beatInterval = rhythmController.GetBeatInterval();
    //    yield return new WaitForSeconds(delay); // ���̃r�[�g�ɓ���
    //    foreach (string count in countdown)
    //    {
    //        countdownText.text = count; // �J�E���g�_�E�����X�V
    //        yield return new WaitForSeconds(beatInterval); // �e���|�ɍ��킹�ĕ\��
    //        Debug.Log($" beatInterval is {beatInterval} ");
    //    }
    //    countdownText.text = ""; // �J�E���g�_�E���I����ɔ�\��

    //    // Animator �̃X�e�[�g���m�F���čĐ�
    //    int layerIndex = 0; // Animator �̃��C���[
    //    if (animator.HasState(layerIndex, Animator.StringToHash("Boxing")))
    //    {
    //        animator.Play("Boxing", layerIndex, 0f); // "Boxing" �X�e�[�g���Đ�
    //        Debug.Log("Animation Boxing started.");
    //    }
    //    else
    //    {
    //        Debug.LogError("State 'Boxing' not found in Animator Controller.");
    //    }

    //    recordingStartTime = Time.time;
    //    StartRecording();
    //    Debug.Log("�L�^�J�n�@by AnimationController");

    //}

    /// <summary>
    /// �X�g�b�v�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    //void OnStopButtonPressed()
    //{
    //    StopRecording();
    //    Debug.Log("�L�^�I���@by AnimationController");

    //    // �{�^����؂�ւ��i�X�^�[�g�{�^����\���j
    //    ToggleButtons(true);
    //}

    /// <summary>
    /// �L�^���J�n
    /// </summary>
    //void StartRecording()
    //{
    //    if (recorder == null)
    //    {
    //        Debug.LogError("Recorder is not assigned!");
    //        return;
    //    }

    //    recorder.StartRecording(shouldEvaluate);
    //    isRecording = true;
    //    Debug.Log("Recording started. (Animation Controller)");
    //}

    /// <summary>
    /// �L�^���~
    ///// </summary>
    //void StopRecording()
    //{
    //    ToggleButtons(true);
    //    if (recorder == null || saver == null || evaluator == null)
    //    {
    //        Debug.LogError("Recorder, Saver, or Evaluator is not assigned!");
    //        return;
    //    }

    //    recorder.StopRecording(shouldEvaluate);
    //    isRecording = false;

    //    // �]���t���O�̊m�F
    //    if (shouldEvaluate)
    //    {
    //        // �]�����J�n
    //        evaluator.EvaluateSimilarity();
    //    }
    //    else
    //    {
    //        // �L�^�f�[�^��ۑ�
    //        saver.SaveReferenceData();
    //        Debug.Log("Reference data saved.");
    //    }
    //}

    /// <summary>
    /// �K�v�ȃR���|�[�l���g���ݒ肳��Ă��邩���m�F
    /// </summary>
    /// <returns>���ׂẴR���|�[�l���g���������ݒ肳��Ă����true</returns>
    //private bool ValidateComponents()
    //{
    //    if (animator == null)
    //    {
    //        Debug.LogError("Animator is not assigned!");
    //        return false;
    //    }

    //    if (recorder == null)
    //    {
    //        Debug.LogError("Recorder is not assigned!");
    //        return false;
    //    }

    //    if (saver == null)
    //    {
    //        Debug.LogError("Saver is not assigned!");
    //        return false;
    //    }

    //    if (evaluator == null)
    //    {
    //        Debug.LogError("Evaluator is not assigned!");
    //        return false;
    //    }

    //    if (rhythmController == null)
    //    {
    //        Debug.LogError("RhythmController is not assigned!");
    //        return false;
    //    }

    //    return true;
    //}
}


