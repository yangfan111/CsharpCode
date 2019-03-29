using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.condition;

namespace com.cpkf.yyjd.tools.data.sort
{
	public interface ISelectOrder : IClausable
	{
		DataBlocks Sort(DataBlock block);

		ICollection<string> GetAllFeatures();
	}
}
