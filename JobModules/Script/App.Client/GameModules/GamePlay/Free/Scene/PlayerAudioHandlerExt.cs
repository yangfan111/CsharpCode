using Assets.Sources.Free;
using Free.framework;
using UnityEngine;
using App.Shared.Audio;
using App.Shared; 
using Core.EntityComponent;
using System.Collections.Generic;

namespace App.Client.GameModules.GamePlay.Free.Scene
{

  
  
    public partial class PlayerAudioHandler : ISimpleMesssageHandler
    {

        public abstract class AudioSimpleProtoProcess
        {
            public AudioSimple_ExecuteType ExeType { get; private set; }
            private AudioSimpleProtoArgs args;
            public AudioSimpleProtoArgs Args { get { return args; } }
            public void DispatchToAudioMeditor()
            {

            }
            public AudioSimpleProtoProcess(SimpleProtoWraper bin)
            {
                ExeType = (AudioSimple_ExecuteType)bin.In_Val(2);
                args = new AudioSimpleProtoArgs(bin.In_Val(3), bin.In_Val(4), bin.St_Val(0));
            }
            public abstract void Process();
     
            public abstract bool IsVailed();
          
            public static AudioSimpleProtoProcess CreateSimpleHandler(SimpleProtoWraper bin)
            {
                var posType = (AudioSimple_SourcePosType)bin.In_Val(1);
                var exeType = (AudioSimple_ExecuteType)bin.In_Val(2);
                switch (posType)
                {
                    case AudioSimple_SourcePosType.FollowEntity:
                        return new FollowEntityProcess(bin);
                    case AudioSimple_SourcePosType.InDefaultListener:
                        return new DefaultProcess(bin);
                    case AudioSimple_SourcePosType.StaticPositiion:
                        return new StaticPositiionProcess(bin);
                    default:
                        throw new System.Exception("Unhandled audio simpleProto type)");
                        

                }
                return null;
            }
        }
        public class DefaultProcess : AudioSimpleProtoProcess
        {
            public override void Process()
            {

            }

            public override bool IsVailed()
            {
                throw new System.NotImplementedException();
            }

            public DefaultProcess(SimpleProtoWraper bin) : base(bin)
            {
                //AKAudioEntry.Dispatcher.PostEvent
                //AKAudioEntry.PostEvent(Args.EventId);
            }
        }
        public class FollowEntityProcess : AudioSimpleProtoProcess
        {
            public int EntityId { get; private set; }
            public override void Process()
            {
             //   EntityKey ek = new EntityKey(EntityId, (short)EEntityType.Player);
                //GameObject entityGo = Assets.Sources.Free.UI.SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(ek); 
                
            }

            public override bool IsVailed()
            {
                throw new System.NotImplementedException();
            }

            public FollowEntityProcess(SimpleProtoWraper bin) : base(bin)
            {
                EntityId = bin.In_Val(2);

            }
        }
        public class StaticPositiionProcess : AudioSimpleProtoProcess
        {
            public Vector3 Pos { get; private set; }

            public readonly static Dictionary<Vector3, GameObject> sceneStaticVoicePoints = new Dictionary<Vector3, GameObject>();
            
            public StaticPositiionProcess(SimpleProtoWraper bin) : base(bin) 
            {
                Pos = new Vector3(bin.Fl_Val(0), bin.Fl_Val(1), bin.Fl_Val(2));
                if(!sceneStaticVoicePoints.ContainsKey(Pos)||sceneStaticVoicePoints[Pos] == null)
                {
                    //TODO:对象池接收
                    sceneStaticVoicePoints[Pos] = new GameObject("ServerVoicePoint");
                    sceneStaticVoicePoints[Pos].transform.position = Pos;
                }
                else
                {
                    if (!sceneStaticVoicePoints[Pos].activeSelf) sceneStaticVoicePoints[Pos].SetActive(true);
                }
             
                //if(sceneStaticVoices.Contains(Pos))
            }
            public override void Process()
            {
            //  AKAudioEntry.PostEvent(Args.EventId, sceneStaticVoicePoints[Pos]);

            }
            public void Recycle(bool force = false)
            {
                foreach(KeyValuePair<Vector3,GameObject> pair in sceneStaticVoicePoints)
                {
                    if (pair.Value)
                    {
                        if (force)
                        {
                            UnityEngine.GameObject.Destroy(pair.Value);
                            sceneStaticVoicePoints.Remove(pair.Key);
                        }
                          
                        else
                            pair.Value.SetActive(false);
                    }
                    else
                    {
                        sceneStaticVoicePoints.Remove(pair.Key);
                    }
                }
            }
            //public override void Update()
            //{

            //}

            public override bool IsVailed()
            {
                return true;
            }
        }

    }


}
