using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Singleton;

namespace Core.SessionState
{
    public class SessionStateOnGuiSystem : IOnGuiSystem
    {
        LoggerAdapter logger = new LoggerAdapter(typeof(SessionStateOnGuiSystem));
        private ISessionState _state;

        public SessionStateOnGuiSystem(ISessionState state)
        {
            _state = state;
        }

        public void OnGUI()
        {
            if (!SingletonManager.Get<SubProgressBlackBoard>().Enabled)
            {
                GUIStyle bb = new GUIStyle();
                bb.normal.background = null; //这是设置背景填充的
                bb.normal.textColor = new Color(1.0f, 0f, 0.0f); //设置字体颜色的
                bb.fontSize = 12; //当然，这是字体大小
                var name = _state.GetType().ToString();
                GUI.Label(new Rect(0, 200, 1000, 220),
                    string.Format("SessionState :{0}", name));
                int h = 200;
                foreach (var stateCondition in _state.Conditions)
                {
                    h += 20;
                    GUI.Label(new Rect(0, h, 10000, h + 20),
                        string.Format("              {0}", stateCondition));
                }
            }
            
        }
    }
}