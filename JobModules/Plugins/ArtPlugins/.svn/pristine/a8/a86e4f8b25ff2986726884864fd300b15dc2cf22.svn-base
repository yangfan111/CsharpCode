using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTerrainBaseTexture : MonoBehaviour
{
    public Texture2D BaseTexture;
    public Texture2D BaseNormal;
    public float TerrainXmin;
    public float TerrainZmin;
    public float TerrainLength;

    private static bool _isDone = false;

    void Awake()
    {
        if (!_isDone)
        {
            Shader.SetGlobalTexture("_VoyagerTerrainBaseTexture", BaseTexture);
            Shader.SetGlobalFloat("_VoyagerTerrainXMin", TerrainXmin);
            Shader.SetGlobalFloat("_VoyagerTerrainZMin", TerrainZmin);
            Shader.SetGlobalFloat("_VoyagerTerrainLength", TerrainLength);

            _isDone = true;
        }
    }

    void OnDestroy()
    {
        _isDone = false;
    }
}
