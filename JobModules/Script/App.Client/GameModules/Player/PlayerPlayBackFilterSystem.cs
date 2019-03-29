using App.Client.GameModules.Player;
using App.Shared.Components;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class PlayerPlayBackFilterSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private PlayerContext _player;
        public static bool Enable;

        public PlayerPlayBackFilterSystem(Contexts contexts) : base(contexts)
        {
            Enable = GameRules.IsChicken(contexts.session.commonSession.RoomInfo.ModeId);
            _player = contexts.player;
        }


        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.NetworkAnimator,
                PlayerMatcher.ThirdPersonAnimator,
                PlayerMatcher.Orientation,
                PlayerMatcher.ThirdPersonAppearance,
                PlayerMatcher.PredictedAppearance,
                PlayerMatcher.LatestAppearance,
                PlayerMatcher.Position).NoneOf(PlayerMatcher.FlagSelf));
        }

        private bool _hasSelfPlay = false;
        private Vector3 _postion;

        private const int MinDistSqr = 2 * 2;
        Plane[] planes = new Plane[0];

        protected override void BeforeOnPlayback()
        {
            var self = _player.flagSelfEntity;
            if (self != null && self.hasPosition && self.hasCameraObj && self.hasOrientation)
            {
                _postion = self.position.Value;
                planes = GeometryUtility.CalculateFrustumPlanes(self.cameraObj.MainCamera);
                _hasSelfPlay = true;
            }
            else
            {
                _hasSelfPlay = false;
            }
        }

        protected override void OnPlayBack(PlayerEntity entity)
        {
            if (!_hasSelfPlay)
            {
                entity.isFlagPlayBackFilter = true;
            }
            else
            {
                var dit = entity.position.Value - _postion;
                if (!Enable || dit.sqrMagnitude < MinDistSqr)
                {
                    entity.isFlagPlayBackFilter = false;
                }
                else
                {
                    entity.isFlagPlayBackFilter = !ContainsSphere(planes, entity.position.Value, 1.3f);
                }
            }
        }

        private static int[] PlaneIdx = new[] {0, 1, 4, 5, 2, 3};

        ///判断某球体是否在平截头体内
        static bool ContainsSphere(Plane[] planes, Vector3 shpereCenter, float radius)
        {
            //球体中心到某裁面的距离
            //遍历所有裁面并计算
            foreach (var i in PlaneIdx)
            {
                //计算距离
                var fDistance = Vector3.Dot(planes[i].normal, shpereCenter) + planes[i].distance;
                //如果距离小于负的球体半径,那么就是外离
                if (fDistance < -radius)
                    return false;
                //如果距离的绝对值小于球体半径,那么就是相交
                if (Mathf.Abs(fDistance) < radius)
                    return true;
            }

            return true;
        }
    }
}