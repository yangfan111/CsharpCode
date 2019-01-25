using System.Collections.Generic;
using Core.ObjectPool;
using Core.Prediction;
using Core.Replicaton;
using Core.Utils;
using Entitas;
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
namespace App.Shared.Components.Player
{

    // 越前面越新
    public class CmdListComponent<TCmd> :IResetableComponent where TCmd : class, ICmd, IRefCounter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<TCmd>.LoggerName);

        public const int SendCount = 5;
     
        public CmdListComponent()
        {
            
        }

        private MyLinkedList<TCmd> _userCmdList = new MyLinkedList<TCmd>();
        private List<TCmd> _userCmdStepList = new List<TCmd>();

        public List<TCmd> UserCmdStepList
        {
            get { return _userCmdStepList; }
        }

        private List<TCmd> _tempUserCmdList = new List<TCmd>();

        public List<TCmd> GetLargerThan(int lastSeq)
        {
            _tempUserCmdList.Clear();
            var head = _userCmdList.First;
            while (head != null)
            {
                var cmd = head.Value;        
              
                if(cmd.Seq <= lastSeq)
                {
                    break;
                }
                head = head.Next;
            }

            //var head = _userCmdList.Last;
            while (head != null)
            {
                var cmd = head.Value;
                head = head.Previous;

                if (cmd.Seq > lastSeq)
                {
                    _tempUserCmdList.Add(cmd);
                }
            }
            return _tempUserCmdList;
        }
        /// <summary>
        /// startSeq <= seq < endSeq
        /// </summary>
        /// <param name="startSeq"></param>
        /// <param name="endSeq"></param>
        /// <returns></returns>
        public List<TCmd> GetBetween(int startSeq, int endSeq)
        {
            _tempUserCmdList.Clear();
            var head = _userCmdList.Last;
            while (head != null)
            {
                var cmd = head.Value;
                head = head.Previous;

                if (cmd.Seq >= startSeq && cmd.Seq < endSeq)
                {
                    _tempUserCmdList.Add(cmd);
                }
            }
            return _tempUserCmdList;
        }


        public void AddLatest(TCmd cmd)
        {
            if (_logger.IsDebugEnabled)
            {
                //_logger.DebugFormat("Cmd List Added {0}", cmd);
            }
            _userCmdList.AddFirst(cmd);
            cmd.AcquireReference();
            Trunc();
        }

        private TCmd _lastTemp;

        public TCmd LastTemp
        {
            get { return _lastTemp; }
        }

        public void AddStepTemp(TCmd cmd)
        {
            _userCmdStepList.Add(cmd);
           // cmd.AcquireReference();
            if (_lastTemp != null)
            {
                _lastTemp.ReleaseReference();
            }
            _lastTemp = cmd;
            _lastTemp.AcquireReference();
        }
        

        private void Trunc()
        {
            if (_userCmdList.Count > 100)
            {
                TCmd cmd = _userCmdList.Last.Value;
                _userCmdList.RemoveLast();
                 cmd.ReleaseReference();
            }
                
        }
        public void AddLatestList(IList<TCmd> cmds)
        {
            TCmd latest = null;
            if (_userCmdList.Count > 0)
            {
                latest = _userCmdList.First.Value;
            }
            for(int i = cmds.Count - 1; i >= 0; i--)
            {
                var cmd = cmds[i];
                if (latest != null && cmd.Seq <= latest.Seq)
                {
                    cmd.ReleaseReference();
                    //RefCounterRecycler.Instance.ReleaseReference(cmd);
                }
                else
                {
                    AddLatest(cmd);
                    cmd.ReleaseReference();
                    Trunc();
                }
            }
        }


        public TCmd Latest
        {
            get { return _userCmdList.Count > 0 ? _userCmdList.First.Value : null; }
        }

        // a list of latest 'SendCount' item, from newest to oldest
        public ReusableList<TCmd> CreateLatestList()
        {
            var rc = ReusableList<TCmd>.Allocate();

            int count = 0;
            foreach (var cmd in _userCmdList)
            {
                rc.Value.Add(cmd);
                count++;
                if (count >= SendCount)
                    break;
            }
            return rc;
        }


        public void Reset()
        {
            _tempUserCmdList.Clear();
            _userCmdList.Clear();
        }
    }
}
#pragma warning restore RefCounter001, RefCounter002 // possible reference counter error