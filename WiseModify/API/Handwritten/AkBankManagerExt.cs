#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Maintains the list of loaded SoundBanks loaded. This is currently used only with AkAmbient objects.
public static partial class AkBankManagerExt
{
   
    public static AKRESULT LoadBankRes(string name, bool decodeBank, bool saveDecodedBank)
    {

        m_Mutex.WaitOne();
        BankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            handle = decodeBank ? new DecodableBankHandle(name, saveDecodedBank) : new BankHandle(name);
            m_BankHandles.Add(name, handle);
            m_Mutex.ReleaseMutex();
            return handle.DoLoadBank();
        }
        else
        {
            m_Mutex.ReleaseMutex();
            return AKRESULT.AK_BankAlreadyLoaded;
        }
    }
    public static AKRESULT LoadBankResAsync(string name, AkCallbackManager.BankCallback callback, System.Object in_cookie)
    {
        m_Mutex.WaitOne();
        BankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            var asyncHandle = new AsyncBankHandle(name, callback, in_cookie);
            m_BankHandles.Add(name, asyncHandle);
            m_Mutex.ReleaseMutex();
            asyncHandle.DoLoadBank();
            return AKRESULT.AK_WaitBankLoadingFinish;
        }
        else
        {

            m_Mutex.ReleaseMutex();
            return AKRESULT.AK_BankAlreadyLoaded;
        }
    }
    public static AKRESULT UnloadBankRes(string name)
    {
        m_Mutex.WaitOne();
        BankHandle handle = null;
        if (m_BankHandles.TryGetValue(name, out handle))
        {
            m_BankHandles.Remove(name);
            m_Mutex.ReleaseMutex();
            return handle.UnloadBank();
            //handle.DecRef();
            //if (handle.RefCount == 0)
            //    m_BankHandles.Remove(name);
        }
        else
        {

            m_Mutex.ReleaseMutex();
            return AKRESULT.AK_BankNotLoadYet;
        }
    }
    
}
