//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//namespace   App.Shared.Audio
//{
//    public delegate void AudioResultHandler();

//    public interface IAudioBankLoader
//    {
//         bool FlagInit { get; set; }
    
//        void Recycle(List<AkBankRes> bankConfigList);
//        void SubscribeToLoadFinish(System.Action handler);
        
//    }
  
//    public interface IAudioAtom
//    {
//        List<int> GetTriggerList();
//        void Restore();

//    }

//    public interface IAudioData
//    {
//        int ID { get; }
        
//        bool IsUsable();
//        List<int> TriggerList { get; }
//    }
//}

