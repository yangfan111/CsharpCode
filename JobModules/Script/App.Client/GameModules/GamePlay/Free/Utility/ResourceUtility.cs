using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using App.Client.GameModules.Free;
using Assets.Sources.Components.Asset;
using UnityEngine;
using UnityEngine.UI;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Utils;
using Utils.AssetManager;
using Core.Utils;
using UnityEngine.Rendering;
using Utils.Singleton;

namespace Assets.Sources.Free.Utility
{
    public class ResourceUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ResourceUtility));

        private static Texture2D _transparentTexture;

        public static Texture2D GetTransparentTexture()
        {
            if (_transparentTexture == null)
            {
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                Color fillColor = new Color(1, 1, 1, 0);
                Color[] fillPixels = new Color[tex.width * tex.height];

                for (int i = 0; i < fillPixels.Length; i++)
                {
                    fillPixels[i] = fillColor;
                }

                tex.SetPixels(fillPixels);
                tex.Apply();

                _transparentTexture = tex;
            }
            return _transparentTexture;
        }

        public static IEnumerator LoadImage(RawImage image, string imageUrl, int width = 0, int height = 0)
        {
            FreeGlobalVars.Loader.RetriveSpriteAsync("", "", (sprite) => image.texture = sprite.texture);
            yield break;
        }

        public static IEnumerator LoadNumberFont(Text text, int fontIndex)
        {
            if (text == null)
            {
                yield break;
            }

            FreeGlobalVars.Loader.LoadAsync("number", fontIndex.ToString(), (font) => text.font = (Font)font);
        }

        public static IEnumerator LoadEffectModel(IEffectModel3D effect, string modelUrl)
        {
            //            if (!string.IsNullOrEmpty(modelUrl))
            //            {
            //                modelUrl = modelUrl.Replace(".wmdl", ".prefab");
            //                var assetPath = GetEffectAssetPath(modelUrl);
            //                var loadOperation = AssetPool.Instance.LoadAndAdd<GameObject>(assetPath);
            //                yield return loadOperation.Operation;
            //
            //                if (loadOperation.Result == null)
            //                {
            //                    _logger.ErrorFormat("failed to load effect model {0}", assetPath);
            //                    yield break;
            //                }
            //
            //                var meshObj = GameObject.Instantiate(loadOperation.Result);
            //                meshObj.transform.parent = effect.gameObject.transform;
            //
            //                meshObj.transform.localPosition = Vector3.zero;
            //                meshObj.transform.localScale = new Vector3(1, 1, 1);
            //                meshObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            //
            //                var meshRender = meshObj.GetComponentInChildren<MeshRenderer>();
            //                meshRender.shadowCastingMode = ShadowCastingMode.Off;
            //                effect.meshRender = meshRender;
            //            }
            //            else
            //            {
            //                var go = effect.gameObject;
            //                var spriteObj = new GameObject("Sprite");
            //                spriteObj.transform.parent = go.transform;
            //
            //                spriteObj.transform.localPosition = Vector3.zero;
            //                spriteObj.transform.localScale = new Vector3(1, -1, 1);
            //                spriteObj.transform.localEulerAngles = new Vector3(90, 270, 0);
            //
            //                var spriteRender = spriteObj.AddComponent<SpriteRenderer>();
            //                effect.meshRender = spriteRender;
            //            }
            //
            //            var mat = new Material(Shader.Find("SSJJ/Diffuse"));
            //            if (effect.depthMode == 0 || effect.depthMode == 1)
            //            {
            //                mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);
            //            }
            //            else
            //            {
            //                mat.SetInt("_ZTest", (int)CompareFunction.Always);
            //            }
            //            if (effect.depthMode == 0 || effect.depthMode == 2)
            //            {
            //                mat.SetInt("_ZWrite", 1);
            //            }
            //            else
            //            {
            //                mat.SetInt("_ZWrite", 0);
            //            }
            //            mat.mainTexture = GetTransparentTexture();
            //            mat.renderQueue = RenderQueueConstant.SceneObjectTransparnt;
            //            effect.material = mat;
            yield break;
        }


        public static IEnumerator LoadEffectTexture(IEffectModel3D effect, string imageUrl)
        {
            //            while (effect.meshRender == null)
            //            {
            //                yield return null;
            //            }
            //
            //            imageUrl = imageUrl.Replace(".atf", ".png");
            //            var assetPath = GetEffectAssetPath(imageUrl);
            //
            //            var loadOperation = AssetPool.Instance.LoadAndAdd<Texture2D>(assetPath);
            //            yield return loadOperation.Operation;
            //            var texture = loadOperation.Result;
            //            if (texture == null)
            //            {
            //                _logger.ErrorFormat("Failed to load free effect texture {0} ", assetPath);
            //                yield break;
            //            }
            //
            //            if (effect.meshRender is SpriteRenderer)
            //            {
            //                var width = texture.width;
            //                var height = texture.height;
            //                (effect.meshRender as SpriteRenderer).sprite = Sprite.Create(texture, new Rect(0, 0, width, height),
            //                    new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            //                if (width > height)
            //                {
            //                    var scale = effect.meshRender.transform.localScale;
            //                    scale.x = ((float)(height) / width);
            //                    effect.meshRender.transform.localScale = scale;
            //                }
            //                else
            //                {
            //                    var scale = effect.meshRender.transform.localScale;
            //                    scale.y = -((float)(width) / height);
            //                    effect.meshRender.transform.localScale = scale;
            //                }
            //            }
            //            else
            //            {
            //                var mat = effect.material;
            //
            //                mat.mainTexture = texture;
            //            }
            yield break;
        }

        public static IEnumerator LoadParticle(IEffectModel3D effect, string url)
        {
            //            var assetPath = GetParticleAssetPath(url);
            //
            //            var loadOperation = AssetPool.Instance.LoadAndAdd<GameObject>(assetPath);
            //            yield return loadOperation.Operation;
            //
            //            if (loadOperation.Result == null)
            //            {
            //                _logger.ErrorFormat("failed to load particle {0}", assetPath);
            //                yield break;
            //            }
            //
            //            var go = GameObject.Instantiate(loadOperation.Result);
            //            go.transform.parent = effect.gameObject.transform;
            //
            //            go.transform.localPosition = Vector3.zero;
            //            go.transform.localScale = new Vector3(-1, 1, 1);
            //            go.transform.localEulerAngles = new Vector3(0, 90, 0);
            yield break;
        }


        public static AssetInfo GetUiAssetPath(string url)
        {
            if (url.EndsWith(".atf"))
            {
                url = url.Replace(".atf", ".png");
            }
            if (url.IndexOf("/weapon") == 0)
            {
                var regex = new Regex(@"\/weapon\/(\w+)");
                var match = regex.Match(url);
                var weaponName = match.Groups[1];
                return new AssetInfo(("weapon/" + weaponName).ToLower(), string.Format("Assets/Res/Assets{0}", url));
            }
            if (url.IndexOf("commonfree") == 0)
            {
                return new AssetInfo("ui/commonfree".ToLower(), string.Format("Assets/Res/Assets/ui/GameGUIRes/{0}", url));
            }
            else if (url.IndexOf("common") == 0)
            {
                return new AssetInfo("ui/client/common".ToLower(), string.Format("Assets/Res/Assets/ui/GameGUIRes/{0}", url));
            }
            else if (url.IndexOf("halfLoadedRes") == 0)
            {
                return new AssetInfo("ui/halfLoadedRes".ToLower(), string.Format("Assets/Res/Assets/ui/GameGUIRes/{0}", url));
            }
            else
            {
                return new AssetInfo(string.Format("ui/{0}Model", SingletonManager.Get<UIDataManager>().FreeType).ToLower(),
                    string.Format("Assets/Res/Assets/ui/GameGUIRes/{0}Model/{1}", SingletonManager.Get<UIDataManager>().FreeType, url));
            }
        }

        public static AssetInfo GetEffectAssetPath(string url)
        {
            if (url.IndexOf("commonfree") == 0)
            {
                url = "common" + url.Substring(10);
                return new AssetInfo("effect/common".ToLower(), string.Format("Assets/Res/Assets/effect/{0}", url));
            }
            else if (url.IndexOf("common") == 0)
            {
                return new AssetInfo("effect/common".ToLower(), string.Format("Assets/Res/Assets/effect/{0}", url));
            }
            else if (url.IndexOf("halfLoadedRes") == 0)
            {
                return new AssetInfo("effect/halfLoadedRes".ToLower(),
                    string.Format("Assets/Res/Assets/effect/{0}", url));
            }
            else
            {
                return new AssetInfo(String.Format("effect/{0}Mode", SingletonManager.Get<UIDataManager>().FreeType).ToLower(),
                    string.Format("Assets/Res/Assets/effect/{0}Mode/{1}", SingletonManager.Get<UIDataManager>().FreeType, url));
            }
        }

        public static AssetInfo GetParticleAssetPath(string url)
        {
            var fileName = System.IO.Path.GetFileName(url);
            return new AssetInfo("effect/particle".ToLower(),
                string.Format("Assets/Res/Assets/effect/particle/{0}/{0}.prefab", url));
        }


    }

    public class UIResourceUtility
    {
        private static IDictionary<int, string> modelUrlDic = new Dictionary<int, string>()
        {

        };

        public static AssetInfo GetCommonTextureName(string name)
        {
            return new AssetInfo("ui/common", String.Format("Assets/Res/Assets/ui/GameGUIRes/common/{0}.png", name));
        }

    }

    public class SoundResourceUtility
    {
        private static AssetInfo GetWeaponSoundAssetInfo(string weapon, String suffix)
        {
            return new AssetInfo(string.Format("sound/weapon/{0}", weapon),
                string.Format("Assets/Res/Assets/sound/weapon/{0}{1}.mp3", weapon, suffix));
        }

        public static AssetInfo GetWeaponSoundFireAssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_fire");
        }

        public static AssetInfo GetWeaponSoundSecFireAssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_sec_fire");
        }

        public static AssetInfo GetWeaponSoundFire3AssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_fire3");
        }

        public static AssetInfo GetWeaponSoundChangeAssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_change");
        }

        public static AssetInfo GetWeaponSoundChange2AssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_change2");
        }

        public static AssetInfo GetWeaponSoundReloadAssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_reload");
        }

        public static AssetInfo GetWeaponSoundReload2AssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_reload2");
        }

        public static AssetInfo GetWeaponSoundRaiseAssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_raise");
        }

        public static AssetInfo GetWeaponSoundRaise2AssetInfo(string weapon)
        {
            return GetWeaponSoundAssetInfo(weapon, "_raise2");
        }

        public static AssetInfo GetPlayerStepOnLadder()
        {
            return new AssetInfo("sound/player",
                "Assets/Res/Assets/sound/player/pl_ladder.mp3");
        }

        public static AssetInfo GetPlayerStepDefault()
        {
            return new AssetInfo("sound/player",
                "Assets/Res/Assets/sound/player/pl_step.mp3");
        }

        public static AssetInfo GetPlayerStepCareer(string career)
        {
            return new AssetInfo("sound/player",
                String.Format("Assets/Res/Assets/sound/player/pl_step_{0}.mp3", career));
        }


        public static AssetInfo GetNpcSound(string soundName)
        {
            return new AssetInfo("sound/npcsound",
                String.Format("Assets/Res/Assets/sound/npcsound/{0}.mp3", soundName));
        }

        public static AssetInfo GetSceneSound(string soundName)
        {
            return new AssetInfo("sound/scenesound",
                String.Format("Assets/Res/Assets/sound/scenesound/{0}.mp3", soundName));
        }

        public static AssetInfo GetPveCommonSoundUrl(string soundName)
        {
            return new AssetInfo("sound/pvecommon",
                String.Format("Assets/Res/Assets/sound/pvecommon/{0}.mp3", soundName));
        }

        public static AssetInfo GetCommonSoundUrl(string soundName)
        {
            return new AssetInfo("sound/common",
                String.Format("Assets/Res/Assets/sound/common/{0}.mp3", soundName));
        }

    }
}