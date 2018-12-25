using System.Collections.Generic;	
public class ShortIDGenerator
	{
		private const uint s_prime32 = 16777619;
		private const uint s_offsetBasis32 = 2166136261;

		private static byte s_hashSize;
		private static uint s_mask;

		static ShortIDGenerator()
		{
			HashSize = 32;
		}

		public static byte HashSize
		{
			get { return s_hashSize; }

			set
			{
				s_hashSize = value;
				s_mask = (uint) ((1 << s_hashSize) - 1);
			}
		}

		public static uint Compute(string in_name)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(in_name.ToLower());

			// Start with the basis value
			var hval = s_offsetBasis32;

			for (var i = 0; i < buffer.Length; i++)
			{
				// multiply by the 32 bit FNV magic prime mod 2^32
				hval *= s_prime32;

				// xor the bottom with the current octet
				hval ^= buffer[i];
			}

			if (s_hashSize == 32)
				return hval;

			// XOR-Fold to the required number of bits
			return (hval >> s_hashSize) ^ (hval & s_mask);
		}
}
public abstract class ShortList<T1,T2> //where T:ANY
{
    // @todo: Use HashSet<ulong> and CopyTo() with a private ulong[]
    private readonly List<T1> listenerIdList = new List<T1>();

    private readonly List<T2> listenerList =
        new List<T2>();

    public List<T2> ListenerList
    {
        get { return listenerList; }
    }

    public abstract bool Add(T2 listener);

    public abstract bool Remove(T2 listener);
   
    public T1[] GetListenerIds()
    {
        return listenerIdList.ToArray();
    }
}


[System.Serializable]
public class WwiseSettings
{
    public const string WwiseSettingsFilename = "WwiseSettings.xml";

    //private static WwiseSettings s_Instance;
    public bool CopySoundBanksAsPreBuildStep = true;
    public bool CreatedPicker = false;
    public bool CreateWwiseGlobal = true;
    public bool CreateWwiseListener = true;
    public bool GenerateSoundBanksAsPreBuildStep = false;
    public bool ShowMissingRigidBodyWarning = true;
    public string SoundbankPath;
    public string WwiseInstallationPathMac;
    public string WwiseInstallationPathWindows;
    public string WwiseProjectPath;

    // Save the WwiseSettings structure to a serialized XML file
    public static void SaveSettings(WwiseSettings Settings) { }


    // Load the WwiseSettings structure from a serialized XML file
    public static WwiseSettings LoadSettings(bool ForceLoad = false)
    { return null; }
 
}