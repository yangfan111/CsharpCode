
using App.Shared.Components.Player;
using App.Shared.GameModules.Vehicle;
using Core.CharacterController;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public static class PlayerMoveUtility
    {
        public static void Move(Contexts contexts, PlayerEntity player, ICharacterControllerContext controller, Vector3 dist, float deltaTime)
        {
            ResolveOverlapWithVehicle(contexts.vehicle, player, controller, deltaTime);
            // enlarge distance if movement too small
            if (dist.sqrMagnitude < 1E-4)
            {
                dist = Vector3.Normalize(dist);
                dist *= 0.01f;
            }
            controller.Move(dist, deltaTime);
        }


        private static float[] _resolveRotateAngle = new float[] {
            0.0f, 45.0f, -45.0f, 90.0f, -90.0f, 135.0f, -135.0f, 180.0f
        };

        private static void ResolveOverlapWithVehicle(VehicleContext context, PlayerEntity player, ICharacterControllerContext controller, float deltaTime)
        {
            var gamePlay = player.gamePlay;
            if (gamePlay.IsLifeState(EPlayerLifeState.Alive) && !gamePlay.IsVehicleCollisionState(EVehicleCollisionState.None))
            {
                var vehicle = context.GetEntityWithEntityKey(gamePlay.CollidedVehicleKey);
                if (vehicle != null && vehicle.hasGameObject)
                {
                    var transform = vehicle.gameObject.UnityObject.AsGameObject.transform;
                    var forward = transform.forward;
                    var right = transform.right;

                    var rootTransform = player.characterContoller.Value.transform;
                    var playerPosition = rootTransform.position;
                    var vehiclePosition = vehicle.gameObject.UnityObject.AsGameObject.transform.position;
                    var deltaPosition = playerPosition - vehiclePosition;
                    deltaPosition.y = 0;
                    deltaPosition = deltaPosition.normalized;
                    if (deltaPosition.sqrMagnitude < 1E-4f)
                    {
                        if (Mathf.Abs(forward.y) > 0.8)
                        {
                            deltaPosition = right;
                        }
                        else
                        {
                            deltaPosition = forward;
                        }
                    }

                    var moveDirection = Mathf.Abs(forward.y) < 0.8 ? forward : right;
                    var baseDirection = moveDirection;
                    var maxDot = float.NegativeInfinity;

                    //select the best overlap-resolve direction
                    foreach (var angle in _resolveRotateAngle)
                    {
                        var direction = Quaternion.AngleAxis(angle, transform.up) * baseDirection;
                        var dot = Vector3.Dot(direction, deltaPosition);
                        if (dot > maxDot)
                        {
                            moveDirection = direction;
                            maxDot = dot;
                        }
                    }

                    //resolve overlap according to direction
                    const int maxIterator = 2;

                    var velcoity = vehicle.GetLinearVelocity().magnitude;
                    var expectedDistance = velcoity * 0.333f;
                    for (int i = 1; i <= maxIterator; ++i)
                    {
                        moveDirection.y = 0;
                        moveDirection = moveDirection.normalized;
                        //move forward to resolve overlap
                        float resolveDist = 5.5f * i /*+ 0.1f*/;
                        controller.Move(moveDirection * resolveDist, deltaTime);
                        
                        var position = rootTransform.position;
                        if (VehicleEntityUtility.IsPlayerOverlapAtPosition(player, position, UnityLayers.VehicleLayerMask))
                        {
                            rootTransform.position = playerPosition;
                            continue;
                        }
                        //move backward to close vehicle
                        var backDist = (playerPosition - rootTransform.position);
//                        var backMag = backDist.magnitude;
//                        if (backMag > expectedDistance)
//                        {
//                            var backNorm = backDist.normalized;
//                            backMag = backMag - expectedDistance;
//                            backDist = backMag * backNorm;
//                        }

                        if (backDist.sqrMagnitude > 1E-4f)
                        {
                            controller.Move(backDist, deltaTime);
                        }
                       
                        break;
                    }
                }
                gamePlay.CollidedVehicleKey = new EntityKey();
            }

        }
    }
}
