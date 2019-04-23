using App.Shared.Player;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions.LadderPackage
{
    public static class LadderCollisionTest
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LadderCollisionTest));
        
        private const string LadderTopTag = "LadderTop";
        private const string LadderMiddleTag = "LadderMiddle";
        private const string LadderBottomTag = "LadderBottom";
        
        public static bool CheckLadderKind(PlayerEntity player, out LadderLocation location, bool considerDirection = false)
        {
            location = LadderLocation.Null;

            var playerTransform = player.RootGo().transform;
            var pos = playerTransform.position;
            
            Vector3 capsuleBottom, capsuleUp;
            float capsuleRadius;
            PlayerEntityUtility.GetCapsule(player, pos, out capsuleBottom,
                out capsuleUp, out capsuleRadius);
            
            //DebugDraw.DebugWireSphere(capsuleBottom, Color.red, capsuleRadius);

            var colliders = Physics.OverlapSphere(capsuleBottom, capsuleRadius, UnityLayers.ClimbLadderLayerMask);
            if (colliders.Length <= 0) return false;


            location = CheckLadderLocation(colliders, playerTransform, considerDirection);

            return location != LadderLocation.Null;
        }

        private static LadderLocation CheckLadderLocation(Collider[] colliders, Transform transform, bool considerDirection)
        {
            var ret = LadderLocation.Null;
            foreach (var collider in colliders)
            {
                var directionCondition = considerDirection &&
                                         Mathf.Acos(Vector3.Dot(transform.forward, collider.transform.forward)) *
                                         Mathf.Rad2Deg > 10f;
                
//                _logger.InfoFormat("angle:  {0}", Mathf.Acos(Vector3.Dot(transform.forward, collider.transform.forward)) *
//                                                  Mathf.Rad2Deg);
                
                if (null == collider || directionCondition) continue;
                
                if (collider.gameObject.CompareTag(LadderBottomTag))
                    return LadderLocation.Bottom;
                
                if (collider.gameObject.CompareTag(LadderTopTag))
                    return LadderLocation.Top;

                if (collider.gameObject.CompareTag(LadderMiddleTag))
                    ret = LadderLocation.Middle;
            }
            
            return ret;
        }
    }
}
