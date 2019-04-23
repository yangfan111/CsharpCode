namespace Utils.Compare
{
    public interface IComparableComponent
    {
        bool IsApproximatelyEqual(object right);
    }
}