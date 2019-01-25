using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ArtPlugins {
    [RequireComponent(typeof(Terrain))]
    public class TerrainProxy : MonoBehaviour
    {
        static Dictionary<int, Terrain> idTerrains = new Dictionary<int, Terrain>();
        Terrain terrain;
        private void Start()
        {
            terrain= GetComponent<Terrain>();
            terrain.drawTreesAndFoliage = true;
            terrain.heightmapPixelError = 20;
            terrain.collectDetailPatches =true;
            terrain.detailObjectDistance =120;
            terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

          //  Stopwatch stopwatch=new Stopwatch();
          //  stopwatch.Start();
            int index = TerrainItemInstanceTools.getTerrainIndex(terrain.terrainData);
            idTerrains.Add(index, terrain);
            updateNeighbors(terrain);
            foreach (var key in idTerrains.Keys)
            {
                var dis = Mathf.Abs(key - index);
                if (dis == 1 || dis == 8)updateNeighbors(idTerrains[key]);
            }
           // stopwatch.Stop();
         //   print("updateNeighbors time:" + stopwatch.ElapsedMilliseconds);

        }
        public static void updateNeighbors(Terrain _terrain) {
            int index = TerrainItemInstanceTools.getTerrainIndex(_terrain.terrainData);
           
                      Terrain left = null, right = null, top = null, bottom = null;

            if (index % 8 != 0) left = getTerrain(index - 1);
            if (index % 8 != 7) right = getTerrain(index + 1);
            if (index / 8 != 0) bottom = getTerrain(index - 8);
            if (index / 8 != 7) top = getTerrain(index + 8);
            _terrain.SetNeighbors(left, top, right, bottom);
        }
        private static Terrain getTerrain(int index) {
            Terrain data = null;
            idTerrains.TryGetValue(index, out data);
            return data;
        }
        private void OnDestroy()
        {
            idTerrains.Remove(TerrainItemInstanceTools.getTerrainIndex(terrain.terrainData));

        }
    }
}
