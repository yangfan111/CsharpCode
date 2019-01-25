using Core.EntityComponent;
using Core.Utils;
using System.Collections.Generic;
using XmlConfig;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerSoundPlaySystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerSoundPlaySystem));

        private Dictionary<EntityKey, bool> _inTransition = new Dictionary<EntityKey, bool>(EntityKeyComparer.Instance);
        private Dictionary<EntityKey, PostureInConfig> _lastPosture = new Dictionary<EntityKey, PostureInConfig>();
        private Contexts _contexts;

        public PlayerSoundPlaySystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntity = owner.OwnerEntity as PlayerEntity;
            if(null == playerEntity)
            {
                return;
            }
            if(!playerEntity.hasWeaponSound)
            {
                return;
            }

            foreach (var sound in playerEntity.weaponSound.PlayList)
            {
                //TODO 实现PlaySound, 注意区分客户端和服务器
                switch(sound)
                {
                }
            }
        }
    }
}