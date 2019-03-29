using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;

namespace App.Server.GameModules.GamePlay.player
{
    public class ServerPlayerCameraInitSystem : ReactiveEntityInitSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerPlayerCameraInitSystem));

        private VehicleContext _vehicleContext;
        private FreeMoveContext _freeMoveContext;
        private ICommonSessionObjects _sessionObjects;
        public ServerPlayerCameraInitSystem(
            PlayerContext playerContext) : base(playerContext)
        {
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            // 只针对预测或服务端entity
            return context.CreateCollector(PlayerMatcher.UserCmdSeq.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        public override void SingleExecute(PlayerEntity player)
        {
            if(null == player)
            {
                Logger.Error("playerentity is null");
                return;
            }
            player.AddCameraObj();
        }

       
    }
}
