using Assets.Sources.Free.Effect;
using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.System;
using App.Shared.Components.Player;
using App.Shared.Components.Ui;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Assets.Sources.Free.Auto;
using UnityEngine;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.Components;
using Core.SpatialPartition;
using Core.Ui.Map;
using Utils.Singleton;
using App.Shared.Components;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class MiniMapUiAdapter : UIAdapter, IMiniMapUiAdapter
    {
        private Contexts _contexts;

        private Vector2 _mapCenterByRice;
        private Vector2 _mapShowSizeByRice;

        public MiniMapUiAdapter(Contexts contexts)
        {
            _contexts = contexts;

            var offSet = SingletonManager.Get<MapsDescription>().SceneParameters.MapOffSet;
            _mapShowSizeByRice = new Vector2(offSet.width, offSet.height);
            _mapCenterByRice = _mapShowSizeByRice / 2 + new Vector2(offSet.x, offSet.y);
        }

        public float CurMapSize
        {
            get { return MapOrigin.Size.x; }
        }

        public Vector2 MapShowSizeByRice {
            get { return _mapShowSizeByRice; }
        }
        public Vector2 MapRealSizeByRice {
            get { return MapOrigin.Size; }
        }

        public Vector2 MapCenterByRice
        {
            get
            {
                return _mapCenterByRice;
            }
        }

        public float MapSegmentingLine
        {
            get { return 400; }
        }

        public MapLevel MapLevel
        {
            get
            {
                return (MapLevel)SingletonManager.Get<MapConfigManager>().SceneParameters.MapLevel;
            }
        }

        private PlayerEntity Player
        {
            get { return _contexts.ui.uI.Player; }
        }

        public float MoveSpeed
        {
            get
            {
                if (IsOnVehicle)
                {
                    return CurVehicleSpeed;
                }

                if (Player.hasPlayerMove) return Player.playerMove.Velocity.magnitude * 3.6f;

                return 0f;
            }
        }

        private bool IsOnVehicle
        {
            get { return null != Player && Player.IsOnVehicle(); }
        }

        private float CurVehicleSpeed
        {
            get
            {
                var vehicle = _contexts.vehicle.GetEntityWithEntityKey(Player.controlledVehicle.EntityKey);
                if (vehicle != null)
                {
                    return vehicle.GetUiPresentSpeed();
                }

                return 0f;
            }
        }

        #region TeamPlayMark
        private MiniMapTeamPlayInfo _MyPlayerInfo;
        public List<MiniMapTeamPlayInfo> TeamInfos
        {
            get
            {
                return _contexts.ui.map.TeamInfos;
            }
        }

        public MiniMapTeamPlayInfo GetPlayerById(long playerId)
        {
            foreach (MiniMapTeamPlayInfo playerInfo in TeamInfos)
            {
                if (playerId == playerInfo.PlayerId)
                    return playerInfo;
            }
            return null;
        }

        public MiniMapTeamPlayInfo CurPlayer
        {
            get { return _contexts.ui.map.CurPlayer; }
        }
        public Vector2 CurPlayerPos
        {
            get
            {
                if (null == CurPlayer)
                    return Vector2.zero;
                return CurPlayer.Pos.ShiftedUIVector2();
            }
        }
        public MapFixedVector3 CurPlayerPos3D
        {
            get
            {
                if (null == CurPlayer)
                    return new MapFixedVector3(Vector3.zero);
                return CurPlayer.Pos3D;
            }
        }
        #endregion

        #region 毒圈数据
        public DuQuanInfo CurDuquan   //当前毒圈数据
        {
            get
            {
                var _currInfo = _contexts.ui.map.CurDuquan;
                if (_currInfo != null)
                {
                    FreeRenderObject circle = SingletonManager.Get<FreeEffectManager>().GetEffect("circle");
                    if (null != circle)
                    {
                        _currInfo.Center = new MapFixedVector2(WorldOrigin.WorldPosition(new Vector3(circle.model3D.x, circle.model3D.y, circle.model3D.z)).To2D());
                        _currInfo.Radius = circle.GetEffect(0).EffectModel3D.model3D.scaleX / 2;
                    }
                }
                
                return _currInfo;
            }
        }
        public DuQuanInfo NextDuquan   //下一个毒圈数据
        {
            get
            {
                var _nextInfo = _contexts.ui.map.NextDuquan;
                return _nextInfo;
            }
        }
        public BombAreaInfo BombArea           //轰炸区
        {
            get
            {
                var _bormAreaInfo = _contexts.ui.map.BombArea;               
                return _bormAreaInfo;
            }
        }
        public int OffLineNum
        {
            get { return _contexts.ui.map.OffLineLevel; }
        }

        #endregion

        #region 航线数据
        public AirPlaneData PlaneData
        {
            get
            {
                return _contexts.ui.map.PlaneData;
            }
        }

        public List<MapFixedVector2> KongTouList()
        {
            int planeId = 0;
            kongTouList.Clear();

            FreeRenderObject plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane1");
            if (plane != null)
            {
                IAutoValue id = plane.GetAuto("id");
                if (id != null && id is AutoConstValue)
                {
                    planeId = int.Parse(((AutoConstValue) id).GetValue());

                    foreach (FreeRenderObject drop in SingletonManager.Get<FreeEffectManager>().FreeEffects.Values)
                    {
                        if (drop.key.StartsWith("dropBox"))
                        {
                            IAutoValue dropBoxId = drop.GetAuto("id");
                            if (dropBoxId != null && dropBoxId is AutoConstValue)
                            {
                                int realId = int.Parse(((AutoConstValue)dropBoxId).GetValue());
                                if (realId == planeId)
                                {
                                    kongTouList.Add(new MapFixedVector2(WorldOrigin.WorldPosition(new Vector3(drop.model3D.x, drop.model3D.y, drop.model3D.z)).To2D()));
//                                    Debug.LogFormat("AirDrop :  planeId:{0} dropId:{1}, pos: {2}", planeId, realId, WorldOrigin.WorldPosition(new Vector3(drop.model3D.x, drop.model3D.y, drop.model3D.z)));
                                }
                               
                            }
                        }
                    }
                }
            }

            return kongTouList;
        }

        private List<MapFixedVector2> kongTouList = new List<MapFixedVector2>() { new MapFixedVector2(10, 10), new MapFixedVector2(15, 15) };
        #endregion

        #region 地图ID
        private int mapId;
        public int MapId
        {
            get
            {
                return mapId = 1;  //1 目前用的默认的地图背景 mapPicture这张图片
            }

            set
            {
                mapId = value;
            }
        }

        #endregion

        #region Map Marks

        public Dictionary<long, MiniMapPlayMarkInfo> MapMarks
        {
            get { return _contexts.ui.map.MapMarks;}
        }



        #endregion


        #region 航线
        public bool IsShowRouteLine    //是否显示航线
        {
            get { return _contexts.ui.map.IsShowRouteLine; }
        }

        public Vector2 RouteLineStartPoint         //跳伞开始位置
        {
            get { return _contexts.ui.map.RouteLineStartPoint.ShiftedUIVector2(); }
        }

        public Vector2 RouteLineEndPoint    //跳伞结束位置
        {
            get { return _contexts.ui.map.RouteLineEndPoint.ShiftedUIVector2(); }
        }
        #endregion

        #region Tool
        
        public void SetCrossActive(bool isActive)
        {
            _contexts.ui.uI.IsShowCrossHair = isActive;
        }

        public void SendMarkMessage(Vector2 markPos)
        {
            PlayerAddMarkUtility.SendMarkMessage(_contexts, markPos);
        }
        #endregion

        public bool IsC4Drop
        {
            get { return _contexts.ui.blast.IsC4Droped; }
        }

        public Vector3 C4DropPosition
        {
            get
            {
                return _contexts.ui.blast.C4DropPosition.ShiftedUIVector3();
            }
        }

        public Vector3 APosition
        {
            get
            {
                return _contexts.ui.blast.BlastAPosition.ShiftedUIVector3();
            }
        }

        public Vector3 BPosition
        {
            get
            {
                return _contexts.ui.blast.BlastBPosition.ShiftedUIVector3();
            }
        }

        public bool isBombMode
        {
            get
            {
                return GameRules.IsBomb(GetGameRule());
            }
        }

        private int GetGameRule()
        {
            return _contexts.session.commonSession.RoomInfo.ModeId;
        }

        public bool IsCampPass()
        {
            var player = Player;
            var camp = player.playerInfo.Camp;
            return camp == (int)EUICampType.T;
        }

        public int C4SetStatus
        {
            get
            {
                return _contexts.ui.blast.C4SetStatus;
            }
        }


        public GamePlayComponent gamePlay
        {
            get { return _contexts.player.flagSelfEntity.gamePlay; }
        }

        #region Bio

        private List<Vector3> _motherPosList = new List<Vector3>();
        public List<Vector3> MotherPos
        {
            get
            {
                UpdatePlayerPos(_contexts.ui.uI.MotherIdList, ref _motherPosList);

                return _motherPosList;
            }
        }
        private List<Vector3> _heroPosList = new List<Vector3>();

        private void UpdatePlayerPos(List<long> idList, ref List<Vector3> posList)
        {
            posList.Clear();
            if (idList.Count == 0)
            {
                return;
            }
            foreach (PlayerEntity pe in _contexts.player.GetEntities())
            {
                if (idList.Contains(pe.playerInfo.PlayerId))
                {
                    posList.Add(new MapFixedVector3(pe.position.FixedVector3).ShiftedUIVector3());
                }
            }

        }

        public List<Vector3> HeroPos
        {
            get
            {
                UpdatePlayerPos(_contexts.ui.uI.HeroIdList, ref _heroPosList);

                return _heroPosList;
            }
        }

        private List<Vector3> _humanPosList = new List<Vector3>();

        public List<Vector3> HumanPos
        {
            get
            {
                UpdatePlayerPos(_contexts.ui.uI.HumanIdList, ref _humanPosList);

                return _humanPosList;
            }
        }

        public Dictionary<string, MapFixedVector3> SupplyPos
        {
            get
            {
                return _contexts.ui.map.SupplyPosMap;
            }
        }

        #endregion
    }
}

