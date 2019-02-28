using App.Shared;
using UserInputManager.Lib;
using UnityEngine;
using App.Client.CastObjectUtil;
using Core.Prediction.UserPrediction.Cmd;
using App.Shared.GameModules.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Core.Enums;
using App.Shared.GameModules.Player;
using I2.Loc;

namespace App.Client.GameModules.Ui.Logic
{
    public class VehicleCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VehicleCastLogic));
        private VehicleContext _vehicleContext;
        private PlayerContext _playerContext;
        private IUserCmdGenerator _userCmdGenerator;
        private int _seatId;
        private int _vehicleId;

        public VehicleCastLogic(
            VehicleContext vehicleContext,
            PlayerContext playerContext,
            IUserCmdGenerator cmdGenerator,
            float maxDistance) : base(playerContext, maxDistance)
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
            var vehicleEntity = _vehicleContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(_vehicleId, (short)EEntityType.Vehicle));
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
                            if(!player.IsVehicleEnterable(vehicleEntity) || 
                                !vehicleEntity.IsFocusable())
                            {
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
