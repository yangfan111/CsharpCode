namespace YF.Utils
{
    public partial class CommonUtil
    {
        public class ShortIDGenerator
        {
            private const uint s_prime32       = 16777619;
            private const uint s_offsetBasis32 = 2166136261;

            private static byte s_hashSize;
            private static uint s_mask;

            static ShortIDGenerator()
            {
                HashSize = 32;
            }

            public static byte HashSize
            {
                get { return s_hashSize; }

                set
                {
                    s_hashSize = value;
                    s_mask     = (uint) ((1 << s_hashSize) - 1);
                }
            }

            //string->uint 计算器
            public static uint Compute(string in_name)
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(in_name.ToLower());

                // Start with the basis value
                var hval = s_offsetBasis32;

                for (var i = 0; i < buffer.Length; i++)
                {
                    // multiply by the 32 bit FNV magic prime mod 2^32
                    hval *= s_prime32;

                    // xor the bottom with the current octet
                    hval ^= buffer[i];
                }

                if (s_hashSize == 32)
                    return hval;

                // XOR-Fold to the required number of bits
                return (hval >> s_hashSize) ^ (hval & s_mask);
            }
        }
    }
}