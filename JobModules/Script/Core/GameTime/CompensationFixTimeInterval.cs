namespace Core.GameTime
{
    public class CompensationFixTimer
    {
        private float lastUpdate1;
        private float compensationInterval ;
        public int Update(float now)
        {
          
            int interval;
            if (lastUpdate1 == 0)
            {
                lastUpdate1 =now;
                interval = 1;
            }
            else
            {
                var temp = now - lastUpdate1;
                interval = (int) temp;
                compensationInterval += (temp - interval);
                while (compensationInterval > 1)
                {
                        compensationInterval -= 1;
                        interval              += 1;
                }

                lastUpdate1 = now;
            }

            return interval;
        }
    }
}