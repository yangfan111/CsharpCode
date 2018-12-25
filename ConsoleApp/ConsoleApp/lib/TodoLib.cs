//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ConsoleApp
//{
//    public class EventCallbackPackagage
//    {
//        ///For more information about the event callback, see the classes derived from AkCallbackInfo.
//        public CallbackInfo info;

//        /// GameObject from whom the callback function was called
//        public UnityEngine.GameObject sender;

//        ///AkSoundEngine.PostEvent callback flags. See the AkCallbackType enumeration for a list of all callbacks
//        public CustomCallbackType type;
//    }

//    public class EventCallbackData : UnityEngine.ScriptableObject
//    {
//        ////AkSoundEngine.PostEvent callback flags. See the AkCallbackType enumeration for a list of all callbacks
//        public System.Collections.Generic.List<int> callbackFlags = new System.Collections.Generic.List<int>();

//        ////Names of the callback functions.
//        public System.Collections.Generic.List<string> callbackFunc = new System.Collections.Generic.List<string>();

//        ////GameObject that will receive the callback
//        public System.Collections.Generic.List<UnityEngine.GameObject> callbackGameObj =
//            new System.Collections.Generic.List<UnityEngine.GameObject>();

//        ////The sum of the flags of all game objects. This is the flag that will be passed to AkSoundEngine.PostEvent
//        public int uFlags = 0;
//    }

//    public class CallbackInfo : global::System.IDisposable
//    {
//        private global::System.IntPtr swigCPtr1;
//        protected bool swigCMemOwn1;

//        internal CallbackInfo(global::System.IntPtr cPtr, bool cMemoryOwn)
//        {
//            swigCMemOwn1 = cMemoryOwn;
//            swigCPtr1 = cPtr;
//        }

//        internal static global::System.IntPtr getCPtr(CallbackInfo obj)
//        {
//            return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr1;
//        }

//        internal virtual void setCPtr(global::System.IntPtr cPtr)
//        {
//            Dispose();
//            swigCPtr1 = cPtr;
//        }

//        ~CallbackInfo()
//        {
//            Dispose1();
//        }

//        public void Dispose1()
//        {
//            lock (this)
//            {
//                if (swigCPtr1 != global::System.IntPtr.Zero)
//                {
//                    if (swigCMemOwn1)
//                    {
//                        swigCMemOwn1 = false;
//                        // AkSoundEnginePINVOKE.CSharp_delete_CallbackInfo(swigCPtr1);
//                    }
//                    swigCPtr1 = global::System.IntPtr.Zero;
//                }
//                global::System.GC.SuppressFinalize(this);
//            }
//        }

//        //public global::System.IntPtr pCookie
//        //{
//        //    get { return AkSoundEnginePINVOKE.CSharp_CallbackInfo_pCookie_get(swigCPtr1); }
//        //}

//        //public ulong gameObjID
//        //{
//        //    get { return AkSoundEnginePINVOKE.CSharp_CallbackInfo_gameObjID_get(swigCPtr1); }
//        //}

//        //public CallbackInfo() : this(AkSoundEnginePINVOKE.CSharp_new_CallbackInfo(), true)
//        //{
//        //}

//    }
//    public class LoadFile { }
//}
//    //private System.Collections.IEnumerator LoadFile()
//    //{
//    //    ms_www = new UnityEngine.WWW(m_bankPath);

//    //    yield return ms_www;

//    //    uint in_uInMemoryBankSize = 0;

//    //    // Allocate an aligned buffer
//    //    try
//    //    {
//    //        ms_pinnedArray =
//    //            System.Runtime.InteropServices.GCHandle.Alloc(ms_www.bytes, System.Runtime.InteropServices.GCHandleType.Pinned);
//    //        ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
//    //        in_uInMemoryBankSize = (uint)ms_www.bytes.Length;

//    //        // Array inside the WWW object is not aligned. Allocate a new array for which we can guarantee the alignment.
//    //        if ((ms_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0)
//    //        {
//    //            var alignedBytes = new byte[ms_www.bytes.Length + AK_BANK_PLATFORM_DATA_ALIGNMENT];
//    //            var new_pinnedArray =
//    //                System.Runtime.InteropServices.GCHandle.Alloc(alignedBytes, System.Runtime.InteropServices.GCHandleType.Pinned);
//    //            var new_pInMemoryBankPtr = new_pinnedArray.AddrOfPinnedObject();
//    //            var alignedOffset = 0;

//    //            // New array is not aligned, so we will need to use an offset inside it to align our data.
//    //            if ((new_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0)
//    //            {
//    //                var alignedPtr = (new_pInMemoryBankPtr.ToInt64() + AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) &
//    //                                 ~AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK;
//    //                alignedOffset = (int)(alignedPtr - new_pInMemoryBankPtr.ToInt64());
//    //                new_pInMemoryBankPtr = new System.IntPtr(alignedPtr);
//    //            }

//    //            // Copy the bank's bytes in our new array, at the correct aligned offset.
//    //            System.Array.Copy(ms_www.bytes, 0, alignedBytes, alignedOffset, ms_www.bytes.Length);

//    //            ms_pInMemoryBankPtr = new_pInMemoryBankPtr;
//    //            ms_pinnedArray.Free();
//    //            ms_pinnedArray = new_pinnedArray;
//    //        }
//    //    }
//    //    catch
//    //    {
//    //        yield break;
//    //    }

//    //    var result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, in_uInMemoryBankSize, out ms_bankID);
//    //    if (result != AKRESULT.AK_Success)
//    //        UnityEngine.Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result);
//    //}
