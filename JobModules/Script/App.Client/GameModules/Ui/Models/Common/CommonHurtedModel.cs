using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using App.Client.GameModules.Ui.Utils;
using Utils.Configuration;
using Assets.XmlConfig;
using App.Client.Utility;
using App.Shared.Components.Ui;
using App.Shared;
using App.Shared.Components.Player;
using System.Linq;

namespace App.Client.GameModules.Ui.Models.Common
{
	public class CommonHurtedModel : ClientAbstractModel, IUiSystem 
    {
        private IHurtedUiAdapter adapter = null;
        private CommonHurtedViewModel _viewModel;
        private bool isGameObjectCreated = false;
        private CrossHairType _lastType = CrossHairType.None;

        //受伤 变量
        private Transform hurtedModel = null;
        private Transform hurtedRoot = null;
        UIUtils.SimplePool pool = null;
        private const float hurtedUnitWidth = 0.28f;        
        private const float hurtedTweenDuring = 5f;
        private Dictionary<int, HurtedTweenData> hurtedRecord = new Dictionary<int, HurtedTweenData>();
        class HurtedTweenData
        {
            int num;
            Tween tween;
            Transform tran;
            CrossHairHurtedData data;

            public HurtedTweenData(int num, Tween tween, Transform tran, CrossHairHurtedData data)
            {
                this.Num = num;
                this.Tween = tween;
                this.tran = tran;
                this.data = data;
            }

            public int Num
            {
                get
                {
                    return num;
                }

                set
                {
                    num = value;
                }
            }

            public Tween Tween
            {
                get
                {
                    return tween;
                }

                set
                {
                    tween = value;
                }
            }

            public Transform Tran
            {
                get
                {
                    return tran;
                }

                set
                {
                    tran = value;
                }
            }

            public CrossHairHurtedData Data
            {
                get
                {
                    return data;
                }

                set
                {
                    data = value;
                }
            }
        }

        public CommonHurtedModel(IHurtedUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
            _viewModel = new CommonHurtedViewModel();         
        }
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
        }
        public override void Update(float interval)
        {
            if(isGameObjectCreated)
                RefreshGui(interval);
        }

        private void InitGui()
        {           
            hurtedModel = FindChildGo("hurtedBg");
            hurtedModel.gameObject.SetActive(false);
            hurtedRoot = FindChildGo("hurted");
            pool = new UIUtils.SimplePool(hurtedModel, hurtedRoot);
        }

        private void RefreshGui(float interval)
        {
            if (adapter != null && isGameObjectCreated)
                RefreshHurted();
        }

        private void RefreshHurted()
        {
            if (adapter.GetPlayerEntity().gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                _viewModel.hurtedRootActive = false;
                foreach (var key in hurtedRecord.Keys)
                {
                    RefreshDisappearHurtedItem(hurtedRecord[key]);
                }
                StopTween();
                adapter.HurtedDataList.Clear();
            }
            else
            {
                _viewModel.hurtedRootActive = true;
                //更新添加的和现有的            
                foreach (var data in adapter.HurtedDataList)
                {
                    if (!hurtedRecord.ContainsKey(data.Key))
                    {
                        var record = new HurtedTweenData(data.Key, null, pool.SpawnGo(), data.Value);
                        hurtedRecord.Add(data.Key, record);
                    }
                    else
                    {
                        var record = hurtedRecord[data.Key];
                        if (record != null)
                        {
                            record.Data.DamageSrcPos = data.Value.DamageSrcPos;
                            record.Data.HurtedAngel = data.Value.HurtedAngel;
                            record.Data.HurtedNum = data.Value.HurtedNum;

                            if (record.Tween != null)
                            {
                                record.Tween.Kill();
                                record.Tween = null;
                            }
                        }
                    }
                    RefreshHurtedItem(hurtedRecord[data.Key]);
                }

                //更新删除的
                foreach (var record in hurtedRecord)
                {
                    //找到是否有消失的 伤害
                    if (!adapter.HurtedDataList.ContainsKey(record.Key))
                    {
                        RefreshDisappearHurtedItem(hurtedRecord[record.Key]);
                    }
                }

                adapter.HurtedDataList.Clear();
            }
        }
        private void RefreshHurtedItem(HurtedTweenData tweenData)
        {
            var go = tweenData.Tran;
            var hurtedData = tweenData.Data;

            //设置go的位置
            go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

            var img = go.GetComponent<Image>();
            //更新宽度
            var temperRate = (hurtedData.HurtedNum / 100);
            temperRate = Mathf.Clamp01(temperRate);
            img.fillAmount = temperRate * hurtedUnitWidth;
            //更新角度
            var adjustedAngel = img.fillAmount * 360; 
            go.localEulerAngles = new Vector3(0, 0, hurtedData.HurtedAngel + adjustedAngel/2);

            //更新小箭头方向
            var jiantou = go.Find("jiantou"); 
            if(jiantou)
            {
                jiantou.localEulerAngles = new Vector3(0, 0, -adjustedAngel / 2);
            }
        }
        private void RefreshDisappearHurtedItem(HurtedTweenData record)
        {
            if (record.Tween == null)
            {
                //新建一个tween
                record.Tween = UIUtils.CallTween(1, 0, (value) =>
                {
                    var image = record.Tran.GetComponent<Image>();
                    if (image)
                        image.color = new Color(image.color.r, image.color.g, image.color.b, value);
                        //根据摄像机朝向 设置当前角度
                        var selfPlayer = adapter.GetPlayerEntity();
                        //摄像机朝向
                        var forword = selfPlayer.cameraObj.MainCamera.transform.forward;
                        var forwordxz = new Vector2(forword.x, forword.z);
                        // Debug.Log("camera:" + forwordxz.ToString());

                        //我的朝向
                        var myPos = selfPlayer.position.Value;
                        //攻击源方向
                        var damageSrcPos = new Vector3(record.Data.DamageSrcPos.x, 0, record.Data.DamageSrcPos.y);
                        var dir = damageSrcPos - myPos;
                        var dirxz = new Vector2(dir.x, dir.z);
                        // Debug.Log("source:" + dirxz.ToString());

                        var cross = dirxz.x * forwordxz.y - dirxz.y * forwordxz.x;
                        var angle = Vector2.Angle(dirxz, forwordxz) * -Mathf.Sign(cross);
                        // Debug.Log("angle:" + angle.ToString());

                        var hurtedData = record.Data;
                        var go = record.Tran;
                        var temRate = (hurtedData.HurtedNum / 100);
                        temRate = Mathf.Clamp01(temRate);
                        image.fillAmount = temRate * hurtedUnitWidth;
                        var adjustedAngel = image.fillAmount * 360;
                        go.localEulerAngles = new Vector3(0, 0, angle + adjustedAngel / 2);
                        // Debug.Log("tt:" + go.localEulerAngles.z);
                },
                (value) =>
                {
                    pool.DespawnGo(record.Tran);
                    record.Tran = null;
                    record.Tween = null;
                    record.Data = null;
                    hurtedRecord.Remove(record.Num);
                },
                hurtedTweenDuring);
            }
        }

        public override void Destory()
        {
            base.Destory();
            StopTween();
        }

        void StopTween()
        {
            foreach (var key in hurtedRecord.Keys.ToList())
            {
                var item = hurtedRecord[key];
                if (item.Tween != null)
                {
                    item.Tween.Kill(true);
                    item.Tween = null;
                }
            }
        }
    }
}
