namespace Core.Statistics
{
    public enum EStatisticsID
    {
        /// <summary>
        /// 排名
        /// </summary>
        Rank = 1,
        /// <summary>
        /// 排名第一
        /// </summary>
        RankAce = 2,
        /// <summary>
        /// 排名前十 
        /// </summary>
        RankTen = 3,
        /// <summary>
        /// 击杀人数
        /// </summary>
        KillCount = 4,
        /// <summary>
        /// 击倒人数
        /// </summary>
        HitDownCount = 5,
        /// <summary>
        /// 有效伤害
        /// </summary>
        PlayerDamage = 6,
        /// <summary>
        /// 总伤害量
        /// </summary>
        TotalDamage = 7,
        /// <summary>
        /// 生存时长
        /// </summary>
        AliveTime = 8,
        /// <summary>
        /// 总开枪数 
        /// </summary>
        ShootingCount = 9,
        /// <summary>
        /// 总命中数
        /// </summary>
        ShootingSuccCount = 10,
        /// <summary>
        /// 有效命中数
        /// </summary>
        ShootingPlayerCount = 11,
        /// <summary>
        /// 总爆头数
        /// </summary>
        CritCount = 12,
        /// <summary>
        /// 总移动距离
        /// </summary>
        TotalMoveDistance = 13,
        /// <summary>
        /// 载具移动距离
        /// </summary>
        VehicleMoveDistance = 14,
        /// <summary>
        /// 助攻数量
        /// </summary>
        AssistCount = 15,
        /// <summary>
        /// 治疗量
        /// </summary>
        CureVolume = 16,
        /// <summary>
        /// 加速时间
        /// </summary>
        AccSpeedTime = 17,
        /// <summary>
        /// 复活数
        /// </summary>
        SaveCount = 18,
        /// <summary>
        /// 承受伤害量
        /// </summary>
        TotalBeDamage = 19,
        /// <summary>
        /// 减免伤害量
        /// </summary>
        DefenseDamage = 20,
        /// <summary>
        /// 死亡次数
        /// </summary>
        DeadCount = 21,
        /// <summary>
        /// 击杀距离 
        /// </summary>
        KillDistance = 22,
        /// <summary>
        /// 摧毁载具数量
        /// </summary>
        DestroyVehicle = 23,
        /// <summary>
        /// 投掷型物品使用数量
        /// </summary>
        UseThrowingCount = 24,
        /// <summary>
        /// 单局存活时间
        /// </summary>
        SurvivalTime = 25,
        /// <summary>
        /// 全副武装
        /// </summary>
        IsFullArmed = 26,
        /// <summary>
        /// 连杀数量
        /// </summary>
        EvenKillCount = 27,
        /// <summary>
        /// 游泳时间
        /// </summary>
        SwimTime = 28,
        /// <summary>
        /// 队伍击杀数
        /// </summary>
        TeamKillCount = 29,
        /// <summary>
        /// 场次
        /// </summary>
        GameCount = 30,
        /// <summary>
        /// 淹死
        /// </summary>
        Drown = 31,
        /// <summary>
        /// 毒死
        /// </summary>
        PoisionDead = 32,
        /// <summary>
        /// 摔死
        /// </summary>
        DropDead = 33,
        /// <summary>
        /// 被载具击杀
        /// </summary>
        KillByVehicle = 34,
        /// <summary>
        /// 被玩家击杀 
        /// </summary>
        KillByPlayer = 35,
        /// <summary>
        /// 被轰炸击杀
        /// </summary>
        KillByAirBomb = 36,
        /// <summary>
        /// 游戏时长
        /// </summary>
        GameTime = 37,
        /// <summary>
        /// 死亡时长
        /// </summary>
        DeadTime = 38,
        /// <summary>
        /// C4成功放置次数
        /// </summary>
        C4SetCount = 39,
        /// <summary>
        /// C4成功引爆次数
        /// </summary>
        C4DetonationCount = 40,
        /// <summary>
        /// C4成功拆除次数
        /// </summary>
        C4DefuseCount = 41,
        /// <summary>
        /// 回合数
        /// </summary>
        RoundCount = 42,
        /// <summary>
        /// 胜利
        /// </summary>
        GameWin = 43,
        /// <summary>
        /// 失败
        /// </summary>
        GameLose = 44,
        /// <summary>
        /// 感染人类次数
        /// </summary>
        InfectCount = 45,
        /// <summary>
        /// 变成英雄次数
        /// </summary>
        HeroCount = 46,
        /// <summary>
        /// 释放技能次数
        /// </summary>
        SkillUseCount = 47,
        /// <summary>
        /// 捡补给箱次数
        /// </summary>
        PickupBackupCount = 48,
        /// <summary>
        /// 被感染次数
        /// </summary>
        InfectedCount = 49,
        /// <summary>
        /// 击杀英雄次数
        /// </summary>
        HeroKill = 50,
        /// <summary>
        /// 击杀母体次数
        /// </summary>
        BroodKill = 51,
        /// <summary>
        /// 成为变异体时长
        /// </summary>
        InfectedTime = 52,
        /// <summary>
        /// 成为英雄时长
        /// </summary>
        HeroTime = 53,
        /// <summary>
        /// 解锁等级
        /// </summary>
        UnlockInfectLvl = 54,
        /// <summary>
        /// 生存（人类获胜）
        /// </summary>
        InfectSurvivalWin = 55,
        /// <summary>
        /// 近战攻击数
        /// </summary>
        MeleeAttackCount = 56,
        /// <summary>
        /// 近战命中数
        /// </summary>
        MeleeHitCount = 57,
        /// <summary>
        /// 有效近战命中数
        /// </summary>
        MeleeHit = 58,
        /// <summary>
        /// 近战爆头数
        /// </summary>
        MeleeCrit = 59,
        /// <summary>
        /// 变异胜利
        /// </summary>
        WinAsInfected = 60,
        /// <summary>
        /// 首杀
        /// </summary>
        GetFirstBlood = 61,
        /// <summary>
        /// 爆头击杀数
        /// </summary>
        CritKillCount = 62,
        /// <summary>
        /// 平局
        /// </summary>
        Draw = 63,
        /// <summary>
        /// 治疗队友
        /// </summary>
        HealVolume = 64,
        /// <summary>
        /// 守护C4时击杀敌人人数
        /// </summary>
        C4ProtectKill = 65,

        /// <summary>
        /// 存活毒圈数
        /// </summary>
        AliveCircle = 116,
        /// <summary>
        /// 单局参加人数
        /// </summary>
        SectionPlayerCount = 117,
        /// <summary>
        /// 模式队伍人数
        /// </summary>
        ModePlayerCount = 118,
        /// <summary>
        /// 普通击杀数
        /// </summary>
        NormalKills = 119,

        /// <summary>
        /// 被步枪类武器击杀
        /// </summary>
        KillByRifle = 1000,
        /// <summary>
        /// 被狙击枪类武器击杀
        /// </summary>
        KillBySniper = 2000,
        /// <summary>
        /// 被冲锋枪类武器击杀
        /// </summary>
        KillBySubMachineGun = 3000,
        /// <summary>
        /// 被机枪类武器击杀
        /// </summary>
        KillByMachineGun = 4000,
        /// <summary>
        /// 被霰弹枪类武器击杀
        /// </summary>
        KillByShotGun = 5000,
        /// <summary>
        /// 被手枪类武器击杀
        /// </summary>
        KillByPistol = 6000,
        /// <summary>
        /// 被投掷类武器击杀
        /// </summary>
        KillByThrowWeapon = 7000,
        /// <summary>
        /// 被近战类武器击杀
        /// </summary>
        KillByMelee = 8000,
    }

    public class EHonorID
    {
        public const int HuntingMaster = 1001;
        public const int GoodDriver = 1002;
        public const int FirstBlood = 1003;
        public const int GoodSwimmer = 1004;
        public const int FullArmed = 1005;
        public const int OutputExpert = 1006;
        public const int Nanny = 1007;
        public const int WestCowboy = 1008;
        public const int ThunderGod = 1009;
    }
}
