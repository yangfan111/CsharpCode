namespace Core.Compare
{
    public interface IComparableComponent
    {
        bool IsApproximatelyEqual(object right);
    }
}