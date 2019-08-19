using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.SessionState
{
    public struct SessionProcessInfo
    {
        public float elapseSecds;

        //index+name
        public string sessionName;
        public long startTimeticks;
        public int orderIndex;
        public bool hasFinished;

        public SessionProcessInfo(string sessionName, int orderIndex)
        {
            this.sessionName = sessionName;
            this.orderIndex  = orderIndex;
            elapseSecds      = 0;
            startTimeticks   = DateTime.Now.Ticks;
            hasFinished      = false;
        }

        public float ElapseSecds
        {
            get
            {
                if (hasFinished)
                    return elapseSecds;
                return ((float) DateTime.Now.Ticks - startTimeticks) / 10000000;
            }
        }
        public void Finish()
        {
            hasFinished = true;
            elapseSecds = ((float) DateTime.Now.Ticks - startTimeticks) / 10000000;
            SingletonManager.Get<SessionStateTimer>().ToString();
        } 
    }

    public class SessionStateTimer :Singleton<SessionStateTimer> 
    {

        private int accumulator;
        private bool isFinished;

        private Dictionary<string, SessionProcessInfo> sessionProcessInfos =
                        new Dictionary<string, SessionProcessInfo>();
        private LoggerAdapter _loggerAdapter = new LoggerAdapter("SessionStateTimer");
        private Stopwatch stopwatch = new Stopwatch();
        private float totalElapsedSecds;

        public float TotalElapsedSecds
        {
            get
            {
                if (isFinished)
                    return totalElapsedSecds;
                return (float) stopwatch.ElapsedTicks / 10000000;
            }
        }

        private const string sucessSession = "LoginSuccState";
        public void Enter(string sessionName,int StateId)
        {
            if (isFinished)
                return;
            if (sucessSession == sessionName)
            {
                Finish();
                return;
            }
                
            string s =string.Format("{0}-{1}", StateId,sessionName); 
            if (sessionProcessInfos.ContainsKey(s))
            {
                throw new Exception("session process multi times");
            }

            if (sessionProcessInfos.Count == 0)
            {
                stopwatch.Start();
            }

            SessionProcessInfo sessionProcessInfo = new SessionProcessInfo(s, ++accumulator);
            sessionProcessInfos[s] = sessionProcessInfo;
        }

        public void Leave(string sessionName,int StateId)
        {
            if (isFinished)
                return;
            string s =string.Format("{0}-{1}", StateId, sessionName);
            var processInfo = sessionProcessInfos[s];
            processInfo.Finish();
            sessionProcessInfos[s] = processInfo;
        }

        public void Finish()
        {
            isFinished = true;
            stopwatch.Stop();
            totalElapsedSecds = (float) stopwatch.ElapsedTicks / 10000000;
            _loggerAdapter.Info(this);
        }
        private StringBuilder stringBuilder = new StringBuilder();
        private string cachedProcessStr;
        public override string ToString()
        {
            if (!isFinished)
                return "";
             if(!string.IsNullOrEmpty(cachedProcessStr))
                return cachedProcessStr;
            stringBuilder.Length = 0;
            if (isFinished)
            {
                var list = sessionProcessInfos.Values.ToList();
                list.Sort((a, b) => { return a.orderIndex - b.orderIndex; });
                stringBuilder.Append("***SessionTimer***\n");
                foreach (SessionProcessInfo info in list)
                {
                    stringBuilder.Append(string.Format("session:{0} isFin:{1},elapse:{2}\n",info.sessionName,info.hasFinished,info.ElapseSecds));
                }
            }

            cachedProcessStr = stringBuilder.ToString();
            return cachedProcessStr;
        }
    }
}