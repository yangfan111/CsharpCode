<<<<<<< HEAD
﻿using App.Shared.GameModules.Weapon;
using App.Shared.Util;
=======
﻿using App.Shared.Util;
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerActionInterruptSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (!cmd.IsInterrupt)
            {
                return;
            }
            var playerEntity = owner.OwnerEntity as PlayerEntity;
<<<<<<< HEAD
           playerEntity.GetController<PlayerWeaponController>().Interrupt();
=======
           playerEntity.GetWeaponController().Interrupt();
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
        }
    }
}
