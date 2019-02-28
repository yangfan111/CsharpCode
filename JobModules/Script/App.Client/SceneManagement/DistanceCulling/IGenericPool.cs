namespace App.Client.SceneManagement.DistanceCulling
{
    interface IGenericPool<T>
    {
        T Get();
    }
}