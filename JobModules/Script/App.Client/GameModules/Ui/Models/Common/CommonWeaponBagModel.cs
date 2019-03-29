using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.Utility;
using App.Shared.Components.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.AssetManager;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common
{
    class WeaponBagUiItem
    {
        public Transform WeaponBagItem;
        public Text WeaponName;
        public Image WeaponIcon;
        public Transform WeaponPartGroup;
        public List<Transform> WeaponPartTfList;
        public List<Image> WeaponPartIconList;
    }
    public class CommonWeaponBagModel : ClientAbstractModel, IUiHfrSystem
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonWeaponBagModel));
        private CommonWeaponBagViewModel _viewModel = new CommonWeaponBagViewModel();
        private IWeaponBagUiAdapter _adapter;

        //TODO 空槽位可能会有特殊图标代替
        private AssetInfo _emptyIcon;
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonWeaponBagModel(IWeaponBagUiAdapter weaponBagState):base(weaponBagState)
        {
            _adapter = weaponBagState;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
            InitOpenKeyReveiver();
        }

        private KeyReceiver keyReveiver,openKeyReceiver;
        private PointerReceiver pointerReceiver;

        private void InitOpenKeyReveiver()
        {
            openKeyReceiver = new KeyReceiver(UiConstant.weaponBagWindowLayer, BlockType.None);
            openKeyReceiver.AddAction(UserInputKey.OpenWeaponBag, (data) =>
            {
                SwitchWeaponBagViewShow();
            });
            _adapter.RegisterKeyReceive(openKeyReceiver);
        }

        private void InitKeyReveiver()
        {
            keyReveiver = new KeyReceiver(UiConstant.weaponBagWindowKeyBlockLayer, BlockType.All);
            for (int i = 0; i < _haveBagForIndexList.Count; i++)
            {
                if (_haveBagForIndexList[i])
                {
                    var index = i - 1;
                    keyReveiver.AddAction(UserInputKey.Switch1 + index,
                        (data) => {
                            btnGroupTab.SetSelect(index);
                            CloseView();
                        });
                    keyReveiver.AddAction(UserInputKey.HideWindow,
                        (data) => {
                            CloseView();
                        });
                }
            }
            pointerReceiver = new PointerReceiver(UiConstant.weaponBagWindowKeyBlockLayer, BlockType.All);
        }

        private void SwitchWeaponBagViewShow()
        {
            var show = _adapter.Enable;
            _adapter.Enable = !show;
        }

        Transform btnGroup;
        UITabBehaviour btnGroupTab;
        Transform indexBtn;
        private bool _inited;

        private List<bool> _haveBagForIndexList;

        private int MaxBagNum
        {
            get { return 5; }
        }

        private int MaxWeaponPartNum
        {
            get { return 5; }
        }

        private void InitBagTag()
        {
            _haveBagForIndexList = new List<bool>(new bool[MaxBagNum + 1]);
            for (int i = 1; i <= MaxBagNum; i++)
            {
                var bagInfo = _adapter.GetWeaponBagInfoByBagIndex(i);
                bool haveBag = bagInfo != null;

                _haveBagForIndexList[i] = haveBag;
                var index = i - 1;
                if(index < btnGroup.childCount)
                {
                    var btn = btnGroup.GetChild(index);
                    if (null != btn)
                    {
                        btn.gameObject.SetActive(haveBag);
                    }
                }
                else
                {
                    Debug.LogFormat("btnGroup child count is {0}, but get {1}", btnGroup.childCount, index);
                    Logger.ErrorFormat("btnGroup child count is {0}, but get {1}", btnGroup.childCount, index);
                }
            }

            btnGroupTab.SetSelect(_adapter.CurBagIndex - 1);
        }

        public void Init()
        {
            InitBagTag();
            InitKeyReveiver();
            _inited = true; 
        }

        List<Transform> weaponItemList;
        Dictionary<Transform, WeaponBagUiItem> weaponItemDict = new Dictionary<Transform, WeaponBagUiItem>();
        Transform primeWeapon, subWeapon, meleeWeapon, tacticalWeapon,grenadeGroup;
        List<Transform> grenadeList = new List<Transform>();
        Transform weaponItem;

        private void InitVariable()
        {
            weaponItem = FindComponent<Transform>("CommonWeaponItem");
            weaponItem.gameObject.SetActive(false);
            primeWeapon = FindComponent<Transform>("PrimeWeaponItem");
            subWeapon = FindComponent<Transform>("SecondaryWeaponItem");
            meleeWeapon = FindComponent<Transform>("MeleeWeaponItem");
            tacticalWeapon = FindComponent<Transform>("TacticalWeaponItem");
            grenadeGroup = FindComponent<Transform>("GrenadeGroup");
            for (int i = 0; i < grenadeGroup.childCount; i++)
            {
                grenadeList.Add(grenadeGroup.GetChild(i));
            }

            btnGroup = FindComponent<Transform>("BtnGroup");
            btnGroupTab = FindComponent<UITabBehaviour>("BtnGroup");
            btnGroupTab.tabSelectAction = SwitchBag;
            indexBtn = FindComponent<Transform>("IndexBtn");

            try
            {
                _viewModel.ConfirmBtnClick = CloseView;
//                _viewModel.ConfirmBtnClick = () => CloseView();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            
        }

        private void CloseView()
        {
            _adapter.Enable = false;
        }

        private void SwitchBag(GameObject go, int index)
        {
            SwitchBag(index);
            //UpdateBagInfo();
        }

        private void SwitchBag(int index)
        {
            int bagIndex = index + 1;
            if (_haveBagForIndexList[bagIndex])
            {
                _adapter.SwitchBagByBagIndex(bagIndex);
            }
        }

        private void InitGui()
        {
            GetNewWeaponItem(primeWeapon,true,I2.Loc.ScriptLocalization.client_common.word21);
            GetNewWeaponItem(subWeapon,true,I2.Loc.ScriptLocalization.client_common.word22);
            GetNewWeaponItem(meleeWeapon,false,I2.Loc.ScriptLocalization.client_common.word37);
            GetNewWeaponItem(tacticalWeapon,false,I2.Loc.ScriptLocalization.client_common.word24);
            for (int i = 0; i < grenadeGroup.childCount; i++)
            {
                GetNewWeaponItem(grenadeGroup.GetChild(i),false);
            }
        }
        bool _haveRegister = false;
        public override void Update(float interval)
        {
            UpdateRemainTime();
            var curBagIndex = _adapter.CurBagIndex;
            if (btnGroupTab.GetSelectIndex() != curBagIndex - 1)
            {
                btnGroupTab.SetSelect(curBagIndex - 1);
            }
            if (lastBagIndex != curBagIndex)
            {
                UpdateBagInfo();
                lastBagIndex = curBagIndex;
            }
            //if(_adapter.CurBagIndex != btnGroupTab.GetSelectIndex() + 1)
        }

        private int lastBagIndex = -1;

        private void UpdateRemainTime()
        {
            var remainTime = _adapter.RemainOperating;

            _viewModel.CloseTipText = string.Format(I2.Loc.ScriptLocalization.client_common.word38, remainTime >= 0 ? remainTime : 0);
        }

        private void RegisterKeyReceiver()
        {
            _adapter.SetCrossVisible(false);

            if (keyReveiver == null)
            {
                return;
            }
            _adapter.RegisterKeyReceive(keyReveiver);
            _adapter.RegisterPointerReceive(pointerReceiver);
            //CursorLocker.SystemUnlock = true;
            _haveRegister = true;
        }

        private void UnRegisterKeyReceiver()
        {
            _adapter.SetCrossVisible(true);

            if (keyReveiver == null)
            {
                return;
            }
            _adapter.UnRegisterKeyReceive(keyReveiver);
            _adapter.UnRegisterPointerReceive(pointerReceiver);

            //CursorLocker.SystemUnlock = false;
            _haveRegister = false;
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);

            if (!_inited && enable && _viewInitialized)
            {
                Init();
            }

            _adapter.Enable = enable;

            if (enable && !_haveRegister)
            {
                RegisterKeyReceiver();
            }
            else if (!enable && _haveRegister)
            {
                UnRegisterKeyReceiver();
            }
        }

        private void UpdateBagInfo()
        {
            int bagIndex = _adapter.CurBagIndex;
            if (_haveBagForIndexList[bagIndex])
            {
                var bagInfo = _adapter.GetWeaponBagInfoByBagIndex(bagIndex);
                if (null == bagInfo)
                {
                    return;
                }
                UpdateBagItem(bagInfo.PrimeWeapon, weaponItemDict[primeWeapon]);
                UpdateBagItem(bagInfo.SubWeapon, weaponItemDict[subWeapon]);
                UpdateBagItem(bagInfo.MeleeWeapon, weaponItemDict[meleeWeapon]);
                UpdateBagItem(bagInfo.TacticalWeapon, weaponItemDict[tacticalWeapon]);
                UpdateGrenadeItem(bagInfo, grenadeList);
            }
        }

        private void UpdateGrenadeItem(IWeaponBagInfo bagInfo, List<Transform> grenadeList)
        {
            for (int i = 0; i < 3; i++)
            {
                var grenadeInfo = bagInfo.GetGrenadeInfoByIndex(i + 1);
                UpdateBagItem(grenadeInfo, weaponItemDict[grenadeList[i]]);
            }
        }

        Dictionary<AssetInfo, Sprite> spritePool = new Dictionary<AssetInfo, Sprite>();


        private void UpdateBagItem(IWeaponBagItemInfo item, WeaponBagUiItem uiItem)
        {
            uiItem.WeaponName.text = null == item ? "" : item.WeaponName;
            var weaponIcon = uiItem.WeaponIcon;
            var assetInfo = null == item ? _emptyIcon : item.WeaponIconAssetInfo;
            if (spritePool.ContainsKey(assetInfo))
            {
                var sprite = spritePool[assetInfo];
                if (string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName) || sprite == null)
                {
                    weaponIcon.enabled = false;
                }
                else
                {
                    weaponIcon.enabled = true;
                    weaponIcon.sprite = spritePool[assetInfo];
                }
                UIUtils.SetImageSuitable(weaponIcon);
            }
            else
            {
                spritePool.Add(assetInfo, new Sprite());
                Loader.RetriveSpriteAsync(assetInfo.BundleName, assetInfo.AssetName, (sprite) =>
                {
                    spritePool[assetInfo] = sprite;
                    weaponIcon.sprite = sprite;
                    UIUtils.SetImageSuitable(weaponIcon);
                });
            }           

            var weaponPartGroup = uiItem.WeaponPartGroup;
            if (weaponPartGroup.gameObject.activeSelf)
            {
                UpdateWeaponPart(uiItem, item);
            }
        }

        private Dictionary<EWeaponPartType, int> WeaponPartTypeToIndexDict = new Dictionary<EWeaponPartType, int>
        {
            {EWeaponPartType.UpperRail, 1},
            {EWeaponPartType.Muzzle, 2},
            {EWeaponPartType.LowerRail, 3},
            {EWeaponPartType.Magazine, 4},
            {EWeaponPartType.Stock, 5}
        };


        private void UpdateWeaponPart(WeaponBagUiItem uiItem, IWeaponBagItemInfo info)
        {
            foreach (var pair in WeaponPartTypeToIndexDict)
            {
                var assetInfo = null == info ? _emptyIcon : info.GetWeaponPartInfoByWeaponPartType(pair.Key);
                var tf = uiItem.WeaponPartTfList[pair.Value - 1];
                var image = uiItem.WeaponPartIconList[pair.Value - 1];

                if (string.IsNullOrEmpty(assetInfo.AssetName) || string.IsNullOrEmpty(assetInfo.BundleName))
                {
                    tf.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    tf.gameObject.SetActive(true);
                }
                if (spritePool.ContainsKey(assetInfo))
                {
                    image.sprite = spritePool[assetInfo];
                }
                else
                {
                    spritePool.Add(assetInfo, new Sprite());
                    Loader.RetriveSpriteAsync(assetInfo.BundleName, assetInfo.AssetName, (sprite) => {
                        spritePool[assetInfo] = sprite;
                        image.sprite = sprite;
                    });
                }
            }
        }

        Transform GetNewWeaponItem(Transform root,bool haveWeaponPart,string type = "")
        {
            var transform =  GameObject.Instantiate(weaponItem, root);
            transform.gameObject.SetActive(true);
            var weaponPartGroup = transform.Find("WeaponPartGroup") as RectTransform;
            var weaponDisplayArea = transform.Find("WeaponDisplayArea") as RectTransform;
            weaponDisplayArea.anchorMin = new Vector2(0.5f,0.5f);
            weaponDisplayArea.anchorMax = new Vector2(0.5f, 0.5f);
            var offset = weaponDisplayArea.offsetMin.x;
            weaponDisplayArea.sizeDelta = new Vector2((root as RectTransform).sizeDelta.x -  2 * offset, weaponDisplayArea.sizeDelta.y);
            if (!haveWeaponPart)
            {
                weaponDisplayArea.sizeDelta = new Vector2(weaponDisplayArea.sizeDelta.x,
                    weaponDisplayArea.sizeDelta.y + weaponPartGroup.sizeDelta.y);

                weaponPartGroup.gameObject.SetActive(false);
            }

            transform.Find("Type").GetComponent<Text>().text = type;

            InitWeaponBagUiItem(root,transform);
            //weaponItemDict[root] = transform;
            return transform;
        }

        private void InitWeaponBagUiItem(Transform root,Transform transform)
        {
            WeaponBagUiItem uiItem = new WeaponBagUiItem();
            {
                uiItem.WeaponBagItem = transform;
                uiItem.WeaponName = transform.Find("Name").GetComponent<Text>();
                uiItem.WeaponIcon = transform.Find("WeaponDisplayArea/WeaponIcon").GetComponent<Image>();
                uiItem.WeaponPartGroup = transform.Find("WeaponPartGroup");
                uiItem.WeaponPartIconList = new List<Image>();
                uiItem.WeaponPartTfList = new List<Transform>();
                for (int i = 0; i < MaxWeaponPartNum; i++)
                {
                    var item = uiItem.WeaponPartGroup.GetChild(i);
                    uiItem.WeaponPartTfList.Add(item);
                    uiItem.WeaponPartIconList.Add(item.Find("Icon").GetComponent<Image>());
                }
            }
            weaponItemDict[root] = uiItem;
        }
    }
}


