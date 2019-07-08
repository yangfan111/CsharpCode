using Core.Utils;
using UnityEngine;
using Utils.Appearance;

namespace Core.HitBox
{
    public class HitBoxGameObjectUpdater
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxGameObjectUpdater));


        public static void DrawBoundBox(Collider collider, float duration)
        {
            var bc = collider as BoxCollider;
            if (bc != null)
            {
                DrawBoxCollider(bc, collider.transform, duration);
                return;
            }

            var cc = collider as CapsuleCollider;
            if (cc != null)
            {
                DrawCapsuleCollider(cc, collider.transform, duration);
                return;
            }

            var sc = collider as SphereCollider;
            if (sc != null)
            {
                DrawSphereCollider(sc, collider.transform, duration);
                return;
            }

        }
        
        public static void DrawBoundBox(Transform hitbox, float duration)
        {
            var bc = hitbox.GetComponent<BoxCollider>();
            if (bc != null && bc.enabled)
            {
                DrawBoxCollider(bc, hitbox, duration);
            }

            var cc = hitbox.GetComponent<CapsuleCollider>();
            if (cc != null && cc.enabled)
            {
                DrawCapsuleCollider(cc, hitbox, duration);
            }

            var sc = hitbox.GetComponent<SphereCollider>();
            if (sc != null && sc.enabled)
            {
                DrawSphereCollider(sc, hitbox, duration);
            }

            foreach (Transform child in hitbox.transform)
            {
                DrawBoundBox(child, duration);
            }
        }

        private static void DrawSphereCollider(SphereCollider sphereCollider, Transform transform, float duration)
        {
            Vector3 size = Vector3.one * sphereCollider.radius;
            DrawABox(sphereCollider.center, size, transform, duration);
        }
        
        private static void DrawCapsuleCollider(CapsuleCollider capsuleCollider, Transform transform, float duration)
        {
            Vector3 size= new Vector3(2*capsuleCollider.radius, 2*capsuleCollider.radius, 2*capsuleCollider.radius);
            switch (capsuleCollider.direction)
            {
                case 0:
                    size.x = capsuleCollider.height;
                    break;    
                case 1:
                    size.y = capsuleCollider.height;
                    break;        
                case 2:
                    size.z = capsuleCollider.height;
                    break;
            }
            DrawABox(capsuleCollider.center,size, transform, duration);
        }

        private static void DrawBoxCollider(BoxCollider boxCollider, Transform transform, float duration)
        {
            DrawABox(boxCollider.center, boxCollider.size, transform, duration);
        }
        
        private static void DrawABox(Vector3 center, Vector3 size, Transform transform, float duration)
        {
            var min = center - size / 2;
            var max = center + size/2;
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
