using UnityEngine;
using Core.SessionState;
using Core.Utils;
using System;
using Entitas;
using Core.Components;
using App.Shared.Player;
using Utils.Appearance;
using System.Collections.Generic;
using App.Shared.EntityFactory;
using Assets.App.Client.GameModules.Ui;
using Utils.AssetManager;
using App.Client.ThreeEyedGames;
using Assets.Sources.Free.Utility;

namespace App.Client.ClientSystems
{
    public class PlayerSprayPaintUtility
    {
        private static Bounds _bounds;
        private static GameObject _debugGameObject;
        private static GameObject _parent;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSprayPaintUtility));

        private class DecalParam {
            public Contexts contexts;
            public Vector3 position;
            public Vector3 forward;
            public Vector3 head;
        }

        private static void CreateDebugGameObject()
        {
            if (_parent == null) {
                _parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _parent.GetComponent<MeshRenderer>().enabled = false;
                _parent.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public static void CreateBasicDecal(Contexts contexts,
            Vector3 position,
            Vector3 forward, 
            Vector3 head)
        {
            CreateDebugGameObject();
            var assetManager = contexts.session.commonSession.AssetManager;

            string bundleNameSpray = AssetBundleConstant.Prefab_Spray;
            DecalParam decalParam = new DecalParam();
            decalParam.contexts = contexts;
            decalParam.position = position;
            decalParam.forward = forward;
            decalParam.head = head;

            _logger.DebugFormat("CreateBasicDecal");
            assetManager.LoadAssetAsync(decalParam, new AssetInfo(bundleNameSpray, "Decal-Combined"), OnLoadSuccess);
        }

        private static void AngleTrans(ref Vector3 angle, Transform child) {
            _parent.transform.eulerAngles = angle;
            child.SetParent(_parent.transform);
            child.localEulerAngles = new Vector3(-120, -90, 90);
            child.SetParent(null);
            angle = child.eulerAngles;
        }

        private static void OnLoadSuccess(DecalParam arg1, UnityObject arg2)
        {
            _logger.DebugFormat("Decal-Combined OnLoadSuccess !");
            if (arg2 == null || arg2.AsObject == null) {
                _logger.DebugFormat("arg2 == null || arg2.AsObject == null  !");
                return;
            }

            _debugGameObject = GameObject.Instantiate<GameObject>(arg2.AsObject as GameObject);
            Decal decal = _debugGameObject.AddComponent<Decal>();

            _debugGameObject.transform.localScale = new Vector3(1.2f, 4.0f, 1.2f);
            _debugGameObject.transform.position = arg1.position;
            //_debugGameObject.transform.right = arg1.head;
            //_debugGameObject.transform.up = -arg1.forward;
            AngleTrans(ref arg1.head, _debugGameObject.transform);

            if (arg1.contexts == null) {
                return;
            }

            var bundleName = AssetBundleConstant.Icon_Spray;
            var paintIdList = arg1.contexts.ui.uI.PaintIdList;
            var selectedPaintIndex = arg1.contexts.ui.uI.SelectedPaintIndex;
            if (paintIdList.Count <= selectedPaintIndex)
            {
                _logger.ErrorFormat("Give me an error SelectedPaintIndex, Please check it !");
                return;
            }
            var assetName = string.Format("Spray_{0}", paintIdList[selectedPaintIndex]); /*"Spray_3004"*/
            _logger.DebugFormat(assetName);
            var assetManager = arg1.contexts.session.commonSession.AssetManager;
            assetManager.LoadAssetAsync(assetName, new AssetInfo(bundleName, assetName), (source, unityObj) =>
            {
                if (unityObj != null && unityObj.AsObject != null)
                {
                    Texture2D texture = unityObj.AsObject as Texture2D;
                    if (texture != null)
                    {
                        assetManager.LoadAssetAsync(texture, new AssetInfo("shaders", "New Material"), OnMaterialLoadSus);
                    }
                    else
                    {
                        _logger.DebugFormat("LoadAssetAsync failed !");
                        GameObject.Destroy(_debugGameObject);
                    }
                }
                else
                {
                    _logger.DebugFormat("bundleName : " + bundleName + " assetName : " + assetName);
                }
            });
        }

        private static void OnMaterialLoadSus(Texture2D arg1, UnityObject arg2)
        {
            _logger.DebugFormat("OnMaterialLoadSus");
            if (arg1 == null || arg2 == null || arg2.AsObject == null) {
                return;
            }
            Material m = arg2.AsObject as Material;
            Material newMat = GameObject.Instantiate<Material>(m);
            Decal decal = _debugGameObject.GetComponent<Decal>();
            _logger.DebugFormat("decal RenderMode : " + decal.RenderMode);
            _debugGameObject.GetComponent<MeshRenderer>().enabled = true;
            decal.Material = newMat;
            newMat.mainTexture = arg1;
        }

        public static void CreateSprayPaint(Contexts contexts,
            Vector3 vecSize,
            Vector3 position,
            Vector3 forward)
        {
            CreateDebugGameObject();
            _debugGameObject.transform.localScale = vecSize;
            _debugGameObject.transform.position = position;
            _debugGameObject.transform.forward = forward;

            MeshRenderer meshRen = _debugGameObject.GetComponent<MeshRenderer>();
            if (meshRen != null) {
                /*加载mainTexture*/
                var assetManager = contexts.session.commonSession.AssetManager;
                var bundleName = AssetBundleConstant.Icon_Spray;
                var paintIdList = contexts.ui.uI.PaintIdList;
                var selectedPaintIndex = contexts.ui.uI.SelectedPaintIndex;
                if (paintIdList.Count <= selectedPaintIndex)
                {
                    _logger.ErrorFormat("Give me an error SelectedPaintIndex, Please check it !");
                    return;
                }
                var assetName = string.Format("Spray_{0}", paintIdList[selectedPaintIndex]); /*"Spray_3004"*/;
                assetManager.LoadAssetAsync(assetName, new AssetInfo(bundleName, assetName), (source, unityObj) =>
                {
                    Texture2D texture = unityObj.AsObject as Texture2D;
                    if (texture != null)
                    {
                        meshRen.sharedMaterial = new Material(Shader.Find("Unlit/Transparent"));
                        meshRen.sharedMaterial.mainTexture = texture;
                    }
                    _bounds = meshRen.bounds;
                    if (BuildDecal(_debugGameObject, _bounds, texture)) {
                        meshRen.enabled = true;
                    }
                });
            }
        }
        private static bool BuildDecal(GameObject decal, Bounds bounds, Texture2D texture)
        {
            MeshFilter filter = decal.GetComponent<MeshFilter>();
            if (filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
            if (decal.GetComponent<Renderer>() == null) decal.gameObject.AddComponent<MeshRenderer>();
            GameObject[] affectedObjects = GetAffectedObjects(bounds,(1 << LayerMask.NameToLayer("Player"))
                | (1 << LayerMask.NameToLayer("Vehicle"))
               /* | (1 << LayerMask.NameToLayer("Default"))*/
                );
            foreach (GameObject go in affectedObjects)
            {
                DecalBuilder.BuildDecalForObject(decal, go, texture);
            }
            DecalBuilder.Push(0.009f);

            Mesh mesh = DecalBuilder.CreateMesh();
            if (mesh != null) {
                mesh.name = "DecalMesh";
                filter.mesh = mesh;
                return true;
            }
            else { /*创建失败*/
                //if (decal != null) {
                //    GameObject.Destroy(decal);
                //}
            }
            return false;
        }

        private static GameObject[] GetAffectedObjects(Bounds bounds, int filterLayers) {
            MeshFilter[] meshFilters = (MeshFilter[])GameObject.FindObjectsOfType<MeshFilter>();
            List<GameObject> objects = new List<GameObject>();
            foreach (MeshFilter m in meshFilters)
            {
                if (IsLayerContains(filterLayers, 1 << m.gameObject.layer)) continue;
                /*if (r.GetComponent<Decal>() != null) continue;*/
                if (bounds.Intersects(m.sharedMesh.bounds))
                {
                    objects.Add(m.gameObject);
                }
            }
            /*地形系统*/
            TerrainCollider[] terrains = (TerrainCollider[])GameObject.FindObjectsOfType<TerrainCollider>();
            foreach (var t in terrains)
            {
                if (!t.enabled) continue;
                if (bounds.Intersects(t.bounds))
                {
                    objects.Add(t.gameObject);
                }
            }

            return objects.ToArray();
        }

        // 多层级判断
        // 单一层级判断
        protected static bool IsLayerContains(int mask, int layer) {
            return ((mask & layer) > 0);
        }
    }
}