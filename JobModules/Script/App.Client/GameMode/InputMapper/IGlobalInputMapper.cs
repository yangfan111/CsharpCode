using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Client.GameMode
{
    public interface IGlobalKeyInputMapper
    {
        void RegisterEnvKeyInput(KeyHandler keyHandler, UserCmd userCmd);
        void RegisterSpecialCmdKeyInput(KeyHandler keyHandler, UserCmd userCmd);
    }
}
