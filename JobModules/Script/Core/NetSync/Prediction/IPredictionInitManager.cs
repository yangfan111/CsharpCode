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
 
}