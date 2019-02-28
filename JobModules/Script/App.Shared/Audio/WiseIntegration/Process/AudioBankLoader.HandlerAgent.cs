using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  App.Shared.Audio
{
    public partial class AudioBankLoader
    {

        public class BankLoadHandlerAgent
        {
            public LoadOrUnloadInternalDelgate InternalLoadHandler { get; private set; }
            public LoadOrUnloadInternalDelgate InternalUnloadHandler { get; private set; }

            public event LoadResultBroadcastDelegate BroadcastAsync_LoadSucessEvt;
            public event LoadResultBroadcastDelegate BroadcastAsync_LoadFailEvt;
            public event LoadResultBroadcastDelegate BroadcastAsync_UpdateRefEvt;
            public event LoadResultBroadcastDelegate BroadcastAsync_UnloadSucessEvt;
            public event LoadResultBroadcastDelegate BroadcastAsync_UnloadFailEvt;

            public BankLoadHandlerAgent(LoadOrUnloadInternalDelgate loadHandler, LoadOrUnloadInternalDelgate unloadHandler)
            {
                InternalLoadHandler = loadHandler;
                InternalUnloadHandler = unloadHandler;
            }
            public void BroadcastBankLoadResult(string bankName, AKRESULT result)
            {
                switch (result)
                {
                    case AKRESULT.AK_Success:
                        if (BroadcastAsync_LoadSucessEvt != null)
                            BroadcastAsync_LoadSucessEvt(bankName);
                        break;
                    case AKRESULT.AK_BankAlreadyLoaded:
                        if (BroadcastAsync_UpdateRefEvt != null)
                            BroadcastAsync_UpdateRefEvt(bankName);
                        break;
                    default:
                        if (BroadcastAsync_UpdateRefEvt != null)
                            BroadcastAsync_UpdateRefEvt(bankName);
                        break;
                }
            }
            public void BroadcastBankUnloadResult(string bankName, AKRESULT result)
            {
                switch (result)
                {
                    case AKRESULT.AK_Success:
                        if (BroadcastAsync_UnloadSucessEvt != null)
                            BroadcastAsync_UnloadSucessEvt(bankName);
                        break;
                    default:
                        if (BroadcastAsync_UnloadFailEvt != null)
                            BroadcastAsync_UnloadFailEvt(bankName);
                        break;
                }
            }
        }
    }


}



