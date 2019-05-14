using UnityEngine;

namespace Core.SessionState
{
    public class BackroundloadSettings
    {
        public readonly ThreadPriority backgroundLoadingPriority;
        public readonly int asyncUploadBufferSize;
        public readonly int asyncUploadTimeSlice;

        public BackroundloadSettings(ThreadPriority backgroundLoadingPriority, int asyncUploadBufferSize, int asyncUploadTimeSlice)
        {
            this.backgroundLoadingPriority = backgroundLoadingPriority;
            this.asyncUploadBufferSize = asyncUploadBufferSize;
            this.asyncUploadTimeSlice = asyncUploadTimeSlice;
        }

        public static BackroundloadSettings GetClientCurrentSettings()
        {
           
            return new BackroundloadSettings(ThreadPriority.Low, QualitySettings.asyncUploadTimeSlice,QualitySettings.asyncUploadBufferSize);
        }
        
        public static BackroundloadSettings GetServerCurrentSettings()
        {
           
            return new BackroundloadSettings(ThreadPriority.Low, QualitySettings.asyncUploadTimeSlice,QualitySettings.asyncUploadBufferSize);
        }

        public static void SetCurrentSettings(BackroundloadSettings settings)
        {
            Application.backgroundLoadingPriority = settings.backgroundLoadingPriority;
            QualitySettings.asyncUploadTimeSlice = settings.asyncUploadTimeSlice;
            QualitySettings.asyncUploadBufferSize = settings.asyncUploadBufferSize;
        }
        public static BackroundloadSettings LoadSettsings = new BackroundloadSettings(ThreadPriority.High,16,8);
    }
}