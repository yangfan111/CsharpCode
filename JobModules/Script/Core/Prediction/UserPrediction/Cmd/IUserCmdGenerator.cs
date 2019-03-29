using System;
using UnityEngine;


namespace Core.Prediction.UserPrediction.Cmd
{
    public enum CmdGeneratorType{
        Hunam,
        Robot

    }

    public interface IUserCmdOwnAdapter
    {
        float Yaw { get; }
        float Pitch { get; }
        Vector3 Position { get; }
        Transform PlayerTransform { get;  }
    }
    public interface IUserCmdGenerator
    {
        CmdGeneratorType Type {get;}
        UserCmd GenerateUserCmd(IUserCmdOwnAdapter player, int intverval);
        void SetLastUserCmd(UserCmd userCmd);
        void SetUserCmd(Action<UserCmd> cb);
        void MockUserCmd(Action<UserCmd> cb);
    }
}