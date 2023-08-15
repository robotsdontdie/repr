using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERewriter.PEReader
{
    public readonly ref struct PEFileHeader
    {
        private readonly PEFileData fileData;
        private readonly ReadOnlySpan<byte> headerSpan;

        public PEFileHeader(PEFileData fileData, int offset)
        {
            this.fileData = fileData;
            Offset = offset;
            headerSpan = fileData.Slice(offset, Length);
        }

        public int Offset { get; }

        public int Length => 0x14;

        public short NumberOfSections => headerSpan.GetShortAt(0x2);
    }

    public enum PEImageType
    {
        PE32,
        PE32Plus,
    }
}
