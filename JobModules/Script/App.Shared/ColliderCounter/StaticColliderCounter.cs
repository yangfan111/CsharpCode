using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using UnityEngine.Profiling;
using Utils.Singleton;

namespace App.Shared
{
    public class StaticColliderCounter: DisposableSingleton<StaticColliderCounter>
    {

        private int curTime;
        private static int nGameObjBuffer = 10;
        private static int nColliderBuffer = 100;
        private int warnNum = 10;
        
        private List<GameObject>[] records = new List<GameObject>[nGameObjBuffer];
        private int[] lastRecordTime = new int[nGameObjBuffer];
        private Queue<int> staticColliderRecord = new Queue<int>();

        private int timeToWait = -1;
        private readonly int waitTime = 5;
        
        private int lastStaticColliderNum;
        private int lastLoadNum;

        private LoggerAdapter _logger = new LoggerAdapter(typeof(StaticColliderCounter));

        public StaticColliderCounter()
        {
            for (int i = 0; i < nGameObjBuffer; i++)
            {
                records[i] = new List<GameObject>();
            }
        }
        
        public void RecordObj(GameObject obj)
        {
            if (curTime == 0) return;
            int site = curTime % nGameObjBuffer;
            if (lastRecordTime[site] != curTime)
            {
                lastRecordTime[site] = curTime;
                records[site].Clear();
            }
            records[site].Add(obj);
        }
        
        public void CalcuColliderNum(int time)
        {
            var curStaticColliderNum = Profiler.GetPhysicsStats().numStaticBodies;
            var loadNum = curStaticColliderNum - lastStaticColliderNum;

            if (curTime == 0)
            {
                curTime = time;
                return;
            }
            curTime = time;
            
            if (staticColliderRecord.Count >= nColliderBuffer)
                staticColliderRecord.Dequeue();
            staticColliderRecord.Enqueue(loadNum);

            lastStaticColliderNum = curStaticColliderNum;
            
            if (loadNum > warnNum)
            {
                if (timeToWait > 0)
                {
                    PrintGameObjMessage();
                }
                timeToWait = waitTime;
                lastLoadNum = loadNum;
            }
            if(timeToWait==0)
                PrintGameObjMessage();
            if (timeToWait >= 0)
                timeToWait--;
            
        }

        public string GetMessage()
        {
            return string.Format(" SCReport: ave:{0}, max:{1}, curTime:{2}", staticColliderRecord.Average(),
                staticColliderRecord.Max(), curTime);
        }
        
        private void PrintGameObjMessage()
        {
            _logger.InfoFormat(" message at seq {0}, loadNum:{1}", curTime - waitTime, lastLoadNum);
            for (int i = 0; i < nGameObjBuffer; i++)
            {            
                StringBuilder sb = new StringBuilder();
                foreach (var obj in records[i])
                {
                    sb.Append(string.Format("{0}_({1})\t", obj.name, obj.GetComponentsInChildren<Collider>().Length));
                }
                _logger.InfoFormat(" {0} : {1}", lastRecordTime[i], sb.ToString());
            }
        }
        
        protected override void OnDispose()
        {
        }
    }
}