using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static TrackerDataSaver;
using System;

/// <summary>
/// �L�^���ꂽ�A�j���[�V�����f�[�^��JSON�`���ŕۑ�����X�N���v�g
/// </summary>
public class AnimationDataSaver : MonoBehaviour
{
    [Header("Recorder Reference")]
    public AnimationRecorder recorder; // AnimationRecorder�X�N���v�g�����蓖��

    [Header("�ۑ���")]
    public string DirectoryName = "Data";

    [Header("�ǂݍ��݌�")]
    public string ReadDirectoryName = "Data";



    /// <summary>
    /// �L�^�f�[�^��JSON�`���ŕۑ�
    /// </summary>
    public void SaveReferenceData()
    {
        if (recorder == null)
        {
            Debug.LogError("Recorder is not assigned!"); // Recorder���ݒ肳��Ă��Ȃ��ꍇ�̃G���[���O
            return;
        }

        // �L�^���ꂽ�f�[�^���擾
        var data = recorder.GetReferenceRecordedData();
        if (data == null || data.Count == 0)
        {
            Debug.LogError("No data recorded to save!"); // �f�[�^����̏ꍇ�̃G���[���O
            return;
        }

        // JSON�t�@�C���Ƃ��ĕۑ�
        string json = JsonUtility.ToJson(new ListWrapper<AnimationRecorder.BoneData> { bones = data }, true);
        try
        {
            // `Assets/Data/` �f�B���N�g���ɕۑ�
            string directoryPath = Path.Combine(Application.dataPath, DirectoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "referenceAnimationWithTracker.json");
            // �t�@�C������������
            File.WriteAllText(filePath, json);
            Debug.Log($"File saved successfully to: {filePath}");
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError($"Access denied: {e.Message}");
        }
        catch (IOException e)
        {
            Debug.LogError($"I/O error: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"An error occurred: {e.Message}");
        }

        //// �ۑ��̊m�F
        //if (File.Exists(filePath))
        //{
        //    Debug.Log($"Reference data successfully saved to: {filePath}");
        //}
        //else
        //{
        //    Debug.LogError("Failed to save the reference data.");
        //}
    }

    /// <summary>
    /// JSON�`����Bone�f�[�^�����[�h
    /// </summary>
    /// <returns>���[�h���ꂽ�l�X�g���ꂽ�g���b�J�[�f�[�^</returns>
    public List<AnimationRecorder.BoneData> LoadReferenceData()
    {
        string directoryPath = Path.Combine(Application.dataPath, ReadDirectoryName);
        string filePath = Path.Combine(directoryPath, "referenceAnimationWithTracker.json");
        //Debug.Log($"Application.persistentDataPath: {Application.persistentDataPath}");
        //Debug.Log($"Load�@filePath: {filePath}");

        // �t�@�C���̑��݂��m�F
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return null;
        }

        // �t�@�C����ǂݍ���
        string json = File.ReadAllText(filePath);
        Debug.Log($"Loaded JSON: {json}");

        // JSON����łȂ����m�F
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("JSON file is empty.");
            return null;
        }

        var loadedData = JsonUtility.FromJson<ListWrapper<AnimationRecorder.BoneData>>(json).bones;
        if (loadedData != null && loadedData.Count > 0)
        {
            Debug.Log("Reference data (Bone) loaded successfully.");
        }
        else
        {
            Debug.LogError("Loaded reference data (Bone) is empty."); // �f�[�^����̏ꍇ�̃G���[���O
        }

        return loadedData;
    }

    [System.Serializable]
    public class ListWrapper<T>
    {
        public List<T> bones; // �{�[���f�[�^�̃��X�g
    }
}
