using Core.Components;
using Core.Playback;
using Core.Room;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;
using UnityEngine;
using System;
using Entitas;

namespace App.Shared.Components.Player
{
   
    /// <summary>
    /// 这里初始化的数据，游戏过程中不会更新
    /// </summary>
    [Player]
    
    public class PlayerInfoComponent : IPlaybackComponent,ICreatePlayerInfo,IResetableComponent
    {
        //玩家信息
        [NetworkProperty]public int EntityId { get; set; }
        [NetworkProperty]public long PlayerId { get; set; }
        [NetworkProperty]public string PlayerName{ get; set; }
        [NetworkProperty]public int RoleModelId{ get; set; }
        [NetworkProperty] public string ModelName { get; set; }
        [NetworkProperty][EntityIndex]public long TeamId{ get; set; }
        [NetworkProperty]public int Num{ get; set; }
        [NetworkProperty]public int Level{ get; set; }
        [NetworkProperty]public int BackId{ get; set; }
        [NetworkProperty]public int TitleId{ get; set; }
        [NetworkProperty]public int BadgeId{ get; set; }
        [NetworkProperty]public List<int> AvatarIds{ get; set; }
        [NetworkProperty]public List<int> WeaponAvatarIds{ get; set; }
        [NetworkProperty]public int Camp { get; set; }
        [DontInitilize] public PlayerWeaponBagData[] WeaponBags { get; set; }
        [DontInitilize] public Transform FirstPersonRightHand;
        [DontInitilize] public Transform ThirdPersonRightHand;
        [DontInitilize] public Transform FirstPersonHead;
        [DontInitilize] public Transform ThirdPersonHead;

        public int GetComponentId()
        {
            { { return (int)EComponentIds.PlayerInfo; } }
        }

        public void CopyFrom(object rightComponent)
        {
            
            var right = (rightComponent as PlayerInfoComponent);
            if (right == null)
            {
                return;
            }
            PlayerId = right.PlayerId;
            PlayerName = right.PlayerName;
            RoleModelId = right.RoleModelId;
            TeamId = right.TeamId;
            ModelName = right.ModelName;
            Num = right.Num;
            Level = right.Level;
            BackId = right.BackId;
            TitleId = right.TitleId;
            BadgeId = right.BadgeId;
            
            if (AvatarIds == null)
            {
                AvatarIds = new List<int>();
            }
            AvatarIds.Clear();
            AvatarIds.AddRange(right.AvatarIds);
            if (WeaponAvatarIds == null)
            {
                WeaponAvatarIds = new List<int>();
            }
            WeaponAvatarIds.Clear();
            WeaponAvatarIds.AddRange(right.WeaponAvatarIds);
            Camp = right.Camp;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void Reset()
        {
            if (AvatarIds == null)
            {
                AvatarIds = new List<int>();
            }
            AvatarIds.Clear();
            if (WeaponAvatarIds == null)
            {
                WeaponAvatarIds = new List<int>();
            }
            WeaponAvatarIds.Clear();
        }

        public override string ToString()
        {
            return string.Format("EntityId: {0}, PlayerId: {1}, PlayerName: {2}, RoleModelId: {3}, ModelName: {4}, AvatarIds: {5}", EntityId, PlayerId, PlayerName, RoleModelId, ModelName, AvatarIds);
        }
    }
}
