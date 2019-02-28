using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using App.Client.GameModules.Ui.Utils;
using Utils.Configuration;
using Assets.XmlConfig;
using App.Client.Utility;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{
	public class CommonCrossHairModel : ClientAbstractModel, IUiSystem 
    {
        private ICrossHairUiAdapter adapter = null;
        private CommonCrossHairViewModel _viewModel;
        private bool isGameObjectCreated = false;
        private CrossHairType _lastType = CrossHairType.None;

        //准心 变量
        private float normalTypeStartPos = 0;   //准心的每个刻度的 起始位置
        private float normalTypeLineWH = 0;     //准心的每个刻度的 长宽 
        private float normalTypeEndPos = 0;
        private Transform sTop = null;
        private RectTransform sTopRt = null;
        private Image sTopImg = null;
        private Transform sDown = null;
        private RectTransform sDownRt = null;
        private Image sDownImg = null;
        private Transform hLeft = null;
        private RectTransform hLeftRt = null;
        private Image hLeftImg = null;
        private Transform hRight = null;
        private RectTransform hRightRt = null;
        private Image hRightImg = null;
        private Transform center = null;
        private RectTransform centerRt = null;
        private Image centerImg = null;


        private Tween moveTween = null;
        private Tween shootTween = null;
        private float lastShootNum = 0;
        private int lastWeaponAvatarId = -1;

        //伤害 变量
        private Tween disAppearRedTween = null;
        private Tween disAppearWhiteTween = null;
        private float disAppearTweenTime = 1f;
        private int rateLow = 1;     //x图片的最小放大倍数
        private int rateHeight = 2;  //x图片的最大放大倍数
        private int attackUnit = 25; //一倍的 对应伤害值
        private float lastAttackNum = 0;

        public CommonCrossHairModel(ICrossHairUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
            _viewModel = new CommonCrossHairViewModel();
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
            sTop = FindChildGo("sTop");
            sTopRt = FindComponent<RectTransform>("sTop");
            sTopImg = FindComponent<Image>("sTop");

            sDown = FindChildGo("sDown");
            sDownRt = FindComponent<RectTransform>("sDown");
            sDownImg = FindComponent<Image>("sDown");

            hLeft = FindChildGo("hLeft");
            hLeftRt = FindComponent<RectTransform>("hLeft");
            hLeftImg = FindComponent<Image>("hLeft");

            hRight = FindChildGo("hRight");
            hRightRt = FindComponent<RectTransform>("hRight");
            hRightImg = FindComponent<Image>("hRight");

            center = FindChildGo("center");
            centerRt = FindComponent<RectTransform>("center");
            centerImg = FindComponent<Image>("center");

            normalTypeStartPos = sTopRt.localPosition.y;
            normalTypeLineWH = sTopRt.sizeDelta.y;
            normalTypeEndPos = normalTypeStartPos + 0.7f * normalTypeLineWH;

            _viewModel.ImageWhiteActive = false;
            _viewModel.ImageRedActive = false;
        }

        private void RefreshGui(float interval)
        {
            if (adapter != null && isGameObjectCreated)
            {
                SetCrossAttackActive(adapter.IsShowCrossHair);
                if (adapter.IsShowCrossHair)
                {
                    RefreshCrossHair(interval);
                    RefreshAttack();
                }
            }
        }

        private void RefreshCrossHair(float interval)
        {
            SelectTypePanel(adapter.Type);
            switch (adapter.Type)
            {
                case CrossHairType.Normal:
                    {
                        SetNormalCrossType(adapter.WeaponAvatarId);  //根据weapon不同过得subtype 设置不同的准心图片
                        CleanTween(adapter.Statue);
                        switch (adapter.Statue)
                        {
                            case CrossHairNormalTypeStatue.Move:
                                {
                                    if (moveTween == null)  //想外移动 0.5秒内到达 endPos
                                    {
                                        float startPos = sTopRt.localPosition.y;
                                        float endPos = normalTypeEndPos;
                                        moveTween = UIUtils.CallTween(startPos, endPos, UpdateNormalLinePos, (value) => { moveTween = null; }, 0.5f);
                                    }
                                }
                                break;
                            case CrossHairNormalTypeStatue.Shot:    //每颗子弹会使准星向外移动1像素，最大移动至自身长度*3的距离（10像素的线段会移动20像素）。
                                {
                                    if (lastShootNum != adapter.ShootNum)
                                    {
                                        float startPos = sTopRt.localPosition.y;
                                        float temperPos = startPos + (adapter.ShootNum - lastShootNum) * 1;
                                        UpdateNormalLinePos(temperPos);
                                        lastShootNum = adapter.ShootNum;
                                    }
                                }
                                break;
                            case CrossHairNormalTypeStatue.StopShot:    //0.3秒内，十字的每一条线段，会向内移动（准星中心点方向）移动到默认位置
                                {
                                    if (shootTween == null)
                                    {
                                        float startPos = sTopRt.localPosition.y;
                                        float endPos = normalTypeStartPos;
                                        shootTween = UIUtils.CallTween(startPos, endPos, UpdateNormalLinePos, (value) => { shootTween = null; }, 0.3f);
                                    }
                                }
                                break;
                        }
                    }
                    break;               
                case CrossHairType.AddBlood:
                    {

                    }
                    break;
                case CrossHairType.Novisible:
                    {

                    }
                    break;
            }
            _lastType = adapter.Type;
        }
        private void RefreshAttack()
        {
            var number = adapter.AttackNum;
            if (number > 0) 
            {
                if (disAppearRedTween != null)
                {
                    disAppearRedTween.Kill();
                    disAppearRedTween = null;
                    _viewModel.ImageRedColor = Color.white;
                }

                if(disAppearWhiteTween != null)
                {
                    disAppearWhiteTween.Kill();
                    disAppearWhiteTween = null;
                    _viewModel.ImageWhiteColor = Color.white;
                }

                float imgSize = 0;
                if (number >= 75)
                    imgSize = 100;
                else if (number >= 50 && number < 75)
                    imgSize = 50 * 1.7f;
                else if (number >= 25 && number < 50)
                    imgSize = 50 * 1.4f;
                else if (number >= 0 && number < 25)
                    imgSize = 50;

                if (adapter.IsBurstHeart)  //爆头
                {
                    _viewModel.ImageWhiteActive = false;
                    _viewModel.ImageRedActive = true;

                    //更新大小
                    if (lastAttackNum != number)
                        _viewModel.ImageRedSize = new Vector2(imgSize, imgSize);
                }
                else
                {
                    _viewModel.ImageWhiteActive = true;
                    _viewModel.ImageRedActive = false;

                    //更新大小
                    if (lastAttackNum != number)
                        _viewModel.ImageWhiteSize = new Vector2(imgSize, imgSize);
                }

                lastAttackNum = number;
            }
            else
            {
                _viewModel.ImageWhiteActive = false;
                _viewModel.ImageRedActive = false;

                //淡出
                if (_viewModel.ImageRedActive && disAppearRedTween == null)  
                {
                    _viewModel.ImageRedColor = Color.white;
                    disAppearRedTween = UIUtils.CallTween(1, 0,
                        (value) =>
                        {
                            _viewModel.ImageRedColor = new Color(_viewModel.ImageRedColor.r, _viewModel.ImageRedColor.g, _viewModel.ImageRedColor.b, value);
                        },
                        (value) =>
                        {
                            disAppearRedTween = null;
                            _viewModel.ImageRedColor = Color.white;
                            _viewModel.ImageRedActive = false;
                        },
                        disAppearTweenTime);
                }
                else if(_viewModel.ImageWhiteActive && disAppearWhiteTween == null)
                {
                    _viewModel.ImageWhiteColor = Color.white;
                    disAppearWhiteTween = UIUtils.CallTween(1, 0,
                        (value) =>
                        {
                            _viewModel.ImageWhiteColor = new Color(_viewModel.ImageWhiteColor.r, _viewModel.ImageWhiteColor.g, _viewModel.ImageWhiteColor.b, value);
                        },
                        (value) =>
                        {
                            disAppearWhiteTween = null;
                            _viewModel.ImageWhiteColor = Color.white;
                            _viewModel.ImageWhiteActive = false;
                        },
                        disAppearTweenTime);
                }                       
            }
        }

        #region detail
        private void SelectTypePanel(CrossHairType type)
        {
            if (type != _lastType)
            {
                _viewModel.normalActive = false;
                _viewModel.addBloodActive = false;
                _viewModel.countDownActive = false;
                _viewModel.noVisibleActive = false;

                switch (adapter.Type)
                {
                    case CrossHairType.Normal:
                        {
                            _viewModel.normalActive = true;
                        }
                        break;
                    case CrossHairType.CountDown:
                        {
                            _viewModel.countDownActive = true;
                        }
                        break;
                    case CrossHairType.AddBlood:
                        {
                            _viewModel.addBloodActive = true;
                        }
                        break;
                    case CrossHairType.Novisible:
                        {
                            _viewModel.noVisibleActive = true;
                        }
                        break;
                    case CrossHairType.None:
                        {

                        }
                        break;
                }
            }
        }     
        private void CleanTween(CrossHairNormalTypeStatue type)
        {            
            if (type == CrossHairNormalTypeStatue.Move)
            {               
                if (shootTween != null)
                {
                    shootTween.Kill();
                    shootTween = null;
                }
            }
            else if (type == CrossHairNormalTypeStatue.StopShot)
            {
                if (moveTween != null)
                {
                    moveTween.Kill();
                    moveTween = null;
                }
            }
            else if (type == CrossHairNormalTypeStatue.Shot)
            {
                if (moveTween != null)
                {
                    moveTween.Kill();
                    moveTween = null;
                }
                if (shootTween != null)
                {
                    shootTween.Kill();
                    shootTween = null;
                }
            }
        }
        //更新上下左右 四个线段的位置
        private void UpdateNormalLinePos(float value)
        {
            if (value > normalTypeEndPos)
                value = normalTypeEndPos;
            if (value < normalTypeStartPos)
                value = normalTypeStartPos;
            sTopRt.localPosition = new Vector3(sTopRt.localPosition.x, value, sTopRt.localPosition.z);
            sDownRt.localPosition = new Vector3(sDownRt.localPosition.x, -value, sDownRt.localPosition.z);

            hLeftRt.localPosition = new Vector3(-value, hLeftRt.localPosition.y, hLeftRt.localPosition.z);
            hRightRt.localPosition = new Vector3(value, hRightRt.localPosition.y, hRightRt.localPosition.z);
        }       
        private void SetNormalCrossType(int weaponAvatarId)
        {
            if(weaponAvatarId != lastWeaponAvatarId)
            {             
                var avatarConfig = SingletonManager.Get<WeaponAvatarConfigManager>().GetConfigById(weaponAvatarId);
                if (avatarConfig != null)
                {
                    //Debug.Log(I2.Loc.ScriptLocalization.client_common.word2 +avatarConfig.Id + " type:" + avatarConfig.SubType + " name:" + avatarConfig.Name);
                    //var result = adapter.IsShowCrossHair == true ? "yes" : "not";
                    //Debug.Log("active:" + result);
                    if (avatarConfig.SubType == (int)EWeaponSubType.Pistol)   //如果是手枪    只有左右
                    {
                        sTop.gameObject.SetActive(false);
                        sDown.gameObject.SetActive(false);
                        hLeft.gameObject.SetActive(true);
                        hRight.gameObject.SetActive(true);
                    }
                    else if(avatarConfig.SubType ==(int)EWeaponSubType.Melee
                        || avatarConfig.SubType == (int)EWeaponSubType.Hand)   //近战武器    只有中心点
                    {
                        sTop.gameObject.SetActive(false);
                        sDown.gameObject.SetActive(false);
                        hLeft.gameObject.SetActive(false);
                        hRight.gameObject.SetActive(false);
                    }
                    else 
                    {
                        sTop.gameObject.SetActive(true);
                        sDown.gameObject.SetActive(true);
                        hLeft.gameObject.SetActive(true);
                        hRight.gameObject.SetActive(true);
                    }


                    var bundleName = avatarConfig.StarReticleBundle;
                    var iconNames = avatarConfig.StarReticle;  //包裹准心和四边的图
                    string centerName = "";
                    string aroundName = "";
                    if (iconNames.Count == 2)        //准心和四周都有图
                    {
                        centerName = iconNames[0];
                        aroundName = iconNames[1];
                        Loader.RetriveSpriteAsync(bundleName, centerName, (sprite) =>
                        {
                            centerImg.sprite = sprite;
                        });

                        Loader.RetriveSpriteAsync(bundleName, aroundName, (sprite) =>
                        {
                            sTopImg.sprite = sprite;
                            sDownImg.sprite = sprite;
                            hLeftImg.sprite = sprite;
                            hRightImg.sprite = sprite;
                        });                      
                    }
                    else if(iconNames.Count == 1)    //只有准心图
                    {
                        centerName = iconNames[0];
                        aroundName = "";
                        Loader.RetriveSpriteAsync(bundleName, centerName, (sprite) =>
                        {
                            centerImg.sprite = sprite;
                        });
                    }
                    else
                    {
                        Debug.Log(I2.Loc.ScriptLocalization.client_common.word46);
                        Loader.RetriveSpriteAsync("icon/reticle", "reticledot", (sprite) =>
                        {
                            centerImg.sprite = sprite;
                        });

                        Loader.RetriveSpriteAsync("icon/reticle", "hairLine", (sprite) =>
                        {
                            sTopImg.sprite = sprite;
                            sDownImg.sprite = sprite;
                            hLeftImg.sprite = sprite;
                            hRightImg.sprite = sprite;
                        });
                    }
                }
                lastWeaponAvatarId = weaponAvatarId;
            }
        }
        #endregion

        private void SetCrossAttackActive(bool active)
        {
            _viewModel.crossHariRootActive = active;
            _viewModel.attackRootActive = active;
        }       

        protected override void OnGameobjectDestoryed()
        {
            if (moveTween != null)
            {
                moveTween.Kill();
                moveTween = null;
            }
            if(shootTween != null)
            {
                shootTween.Kill();
                shootTween = null;
            }

            if(disAppearRedTween != null)
            {
                disAppearRedTween.Kill();
                disAppearRedTween = null;
            }

            if(disAppearWhiteTween != null)
            {
                disAppearWhiteTween.Kill();
                disAppearWhiteTween = null;
            }          
        }
    }
}
