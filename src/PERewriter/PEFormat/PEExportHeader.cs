using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PERewriter.Program;

namespace PERewriter.PEReader
{
    public readonly ref struct PEExportTable
    {
        private readonly PEFileData fileData;
        private readonly ReadOnlySpan<byte> tableSpan;

        public PEExportTable(PEFileData fileData, int offset, int size)
        {
            this.fileData = fileData;
            Offset = offset;
            Size = size;
            tableSpan = fileData.Slice(offset, size);
        }

        public int Offset { get; }
        public int Size { get; }
        public int EndOffset => Offset + Size;

        public int ExportFlags => tableSpan.GetIntAt(0);
        public int Timestamp => tableSpan.GetIntAt(4);
        public short MajorVersion => tableSpan.GetShortAt(8);
        public short MinorVersion => tableSpan.GetShortAt(10);
        public int NameRva => tableSpan.GetIntAt(12);
        public int OrdinalBase => tableSpan.GetIntAt(16);
        public int NumberOfFunctions => tableSpan.GetIntAt(20);
        public int NumberOfNames => tableSpan.GetIntAt(24);
        public int AddressOfFunctions => tableSpan.GetIntAt(28);
        public int AddressOfNames => tableSpan.GetIntAt(32);
        public int AddressOfNameOrdinals => tableSpan.GetIntAt(36);

        public ShortArrayIterator Ordinals => new ShortArrayIterator(fileData.Slice(AddressOfNameOrdinals, NumberOfNames * 2));

        public IntArrayIterator Names => new IntArrayIterator(fileData.Slice(AddressOfNames, NumberOfNames * 4));

        public IntArrayIterator Functions => new IntArrayIterator(fileData.Slice(AddressOfFunctions, NumberOfFunctions * 4));

        public StringSpan GetName(int address)
        {
            if (!IsWithinRange(address))
            {
                throw new ArgumentException($"Address {address} is outside of export table range {Offset} + {Size}.");
            }

            return fileData.GetString(address);
        }

        public bool IsWithinRange(int address)
        {
            return address >= Offset && address < EndOffset;
        }
    }
}
