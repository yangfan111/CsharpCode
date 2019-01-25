using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmbedPrefab : MonoBehaviour
{
#if UNITY_EDITOR
    public string PrefabInAssets;
#else
    void Awake()
    {
        Destroy(this);
    }
#endif
}
