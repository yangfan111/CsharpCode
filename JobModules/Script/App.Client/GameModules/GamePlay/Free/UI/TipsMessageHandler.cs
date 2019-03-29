using Core.Free;
using Free.framework;
using System.Collections.Generic;

namespace Assets.Sources.Free.UI
{
    public class TipsMessageHandler : ISimpleMesssageHandler
    {

        private const string TYPE_DESC = "desc";
        private const string TYPE_TIP = "tip";


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_UI_TIPS;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var ft = simpleUI.Ss[0];

            var map = new Dictionary<string, IList<string>>();
            for (var i = 1; i < simpleUI.Ss.Count - 1; i = i + 3)
            {
                var lang = simpleUI.Ss[i];
                var type = simpleUI.Ss[i + 1];
                var tip = simpleUI.Ss[i + 2];

                if (type == TYPE_DESC)
                {
                    ModeTips.SetDesc(lang, ft, tip);
                }
                else
                {
                    if (!map.ContainsKey(lang))
                    {
                        map.Add(lang, new List<string>());
                    }
                    map[lang].Add(tip);
                }
            }

            foreach (var lang in map.Keys)
            {
                ModeTips.AddTip(lang, ft, map[lang]);
            }
        }

    }
}
