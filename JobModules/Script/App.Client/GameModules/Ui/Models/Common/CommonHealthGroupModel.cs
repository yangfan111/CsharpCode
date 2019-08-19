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
using App.Shared;
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
                        _viewModel.rootRectTransform.anchoredPosition = new UnityEngine.Vector2(9F, 10f);
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
                        _viewModel.rootRectTransform.sizeDelta = new UnityEngine.Vector2(halfScreenWidth - halfGoWidth, 50f);
                    }
                    lastUIMode = adapter.CurUIModel;
                }
            }            
        }

        private void InitHpGroup()
        {
            _viewModel.CurrentHpSliderValue = 0;
            _viewModel.PowerBarImageImage.fillAmount = 0;

            addImgCom = FindChildGo("addImg").GetComponent<RectTransform>();
            reduceImgCom = FindChildGo("reduceImg").GetComponent<RectTransform>();
            addImgCom.offsetMin = new Vector2(0,2);
            addImgCom.offsetMax = new Vector2(0, -2);
            reduceImgCom.offsetMin = new Vector2(0, 2);
            reduceImgCom.offsetMax = new Vector2(0, -2);

            _viewModel.curO2Image.fillAmount = 0;
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
                    _viewModel.HpGroup.SetActive(true);
                    _viewModel.HpGroupInHurt.SetActive(false);
                        
                    if(mayRecoverHp == 0)    //此时只显示当前血量 
                    {
                        float number = _viewModel.CurrentHpSliderValue;
                     

                        float endNumber = (float)curHp / (float)maxHp;
                        if(number < endNumber)
                        {
                            addImgCom.gameObject.SetActive(true);
                            reduceImgCom.gameObject.SetActive(false);

                        }
                        else if(number > endNumber)
                        {
                            addImgCom.gameObject.SetActive(false);
                            reduceImgCom.gameObject.SetActive(true);
                        }
                        else
                        {
                            addImgCom.gameObject.SetActive(false);
                            reduceImgCom.gameObject.SetActive(false);
                        }

                        _viewModel.PercentTextUIText.text = Mathf.RoundToInt(endNumber * 100) + "%";
                        hpTemperTime += interval;
                        if (hpTemperTime >= hpTimeInterval)
                        {
                            if (addDecreaseTween != null)
                                addDecreaseTween.Kill();
                            if (number != endNumber)
                            {
                                hpTemperTime = 0;
                                var startNum = number;
                                var endNum = endNumber;
                                bool add = endNum - startNum > 0;
                                addDecreaseTween = DOTween.To(() => startNum, x => startNum = x, endNum, 0.3f);
                                addDecreaseTween.SetEase(Ease.Linear);
                                addDecreaseTween.OnUpdate(() =>
                                {
                                    _viewModel.CurrentHpSliderValue = startNum;
                                    if(add)
                                    {
                                        addImgCom.anchorMin = new Vector2(startNum, 0f);
                                        addImgCom.anchorMax = new Vector2(endNumber, 1f);
                                    }
                                    else
                                    {
                                        reduceImgCom.anchorMin = new Vector2(endNumber, 0.0f);
                                        reduceImgCom.anchorMax = new Vector2(startNum, 1f);
                                    }
                                    
                                    if (_viewModel.CurrentHpSliderValue < 0.3f)
                                    {
                                        Color c;
                                        ColorUtility.TryParseHtmlString("#f83e1fe6", out c);
                                        _viewModel.currentHpFillImage.color = c;
                                    }
                                    else
                                    {
                                        Color c;
                                        ColorUtility.TryParseHtmlString("#ffffffe6", out c);
                                        _viewModel.currentHpFillImage.color = c;
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
                        _viewModel.HpGroup.SetActive(false);
                        _viewModel.HpGroupInHurt.SetActive(false);

                        _viewModel.CurrentHpSliderValue = 0;
                        _viewModel.HpGroupInHurtFillImage.fillAmount = 0;
                        _viewModel.PercentTextUIText.text = "";
                    }
                    else                  //受伤-非死亡状态
                    {
                        _viewModel.HpGroup.SetActive(false);
                        _viewModel.HpGroupInHurt.SetActive(true);

                        //受伤- 当前血量
                        float startNum = _viewModel.HpGroupInHurtFillImage.fillAmount;
                        float endNumber = (float)curHpInHurted / (float)maxHp;
                        _viewModel.PercentTextUIText.text = Mathf.RoundToInt(endNumber * 100) + "%";
                        _viewModel.CurrentHpSliderValue = 0;
                        Tween t = DOTween.To(() => startNum, x => startNum = x, endNumber, 0.3f);
                        t.OnUpdate(() =>
                        {
                            _viewModel.HpGroupInHurtFillImage.fillAmount = (float)startNum;
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
                if (firstOrThird == 1)
                {
                    _viewModel.ShowPoseGroup.SetActive(true);

                    switch (curPos)
                    {
                        case 1: //站
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_stand", (sprite) =>
                                {
                                    _viewModel.currentPoseImage.sprite = sprite;
                                });
                            }
                            break;
                        case 2:  //蹲
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_squat", (sprite) =>
                                {
                                    _viewModel.currentPoseImage.sprite = sprite;
                                });
                            }
                            break;
                        case 3:  //趴着
                            {
                                Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "pose_fall", (sprite) =>
                                {
                                    _viewModel.currentPoseImage.sprite = sprite;
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
                    _viewModel.ShowPoseGroup.SetActive(false);
            }
        }

        private void RefreshBuffGroup()
        {
            if (adapter != null)
            {
                if (adapter.RecureBufActive == true)
                {
                    Color c;
                    ColorUtility.TryParseHtmlString("#8fc1d8", out c);
                    _viewModel.retreatBuffImage.color = c;
                }
                else
                {
                    Color c;
                    ColorUtility.TryParseHtmlString("#888888", out c);
                    _viewModel.retreatBuffImage.color = c;
                }

                if (adapter.SpeedBufActive == true)
                {
                    Color c;
                    ColorUtility.TryParseHtmlString("#f3b153", out c);
                    _viewModel.speedBuffImage.color = c;
                }
                else
                {
                    Color c;
                    ColorUtility.TryParseHtmlString("#888888", out c);
                    _viewModel.speedBuffImage.color = c;
                }
                 _viewModel.curO2Image.fillAmount = 1 - (adapter.CurO2 / adapter.MaxCurO2);
            }
        }

        private void PreparedPowerGroup()
        {
            
        }

        private void RefreshPowerGroup()
        {
            if (adapter != null)
            {
                float curPower = adapter.CurPower;
                if (curPower != lastPower)
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

        private void UpdateFunc(float curPower)
        {
            _viewModel.PowerBarImageImage.fillAmount = curPower / 100f;

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
                    _viewModel.HeadIconImage.color = Color.white;
                    ShowEquipLevel(_viewModel.HeadIconImage.transform.parent, adapter.HelmetLevel);
                    if (adapter.maxHelmet == adapter.curHelmet)
                    {
                        _viewModel.HelmetImage.fillAmount = 0;
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
                            _viewModel.HelmetImage.fillAmount =   (value / maxHValue);
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
                    _viewModel.HelmetImage.fillAmount = 0;
                    Color c;
                    ColorUtility.TryParseHtmlString("#888888",out c);
                    _viewModel.HeadIconImage.color = c;
                    ShowEquipLevel(_viewModel.HeadIconImage.transform.parent, 0);
                }

                var curBValue = adapter.maxArmor - adapter.curArmor;
                var maxBValue = adapter.maxArmor;
                if (adapter.maxArmor > 0 && adapter.curArmor > 0 && !isDead)
                {
                    _viewModel.ClothIconImage.color = Color.white;
                    ShowEquipLevel(_viewModel.ClothIconImage.transform.parent, adapter.ArmorLevel);
                    if (adapter.maxArmor == adapter.curArmor)
                    {
                        _viewModel.BulletproofImage.fillAmount = 0;
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
                            _viewModel.BulletproofImage.fillAmount =  (value / maxBValue);
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
                    _viewModel.BulletproofImage.fillAmount = 0;
                    Color c;
                    ColorUtility.TryParseHtmlString("#888888", out c);
                    _viewModel.ClothIconImage.color = c;
                    ShowEquipLevel(_viewModel.ClothIconImage.transform.parent, 0);
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
            bool isHX = SharedConfig.IsHXMod;
            if (isHX) return;
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

        private void ShowEquipLevel(Transform trans,int level)
        {
            int childCount = trans.childCount;
            for (int i =0;i<3;i++ )
            {
                var child = trans.GetChild(childCount - (3 - i));
                if (level == i+1)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }
}
