using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERewriter.PEReader
{
    public readonly ref struct PEExportNames
    {
        private readonly ReadOnlySpan<byte> arraySpan;

        public PEExportNames(ReadOnlySpan<byte> arraySpan)
        {
            this.arraySpan = arraySpan;
        }

        public int Length => arraySpan.Length / 2;
    }
}
