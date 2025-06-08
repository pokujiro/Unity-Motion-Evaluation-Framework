using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �X�N���v�g���łP�W�O�iy���j��]���Ă���
/// �C���X�y�N�^�[�ōĐ��ʒu�͌��߂�B
/// </summary>

public class MaintainInitialPosition : MonoBehaviour
{
    public Transform tracker; // �g���b�J�[��Transform
    private Vector3 initialTrackerPosition;
    private Quaternion initialTrackerRotation;
    private Vector3 initialObjectPosition;
    private Quaternion initialObjectRotation;
    private bool isInitialized = false;
    private int frameCounter = 0;
    private int initializationDelayFrames = 5; // 5�t���[����ɏ����ʒu���L�^

    [SerializeField] private InputActionReference CaliblationButton; // Stop�{�^������

    void Start()
    {
        initialObjectPosition = transform.position;
        Debug.Log($"�����ʒu{initialObjectPosition}");
    }

    void Update()
    {
        if (!isInitialized)
        {
            frameCounter++;
            if (frameCounter >= initializationDelayFrames && tracker.position != Vector3.zero)
            {
                InitializePosition();
            }
            return;
        }

        if (CaliblationButton.action.WasPressedThisFrame())
        {
            TrackerInitializePosition();
            Debug.Log(" Calliblation ");
        }

        // �P�W�O��]���Ă����W�n�̎��̌����͕ς���Ă��Ȃ��H
        Vector3 targetPosition = tracker.position;
        Vector3 targetDistance = targetPosition - initialTrackerPosition;
        transform.position = new Vector3(targetDistance.x, targetDistance.y, -targetDistance.z) + initialObjectPosition + initialTrackerPosition;

        Quaternion targetRotation = tracker.rotation;

        Quaternion yRotation = Quaternion.Euler(0, 180, 0);

        transform.rotation = yRotation * new Quaternion(targetRotation.x, -targetRotation.y, -targetRotation.z, targetRotation.w);

    }

    void InitializePosition()
    {
        initialTrackerPosition = new Vector3(tracker.position.x, tracker.position.y, -tracker.position.z);
        initialTrackerRotation = tracker.rotation;
        initialObjectPosition = transform.position;
        initialObjectRotation = transform.rotation;
        isInitialized = true;
        Debug.Log($"Initialized after {frameCounter} frames.");
    }

    void TrackerInitializePosition()
    {
        initialTrackerPosition = tracker.position;
        initialTrackerRotation = tracker.rotation;
        isInitialized = true;
        Debug.Log($"Initialized after {frameCounter} frames.");
    }
}


