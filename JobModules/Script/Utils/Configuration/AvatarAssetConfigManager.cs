using System.Collections.Generic;
using Utils.AssetManager;
using Core.Utils;
using Shared.Scripts;
using UnityEngine;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Configuration
{
    public class AvatarAssetConfigManager :  AbstractConfigManager<AvatarAssetConfigManager>
    {
       
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AvatarAssetConfigManager));
        private Dictionary<int, AvatarAssetItem> _avatarAssetConfigs = new Dictionary<int, AvatarAssetItem>();

        public AvatarAssetConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("AvatarAsset config xml is empty !");
                return;
            }
            _avatarAssetConfigs.Clear();
            var cfg = XmlConfigParser<AvatarAssetConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("AvatarAsset config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _avatarAssetConfigs.Add(item.Id, item);
            }
        }

        public AssetInfo GetAssetInfoById(int id)
        {
            AssetInfo info = new AssetInfo();
            info.AssetName = _avatarAssetConfigs[id].AssetName;
            info.BundleName = _avatarAssetConfigs[id].BundleName;
            return info;
        }

        public AssetInfo GetSecondAssetInfoById(int id)
        {
            AssetInfo info = new AssetInfo();
            info.AssetName = _avatarAssetConfigs[id].SecondRes;
            info.BundleName = _avatarAssetConfigs[id].BundleName;
            return info;
        }

        public bool HaveSecondAsset(int id)
        {
            return "0" != _avatarAssetConfigs[id].SecondRes;
        }

        public bool NeedMappingBones(int id)
        {
            return _avatarAssetConfigs[id].NeedMapping;
        }

        public Wardrobe[] GetHideAvatars(int id)
        {
            List<int> avatars = _avatarAssetConfigs[id].HideAvatars;
            Wardrobe[] arr = new Wardrobe[avatars.Count];
            for (int i = 0; i < avatars.Count; i++)
            {
                arr[i] = (Wardrobe) avatars[i];
            }
            return arr;
        }

        //当前部位的类型
        public Wardrobe GetAvatarType(int id)
        {
            if (_avatarAssetConfigs.ContainsKey(id))
                return (Wardrobe)_avatarAssetConfigs[id].AvatarType;
            else
                Logger.Error("AvatarIdNotInAvatarAssetConfig:   " + id);
            return Wardrobe.Cap;
        }

        public bool IsModelPart(Wardrobe pos)
        {
            return Wardrobe.CharacterFoot == pos ||
                Wardrobe.CharacterGlove == pos ||
                Wardrobe.CharacterHair == pos ||
                Wardrobe.CharacterHairContainer == pos ||
                Wardrobe.CharacterHead == pos ||
                Wardrobe.CharacterInner == pos ||
                Wardrobe.CharacterTrouser == pos;
        }

        public AssetInfo GetSceneAssetById(int id)
        {
            if(_avatarAssetConfigs.ContainsKey(id))
            {
                return new AssetInfo
                {
                    BundleName = _avatarAssetConfigs[id].BundleNameInside,
                    AssetName = _avatarAssetConfigs[id].AssetName + "_PickUp",
                };
            }
            else
            {
                return new AssetInfo();
            }
        }

        public Dictionary<int,AvatarAssetItem> GetAvatarAssetItems()
        {
            return _avatarAssetConfigs;
        }

        public AvatarAssetItem GetAvatarAssetItemById(int id)
        {
            AvatarAssetItem ret;
            if (!_avatarAssetConfigs.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist avater item id: {0}", id);
            }
            return ret;
        }

    }
}
