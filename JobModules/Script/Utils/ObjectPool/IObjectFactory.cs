namespace Core.ObjectPool
{
	public interface IObjectFactory
	{

		object MakeObject();

		void ActivateObject(object arg0);

		void DestroyObject(object arg0);
	}
}
