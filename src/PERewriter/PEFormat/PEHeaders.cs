using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PERewriter.Program;

namespace PERewriter.PEReader
{
    public readonly ref struct PEHeaders
    {
        private readonly PEFileData fileData;
        private readonly ReadOnlySpan<byte> headerSpan;

        public PEHeaders(PEFileData fileData, int offset, int length)
        {
            this.fileData = fileData;
            Offset = offset;
            Length = length;
            headerSpan = fileData.Slice(offset, length);
        }

        public int Offset { get; }

        public int Length { get; }

        public int Signature => headerSpan.GetIntAt(0);

        public PEFileHeader FileHeader => new PEFileHeader(fileData, Offset + 0x4);

        public PEOptionalHeader OptionalHeader => new PEOptionalHeader(fileData, Offset + 0x18);
    }
}
