#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "LightingBoxRecords", menuName = "LightingBoxRecords", order = 1)]
public class LightingBoxScenesRecord : ScriptableObject
{
    [System.Serializable]
    public class RecordST
    {
        public SceneAsset scene;
        public LB_LightingProfile profile;
    }

    public List<RecordST> records = new List<RecordST>();

    private static LightingBoxScenesRecord _instance;
    public static LightingBoxScenesRecord instance
    {
        get
        {
            // 修复asset配置文件同步时_instance依旧引用旧数据的bug
            // if (_instance == null)
            {
                _instance = Resources.Load("LightingBoxRecords", typeof(LightingBoxScenesRecord)) as LightingBoxScenesRecord;
                if (_instance == null) Debug.LogError("load LightingBoxScenesRecord error");
            }

            return _instance;
        }
    }

    public bool AddRecord(string path, LB_LightingProfile profile)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("AddRecord error, path is null");
            return false;
        }

        SceneAsset sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        if (sa == null)
        {
            Debug.LogError("AddRecord error, sa is null, path:" + path);
            return false;
        }

        RecordST record = new RecordST()
        {
            scene = sa,
            profile = profile,
        };
        records.Add(record);

        return true;
    }

    public bool RemoveRecord(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("RemoveRecord error, path is null");
            return false;
        }

        string guid01 = AssetDatabase.AssetPathToGUID(path);

        for (int i = records.Count - 1; i >= 0; i--)
        {
            RecordST record = records[i];
            if (record != null && record.scene != null)
            {
                string p = AssetDatabase.GetAssetPath(record.scene);
                string guid = AssetDatabase.AssetPathToGUID(p);
                if (guid.Equals(guid01))
                {
                    records.RemoveAt(i);
                    return true;
                }
            }
        }

        return false;
    }

    public LB_LightingProfile GetRecord(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("GetRecord error, path is null");
            return null;
        }

        string guid01 = AssetDatabase.AssetPathToGUID(path);

        foreach (var r in records)
        {
            if (r != null && r.scene != null)
            {
                string p = AssetDatabase.GetAssetPath(r.scene);
                string guid = AssetDatabase.AssetPathToGUID(p);

                if (guid.Equals(guid01)) return r.profile;
            }
        }

        return null;
    }

    public bool UpdateRecord(string path, LB_LightingProfile profile)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("UpdateRecord error, path is null");
            return false;
        }

        string guid01 = AssetDatabase.AssetPathToGUID(path);

        foreach (var r in records)
        {
            if (r != null && r.scene != null)
            {
                string p = AssetDatabase.GetAssetPath(r.scene);
                string guid = AssetDatabase.AssetPathToGUID(p);
                if (guid.Equals(guid01))
                {
                    r.profile = profile;
                    return true;
                }
            }
        }

        Debug.LogWarning("UpdateRecord error, can't find scene record, path:" + path);
        return false;
    }

    public void TryUpdateOrAddRecord(string path, LB_LightingProfile profile)
    {
        if (!UpdateRecord(path, profile)) AddRecord(path, profile);
    }
}
#endif