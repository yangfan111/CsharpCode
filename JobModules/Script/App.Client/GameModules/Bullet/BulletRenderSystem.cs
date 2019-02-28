using App.Client.GameModules.ClientPlayer;
using App.Client.GameModules.Player;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;


namespace App.Client.GameModules.Bullet
{
    public class BulletRenderSystem : AbstractRenderSystem<BulletEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerPlaybackSystem));

        public BulletRenderSystem(Contexts contexts) : base(contexts)
        {
        }

        private const float InitDistanceForDisplay = 10;
        private const float MaxDistanceForCorrent = 20;
        private const float InitZDown = 0.3f;

        protected override IGroup<BulletEntity> GetIGroup(Contexts contexts)
        {
            return contexts.bullet.GetGroup(BulletMatcher.AllOf(BulletMatcher.BulletGameObject,
                BulletMatcher.Position));
        }

        protected override bool Filter(BulletEntity entity)
        {
            return true;
        }

        protected override void OnRender(BulletEntity bullet)
        {
            if (bullet.bulletData.Distance > InitDistanceForDisplay)
            {
                var go = bullet.bulletGameObject.UnityObject.AsGameObject;
                go.SetActive(true);
            }
            else
            {
                var go = bullet.bulletGameObject.UnityObject.AsGameObject;
                go.SetActive(false);
            }

            var bulletGo = bullet.bulletGameObject.UnityObject.AsGameObject;
            if (bullet.bulletData.Distance < MaxDistanceForCorrent)
            {
                float yDown = (MaxDistanceForCorrent - bullet.bulletData.Distance) / MaxDistanceForCorrent *
                              InitZDown;
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