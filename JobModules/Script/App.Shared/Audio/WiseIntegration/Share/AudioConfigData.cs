


namespace App.Shared.Audio
{
    //default data 
    [System.Serializable]
    public class AudioConfigData
    {
        public bool isForbiden;
        /// <summary>
        /// Sync,Async
        /// </summary>
        public string audioLoadTypeWhenStarup ;
        public bool usePicker ;
        public string wiseInstallationPath ;
        public string wiseProjectPath ;
        public AudioConfigData(bool forbidden,string loadType)
        {
            isForbiden = forbidden;
            audioLoadTypeWhenStarup = loadType;
        }
        public AudioConfigData() { }
        public static readonly AudioConfigData Output = new AudioConfigData(false,"Sync");
       
      


    }


}