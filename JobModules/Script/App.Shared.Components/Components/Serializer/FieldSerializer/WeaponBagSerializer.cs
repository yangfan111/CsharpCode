using System.Collections.Generic;
using System.IO;
using App.Shared.Components.Player;
using Core;
using Core.Event;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    internal class WeaponBagSerializer
    {
        public void Write(WeaponBagContainer data, MyBinaryWriter writer)
        {
           
            data.Write(writer);
        }

        public WeaponBagContainer Read(BinaryReader reader,  WeaponBagContainer data)
        {
            if(data == null)
                data = new WeaponBagContainer();
            data.Read(reader);
            return data;
        }
    }
}