using Core.Utils;
using UnityEngine;

namespace Core.Prediction.VehiclePrediction.Cmd
{

    /* _______________________
   |BackDriver  | Driver   \
   |____________|___________\
   |BackCoDriver|CoDriver   /
   |____________|__________/
     */

    public enum VehicleSeatIndex
    {
        None = 0,

        Driver = 1,
        Codriver = 2,
        BackDriver = 3,
        BackCodriver = 4,
        BackDriver_1  = 5,
        BackCodriver_1 = 6,
        MaxSeatCount = 6,
    }


    public enum VehiclePartIndex
    {
        Invalid = -2,
        Body = -1,
        FirstWheel = 0,
        SecondWheel,
        ThirdWheel,
        FourthWheel,
        MaxWheelCount,
        FirstRudder,
        SecondRudder,
        MaxRudderCount = 2,
        MaxFlexibleCount = 2,
    }

    public static class VehicleIndexHelper
    {
        private static VehicleSeatIndex[] _seatIndexArray =
        {
            VehicleSeatIndex.Driver,
            VehicleSeatIndex.Codriver,
            VehicleSeatIndex.BackDriver,
            VehicleSeatIndex.BackCodriver,
            VehicleSeatIndex.BackDriver_1,
            VehicleSeatIndex.BackCodriver_1
        };

        private static VehiclePartIndex[] _wheelIndexArray =
        {
            VehiclePartIndex.FirstWheel,
            VehiclePartIndex.SecondWheel,
            VehiclePartIndex.ThirdWheel,
            VehiclePartIndex.FourthWheel
        };

        private static VehiclePartIndex[] _rudderIndexArray =
        {
            VehiclePartIndex.FirstRudder,
            VehiclePartIndex.SecondRudder
        };

        public static VehiclePartIndex[] _allPartIndexArray =
        {
            VehiclePartIndex.Body,
            VehiclePartIndex.FirstWheel,
            VehiclePartIndex.SecondWheel,
            VehiclePartIndex.ThirdWheel,
            VehiclePartIndex.FourthWheel
        };

        public static VehicleSeatIndex[] GetSeatIndexArray()
        {
            AssertUtility.Assert(_seatIndexArray.Length == (int)VehicleSeatIndex.MaxSeatCount);
            return _seatIndexArray;
        }

       
        public static VehiclePartIndex[] GetWheelIndexArray()
        {
            AssertUtility.Assert(_wheelIndexArray.Length == (int)VehiclePartIndex.MaxWheelCount);
            return _wheelIndexArray;
        }

        public static VehiclePartIndex[] GetRudderIndexArray()
        {
            AssertUtility.Assert(_rudderIndexArray.Length == (int)VehiclePartIndex.MaxRudderCount);
            return _rudderIndexArray;
        }

        public static VehiclePartIndex[] GetAllPartIndexArray()
        {
            return _allPartIndexArray;
        }


        public static int ToVehicleControllerWheelIndex(VehiclePartIndex index)
        {
            return (int) index - (int) VehiclePartIndex.FirstWheel;
        }

        public static int ToVehicleControllerRudderIndex(VehiclePartIndex index)
        {
            return (int) index - (int) VehiclePartIndex.FirstRudder;
        }
    }
}
