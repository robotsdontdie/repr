using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PERewriter
{
    public readonly ref struct StringSpan
    {
        private readonly ReadOnlySpan<byte> span;

        public StringSpan(ReadOnlySpan<byte> span)
        {
            this.span = span;
        }

        public int Terminator => span.IndexOf((byte)0);

        public bool TryGetString([NotNullWhen(true)] out string? str)
        {
            int terminator = Terminator;
            if (terminator == -1)
            {
                str = null;
                return false;
            }

            str = Encoding.ASCII.GetString(span.Slice(0, terminator));
            return true;
        }
    }
}