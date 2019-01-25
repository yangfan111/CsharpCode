using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Core.UpdateLatest
{
    public interface IUpdateMessagePool: IDisposable
    {
        UpdateLatestPacakge LatestMessage { get; }
        void AddMessage(UpdateLatestPacakge message);
        UpdateLatestPacakge GetPackageBySeq(int seq);
        List<UpdateLatestPacakge> GetPackagesLargeThan(int seq);
        int LatestMessageSeq { get; }
        bool ContainsKey(int baseUserCmdSeq);
    }

    public class UpdateMessagePool : IUpdateMessagePool
    {
        private List<UpdateLatestPacakge> _list = new List<UpdateLatestPacakge>(16);
        private Dictionary<int, UpdateLatestPacakge> dict = new Dictionary<int, UpdateLatestPacakge>();
        public UpdateLatestPacakge LatestMessage { get; private set; }
        private const int MaxHistoryCount = 200;
        private int _lastSeq = -1;

        public void AddMessage(UpdateLatestPacakge message)
        {
            if (_lastSeq < message.Head.UserCmdSeq)
            {
                LatestMessage = message;
                _lastSeq = message.Head.UserCmdSeq;
                _list.Add(message);
                message.AcquireReference();
                dict.Add(message.Head.UserCmdSeq, message);
                Trun();
            }
        }
#pragma warning disable RefCounter001,RefCounter002 
        private void Trun()
        {
            if (_list.Count > MaxHistoryCount)
            {
                var r = _list[0];
                _list.RemoveAt(0);
                dict.Remove(r.Head.UserCmdSeq);
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
        private List<UpdateLatestPacakge> _tempList = new List<UpdateLatestPacakge>();
        public List<UpdateLatestPacakge>  GetPackagesLargeThan(int seq)
        {
            _tempList.Clear();
            foreach (var updateLatestPacakge in _list)
            {
                if (updateLatestPacakge.Head.UserCmdSeq > seq)
                {
                    _tempList.Add(updateLatestPacakge);
                }
            }

            return _tempList;
        }

        public int LatestMessageSeq {
            get
            {
                if (LatestMessage != null) return LatestMessage.Head.UserCmdSeq;
                else
                {
                    return -1;
                }
            }
        }

        public bool ContainsKey(int baseUserCmdSeq)
        {
            return dict.ContainsKey(baseUserCmdSeq);
        }

        public void Dispose()
        {
            LatestMessage = null;
            foreach (var updateLatestPacakge in _list)
            {
                updateLatestPacakge.ReleaseReference();
            }
            _list.Clear();
            dict.Clear();
        }
    }
}