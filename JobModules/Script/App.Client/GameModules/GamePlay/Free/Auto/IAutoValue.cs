namespace Assets.Sources.Free.Auto
{
    public interface IAutoValue
    {
        bool Started { get; }
        void Start();
        void Stop();
        void SetValue(params object[] value);
        object Frame(int frameTime);
        IAutoValue Parse(string config);
    }
}
