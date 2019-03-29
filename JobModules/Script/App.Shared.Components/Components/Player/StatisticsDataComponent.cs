using System.Security.Cryptography.X509Certificates;
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

        private void SetBattleInfo(EntityKey key, int weaponId, PlayerInfoComponent playerInfo, PlayerBattleInfo battleInfo, long timestamp)
        {
            battleInfo.PlayerKey = key;
            battleInfo.PlayerLv = playerInfo.Level;
            battleInfo.PlayerName = playerInfo.PlayerName;
            battleInfo.BackId = playerInfo.BackId;
            battleInfo.TitleId = playerInfo.TitleId;
            battleInfo.BadgeId = playerInfo.BadgeId;
            battleInfo.WeaponId = weaponId;
            battleInfo.timestamp = timestamp;
        }

        public void AddOpponentInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, float damage, PlayerInfoComponent playerInfo, int deathCount)
        {
            OpponentBattleInfo opponent = new OpponentBattleInfo();;
            bool containsFlag = false;
            if (Battle.OpponentDict.Count > 0)
            {
                foreach (OpponentBattleInfo obi in Battle.OpponentDict.Values)
                {
                    if (obi.PlayerKey == key && obi.DeathCount == deathCount)
                    {
                        opponent = obi;
                        containsFlag = true;
                        break;
                    }
                }
            }
            if (!containsFlag)
            {
                Battle.OpponentDict.Add(Battle.OpponentDict.Count, opponent);
            }
            if (!opponent.IsKill) opponent.IsKill = isKill;
            if (!opponent.IsHitDown) opponent.IsHitDown = isHitDown;
            opponent.TrueDamage += damage;
            opponent.Damage = (int) opponent.TrueDamage;
            opponent.DeathCount = deathCount;
            SetBattleInfo(key, weaponId, playerInfo, opponent, 0L);
        }

        public void AddKillerInfo(EntityKey key, int weaponId, int deadType, PlayerInfoComponent playerInfo)
        {
            SetBattleInfo(key, weaponId, playerInfo, Battle.Killer, 0L);
            SetDeadType(deadType);
        }

        public void AddOtherInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, float damage, PlayerInfoComponent playerInfo, long timestamp)
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
            other.TrueDamage += damage;
            other.Damage = (int) other.TrueDamage;
            SetBattleInfo(key, weaponId, playerInfo, other, timestamp);
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
