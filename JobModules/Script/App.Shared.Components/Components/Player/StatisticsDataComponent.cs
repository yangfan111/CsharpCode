using Core.Components;
using Core.EntityComponent;
using Core.Enums;
using Core.Statistics;
using Entitas;

namespace App.Shared.Components.Player
{
    [Player]
    
    public class StatisticsDataComponent : IComponent, IResetableComponent
    {
        //是否显示
        public bool IsShow;
        
        //战斗数据
        public BattleData Battle;
        //统计数据
        public StatisticsData Statistics;

        public int GetComponentId()
        {
            { { return (int)EComponentIds.PlayerStatisticsData; } }
        }

        public void Reset()
        {
            if (null == Battle)
            {
                Battle = new BattleData();
            }
            else
            {
                Battle.Reset();
            }
            if (null == Statistics)
            {
                Statistics = new StatisticsData();
            }
        }

        private void SetBattleInfo(EntityKey key, int weaponId, PlayerInfoComponent playerInfo, PlayerBattleInfo battleInfo)
        {
            battleInfo.PlayerKey = key;
            battleInfo.PlayerLv = playerInfo.Level;
            battleInfo.PlayerName = playerInfo.PlayerName;
            battleInfo.BackId = playerInfo.BackId;
            battleInfo.TitleId = playerInfo.TitleId;
            battleInfo.BadgeId = playerInfo.BadgeId;
            battleInfo.WeaponId = weaponId;
        }

        public void AddOpponentInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, int damage, PlayerInfoComponent playerInfo)
        {
            OpponentBattleInfo opponent;
            Battle.OpponentDict.TryGetValue(key, out opponent);
            if (null == opponent)
            {
                opponent = new OpponentBattleInfo();
                Battle.OpponentDict.Add(key, opponent);
            }
            if (!opponent.IsKill)
                opponent.IsKill = isKill;
            if (!opponent.IsHitDown)
                opponent.IsHitDown = isHitDown;
            opponent.Damage += damage;
            SetBattleInfo(key, weaponId, playerInfo, opponent);
        }

        public void AddKillerInfo(EntityKey key, int weaponId, int deadType, PlayerInfoComponent playerInfo)
        {
            SetBattleInfo(key, weaponId, playerInfo, Battle.Killer);
            SetDeadType(deadType);
        }

        public void AddOtherInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, int damage, PlayerInfoComponent playerInfo)
        {
            OpponentBattleInfo other;
            Battle.OtherDict.TryGetValue(key, out other);
            if (null == other)
            {
                other = new OpponentBattleInfo();
                Battle.OtherDict.Add(key, other);
            }
            if (!other.IsKill)
                other.IsKill = isKill;
            if (!other.IsHitDown)
                other.IsHitDown = isHitDown;
            other.Damage += damage;
            SetBattleInfo(key, weaponId, playerInfo, other);
        }

        public bool BeKilledOrHitDown(EntityKey key)
        {
            OpponentBattleInfo other;
            Battle.OtherDict.TryGetValue(key, out other);
            if (null != other && (other.IsKill || other.IsHitDown))
            {
                return true;
            }
            return false;
        }

        public void SetDeadType(int deadType)
        {
            Battle.Killer.DeadType = (EUIDeadType)deadType;
            SaveHisData();
        }

        public void SaveHisData()
        {
            foreach (var opponent in Battle.OpponentDict.Values)
            {
                Battle.OpponentList.Add(opponent);
            }
            Battle.OpponentDict.Clear();
        }
    }
}
