using System.IO;
using System.Text;

namespace NamedPipeConnection
{
    public class StreamString
    {
        private Stream _ioStream;
        private UnicodeEncoding _streamEncoding;

        public StreamString(Stream ioStream)
        {
            _ioStream = ioStream;
            _streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = _ioStream.ReadByte() * 256;
            len += _ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            _ioStream.Read(inBuffer, 0, len);

            return _streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = _streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > ushort.MaxValue)
            {
                len = ushort.MaxValue;
            }
            _ioStream.WriteByte((byte)(len / 256));
            _ioStream.WriteByte((byte)(len & 255));
            _ioStream.Write(outBuffer, 0, len);
            _ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
