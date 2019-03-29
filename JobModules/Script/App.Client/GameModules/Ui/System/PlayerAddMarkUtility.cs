using App.Shared;
using Core.Free;
using Free.framework;
using UnityEngine;

namespace App.Client.GameModules.Ui.System
{
    public class PlayerAddMarkUtility
    {
        private static Vector2 lastAddMarkPos = Vector2.zero;
        public static float rate = 1f;

        public static void SendMarkMessage(Contexts contexts, Vector2 markPos)
        {
            SimpleProto loginSucess = FreePool.Allocate();
            if (Vector2.Distance(markPos, lastAddMarkPos) > 10 / rate)
            {
                loginSucess.Key = FreeMessageConstant.MarkPos;
                loginSucess.Fs.Add(markPos.x);
                loginSucess.Fs.Add(markPos.y);
                loginSucess.Fs.Add(1);              //add mark
                lastAddMarkPos = markPos;
            }
            else
            {
                loginSucess.Key = FreeMessageConstant.MarkPos;
                loginSucess.Fs.Add(markPos.x);
                loginSucess.Fs.Add(markPos.y);
                loginSucess.Fs.Add(0);              //remove mark
                lastAddMarkPos = Vector2.zero;
            }
            if (contexts.session.clientSessionObjects.NetworkChannel != null)
            {
                contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, loginSucess);
            }
        }
    }
    
}
