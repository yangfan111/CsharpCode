﻿using UnityEngine;
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
using App.Client.ClientSystems;

namespace App.Client.ClientSystems
{
    public class PlayerSprayPaintUtility
    {
        private static Bounds _bounds;
        private static GameObject _debugGameObject;
        private static GameObject _parent;
        private static GameObject _decal;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSprayPaintUtility));

        private class DecalParam {
            public ClientEffectEntity entity;
            public Contexts contexts;
            public Vector3 position;
            public Vector3 forward;
            public Vector3 head;
            public AssetInfo assetInfo;
            public int sprayPrintSpriteId;
        }

        private class DecalVolumeParam {
            public MeshRenderer meshRen;
            public Texture2D texture;
        }

        public static void CreateDecalVolume(ClientEffectEntity entity,
            Contexts contexts,
            Vector3 position,
            Vector3 forward,
            Vector3 head,
            int sprayPrintSpriteId) {
            CreateDebugGameObject();
            _decal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshRenderer meshRen = _decal.GetComponent<MeshRenderer>();
            meshRen.enabled = false;
            _decal.GetComponent<BoxCollider>().enabled = false;
            _decal.transform.localScale = Vector3.one;

            DecalVolume dv = _decal.AddComponent<DecalVolume>();
            dv.Volume.m_origin = Vector3.zero;
            dv.Volume.m_size = new Vector3(1.2f, 1.2f, 1.2f);

            _decal.transform.position = position;
            _decal.transform.Rotate(head, Space.Self);
             var bundleName = AssetBundleConstant.Icon_Spray;
            var assetName = string.Format("Spray_{0}", sprayPrintSpriteId);
            _logger.DebugFormat(assetName);
            var assetManager = contexts.session.commonSession.AssetManager;

            AssetInfo assetInfo = new AssetInfo(bundleName, assetName);
            assetManager.LoadAssetAsync(assetName, assetInfo, (source, unityObj) =>
            {
                DecalVolumeParam param = new DecalVolumeParam();
                param.meshRen = meshRen;
                if (unityObj != null && unityObj.AsObject != null)
                {
                    if (unityObj.AsObject is Texture2D)
                    {
                        param.texture = unityObj.AsObject as Texture2D;
                    }
                    else if (unityObj.AsObject is Sprite)
                    {
                        Sprite sprite = unityObj.AsObject as Sprite;
                        param.texture = sprite.texture;
                    }
                    else {
                        _logger.DebugFormat("unityObj.AsObject is null !");
                        return;
                    }
                    assetManager.LoadAssetAsync(param, new AssetInfo("shaders", "MaterialForDecal"), OnDecalMaterialLoadSus);
                    dv.Create(forward, 0);
                }
                else
                {
                    _logger.DebugFormat("bundleName : " + bundleName + " assetName : " + assetName);
                }
            });
            entity.assets.LoadedAssets.Add(assetInfo, new UnityObject(_decal, assetInfo));
        }

        private static void OnDecalMaterialLoadSus(DecalVolumeParam arg1, UnityObject arg2)
        {
            _logger.DebugFormat("OnDecalMaterialLoadSus");
            if (arg1 == null || arg2 == null || arg2.AsObject == null)
            {
                _logger.DebugFormat("OnDecalMaterialLoad failed !");
                return;
            }
            Material m = arg2.AsObject as Material;
            Material newMat = GameObject.Instantiate<Material>(m);
            arg1.meshRen.sharedMaterial = newMat;
            arg1.meshRen.sharedMaterial.mainTexture = arg1.texture;
            arg1.meshRen.enabled = true;
        }

        private static void CreateDebugGameObject()
        {
            if (_parent == null) {
                _parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _parent.GetComponent<MeshRenderer>().enabled = false;
                _parent.GetComponent<BoxCollider>().enabled = false;
            }
        }


        public static void CreateBasicDecal(ClientEffectEntity entity,
            Contexts contexts,
            Vector3 position,
            Vector3 forward, 
            Vector3 head, 
            int sprayPrintSpriteId)
        {
            CreateDebugGameObject();
            var assetManager = contexts.session.commonSession.AssetManager;

            string bundleNameSpray = AssetBundleConstant.Prefab_Spray;
            DecalParam decalParam = new DecalParam();
            AssetInfo assetInfo = new AssetInfo(bundleNameSpray, "Decal-Combined");
            decalParam.entity = entity;
            decalParam.contexts = contexts;
            decalParam.position = position;
            decalParam.forward = forward;
            decalParam.head = head;
            decalParam.sprayPrintSpriteId = sprayPrintSpriteId;
            decalParam.assetInfo = assetInfo;
            _logger.DebugFormat("CreateBasicDecal");
            assetManager.LoadAssetAsync(decalParam, assetInfo, OnLoadSuccess);
        }

        private static void AngleTrans(ref Vector3 angle, Transform child) {
            _parent.transform.eulerAngles = angle;
            child.SetParent(_parent.transform);
            child.localEulerAngles = new Vector3(-90, 0, 0);
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

            _debugGameObject = arg2.AsObject as GameObject;
            arg1.entity.assets.LoadedAssets.Add(arg1.assetInfo, arg2);
            Decal decal = _debugGameObject.AddComponent<Decal>();
            _debugGameObject.transform.localEulerAngles = Vector3.zero;
            Vector3 forward = arg1.forward;
            float multiplyingpower = forward.y / Math.Max(Math.Max(forward.x, forward.z), 0.01f);
            bool ground = (multiplyingpower > 0.577f);

            arg1.head.x = FreeMathUtility.yRoundx(forward);
            if (ground)
            {
                decal.transform.position = arg1.position;
            }
            else
            {
                arg1.head.y = 0;
                arg1.head.z = FreeMathUtility.yRoundz(forward);
                decal.transform.position = arg1.position - forward * 0.3f;
            }
            _debugGameObject.transform.localScale = new Vector3(0.75f, ground ? 0.06f : 0.64f, 0.75f);
            decal.transform.Rotate(arg1.head, Space.World);
            float result = Vector3.Dot(forward, decal.transform.forward);
            if (Math.Abs(result) > 0.1f) {
                Vector3 localEulerAngles = decal.transform.localEulerAngles;
                localEulerAngles.x = -localEulerAngles.x;
                localEulerAngles.z = -localEulerAngles.z;
                decal.transform.localEulerAngles = localEulerAngles;
            }

            if (arg1.contexts == null) {
                return;
            }
            var bundleName = AssetBundleConstant.Icon_Spray;
            var assetName = string.Format("Spray_{0}", arg1.sprayPrintSpriteId);
            _logger.DebugFormat(assetName);
            var assetManager = arg1.contexts.session.commonSession.AssetManager;
            assetManager.LoadAssetAsync(assetName, new AssetInfo(bundleName, assetName), (source, unityObj) =>
            {
                if (unityObj != null && unityObj.AsObject != null)
                {
                    if (unityObj.AsObject is Texture2D) {
                        Texture2D texture = unityObj.AsObject as Texture2D;
                        assetManager.LoadAssetAsync(texture, new AssetInfo("shaders", "New Material"), OnMaterialLoadSus);
                    } else if (unityObj.AsObject is Sprite) {
                        Sprite sprite = unityObj.AsObject as Sprite;
#if UNITY_EDITOR
                        Material m = new Material(Shader.Find("Decalicious/Deferred Spray"));
                        decal.Material = m;
                        m.mainTexture = sprite.texture;
                        _debugGameObject.GetComponent<MeshRenderer>().enabled = true;
#else
                        assetManager.LoadAssetAsync(sprite.texture, new AssetInfo("shaders", "New Material"), OnMaterialLoadSus);
#endif
                    } else {
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
            /*CreateDebugGameObject();*/
            _debugGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _debugGameObject.GetComponent<MeshRenderer>().enabled = false;
            _debugGameObject.GetComponent<BoxCollider>().enabled = false;
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