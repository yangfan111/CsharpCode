using Sharpen;
using com.wd.free.para;
using com.wd.free.unit;
using gameplay.gamerule.free.action;
using Core.Free;
using com.wd.free.trigger;
using gameplay.gamerule.free.component;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.map;
using App.Shared.FreeFramework.framework.@event;
using com.wd.free.action;

namespace com.wd.free.@event
{
    public interface IEventArgs : IFreeArgs
    {
        void AddDefault(IParable para);

        IParable GetUnit(string key);

        void SetPara(string key, IParable paras);

        // 临时使用对象
        void TempUse(string key, IParable paras);

        // 释放临时使用对象
        void Resume(string key);

        // 临时使用全局变量
        void TempUsePara(IPara para);

        // 释放全局临时使用变量
        void ResumePara(string paraName);

        IParable RemovePara(string key);

        IParable GetDefault();

        Contexts GameContext { get; }

        FreeContext FreeContext { get; }

        string[] GetUnitKeys();

        CommonActions Functions
        {
            get;
        }

        GameTriggers Triggers
        {
            get;
        }

        GameComponentMap ComponentMap
        {
            get; set;
        }

        GameUnitSet GetGameUnits();

        void Trigger(int eventId);

        void Trigger(int eventId, TriggerArgs args);

        void Trigger(int eventId, IPara para);

        void Trigger(int eventId, params TempUnit[] units);

        void Trigger(int eventId, TempUnit[] units, IPara[] paras);

        void Act(IGameAction action);

        void Act(IGameAction action, TriggerArgs args);

        void Act(IGameAction action, params TempUnit[] units);

        int GetInt(string v);

        float GetFloat(string v);

        bool GetBool(string v);

        double getDouble(string v);

        long GetLong(string v);

        string GetString(string v);
    }

    public static class IEventArgsConstants
    {

    }
}
