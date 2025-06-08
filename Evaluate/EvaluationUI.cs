using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationUI : MonoBehaviour
{
    public Text evaluationText; // �]�����ʂ�\������UI�e�L�X�g
    public MotionEvaluator evaluator;

    void Update()
    {
        // �]�����ʂ��擾�i����MotionEvaluator����擾����ꍇ�j
        string result = $"Left Hand Error: {evaluator.leftPositionError}, {evaluator.leftRotationError}\n" +
                        $"Right Hand Error: {evaluator.rightPositionError}, {evaluator.rightRotationError}";

        // �e�L�X�g���X�V
        evaluationText.text = result;
    }
}