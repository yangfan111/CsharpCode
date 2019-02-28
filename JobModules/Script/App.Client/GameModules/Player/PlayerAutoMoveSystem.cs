
using App.Client.GameModules.Player;
using App.Shared.GameModules.Player;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.ClientPlayer
{
    public class PlayerAutoMoveSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAutoMoveSystem));

        private Contexts _contexts;
        private ITimeManager _timeManager;
        private IGroup<PlayerEntity> _players;
        public PlayerAutoMoveSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
            _timeManager = contexts.session.clientSessionObjects.TimeManager;
           
        }

        private int count = 0;

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.FlagAutoMove, PlayerMatcher.ThirdPersonModel,PlayerMatcher.Position,PlayerMatcher.CharacterContoller,PlayerMatcher.GamePlay));
            
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.characterContoller.Value !=null;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            var cc = player.characterContoller.Value;
          
            var velociy = Vector3.zero;

            count++;
            if (count < 1000)
            {
                velociy.x = 0.02f;
                velociy.z = 0.02f;
            }
            else if (count < 2000)
            {
                velociy.x = -0.02f;
                velociy.z = -0.02f;
            }
            else
            {
                count = 0;
            }
            velociy.y = -1;
            velociy.x = velociy.z = 0;
            PlayerMoveUtility.Move(_contexts, player, cc, velociy, _timeManager.FrameInterval * 0.001f);

            player.position.Value = cc.transform.position;
        }
    }
}