using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//小物件距离裁剪类 大场景的每个场景放一个
public class LodGroupCell : MonoBehaviour {
    private Camera cmr;
    public int cellSize = 100;
    public int visibleCellCount = 1;
    public List<LODGroup> [,]sceneCells;
    int cellCount;
    HashSet<int> visibleSet;
    Vector2 lastCmrUpdatePos = Vector2.one*100000;
    Rect sceneRect;
    // Use this for initialization
    private void OnEnable()
    {
        var sname= gameObject.scene.name.Split(" ".ToCharArray())[1].Split("x".ToCharArray());
        sceneRect.center = new Vector2(int.Parse(sname[0]) * 1000 - 4000+500, int.Parse(sname[1]) * 1000 - 4000+500);
        sceneRect.size = new Vector2(1000 + cellSize * 2, 1000 + cellSize * 2);
        print(sceneRect);
        visibleSet = new HashSet<int>();
        cmr = Camera.main;
        cellCount = 8000 / cellSize;
        sceneCells = new List<LODGroup>[cellCount,cellCount];
        foreach (GameObject item in this.gameObject.scene.GetRootGameObjects())
        {
            addToCell(item);
        } 
    }

    private void addToCell(GameObject rootItem)
    {
        foreach (var lg in rootItem.GetComponentsInChildren<LODGroup>())
        {
            int x = ((int)lg.transform.position.x + 4000) / cellSize;
            int z = ((int)lg.transform.position.z + 4000) / cellSize;
            if (x < 0 || x >= cellCount || z < 0 || z >= cellCount) {
                lg.gameObject.SetActive(false);
                continue;
            }
            var groupList = sceneCells[x, z];
            if (groupList == null)
            {
                groupList= sceneCells[x, z] = new List<LODGroup>();
            }
            lg.gameObject.SetActive(false);
            groupList.Add(lg);

        }
    }


    // Update is called once per frame
    void Update () {

        var posCmr = new Vector2(cmr.transform.position.x, cmr.transform.position.z);
        if (!sceneRect.Contains(posCmr)) { return; }
        if (Vector2.Distance(posCmr, lastCmrUpdatePos) < cellSize / 4) return;
        lastCmrUpdatePos = posCmr;


        int x = ((int)cmr.transform.position.x + 4000) / cellSize;
        int z = ((int)cmr.transform.position.z + 4000) / cellSize;

        int xMin = Mathf.Max(0, x - visibleCellCount);
        int xMax = Mathf.Min(cellCount, x + visibleCellCount);

        int zMin = Mathf.Max(0, z - visibleCellCount);
        int zMax = Mathf.Min(cellCount, z + visibleCellCount);

        List<int> willRemvoved = new List<int>();
        //remove old
        foreach (int index in visibleSet)
        {
            int indexX = index % 10000;
            int indexZ = index / 10000;
            if (indexX < xMin || indexX > xMax || indexZ < zMin || indexZ > zMax) {
                willRemvoved.Add(index);
            }
        }
        foreach (int index in willRemvoved)
        {
            visibleSet.Remove(index);
            int indexX = index % 10000;
            int indexZ = index / 10000;
            foreach (var item in sceneCells[indexX,indexZ])
            {
                item.gameObject.SetActive(false);
            }

        }
        // addnew
        for (int i = xMin; i < xMax; i++)
        {
            for (int j = zMin; j < zMax; j++)
            {
                if (sceneCells[i, j] == null) continue;
                int index = j * 10000+i;
                if (visibleSet.Contains(index)) continue;
                visibleSet.Add(index);
                foreach (var item in sceneCells[i, j])
                {
                    item.gameObject.SetActive(true);
                }

            }
        }


    }
}
