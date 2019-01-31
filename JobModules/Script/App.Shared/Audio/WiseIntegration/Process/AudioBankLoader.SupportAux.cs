using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace   App.Shared.Audio
{
    public partial class AudioBankLoader
    {
        abstract class BankLoadModelHandler
        {
            public abstract void Init();
            public BankLoadModelHandler(bool saveDecode,bool decodeLoad,bool loadAsync)
            {

            }
        }
        class ExplicitLoadHandler : BankLoadModelHandler
        {
            private uint m_BankID;

            public ExplicitLoadHandler(bool saveDecode, bool decodeLoad, bool loadAsync) : base(saveDecode, decodeLoad, loadAsync)
            {
            }

            public override void Init()
            {
                string[] allBanks = null;
                foreach(string bank in allBanks)
                {
                    AkSoundEngine.LoadBank(bank, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
                    AkBankManager.LoadBank(bank, false, false);
                }
            }
        }
        
        public delegate void LoadResultStackDelegate(AKRESULT result, string bankName, System.Object customData);
        public delegate void LoadResultBroadcastDelegate(string bankName);
        public delegate void LoadOrUnloadInternalDelgate(AKBankAtom atom);
        public struct BankLoadInfo
        {
            public AKRESULT result;
            public string bname;
            public BankLoadInfo(AKRESULT result,string name)
            {
                this.result = result;
                this.bname = name;
            }
        }
        private class LoadResultCallback_Data
        {
            private System.Object customData;
            private LoadResultStackDelegate callbackObj;

            public string Name { get; private set; }
            public LoadResultCallback_Data(string bankName, LoadResultStackDelegate cb, System.Object so)
            {
                customData = so;
                callbackObj = cb;
                Name = bankName;
            }
            public void Call(AKRESULT result)
            {
                callbackObj(result, Name, customData);
            }
        }
 
    }
}
