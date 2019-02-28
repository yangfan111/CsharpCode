using System;
using Assets.Sources.Free.Auto;
using Assets.Sources.Utils;
using Core.Utils;

namespace Assets.Sources.Free.Scene
{
    public class FreeFog : BaseAutoFields
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FreeFog));
        private int fogId;

        public FreeFog(int id)
        {
            this.fogId = id;
        }

        public int getFogId()
        {
            return fogId;
        }

        public void frame(int frameTime)
        {
            base.AutoValue(frameTime);
        }

        protected override void InitialAutoValue(string field, IAutoValue auto)
        {

        }

        protected override void SetAutoValueTo(AutoField auto, int frammeTime)
        {
            
        }
    }
}
