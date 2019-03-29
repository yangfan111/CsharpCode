using System;
using System.Collections.Generic;
using Entitas;

namespace Core.SessionState
{
    public interface ISessionCondition : IDisposable
    {
        void CreateExitCondition(string conditionId);
        void FullfillExitCondition(string conditionId);
        void CreateExitCondition(Type conditionId);
        void FullfillExitCondition(Type conditionId);
        bool IsFullfilled { get; }
    }
    public interface ISessionState:ISessionCondition
    {
        int StateId { get; }
        int NextStateId { get; }
        void Initialize();
        void Leave();
        Systems GetUpdateSystems();
        Systems GetOnDrawGizmos();
        Systems GetLateUpdateSystems();

      

        Systems GetOnGuiSystems();
        HashSet<string> Conditions { get; }

        int LoadingProgressNum { get; }
        string LoadingTip { get; }
       
    }
}