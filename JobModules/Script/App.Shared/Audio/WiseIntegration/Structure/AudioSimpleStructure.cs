using System.Collections.Generic;

namespace App.Shared.Audio
{
    public struct AudioSimpleProtoArgs
    {
        private int eventId;
        public int EventId { get { return eventId; } }
        public bool EventVailed { get { return eventId == int.MaxValue; } }
        private int groupId;
        public int GroupId { get { return groupId; } }

        public bool GroupIdVailed { get { return groupId == int.MaxValue; } }
        private string stateName;
        public string StateName { get { return stateName; } }
        public bool StateVailed { get { return !string.IsNullOrEmpty(stateName); } }

        

        public AudioSimpleProtoArgs(int evt,int grp,string sts )
        {
            eventId = evt;  
            groupId = grp;
            stateName = sts; 

        }

    }
    public enum AudioSimple_SourcePosType
    {
        FollowEntity = 1,//entityid
        StaticPositiion = 2,//px,py,pz
        InDefaultListener = 3,
    }


    public enum AudioSimple_ExecuteType
    {
        PlayOnce = 1,
        PlayLoop = 2,
        StopImmediately = 3,
        StopGradually = 4,
    }

   


}
