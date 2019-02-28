namespace Assets.Sources.Free.UI
{
    public interface IComponentGroup
    {
        int ComponentCount { get; }

        IFreeComponent GetComponent(int index);
    }
}
