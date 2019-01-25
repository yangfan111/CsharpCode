using Sharpen;

namespace com.wd.free.skill
{
	/// <summary>定义了多个效果同时存在时如何选择效果</summary>
	/// <author>Dave</author>
	public interface IEffectSelector
	{
		ISkillEffect Select(ISkillEffect[] effects);
	}
}
