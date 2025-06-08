using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro���g�p���邽�߂̖��O���

// Update()�ŕ]�����ʂ�\��
public class EvaluationText : MonoBehaviour
{
    public TextMeshProUGUI similarityText; // TextMeshPro-Text(UI)�R���|�[�l���g
    public TextMeshProUGUI countDownText; // TextMeshPro-Text(UI)�R���|�[�l���g
    private AnimationEvaluator evaluator;

    void Start()
    {
        // AnimationEvaluator�R���|�[�l���g���擾
        evaluator = GetComponent<AnimationEvaluator>();
    }

    // �]�����ʂ�TextMeshPro�ɕ\��
    public void CountDownTextChange(string showtext)
    {
        countDownText.text = showtext;
    }

    public void ShowEvaluateSimilarity()
    {
        if (evaluator != null)
        {
            // similarityText�Ɍ��݂̈�v�x��ݒ�
            similarityText.text = "Similarity: " + evaluator.similarity.ToString("F2");
        }
    }
}
