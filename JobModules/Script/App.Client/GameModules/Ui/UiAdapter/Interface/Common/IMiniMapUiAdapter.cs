using Entitas;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Ui.Map;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
//    public struct MiniMapPlayMarkInfo
//    {
//        private Vector2 pos;     //米   只用x 和 z
//        private int num;         //对应的玩家编号 用来去颜色的
//        public MiniMapPlayMarkInfo(Vector2 pos, int num)
//        {
//            this.pos = pos;    
//            this.num = num;
//        }
//        public Vector2 Pos
//        {
//            get
//            {
//                return pos;
//            }
//
//            set
//            {
//                pos = value;
//            }
//        }
//
//        public int Num
//        {
//            get
//            {
//                return num;
//            }
//
//            set
//            {
//                num = value;
//            }
//        }
//    }
//
//    public enum MiniMapPlayStatue
//    {
//        NONE,
//        NORMAL,
//        TIAOSAN,
//        ZAIJU,
//        HURTED,
//        DEAD
//    }
//
//    public class MiniMapTeamPlayInfo
//    {
//        public MiniMapTeamPlayInfo(long playerId, bool isPlayer, int num, Color color, MiniMapPlayStatue statue, Vector2 pos, float faceDirection, List<MiniMapPlayMarkInfo> markList, bool isShooting, int shootingCount,
//            string playerName, int curHp, int maxHp, int curHpInHurted, bool isMark, Vector3 topPos)
//        {
//            this.PlayerId = playerId;
//            this.IsPlayer = isPlayer;
//            this.Num = num;
//            this.Color = color;
//            this.Statue = statue;
//            this.Pos = pos;
//            this.FaceDirection = faceDirection;
//            this.MarkList = markList;
//            this.IsShooting = isShooting;
//            this.ShootingCount = shootingCount;
//            this.PlayerName = playerName;
//            this.CurHp = curHp;
//            this.MaxHp = maxHp;
//            this.CurHpInHurted = curHpInHurted;
//            this.IsMark = isMark;
//            this.TopPos = topPos;
//        }
//
//        public long PlayerId;                       //玩家ID
//        public bool IsPlayer;                       //是不是玩家本人
//        public int Num;                             //0 就是不现实编号
//        public Color Color;                         //小队颜色
//        public MiniMapPlayStatue Statue;            // 1 常态 2跳伞 3载具 4受伤 5死亡
//        public Vector2 Pos;                         //只用到  真实世界的 x和z的坐标
//        public float FaceDirection;                 //0 到 360 
//        public List<MiniMapPlayMarkInfo> MarkList;  //标记列表
//        public bool IsShooting;                     //是否在开枪
//        public int ShootingCount;                   //打了多少枪
//
//        public string PlayerName;                   //玩家名
//        public int CurHp;                           //当前血量
//        public int MaxHp;                           //最大血量
//        public int CurHpInHurted;                   //受伤血量
//        public bool IsMark;                         //是否标记
//        public Vector3 TopPos;                      //人物名字位置
//
//        //是否受伤状态
//        public bool IsInHurtedStatue
//        {
//            get { return Statue == MiniMapPlayStatue.HURTED; }
//        }
//
//        //是否死亡状态
//        public bool IsDead
//        {
//            get { return Statue == MiniMapPlayStatue.DEAD; }
//        }
//    }
//
//    public class BombAreaInfo   //轰炸区数据
//    {
//        private float num;
//        private Vector3 center;
//        private float radius;
//
//        public Vector3 Center
//        {
//            get
//            {
//                return center;
//            }
//
//            set
//            {
//                center = value;
//            }
//        }
//
//        public float Radius
//        {
//            get
//            {
//                return radius;
//            }
//
//            set
//            {
//                radius = value;
//            }
//        }
//
//        public float Num
//        {
//            get
//            {
//                return num;
//            }
//
//            set
//            {
//                num = value;
//            }
//        }
//
//        public BombAreaInfo(Vector3 center, float radius, float num)
//        {
//            this.center = center;
//            this.radius = radius;
//            this.num = num;
//        }
//
//        public BombAreaInfo()
//        {
//            this.center = Vector2.zero;
//            this.radius = 0;
//            this.num = -2;  //-2 不能修改 这里用来线上线下区别的
//        }
//    }
//
//    public class AirPlaneData
//    {
//        private int type;  //0 无飞机 //1 空投机  2运输机
//        private Vector2 pos;  //3d世界的x和z 坐标值
//        private float direction; //飞机的朝向
//
//        public AirPlaneData(int type, Vector2 pos, float direction)
//        {
//            this.type = type;
//            this.pos = pos;
//            this.direction = direction;
//        }
//
//        public int Type
//        {
//            get
//            {
//                return type;
//            }
//
//            set
//            {
//                type = value;
//            }
//        }
//
//        public Vector2 Pos
//        {
//            get
//            {
//                return pos;
//            }
//
//            set
//            {
//                pos = value;
//            }
//        }
//
//        public float Direction
//        {
//            get
//            {
//                return direction;
//            }
//
//            set
//            {
//                direction = value;
//            }
//        }
//    }

    public interface IMiniMapUiAdapter : IAbstractUiAdapter
    {
        List<MiniMapTeamPlayInfo> TeamInfos { get;}

        int OffLineNum { get; }
        DuQuanInfo CurDuquan { get;}  //当前毒圈
        DuQuanInfo NextDuquan { get; }  //下一个毒圈
        BombAreaInfo BombArea { get;}  //轰炸区
        MiniMapTeamPlayInfo GetPlayerById(long playerId);   //取玩家
        MiniMapTeamPlayInfo CurPlayer { get; }    //当前玩家
        Vector2 CurPlayerPos { get; }   //当前玩家位置

        AirPlaneData PlaneData { get; } //当前飞机 
        List<MapFixedVector2> KongTouList(); //空投点

        int MapId { get; set; } //地图ID

        void AddMapMark(long playerId, int playerNum, float mx, float my);
        void RemoveMapMark(long playerId);
        Dictionary<long, MiniMapPlayMarkInfo> MapMarks { get; }

        bool IsShowRouteLine { get;}   //是否显示航线
        Vector2 RouteLineStartPoint { get;}    //跳伞开始位置
        Vector2 RouteLineEndPoint { get;}      //跳伞结束位置

        void SetCrossActive(bool isActive);
        void SendMarkMessage(Vector2 markPos);

        float CurMapSize { get; }
        MapLevel MapLevel { get; }
        float MapSegmentingLine { get; }
        Vector2 MapRealSizeByRice { get; }
        Vector2 MapShowSizeByRice { get; }
        Vector2 MapCenterByRice { get; }

        float MoveSpeed { get; }

        bool IsC4Drop { get; }
        Vector3 C4DropPosition { get; }

        GamePlayComponent gamePlay { get; }
    }
}
