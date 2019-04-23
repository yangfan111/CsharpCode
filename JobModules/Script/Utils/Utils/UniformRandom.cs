namespace Core.Utils
{
	public class UniformRandom
	{
		private const int NTAB = 32;

		private const int IA = 16807;

		private const int IM = 2147483647;

		private const int IQ = 127773;

		private const int IR = 2836;

		private const int NDIV = (1 + (IM - 1) / NTAB);

		private const int MAX_RANDOM_RANGE = unchecked((int)(0x7FFFFFFE));

		private const double AM = 1.0 / 2147483647;

		private const double EPS = 1.2e-7;

		private const double RNMX = (1.0 - EPS);

		private static int m_idum;

		private static int m_iy;

		private static int[] m_iv = new int[NTAB];

		public static void SetSeed(int seed)
		{
			m_idum = ((seed < 0) ? seed : -seed);
			m_iy = 0;
		}

		private static int GenerateRandomNumber()
		{
			int j;
			int k;
			if (m_idum <= 0 || m_iy == 0)
			{
				if (-(m_idum) < 1)
				{
					m_idum = 1;
				}
				else
				{
					m_idum = -(m_idum);
				}
				for (j = NTAB + 7; j >= 0; j--)
				{
					k = (m_idum) / IQ;
					m_idum = IA * (m_idum - k * IQ) - IR * k;
					if (m_idum < 0)
					{
						m_idum += IM;
					}
					if (j < NTAB)
					{
						m_iv[j] = m_idum;
					}
				}
				m_iy = m_iv[0];
			}
			k = (m_idum) / IQ;
			m_idum = IA * (m_idum - k * IQ) - IR * k;
			if (m_idum < 0)
			{
				m_idum += IM;
			}
			j = m_iy / NDIV;
			m_iy = m_iv[j];
			m_iv[j] = m_idum;
			return m_iy;
		}

		public static int RandomInt(int seed, int iLow, int iHigh)
		{
			SetSeed(seed);
			//ASSERT(lLow <= lHigh);
			int maxAcceptable;
			int x = iHigh - iLow + 1;
			int n;
			if (x <= 1 || MAX_RANDOM_RANGE < x - 1)
			{
				return iLow;
			}
			// The following maps a uniform distribution on the interval [0,MAX_RANDOM_RANGE]
			// to a smaller, client-specified range of [0,x-1] in a way that doesn't bias
			// the uniform distribution unfavorably. Even for a worst case x, the loop is
			// guaranteed to be taken no more than half the time, so for that worst case x,
			// the average number of times through the loop is 2. For cases where x is
			// much smaller than MAX_RANDOM_RANGE, the average number of times through the
			// loop is very close to 1.
			//
			maxAcceptable = (int)(MAX_RANDOM_RANGE - ((MAX_RANDOM_RANGE + 1) % x));
			do
			{
				n = GenerateRandomNumber();
			}
			while (n > maxAcceptable);
			return iLow + (n % x);
		}

		public static double RandomFloat(int seed, double flLow, double flHigh)
		{
			SetSeed(seed);
			// float in [0,1)
			double fl = AM * GenerateRandomNumber();
			if (fl > RNMX)
			{
				fl = RNMX;
			}
			return (fl * (flHigh - flLow)) + flLow;
		}
		// float in [low,high)
	}
}
