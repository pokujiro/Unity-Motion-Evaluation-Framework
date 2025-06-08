using UnityEngine;

public class BoneErrorVisualizer : MonoBehaviour
{
    public Transform referenceBone; // ��f�[�^�̃{�[��
    public Transform currentBone;   // ���݂̃{�[��
    public Material boneMaterial;   // �{�[���̕\���p�}�e���A��
    public LineRenderer lineRenderer; // �{�[���Ԃ̃��C��

    public float positionThreshold = 0.05f; // �ʒu�덷��臒l
    public float rotationThreshold = 10f;   // ��]�덷��臒l
    private Color defaultColor = Color.white;

    void Update()
    {
        if (referenceBone == null || currentBone == null) return;

        // �ʒu�덷���v�Z
        float positionError = Vector3.Distance(referenceBone.position, currentBone.position);

        // ��]�덷���v�Z
        float rotationError = Quaternion.Angle(referenceBone.rotation, currentBone.rotation);

        // **�F�̕ύX����**
        Color newColor = defaultColor;

        if (positionError > positionThreshold && rotationError > rotationThreshold)
        {
            newColor = Color.red; // �����Ⴄ
        }
        else if (positionError > positionThreshold)
        {
            newColor = Color.blue; // �ʒu���Ⴄ
        }
        else if (rotationError > rotationThreshold)
        {
            newColor = Color.yellow; // ��]���Ⴄ
        }

        // �{�[���̐F�ύX
        if (boneMaterial != null)
        {
            boneMaterial.color = newColor;
        }

        // ���C���̐F�ύX
        if (lineRenderer != null)
        {
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
        }
    }
}
