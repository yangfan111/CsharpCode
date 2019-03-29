using System.Collections.Generic;
using App.Protobuf;
using Assets.Sources.Free.Effect;
using Assets.Sources.Free.UI;
using Free.framework;

namespace Assets.Sources.Free.UI
{
    public class ShowSimpleStyle : IShowStyle
    {


        private readonly IList<UITimeChange> _changes;

        public ShowSimpleStyle()
        {
            this._changes = new List<UITimeChange>();
        }

        public IShowStyle Parse(SimpleProto simpleProto)
        {
            if (simpleProto.Ks[1] == 1)
            {
                var st = new ShowSimpleStyle();

                for (var i = 0; i < simpleProto.Ss.Count - 1; i++)
                {
                    st._changes.Add(new UITimeChange(simpleProto.Ins[i * 2], simpleProto.Ins[i * 2 + 1], simpleProto.Fs[i * 2], simpleProto.Fs[i * 2 + 1], simpleProto.Ss[i + 1]));
                }

                return st;
            }

            return null;
        }

        public void Show(SimpleFreeUI sprite, int currentTime, int totalTime)
        {
            if (currentTime < totalTime || totalTime == 0)
            {
                sprite.Visible = true;

                for (var index = 0; index < _changes.Count; index++)
                {
                    var uc = _changes[index];
                    uc.SetValue(currentTime, totalTime, sprite);
                }
            }
            else
            {
                sprite.Visible = false;
                sprite.X = sprite.OrignalX;
                sprite.Y = sprite.OrignalY;
                sprite.Width = sprite.OrignalWidth;
                sprite.Height = sprite.OrignalHeight;
                sprite.ScaleX = 1;
                sprite.ScaleY = 1;
                sprite.Alpha = 1;
            }
        }

        public void ShowEffect(FreeRenderObject effect, int currentTime, int totalTime)
        {
            if (currentTime < totalTime || totalTime <= 0)
            {
                effect.Visible = true;
            }
            else
            {
                effect.Visible = false;
            }
        }

    }
}

class UITimeChange
{
    public int FromTime;
    public int ToTime;
    public float FromValue;
    public float ToValue;
    public string Field;

    public UITimeChange(int ft = 0, int tt = 0, float fv = 0, float tv = 0, string field = "")
    {
        this.FromTime = ft;
        this.ToTime = tt;
        this.FromValue = fv;
        this.ToValue = tv;
        this.Field = field;
    }

    public void SetValue(int currentTime, int totalTime, SimpleFreeUI ui)
    {

        if (currentTime >= FromTime && currentTime <= ToTime)
        {
            var delta = FromValue + (ToValue - FromValue) * (currentTime - FromTime) / (ToTime - FromTime);

            if (FromValue < 0)
            {
                delta = ToValue * (currentTime - FromTime) / (ToTime - FromTime);
            }

            if (Field == "x")
            {
                ui.X = ui.OrignalX + delta;
            }
            if (Field == "y")
            {
                ui.Y = ui.OrignalY + delta;
            }
            if (Field == "w")
            {
                ui.Width = (int)(ui.OrignalWidth + delta);
            }
            if (Field == "h")
            {
                ui.Height = (int)(ui.OrignalHeight + delta);
            }
            if (Field == "a")
            {
                ui.Alpha = delta;
            }
            if (Field == "s")
            {
                ui.Width = (int)(ui.OrignalWidth * delta);
                ui.Height = (int)(ui.OrignalHeight * delta);

                ui.X = ui.OrignalX + (ui.OrignalWidth * (1 - delta));
                ui.Y = ui.OrignalY + (ui.OrignalHeight * (1 - delta));
            }
        }
    }
}
