using System;
using System.Collections.Generic;
using Core.Enums;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class UIComponent : IComponent
    {
        [DontInitilize] public bool IsShowCrossHair;

        [DontInitilize] public bool CountingDown;
        [DontInitilize] public float CountDownNum;

        [DontInitilize] public SplitPropInfo SplitPropInfo;

        [DontInitilize] public ScreenFlashInfo ScreenFlashInfo;

        //受伤组
        [DontInitilize] public Dictionary<int, CrossHairHurtedData> HurtedDataList;

        [DontInitilize] public List<IKillInfoItem> KillInfos;
        [DontInitilize] public bool KillMessageChanged;

        [DontInitilize] public bool IsGameBegin;
        [DontInitilize] public int JoinPlayerCount;
        [DontInitilize] public int SurvivalCount;
        [DontInitilize] public int BeatCount;

        [DontInitilize] public List<int> KillFeedBackList;

        [DontInitilize] public int GameTime;
        [DontInitilize] public int ScoreForWin;
        [DontInitilize] public Dictionary<EUICampType, int> ScoreByCampTypeDict;

//        [DontInitilize] public bool IsShowWeaponBag;

        #region GroupTechStat
        //[DontInitilize] public bool IsShowGroupTechStat;
        [DontInitilize] public Dictionary<EUICampType, List<IGroupBattleData>> GroupBattleDataDict;
        [DontInitilize] public bool GroupBattleDataChanged;
        [DontInitilize] public string ChannelName;
        [DontInitilize] public string RoomName;
        [DontInitilize] public int PlayerCount;
        [DontInitilize] public int PlayerCapacity;
        [DontInitilize] public int RoomId;
        #endregion

        #region CommonGameOver
        //[DontInitilize] public bool IsGameOver;
        [DontInitilize] public EUIGameResultType GameResult;
        #endregion

        [DontInitilize] public int MyselfGameTitle;




        //通知界面全局数据 使用ShowNoticeWindow 设置数据
        [DontInitilize] public INoticeInfoItem NoticeInfoItem;

        public void ShowNoticeWindow(NoticeWindowStyle style, string title, Action yesCB, Action noCB, string yesText, string noText, float countDownTime, Action countDownCallback)
        {
            NoticeInfoItem.style = style;
            NoticeInfoItem.Title = title;
            NoticeInfoItem.YesCallback = yesCB;
            NoticeInfoItem.NoCallback = noCB;
            NoticeInfoItem.YesText = yesText;
            NoticeInfoItem.NoText = noText;
            NoticeInfoItem.CountDownTime = countDownTime;
            NoticeInfoItem.CountDownCallback = countDownCallback;
        }

        [DontInitilize] public EUIBombInstallState C4InstallState;
        [DontInitilize] public int CurRoundCount;
        [DontInitilize] public Dictionary<EUICampType, IPlayerCountData> PlayerCountByCampTypeDict;
        //[DontInitilize] public bool IsRoundOver;
        [DontInitilize] public List<ICountdownTipData> CountdownTipDataList;
        [DontInitilize] public List<ITaskTipData> TaskTipDataList;

        [DontInitilize] public float LoadingRate;   //预加载 当前比例
        [DontInitilize] public string LoadingText;  //预加载 当前加载文本提示
        [DontInitilize] public string KillerName;//击杀当前玩家的杀手名称
        [DontInitilize] public Vector3 KillerTopPos;//击杀当前玩家的杀手头顶位置
        [DontInitilize] public bool DeadButtonShow;//死亡界面上的按钮是否显示
        [DontInitilize] public bool HaveAliveTeammate;//当前玩家是否有存活队友
        [DontInitilize] public long KillerId;//击杀者id
        [DontInitilize] public bool KillerChanged;//击杀者发生改变
        [DontInitilize] public bool IsPause;//游戏是否暂停
        [DontInitilize] public List<int> PaintIdList;
        [DontInitilize] public int SelectedPaintIndex;
        [DontInitilize] public List<IBaseChickenBagItemData> ChickenBagItemDataList;

        [DontInitilize] public int CurPlayerCountInPlane;
        [DontInitilize] public int TotalPlayerCountInPlane;
    }
}
