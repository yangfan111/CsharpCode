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
                if (xzSpeed >= 8.5)
                {
                    damage += (xzSpeed * 3.6f - 30) * 2;
                }
                
                //垂直伤害
                float ySpeed = -player.playerMove.LastVelocity.y;
                if (ySpeed >= 14)
                {
                    damage += (ySpeed - 14) * 14;
                }

                if ((damage > 0) && (SharedConfig.HaveFallDamage ==true ))
                {
                    VehicleDamageUtility.DoPlayerDamage(_contexts, null, player, damage, EUIDeadType.Fall);
//                    Debug.LogFormat("IsCollided ... xzSpeed:{0}, ySpeed:{1}, damage:{2}", xzSpeed, ySpeed, damage);
                }
            }

            player.playerMove.LastIsCollided = player.playerMove.IsCollided;
            player.playerMove.LastVelocity = player.playerMove.Velocity;
        }
    }
}
