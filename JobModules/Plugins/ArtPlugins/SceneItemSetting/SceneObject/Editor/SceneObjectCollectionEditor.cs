using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UltimateFracturing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtPlugins
{
    public class SceneObjectCollectionEditor : Editor
    {
        public enum ObjectType
        {
            Door,
            DestructibleObject,
            GlassyObject,
        }

        private static readonly string DoorCollectionName = "DoorCollection";
        private static readonly string FracturedObjectCollection = "FracturedObjectCollection";
        private static readonly string FracturedGlassObjectCollection = "FracturedGlassyObjectCollection";
        private static HashSet<string> _objectPathSet = new HashSet<string>();

        private static bool needSave = true;

        [InitializeOnLoadMethod]
        public static void Init()
        {
           // EditorSceneManager.sceneSaved -= OnSceneSaved;
           // EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        [MenuItem("场景物件/收集所有门")]
        public static void getAllDoor()
        {
            GetCollection(ObjectType.Door);
        }

        [MenuItem("场景物件/收集所有可破坏物件")]
        public static void getAllDesructibleObject()
        {
            GetCollection(ObjectType.DestructibleObject);
            GetCollection(ObjectType.GlassyObject);
        }

        private static void GetCollection(ObjectType tp)
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);
                if (!scene.isLoaded || !IsCollectableScene(scene)) continue;
                if (tp == ObjectType.Door)
                    CollectSceneObject<DoorCollection, Door>(scene, DoorCollectionName);
                else if (tp == ObjectType.DestructibleObject)
                    CollectSceneObject<FracturedObjectCollection, FracturedObject>(scene, FracturedObjectCollection, (obj) => obj.GetComponent<Door>() != null);
                else if (tp == ObjectType.GlassyObject)
                    CollectSceneObject<FracturedGlassyObjectCollection, FracturedGlassyObject>(scene, FracturedGlassObjectCollection);
            }
        }

        public static void CollectObjsInScene(Scene scene, ObjectType objType)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogErrorFormat("CollectObjsInScene error, sceneName:{0}", scene.name);
                return;
            }

            if (!IsCollectableScene(scene)) return;

            Debug.LogFormat("CollectObjsInScene:{0} type:{1}", scene.name, objType.ToString());

            if (objType == ObjectType.Door)
            {
                CollectSceneObject<DoorCollection, Door>(scene, DoorCollectionName);
            }
            else if (objType == ObjectType.DestructibleObject)
            {
                CollectSceneObject<FracturedObjectCollection, FracturedObject>(scene, FracturedObjectCollection, (obj) => obj.GetComponent<Door>() != null);
            }
            else if (objType == ObjectType.GlassyObject)
            {
                CollectSceneObject<FracturedGlassyObjectCollection, FracturedGlassyObject>(scene, FracturedGlassObjectCollection);
            }
        }

        private static void OnSceneSaved(Scene scene)
        {
            if (string.IsNullOrEmpty(scene.path) || !IsCollectableScene(scene))
                return;
            if (!needSave)                                      // 防止保存的不间断回调
            {
                needSave = true;
                return;
            }
            needSave = false;
            getAllDoor();
            getAllDesructibleObject();
            EditorSceneManager.SaveScene(scene);
        }

        private static bool IsCollectableScene(Scene scene)
        {
            return scene.path.StartsWith("Assets/Maps/maps/0001/Scenes/002") ||
             scene.path.StartsWith("Assets/ArtSubmit/Level/test/test.unity");
        }

        private static void CollectSceneObject<TCollection, TObject>(Scene scene, string collectionName, Func<TObject, bool> filter = null)
            where TCollection : SceneObjectCollection<TObject>
            where TObject : MonoBehaviour
        {
            var roots = scene.GetRootGameObjects();
            foreach (var item in roots)
            {
                if (item.name.Equals(collectionName) || item.GetComponent<TCollection>() != null)
                {
                    DestroyImmediate(item);
                }
            }

            var collection = new GameObject(collectionName).AddComponent<TCollection>();
            SceneManager.MoveGameObjectToScene(collection.gameObject, scene);

            List<TObject> objList = new List<TObject>();
            foreach (var root in roots)
            {
                if (root != null)
                {
                    var objs = root.GetComponentsInChildren<TObject>();
                    foreach (var obj in objs)
                    {
                        if (filter != null && filter(obj)) continue;

                        objList.Add(obj);
                    }
                }
            }

            //foreach (var obj in GameObject.FindObjectsOfType<TObject>())
            //{
            //    if (obj.gameObject.scene != scene || (filter != null && filter(obj))) continue;
            //    if (objList.Count >= (1 << 20))
            //    {
            //        EditorUtility.DisplayDialog("错误", String.Format("物件个数大于 {0}", 1 << 20), "确定");
            //    }
            //    objList.Add(obj);
            //}

            var objPathList = new List<string>();
            _objectPathSet.Clear();
            foreach (var obj in objList)
            {
                var transform = obj.transform;
                var path = transform.name;

                while (transform.parent != null)
                {
                    transform = transform.parent;
                    path = transform.name + "/" + path;
                }

                if (_objectPathSet.Contains(path))
                {
                    EditorUtility.DisplayDialog("命名重复错误", String.Format("场景中含有相同命名的对象: {0}", path), "确定");
                    continue;
                }

                objPathList.Add(path);
                _objectPathSet.Add(path);
            }

            collection.ObjectPathArray = objPathList.ToArray();
            EditorSceneManager.MarkSceneDirty(scene);
            //EditorSceneManager.SaveScene(scene);
        }
    }

}