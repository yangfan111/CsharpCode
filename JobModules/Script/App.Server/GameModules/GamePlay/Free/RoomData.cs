using Sharpen;

namespace App.Server.GameModules.GamePlay.Free
{
	[System.Serializable]
	public class RoomData
	{
		private const long serialVersionUID = 3329628733791075350L;

		public long roomId = 0;

		public int sceneId = 0;

		public int winCondition = 0;

		public int winScore = 0;

		public int raceType = 0;

		public int subRaceType = 0;

		public int sectionTime = 0;

		public int reLiveTime = 3;

		public int createTime = 0;

		public bool changeTeam = false;

		public int playerNum = 0;

		public bool isPromise = false;

		public bool moreServerFlag = false;

		public string freeType;
		//地图ID
		//比赛规则，回合数、时间、杀人数等
		//比赛规则数值
		//比赛类型，团战、爆破战、特殊战等
		//子模式，刀战、手枪战、雷战等;
		//复活时间(秒数)
		//爆破模式 半场换边
		//人数，当前模型下满人个数
		//跨服
	}
}
