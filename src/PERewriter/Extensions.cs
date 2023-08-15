using System.Runtime.InteropServices;

namespace PERewriter
{
    public static class Extensions
    {
        public static int GetIntAt(this ReadOnlySpan<byte> span, int offset)
        {
            return MemoryMarshal.Read<int>(span.Slice(offset, 4));
        }

        public static short GetShortAt(this ReadOnlySpan<byte> span, int offset)
        {
            return MemoryMarshal.Read<short>(span.Slice(offset, 2));
        }

        public static int RoundDownTo(this int value, int roundingTarget)
        {
            return value / roundingTarget * roundingTarget;
        }

        public static int RoundUpTo(this int value, int roundingTarget)
        {
            int remainder = value % roundingTarget;
            if (remainder == 0)
            {
                return value;
            }

            return value.RoundDownTo(roundingTarget) + roundingTarget;
        }
    }
}