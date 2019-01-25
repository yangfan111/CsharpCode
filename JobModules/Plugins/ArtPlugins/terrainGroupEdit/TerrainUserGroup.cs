using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainUserGroup : MonoBehaviour
{

    public Terrain[] Terrains;

    public bool hide = false;

    private bool hided = false;
	// Use this for initialization
	void Start () {
#if UNITY_EDITOR

#else
        Destroy(this);
#endif
    }

    // Update is called once per frame
    void Update () {
        if (hide!=hided)
        {
            hided = hide;
            foreach (var itemTerrain in Terrains)
            {
                itemTerrain.gameObject.SetActive(!hide);
            }
        }
	}
}
