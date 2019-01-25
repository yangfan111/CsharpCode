


namespace App.Shared.Audio
{
    //default data 
    [System.Serializable]
    public class AudioCoreSetingData
    {
        public bool isForbiden;
        public string audioLoadTypeWhenStarup ;
        public bool usePicker ;
        public string wiseInstallationPath ;
        public string wiseProjectPath ;
        public AudioCoreSetingData(bool forbidden,string loadType)
        {
            isForbiden = forbidden;
            audioLoadTypeWhenStarup = loadType;
        }
        public AudioCoreSetingData() { }
        public static readonly AudioCoreSetingData Output = new AudioCoreSetingData(false,"Sync");
       
      


    }


}