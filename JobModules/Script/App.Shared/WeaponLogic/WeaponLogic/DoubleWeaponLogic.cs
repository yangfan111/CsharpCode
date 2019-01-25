using Core.Prediction.UserPrediction.Cmd;

namespace Core.WeaponLogic
{
    public class DoubleWeaponLogic : IWeaponLogic
    {
        private IFireLogic _leftFireLogic;
        private IFireLogic _rightFireLogic;
        private LeftWeaponCmd _leftcmd = new LeftWeaponCmd();
        private RightWeaponCmd _rightCmd = new RightWeaponCmd();
        public DoubleWeaponLogic(IFireLogic leftFireLogic, IFireLogic rightFireLogic)
        {
            _leftFireLogic = leftFireLogic;
            _rightFireLogic = rightFireLogic;
        }

        public void Update(PlayerEntity playerEntity, WeaponEntity weaponEntity, IUserCmd cmd)
        {
            _leftcmd.SetCurrentCmd(cmd);
            _rightCmd.SetCurrentCmd(cmd);
            _leftFireLogic.OnFrame(playerEntity, weaponEntity, _leftcmd);
            _rightFireLogic.OnFrame(playerEntity, weaponEntity, _rightCmd);
        }
    }
}