using System.Data;
using Core.Fsm;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class FsmInputCommand : IFsmInputCommand
    {

        public FsmInputCommand()
        {
            Reset();
        }
        
        private FsmInput _type;
        public FsmInput Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (_type == FsmInput.None)
                    Handled = true;
                else
                    Handled = false;
            }
        }
        public float AdditioanlValue { get; set; }
        public float AlternativeAdditionalValue { get; set; }
        private bool _handled;
        public bool Handled
        {
            get { return _handled; }
            set
            {
                _handled = value;
            }
        }

        public bool IsMatch(FsmInput type)
        {
            return !_handled && _type == type;
        }

      
        
        public void Reset()
        {
            Type = FsmInput.None;
            AdditioanlValue = 0;
            AlternativeAdditionalValue = 0;
        }
    }
}