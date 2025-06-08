using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovdRender : MonoBehaviour
{
    public Animator animator; // �A�o�^�[��Animator�R���|�[�l���g

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        // �A�o�^�[���̂��ׂĂ�Renderer�𖳌����i���b�V����\���j
        foreach (var renderer in animator.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        Debug.Log("All meshes are now hidden. Only bones are visible.");
    }
}

