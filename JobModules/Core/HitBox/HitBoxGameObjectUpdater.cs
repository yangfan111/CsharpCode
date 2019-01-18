using Core.Utils;
using UnityEngine;

namespace Core.HitBox
{
    public class HitBoxGameObjectUpdater
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxGameObjectUpdater));

        public static void Update(Transform hitbox, IHitBoxTransformProvider transformProvider)
        {
            UpdateBones(hitbox.GetChild(0), transformProvider);
            hitbox.transform.position = transformProvider.RootPosition;
            hitbox.transform.rotation = transformProvider.RootRotation;
        }
        private static void UpdateBones(Transform hitbox, IHitBoxTransformProvider transformProvider)
        {
            for (int i = 0; i < hitbox.childCount; i++)
            {
                var child = hitbox.GetChild(i);
                
                Transform modelTransform = transformProvider.GetTransform(child);
                if (modelTransform != null)
                {
                    child.localPosition = modelTransform.transform.localPosition;
                    child.localRotation = modelTransform.transform.localRotation;
                }
                else
                {
                    _logger.DebugFormat("can't find hitbox node [{0}] in model {1}", child, transformProvider);
                }

                UpdateBones(child, transformProvider);
            }
        }

        public static void DrawBoundBox(Transform hitbox, float duration)
        {
            var bc = hitbox.GetComponent<BoxCollider>();
            if (bc != null && bc.enabled)
            {
                DrawBoxCollider(bc, hitbox, duration);
            }

            foreach (Transform child in hitbox.transform)
            {
                DrawBoundBox(child, duration);
            }
        }

        private static void DrawBoxCollider(BoxCollider boxCollider, Transform transform, float duration)
        {
            var min = boxCollider.center - boxCollider.size / 2;
            var max = boxCollider.center + boxCollider.size/2;
            var points = new Vector3[8];
            points[0] = new Vector3(min.x, min.y, min.z);
            points[1] = new Vector3(max.x, min.y, min.z);
            points[2] = new Vector3(max.x, max.y, min.z);
            points[3] = new Vector3(min.x, max.y, min.z);

            points[4] = new Vector3(min.x, min.y, max.z);
            points[5] = new Vector3(max.x, min.y, max.z);
            points[6] = new Vector3(max.x, max.y, max.z);
            points[7] = new Vector3(min.x, max.y, max.z);

            var upoints2 = new Vector3[8];
            for (var i = 0; i < 8; i++)
            {
                upoints2[i] = transform.TransformPoint(points[i]);
            }

            var color = Color.red;
            RuntimeDebugDraw.Draw.DrawLine(upoints2[0], upoints2[4], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[1], upoints2[5], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[2], upoints2[6], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[3], upoints2[7], color, duration);

            RuntimeDebugDraw.Draw.DrawLine(upoints2[0], upoints2[1], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[4], upoints2[5], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[3], upoints2[2], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[6], upoints2[7], color, duration);


            RuntimeDebugDraw.Draw.DrawLine(upoints2[0], upoints2[3], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[1], upoints2[2], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[4], upoints2[7], color, duration);
            RuntimeDebugDraw.Draw.DrawLine(upoints2[5], upoints2[6], color, duration);
        }
    }
}
