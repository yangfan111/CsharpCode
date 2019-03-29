using System.Collections.Generic;
using Core.EntityComponent;
using Core.Enums;

namespace Core.Statistics
{
    public class BattleData
    {
        //击杀者（击杀我的人）
        public PlayerBattleInfo Killer = new PlayerBattleInfo();
        //本场对手数据（我对别人造成的伤害）
        public Dictionary<int, OpponentBattleInfo> OpponentDict = new Dictionary<int, OpponentBattleInfo>();
        //历史对手数据（我对别人造成的伤害）
        public List<OpponentBattleInfo> OpponentList = new List<OpponentBattleInfo>();

        //别人对我造成的伤害（助攻用）
        public Dictionary<EntityKey, OpponentBattleInfo> OtherDict = new Dictionary<EntityKey, OpponentBattleInfo>(new EntityKeyComparer());

        public void Reset()
        {
            Killer = new PlayerBattleInfo();
            OpponentDict.Clear();
            OpponentList.Clear();
            OtherDict.Clear();
        }
    }

    public class PlayerBattleInfo
    {
        public EntityKey PlayerKey;
        public int PlayerLv;
        public string PlayerName = "";
        public int BackId;
        public int TitleId;
        public int BadgeId;
        public int WeaponId;
	    public EUIDeadType DeadType;
        public long timestamp;
    }

    public class OpponentBattleInfo : PlayerBattleInfo
    {
        public bool IsKill;
        public bool IsHitDown;
        public float TrueDamage;
        public int Damage;
        public int DeathCount;
    }
}
