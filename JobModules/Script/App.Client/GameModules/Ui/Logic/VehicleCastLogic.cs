using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Logic
{
    public class VehicleCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VehicleCastLogic));

        private VehicleContext _vehicleContext;
        private IUserCmdGenerator _userCmdGenerator;
        private int _seatId;
        private int _vehicleId;

        public VehicleCastLogic(VehicleContext vehicleContext, PlayerContext playerContext, IUserCmdGenerator cmdGenerator, float maxDistance) : base(playerContext, maxDistance)
        {
            _vehicleContext = vehicleContext;
            _playerContext = playerContext;
            _userCmdGenerator = cmdGenerator;
        }

        public override void OnAction()
        {
            _userCmdGenerator.SetUserCmd((cmd) => cmd.IsUseAction = true);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.UseVehicleSeat = _seatId);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.UseEntityId = _vehicleId);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.UseType = (int)EUseActionType.Vehicle);
            Logger.DebugFormat("Action vehicle {0} seat {1}", _vehicleId, _seatId);
        }

        protected override void DoSetData(PointerData data)
        {
            _seatId = 0;
            _vehicleId = VehicleCastData.EntityId(data.IdList);
            var vehicleEntity = _vehicleContext.GetEntityWithEntityKey(new EntityKey(_vehicleId, (short)EEntityType.Vehicle));
            if(null != vehicleEntity)
            {
                if(vehicleEntity.hasPosition)
                {
                    var player = _playerContext.flagSelfEntity;
                    if(player.hasPosition)
                    {
                        var dis = Vector3.Distance(vehicleEntity.position.Value, player.position.Value);
                        if(IsUntouchableOffGround(player, data.Position, vehicleEntity.gameObject.UnityObject))
                        {
                            return;
                        }
                       if(!player.IsOnVehicle())
                        {
                            if(!player.IsVehicleEnterable(vehicleEntity) || !vehicleEntity.IsFocusable())
                            {
                                Logger.DebugFormat("Player is unable to enter the vehicle, or the vehicle is unfocusable.");
                                return;
                            }

                            if (!vehicleEntity.IsRidable())
                            {
                                Tip = string.Format(ScriptLocalization.client_actiontip.pullupvehicle, vehicleEntity.vehicleAssetInfo.TipName);
                                return;
                            }

                            var hitPos = data.Position;
                            _seatId = vehicleEntity.FindPreferedSeat(hitPos);
                            if(_seatId == (int)VehicleSeatIndex.None)
                            {
                                Logger.DebugFormat("No seats available.");
                                return;
                            }
                            if (_seatId == (int)VehicleSeatIndex.Driver)
                            {
                                Tip = string.Format(ScriptLocalization.client_actiontip.drivevehicle, vehicleEntity.vehicleAssetInfo.TipName);
                            }
                            else
                            {
                                Tip = string.Format(ScriptLocalization.client_actiontip.entervehicle, vehicleEntity.vehicleAssetInfo.TipName);
                            }
                        }
                    }
                }
            }
        }
    }
}
