using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using XmlConfig.HitBox;

namespace Core.HitBox
{

    /// <summary>
    /// 碰撞信息解析器
    /// </summary>
    /// 
    public class HitBoxConfigParser
    {
        public const string RootName = "HitBoxRoot";
        public static HitBoxInfo ParseFromTransform(Transform root)
        {
            var info = new HitBoxInfo();
            Read(info, root, RootName);
            
            var sp = HitBoxConstants.FindBoundingSphereModel(root.gameObject).GetComponent<SphereCollider>();
            info.HitPreliminaryGeo = new BoundingSphere {position = sp.center, radius = sp.radius};
            info.Root = root.name;
            return info;
        }

        public static void ApplyToTransfrom(HitBoxInfo info, Transform root)
        {
            if (info.Root != null)
            {
                bool found = false;
                root.Recursively(t =>
                {
                    if (!found && t.name == info.Root)
                    {
                        root = t;
                        found = true;
                    }
                });
            }

            root.Recursively(t =>
            {
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(t.gameObject.GetComponent<Collider>());
#else
                Object.Destroy(t.gameObject.GetComponent<Collider>());
#endif
            });

            var bs = new GameObject("boundingSphere");
            bs.transform.SetParent(root, false);
            

            var sp = bs.AddComponent<SphereCollider>();
            sp.center = info.HitPreliminaryGeo.position;
            sp.radius = info.HitPreliminaryGeo.radius;

            foreach (var hitbox in info.HitBoxList)
            {
                Transform t;
                if (hitbox.Name == RootName)
                {
                    t = root;
                }
                else
                {
                    t = root.FindChildRecursively(hitbox.Name);
                }
                if (t == null)
                {
                    Debug.LogError("Can't find node " + hitbox.Name);
                    continue;
                }

                if (hitbox is BoxHitBox)
                {
                    var hbGo = new GameObject(t.name + "_hitbox");
                    hbGo.transform.SetParent(t, false);
                    var b = hbGo.gameObject.AddComponent<BoxCollider>();
                    BoxHitBox hb = (BoxHitBox)hitbox;
                    b.center = hb.Center;
                    b.size = hb.Size;
                }
            }
        }

        public static int Read(HitBoxInfo info, Transform root, string parent)
        {
            int count = 0;
            foreach (Transform t in root)
            {
                count += Read(info, t, root.parent.name);
            }

            var collider = root.gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                BoxHitBox bh = new BoxHitBox();
                bh.Center = collider.center;
                bh.Size = collider.size;
                bh.Name = parent;
                bh.Parent = parent;
                bh.Rotation = root.transform.localEulerAngles;
                bh.Position = root.transform.localPosition;
                bh.Scale = root.transform.localScale;
                info.HitBoxList.Add(bh);
                return count + 1;
            }
            else
            {
                if (count > 0)
                {
                    HitBox bh = new HitBox();
                    if (root.parent != null)
                    {
                        bh.Name = parent;
                        bh.Parent = parent;
                        bh.Rotation = root.transform.localEulerAngles;
                        bh.Position = root.transform.localPosition;
                        bh.Scale = root.transform.localScale;
                        info.HitBoxList.Add(bh);
                    }
                    else
                    {
                        Debug.LogError("node has no parent " + root.name);
                    }
                }
                return count;
            }
        }

        public static HitBoxInfo ParseFromString(string xml)
        {
            var serializer = new XmlSerializer(typeof(HitBoxInfo));
            StringReader sr = new StringReader(xml);
            HitBoxInfo info = (HitBoxInfo)serializer.Deserialize(sr);
            return info;
        }       
    }
}
