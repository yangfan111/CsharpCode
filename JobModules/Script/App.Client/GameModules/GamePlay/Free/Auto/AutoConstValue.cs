using Assets.Sources.Free.Utility;

#pragma warning disable 169
namespace Assets.Sources.Free.Auto
{
    public class AutoConstValue :IAutoValue
    {
#pragma warning disable CS0414 // The field 'AutoConstValue._started' is assigned but its value is never used
        private bool _started;
#pragma warning restore CS0414 // The field 'AutoConstValue._started' is assigned but its value is never used
		
        private string _currentValue;
		
        public object Frame(int frameTime)
        {
            return _currentValue;
        }

        public bool Started
        {
            get
            {
                return _started;
            }
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if(ss.Length == 2 && ss[0] == "const"){
                var at = new AutoConstValue();
                at._currentValue = ss[1];
				
                return at;
            }
			
            return null;
        }
		
        public void Start()
        {
            _started = true;
        }
		
        public void Stop()
        {

            _started = false;

        }

        public string GetValue()
        {
            return _currentValue;
        }

        public void SetValue(params object[] vs)
        {

            _currentValue = vs[0] as string;

        }
    }
}
