using System.Text;

namespace PERewriter
{
    public class ByteArrayBuilder
    {
        List<byte> builderBytes;
        private readonly int builderOffset;

        public ByteArrayBuilder(int offset)
        {
            builderBytes = new List<byte>();
            this.builderOffset = offset;
        }

        public ByteArrayBuilder()
            : this(0)
        {
        }

        public int NextAddressWithOffset => builderBytes.Count + builderOffset;

        public int PutInt(int value)
        {
            int ret = NextAddressWithOffset;
            builderBytes.AddRange(BitConverter.GetBytes(value));
            return ret;
        }

        public void UpdateInt(int offset, int value)
        {
            if (offset + 3 >= builderBytes.Count)
            {
                throw new IndexOutOfRangeException($"Cannot update bytes at {offset}, buffer is length {builderBytes.Count}.");
            }

            var valueBytes = BitConverter.GetBytes(value);
            builderBytes[offset] = valueBytes[0];
            builderBytes[offset + 1] = valueBytes[1];
            builderBytes[offset + 2] = valueBytes[2];
            builderBytes[offset + 3] = valueBytes[3];
        }

        public int PutShort(short value)
        {
            int ret = NextAddressWithOffset;
            builderBytes.AddRange(BitConverter.GetBytes(value));
            return ret;
        }

        public int PutString(string value)
        {
            int ret = NextAddressWithOffset;
            builderBytes.AddRange(Encoding.ASCII.GetBytes(value));
            builderBytes.Add(0);
            return ret;
        }

        public int PutBytes(ByteArrayBuilder builder)
        {
            int ret = NextAddressWithOffset;
            builderBytes.AddRange(builder.Build());
            return ret;
        }

        public int PutBytes(IEnumerable<byte> bytes)
        {
            int ret = NextAddressWithOffset;
            builderBytes.AddRange(bytes);
            return ret;
        }

        public byte[] Build()
        {
            return builderBytes.ToArray();
        }
    }
}