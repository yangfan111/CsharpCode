using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Core.Audio
{
    public partial class AKAudioBankLoader
    {
        
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
