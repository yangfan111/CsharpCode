using System.Collections.Generic;
using App.Shared.Components.Player;
using Core.Components;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.UpdateLatest;
using Core.Utils;
using Utils.Singleton;
using System.Diagnostics;

namespace App.Shared.UpdateLatest
{
    public class SyncUpdateLatestMsgHandler : ISyncUpdateLatestMsgHandler
    {
        private Dictionary<int, CustomProfileInfo> _profiles;

        public static void CopyForm(UserCmd cmd, SendUserCmdComponent right)
        {
            //cmd.Seq = right.Seq;
            cmd.FrameInterval = right.FrameInterval;
            cmd.MoveHorizontal = right.MoveHorizontal;
            cmd.MoveUpDown = right.MoveUpDown;
            cmd.MoveVertical = right.MoveVertical;
            cmd.DeltaYaw = right.DeltaYaw;
            cmd.DeltaPitch = right.DeltaPitch;
            cmd.RenderTime = right.RenderTime;
            cmd.ClientTime = right.ClientTime;
            cmd.ChangedSeat = right.ChangedSeat;
            //cmd.SnapshotId = right.SnapshotId;
            cmd.BeState = right.BeState;
            cmd.Buttons = right.Buttons;
            cmd.SwitchNumber = right.SwitchNumber;

            cmd.CurWeapon = right.CurWeapon;
            cmd.UseEntityId = right.UseEntityId;
            cmd.ManualPickUpEquip = right.ManualPickUpEquip;

            cmd.AutoPickUpEquip = UserCmd.CopyList(cmd.AutoPickUpEquip, right.AutoPickUpEquip);
            cmd.UseVehicleSeat = right.UseVehicleSeat;
            cmd.UseType = right.UseType;
            cmd.ChangeChannel = right.ChangeChannel;
            cmd.BagIndex = right.BagIndex;
        }


        public SyncUpdateLatestMsgHandler()
        {
            _profiles = new Dictionary<int, CustomProfileInfo>();
        }
       
        private CustomProfileInfo GetProfile(int cid)
        {
            if (_profiles.ContainsKey(cid))
            {
                return _profiles[cid];
            }

            var c = SingletonManager.Get<DurationHelp>()
                .GetCustomProfileInfo(string.Format("CopyFrom_{0}", (EComponentIds) cid));
            _profiles[cid] = c;
            return c;
        }

        public void SyncToEntity(IUserCmdOwner owner, UpdateLatestPacakge package)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;
            IGameEntity gameEntity = playerEntity.entityAdapter.SelfAdapter;
            int count = package.UpdateComponents.Count;
            for (var i = 0; i < count; i++)
            {
                var component = package.UpdateComponents[i];
                var id = component.GetComponentId();
                var target = gameEntity.GetComponent(id);
                if (target == null)
                {
                    target = gameEntity.AddComponent(id);
                }

                var p = GetProfile(component.GetComponentId());
                try
                {
                    p.BeginProfileOnlyEnableProfile();
                    ((INetworkObject) target).CopyFrom(component);
                    if (id == (int) EComponentIds.SendUserCmd)
                    {
                        UserCmd cmd = UserCmd.Allocate();
                        CopyForm(cmd, component as SendUserCmdComponent);
                        playerEntity.userCmd.AddLatest(cmd);
                        cmd.Seq = package.Head.UserCmdSeq;
                        cmd.SnapshotId = package.Head.LastSnapshotId;
                        cmd.ReleaseReference();
                    }
                }
                finally
                {
                    p.EndProfileOnlyEnableProfile();
                }
            }
        }
    }
}