using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;
using Assets.XmlConfig;
using Core;

namespace App.Shared.Components.Weapon
{
    [Player,]
    public class PlayerWeaponServerUpdateComponent : IUserPredictionComponent
    {
     

        public EWeaponUpdateCmdType EUpdateCmdType
        {
            get { return (EWeaponUpdateCmdType) UpdateCmdType; }
        }
        [DontInitilize, NetworkProperty] public byte UpdateCmdType;
        [DontInitilize, NetworkProperty] public List<int> ReservedWeaponSubType = new List<int>();

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponServerUpdate;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
           
        }

        public bool CompareArray(List<int> rReservedWeaponSubType)
        {
            for (int i = 0; i < ReservedWeaponSubType.Count; i++)
            {
                if (ReservedWeaponSubType[i] != rReservedWeaponSubType[i])
                    return false;
            }

            return true;
        }
        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as PlayerWeaponServerUpdateComponent;
            return UpdateCmdType == remote.UpdateCmdType &&
                   ReservedWeaponSubType.Count == remote.ReservedWeaponSubType.Count &&
                   CompareArray(remote.ReservedWeaponSubType);
          
        }
        public  void CopyList(List<int> l, List<int> r)
        {
            if (l == null)
                l = new List<int>();
            else
                l.Clear();

            if (r != null)
                l.AddRange(r);
        }
        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerWeaponServerUpdateComponent;
            UpdateCmdType = remote.UpdateCmdType;
            ReservedWeaponSubType = remote.ReservedWeaponSubType ;
            CopyList(ReservedWeaponSubType, remote.ReservedWeaponSubType);


        }
    }
}