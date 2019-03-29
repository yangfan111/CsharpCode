using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.Components.GenericActions
{
    public interface IGenericAction
    {
        void Update(PlayerEntity player);
        void ActionInput(PlayerEntity player);
        void PlayerReborn(PlayerEntity player);
        void PlayerDead(PlayerEntity player);
    }
}
