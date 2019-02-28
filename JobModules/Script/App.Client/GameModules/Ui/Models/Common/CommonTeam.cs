using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.Utils.MiniMaxMapCommon;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonTeam : ClientAbstractModel, IUiSystem
    {
        ITeamUiAdapter adapter = null;
        private bool isGameObjectCreated = false;
        private Transform infoItemModel = null;
        private Transform infoItemRoot = null;
        private UIUtils.SimplePool pool = null;
        private float modelHeight = 0;        //每个Item的高度
        private float teamItemInterval = 10;  //Item之间的垂直间隔
        private float teamInfoHeight = 0;     //组队所有Item的高度

        private CommonTeamViewModel _viewModel = new CommonTeamViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonTeam(ITeamUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGUI();            
        }

       public override void Update(float interval)
        {
            RefreshGUI();
        }   

        private void InitGUI()
        {
            infoItemRoot = FindChildGo("TeamRoot");
            infoItemModel = FindChildGo("TeamItem");
            if (infoItemRoot == null || infoItemModel == null)
                return;
            modelHeight = infoItemModel.GetComponent<RectTransform>().sizeDelta.y;
            infoItemModel.gameObject.SetActive(false);
            pool = new UIUtils.SimplePool(infoItemModel, infoItemRoot);          
        }
        
        private void RefreshGUI()
        {
            if (!isGameObjectCreated)
                return;
            if (infoItemRoot == null || infoItemModel == null)
                return;

            var modelId = adapter.ModeID;
            if (modelId == 1)    //单人模式
            {
                _viewModel.rootActiveSelf = false;
            }
            else                 //其他模式
            {
                _viewModel.rootActiveSelf = true;
                var datas = adapter.TeamPlayerInfos;
                if (datas.Count > 0)
                {
                    pool.DespawnAllGo();
                    teamInfoHeight = 0;

                    //排序 保证序号从1开始向下
                    datas.Sort((left, right)=> 
                    {
                        if (left.Num > right.Num)
                            return 1;
                        else if (left.Num == right.Num)
                            return 0;
                        else
                            return -1;
                    });

                    for (int i = 0; i < datas.Count; i++)
                    {
                        var tran = pool.SpawnGo();
                        RefreshTeamItem(tran, datas[i]);
                    }
                }
            }
        } 

        private void RefreshTeamItem(Transform tran, MiniMapTeamPlayInfo data)
        {
            //刷新位置
            var tranRt = tran.GetComponent<RectTransform>();
            tranRt.anchoredPosition = -1 * new Vector2(0, teamInfoHeight + teamItemInterval);
            teamInfoHeight += modelHeight + teamItemInterval;

            //刷新编号和编号背景色
            var numberGo = tran.Find("Number");
            var numberImage = numberGo.GetComponent<Image>();
            if(numberImage)
            {        
                if (data.IsDead)  //死亡状态
                {
                    numberImage.color = new Color(data.Color.r, data.Color.g, data.Color.b, 0.5f);
                }
                else
                {
                    numberImage.color = data.Color;
                }

                var numberText = numberGo.Find("NumberText");                
                if(numberText)
                {
                    var numberTextCom = numberText.GetComponent<Text>();
                    if(numberTextCom)
                    {
                        numberTextCom.text = data.Num.ToString();
                    }
                }
            }

            //刷新名字
            var nameGo = tran.Find("Name");
            var nameText = nameGo.GetComponent<Text>();
            if(nameText)
            {
                nameText.text = data.PlayerName;
                if (data.IsDead)  //死亡状态
                {
                    nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
                }
                else
                {
                    nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 1);
                }
            }

            //刷新血量
            var hpGo = tran.Find("Hp");
            var hpSlider = hpGo.GetComponent<Slider>();
            var fillBg = hpGo.Find("Background");    //血条背景
            var fillParent = hpGo.Find("FillArea");
            Transform fillFront = null;
            fillFront = fillParent.Find("Fill");      //血量填充      

            if (hpSlider && fillParent && fillFront && fillBg)
            {
                var fillFrontImgCom = fillFront.GetComponent<Image>(); 
                if (data.IsDead)    //死亡状态
                {
                    hpSlider.value = 0;
                }
                else if (data.IsInHurtedStatue) //受伤状态
                {
                    fillFrontImgCom.color = Color.red;
                    hpSlider.value = (float)data.CurHpInHurted / data.MaxHp;
                }
                else                            //正常状态
                {
                    hpSlider.value = (float)data.CurHp / data.MaxHp;
                    if (hpSlider.value < 0.3f)
                    {
                        fillFrontImgCom.color = new UnityEngine.Color(237f / 255f, 129f / 255f, 129f / 255f, 1.0f);
                    }
                    else
                    {
                        fillFrontImgCom.color = new UnityEngine.Color(247f / 255f, 238f / 255f, 201f / 255f, 1.0f);
                    }
                }               
            }
            
            //刷新标记
            var markGo = tran.Find("Mark");
            var markImgCom = markGo.GetComponent<Image>();
            if (markGo != null && markImgCom != null)
            {
                if (data.IsDead || !data.IsMark)  //死亡状态 获取没有标记的话
                {
                    markGo.gameObject.SetActive(false);
                }
                else
                {
                    markGo.gameObject.SetActive(true);
                    markImgCom.color = data.Color;
                }
            }

            //刷新状态  
            var statueBgGo = tran.Find("StatueBg");
            var statue = statueBgGo.Find("Statue");
            var statueImgCom = statue.GetComponent<Image>();
            statueBgGo.gameObject.SetActive(true);
            if (statueBgGo != null && statue != null && statueImgCom != null)
            {
                switch (data.Statue)
                {
                    case MiniMapPlayStatue.TIAOSAN:  //跳伞
                        {
                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_parachute");
                            if (temperSprite != null && temperSprite != statueImgCom.sprite)
                            {
                                statueImgCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.ZAIJU:  //载具
                        {
                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_drive");
                            if (temperSprite != null && temperSprite != statueImgCom.sprite)
                            {
                                statueImgCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.HURTED:  //受伤
                        {
                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_hurt");
                            if (temperSprite != null && temperSprite != statueImgCom.sprite)
                            {
                                statueImgCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.DEAD:  //死亡
                        {
                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_die");
                            if (temperSprite != null && temperSprite != statueImgCom.sprite)
                            {
                                statueImgCom.sprite = temperSprite;
                            }
                        }
                        break;
                    default:
                        {
                            statueBgGo.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }       
    }    
}    
