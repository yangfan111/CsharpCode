using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XmlConfig
{
    public enum EVehicleSoundType
    {
        Unkown,
        Music,
        Record,
    }


    public class VehicleSoundConfigItem
    {
        public EVehicleSoundType Type;
        public int Id;
        public int Channel;
        public float Weight;
    }

    public class VehicleSoundConfig
    {
        public VehicleSoundConfigItem[] Items;
    }


}
