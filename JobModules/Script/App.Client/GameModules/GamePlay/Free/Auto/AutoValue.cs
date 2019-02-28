using System.Collections.Generic;
using Assets.Sources.Utils;
using Core.Utils;
using App.Client.GameModules.GamePlay.Free.Auto.Prefab;
using App.Client.GameModules.GamePlay.Free.Auto;

namespace Assets.Sources.Free.Auto
{
    public class AutoValue
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(AutoValue));
        private static IList<IAutoValue> values;

        public static IAutoValue Parse(string config)
        {
            if (values == null)
            {
                values = new List<IAutoValue>();
                values.Add(new AutoConstValue());
                values.Add(new AutoPercentValue());
                values.Add(new AutoPlayerValue());
                values.Add(new AutoTimeValue());
                values.Add(new AutoRotateValue());
                values.Add(new AutoTimeStringValue());
                values.Add(new AutoPositionValue());
                values.Add(new AutoTwoPositionValue());
                values.Add(new AutoTimeUnitValue());
                values.Add(new AutoClientValue());
                //                values.push(new AutoPointValue());
                //                values.push(new AutoVisibleValue());
                //                values.push(new AutoImgCoverValue());
                values.Add(new AutoOneTimeValue());
                values.Add(new AutoSimpleBag());
                values.Add(new AutoScaleValue());
            }
            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                var r = value.Parse(config);
                if (r != null)
                    return r;
            }
            Logger.ErrorFormat("Auto Value for {0} not found", config);
            return null;
        }
    }
}
