using System;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.Utility;
using Assets.Scripts.Utils.Coroutine;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using UnityEngine;
using App.Client.GameModules.GamePlay.Free;
using Utils.AssetManager;

namespace Assets.Sources.Free.Effect
{

    public class FreeParticleEffect : FreeBaseEffect, IFreeEffect
    {

        private string _particleName;

        private GameObject currentObj;

        private string particleName
        {
            get { return _particleName; }
            set
            {
                if (value == _particleName)
                    return;
                _particleName = value;

                FreeUrl url = FreeResourceUtil.Convert(_particleName);
                FreePrefabLoader.Load(url.BuddleName, url.AssetName,
                    (sprite) =>
                    {
                        GameObject go = (GameObject)sprite;
                        currentObj = go;
                        currentObj.SetActive(true);
                        go.transform.parent = this.model3D.gameObject.transform;
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localScale = new Vector3(1, 1, 1);
                        go.transform.localEulerAngles = new Vector3(0, 0, 0);
                    });
            }
        }

        public override void Recycle()
        {
            if(currentObj != null)
            {
                FreeUrl url = FreeResourceUtil.Convert(_particleName);
                currentObj.SetActive(false);
                FreePrefabLoader.ReturnGameObject(currentObj, new AssetInfo(url.BuddleName, url.AssetName));
            }
        }

        public FreeParticleEffect()
        {
        }

        public IFreeEffect Clone()
        {
            return new FreeParticleEffect();
        }

        public void Frame(int frameTime, ISceneManage scene, FreeRenderObject freeRender)
        {
            base.AutoValue(frameTime);

            var p = GetAuto("pos");
            if (p != null)
            {
                var obj = p.Frame(frameTime);
                if (obj != null)
                {
                    Vector3 v = (Vector3)obj;
                    obj3D.x = v.x;
                    obj3D.y = v.y;
                    obj3D.z = v.z;
                }
            }
            var s = GetAuto("scale");
            if (s != null)
            {
                object obj = s.Frame(frameTime);
                if(obj != null)
                {
                    Vector3 v = (Vector3)obj;
                    obj3D.scaleX = (float)v.x;
                    obj3D.scaleY = (float)v.y;
                    obj3D.scaleZ = (float)v.z;
                }
            }
        }

        protected override void SetPureValue(string value)
        {
            model3D.resUrl = GetEffectUrl(value);
        }

        public void Initial(params object[] ini)
        {
            var ss = (ini[0] as string).Split("_$$$_");
            if (ss.Length == 1)
            {
                particleName = ss[0];
            }


            InitialAuto(ini[1] as String);

            base.StartAuto();
        }

        public int Type
        {
            get
            {
                return 3;
            }
        }

    }
}
