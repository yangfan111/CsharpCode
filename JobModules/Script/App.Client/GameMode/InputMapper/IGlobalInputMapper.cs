using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Client.GameMode
{
    public interface IGlobalKeyInputMapper
    {
        void RegisterEnvKeyInput(KeyReceiver keyReceiver, UserCmd userCmd);
        void RegisterSpecialCmdKeyInput(KeyReceiver keyReceiver, UserCmd userCmd);
    }
}
