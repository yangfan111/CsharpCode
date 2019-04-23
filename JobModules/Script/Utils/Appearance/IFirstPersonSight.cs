using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Appearance
{
    public interface IFirstPersonSight
    {
        void Update(int interval);
        void SetHoldBreath(bool value);
        void SetProne(bool value);
        void SetAttachmentFactor(float value);
        float Buff { get; }
        void Clear();
    }
}
