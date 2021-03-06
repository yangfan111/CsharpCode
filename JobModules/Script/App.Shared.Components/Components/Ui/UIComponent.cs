﻿using System;
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
        [DontInitilize] public bool HaveCompletedCountDown;

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
        [DontInitilize] public int[] ScoreByCampTypeDict;

//        [DontInitilize] public bool IsShowWeaponBag;

        #region GroupTechStat
        //[DontInitilize] public bool IsShowGroupTechStat;
        [DontInitilize] public List<IGroupBattleData>[] GroupBattleDataDict;
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


        [DontInitilize] public EUIBombInstallState C4InstallState;
        [DontInitilize] public int CurRoundCount;
        [DontInitilize] public IPlayerCountData[] PlayerCountByCampTypeDict;
        //[DontInitilize] public bool IsRoundOver;
        [DontInitilize] public List<ITipData> CountdownTipDataList;
        [DontInitilize] public Queue<ITipData> SystemTipDataQueue;
        [DontInitilize] public ITipData OperationTipData;
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
        [DontInitilize] public bool FreshSelectedPaintIndex;
        [DontInitilize] public List<IBaseChickenBagItemData> ChickenBagItemDataList;

        [DontInitilize] public int CurPlayerCountInPlane;
        [DontInitilize] public int TotalPlayerCountInPlane;

        [DontInitilize] public List<long> MotherIdList;
        [DontInitilize] public List<long> HeroIdList;
        [DontInitilize] public List<long> HumanIdList;

        [DontInitilize] public int[] WeaponIdList;

        [DontInitilize] public int[,] WeaponPartList;

        [DontInitilize] public KeyValuePair<int, int>[] EquipIdList;

        [DontInitilize] public int HoldWeaponSlotIndex;
        [DontInitilize] public int CurBagWeight;
        [DontInitilize] public int TotalBagWeight;
        [DontInitilize] public bool ShowBuffTip;
        [DontInitilize] public string BuffTip;
        [DontInitilize] public PlayerEntity Player;
        [DontInitilize] public float C4InitialProgress;
    }
}
