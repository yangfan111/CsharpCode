//using UnityEngine;
//using System.Collections.Generic;
//using System;
//namespace   App.Shared.Audio
//{

//    public static class AudioStatic
//    {

////        private static AKAKAudioDispatcher instance;


//        //public static System.Type LoadSysType
//        //{
//        //    get
//        //    {
//        //        return AudioConst.PluginName == "AK" ? typeof(AKAudioBankLoader) : null;
//        //    }
//        //}

//    //    public static AKAKAudioDispatcher Dispatcher
//    //    {
//    //        get
//    //        {
//    //            if (instance == null)
//    //                instance = new AKAKAudioDispatcher(new AKAudioEngineDriver());
//    //            return instance;
//    //        }
//    //    }

//    //    public static AudioRegulator Regular
//    //    {
//    //        get
//    //        {
//    //            return Dispatcher.Regulator;
//    //        }
//    //    }
//    //    public static IAudioEngineDriver Driver
//    //    {
//    //        get
//    //        {
//    //            return Dispatcher.Driver;
//    //        }
//    //    }
      
       
//    //    public static GameObject AudioObject { get; private set; }
//    //    public static void Launch(UnityEngine.GameObject initilizer)
//    //    {
//    //        Debug.Log("Sound Engine Begin ==============> ");
//    //        AudioObject = initilizer;
//    //        //测试代码
//    //        var list = getTestCfg();
//    //        IAudioBankLoader loader = new AKAudioBankLoader(list);
//    //        Dispatcher.BindAssetLoader(loader);
        
          
//    //    }
        
//    //    static List<AkBankRes> getTestCfg()
//    //    {
//    //        var list = new List<AkBankRes>();
//    //        //AkBankRes c1 = new AkBankRes();
//    //        //c1.LoadType = AudioBankLoadType.DecodeOnLoadAndSave;
//    //        //c1.BankName = "Test";
//    //        ////c1.LoadAsynchronous = true;
//    //        //// c1.BankId = BANKS.INIT;
//    //        //AkBankRes c2 = new AkBankRes();

//    //        //c1.BankName = "Test";
//    //        //c1.LoadTriggerList = new List<uint> { (uint)AudioTriggerEventType.SceneLoad };
//    //        ////   c1.BankId = BANKS.INIT;
//    //        //list.Add(c1);
//    //        return list;
//    //    }


//    //}
//}