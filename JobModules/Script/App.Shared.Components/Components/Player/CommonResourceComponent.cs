using App.Shared.CommonResource;
using Core.Appearance;
using Core.CharacterBone;
using Core.CharacterController;
using Core.CommonResource;
using Core.Components;
using Entitas.CodeGeneration.Attributes;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.Components.Player
{
    public enum EPlayerCommonResourceType
    {
        FirstPlayer,
        ThirdPlayer,
        HitBoxs,
        Parachute,
        WardrobeStart,
        LatestWeaponStateStart = WardrobeStart + Wardrobe.EndOfTheWorld - 1,
        End = LatestWeaponStateStart + LatestWeaponStateIndex.EndOfTheWorld - 1
              
    }

   
    
    
    [Player]
    public class PlayerResourceComponent : AbstractCommonResourceComponent
    {
        private int _resourceLength;
        public GameObject Root;
      

        public override int GetComponentId()
        {
            return (int) EComponentIds.PlayerResource;
        }

        protected override int ResourceLength
        {
            get { return (int) EPlayerCommonResourceType.End - 1; }
        }


        public AssetStatus GetResource(EPlayerCommonResourceType playerCommonResourceType)
        {
            return Resources[(int) playerCommonResourceType];
        }
    }
}