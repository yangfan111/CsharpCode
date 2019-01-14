using Sharpen;

namespace com.wizard.fastpool
{
	public interface IPrimaryTypeFactory<T>
	{
		T MakeObject();
	}
}
