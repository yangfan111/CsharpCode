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
        public static Collider[]colliders = new Collider[512];
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

            var length = Physics.OverlapSphereNonAlloc(capsuleBottom, capsuleRadius, colliders, UnityLayers.ClimbLadderLayerMask);
            if (length == 0) return false;


            location = CheckLadderLocation(colliders, length, playerTransform, considerDirection);

            return location != LadderLocation.Null;
        }

        private static LadderLocation CheckLadderLocation(Collider[] colliders, int length, Transform transform, bool considerDirection)
        {
            var ret = LadderLocation.Null;
            for (var i = 0; i < length; i++)
            {
                var collider = colliders[i];
            
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
