using System.Collections.Generic;
using Utils.AssetManager;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace Core.HitBox
{
    public class HitBoxHierarchyGenerator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxHierarchyGenerator));
        private HitBoxInfo _hitboxInfo;
        private Dictionary<string, GameObject> _goDic = new Dictionary<string, GameObject>();
        private EntityKey _owerEntityKey;
        private GameObject _skeletonRoot;
        private int _layer;
        public HitBoxHierarchyGenerator(HitBoxInfo hitboxInfo, EntityKey owerEntityKey, int layer)
        {
            _hitboxInfo = hitboxInfo;
            _owerEntityKey = owerEntityKey;
            _layer = layer;
        }

        public GameObject GetNode(string name)
        {
            
            if (_goDic.ContainsKey(name))
            {
                return _goDic[name];
            }

            foreach (var hitbox in _hitboxInfo.HitBoxList)
            {
                if (hitbox.Name == name)
                {
                    GameObject go = new GameObject();
                    if (HitBoxConfigParser.RootName == hitbox.Parent)
                    {
                        _skeletonRoot = go;
                    }
                    else
                    {
                        GameObject parent = GetNode(hitbox.Parent);
                        if (parent == null)
                        {
                            _logger.ErrorFormat("parent of {0} not exist", name);
                        }
                        go.transform.parent = parent.transform;
                    }
                    
                    var pc = go.AddComponent<HitBoxOwnerComponent>();
                    pc.OwnerEntityKey = _owerEntityKey;
                    pc.gameObject.layer = _layer;
                    if (hitbox is BoxHitBox)
                    {
                        var boxCollider = go.AddComponent<BoxCollider>();
                        boxCollider.center = ((BoxHitBox)hitbox).Center;
                        boxCollider.size = ((BoxHitBox)hitbox).Size;
                        boxCollider.isTrigger = true;
                    }
                    
                    go.name = hitbox.Name;
                    _goDic[name] = go;
                    return go;
                }
            }
            return null;
        }
        public GameObject GenerateHitBoxGameObject()
        {
            
            
            foreach (var hitbox in _hitboxInfo.HitBoxList)
            {
                var node = GetNode(hitbox.Name);
                node.transform.localEulerAngles = hitbox.Rotation;
                node.transform.localPosition = hitbox.Position;
                if (hitbox.Scale.x != 0)
                    node.transform.localScale = hitbox.Scale;
            }

            // todo: delete
            GameObject coodTransformer = new GameObject("coodTransformer");
            _skeletonRoot.transform.parent = coodTransformer.transform;
            coodTransformer.transform.localRotation = Quaternion.identity;

            GameObject wrapper = new GameObject("hitbox_" + _owerEntityKey);
            coodTransformer.transform.parent = wrapper.transform;

            DefaultGo.SetParentToDefaultGo(wrapper);

			return wrapper;
        }
    }
}