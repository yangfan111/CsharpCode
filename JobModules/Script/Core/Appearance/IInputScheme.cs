using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Appearance
{
    public static class InputSchemeConst
    {
        public static readonly int Default = 1;
        public static readonly int Dive = 2;

        public static readonly string ActionHorizontal = "Horizontal";
        public static readonly string ActionVertical = "Vertical";
        public static readonly string ActionUpDown = "UpDown";
    }

    public interface IInputScheme
    {
        void SetInputSchemeActionField(int schemeIndex, string actionName);
        List<KeyValuePair<int, string>> GetInputSchemeActionFieldToUpdate();
    }
}
