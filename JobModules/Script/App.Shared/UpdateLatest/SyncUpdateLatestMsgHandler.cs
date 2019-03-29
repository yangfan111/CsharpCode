using App.Shared.Components.Player;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.UpdateLatest;

namespace App.Shared.UpdateLatest
{
    public class SyncUpdateLatestMsgHandler:ISyncUpdateLatestMsgHandler
    {
        public static void CopyForm( UserCmd cmd, SendUserCmdComponent right)
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
            cmd.PickUpEquip = right.PickUpEquip;
            cmd.UseVehicleSeat = right.UseVehicleSeat;
            cmd.UseType = right.UseType;
            cmd.ChangeChannel = right.ChangeChannel;
            cmd.BagIndex = right.BagIndex;
        }
        public void SyncToEntity(IUserCmdOwner owner, UpdateLatestPacakge package)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;
            IGameEntity gameEntity = playerEntity.entityAdapter.SelfAdapter;
            foreach (var component in package.UpdateComponents)
            {

               var target= gameEntity.GetComponent(component.GetComponentId());
                if (target == null)
                {
                    target = gameEntity.AddComponent(component.GetComponentId());
                }
                ((INetworkObject)target).CopyFrom(component);
                if (component is SendUserCmdComponent)
                {
                    UserCmd cmd =UserCmd.Allocate();
                    CopyForm(cmd, component as SendUserCmdComponent);
                    playerEntity.userCmd.AddLatest(cmd);
                    cmd.Seq = package.Head.UserCmdSeq;
                    cmd.SnapshotId = package.Head.LastSnapshotId;
                    cmd.ReleaseReference();
                }
            }
        }
    }
}