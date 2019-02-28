using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Server.GameModules.GamePlay.free.player;
using Free.framework;
using Core.Free;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class UiTestValue : AbstractTestValue
    {
        public string ui;

        public string[] UIValues;

        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();

            FreeData fd = (FreeData)args.GetUnit(UnitTestConstant.Tester);
            if (fd != null)
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.FetchUIValue;
                msg.Ss.Add(ui);
                FreeMessageSender.SendMessage(fd.Player, msg);

                if (UIValues != null && UIValues.Length % 2 == 0)
                {
                    for (int i = 0; i < UIValues.Length; i = i + 2)
                    {
                        tv.AddField(UIValues[i], UIValues[i + 1]);
                    }
                }
            }

            return tv;
        }
    }
}
