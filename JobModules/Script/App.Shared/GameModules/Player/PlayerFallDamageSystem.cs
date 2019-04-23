using App.Shared.GameModules.Vehicle;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class PlayerFallDamageSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerFallDamageSystem));
        private Contexts _contexts;

        public const float SpeedHorizontal = 8.5f;
        public const float SpeedVertical = 14f;

        public PlayerFallDamageSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity) owner.OwnerEntity;

            if (player.playerMove.IsGround && !player.playerMove.FirstOnGround)
            {
                player.playerMove.FirstOnGround = true;
            }
            else if (player.playerMove.FirstOnGround && !player.playerMove.LastIsCollided && player.playerMove.IsCollided)
            {
                float damage = 0;
                
                //水平伤害
                Vector2 inVel = new Vector2(player.playerMove.LastVelocity.x, player.playerMove.LastVelocity.z);
                Vector2 outVel = new Vector2(0, 0);//(player.playerMove.Velocity.x, player.playerMove.Velocity.z);
                float xzSpeed = (inVel - outVel).magnitude;

                float hurtSpeedHorizontal = SpeedHorizontal;
                if (player.playerMove.SpeedAffect > 0 && !player.IsOnVehicle())//有加速buff且不在载具内
                {
                    hurtSpeedHorizontal *= player.playerMove.SpeedAffect + 1;
                }
                if (xzSpeed >= hurtSpeedHorizontal)
                {
                    damage += (xzSpeed * 3.6f - 30) * 2;
                }
                
                //垂直伤害
                float ySpeed = -player.playerMove.LastVelocity.y;

                float hurtSpeedVertical = SpeedVertical;
                if (player.playerMove.JumpAffect > 0 && !player.IsOnVehicle())
                {
                    hurtSpeedVertical *= player.playerMove.JumpAffect + 1;
                }
                if (ySpeed >= hurtSpeedVertical)
                {
                    //damage += (ySpeed - hurtSpeedVertical) * 14; 伤害取二者大值而非相加
                    damage = Mathf.Max(damage, (ySpeed - hurtSpeedVertical) * 14);
                }

                if (damage > 0 && SharedConfig.HaveFallDamage)
                {
                    VehicleDamageUtility.DoPlayerDamage(_contexts, null, player, damage, EUIDeadType.Fall);
                }
            }

            player.playerMove.LastIsCollided = player.playerMove.IsCollided;
            player.playerMove.LastVelocity = player.playerMove.Velocity;
        }
    }
}
