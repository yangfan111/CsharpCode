using Core.EntityComponent;
using System;
using UnityEngine;

namespace Core.Statistics
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class DebugInfo : Attribute
    {
        public string ShowInfo;
        public DebugInfo(string showInfo)
        {
            ShowInfo = showInfo; 
        }
    }

    public class StatisticsData
    {
        public bool DataCollectSwitch;
        /// <summary>
        /// 击杀人数
        /// </summary>
        public int KillCount;
        /// <summary>
        /// 击杀队友人数
        /// </summary>
        public int KillTeamCount;
        /// <summary>
        /// 爆头击杀人数
        /// </summary>
        public int CritKillCount;
        /// <summary>
        /// 最大击杀距离
        /// </summary>
        public float MaxKillDistance;
        /// <summary>
        /// 救援人数（复活数）
        /// </summary>
        public int SaveCount;
        /// <summary>
        /// 存活时间（second）
        /// </summary>
        public int AliveTime;

        /// <summary>
        /// 游戏时间（second）
        /// </summary>
        public int GameTime;

        /// <summary>
        /// 助攻数
        /// </summary>
        public int AssistCount;
        /// <summary>
        /// 死亡顺序
        /// </summary>
        public int DeathOrder;

        /// <summary>
        /// 击倒次数
        /// </summary>
        public int HitDownCount;
        /// <summary>
        /// 有效伤害
        /// </summary>
        public float PlayerDamage;
        /// <summary>
        /// 总伤害量（包括载具、受伤）
        /// </summary>
        public float TotalDamage;
        /// <summary>
        /// 总开枪数
        /// </summary>
        public int ShootingCount;
        /// <summary>
        /// 总命中数（包括载具、门、受伤）
        /// </summary>
        public int ShootingSuccCount;
        /// <summary>
        /// 有效命中数（玩家正常状态下）
        /// </summary>
        public int ShootingPlayerCount;
        /// <summary>
        /// 总爆头数
        /// </summary>
        public int CritCount;
        /// <summary>
        /// 总移动距离（只算水平距离）
        /// </summary>
        public float TotalMoveDistance;
        /// <summary>
        /// 载具移动距离（只算水平距离）
        /// </summary>
        public float VehicleMoveDistance;
        /// <summary>
        /// 治疗量
        /// </summary>
        public float CureVolume;
        /// <summary>
        /// 加速时间ms
        /// </summary>
        public int AccSpeedTime;
        /// <summary>
        /// 承受伤害量
        /// </summary>
        public float TotalBeDamage;
        /// <summary>
        /// 减免伤害量
        /// </summary>
        public float DefenseDamage;
        /// <summary>
        /// 摧毁载具数量
        /// </summary>
        public int DestroyVehicle;
        /// <summary>
        /// 投掷型物品使用数量
        /// </summary>
        public int UseThrowingCount;
        /// <summary>
        /// 全副武装
        /// </summary>
        public bool IsFullArmed;
        /// <summary>
        /// 连杀数量
        /// </summary>
        public int EvenKillCount;
        /// <summary>
        /// 最大连杀数量
        /// </summary>
        public int MaxEvenKillCount;
        /// <summary>
        /// 被救援次数
        /// </summary>
        public int BeSaveCount;
        /// <summary>
        /// 游泳时间ms
        /// </summary>
        public int SwimTime;

        /// <summary>
        /// 拿下全场1血
        /// </summary>
        public bool GetFirstBlood;
        /// <summary>
        /// 手枪击杀数
        /// </summary>
        public int PistolKillCount;
        /// <summary>
        /// 手雷击杀数
        /// </summary>
        public int GrenadeKillCount;

        /// <summary>
        /// 存活圈数
        /// </summary>
        public int AliveCircle;
        /// <summary>
        /// 死亡次数
        /// </summary>
        public int DeadCount;
        /// <summary>
        /// 击杀者使用武器
        /// </summary>
        public int KillerWeaponId;
        //击杀距离
        //跳伞落地点
        //阵亡地点
        //获胜时地点

        /// <summary>
        /// 被载具击杀
        /// </summary>
        public bool KillByVehicle;
        /// <summary>
        /// 被玩家击杀
        /// </summary>
        public bool KillByPlayer;
        /// <summary>
        /// 被轰炸击杀
        /// </summary>
        public bool KillByAirBomb;
        /// <summary>
        /// 淹死
        /// </summary>
        public bool Drown;
        /// <summary>
        /// 摔死
        /// </summary>
        public bool DropDead;
        /// <summary>
        /// 毒死
        /// </summary>
        public bool PoisionDead;

        /// <summary>
        /// 步枪击杀
        /// </summary>
        public int KillWithRifle;
        /// <summary>
        /// 机枪击杀
        /// </summary>
        public int KillWithMachineGun;
        /// <summary>
        /// 被冲锋枪击杀
        /// </summary>
        public int KillWithSubmachineGun;
        /// <summary>
        /// 狙击枪击杀
        /// </summary>
        public int KillWithSniper;
        /// <summary>
        /// 投掷武器击杀
        /// </summary>
        public int KillWithThrowWeapon;
        /// <summary>
        /// 近战武器击杀
        /// </summary>
        public int KillWithMelee;
        /// <summary>
        /// 霰弹枪击杀
        /// </summary>
        public int KillWithShotGun;
        /// <summary>
        /// 手枪击杀
        /// </summary>
        public int KillWithPistol;
        /// <summary>
        /// 是否逃跑
        /// </summary>
        public bool IsRunaway;

        //正常状态下，最后击伤我的
        public bool IsHited;
        public EntityKey LastHurtKey;
        public int LastHurtType;    //EUIDeadType
        public int LastHurtPart;
        public int LastHurtWeaponId;

        //统计变量
        public Vector3 LastPosition;
        public bool LastIsSwimState;
        public int LastSwimTime;
        public int LastSamplingTime;
        /// <summary>
        /// 上次击杀的时间
        /// </summary>
        public int LastKillTime;

        /// <summary>
        /// 死亡的持续时间
        /// </summary>
        public int DeadTime;
  
        /// <summary>
        /// 上次死亡的时间
        /// </summary>
        public int LastDeadTime;

        public int Rank;

        public int C4PlantCount;
        public int C4DefuseCount;
        public bool HasC4;
        public long RevengeKillerId;
    }
}
