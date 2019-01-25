using App.Shared.Components.Ui;
using Core.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;

namespace App.Shared.WeaponLogic.FireLogic
{
    public class ShowFireInMap : IBulletFire
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ShowFireInMap));
        private UiContext _uiContext;
        public ShowFireInMap(UiContext uiContext)
        {
            _uiContext = uiContext;
        }

        public void OnBulletFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            if(playerEntity.hasStatisticsData)
            {
                playerEntity.statisticsData.Statistics.ShootingCount += 1;
            }
            else
            {

            }
            //服务端不需要处理,uicontext为null
            if (null != _uiContext && _uiContext.hasMap)
            {
                var map = _uiContext.map;

                MiniMapTeamPlayInfo playerInfo = null;
                foreach (var player in map.TeamInfos)
                {
                    if (playerEntity.playerInfo.PlayerId == player.PlayerId)
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
