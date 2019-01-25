using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Shared.SceneManagement
{
    public static class SceneDebugger
    {
        public static void SetTreeDistance(float distance)
        {
            var count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                foreach (var gameObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        terrain.treeDistance = distance;
                    }
                }
            }
        }
        
        public static void SetGrassDensity(float density)
        {
            var count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                foreach (var gameObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        terrain.detailObjectDensity = density;
                    }
                }
            }
        }
        
        public static float GetTreeDistance()
        {
            var count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                foreach (var gameObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        return terrain.treeDistance;
                    }
                }
            }
            
            return -1;
        }

        public static float GetGrassDensity()
        {
            var count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                foreach (var gameObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        return terrain.detailObjectDensity;
                    }
                }
            }
            
            return -1;
        }
    }
}