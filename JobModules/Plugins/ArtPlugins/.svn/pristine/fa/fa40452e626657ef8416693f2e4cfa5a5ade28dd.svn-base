using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtPlugins
{
    public abstract class SceneObjectCollection<T> : MonoBehaviour where T: MonoBehaviour
    {
        public string[] ObjectPathArray;
        public T[] ObjectArray;

        private bool _isCollected = false;

        private void CollectionObjects()
        {
            if (ObjectPathArray != null)
            {
                var objLength = ObjectPathArray.Length;
                if (ObjectArray == null || objLength != ObjectArray.Length)
                {
                    ObjectArray = new T[objLength];
                }

                for (int i = 0; i < objLength; ++i)
                {
                    var obj = FindGameObjectWithPath(ObjectPathArray[i]);
                    if (obj != null)
                    {
//                        if (obj.scene != gameObject.scene)
//                        {
//                            Debug.LogErrorFormat("Game object {0} is not in same scene {1} with sceneobjectcollection scene {2}",
//                                obj.name, obj.scene.name, gameObject.scene.name);
//                            continue;
//                        }
                        ObjectArray[i] = obj.GetComponent<T>();
                    }
                }
            }
        }

        private GameObject FindGameObjectWithPath(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }

            string[] pathList = path.Split('/');
            var pathName = pathList[0];

            var scene = gameObject.scene;
            var rootObjects = scene.GetRootGameObjects();
            int count = rootObjects.Length;
            for (int i = 0; i < count; ++i)
            {
                var rootObject = rootObjects[i].transform;
                if (rootObject.name.Equals(pathName))
                {
                    var child = FindChildGameObjectWithPath(rootObject, pathList, 1);
                    if (child != null)
                        return child.gameObject;
                }
            }

            return null;
        }

        private Transform FindChildGameObjectWithPath(Transform parent, string[] pathList, int startIndex)
        {
            int pathLength = pathList.Length;
            if (startIndex >= pathLength)
            {
                return parent;
            }

            var pathName = pathList[startIndex];
            if (!String.IsNullOrEmpty(pathName))
            {
                var childCount = parent.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    var child = parent.GetChild(i).transform;
                    if (child.name.Equals(pathName))
                    {
                        var go = FindChildGameObjectWithPath(child, pathList, startIndex + 1);
                        if (go != null)
                        {
                            return go;
                        }
                    }
                }
            }

            return null;
        }
        
        public  void ObjectCollection_Data(object array)
        {
            if (!_isCollected)
            {
                CollectionObjects();
                _isCollected = true;
            }

            var objArrayLenth = ObjectArray == null ? 0 : ObjectArray.Length;

            int[] idList = new int[objArrayLenth];
            GameObject[] objList = new GameObject[objArrayLenth];
            for (int i = 0; i < objArrayLenth; i++)
            {
                if (ObjectArray[i] != null)
                {
                    idList[i] = i;
                    objList[i] = ObjectArray[i].gameObject;
                }
            }
            (array as object[])[0] = idList;
            (array as object[])[1] = objList;
        }
    }
}