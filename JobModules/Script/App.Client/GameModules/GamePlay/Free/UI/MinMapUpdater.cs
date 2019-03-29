using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.GameModules.Vehicle;
using Assets.Sources.Free.UI;
using Core.Utils.System46;
using UnityEngine;
using App.Shared.Terrains;
using System;
using App.Client.GameModules.Terrain;
using App.Shared;
using App.Shared.Components.Ui;
using App.Shared.GameModules.Player;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class MinMapUpdater : IUIUpdater
    {
        public bool IsDisabled { get; set; }

        private MyDictionary<long, MiniMapTeamPlayInfo> infoMap = new MyDictionary<long, MiniMapTeamPlayInfo>();

        private string _teamNums = "";

        public void UIUpdate(int frameTime)
        {
            if (SingletonManager.Get<FreeUiManager>().Contexts1 == null
                || SingletonManager.Get<FreeUiManager>().Contexts1.ui.uISessionEntity.uISession == null
                || SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity == null)
            {
                return;
            }

            PlayerEntity selfEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;

            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;
            data.TeamInfos.Clear();
            _teamNums = "";

            var map = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;
            map.TeamPlayerMarkInfos.Clear();

            foreach (PlayerEntity playerEntity in SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntities())
            {
                try
                {
                    if (playerEntity.hasPlayerInfo && playerEntity.hasPosition
                        && playerEntity.playerInfo.TeamId == selfEntity.playerInfo.TeamId)
                    {
                        if (!infoMap.ContainsKey(playerEntity.playerInfo.PlayerId))
                        {
                            MiniMapTeamPlayInfo info = new MiniMapTeamPlayInfo(playerEntity.playerInfo.PlayerId, playerEntity.playerInfo.EntityId, true, 1,
                                TeamColor(playerEntity.playerInfo.Num), MiniMapPlayStatue.NORMAL, new Vector2(500, 300), 45f,
                                new List<MiniMapPlayMarkInfo>()
                                {
                                    new MiniMapPlayMarkInfo(new Vector3(500, 250), 1),
                                }, false, 0,
                                playerEntity.playerInfo.PlayerName, (int)playerEntity.gamePlay.CurHp, playerEntity.gamePlay.MaxHp, (int)playerEntity.gamePlay.InHurtedHp, false, playerEntity.position.Value);
                            
                            infoMap[playerEntity.playerInfo.PlayerId] = info;
                        }

                        Vector3 leftMinPos = TerrainCommonData.leftMinPos;

                        MiniMapTeamPlayInfo oneInfo = infoMap[playerEntity.playerInfo.PlayerId];

                        if (playerEntity == selfEntity)
                        {
                            oneInfo.IsPlayer = true;
                            data.CurPlayer = oneInfo;
                        }
                        else
                        {
                            oneInfo.IsPlayer = false;
                        }

                        if (oneInfo.Num != playerEntity.playerInfo.Num)
                        {
                            oneInfo.Num = playerEntity.playerInfo.Num;
                            oneInfo.Color = TeamColor(oneInfo.Num);
                        }

                        //pos,rot
                        /*if (playerEntity.hasControlledVehicle)
                        {
                            //车上
                            Transform trans = playerEntity.GetVehicleSeatTransform(SingletonManager.Get<FreeUiManager>().Contexts1.vehicle);
                            oneInfo.Pos = new Vector2(trans.position.x - leftMinPos.x, trans.position.z - leftMinPos.z);
                            oneInfo.FaceDirection = trans.eulerAngles.y;
                        }
                        else */
                        if (playerEntity.gamePlay.GameState == GameState.AirPlane)
                        {
                            //飞机上（位置已做过偏移，这里不再偏移）
                            AirPlaneData planeData = data.PlaneData;
                            oneInfo.Pos = new Vector2(planeData.Pos.x, planeData.Pos.y);
                            oneInfo.FaceDirection = planeData.Direction;
                        }
                        else
                        {
                            oneInfo.Pos = new Vector2(playerEntity.position.Value.x - leftMinPos.x, playerEntity.position.Value.z - leftMinPos.z);
                            oneInfo.FaceDirection = playerEntity.orientation.Yaw;
                        }

                        //Test Trace
                        if (oneInfo.IsPlayer)
                        {
                            TerrainTestSystem.yaw = oneInfo.FaceDirection;
                        }
                        if (_teamNums.Equals(""))
                        {
                            _teamNums = oneInfo.Num.ToString();
                        }
                        else
                        {
                            _teamNums += "|" + oneInfo.Num;
                        }

                        //status
                        if (playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dead))
                        {
                            oneInfo.Statue = MiniMapPlayStatue.DEAD;
                            data.RemoveMapMark(playerEntity.playerInfo.PlayerId);
                        }
                        else if (playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dying))
                        {
                            oneInfo.Statue = MiniMapPlayStatue.HURTED;
                        }
                        else
                        {
                            if (playerEntity.IsOnVehicle())
                            {
                                oneInfo.Statue = MiniMapPlayStatue.ZAIJU;
                            }
                            else if (PlayerIsDrop(playerEntity))
                            {
                                oneInfo.Statue = MiniMapPlayStatue.TIAOSAN;
                            }
                            else
                            {
                                oneInfo.Statue = MiniMapPlayStatue.NORMAL;
                            }
                        }

                        //map mark
                        oneInfo.MarkList.Clear();
                        if (oneInfo.IsPlayer)
                        {
                            foreach (var mark in data.MapMarks.Values)
                            {
                                oneInfo.MarkList.Add(mark);

                                TeamPlayerMarkInfo lmark = new TeamPlayerMarkInfo();
                                lmark.Angel = CommonMathUtil.GetAngle(mark.Pos, oneInfo.Pos);
                                lmark.MarkColor = TeamColor(mark.Num);
                                map.TeamPlayerMarkInfos.Add(lmark);
                            }
                            TerrainTestSystem.mark = map.TeamPlayerMarkInfos.Count;
                        }

                        //New
                        oneInfo.PlayerName = playerEntity.playerInfo.PlayerName;
                        oneInfo.CurHp = (int)playerEntity.gamePlay.CurHp;
                        oneInfo.MaxHp = playerEntity.gamePlay.MaxHp;
                        oneInfo.CurHpInHurted = (int)playerEntity.gamePlay.InHurtedHp;
                        oneInfo.IsMark = data.MapMarks.ContainsKey(oneInfo.PlayerId) ? true : false;
                        oneInfo.TopPos = PlayerEntityUtility.GetPlayerTopPosition(playerEntity);
                        oneInfo.EntityId = playerEntity.entityKey.Value.EntityId;
                        data.TeamInfos.Add(oneInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat(e.Message);
                }
            }

            TerrainTestSystem.teamCnt = data.TeamInfos.Count;
            TerrainTestSystem.teamNum = _teamNums;
        }

        private Color TeamColor(int num)
        {
            switch (num)
            {
                case 1:
                    return new Color(0xff / 255f, 0xff / 255f, 0xff / 255f);
                case 2:
                    return new Color(0x3b / 255f, 0x80 / 255f, 0xe8/ 255f);
                case 3:
                    return new Color(0xe8 / 255f, 0x3f / 255f, 0x3b / 255f);
                case 4:
                    return new Color(0xe8 / 255f, 0xc9 / 255f, 0x3b / 255f);
                case 5:
                    return new Color(0xde / 255f, 0x3b / 255f, 0xe8 / 255f);
                default:
                    return new Color(0xff / 255f, 0xff / 255f, 0xff / 255f);
            }
        }

        public static bool PlayerIsDrop(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
                return false;
            if (SharedConfig.IsOffline)
            {
                return playerEntity.hasStateInterface && (playerEntity.stateInterface.State.GetActionState() == ActionInConfig.Gliding
                                                          || playerEntity.stateInterface.State.GetActionState() == ActionInConfig.Parachuting);
            }
            else
            {
                return playerEntity.hasGamePlay && (playerEntity.gamePlay.GameState == GameState.Gliding
                                                    || playerEntity.gamePlay.GameState == GameState.JumpPlane);
            }
        }

    }
}
