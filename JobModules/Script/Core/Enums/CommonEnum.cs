namespace Core.Enums
{

    public enum EUIBombInstallState
    {
        None,
        Installing,
        Completed
    }

    public enum EUIGameResultType
    {
        None,
        Win,
        Lose,
        Tie//平局
    }

    public enum EUIGameTitleType
    {
        None,
        KdKing,
        Ace,
        Second,
        Third,
    }

    public enum EUICampType
    {
        None,
        T,
        CT
    }

    public enum EUIKillType
    {
        //1.爆头
        Crit = 1,
        //2.击倒
        Hit
    }

    public enum EUIDeadType
    {
        Unkown = 0,
        //1:武器击杀
        Weapon = 1,
        //2:摔死
        Fall,
        //3:淹死
        Drown,
        //4:载具爆炸
        VehicleBomb,
        //5:载具击杀
        VehicleHit,
        //6:无人救助死亡
        NoHelp,
        //7:毒圈毒死
        Poison,
        //8:轰炸区炸死
        Bombing,
        //9:空手打死
        Unarmed,
        // 爆破模式炸弹
        Bomb
    }

    //击杀反馈
    public enum EUIKillFeedbackType
    {
        //普通
        Normal = 1,
        //爆头
        CritKill,
        //连杀（吃鸡不要这个）
        EvenKill,
        //穿墙击杀
        ThroughWall,
        //刀杀（近战武器）
        MeleeWeapon,
        //合作击杀（助攻）
        Cooperate,
        //复仇
        Revenge,
        //燃烧瓶击杀
        Burning,
        //手雷击杀
        Grenade,
        //一血
        FirstBlood
    }
}
