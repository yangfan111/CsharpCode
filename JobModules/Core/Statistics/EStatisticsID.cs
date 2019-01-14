namespace Core.Statistics
{
    public class EStatisticsID
    {
        /// <summary>
        /// 排名
        /// </summary>
        public const int Rank = 1;
        /// <summary>
        /// 排名第一
        /// </summary>
        public const int RankAce = 2;
        /// <summary>
        /// 排名前十 
        /// </summary>
        public const int RankTen = 3;
        /// <summary>
        /// 击杀人数
        /// </summary>
        public const int KillCount = 4;
        /// <summary>
        /// 击倒人数
        /// </summary>
        public const int HitDownCount = 5;
        /// <summary>
        /// 有效伤害
        /// </summary>
        public const int PlayerDamage = 6;
        /// <summary>
        /// 总伤害量
        /// </summary>
        public const int TotalDamage = 7;
        /// <summary>
        /// 生存时长
        /// </summary>
        public const int AliveTime = 8;
        /// <summary>
        /// 总开枪数 
        /// </summary>
        public const int ShootingCount = 9;
        //public const int AliveCircle = 7;
        /// <summary>
        /// 总命中数
        /// </summary>
        public const int ShootingSuccCount = 10;
        /// <summary>
        /// 有效命中数
        /// </summary>
        public const int ShootingPlayerCount = 11;
        /// <summary>
        /// 总爆头数
        /// </summary>
        public const int CritCount = 12;
        /// <summary>
        /// 总移动距离
        /// </summary>
        public const int TotalMoveDistance = 13;
        /// <summary>
        /// 载具移动距离
        /// </summary>
        public const int VehicleMoveDistance = 14;
        /// <summary>
        /// 助攻数量
        /// </summary>
        public const int AssistCount = 15;
        /// <summary>
        /// 治疗量
        /// </summary>
        public const int CureVolume = 16;
        /// <summary>
        /// 加速时间
        /// </summary>
        public const int AccSpeedTime = 17;
        /// <summary>
        /// 复活数
        /// </summary>
        public const int SaveCount = 18;
        /// <summary>
        /// 承受伤害量
        /// </summary>
        public const int TotalBeDamage = 19;
        /// <summary>
        /// 减免伤害量
        /// </summary>
        public const int DefenseDamage = 20;
        /// <summary>
        /// 死亡次数
        /// </summary>
        public const int DeadCount = 21;
        /// <summary>
        /// 击杀距离 
        /// </summary>
        public const int KillDistance = 22;
        /// <summary>
        /// 摧毁载具数量
        /// </summary>
        public const int DestroyVehicle = 23;
        /// <summary>
        /// 投掷型物品使用数量
        /// </summary>
        public const int UseThrowingCount = 24;
        /// <summary>
        /// 单据存活时间
        /// </summary>
        public const int SurvivalTime = 25;
        /// <summary>
        /// 全副武装
        /// </summary>
        public const int IsFullArmed = 26;
        /// <summary>
        /// 连杀数量
        /// </summary>
        public const int EvenKillCount = 27;
        /// <summary>
        /// 游泳时间
        /// </summary>
        public const int SwimTime = 28;
        /// <summary>
        /// 队伍击杀数
        /// </summary>
        public const int TeamKillCount = 29;
        /// <summary>
        /// 场次
        /// </summary>
        public const int GameCount = 30;
        /// <summary>
        /// 淹死
        /// </summary>
        public const int Drown = 31;
        /// <summary>
        /// 毒死
        /// </summary>
        public const int PoisionDead = 32;
        /// <summary>
        /// 摔死
        /// </summary>
        public const int DropDead = 33;
        /// <summary>
        /// 被载具击杀
        /// </summary>
        public const int KillByVehicle = 34;
        /// <summary>
        /// 被玩家击杀 
        /// </summary>
        public const int KillByPlayer = 35;
        /// <summary>
        /// 被轰炸击杀
        /// </summary>
        public const int KillByAirBomb = 36;
        /// <summary>
        /// 游戏时长
        /// </summary>
        public const int GameTime = 37;
        /// <summary>
        /// 死亡时长
        /// </summary>
        public const int DeadTime = 38;

        /// <summary>
        /// 被步枪类武器击杀
        /// </summary>
        public const int KillByRifle = 1000;
        /// <summary>
        /// 被狙击枪类武器击杀
        /// </summary>
        public const int KillBySniper = 2000;
        /// <summary>
        /// 被冲锋枪类武器击杀
        /// </summary>
        public const int KillBySubMachineGun = 3000;
        /// <summary>
        /// 被机枪类武器击杀
        /// </summary>
        public const int KillByMachineGun = 4000;
        /// <summary>
        /// 被霰弹枪类武器击杀
        /// </summary>
        public const int KillByShotGun = 5000;
        /// <summary>
        /// 被手枪类武器击杀
        /// </summary>
        public const int KillByPistol = 6000;
        /// <summary>
        /// 被投掷类武器击杀
        /// </summary>
        public const int KillByThrowWeapon = 7000;
        /// <summary>
        /// 被近战类武器击杀
        /// </summary>
        public const int KillByMelee = 8000;
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
