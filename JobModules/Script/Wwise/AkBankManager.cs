using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Maintains the list of loaded SoundBanks loaded. This is currently used only with AkAmbient objects.
public static partial class AkBankManager
{
	private static readonly Dictionary<string, BankHandle> m_BankHandles =
		new Dictionary<string, BankHandle>();


	private static readonly HashSet<AsyncBankHandle> m_AyncloadingBanks = new HashSet<AsyncBankHandle>();
	
	
	
	private static void GlobalBankCallback(uint in_bankID, System.IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult,
	                                       uint in_memPoolId, object in_Cookie)
	{
		m_Mutex.WaitOne();
		var handle   = (AsyncBankHandle) in_Cookie;
		m_AyncloadingBanks.Remove(handle);
		if (in_eLoadResult != AKRESULT.AK_Success)
		{
			handle.LogLoadResult(in_eLoadResult);
			//	m_BankHandles.Remove(handle.bankName);
		}
		else
		{
			m_BankHandles.Add(handle.bankName,handle);
		}
		m_Mutex.ReleaseMutex();

		if (handle.bankCallback != null)
			handle.bankCallback(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, handle.Cookie);
	}
	// private static readonly List<BankHandle> BanksToUnload =
	// 	new List<BankHandle>();

	private static readonly System.Threading.Mutex m_Mutex = new System.Threading.Mutex();

	internal static void Reset()
	{
		m_BankHandles.Clear();
		m_AyncloadingBanks.Clear();
	}

    /// Loads a SoundBank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
    [System.Obsolete]
    public static void LoadBankEditor(string name, bool decodeBank, bool saveDecodedBank)
	{
#if UNITY_SWITCH
// No bank decoding on Nintendo switch
		decodeBank = false;
#endif

		m_Mutex.WaitOne();
		BankHandle handle = null;
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			handle = decodeBank ? new DecodableBankHandle(name, saveDecodedBank) : new BankHandle(name);
			m_BankHandles.Add(name, handle);
			m_Mutex.ReleaseMutex();
			handle.LoadBank();
		}
		else
		{
			m_Mutex.ReleaseMutex();
		}
	}

    public static AKRESULT LoadBank(string name)
    {
	    return LoadBank(name, false, false, null);
    }

    public static AKRESULT LoadBank(string name, AkCallbackManager.BankCallback callback)
    {
	    return LoadBank(name, false, false, callback);
    }
	/// Loads a SoundBank. This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information.
	public static AKRESULT LoadBank(string name,bool decodeBank, bool saveDecodedBank, AkCallbackManager.BankCallback callback = null )
	{
		m_Mutex.WaitOne();
		if (m_AyncloadingBanks.Count > 0)
		{
			foreach (var lAyncloadingBank in m_AyncloadingBanks)
			{
				if (lAyncloadingBank.bankName == name)
				{
					return AKRESULT.AK_BankAsyncLoading;
				}
			}
		}
		BankHandle handle = null;
		
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			if(callback != null)
				handle = new AsyncBankHandle(name, callback,null);
			else if(decodeBank)
				handle = new DecodableBankHandle(name,saveDecodedBank);
			else
				handle = new BankHandle(name);
			if(callback == null)
				m_BankHandles.Add(name, handle);
			m_Mutex.ReleaseMutex();
			return handle.LoadBank();
		}
		else
		{
			// Bank already loaded, increment its ref count.
			m_Mutex.ReleaseMutex();
			return AKRESULT.AK_BankAlreadyLoaded;
		}
	}
	public static AKRESULT UnloadBank(string name)
	{
		m_Mutex.WaitOne();
		BankHandle handle = null;

		if (m_AyncloadingBanks.Count > 0)
		{
			foreach (var lAyncloadingBank in m_AyncloadingBanks)
			{
				if (lAyncloadingBank.bankName == name)
				{
					handle = lAyncloadingBank;
					handle.UnloadBank();
					break;
				}
			}

			if (handle != null)
			{
				m_AyncloadingBanks.Remove(handle as AsyncBankHandle);
				return AKRESULT.AK_Success;
			}
		}
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
    /// Unloads a SoundBank. See AK::SoundEngine::UnloadBank for more information.
    [System.Obsolete]
    public static void UnloadBankEditor(string name)
	{
		m_Mutex.WaitOne();
		BankHandle handle = null;
		if (m_BankHandles.TryGetValue(name, out handle))
		{
				m_BankHandles.Remove(name);
		}

		m_Mutex.ReleaseMutex();
	}

	private class BankHandle
	{
		internal readonly string bankName;
		protected uint m_BankID = AkSoundEngine.AK_INVALID_BANK_ID;
		protected int m_PoolId = AkSoundEngine.AK_DEFAULT_POOL_ID;
		public BankHandle(string name)
		{
			bankName = name;
		}
		public BankHandle(string name,int mPoolId)
		{
			bankName = name;
			m_PoolId = mPoolId;
		}
		// public int RefCount { get; private set; }

		/// Loads a bank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
		protected virtual AKRESULT DoLoadBank()
		{
			return AkSoundEngine.LoadBank(bankName, m_PoolId, out m_BankID);
		}

		public AKRESULT LoadBank()
		{
#if UNITY_EDITOR
			if (!AkSoundEngineController.Instance.IsSoundEngineLoaded)
				return AKRESULT.AK_SoundEngineNotLoadYet; 
#endif
			var res = DoLoadBank();
			LogLoadResult(res);
			return res;

		}
        /// Unloads a bank.
        public virtual AKRESULT UnloadBank()
		{
			if(m_BankID == AkSoundEngine.AK_INVALID_BANK_ID)
				return AkSoundEngine.UnloadBank(bankName, System.IntPtr.Zero, null, null);
			return AkSoundEngine.UnloadBank(m_BankID, System.IntPtr.Zero, null, null);
		}

		internal void LogLoadResult(AKRESULT result)
		{
			if (result != AKRESULT.AK_Success && AkSoundEngine.IsInitialized())
				UnityEngine.Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (" + result + ")");
		}
	}

	private class AsyncBankHandle : BankHandle
	{
		internal readonly AkCallbackManager.BankCallback bankCallback;
        internal System.Object Cookie;

		public AsyncBankHandle(string name, AkCallbackManager.BankCallback callback,System.Object in_Cookie) : base(name)
		{
			bankCallback = callback;
            Cookie = in_Cookie;

        }
		
		/// Loads a bank.  This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information
		protected override AKRESULT DoLoadBank()
		{
			return AkSoundEngine.LoadBank(bankName, GlobalBankCallback, this, m_PoolId, out m_BankID);
		}

	
	}

	private class DecodableBankHandle : BankHandle
	{
		//解码文件修改时间<编码文件修改时间，才会执行
		private readonly bool decodeBank = true;
		private static string decodedBankPath;
		private readonly bool saveDecodedBank;
		private readonly string bankNameExt;
		public DecodableBankHandle(string name, bool save) : base(name)
		{
			if (string.IsNullOrEmpty(decodedBankPath))
				decodedBankPath = AkUtilities.GetWiseBankFolder_Full_Decode();
			saveDecodedBank = save;
			bankNameExt = Path.HasExtension(bankName) ? bankName + ".bnk" : bankName;
			// test language-specific decoded file path
			// var language = AkSoundEngine.GetCurrentLanguage();
			// var decodedBankFullPath = AkUtilities.GetWiseBankFolder_Full_Decode();

			var decodedBankFilePath = Path.Combine(decodedBankPath, bankNameExt);
			if (File.Exists(decodedBankFilePath))
			{
				try
				{
					var decodedFileTime = File.GetLastWriteTime(decodedBankFilePath);
					var defaultBankPath = AkUtilities.GetWiseBankFolder_Full();
					var encodedBankFilePath = Path.Combine(defaultBankPath, bankNameExt);
					var encodedFileTime = File.GetLastWriteTime(encodedBankFilePath);

					decodeBank = decodedFileTime <= encodedFileTime;
				}
				catch
				{
					// Assume the decoded bank exists, but is not accessible. Re-decode it anyway, so we do nothing.
				}
			}
		}

		/// Loads a bank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
		protected override AKRESULT DoLoadBank()
		{
			if (decodeBank)
				return AkSoundEngine.LoadAndDecodeBank(bankName, saveDecodedBank, out m_BankID);


			var res = AkSoundEngine.SetBasePath(decodedBankPath);

			if (res == AKRESULT.AK_Success)
			{
				res = AkSoundEngine.LoadBank(bankName,m_PoolId, out m_BankID);
			}

			return res;
		}

		/// Unloads a bank.
		public override AKRESULT UnloadBank()
		{
			if (decodeBank && !saveDecodedBank)
				return AkSoundEngine.PrepareBank(AkPreparationType.Preparation_Unload, m_BankID);
			else
			   return base.UnloadBank();
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.