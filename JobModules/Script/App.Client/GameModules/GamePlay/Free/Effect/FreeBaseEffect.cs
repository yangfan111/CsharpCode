using Assets.Sources.Free.Auto;
using Assets.Sources.Free.Render;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Sources.Free.Effect
{
    public abstract class FreeBaseEffect : BaseAutoFields
    {

        protected IObject3D obj3D;
        protected IEffectModel3D model3D;
        protected EffectFixInfo _fixInfo;
        protected IObject3D oriObj3D;
        private float per;

        public FreeBaseEffect()
        {
            obj3D = new UnityObject3D(new GameObject("Obj3d"));
            oriObj3D = new UnityObject3D(new GameObject("OriObj3d"));
            model3D = new UnityEffectModel3D(obj3D, 0);
            _fixInfo = new EffectFixInfo();
        }

        public virtual void Destroy()
        {
            if(obj3D.GameObject != null)
                UnityEngine.Object.Destroy(obj3D.GameObject);
            if (oriObj3D.GameObject != null)
                UnityEngine.Object.Destroy(oriObj3D.GameObject);
            if(model3D.gameObject != null)
                UnityEngine.Object.Destroy(model3D.gameObject);
        }

        public IEffectModel3D EffectModel3D
        {
            get
            {
                return model3D;
            }
        }

        public virtual void Recycle()
        {

        }

        public void SetPos(float x, float y, float z, float scaleX, float scaleY, float scaleZ, float rotationX, float rotationY, float rotationZ)
        {

            obj3D.x = x;

            obj3D.y = y;

            obj3D.z = z;

            obj3D.scaleX = scaleX;

            obj3D.scaleY = scaleY;

            obj3D.scaleZ = scaleZ;

            obj3D.rotationX = rotationX;

            obj3D.rotationY = rotationY;

            obj3D.rotationZ = rotationZ;


            oriObj3D.x = x;

            oriObj3D.y = y;

            oriObj3D.z = z;

            oriObj3D.scaleX = scaleX;

            oriObj3D.scaleY = scaleY;

            oriObj3D.scaleZ = scaleZ;

            oriObj3D.rotationX = rotationX;

            oriObj3D.rotationY = rotationY;

            oriObj3D.rotationZ = rotationZ;

        }

        public IObject3D OriObject3D
        {
            get
            {
                return oriObj3D;
            }
        }

        public EffectFixInfo FixInfo
        {
            get
            {
                return _fixInfo;
            }


        }

        public void SetValue(params object[] vs)
        {
            var sa = Convert.ToInt32(vs[0]);
            var auto = sa / 100;
            var index = sa % 100;
            if (auto == AUTO_START_NEW || auto == AUTO_START_OLD)
            {

                StartAuto(index);
            }
            else
            {

                StopAuto(index);
            }

            if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW || auto == AUTO_SET)
            {
                var value = vs[1] as string;
                if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW)
                {

                    SetAutoValue(value, index);
                }
                if (auto == AUTO_SET)
                {

                    SetPureValue(value);
                }
            }
        }

        protected abstract void SetPureValue(string v);

        protected override void InitialAutoValue(string field, IAutoValue auto)
        {
            if ("x" == field)
            {
                auto.SetValue(obj3D.x);
            }
            else if ("y" == field)
            {
                auto.SetValue(obj3D.y);
            }
            else if ("z" == field)
            {
                auto.SetValue(obj3D.z);
            }
            else if ("sx" == field)
            {
                //auto.SetValue(obj3D.scaleX);
            }
            else if ("sy" == field)
            {
                //auto.SetValue(obj3D.scaleY);
            }
            else if ("sz" == field)
            {
                //auto.SetValue(obj3D.scaleZ);
            }
            else if ("rx" == field)
            {
                auto.SetValue(obj3D.rotationX);
            }
            else if ("ry" == field)
            {
                auto.SetValue(obj3D.rotationY);
            }
            else if ("rz" == field)
            {
                auto.SetValue(obj3D.rotationZ);
            }
            else if ("r" == field)
            {
                auto.SetValue(((ISprite3D)obj3D).rotation);
            }
            else if ("per" == field)
            {
                auto.SetValue(per);
            }
        }

        protected void UpdateGeometryData(float p)
        {
//            model3D.vertexPostionByteArray.position = 0;
//            model3D.vertexTexcoordByteArray.position = 0;
//            model3D.indexByteArray.position = 0;
//
//            var vertexList = GetRotationProgressVertexList(p);
//            var length = vertexList.Count;
//            var i = 0;
//
//            for (i = 0; i < length; ++i)
//            {
//                model3D.vertexPostionByteArray.writeFloat(vertexList[i].x);
//                model3D.vertexPostionByteArray.writeFloat(vertexList[i].y);
//                model3D.vertexPostionByteArray.writeFloat(vertexList[i].z);
//
//                model3D.vertexTexcoordByteArray.writeFloat(vertexList[i].x);
//                model3D.vertexTexcoordByteArray.writeFloat(vertexList[i].y);
//            }
//
//            for (i = 0; i < length - 2; ++i)
//            {
//                model3D.indexByteArray.writeShort(i + 2);
//                model3D.indexByteArray.writeShort(i + 1);
//                model3D.indexByteArray.writeShort(0);
//            }
        }

        private static IList<Vector3> GetRotationProgressVertexList(float p)
        {
            if (p < 0)
            {
                p = 0;
            }
            else if (p > 1)
            {
                p = 1;
            }

            var vertexList = new List<Vector3>();
            vertexList.Add(new Vector3(0.5f, 0.5f, 0.5f));
            vertexList.Add(new Vector3(0.5f, 0, 0.5f));
            if (p <= 1 / 8)
            {
                vertexList.Add(new Vector3(p * 4 + 0.5f, 0, 0.5f));
            }
            else if (p <= 3 / 8)
            {
                vertexList.Add(new Vector3(1, 0, 0.5f));
                vertexList.Add(new Vector3(1, -(2 / 8 - p) * 4 + 0.5f, 0.5f));
            }
            else if (p <= 5 / 8)
            {
                vertexList.Add(new Vector3(1, 0, 0.5f));
                vertexList.Add(new Vector3(1, 1, 0.5f));
                vertexList.Add(new Vector3((4 / 8 - p) * 4 + 0.5f, 1, 0.5f));
            }
            else if (p <= 7 / 8)
            {
                vertexList.Add(new Vector3(1, 0, 0.5f));
                vertexList.Add(new Vector3(1, 1, 0.5f));
                vertexList.Add(new Vector3(0, 1, 0.5f));
                vertexList.Add(new Vector3(0, -(p - 6 / 8) * 4 + 0.5f, 0.5f));
            }
            else
            {
                vertexList.Add(new Vector3(1, 0, 0.5f));
                vertexList.Add(new Vector3(1, 1, 0.5f));
                vertexList.Add(new Vector3(0, 1, 0.5f));
                vertexList.Add(new Vector3(0, 0, 0.5f));
                vertexList.Add(new Vector3(-(1 - p) * 4 + 0.5f, 0, 0.5f));
            }

            return vertexList;
        }

        protected override void SetAutoValueTo(AutoField auto, int frammeTime)
        {
            if ("x" == auto.field)
            {
                obj3D.x = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("y" == auto.field)
            {
                obj3D.y = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("z" == auto.field)
            {
                obj3D.z = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("sx" == auto.field)
            {
                obj3D.scaleX = Convert.ToSingle(auto.auto.Frame(frammeTime)) / 100;
            }
            else if ("sy" == auto.field)
            {
                obj3D.scaleY = Convert.ToSingle(auto.auto.Frame(frammeTime)) / 100;
            }
            else if ("sz" == auto.field)
            {
                obj3D.scaleZ = Convert.ToSingle(auto.auto.Frame(frammeTime)) / 100;
            }
            else if ("rx" == auto.field)
            {
                obj3D.rotationX = Convert.ToSingle(auto.auto.Frame(frammeTime));
//                obj3D.rotationX = (float)(obj3D.rotationX * Math.PI / 180);
            }
            else if ("ry" == auto.field)
            {
                obj3D.rotationY = Convert.ToSingle(auto.auto.Frame(frammeTime));
//                obj3D.rotationY = (float)(obj3D.rotationY * Math.PI / 180);
            }
            else if ("rz" == auto.field)
            {
                obj3D.rotationZ = Convert.ToSingle(auto.auto.Frame(frammeTime));
//                obj3D.rotationZ = (float)(obj3D.rotationZ * Math.PI / 180);
            }
            else if ("r" == auto.field)
            {

                ((ISprite3D)obj3D).rotation = Convert.ToSingle(auto.auto.Frame(frammeTime));

                ((ISprite3D)obj3D).rotation = (float)(((ISprite3D)obj3D).rotation * Math.PI / 180);
            }
            else if ("vi" == auto.field)
            {
                this.obj3D.visible = bool.TrueString.ToLower() == auto.auto.Frame(frammeTime).ToString();
            }
            else if ("per" == auto.field)
            {
                per = Convert.ToSingle(auto.auto.Frame(frammeTime));

                UpdateGeometryData(per / 1000);
            }
        }

        protected string GetEffectUrl(string url)
        {
            return url;
            //			if(url.indexOf("/effect") == 0){
            //				return ResourceURLGenerator.instance.urlBase + url;
            //			}else if(url.indexOf("common/") >= 0 || 
            //				url.indexOf("noload/") >= 0 ||
            //				url.indexOf("pvecommon/") >= 0 ||
            //				url.indexOf("spary/") >= 0 ||
            //				url.indexOf("halfloadeffect/") >= 0){
            //				
            //				return ResourceURLGenerator.instance.urlBase + "/effect/" + url; 
            //			}
            //			
            //			return ResourceURLGenerator.instance.urlBase + "/effect/" +  
            //				GameModelLocator.getInstance().gameModel.roomData.freeType 
            //				+ "Mode/" + url;
        }
    }
}
