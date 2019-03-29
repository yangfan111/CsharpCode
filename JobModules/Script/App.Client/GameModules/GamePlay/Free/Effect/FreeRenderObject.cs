using System;
using System.Collections.Generic;
using Assets.Sources.Free.Auto;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Const;
using Assets.Sources.Free.Utility;
using UnityEngine;
using Object = System.Object;
using Core.Utils;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{

    public class FreeRenderObject : MonoBehaviour, IUIUpdater , IDisposable
    {

        private string _key;

        public string key
        {
            get { return _key; }
            set { _key = value; }
        }

        private IList<AutoField> autos;

        private IShowStyle showStyle;
        private int currentTime;
        private int totalTime;

        private Vector3 _min;
        private Vector3 _max;

        private Vector3 boundMin;
        private Vector3 boundMax;

        public RaderImage raderImage;
        private SceneObjectEntity _raderEntity;
        private SceneObjectEntity _smallMapEntity;

        private IList<IFreeEffect> effects;
        private bool hide;

        public bool needPvs;

        // base property
        public IObject3D model3D;

        private Vector3 vertexColor;

        private IList<IEffectModel3D> effectModel3DList;

        private float scale;

        private Vector3 dis;
        private bool _isDisposed;

        //base property end

        void Awake()
        {
            //        super();
            autos = new List<AutoField>();

            effects = new List<IFreeEffect>();
            boundMin = new Vector3(-10, -10, -10);
            boundMax = new Vector3(10, 10, 10);

            _min = new Vector3();
            _max = new Vector3();

            model3D = new UnityObject3D(new GameObject("FreeRenderObject"));
            effectModel3DList = new List<IEffectModel3D>();
            SingletonManager.Get<SimpleUIUpdater>().Add(this);
        }

        void UpdateSmallMapImage()
        {
            if (string.IsNullOrEmpty(raderImage.img))
                return;
            //            if (_raderEntity == null)
            //            {
            //                _raderEntity = Contexts.sharedInstance.sceneObject.CreateEntity();
            //                _raderEntity.AddComponent(SceneObjectComponentsLookup.SmallMapImage, new SmallMapImageComponent());
            //            }
            //            var comp = _raderEntity.smallMapImage;
            //            comp.Position = new Vector3(model3D.x, model3D.y, model3D.z);
            //            comp.RadarImage = ResourceUtility.GetUiAssetPath(raderImage.img);
            //            comp.SmallMapImage = ResourceUtility.GetUiAssetPath(raderImage.img);
            //            comp.FullImage = ResourceUtility.GetUiAssetPath(raderImage.smallMapFullImg);
            //            comp.Alpha = raderImage.alpha / 100f;
            //            comp.Mask = raderImage.mask;
            //            comp.ScaleX = raderImage.scaleX / 100f;
            //            comp.ScaleY = raderImage.scaleY / 100f;
        }

        void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                for (var index = 0; index < effects.Count; index++)
                {
                    var effect = effects[index];
                    effect.Destroy();
                }

                if(model3D.GameObject != null)
                    Destroy(model3D.GameObject);

                SingletonManager.Get<SimpleUIUpdater>().Remove(this);

                _isDisposed = true;
            }
        }


        //        public RenderObjectSerializer GetSerializer()
        //        {
        //            return _serializer;
        //        }

        private Vector3 globalCullBoundBoxMin = new Vector3();
        private Vector3 globalCullBoundBoxMax = new Vector3();

        //        public bool cullingVisible(IList<CPlane> frustumPlaneList)
        //		{
        //			var lerp = new Vector3D(this.model3D.x, this.model3D.y, this.model3D.z);
        //
        //        Math3D.addVecAToB(boundMin, lerp, globalCullBoundBoxMin);
        //			Math3D.addVecAToB(boundMax, lerp, globalCullBoundBoxMax);
        //			
        //			return Math3DScene.cullingVisible( 15,globalCullBoundBoxMin, globalCullBoundBoxMax, frustumPlaneList );
        //		}


        public void ChangeAutoValue(int index, string value)
        {
            BaseAutoFields.SetValue(autos, index, value);
        }

        public Vector3 GetMin()
        {
            _min.x = boundMin.x + model3D.x;
            _min.y = boundMin.y + model3D.y;
            _min.z = boundMin.z + model3D.z;

            return _min;
        }

        public Vector3 GetMax()
        {
            _max.x = boundMax.x + model3D.x;
            _max.y = boundMax.y + model3D.y;
            _max.z = boundMax.z + model3D.z;

            return _max;
        }

        public bool Visible
        {
            get { return model3D.visible; }
            set { model3D.visible = value; }
        }

        public Vector3 GetVertexColor()
        {
            return vertexColor;
        }

        // 在屏幕中消失,但是不影响小地图
        public bool IsHide
        {
            get { return hide; }
        }

        public bool IsDisabled { get; set; }

        public IList<IFreeEffect> GetEffects()
        {
            return effects;
        }

        public IFreeEffect GetEffect(int index)
        {
            if (index < effects.Count)
            {
                return effects[index];
            }

            return null;
        }

        public void AddEffect(IFreeEffect effect)
        {

            effects.Add(effect);


            model3D.AddChild(effect.EffectModel3D.model3D);

            effectModel3DList.Add(effect.EffectModel3D);

        }

        public void Show(int total)
        {
            totalTime = total;

            currentTime = 0;
        }

        public void Move(float x, float y, float z)
        {

            model3D.x = x;

            model3D.y = y;

            model3D.z = z;

        }

        public void Render(Object centity, ISceneManage scene)
        {

            AutoValue(scene.frameTime);

            for (var index = 0; index < effects.Count; index++)
            {
                var effect = effects[index];
                effect.Frame(scene.frameTime, scene, this);
                if (effect.FixInfo.Fixed)
                {
                    //            var battleModel :BattleModel = GameModelLocator.getInstance().gameModel;
                    //            var playerEntity:PlayerEntity = battleModel.getCurrentSelfPlayerEntity();
                    //            updateScaleV(battleModel.camareOrg, effect.effectModel3D.model3D, 430);
                    //            updateScale(playerEntity.fov, effect.effectModel3D.model3D, effect.oriObject3D);
                    //            updateZ(effect.fixInfo.fixZ, playerEntity.fov, effect.effectModel3D.model3D, effect.oriObject3D);
                    //            updateX(effect.fixInfo.fixX, playerEntity.fov, effect.effectModel3D.model3D, effect.oriObject3D);
                }
            }

            if (showStyle == null)
            {
                showStyle = new ShowSimpleStyle();
            }

            showStyle.ShowEffect(this, currentTime, totalTime);

            currentTime += scene.frameTime;
            //			super.render(centity, scene);
        }

        protected void UpdateX(float scaleZ, int fov, IObject3D model3D, IObject3D ori)
        {
            model3D.x = UpdateScaleXZ(scaleZ, ori.scaleX, fov, model3D, ori.x);
        }

        protected void UpdateZ(float scaleZ, int fov, IObject3D model3D, IObject3D ori)
        {
            model3D.z = UpdateScaleXZ(scaleZ, ori.scaleY, fov, model3D, ori.z);
        }

        protected float UpdateScaleXZ(float scaleZ, float len, int fov, IObject3D model3D, float oldV)
        {
            var deltaZ = scale * scaleZ;

            var r = oldV;

            if (FOVConstant.NORMAL == fov)
            {
                r += deltaZ;
            }
            else if (FOVConstant.AWP_FOV1 == fov)
            {
                r += deltaZ * FOVConstant.AWP_FOV1_SCALE;
            }
            else if (FOVConstant.AWP_FOV2 == fov)
            {
                r += deltaZ * FOVConstant.AWP_FOV2_SCALE;
            }
            else if (FOVConstant.AUG_FOV1 == fov)
            {
                r += deltaZ * FOVConstant.AUG_FOV1_SCALE;
            }

            return r;
        }



        protected void UpdateScale(int fov, IObject3D object3D, IObject3D ori)
        {

            if (FOVConstant.NORMAL == fov)
            {
                object3D.scaleX = scale * ori.scaleX;
                object3D.scaleY = scale * ori.scaleY;
                object3D.scaleZ = scale * ori.scaleZ;
            }
            else if (FOVConstant.AWP_FOV1 == fov)
            {
                object3D.scaleX = scale * ori.scaleX * FOVConstant.AWP_FOV1_SCALE;
                object3D.scaleY = scale * ori.scaleY * FOVConstant.AWP_FOV1_SCALE;
                object3D.scaleZ = scale * ori.scaleZ * FOVConstant.AWP_FOV1_SCALE;
            }
            else if (FOVConstant.AWP_FOV2 == fov)
            {
                object3D.scaleX = scale * ori.scaleX * FOVConstant.AWP_FOV2_SCALE;
                object3D.scaleY = scale * ori.scaleY * FOVConstant.AWP_FOV2_SCALE;
                object3D.scaleZ = scale * ori.scaleZ * FOVConstant.AWP_FOV2_SCALE;
            }
            else if (FOVConstant.AUG_FOV1 == fov)
            {
                object3D.scaleX = scale * ori.scaleX * FOVConstant.AUG_FOV1_SCALE;
                object3D.scaleY = scale * ori.scaleY * FOVConstant.AUG_FOV1_SCALE;
                object3D.scaleZ = scale * ori.scaleZ * FOVConstant.AUG_FOV1_SCALE;
            }
        }

        protected void UpdateScaleV(Vector3 viewOrg, IObject3D pos, int distanceSacle = 200)
        {
            dis.Set(pos.x + model3D.x, pos.y + model3D.y, pos.z + model3D.z);
            dis = dis.Subtract(viewOrg);

            scale = dis.Length() / distanceSacle;
        }

        protected void StartAuto()
        {
            for (var index = 0; index < autos.Count; index++)
            {
                var af = autos[index];
                af.auto.Start();
            }
        }

        protected void StopAuto()
        {
            for (var index = 0; index < autos.Count; index++)
            {
                var af = autos[index];
                af.auto.Stop();
            }
        }

        public IAutoValue GetAuto(string field)
        {
            for (var index = 0; index < autos.Count; index++)
            {
                var af = autos[index];
                if (af.field == field)
                {
                    return af.auto;
                }
            }

            return null;
        }

        public void SetPos(float x, float y, float z, float scaleX, float scaleY, float scaleZ, float rotationX, float rotationY, float rotationZ)
        {

            model3D.x = x;

            model3D.y = y;

            model3D.z = z;

            model3D.scaleX = scaleX;

            model3D.scaleY = scaleY;

            model3D.scaleZ = scaleZ;

            model3D.rotationX = (float)(rotationX * Math.PI / 180);

            model3D.rotationY = (float)(rotationY * Math.PI / 180);

            model3D.rotationZ = (float)(rotationZ * Math.PI / 180);

        }

        public void InitialAuto(string config)
        {
            if (config != null)
            {
                var ss = config.Split("|||");
                for (var i = 0; i < ss.Length; i++)
                {
                    var index = ss[i].IndexOf("=");
                    if (index > 0)
                    {
                        InitialOneAuto(ss[i].Substring(0, index), ss[i].Substring(index + 1));
                    }
                }


                StartAuto();
            }
        }

        private void InitialOneAuto(string field, string config)
        {
            var auto = Auto.AutoValue.Parse(config);


            InitialAutoValue(field, auto);


            autos.Add(new AutoField(field, auto));
        }

        protected void InitialAutoValue(string field, IAutoValue auto)
        {
            if ("x" == field)
            {
                auto.SetValue(model3D.x);
            }
            else if ("y" == field)
            {
                auto.SetValue(model3D.y);
            }
            else if ("z" == field)
            {
                auto.SetValue(model3D.z);
            }
            else if ("sx" == field)
            {
                auto.SetValue(model3D.scaleX);
            }
            else if ("sy" == field)
            {
                auto.SetValue(model3D.scaleY);
            }
            else if ("sz" == field)
            {
                auto.SetValue(model3D.scaleZ);
            }
            else if ("rx" == field)
            {
                auto.SetValue(model3D.rotationX);
            }
            else if ("ry" == field)
            {
                auto.SetValue(model3D.rotationY);
            }
            else if ("rz" == field)
            {
                auto.SetValue(model3D.rotationZ);
            }
            if ("vi" == field)
            {
                auto.SetValue(!hide);
            }
        }

        protected void AutoValue(int frameTime)
        {
            for (var index = 0; index < autos.Count; index++)
            {
                var autoField = autos[index];
                SetAutoValueTo(autoField, frameTime);
            }

            var pos = GetAuto("pos");
            if (pos != null)
            {
                var obj = pos.Frame(frameTime);
                if(obj != null)
                {
                    Vector3 v = (Vector3)obj;
                    model3D.x = v.x;
                    model3D.y = v.y;
                    model3D.z = v.z;
                }
            }
        }

        protected void SetAutoValueTo(AutoField auto, int frammeTime)
        {
            if ("x" == auto.field)
            {
                model3D.x = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("y" == auto.field)
            {
                model3D.y = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("z" == auto.field)
            {
                model3D.z = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("sx" == auto.field)
            {
                model3D.scaleX = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("sy" == auto.field)
            {
                model3D.scaleY = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("sz" == auto.field)
            {
                model3D.scaleZ = Convert.ToSingle(auto.auto.Frame(frammeTime));
            }
            else if ("rx" == auto.field)
            {
                model3D.rotationX = Convert.ToSingle(auto.auto.Frame(frammeTime));
                model3D.rotationX = (float)(model3D.rotationX * Math.PI / 180);
            }
            else if ("ry" == auto.field)
            {
                model3D.rotationY = Convert.ToSingle(auto.auto.Frame(frammeTime));
                model3D.rotationY = (float)(model3D.rotationY * Math.PI / 180);
            }
            else if ("rz" == auto.field)
            {
                model3D.rotationZ = Convert.ToSingle(auto.auto.Frame(frammeTime));
                model3D.rotationZ = (float)(model3D.rotationZ * Math.PI / 180);
            }
            else if ("vi" == auto.field)
            {

                hide = !(Convert.ToBoolean(auto.auto.Frame(frammeTime)));

            }
            else if ("rd-img" == auto.field)
            {

                raderImage.img = Convert.ToString(auto.auto.Frame(frammeTime));

            }
            else if ("rd-sx" == auto.field)
            {

                raderImage.scaleX = Convert.ToSingle(auto.auto.Frame(frammeTime));

            }
            else if ("rd-sy" == auto.field)
            {

                raderImage.scaleY = Convert.ToSingle(auto.auto.Frame(frammeTime));

            }
            else if ("rd-alpha" == auto.field)
            {

                raderImage.alpha = Convert.ToSingle(auto.auto.Frame(frammeTime));

            }
            else if ("rd-mask" == auto.field)
            {

                raderImage.mask = Convert.ToInt32(auto.auto.Frame(frammeTime));

            }
            else if ("rd-fullimg" == auto.field)
            {

                raderImage.smallMapFullImg = Convert.ToString(auto.auto.Frame(frammeTime));

            }
        }

        public void UIUpdate(int frameTime)
        {
            Render(null, SingletonManager.Get<UnitySceneManager>());
            UpdateSmallMapImage();
        }
    }
}
