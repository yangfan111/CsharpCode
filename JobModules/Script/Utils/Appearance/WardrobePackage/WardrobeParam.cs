using System;
using Shared.Scripts;
using System.Linq;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Appearance.WardrobePackage
{
    public class WardrobeParam
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WardrobeParam));
        public UnityObject DefaultGameObject;
        public UnityObject AlternativeGameObject;
        
        public int Id { get; private set; }

        // 装扮的位置
        public Wardrobe Type { get; private set; }
        // 是否是蒙皮
        public bool IsSkinned { get; private set; }
        // 默认显示
        public AssetInfo P3DefaultResAddr { get; private set; }
        // 是否有第二套的显示
        public bool HasAlterAppearance { get; private set; }
        // 被指定二套换装时的显示
        public AssetInfo AlterResAddr { get; private set; }

        // 让其他部位更换为二套换装
        public bool EnableOtherAlter { get; private set; }
        // 二套换装的部位
        public Wardrobe AlterType { get; private set; }
        
        // 是否能隐藏其他部位
        public bool HasHideAvatar { get; private set; }
        // 隐藏的其他部位
        public Wardrobe[] HidePositions { get; private set; }

        // 是否有一人称换装
        public bool HaveP1Avatar { get; private set; }
        public AssetInfo P1DefaultResAddr { get; private set; }

        // 是否需要映射骨骼
        public bool NeedMappingBones { get; private set; }

        // 通过mask确保不穿帮的其他部位
        public Tuple<Wardrobe, Texture>[] Masks { get; private set; }

        private static readonly int DefaultMaterialId = 0;

        public WardrobeParam(AvatarAssetItem originData)
        {
            Id = originData.Id;
            Type = (Wardrobe)originData.AvatarType;
            IsSkinned = originData.IsSkinned;
            P3DefaultResAddr = new AssetInfo(originData.BundleName, originData.AssetName);
            HasAlterAppearance = "0" != originData.SecondRes;
            AlterResAddr = HasAlterAppearance
                           ? new AssetInfo(originData.BundleName, originData.SecondRes)
                           : new AssetInfo();

            if (originData.HaveSecond)
            {
                EnableOtherAlter = true;
                AlterType = (Wardrobe)originData.SecondType;
            }

            HidePositions = (HasHideAvatar = originData.HaveHideAvatar)
                            ? SingletonManager.Get<AvatarAssetConfigManager>().GetHideAvatars(originData.Id)
                            : null;

            HaveP1Avatar = originData.HaveP1Avatar;
            P1DefaultResAddr = HaveP1Avatar ?
                new AssetInfo(originData.BundleName, originData.AssetName + "_Sleeve") : new AssetInfo();

            NeedMappingBones = SingletonManager.Get<AvatarAssetConfigManager>().NeedMappingBones(originData.Id);

        }

        public void PrepareMasks()
        {
            if(DefaultGameObject.AsGameObject == null)
            {
                return;
            }
            var comp = DefaultGameObject.AsGameObject.GetComponent<CharacterMaskParams>();
            Masks = comp != null
                ? comp.AvatarItems.Select(x => Tuple.Create(x.Type, x.Tex)).ToArray()
                : null;
        }

        
        public void ApplySkin(int skinId)
        {
            //Logger.InfoFormat("apply skinid:{0}", skinId);
            if(DefaultGameObject.AsGameObject == null || skinId == DefaultMaterialId)
            {
                return;
            }
            var config = SingletonManager.Get<AvatarSkinConfigManager>().GetSkinConfigById(skinId, Id);
            if (config == null)
            {
                return;
            }
            var meshRenders = DefaultGameObject.AsGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            try
            {
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenders)
                {
                    if (skinnedMeshRenderer.material != null)
                    {
                        MaterialHelper.SetParam(skinnedMeshRenderer.material, config);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("{0}",e.ToString());
            }
            
        }
    }
}
