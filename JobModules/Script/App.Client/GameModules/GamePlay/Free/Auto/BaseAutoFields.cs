using System.Collections.Generic;
using Assets.Sources.Free.Utility;

namespace Assets.Sources.Free.Auto
{
    public abstract class BaseAutoFields
    {

        public const int AUTO_SET = 0;
        public const int AUTO_START_OLD = 1;
        public const int AUTO_START_NEW = 2;
        public const int AUTO_STOP_OLD = 3;
        public const int AUTO_STOP_NEW = 4;

        protected IList<AutoField> Autos;

        protected BaseAutoFields()
        {
            Autos = new List<AutoField>();
        }

        public static void StartAutos(IList<AutoField> autos, int index = 0)
        {
            if (index == 0)
            {
                for (var i = 0; i < autos.Count; i++)
                {
                    var af = autos[i];
                    af.auto.Start();
                }
            }
            else
            {
                if (index > 0 && index <= autos.Count)
                    autos[index - 1].auto.Start();
            }
        }

        public void StartAuto(int index = 0)
        {

            StartAutos(Autos, index);
        }

        public static void StopAutos(IList<AutoField> autos, int index = 0)
        {
            if (index == 0)
            {
                for (var i = 0; i < autos.Count; i++)
                {
                    var af = autos[i];
                    af.auto.Stop();
                }
            }
            else
            {
                if (index > 0 && index <= autos.Count)
                    autos[index - 1].auto.Stop();
            }
        }

        protected void StopAuto(int index = 0)
        {

            StopAutos(Autos, index);
        }

        protected IAutoValue GetAuto(string field)
        {
            for (int i = 0; i < Autos.Count; i++)
            {
                var af = Autos[i];
                if (af.field == field)
                    return af.auto;
            }

            return null;
        }

        // 如果返回值为空,则为设置自动化的开始停止,否则则为设置为返回值为当前的值
        public static string SetValue(IList<AutoField> autos, int autoIndex, string value)
        {
            var sa = autoIndex;
            var auto = sa / 100;
            var index = sa % 100;
            if (auto == AUTO_START_NEW || auto == AUTO_START_OLD)
                StartAutos(autos, index);
            else
                StopAutos(autos, index);

            if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW || auto == AUTO_SET)
            {
                if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW)
                    SetAutoValues(autos, value, index);
                if (auto == AUTO_SET)
                    return value;
            }

            return null;
        }

        public static void SetAutoValues(IList<AutoField> autos, object vs, int index)
        {
            if (index == 0)
            {
                for (var i = 0; i < autos.Count; i++)
                {
                    var af = autos[i];
                    af.auto.SetValue(vs);
                }
            }
            else
            {
                if (index > 0 && index <= autos.Count)
                    autos[index - 1].auto.SetValue(vs);
            }
        }

        protected void SetAutoValue(object vs, int index = 0)
        {

            SetAutoValues(Autos, vs, index);
        }

        public void InitialAuto(string config)
        {
            if (!string.IsNullOrEmpty(config))
            {
                var ss = config.Split("|||");
                for (var i = 0; i < ss.Length; i++)
                {
                    var index = ss[i].IndexOf("=");
                    if (index > 0)
                        InitialOneAuto(ss[i].Substring(0, index), ss[i].Substring(index + 1));
                }
            }
        }

        private void InitialOneAuto(string field, string config)
        {
            var auto = Auto.AutoValue.Parse(config);
            InitialAutoValue(field, auto);
            Autos.Add(new AutoField(field, auto));
        }

        protected abstract void InitialAutoValue(string field, IAutoValue auto);

        protected void AutoValue(int frameTime)
        {
            for (int i = 0; i < Autos.Count; i++)
            {
                var autoField = Autos[i];
                SetAutoValueTo(autoField, frameTime);
            }
        }

        protected abstract void SetAutoValueTo(AutoField auto, int frammeTime);
    }
}
