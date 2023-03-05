namespace Utilities
{
    public static class IntUtilities
    {
        public static string FromLittleEndianByteOrderToIP(this int original)
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = (byte)(original & 255);
                original >>= 8;
            }

            return string.Join(".", result);
        }
    }
}
