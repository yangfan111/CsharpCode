using Core.EntityComponent;

namespace Core.UpdateLatest
{
    public interface IUpdateLatestHandler
    {
        UpdateLatestPacakge GetUpdateLatestPacakge(EntityKey selfKey);
        
        int BaseUserCmdSeq { get; set; }
        int UserCmd { get; set; }
        int LastSnapshotId { get; set; }
    }
}