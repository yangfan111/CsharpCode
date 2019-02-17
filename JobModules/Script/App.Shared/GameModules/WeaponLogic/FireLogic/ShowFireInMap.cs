using App.Shared.Components.Ui;
using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ShowFireInMap" />
    /// </summary>
    public class ShowFireInMap : IBulletFire
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ShowFireInMap));

        private UiContext _uiContext;

        public ShowFireInMap(UiContext uiContext)
        {
            _uiContext = uiContext;
        }

        public void OnBulletFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            controller.RelatedStatics.ShootingCount += 1;
            //服务端不需要处理,uicontext为null
            if (null != _uiContext && _uiContext.hasMap)
            {
                var map = _uiContext.map;

                MiniMapTeamPlayInfo playerInfo = null;
                foreach (var player in map.TeamInfos)
                {
                    if (controller.RelatedPlayerInfo.PlayerId == player.PlayerId)
                        playerInfo = player;
                }

                if (null != playerInfo)
                {
                    playerInfo.IsShooting = true;
                    playerInfo.ShootingCount++;
                }
            }
        }
    }
}
