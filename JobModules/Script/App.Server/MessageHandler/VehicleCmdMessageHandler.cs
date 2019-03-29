using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Shared;
using App.Shared.Network;
using Core.Network;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Server.MessageHandler
{
    class VehicleCmdMessageHandler : AbstractServerMessageHandler<PlayerEntity, ReusableList<IVehicleCmd>>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleCmdMessageHandler));

        private Contexts _contexts;
        private int _lastSeq = -1;
        private DateTime _lastRecvTime = DateTime.MinValue;
        private int _frameRateInv = 50;

        public VehicleCmdMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            _contexts = contexts;

        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity,
            EClient2ServerMessage eClient2ServerMessage, ReusableList<IVehicleCmd> messageBody)
        {
            //_logger.DebugFormat("Server VehicleCmd seq is {0}", messageBody.Seq);
            var vehicle = PlayerVehicleUtility.GetControlledVehicle(entity, _contexts.vehicle);
            var cmdList = messageBody.Value;
            if (vehicle != null)
            {
                vehicle.vehicleCmd.AddLatestList(cmdList);
            }

            CheckVehicleCmdList(cmdList);
        }

        private void CheckVehicleCmdList(List<IVehicleCmd> cmdList)
        {
            if (_lastSeq < 0)
            {
                _lastSeq = cmdList[0].CmdSeq;
                _lastRecvTime = DateTime.Now;
                _frameRateInv = 1000 / Application.targetFrameRate + 16 + 5;

                return;
            }

            if (_logger.IsDebugEnabled)
            {
                AssertUtility.Assert(cmdList.Count > 0);
                var expectedSeq = _lastSeq + 1;
                bool found = false;
                foreach (var cmd in cmdList)
                {
                    if (cmd.CmdSeq == expectedSeq)
                    {
                        found = true;
                    }

                    if (cmd.CmdSeq > _lastSeq)
                    {
                        _lastSeq = cmd.CmdSeq;
                    }
                }

                if (!found)
                {
                    _logger.InfoFormat("ServerSync Lost Cmd Seq {0}", expectedSeq);
                }
            }

            if (_logger.IsInfoEnabled)
            {
                var curTime = DateTime.Now;
               
                var deltaTime = curTime - _lastRecvTime;

                if (deltaTime.TotalMilliseconds > _frameRateInv)
                {
                    _logger.InfoFormat("The delta time of received vehicle cmd  is too large {0}, seq {1}", deltaTime.TotalMilliseconds, cmdList[0].CmdSeq);
                }

                _lastRecvTime = curTime;
            }
        }
    }
}
