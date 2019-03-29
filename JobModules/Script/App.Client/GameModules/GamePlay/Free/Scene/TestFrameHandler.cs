using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using App.Shared;
using UnityEngine;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.Scripts;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Core.Utils;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Scene
{
    public class TestFrameHandler : ISimpleMesssageHandler
    {
        static LoggerAdapter logger = new LoggerAdapter("FrameTest");

        public bool CanHandle(int key)
        {
            return FreeMessageConstant.TestFrame == key;
        }

        public void Handle(SimpleProto data)
        {
            ClientFPSUpdater fps = SingletonManager.Get<ClientFPSUpdater>();

            PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            
            #if UNITY_EDITOR

            logger.InfoFormat("{0},{1},{2},{3},{4} at ({5},{6}) with Yaw {7}", fps.Fps, UnityStats.triangles, UnityStats.vertices, UnityStats.drawCalls, UnityStats.batches,
                player.position.Value.x, player.position.Value.z, player.orientation.Yaw);
            
            #else

            logger.InfoFormat("{0} at ({1},{2}) with Yaw {3}", fps.Fps, player.position.Value.x, player.position.Value.z, player.orientation.Yaw);
    
            #endif


            //if (SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel != null)
            //{
            //    SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, frame);
            //}
        }
    }
}
