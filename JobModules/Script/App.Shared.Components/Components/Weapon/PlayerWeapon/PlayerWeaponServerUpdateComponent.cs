using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;

namespace App.Shared.Components.Weapon
{
    [Player,]
    public class PlayerWeaponServerUpdateComponent : ISelfLatestComponent
    {
        [DontInitilize, NetworkProperty] public byte UpdateCmdType;
        [DontInitilize, NetworkProperty] public List<int> ReservedWeaponSubType;
    
        public int GetComponentId()    
        {
            return (int)EComponentIds.WeaponServerUpdate;
        }

        public void Initialize()
        {
            if(ReservedWeaponSubType == null)
                ReservedWeaponSubType = new List<int>();
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
            Initialize();
            remote.Initialize();
            return UpdateCmdType == remote.UpdateCmdType &&
                   ReservedWeaponSubType.Count == remote.ReservedWeaponSubType.Count &&
                   CompareArray(remote.ReservedWeaponSubType);
          
        }

        public override string ToString()
        {
            string s = "";
            ReservedWeaponSubType.ForEach(val=>s+=val+"|");
            return string.Format("UpdateCmdType:{0},ReservedWeaponSubType:{1}", UpdateCmdType, s);
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerWeaponServerUpdateComponent;
            Initialize();
            remote.Initialize();
            UpdateCmdType = remote.UpdateCmdType;
            ReservedWeaponSubType.Clear();
            ReservedWeaponSubType.AddRange(remote.ReservedWeaponSubType);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}