namespace Utilities
{
    public static class LongUtilities
    {
        public static string FromLittleEndianByteOrderToIP(this long original)
        {
            original >>= 32;

            byte[] result = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = (byte)(original & 255);
                original >>= 8;
            }

            return string.Join(":", result);
        }
    }
}
