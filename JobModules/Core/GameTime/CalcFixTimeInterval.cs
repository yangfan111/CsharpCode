namespace Core.GameTime
{
    public class CalcFixTimeInterval
    {
        private float _lastUpdate;
        private float _compensationInterval = 0;
        public int Update(float now)
        {
          
            int interval;
            if (_lastUpdate == 0)
            {
                _lastUpdate =now;
                interval = 1;
            }
            else
            {
                var temp = now - _lastUpdate;
                interval = (int) temp;
                _compensationInterval += (temp - interval);
                if (_compensationInterval > 1)
                {
                    _compensationInterval -= 1;
                    interval += 1;
                }
                _lastUpdate = now;
            }

            return interval;
        }
    }
}