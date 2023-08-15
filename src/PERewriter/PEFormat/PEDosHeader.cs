using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERewriter.PEReader
{
    public ref struct PEDosHeader
    {
        private readonly ReadOnlySpan<byte> span;

        public PEDosHeader(ReadOnlySpan<byte> span)
        {
            this.span = span;
        }

        public int NewHeaderFileAddress => span.GetIntAt(0x3c);
    }
}
