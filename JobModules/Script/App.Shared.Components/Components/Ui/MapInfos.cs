using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Components.Ui
{
    public struct MiniMapPlayMarkInfo
    {
        private Vector2 pos;     //米   只用x 和 z
        private int num;         //对应的玩家编号 用来去颜色的
        public MiniMapPlayMarkInfo(Vector2 pos, int num)
        {
            this.pos = pos;
            this.num = num;
        }
        public Vector2 Pos
        {
            get
            {
                return pos;
            }

            set
            {
                pos = value;
            }
        }

        public int Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
            }
        }
    }

    public enum MapLevel
    {
        None = 0,
        Min = 1,
        Medium = 2,
        Max = 3,
    }

    public enum MiniMapPlayStatue
    {
        NONE,
        NORMAL,
        TIAOSAN,
        ZAIJU,
        HURTED,
        DEAD
    }

    public class MiniMapTeamPlayInfo
    {
        public MiniMapTeamPlayInfo(long playerId,int entityId, bool isPlayer, int num, Color color, MiniMapPlayStatue statue, Vector2 pos, float faceDirection, List<MiniMapPlayMarkInfo> markList, bool isShooting, int shootingCount,
            string playerName, int curHp, int maxHp, int curHpInHurted, bool isMark, Vector3 topPos)
        {
            this.PlayerId = playerId;
            this.EntityId = entityId;
            this.IsPlayer = isPlayer;
            this.Num = num;
            this.Color = color;
            this.Statue = statue;
            this.Pos = pos;
            this.FaceDirection = faceDirection;
            this.MarkList = markList;
            this.IsShooting = isShooting;
            this.ShootingCount = shootingCount;
            this.PlayerName = playerName;
            this.CurHp = curHp;
            this.MaxHp = maxHp;
            this.CurHpInHurted = curHpInHurted;
            this.IsMark = isMark;
            this.TopPos = topPos;
        }

        public MiniMapTeamPlayInfo()
        { }


        public long PlayerId;                       //玩家ID
        public int EntityId;
        public bool IsPlayer;                       //是不是玩家本人
        public int Num;                             //0 就是不现实编号
        public Color Color;                         //小队颜色
        public MiniMapPlayStatue Statue;            // 1 常态 2跳伞 3载具 4受伤 5死亡
        public Vector2 Pos;                         //只用到  真实世界的 x和z的坐标
        public float FaceDirection;                 //0 到 360 
        public List<MiniMapPlayMarkInfo> MarkList;  //标记列表
        public bool IsShooting;                     //是否在开枪
        public int ShootingCount;                   //打了多少枪

        public string PlayerName;                   //玩家名
        public int CurHp;                           //当前血量
        public int MaxHp;                           //最大血量
        public int CurHpInHurted;                   //受伤血量
        public bool IsMark;                         //是否标记
        public Vector3 TopPos;                      //人物名字位置

        //是否受伤状态
        public bool IsInHurtedStatue
        {
            get { return Statue == MiniMapPlayStatue.HURTED; }
        }

        //是否死亡状态
        public bool IsDead
        {
            get { return Statue == MiniMapPlayStatue.DEAD; }
        }

    }

    public class BombAreaInfo   //轰炸区数据
    {
        private float num;
        private Vector3 center;
        private float radius;

        public Vector3 Center
        {
            get
            {
                return center;
            }

            set
            {
                center = value;
            }
        }

        public float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                radius = value;
            }
        }

        public float Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
            }
        }

        public BombAreaInfo(Vector3 center, float radius, float num)
        {
            this.center = center;
            this.radius = radius;
            this.num = num;
        }
    }

    public class AirPlaneData
    {
        private int type;  //0 无飞机 //1 空投机  2运输机
        private Vector2 pos;  //3d世界的x和z 坐标值
        private float direction; //飞机的朝向

        public AirPlaneData() { }
        public AirPlaneData(int type, Vector2 pos, float direction)
        {
            this.type = type;
            this.pos = pos;
            this.direction = direction;
        }

        public int Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public Vector2 Pos
        {
            get
            {
                return pos;
            }

            set
            {
                pos = value;
            }
        }

        public float Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }
    }

    public class DuQuanInfo     //毒圈 数据
    {
        private int level;
        Vector2 center;
        float radius;
        float waitTime;      //毒圈等待时间
        float moveTime;      //毒圈移动时间

        public Vector2 Center
        {
            get
            {
                return center;
            }

            set
            {
                center = value;
            }
        }

        public float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                radius = value;
            }
        }

        public float WaitTime
        {
            get
            {
                return waitTime;
            }

            set
            {
                waitTime = value;
            }
        }

        public float MoveTime
        {
            get
            {
                return moveTime;
            }

            set
            {
                moveTime = value;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        // centerX radius 是实际的长  米单位
        public DuQuanInfo(int level, UnityEngine.Vector2 pos, float radius, float waitTime, float moveTime)
        {
            this.level = level;
            this.center = pos;
            this.radius = radius;
            this.waitTime = waitTime;
            this.moveTime = moveTime;
        }

        public void SetValue(int _level, Vector2 _center, float _radius, float _waitTime, float _moveTime)
        {
            this.level = _level;
            this.center = _center;
            this.radius = _radius;
            this.waitTime = _waitTime;
            this.moveTime = _moveTime;
        }
    }

    public struct TeamPlayerMarkInfo
    {
        private float angel;
        private Color markColor;

        public TeamPlayerMarkInfo(float angel, Color markColor)
        {
            this.angel = angel;
            this.markColor = markColor;
        }

        public float Angel
        {
            get
            {
                return angel;
            }

            set
            {
                angel = value;
            }
        }

        public Color MarkColor
        {
            get
            {
                return markColor;
            }

            set
            {
                markColor = value;
            }
        }
    }
}