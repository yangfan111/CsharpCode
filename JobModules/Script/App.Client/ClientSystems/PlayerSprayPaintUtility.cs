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

namespace App.Client.ClientSystems
{
    public class PlayerSprayPaintUtility
    {
        private static Bounds _bounds;
        private static GameObject _debugGameObject;
        private static void CreateDebugGameObject()
        {
            _debugGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _debugGameObject.GetComponent<MeshRenderer>().enabled = false;
            _debugGameObject.GetComponent<BoxCollider>().enabled = false;
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
                var assetName = "Spray_3004";

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
            GameObject[] affectedObjects = GetAffectedObjects(bounds, LayerMask.NameToLayer("Hitbox"));
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
                if (decal != null) {
                    GameObject.Destroy(decal);
                }
            }
            return false;
        }

        private static GameObject[] GetAffectedObjects(Bounds bounds, int affectedLayers) {
            MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();
            List<GameObject> objects = new List<GameObject>();
            foreach (Renderer r in renderers)
            {
                if (!r.enabled) continue;
                if (!IsLayerContains(affectedLayers, r.gameObject.layer)) continue;
                /*if (r.GetComponent<Decal>() != null) continue;*/
                if (bounds.Intersects(r.bounds))
                {
                    objects.Add(r.gameObject);
                }
            }
            return objects.ToArray();
        }

        // 多层级判断
        // 单一层级判断
        protected static bool IsLayerContains(int mask, int layer) {
            return (mask == layer);
        }
    }
}