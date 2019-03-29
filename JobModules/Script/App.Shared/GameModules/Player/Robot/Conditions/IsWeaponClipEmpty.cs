using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime.Tasks;

namespace Assets.App.Shared.GameModules.Player.Robot.Conditions
{
    [TaskCategory("Voyager ")]
    [TaskDescription("检查当前武器是否还有子弹")]
    class IsWeaponClipEmpty:Conditional
    {
    }
}
