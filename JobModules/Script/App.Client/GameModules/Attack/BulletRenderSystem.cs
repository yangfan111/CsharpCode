using App.Client.GameModules.ClientPlayer;
using App.Client.GameModules.Player;
using Core;
using Core.Utils;
using Entitas;
using UnityEngine;


namespace App.Client.GameModules.Attack
{
    public class BulletRenderSystem : AbstractRenderSystem<BulletEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerPlaybackSystem));

        public BulletRenderSystem(Contexts contexts) : base(contexts)
        {
        }


        protected override IGroup<BulletEntity> GetIGroup(Contexts contexts)
        {
            return contexts.bullet.GetGroup(BulletMatcher.AllOf(BulletMatcher.BulletAsset,
                BulletMatcher.Position));
        }

        protected override bool Filter(BulletEntity entity)
        {
            return true;
        }

        protected override void OnRender(BulletEntity bullet)
        {
            var bulletGo = bullet.bulletAsset.UnityObject.AsGameObject;
            if (!bulletGo)
                return;
            bulletGo.SetActive( true/*bullet.bulletData.Distance > GlobalConst.InitDistanceForDisplay*/);
            if (bullet.bulletData.Distance < GlobalConst.MaxDistanceForCorrent)
            {
                float yDown = (GlobalConst.MaxDistanceForCorrent - bullet.bulletData.Distance) / GlobalConst.MaxDistanceForCorrent *
                              GlobalConst.InitZDown;
                var last = bulletGo.transform.position;
                bulletGo.transform.position = bullet.position.Value + new Vector3(0, -1 * yDown, 0);
                if (DebugConfig.DrawBulletLine)
                {
                    RuntimeDebugDraw.Draw.DrawLine(last, bulletGo.transform.position, Color.blue, 3f);
                }
            }
            else
            {
                var last = bulletGo.transform.position;
                bulletGo.transform.position = bullet.position.Value;
                if (DebugConfig.DrawBulletLine)
                {
                    RuntimeDebugDraw.Draw.DrawLine(last, bulletGo.transform.position, Color.blue, 3f);
                }
            }

            bulletGo.transform.rotation = Quaternion.LookRotation(bullet.bulletData.Velocity);
        }
    }
}