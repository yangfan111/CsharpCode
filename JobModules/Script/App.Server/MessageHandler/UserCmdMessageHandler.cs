using System;
using System.Collections.Generic;
using App.Shared;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Server.MessageHandler
{
    
    class UserCmdMessageHandler : AbstractServerMessageHandler<PlayerEntity, ReusableList<IUserCmd>>
    {
        private int _lastSeq = -1;
        private DateTime _lastRecvTime = DateTime.MinValue;
        private int _frameRateInv = 50;

        public UserCmdMessageHandler(IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {

        }


        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, ReusableList<IUserCmd> messageBody)
        {
            var cmdList = messageBody.Value;
            entity.userCmd.AddLatestList(cmdList);
            
            entity.network.NetworkChannel.Serializer.MessageTypeInfo.SetReplicationAckId(entity.userCmd.Latest.SnapshotId);

            var lastCmd = cmdList[cmdList.Count - 1];
            var firstCmd = cmdList[0];
#pragma warning disable RefCounter002 // possible reference counter error
            var cmd = firstCmd.Seq > lastCmd.Seq ? firstCmd : lastCmd;
#pragma warning restore RefCounter002 // possible reference counter error
            if (_logger.IsDebugEnabled)
              {
                  _logger.DebugFormat("received usercmd message, seq {0}, ack snapshot {1}", cmd.Seq, cmd.SnapshotId);
              }

            CheckVehicleCmdList(cmdList);
        }

        private void CheckVehicleCmdList(List<IUserCmd> cmdList)
        {
            if (_lastSeq < 0)
            {
                _lastSeq = cmdList[0].Seq;
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
                    if (cmd.Seq == expectedSeq)
                    {
                        found = true;
                    }

                    if (cmd.Seq > _lastSeq)
                    {
                        _lastSeq = cmd.Seq;
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
                    _logger.InfoFormat("The delta time of received user cmd  is too large {0}, last seq {1}", deltaTime.TotalMilliseconds, cmdList[0].Seq);
                }

                _lastRecvTime = curTime;
            }
        }
    }
}