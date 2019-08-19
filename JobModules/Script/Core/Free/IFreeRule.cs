namespace Core.Free
{
    public interface IFreeRule
    {
        long ServerTime { get; }
        string FreeType { get; }
        bool GameOver { get; set; }
        bool GameExit { get; set; }
        long GameStartTime { get; set; }
        long GameEndTime { get; set; }
        long StartTime { get; }
    }
}
