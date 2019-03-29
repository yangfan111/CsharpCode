using Core.Prediction.UserPrediction.Cmd;
using Sharpen;

namespace com.wd.free.skill
{
    public interface IPlayerInput
    {
        void SetUserCmd(IUserCmd cmd);
        bool IsPressed(int key);
    }
}
