using System;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using UnityEngine;

namespace Assets.Sources.Free.Effect
{
    public class FreeSingleEffect : FreeBaseEffect, IFreeEffect
    {

        public IFreeEffect Clone()
        {
            return new FreeSingleEffect();
        }

        public void Frame(int frameTime, ISceneManage scene, FreeRenderObject freeRender)
        {
            base.AutoValue(frameTime);

            var index = GetAuto("v");
            if (index != null)
            {
                model3D.resUrl = index.Frame(frameTime) as string;
            }

            var p = GetAuto("pos");
            if (p != null)
            {
                var obj = p.Frame(frameTime);
                if(obj != null)
                {
                    Vector3 v = (Vector3)obj;
                    obj3D.x = v.x;
                    obj3D.y = v.y;
                    obj3D.z = v.z;
                }
            }
        }

        protected override void SetPureValue(string value)
        {
            model3D.resUrl = value;
        }

        public void Initial(params object[] ini)
        {
            var ss = (ini[0] as string).Split("_$$$_");
            if (ss.Length >= 9)
            {

                this.obj3D = new UnityObject3D(new GameObject());

                this.model3D = new UnityEffectModel3D(this.obj3D, 0);

                model3D.depthMode = Convert.ToInt32(ss[0]);

                //                EffectUrlParser.Parse(ss[3], model3D);
                model3D.textureName = ss[3];

                string modelName = null;
                if (ss[4].Length > 0 && ss[4] != "null")
                {
                    modelName = ss[4];
                }
                model3D.modelName = modelName;
                model3D.geometryType = Convert.ToInt32(ss[5]);

                model3D.alpha = Convert.ToSingle(ss[6]);
                this.FixInfo.Fixed = ss[7] == "true";
                var fs = (ss[8] as string).Split(",");
                if (fs.Length == 2)
                {
                    this.FixInfo.FixX = Convert.ToSingle(fs[0]);
                    this.FixInfo.FixZ = Convert.ToSingle(fs[1]);
                }

                ((UnityEffectModel3D)model3D).load();

                var index = GetAuto("v");
                if (index != null)
                {
                    index.SetValue(model3D.resUrl);
                }
            }


            InitialAuto(ini[1] as String);

            base.StartAuto();
        }

        public int Type
        {
            get { return 1; }
        }

    }
}
