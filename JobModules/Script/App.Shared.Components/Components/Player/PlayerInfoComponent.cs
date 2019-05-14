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

    [Player]

    public class PlayerTokenComponent : IComponent
    {
        //玩家信息
        [EntityIndex] public string Token;
    }

    /// <summary>
    /// 这里初始化的数据，游戏过程中不会更新
    /// </summary>
    [Player]
    
    public class PlayerInfoComponent : IPlaybackComponent,ICreatePlayerInfo,IResetableComponent
    {
        //玩家信息
        public string Token { get; set; }
        [NetworkProperty]public int EntityId { get; set; }
        [NetworkProperty]public long PlayerId { get; set; }
        [NetworkProperty]public string PlayerName{ get; set; }
        [NetworkProperty]public int RoleModelId{ get; set; }
        [NetworkProperty][EntityIndex]public long TeamId{ get; set; }
        [NetworkProperty]public int Num{ get; set; }
        [NetworkProperty]public int Level{ get; set; }
        [NetworkProperty]public int BackId{ get; set; }
        [NetworkProperty]public int TitleId{ get; set; }
        [NetworkProperty]public int BadgeId{ get; set; }
        [NetworkProperty]public List<int> AvatarIds{ get; set; }
        [NetworkProperty]public List<int> WeaponAvatarIds{ get; set; }
        [NetworkProperty]public int Camp { get; set; }
        [NetworkProperty]public List<int> SprayLacquers { get; set; } /*喷漆*/

        [NetworkProperty] public int JobAttribute { get; set; } /*职业*/
        /// <summary>
        /// 初始位置
        /// </summary>
        public Vector3 InitPosition { get; set; }
        [DontInitilize] public int SpecialFeedbackType { get; set; } /*击杀，非复合方式*/
        [DontInitilize] public List<int> CacheAvatarIds{ get; set; } /*缓存 ChangeNewRole */
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

            if (SprayLacquers == null) {
                SprayLacquers = new List<int>();
            }
            SprayLacquers.Clear();
            SprayLacquers.AddRange(right.SprayLacquers);

            Camp = right.Camp;
            JobAttribute = right.JobAttribute;
            Token = right.Token;
            InitPosition = right.InitPosition;
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
            if (SprayLacquers == null) {
                SprayLacquers = new List<int>();
            }
            SprayLacquers.Clear();
        }

        public void ChangeNewRole(int roleId)
        {
            RoleModelId = roleId;
            if (null == CacheAvatarIds){
                CacheAvatarIds = new List<int>();
            }
            if (AvatarIds.Count > 0) {
                CacheAvatarIds.Clear();
                CacheAvatarIds.AddRange(AvatarIds);
            }
            AvatarIds.Clear();
        }

        public void InitTransform()
        {
            FirstPersonRightHand = null;
            ThirdPersonRightHand = null;
            FirstPersonHead = null;
            ThirdPersonHead = null;
        }

        public override string ToString()
        {
            return string.Format("EntityId: {0}, PlayerId: {1}, PlayerName: {2}, RoleModelId: {3}, AvatarIds: {4}", EntityId, PlayerId, PlayerName, RoleModelId, AvatarIds);
        }
    }
}
