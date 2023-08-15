using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERewriter.PEReader
{
    public readonly ref struct PESectionHeader
    {
        public const int Length = 40;

        private readonly PEFileData fileData;
        private readonly ReadOnlySpan<byte> headerSpan;

        public PESectionHeader(PEFileData fileData, int offset)
        {
            this.fileData = fileData;
            Offset = offset;
            headerSpan = fileData.Slice(offset, Length);
        }

        public int Offset { get; }

        public string Name => Encoding.ASCII.GetString(headerSpan.Slice(0, 8));
        public int PhysicalAddressOrVirtualSize => headerSpan.GetIntAt(8);
        public int VirtualAddress => headerSpan.GetIntAt(12);
        public int SizeOfRawData => headerSpan.GetIntAt(16);
        public int PointerToRawData => headerSpan.GetIntAt(20);
        public int PointerToRelocations => headerSpan.GetIntAt(24);
        public int PointerToLineNumbers => headerSpan.GetIntAt(28);
        public short NumberOfRelocations => headerSpan.GetShortAt(32);
        public short NumberOfLineNumbers => headerSpan.GetShortAt(34);
        public int Characteristics => headerSpan.GetIntAt(36);
    }
}
