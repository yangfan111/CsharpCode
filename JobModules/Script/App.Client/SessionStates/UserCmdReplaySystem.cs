using App.Shared.Components.Player;
using Common;
using Core.ObjectPool;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SessionState;
using Core.UpdateLatest;
using Core.Utils;
using Sharpen;
using Utils.Replay;

namespace App.Client.ClientGameModules
{
    public class UserCmdReplaySystem : AbstractStepExecuteSystem
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdReplaySystem));
        private readonly Contexts _contexts;
       

        public UserCmdReplaySystem(Contexts contexts)
        {
            _contexts = contexts;

        }

        protected override void InternalExecute()
        {
            while (true)
            {
                var item = _contexts.session.clientSessionObjects.Replay.GetItem(EReplayMessageType.OUT, MyGameTime.stage,
                    MyGameTime.seq,
                    _contexts.session.clientSessionObjects.NetworkChannel.Id);
                if (item == null) return;
                
                if (item.MessageBody is ReusableList<UpdateLatestPacakge> && item.ProcessSeq == MyGameTime.seq)
                {
                    var last = _contexts.player.flagSelfEntity.userCmd.Latest as UserCmd;
                    ReusableList<UpdateLatestPacakge> reusableList =
                        item.MessageBody as ReusableList<UpdateLatestPacakge>;
                    var current = reusableList.Value.Last();
                    foreach (var updateComponent in current.UpdateComponents)
                    {
                        if (updateComponent is SendUserCmdComponent)
                        {
                            
                           
                            var right = updateComponent as SendUserCmdComponent;
                            _logger.DebugFormat("replay usercmd: last:{0}, timeseq:{1}, msgSeq:{2} {3}", last.Seq, MyGameTime.seq, current.Head.UserCmdSeq, item);
                            //last.Seq = current.Head.UserCmdSeq;
                            //last.SnapshotId = current.Head.LastSnapshotId;
                            _contexts.session.clientSessionObjects.TimeManager.RenderTime = right.RenderTime;
                            last.FrameInterval = right.FrameInterval;
                            last.MoveHorizontal = right.MoveHorizontal;
                            last.MoveUpDown = right.MoveUpDown;
                            last.MoveVertical = right.MoveVertical;
                            last.DeltaYaw = right.DeltaYaw;
                            last.DeltaPitch = right.DeltaPitch;
                            last.RenderTime = right.RenderTime;
                            last.ClientTime = right.ClientTime;
                            last.ChangedSeat = right.ChangedSeat;

                            last.BeState = right.BeState;
                            last.Buttons = right.Buttons;
                            last.SwitchNumber = right.SwitchNumber;

                            last.CurWeapon = right.CurWeapon;
                            last.UseEntityId = right.UseEntityId;
                            last.ManualPickUpEquip = right.ManualPickUpEquip;

                            last.AutoPickUpEquip = UserCmd.CopyList(last.AutoPickUpEquip, right.AutoPickUpEquip);
                            last.UseVehicleSeat = right.UseVehicleSeat;
                            last.UseType = right.UseType;
                            last.ChangeChannel = right.ChangeChannel;
                            last.BagIndex = right.BagIndex;
                        }
                    }
                }

                if (item.MessageBody is ReusableList<IVehicleCmd> && item.ProcessSeq == MyGameTime.seq)
                {
                    var cmd = (item.MessageBody as ReusableList<IVehicleCmd>).Value.Last();
                    var controlledVehicle = PlayerVehicleUtility.GetControlledVehicle(_contexts.player.flagSelfEntity, _contexts.vehicle);
                    if (controlledVehicle != null)
                    {
                        controlledVehicle.vehicleCmd.AddLatest(cmd);
                    }
                  
                }
                if (item.MessageBody is ReusableList<IRefCounter>)
                {
                    foreach (var updateLatestPacakge in (item.MessageBody as ReusableList<UpdateLatestPacakge>).Value)
                    {
                        updateLatestPacakge.ReleaseReference();
                    }
                }
                item.ReleaseReference();
            }
        }
    }
}