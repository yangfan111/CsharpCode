using App.Shared.GameModules.Player;
using Core.Compensation;
using System.Collections.Generic;

namespace App.Shared.GameModules.Throwing
{
    public interface IThrowingSegment
    {
        int ServerTime { get; }
        RaySegment RaySegment { get; }
        bool IsValid { get; }
        ThrowingEntity ThrowingEntity { get; }
        List<int> ExcludePlayerList { get; }
    }

    public class DefaultThrowingSegment : IThrowingSegment
    {
        private PlayerContext _playerContext;
        private List<int> _excludeList;
        public DefaultThrowingSegment(int serverTime, RaySegment raySegment, ThrowingEntity throwingEntity, PlayerContext playerContext)
        {
            _serverTime = serverTime;
            _raySegment = raySegment;
            _throwingEntity = throwingEntity;
            _playerContext = playerContext;
        }

        private int _serverTime;
        private RaySegment _raySegment;
        private ThrowingEntity _throwingEntity;

        public int ServerTime
        {
            get { return _serverTime; }
        }

        public RaySegment RaySegment
        {
            get { return _raySegment; }
        }

        public bool IsValid
        {
            get { return !_throwingEntity.isFlagDestroy; }
        }

        public ThrowingEntity ThrowingEntity
        {
            get { return _throwingEntity; }
        }

        public List<int> ExcludePlayerList
        {
            get
            {
                var selfPlayer = _playerContext.GetEntityWithEntityKey(_throwingEntity.ownerId.Value);
                return selfPlayer.playerHitMaskController.HitMaskController.ThrowingExcludeTargetList;
            }
        }
    }
}