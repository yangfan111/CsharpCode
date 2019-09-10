using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Core.Utils;

namespace Core.UpdateLatest
{


    public class ServerUpdateMessagePool :IDisposable
    {
        private static LoggerAdapter logger = new LoggerAdapter(typeof(ServerUpdateMessagePool));
        private List<UpdateLatestPacakge> list = new List<UpdateLatestPacakge>(512);
        private Dictionary<int, UpdateLatestPacakge> dict = new Dictionary<int, UpdateLatestPacakge>();
        public UpdateLatestPacakge LatestMessage { get; private set; }
        private const int MaxHistoryCount = 400;
        private int lastSeq = -1;

        public void AddMessage(UpdateLatestPacakge message)
        {
            if (lastSeq < message.Head.LastUserCmdSeq)
            {
                LatestMessage = message;
                lastSeq = message.Head.LastUserCmdSeq;
                list.Add(message);
                message.AcquireReference();
                dict.Add(message.Head.LastUserCmdSeq, message);
                Trun();
            }
        }
#pragma warning disable RefCounter001,RefCounter002 
        private void Trun()
        {
            if (list.Count > MaxHistoryCount)
            {
                var r = list[0];
                list.RemoveAt(0);
                dict.Remove(r.Head.LastUserCmdSeq);
                r.ReleaseReference();
            }
        }
#pragma warning restore RefCounter001, RefCounter002
        public UpdateLatestPacakge GetPackageBySeq(int seq)
        {
           
            UpdateLatestPacakge ret;
            dict.TryGetValue(seq, out ret);
            return ret;
        }
        private List<UpdateLatestPacakge> tempList = new List<UpdateLatestPacakge>();
     
        public List<UpdateLatestPacakge>  GetPackagesLargeThan(int seq)
        {
            tempList.Clear();
            foreach (var updateLatestPacakge in list)
            {
                if (updateLatestPacakge.Head.LastUserCmdSeq > seq)
                {
                    tempList.Add(updateLatestPacakge);
                }
            }

            return tempList;
        }

        public int LatestMessageSeq {
            get
            {
                if (LatestMessage != null) return LatestMessage.Head.LastUserCmdSeq;
                return -1;
            }
        }

        public bool ContainsKey(int baseUserCmdSeq)
        {
            return dict.ContainsKey(baseUserCmdSeq);
        }

        public void ClearOldMessage(int headBaseUserCmdSeq)
        {
            int count = 0;
            foreach (var updateLatestPacakge in list)
            {
                if (updateLatestPacakge.Head.LastUserCmdSeq < headBaseUserCmdSeq)
                {
                    updateLatestPacakge.ReleaseReference();
                    count++;
                }
                else
                {
                    break;
                }
            }
            list.RemoveRange(0, count);
            
        }

        public void Dispose()
        {
            logger.InfoFormat("Dispose {0}",list.Count);
            LatestMessage = null;
            try
            {

           
            foreach (var updateLatestPacakge in list.ToArray())
            {
                try
                {
                   // logger.InfoFormat("{0}",updateLatestPacakge.RefCount);
                    updateLatestPacakge.ReleaseReference();
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Dispose :{0}", e);
                }
              
            }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Dispose :{0}", e);
            }
            list.Clear();
            dict.Clear();
            lastSeq = -1;
        }
    }
}