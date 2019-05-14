using App.Shared.Player;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Utils;

namespace App.Shared.GameModules.Player.Actions
{
    public static class ClimbUpCollisionTest
    {
        //private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClimbUpCollisionTest));
        
        private static RaycastHit _hit;
        private static Vector3 _overlapPos = Vector3.zero;
        private static Vector3 _capsuleBottom = Vector3.zero;
        private static Vector3 _capsuleUp = Vector3.zero;

        private static Quaternion _matchRotation = Quaternion.identity;
        private static Vector3 _matchTarget = Vector3.one;
        private static Vector3 _matchForward = Vector3.zero;
        private static Vector3 _matchRight = Vector3.zero;
        
        private static GenericActionKind _kind = GenericActionKind.Null;
        private static float _capsuleRadius;
        private static float _capsuleHeight;

        private const float VaultPly = 0.6f;

        private const float VerticalDistanceDeviation = 0.1f;

        private const float ClimbHeightOne = 0.5f;
        private const float ClimbHeightTwo = 1f;
        private const float ClimbHeightThree = 1.5f;
        private const float ClimbHeightFour = 2f;

        private const float AllRoundCastZoOffset = 0.5f;
        private const float AllRoundCastYoOffsetOne = 2.4f;
        private const float AllRoundCastYoOffsetTwo = 1.5f;

        // 探测前方1m是否有障碍物
        public static bool ClimbUpFrontDistanceTest(PlayerEntity player)
        {
            if (null == player) return false;
            CreateData(player);
            var playerTransform = player.RootGo().transform;

            var ret = Physics.CapsuleCast(_capsuleBottom, _capsuleUp, 0.1f, playerTransform.forward, out _hit, 1,
                UnityLayers.ClimbCollisionLayerMask);

//            if (ret)
//            {
//                //_logger.InfoFormat("PlayerDirection:  {0}", playerTransform.forward.ToStringExt());
//                
//                DebugDraw.DebugWireSphere(_hit.point, 0.1f, 10.0f);
//
//                Draw.DrawLine(_hit.point/* + 2.0f * _hit.normal*/, _hit.point + 2.0f * -_hit.normal, Color.red, 10.0f);
//            }

            return ret && _hit.distance < _capsuleRadius + 0.1f;
        }

        public static void ClimbUpTypeTest(PlayerEntity player, out GenericActionKind climbUpKind,
            out float yTranslateOffset, out float yRotationOffset)
        {
            yRotationOffset = CreateMatchQuaternion(player.RootGo().transform.forward, -_hit.normal);
            DownRayTest(player, out climbUpKind, out yTranslateOffset);
            if (GenericActionKind.Null == climbUpKind)
                AllRoundRayTest(player, out climbUpKind, out yTranslateOffset);
        }

        // 自上而下
        private static void DownRayTest(PlayerEntity player, out GenericActionKind kind, out float yTranslateOffset)
        {
            var sphereCenter = new Vector3(_hit.point.x,
                _hit.collider.bounds.center.y + _hit.collider.bounds.extents.y + _capsuleHeight, _hit.point.z);

            if (SourceInCollisionTest(sphereCenter, out kind, out yTranslateOffset)) return;

            RaycastHit sphereHit;
            Physics.SphereCast(sphereCenter, 0.3f, Vector3.down, out sphereHit,
                _hit.collider.bounds.center.y + _hit.collider.bounds.extents.y + _capsuleHeight,
                UnityLayers.SceneCollidableLayerMask);

            if (VaultUpTest(sphereHit.point, player, out kind, out yTranslateOffset)) return;
            ClimbUpTest(sphereHit.point, player, out kind, out yTranslateOffset);
        }

        //四方向,镂空模型测试
        private static void AllRoundRayTest(PlayerEntity player, out GenericActionKind kind, out float yTranslateOffset)
        {
            if (RoundTestByYDistance(AllRoundCastYoOffsetOne, player, out kind, out yTranslateOffset)) return;
            RoundTestByYDistance(AllRoundCastYoOffsetTwo, player, out kind, out yTranslateOffset);
        }

        private static bool RoundTestByYDistance(float yDistance, PlayerEntity player, out GenericActionKind kind,
            out float yTranslateOffset)
        {
            _kind = kind = GenericActionKind.Null;
            yTranslateOffset = 0.0f;
            var playerTransform = player.RootGo().transform;

            // boxCast 测试前方向是否有障碍物
            var castStartPos = playerTransform.position + playerTransform.up * yDistance;
            if (Physics.BoxCast(castStartPos, new Vector3(0.05f, 0.05f, 0.05f), _matchForward,
                playerTransform.rotation,
                1.0f, UnityLayers.SceneCollidableLayerMask)) return false;

            var castCenter = castStartPos + _matchForward * AllRoundCastZoOffset;
            AllRoundRayTestImpl(castCenter, player, out kind, out yTranslateOffset);

            return GenericActionKind.Null != kind;
        }

        private static void AllRoundRayTestImpl(Vector3 castCenter, PlayerEntity player, out GenericActionKind kind,
            out float yTranslateOffset)
        {
            _kind = kind = GenericActionKind.Null;
            yTranslateOffset = 0.0f;

            Vector3 top, bottom, left, right;
            CalcBorderHitPoint(castCenter, Vector3.up, _matchRotation, out top);
            CalcBorderHitPoint(castCenter, -Vector3.up, _matchRotation, out bottom);
            CalcBorderHitPoint(castCenter, -_matchRight, _matchRotation, out left);
            CalcBorderHitPoint(castCenter, _matchRight, _matchRotation, out right);

            if (Vector3.Distance(top, bottom) < 1.0f || Vector3.Distance(left, right) < 0.8f) return;

            if (VaultUpTest(bottom, player, out kind, out yTranslateOffset)) return;
            ClimbUpTest(bottom, player, out kind, out yTranslateOffset);
        }

        private static void CalcBorderHitPoint(Vector3 castCenter, Vector3 direction, Quaternion rotation,
            out Vector3 borderPoint)
        {
            RaycastHit hitInfo;
            if (!Physics.BoxCast(castCenter, new Vector3(0.1f, 0.1f, 0.5f), direction, out hitInfo, rotation,
                2.0f, UnityLayers.SceneCollidableLayerMask))
            {
                borderPoint = castCenter + direction * 2.0f;
                //DebugDraw.DebugWireSphere(borderPoint, Color.black, 0.2f, 10.0f);
            }
            else
            {
                borderPoint = hitInfo.point;
                //DebugDraw.DebugWireSphere(borderPoint, Color.red, 0.1f, 10.0f);
            }
        }

        private static void ClimbUpTest(Vector3 collisionPoint, PlayerEntity player, out GenericActionKind kind,
            out float yTranslateOffset)
        {
            _kind = kind = GenericActionKind.Null;
            yTranslateOffset = 0.0f;

            var playerTransform = player.RootGo().transform;
            var distance = collisionPoint.y - playerTransform.position.y;
            if (distance < 0.3 || distance > 2.3) return;

            //最终位置能否站人
            var overlapPos = collisionPoint + playerTransform.up * 0.2f;
            PlayerEntityUtility.GetCapsule(player, overlapPos, out _capsuleBottom, out _capsuleUp,
                out _capsuleRadius);
            if (Physics.OverlapCapsule(_capsuleBottom, _capsuleUp, _capsuleRadius, UnityLayers.SceneCollidableLayerMask)
                    .Length > 0) return;

            _kind = kind = CheckClimbKindByDistance(distance, GenericActionKind.Climb50Cm, out yTranslateOffset);
            _matchTarget = collisionPoint;
        }

        private static bool VaultUpTest(Vector3 collisionPoint, PlayerEntity player, out GenericActionKind kind,
            out float yTranslateOffset)
        {
            _kind = kind = GenericActionKind.Null;
            yTranslateOffset = 0.0f;
            
            var playerTransform = player.RootGo().transform;
            var distance = collisionPoint.y - playerTransform.position.y;
            if (distance < 0.3 || distance > 2.3) return false;

            // 检测翻越过程中障碍
            var testPoint = new Vector3(playerTransform.position.x, collisionPoint.y, playerTransform.position.z);
            _capsuleBottom = testPoint + playerTransform.up * 0.3f;
            _capsuleUp = _capsuleBottom + playerTransform.up * 0.5f;
            if (Physics.CapsuleCast(_capsuleBottom, _capsuleUp, 0.1f, _matchForward,
                1, UnityLayers.SceneCollidableLayerMask)) return false;

            // 
            _overlapPos = playerTransform.position + _matchForward * (VaultPly + 2.0f * _capsuleRadius - PlayerEntityUtility.CcSkinWidth);
            _overlapPos.y = (collisionPoint + -playerTransform.up * 0.45f).y;
            
            PlayerEntityUtility.GetCapsule(player, _overlapPos, out _capsuleBottom, out _capsuleUp,
                out _capsuleRadius);
            
//            DebugDraw.DebugWireSphere(_overlapPos, Color.red, 0.4f, 10.0f);
//            DebugDraw.DebugWireSphere(_capsuleBottom, Color.yellow, 0.4f, 10.0f);
//            DebugDraw.DebugWireSphere(_capsuleUp, Color.yellow, 0.4f, 10.0f);

            if (Physics.OverlapCapsule(_capsuleBottom, _capsuleUp, _capsuleRadius,
                    UnityLayers.SceneCollidableLayerMask).Length > 0) return false;

            _kind = kind = CheckClimbKindByDistance(distance, GenericActionKind.Vault50Cm, out yTranslateOffset);
            _matchTarget = collisionPoint;
            return true;
        }

        private static GenericActionKind CheckClimbKindByDistance(float distance, GenericActionKind kind, out float yOffset)
        {
            if (distance > 0.4 && distance <= 0.9)
            {
                yOffset = distance - 0.5f;
                return (GenericActionKind) ((int) kind + 0);
            }

            if (distance > 0.9 && distance <= 1.4)
            {
                yOffset = distance - 1.0f;
                return (GenericActionKind) ((int) kind + 1);
            }

            if (distance > 1.4 && distance <= 1.9)
            {
                yOffset = distance - 1.5f;
                return (GenericActionKind) ((int) kind + 2);
            }

            if (distance > 1.9 && distance <= 2.3)
            {
                yOffset = distance - 2.0f;
                return (GenericActionKind) ((int) kind + 3);
            }

            yOffset = 0;
            return GenericActionKind.Null;
        }

        private static bool SourceInCollisionTest(Vector3 sourcePoint, out GenericActionKind kind,
            out float yTranslateOffset)
        {
            _kind = kind = GenericActionKind.Null;
            yTranslateOffset = 0.0f;
            return Physics.OverlapSphere(sourcePoint, 0.3f, UnityLayers.SceneCollidableLayerMask).Length > 0;
        }

        private static void CreateData(PlayerEntity player)
        {
            if (null == player) return;
            var playerTransform = player.RootGo().transform;
            _overlapPos = playerTransform.position + playerTransform.up * 0.1f;

            PlayerEntityUtility.GetCapsule(player, _overlapPos, out _capsuleBottom,
                out _capsuleUp, out _capsuleRadius);
            _capsuleHeight = _capsuleUp.y - _capsuleBottom.y;

            _capsuleRadius += PlayerEntityUtility.CcSkinWidth;
        }

        private static float CreateMatchQuaternion(Vector3 playerDirection, Vector3 direction)
        {
            var dir = new Vector3(direction.x, 0, direction.z).normalized;
            
            _matchRotation = Quaternion.LookRotation(dir);
            _matchForward = _matchRotation.Forward().normalized;
            _matchRight = _matchRotation.Right().normalized;

            var angle = Mathf.Acos(Vector3.Dot(playerDirection, dir)) * Mathf.Rad2Deg;
            
            if(Vector3.Dot(Vector3.Cross(playerDirection, dir).normalized, Vector3.up) >= 0)
                return angle;
            return -angle;
        }

        public static void ClimbStateFreeFallTest(PlayerEntity player, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            if (!CanTestFreeFall(player) || OverlapCapsuleTest(player) || IsHitGround(player)) return;

            var item = commands.GetAvailableItem(command => command.Type == FsmInput.None);
            item.Type = FsmInput.Freefall;
        }

        private static bool CanTestFreeFall(PlayerEntity player)
        {
            if (null == player || !player.hasThirdPersonAnimator) return false;
            var thirdPersonAnimator = player.thirdPersonAnimator.UnityAnimator;
            return thirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClimbEnd");
        }

        private static bool OverlapCapsuleTest(PlayerEntity playerEntity)
        {
            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            var prev = IntersectionDetectTool.SetColliderDisable(gameObject);

            var overlapPos = gameObject.transform.position;
            PlayerEntityUtility.GetCapsule(playerEntity, overlapPos, out _capsuleBottom, out _capsuleUp,
                out _capsuleRadius);
            var casts = Physics.OverlapCapsule(_capsuleBottom, _capsuleUp, _capsuleRadius,
                UnityLayers.AllCollidableLayerMask);

            IntersectionDetectTool.RestoreCollider(gameObject, prev);

            return casts.Length > 0;
        }

        private static bool IsHitGround(PlayerEntity playerEntity)
        {
            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            var prev = IntersectionDetectTool.SetColliderDisable(gameObject);

            var startPoint = gameObject.transform.position;
            startPoint.y += _capsuleRadius;

            RaycastHit outHit;
            var dist = CheckHitGroundDistance();
            var hitYDistance = dist /* - ClimbStartYOffset*/;
            var isHit = Physics.SphereCast(startPoint, _capsuleRadius, Vector3.down, out outHit, hitYDistance,
                UnityLayers.AllCollidableLayerMask);

            IntersectionDetectTool.RestoreCollider(gameObject, prev);

            // dist高度没碰到，直接返回false
            // dist高度碰到了，但是碰撞点距离手扶点高度小于dist - VerticalDistanceDeviation 或大于dist + VerticalDistanceDeviation
            return isHit && _matchTarget.y - outHit.point.y >= dist - VerticalDistanceDeviation &&
                   _matchTarget.y - outHit.point.y <= dist + VerticalDistanceDeviation;
        }

        private static float CheckHitGroundDistance()
        {
            switch (_kind)
            {
                case GenericActionKind.Vault50Cm:
                case GenericActionKind.Climb50Cm:
                    return ClimbHeightOne;
                case GenericActionKind.Vault1M:
                case GenericActionKind.Climb1M:
                    return ClimbHeightTwo;
                case GenericActionKind.Vault150Cm:
                case GenericActionKind.Climb150Cm:
                    return ClimbHeightThree;
                case GenericActionKind.Vault2M:
                case GenericActionKind.Climb2M:
                    return ClimbHeightFour;
                case GenericActionKind.Null:
                    return 0;
                default:
                    return 0;
            }
        }
    }
}
