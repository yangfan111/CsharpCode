using Core.EntityComponent;

namespace Core.Prediction
{
    public class SavedHistory
    {
        public SavedHistory(int historyId, EntityMap entityMap)
        {
            HistoryId = historyId;
            EntityMap = entityMap;
        }

        public int HistoryId;
        public EntityMap EntityMap;

            
    }
    public interface IPredictionInitManager
    {
        
        void PredictionInit();
        void SavePredictionCompoments(int historyId);
        SavedHistory GetTargetHistory(int cmdSeq);
    }
}