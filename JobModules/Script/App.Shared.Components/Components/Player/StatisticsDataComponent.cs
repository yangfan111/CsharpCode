using Core.Components;
using Core.EntityComponent;
using Core.Enums;
using Core.Free;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Statistics;
using Core.SyncLatest;
using Core.UpdateLatest;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
 
    [Player]
    public class StatisticsServerDataComponent : ISelfLatestComponent,IRule
    {

        public int RecordedShootTotalCount
        {
            get { return ShootPlayerMatchCount + ServerShootPlayerExtraCount + ClientShootPlayerMissCount; }
        }

        public float GetPer()
        {
            var div = RecordedShootTotalCount - CacheedShootTotalCount;
            return div == 0 ?100:(ShootPlayerMatchCount - CachedShootPlayerMatchCount) *100/div;
        }
        public int CacheedShootTotalCount
        {
            get { return CachedShootPlayerMatchCount + CachedServerShootPlayerExtraCount + CachedClientShootPlayerMissCount; }
        }
        public int GetRuleID()
        {
            return (int) ERuleIds.StatisticsServerDataComponent;
        }
        public string GetMatchStr()
        {
            return string.Format("Per:{4}%\nMatch:{0}/{1}\nClientOnly:{2},ServerOnly:{3}",
                ShootPlayerMatchCount-CachedShootPlayerMatchCount, RecordedShootTotalCount -CacheedShootTotalCount,
                ClientShootPlayerMissCount -CachedClientShootPlayerMissCount, ServerShootPlayerExtraCount-CachedServerShootPlayerExtraCount,GetPer());
        }
        [DontInitilize] public int CachedShootPlayerMatchCount;
        [DontInitilize] public int CachedServerShootPlayerExtraCount;
        [DontInitilize] public int CachedClientShootPlayerMissCount;

        public void GUIClean()
        {
            CachedClientShootPlayerMissCount = ClientShootPlayerMissCount;
            CachedServerShootPlayerExtraCount = ServerShootPlayerExtraCount;
            CachedShootPlayerMatchCount = ShootPlayerMatchCount;

        }
        
        [System.Obsolete]
        [DontInitilize] public int ServerShootPlayerTotalCount;
        /// <summary>
        ///     前端击中后端Miss次数(假红)
        /// </summary>
        [DontInitilize, NetworkProperty] public int ClientShootPlayerMissCount;

        /// <summary>
        ///     实际前后端都同步击中的次数
        /// </summary>
        [DontInitilize, NetworkProperty] public int ShootPlayerMatchCount;

        /// <summary>
        ///     后端击中前端Miss次数
        /// </summary>
        [DontInitilize, NetworkProperty] public int ServerShootPlayerExtraCount;
        
        

        public void CopyFrom(object rightComponent)
        {
            var right = (rightComponent as StatisticsServerDataComponent);
            ServerShootPlayerExtraCount = right.ServerShootPlayerExtraCount;
            ClientShootPlayerMissCount  = right.ClientShootPlayerMissCount;
            ShootPlayerMatchCount       = right.ShootPlayerMatchCount;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.StatisticsServerData;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightComponent = (right as StatisticsServerDataComponent);
             return   ServerShootPlayerExtraCount == rightComponent.ServerShootPlayerExtraCount &&
                   ClientShootPlayerMissCount == rightComponent.ClientShootPlayerMissCount &&
                   ShootPlayerMatchCount == rightComponent.ShootPlayerMatchCount;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class StatisticsDataComponent : IResetableComponent, IComponent
    {
        //战斗数据
        public BattleData Battle;

        //是否显示
        public bool IsShow;

        //统计数据
        public StatisticsData Statistics;

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

        public int GetComponentId()
        {
            {
                {
                    return (int) EComponentIds.PlayerStatisticsData;
                }
            }
        }

        private void SetBattleInfo(EntityKey key, int weaponId, PlayerInfoComponent playerInfo,
                                   PlayerBattleInfo battleInfo, long timestamp)
        {
            battleInfo.PlayerKey  = key;
            battleInfo.PlayerLv   = playerInfo.Level;
            battleInfo.PlayerName = playerInfo.PlayerName;
            battleInfo.BackId     = playerInfo.BackId;
            battleInfo.TitleId    = playerInfo.TitleId;
            battleInfo.BadgeId    = playerInfo.BadgeId;
            battleInfo.WeaponId   = weaponId;
            battleInfo.timestamp  = timestamp;
        }

        public void AddOpponentInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, float damage,
                                    PlayerInfoComponent playerInfo, int deathCount)
        {
            OpponentBattleInfo opponent = new OpponentBattleInfo();
            ;
            bool containsFlag = false;
            if (Battle.OpponentDict.Count > 0)
            {
                foreach (OpponentBattleInfo obi in Battle.OpponentDict.Values)
                {
                    if (obi.PlayerKey == key && obi.DeathCount == deathCount)
                    {
                        opponent     = obi;
                        containsFlag = true;
                        break;
                    }
                }
            }

            if (!containsFlag)
            {
                Battle.OpponentDict.Add(Battle.OpponentDict.Count, opponent);
            }

            if (!opponent.IsKill) opponent.IsKill       = isKill;
            if (!opponent.IsHitDown) opponent.IsHitDown = isHitDown;
            opponent.TrueDamage += damage;
            opponent.Damage     =  (int) opponent.TrueDamage;
            opponent.DeathCount =  deathCount;
            SetBattleInfo(key, weaponId, playerInfo, opponent, 0L);
        }

        public void AddKillerInfo(EntityKey key, int weaponId, int deadType, PlayerInfoComponent playerInfo)
        {
            SetBattleInfo(key, weaponId, playerInfo, Battle.Killer, 0L);
            SetDeadType(deadType);
        }

        public void AddOtherInfo(EntityKey key, int weaponId, bool isKill, bool isHitDown, float damage,
                                 PlayerInfoComponent playerInfo, long timestamp)
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
            other.Damage     =  (int) other.TrueDamage;
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
            Battle.Killer.DeadType = (EUIDeadType) deadType;
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