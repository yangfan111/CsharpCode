using System;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.Utility;
using Assets.Sources.Free.Utility;
using Assets.Scripts.Utils.Coroutine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Sources.Free.Render
{
    public class UnityEffectModel3D : IEffectModel3D
    {
        public IObject3D model3D { get; private set; }
        public float alpha { get; set; }

        public string resUrl
        {
            get { return textureName; }
            set { textureName = value; }
        }

        public int geometryType { get; set; }

        private string _modelName;
        public string modelName
        {
            get { return _modelName; }
            set
            {
                _modelName = value;
            }
        }

        public void load()
        {
            var mat = new Material(Shader.Find("ProBuilder/Diffuse Vertex Color"));
            if (depthMode == 0 || depthMode == 1)
            {
                mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);
            }
            else
            {
                mat.SetInt("_ZTest", (int)CompareFunction.Always);
            }
            if (depthMode == 0 || depthMode == 2)
            {
                mat.SetInt("_ZWrite", 1);
            }
            else
            {
                mat.SetInt("_ZWrite", 0);
            }

            material = mat;

            if (!string.IsNullOrEmpty(_modelName))
            {
                FreeUrl url = FreeResourceUtil.Convert(_modelName);
                FreeGlobalVars.Loader.LoadAsync(url.BuddleName, url.AssetName,
                    (sprite) =>
                    {
                        var meshObj = (GameObject)sprite;
                        meshObj.transform.parent = gameObject.transform;

                        meshObj.transform.localPosition = Vector3.zero;
                        meshObj.transform.localScale = new Vector3(1, 1, 1);
                        meshObj.transform.localEulerAngles = new Vector3(0, 0, 0);

                        var meshRender = meshObj.GetComponentInChildren<MeshRenderer>();
                        meshRender.shadowCastingMode = ShadowCastingMode.Off;
                        this.meshRender = meshRender;
                        this.meshRender.material = mat;

                        url = FreeResourceUtil.Convert(_textureName);
                        FreeGlobalVars.Loader.LoadAsync(url.BuddleName, url.AssetName,
                            (modelTexture) =>
                            {
                                Texture2D texture = (Texture2D)modelTexture;
                                this.meshRender.material.mainTexture = texture;
                            });

                    });
            }
            else
            {
                var go = gameObject;
                var spriteObj = new GameObject("Sprite");
                spriteObj.transform.parent = go.transform;

                spriteObj.transform.localPosition = Vector3.zero;
                spriteObj.transform.localScale = new Vector3(1, -1, 1);
                spriteObj.transform.localEulerAngles = new Vector3(90, 270, 0);

                var spriteRender = spriteObj.AddComponent<SpriteRenderer>();
                meshRender = spriteRender;

                FreeUrl url = FreeResourceUtil.Convert(_textureName);
                FreeGlobalVars.Loader.LoadAsync(url.BuddleName, url.AssetName,
                    (sprite) =>
                    {
                        Texture2D texture = (Texture2D)sprite;
                        var width = texture.width;
                        var height = texture.height;
                        (meshRender as SpriteRenderer).sprite = Sprite.Create(texture, new Rect(0, 0, width, height),
                            new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                        if (width > height)
                        {
                            var scale = meshRender.transform.localScale;
                            scale.x = ((float)(height) / width);
                            meshRender.transform.localScale = scale;
                        }
                        else
                        {
                            var scale = meshRender.transform.localScale;
                            scale.y = -((float)(width) / height);
                            meshRender.transform.localScale = scale;
                        }
                    });
            }
        }

        private string _textureName;

        public string textureName
        {
            get { return _textureName; }
            set
            {
                if (value == _textureName)
                    return;
                _textureName = value;
            }
        }

        public Renderer meshRender { get; set; }

        private Material _material;
        public Material material
        {
            get { return _material; }
            set
            {
                _material = value;
                if (meshRender != null)
                {
                    meshRender.material = material;
                }
            }
        }

        public GameObject gameObject
        {
            get { return model3D.GameObject; }
        }

        public int depthMode { get; set; }

        public UnityEffectModel3D(IObject3D obj, int resId)
        {
            model3D = obj;
        }
    }
}
