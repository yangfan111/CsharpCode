using System;
using System.Runtime.CompilerServices;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas.CodeGeneration.Attributes;

// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    
    public class VehicleSeatComponent : INonSelfLatestComponent, IUserPredictionComponent, IVehicleResetableComponent
    {
        public static int MaxSeatCount = 6;

        public int GetComponentId() { { return (int)EComponentIds.VehicleSeat; } }

        [DontInitilize]
        public int OriginalConfigFlag;

        [NetworkProperty]
        [DontInitilize]
        public int ConfigFlag;

        [NetworkProperty]
        [DontInitilize]
        public int OccupationFlag;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey1;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey2 ;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey3;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey4;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey5;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey SeatedEntityKey6;

        [NetworkProperty]
        [DontInitilize]
        public EntityKey LastDriverEntityKey;


        public void CopyFrom(object rightComponent)
        {
            var r = (rightComponent) as VehicleSeatComponent;
            ConfigFlag = r.ConfigFlag;
            OccupationFlag = r.OccupationFlag;
            SetEntityKey(r);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as VehicleSeatComponent;
            return ConfigFlag == r.ConfigFlag &&
                   OccupationFlag == r.OccupationFlag &&
                   SeatedEntityKey1 == r.SeatedEntityKey1 &&
                   SeatedEntityKey2 == r.SeatedEntityKey2 &&
                   SeatedEntityKey3 == r.SeatedEntityKey3 &&
                   SeatedEntityKey4 == r.SeatedEntityKey4 &&
                   SeatedEntityKey5 == r.SeatedEntityKey5 &&
                   SeatedEntityKey6 == r.SeatedEntityKey6 &&
                   LastDriverEntityKey == r.LastDriverEntityKey;
        }

       

        private void SetEntityKey(VehicleSeatComponent r)
        {
            SeatedEntityKey1 = r.SeatedEntityKey1;
            SeatedEntityKey2 = r.SeatedEntityKey2;
            SeatedEntityKey3 = r.SeatedEntityKey3;
            SeatedEntityKey4 = r.SeatedEntityKey4;
            SeatedEntityKey5 = r.SeatedEntityKey5;
            SeatedEntityKey6 = r.SeatedEntityKey6;
            LastDriverEntityKey = r.LastDriverEntityKey;
        }

        public void SetAllSeatBroken()
        {
            OriginalConfigFlag = ConfigFlag;
            ConfigFlag = 0;
        }

        public void SetOccupation(int seatId, EntityKey key, bool isDriverSeat)
        {
            OccupationFlag |= 1 << seatId;
            SetEntityKey(seatId, key);
            if (isDriverSeat)
            {
                LastDriverEntityKey = key;
            }
        }

        public void RemoveOccupation(int seatId)
        {
            OccupationFlag &= ~(1 << seatId);
        }

        public bool IsOccupied(int seatId)
        {
            return (OccupationFlag & (1 << seatId)) != 0;
        }

        public bool IsAnyOccupied()
        {
            return OccupationFlag != 0;
        }


        public bool IsConfigured(int seatId)
        {
            return (ConfigFlag & (1 << seatId)) != 0;
        }

        public int GetSeatIdByIndex(int index)
        {
            int seatId = -1;
            
            for (int id = 0; id <= MaxSeatCount; ++id)
            {
                if (index <= 0)
                {
                    break;
                }

                if (IsConfigured(id))
                {
                    seatId = id;
                    index--;
                }
            }

            return seatId;
        }

        public void AddSeat(int seatId)
        {
            ConfigFlag |= 1 << seatId;
        }

        public bool GetEntityKey(int seatId, out EntityKey entityKey)
        {

            if (IsOccupied(seatId))
            {
                entityKey = GetEntityKey(seatId);
                return true;
            }

            entityKey = new EntityKey();
            return false;
        }

        private EntityKey GetEntityKey(int seatId)
        {
            switch (seatId)
            {
                case 1:
                    return SeatedEntityKey1;
                case 2:
                    return SeatedEntityKey2;
                case 3:
                    return SeatedEntityKey3;
                case 4:
                    return SeatedEntityKey4;
                case 5:
                    return SeatedEntityKey5;
                case 6:
                    return SeatedEntityKey6;
                default:
                    throw new Exception(String.Format("Can not get the entity key for seat {0}", seatId));
            }
        }
      
        private void SetEntityKey(int seatId, EntityKey key)
        {
            switch (seatId)
            {
                case 1:
                    SeatedEntityKey1 = key;
                    break;
                case 2:
                    SeatedEntityKey2 = key;
                    break;
                case 3:
                    SeatedEntityKey3 = key;
                    break;
                case 4:
                    SeatedEntityKey4 = key;
                    break;
                case 5:
                    SeatedEntityKey5 = key;
                    break;
                case 6:
                    SeatedEntityKey6 = key;
                    break;
                default:
                    throw new Exception(String.Format("Can not allocate the entity key for seat {0}", seatId));
            }
        }

        public void ClearOccupation()
        {
            OccupationFlag = 0;
            SeatedEntityKey1 = SeatedEntityKey2 = EntityKey.Default;
            SeatedEntityKey3 = SeatedEntityKey4 = EntityKey.Default;
            SeatedEntityKey5 = SeatedEntityKey6 = EntityKey.Default;
        }

        public void Reset()
        {
            ConfigFlag = OriginalConfigFlag;
            OccupationFlag = 0;
            SeatedEntityKey1 = SeatedEntityKey2 = SeatedEntityKey3 = SeatedEntityKey4 = EntityKey.Default;
        }

        public override string ToString()
        {
            return string.Format("ConfigFlag: {0}, OccupationFlag: {1}", ConfigFlag, OccupationFlag);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
