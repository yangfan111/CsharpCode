using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonHealthGroup : ClientAbstractModel, IUiSystem
    {
        private IPlayerStateUiAdapter adapter = null;

        private int lastUIMode = 0;
        private bool isGameObjectCreated = false;
        private List<float> powerConfigList = new List<float>();  //能量条范围 读取配表
        private float lastPower = 0;
        private float curtTweenPower = 0;
        private Tween powerTween = null;

        //血量组变量
        private float currentHpWidth = 0;
        private RectTransform addImgCom = null;
        private RectTransform reduceImgCom = null;
        private float hpTimeInterval = 0.4f;
        private float hpTemperTime = 0;
        private Tween addDecreaseTween = null;


        //头盔和防弹衣
        private float lastHelmet = 0;
        private float lastBulletproof = 0;
        private Tween helmetTween = null;
        private Tween bulletproofTween = null;

        private CommonHealthGroupViewModel _viewModel = new CommonHealthGroupViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonHealthGroup(IPlayerStateUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }


        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            PreparedPowerGroup();
            InitHpGroup();
        }

       public override void Update(float interval)
        {
            if (!isGameObjectCreated) return;
                            
            RefreshLocation();
            RefresHpGroup(interval);
            RefreshPoseGroup();
            RefreshBuffGroup();
            RefreshPowerGroup();
            RefreshEquipGroup();

            RefreshLossBloodEffect();
        }

        //自定义方法
        private void RefreshLocation()
        {
            if (adapter != null && isGameObjectCreated)
            {
                if (lastUIMode != adapter.CurUIModel)
                {
                    if (adapter.CurUIModel == 1) //默认模式      左下角
                    {
                        _viewModel.rootLocation = new UnityEngine.Vector2(9F, 10f);
                    }
                    else                                //可选模式         居中
                    {
                        //float halfGoWidth = _viewModel.rootSizeDelta.x / 2;  目前绑定属性过后 数据都是空 并不是和GameObject相对应 所以改用下面的方法或获取实际的数据
                        Transform FirstGo = FindChildGo("ShowHpGroup");
                        Transform SecondGo = FindChildGo("ShowPoseGroup");
                        float halfGoWidth = 0;
                        if (FirstGo != null && SecondGo != null)
                        {   if (FirstGo.gameObject.GetComponent<RectTransform>())
                            {
                                halfGoWidth = (FirstGo.gameObject.GetComponent<RectTransform>().sizeDelta.x + SecondGo.gameObject.GetComponent<RectTransform>().sizeDelta.x) /2;
                            }
                        }
                        float halfScreenWidth = Screen.width / 2;
                        _viewModel.rootLocation = new UnityEngine.Vector2(halfScreenWidth - halfGoWidth, 50f);
                    }
                    lastUIMode = adapter.CurUIModel;
                }
            }            
        }

        private void InitHpGroup()
        {
            _viewModel.currentHpValue = 0;
            _viewModel.specialHpBgValue = 0.75f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(FindChildGo("root").GetComponent<RectTransform>());
            currentHpWidth = FindChildGo("currentHp").GetComponent<RectTransform>().rect.width;
            addImgCom = FindChildGo("addImg").GetComponent<RectTransform>();
            reduceImgCom = FindChildGo("reduceImg").GetComponent<RectTransform>();
        }
        private void RefresHpGroup(float interval)
        {
            if (adapter != null)
            {
                var curHp = adapter.CurrentHp;
                var maxHp = adapter.MaxHp;
                var mayRecoverHp = adapter.MayRecoverHp;
                var curHpInHurted = adapter.CurrentHpInHurtedState;

                if (!adapter.IsInHurtedState)      //非受伤状态
                {
                    _viewModel.HpGroupGameObjectActiveSelf = true;
                    _viewModel.HpGroupInHurtGameObjectActiveSelf = false;                       
                        
                    if(mayRecoverHp == 0)    //此时只显示当前血量 
                    {
                        float number = _viewModel.currentHpValue;
                        float endNumber = (float)curHp / (float)maxHp;

                        if(number < endNumber)
                        {
                            addImgCom.gameObject.SetActive(true);
                            reduceImgCom.gameObject.SetActive(false);

                            //设置位置
                            addImgCom.anchoredPosition = new Vector2(currentHpWidth * number - 2, 0);
                            //设置长度
                            addImgCom.sizeDelta = new Vector2((currentHpWidth * (endNumber - number)), addImgCom.sizeDelta.y);
                        }
                        else if(number > endNumber)
                        {
                            addImgCom.gameObject.SetActive(false);
                            reduceImgCom.gameObject.SetActive(true);

                            //设置位置
                            reduceImgCom.anchoredPosition = new Vector2(-currentHpWidth * (1 - number) + 2, 0);
                            //设置长度
                            reduceImgCom.sizeDelta = new Vector2((currentHpWidth * (number - endNumber)), addImgCom.sizeDelta.y);
                        }
                        else
                        {
                            addImgCom.gameObject.SetActive(false);
                            reduceImgCom.gameObject.SetActive(false);
                        }


                        hpTemperTime += interval;
                        if (hpTemperTime >= hpTimeInterval)
                        {
                            if (addDecreaseTween == null && number != endNumber)
                            {
                                hpTemperTime = 0;
                                var startNum = number;
                                var endNum = endNumber;
                                addDecreaseTween = DOTween.To(() => startNum, x => startNum = x, endNum, 0.3f);
                                addDecreaseTween.OnUpdate(() =>
                                {
                                    _viewModel.currentHpValue = (float)startNum;
                                    if (_viewModel.currentHpValue < 0.3f)
                                    {
                                        _viewModel.currentHpFillColor = new UnityEngine.Color(237f / 255f, 129f / 255f, 129f / 255f, 1.0f);
                                    }
                                    else
                                    {
                                        _viewModel.currentHpFillColor = new UnityEngine.Color(247f / 255f, 238f / 255f, 201f / 255f, 1.0f);
                                    }
                                });
                                addDecreaseTween.OnComplete(() =>
                                {
                                    addDecreaseTween = null;
                                });
                            }
                        }
                    }
                    else if(mayRecoverHp != 0)
                    {
                        //比如 使用血包啥的 有cd的 还没有做
                    }                                                                   
                }
                else    
                {
                    if (curHpInHurted == 0) //受伤-死亡状态
                    {
                        _viewModel.HpGroupGameObjectActiveSelf = false;
                        _viewModel.HpGroupInHurtGameObjectActiveSelf = false;

                        _viewModel.currentHpValue = 0;
                        _viewModel.HpGroupHurtValue = 0;
                    }
                    else                  //受伤-非死亡状态
                    {
                        _viewModel.HpGroupGameObjectActiveSelf = false;
                        _viewModel.HpGroupInHurtGameObjectActiveSelf = true;

                        //受伤- 当前血量
                        double startNum = _viewModel.HpGroupHurtValue;
                        double endNumber = (float)curHpInHurted / (float)maxHp;
                        Tween t = DOTween.To(() => startNum, x => startNum = x, endNumber, 0.3f);
                        t.OnUpdate(() =>
                        {
                            _viewModel.HpGroupHurtValue = (float)startNum;
                        });                                                                           
                    }
                }
            }
        }
        
        private void RefreshPoseGroup()
        {
            if (adapter != null)
            {
                var curPos = adapter.CurPose;
                var firstOrThird = adapter.FirstOrThirdView;
                if(firstOrThird == 1)
                {
                    _viewModel.ShowPoseGroupGameObjectActiveSelf = true;

                    switch (curPos)
                    {
                        case 1: //站
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_stand", (sprite)=> {
                                    _viewModel.currentPoseImg = sprite;
                                });
                            }
                            break;
                        case 2:  //蹲
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_squat", (sprite) =>
                                {
                                    _viewModel.currentPoseImg = sprite;
                                });
                            }
                            break;
                        case 3:  //趴着
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_fall", (sprite) =>
                                {
                                    _viewModel.currentPoseImg = sprite;
                                });
                            }
                            break;
                        case 4:  //跳
                            {
                                //_viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                            }
                            break;
                        //case 5:  //游泳
                        //    {
                        //        _viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                        //    }
                        //    break;
                        //case 6:  //死亡
                        //    {
                        //        _viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                        //    }
                        //    break;
                        //case 7:  // IsLeanLeft
                        //    {
                        //        _viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                        //    }
                        //    break;
                        //case 8:  //IsLeanRight
                        //    {
                        //        _viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                        //    }
                        //    break;
                        //case 9:  //IsJumpStart
                        //    {
                        //        _viewModel.currentPoseImg = Loader.RetriveSpriteAsync(uiIconsBundleName, "pose_fall");
                        //    }
                        //    break;
                        default:
                            break;
                    }
                }
                else
                    _viewModel.ShowPoseGroupGameObjectActiveSelf = false;
            }
        }

        private void RefreshBuffGroup()
        {
            if (adapter != null)
            {
                if (adapter.RecureBufActive == true)
                {
                    _viewModel.retreatBufActive = true;
                }
                else
                {
                    _viewModel.retreatBufActive = false;
                }

                if (adapter.SpeedBufActive == true)
                {
                    _viewModel.speedBufActive = true;
                }
                else
                {
                    _viewModel.speedBufActive = false;
                }

                if (adapter.CurO2 == adapter.MaxCurO2)
                {
                    _viewModel.o2BufActive = false;
                }
                else
                {
                    _viewModel.o2BufActive = true;
                    _viewModel.curO2FillAmount = 1 - (adapter.CurO2 / adapter.MaxCurO2);
                }
            }
        }

        private void PreparedPowerGroup()
        {
            //读表
            powerConfigList.Add(0f);
            powerConfigList.Add(20f);
            powerConfigList.Add(60f);
            powerConfigList.Add(90f);
            powerConfigList.Add(100f);
            
            Transform duan1 = FindChildGo("duan1");
            Transform duan2 = FindChildGo("duan2");
            Transform duan3 = FindChildGo("duan3");
            Transform duan4 = FindChildGo("duan4");

            if (duan1!= null)
            {
                var slider = duan1.GetComponent<Slider>();
                if (slider)
                {
                    slider.maxValue = powerConfigList[1] / powerConfigList[4];
                    slider.minValue = powerConfigList[0] / powerConfigList[4]; 
                }
            }

            if (duan2 != null)
            {
                var slider = duan2.GetComponent<Slider>();
                if (slider)
                {
                    slider.maxValue = powerConfigList[2] / powerConfigList[4];
                    slider.minValue = powerConfigList[1] / powerConfigList[4]; 
                }
            }

            if (duan3 != null)
            {
                var slider = duan3.GetComponent<Slider>();
                if (slider)
                {
                    slider.maxValue = powerConfigList[3] / powerConfigList[4];
                    slider.minValue = powerConfigList[2] / powerConfigList[4];
                }
            }

            if (duan4 != null)
            {
                var slider = duan4.GetComponent<Slider>();
                if (slider)
                {
                    slider.maxValue = powerConfigList[4] / powerConfigList[4];
                    slider.minValue = powerConfigList[3] / powerConfigList[4];
                }
            }
        }

        private void RefreshPowerGroup()
        {
            if (adapter != null)
            {
                if (powerConfigList.Count < 5)
                    return;
                float curPower = adapter.CurPower;                   
                if (curPower <= 0)   //能量值小于0时候 隐藏能量条
                {
                    _viewModel.PowerGroupActive = false;
                }
                else
                {
                    _viewModel.PowerGroupActive = true;
                    if(curPower != lastPower)
                    {
                        if (powerTween != null)
                        {
                            powerTween.Kill();
                            powerTween = null;
                        }
                        powerTween = UIUtils.CallTween(curtTweenPower, curPower, (value) => { UpdateFunc(value); }, (value) => { CompleteFunc(value); }, 1f);
                        lastPower = curPower;
                    }
                }
            }
        }

        private void UpdateFunc(float curPower)
        {
            if (curPower >= powerConfigList[3])
            {
                _viewModel.duan1 = powerConfigList[1] / powerConfigList[4];
                _viewModel.duan2 = powerConfigList[2] / powerConfigList[4];
                _viewModel.duan3 = powerConfigList[3] / powerConfigList[4];
                _viewModel.duan4 = curPower / powerConfigList[4];
            }
            else if (curPower >= powerConfigList[2] && curPower < powerConfigList[3])
            {
                _viewModel.duan1 = powerConfigList[1] / powerConfigList[4];
                _viewModel.duan2 = powerConfigList[2] / powerConfigList[4];
                _viewModel.duan3 = curPower / powerConfigList[4];
                _viewModel.duan4 = powerConfigList[3] / powerConfigList[4];
            }
            else if (curPower >= powerConfigList[1] && curPower < powerConfigList[2])
            {
                _viewModel.duan1 = powerConfigList[1] / powerConfigList[4];
                _viewModel.duan2 = curPower / powerConfigList[4];
                _viewModel.duan3 = powerConfigList[2] / powerConfigList[4];
                _viewModel.duan4 = powerConfigList[3] / powerConfigList[4]; ;
            }
            else if (curPower < powerConfigList[1])
            {
                _viewModel.duan1 = curPower / powerConfigList[4];
                _viewModel.duan2 = powerConfigList[1] / powerConfigList[4];
                _viewModel.duan3 = powerConfigList[2] / powerConfigList[4];
                _viewModel.duan4 = powerConfigList[3] / powerConfigList[4]; ;
            }

            curtTweenPower = curPower;
        }

        private void CompleteFunc(float curPower)
        {
            powerTween.Kill();
            powerTween = null;
        }

        private void RefreshEquipGroup()
        {
            if (adapter != null)
            {
                var isDead = adapter.IsDead;
                var curHValue = adapter.maxHelmet - adapter.curHelmet;
                var maxHValue = adapter.maxHelmet;
                if (adapter.maxHelmet > 0 && adapter.curHelmet > 0 && !isDead)
                {
                    _viewModel.HelmetActive = true;
                    if (adapter.maxHelmet == adapter.curHelmet)
                    {
                        _viewModel.HelmetFillAmount = 0;
                    }
                    if (lastHelmet != curHValue)
                    {
                        if (helmetTween != null)
                        {
                            helmetTween.Kill();
                            helmetTween = null;
                        }
                        helmetTween = UIUtils.CallTween(lastHelmet, curHValue, (value) => 
                        {
                            _viewModel.HelmetFillAmount = (value / maxHValue);
                        }, 
                        (value) => 
                        {
                            helmetTween.Kill();
                            helmetTween = null;
                        }, 0.5f);
                        lastHelmet = curHValue;
                    }
                }
                else
                {
                    _viewModel.HelmetActive = false;
                }

                var curBValue = adapter.maxArmor - adapter.curArmor;
                var maxBValue = adapter.maxArmor;
                if (adapter.maxArmor > 0 && adapter.curArmor > 0 && !isDead)
                {
                    _viewModel.BulletproofActive = true;
                    if (adapter.maxArmor == adapter.curArmor)
                    {
                        _viewModel.BulletproofFillAmount = 0;
                    }
                    if (lastBulletproof != curBValue)
                    {
                        if (bulletproofTween != null)
                        {
                            bulletproofTween.Kill();
                            bulletproofTween = null;
                        }
                        bulletproofTween = UIUtils.CallTween(lastBulletproof, curBValue, (value) =>
                        {
                            _viewModel.BulletproofFillAmount = (value / maxBValue);
                        },
                        (value) =>
                        {
                            bulletproofTween.Kill();
                            bulletproofTween = null;
                        }, 0.5f);
                        lastBulletproof = curBValue;
                    }                  
                }
                else
                {
                    _viewModel.BulletproofActive = false;
                }
            }
        }

        //掉血特效
        private static int ShowEffectTime = 3000;
        private DateTime _lossBloodTime;
        private int _lastBloodVal;
        private bool _isPlaying;

        private void RefreshLossBloodEffect()
        {
            bool isChange;
            if (adapter.IsInHurtedState)
            {
                isChange = _lastBloodVal > adapter.CurrentHpInHurtedState;
                _lastBloodVal = adapter.CurrentHpInHurtedState;
            }
            else
            {
                isChange = _lastBloodVal > adapter.CurrentHp;
                _lastBloodVal = adapter.CurrentHp;
            }

            if (isChange)
            {
                _lossBloodTime = DateTime.Now;
                //start
                ParticleSystem particle = adapter.MyParticle;
                if (null != particle && !particle.isPlaying)
                {
                    particle.Stop();
                    particle.Play();
                }
                _isPlaying = true;
            }
            else if (_isPlaying)
            {
                //stop
                if ((DateTime.Now - _lossBloodTime).TotalMilliseconds > ShowEffectTime)
                {
                    ParticleSystem particle = adapter.MyParticle;
                    if (null != particle && particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    _isPlaying = false;
                }
            }
        }
    }
}
