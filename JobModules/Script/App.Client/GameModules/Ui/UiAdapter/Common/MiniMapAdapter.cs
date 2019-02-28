using Assets.Sources.Free.Effect;
using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.System;
using App.Shared.Components.Ui;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Assets.Sources.Free.Auto;
using UnityEngine;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Utils.Singleton;

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
            get { return TerrainCommonData.size.x; }
        }

        public Vector2 MapShowSizeByRice {
            get { return _mapShowSizeByRice; }
        }
        public Vector2 MapRealSizeByRice {
            get { return TerrainCommonData.size; }
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

        

        public float MoveSpeed
        {
            get
            {
                if (IsOnVehicle)
                {
                    return CurVehicleSpeed;
                }
                return _contexts.player.flagSelfEntity.playerMove.Velocity.magnitude * 3.6f;
            }
        }

        private bool IsOnVehicle
        {
            get { return null != _contexts.player.flagSelfEntity && _contexts.player.flagSelfEntity.IsOnVehicle(); }
        }

        private float CurVehicleSpeed
        {
            get
            {
                var vehicle = _contexts.vehicle.GetEntityWithEntityKey(_contexts.player.flagSelfEntity.controlledVehicle.EntityKey);
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
                return CurPlayer.Pos;
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
                        _currInfo.Center = new Vector2(circle.model3D.x - TerrainCommonData.leftMinPos.x, circle.model3D.z - TerrainCommonData.leftMinPos.z);
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
                AirPlaneData planeData = _contexts.ui.map.PlaneData;
                if (planeData == null)
                {
                    planeData = new AirPlaneData(1, new Vector2(0, 0), 0);
                }

                Vector3 leftMinPos = TerrainCommonData.leftMinPos;

                FreeRenderObject plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane");
                if (plane == null)
                {                   
                    plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane1");
                    if (plane != null)
                    {
                        planeData.Type = 1;
                        planeData.Pos = new Vector2(plane.model3D.x - leftMinPos.x, plane.model3D.z - leftMinPos.z);
                        planeData.Direction = -plane.GetEffect(0).EffectModel3D.model3D.rotationY;
                    }
                }
                else
                {
                    planeData.Type = 2;
                    planeData.Pos = new Vector2(plane.model3D.x - leftMinPos.x, plane.model3D.z - leftMinPos.z);
                    planeData.Direction = -plane.GetEffect(0).EffectModel3D.model3D.rotationY;
                }

                if (plane == null)
                {
                    planeData.Type = 0;
                    planeData.Pos = new Vector2(0, 0);
                    planeData.Direction = 0;
                }

                return planeData;
            }
        }

        public List<Vector2> KongTouList()
        {
            int planeId = 0;
            FreeRenderObject plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane1");
            if (plane != null)
            {
                IAutoValue id = plane.GetAuto("id");
                if (id != null && id is AutoConstValue)
                {
                    planeId = int.Parse(((AutoConstValue) id).GetValue());
                }
            }

            kongTouList.Clear();
            Vector3 leftMinPos = TerrainCommonData.leftMinPos;

            foreach (FreeRenderObject drop in SingletonManager.Get<FreeEffectManager>().FreeEffects.Values)
            {
                if (drop.key.StartsWith("dropBox"))
                {
                    IAutoValue id = drop.GetAuto("id");
                    if (id != null && id is AutoConstValue)
                    {
                        int realId = int.Parse(((AutoConstValue)id).GetValue());
                        if (realId == planeId)
                        {
                            kongTouList.Add(new Vector2(drop.model3D.x - leftMinPos.x, drop.model3D.z - leftMinPos.z));
                        }
//                        Debug.LogFormat("********************************** planeId:{0} dropId:{1}", planeId, realId);
                    }
                }
            }
            
            return kongTouList;
        }

        private List<Vector2> kongTouList = new List<Vector2>() { new Vector2(10, 10), new Vector2(15, 15) };
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

        public void AddMapMark(long playerId, int playerNum, float mx, float my)
        {
            _contexts.ui.map.AddMapMark(playerId, playerNum, mx, my);
        }

        public void RemoveMapMark(long playerId)
        {
            _contexts.ui.map.RemoveMapMark(playerId);
        }

        public Dictionary<long, MiniMapPlayMarkInfo> MapMarks { get{return _contexts.ui.map.MapMarks;} }

        #endregion


        #region 航线
        public bool IsShowRouteLine    //是否显示航线
        {
            get { return _contexts.ui.map.IsShowRouteLine; }
        }

        public Vector2 RouteLineStartPoint         //跳伞开始位置
        {
            get { return _contexts.ui.map.RouteLineStartPoint; }
        }

        public Vector2 RouteLineEndPoint    //跳伞结束位置
        {
            get { return _contexts.ui.map.RouteLineEndPoint; }
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
                return _contexts.ui.blast.C4DropPosition - TerrainCommonData.leftMinPos;
            }
        }
    }
}

