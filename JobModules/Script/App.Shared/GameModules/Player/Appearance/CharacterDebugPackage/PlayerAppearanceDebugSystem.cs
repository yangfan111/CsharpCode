using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.CharacterDebugPackage
{
    public class PlayerAppearanceDebugSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAppearanceDebugSystem));
        private readonly PlayerContext _playerContext;

        public PlayerAppearanceDebugSystem(PlayerContext player)
        {
            _playerContext = player;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var players = _playerContext.GetEntities();
            if (null == players) return;

            if (SharedConfig.EnableAnimator)
            {
                EnableAnimator(true, players);
                SharedConfig.EnableAnimator = false;
            }

            if (SharedConfig.DisableAnimator)
            {
                EnableAnimator(false, players);
                SharedConfig.DisableAnimator = false;
            }
        }

        private void EnableAnimator(bool enable, PlayerEntity[] players)
        {
            foreach (var player in players)
            {
                var animator = player.thirdPersonAnimator.UnityAnimator;
                if (null == animator) continue;

                animator.enabled = enable;
                
                _logger.InfoFormat("player: {0}  animatorIsActiveAndEnable:  {1}", player.entityKey, animator.isActiveAndEnabled);
            }
        }

        private void UpdateRender(PlayerEntity[] players)
        {
            foreach (var player in players)
            {
                var animator = player.thirdPersonAnimator.UnityAnimator;
                if (null == animator) continue;

                var previousMode = animator.cullingMode;
                animator.cullingMode = AnimatorCullingMode.CullCompletely;
                animator.cullingMode = previousMode;

                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                
                _logger.InfoFormat("player: {0}  animatorCullingMode:  {1}", player.entityKey, animator.cullingMode);
            }
        }
    }
}
