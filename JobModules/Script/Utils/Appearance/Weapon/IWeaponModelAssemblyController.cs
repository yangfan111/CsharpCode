﻿using UnityEngine;
using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public interface IWeaponModelAssemblyController
    {
        void Attach(WeaponPartLocation partType, object partGo, object weaponGo);
        void ShowWeapon(object weaponGo);
        void RefreshRemovableAttachment(GameObject go, bool hasSights);
    }
}
