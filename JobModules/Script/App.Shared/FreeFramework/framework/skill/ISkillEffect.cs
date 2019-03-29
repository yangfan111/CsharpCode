using Sharpen;

namespace com.wd.free.skill
{
	public interface ISkillEffect
	{
		bool IsTimeOut(long time);

		long GetSource();

		string GetKey();

		int GetTime();

		void SetSource(long source);

		void SetKey(string key);

		void SetTime(int time);

		int GetLevel();

		void Effect(ISkillArgs args);

		void Resume(ISkillArgs args);
	}
}
