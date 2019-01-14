using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public interface IFeaturable
	{
		bool HasFeature(string feature);

		object GetFeatureValue(string feature);
	}
}
