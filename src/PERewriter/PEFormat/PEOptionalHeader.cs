using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PERewriter.Program;

namespace PERewriter.PEReader
{
    public readonly ref struct PEOptionalHeader
    {
        public const int DataDirectoryEntryLength = 8;

        private readonly ReadOnlySpan<byte> headerSpan;
        private readonly PEFileData fileData;

        public PEOptionalHeader(PEFileData fileData, int offset)
        {
            this.fileData = fileData;
            // first just slice 4 bytes to read the magic number to determine length
            headerSpan = fileData.Slice(offset, 4);
            headerSpan = fileData.Slice(offset, Length);
            Offset = offset;
        }

        public int Offset { get; }

        public int Length => DataDirectoryOffset + DataDirectoryEntryLength * 16;

        public int Magic => headerSpan.GetShortAt(0);

        public int SizeOfImage => headerSpan.GetIntAt(56);

        public PEImageType ImageType => Magic switch
        {
            0x10b => PEImageType.PE32,
            0x20b => PEImageType.PE32Plus,
            _ => throw new ArgumentOutOfRangeException($"Unrecognized magic number {Magic}"),
        };

        public int DataDirectoryOffset => ImageType switch
        {
            PEImageType.PE32 => 96,
            PEImageType.PE32Plus => 112,
            _ => throw new ArgumentOutOfRangeException($"Unrecognized image type {ImageType}"),
        };

        public PEExportTable ExportTable
        {
            get
            {
                var entry = GetTableEntry(0);
                return new PEExportTable(fileData, entry.Address, entry.Length);
            }
        }

        public TableEntry GetTableEntry(int tableIndex)
        {
            int offset = DataDirectoryOffset + (tableIndex * DataDirectoryEntryLength);
            int tableAddress = headerSpan.GetIntAt(offset);
            int tableLength = headerSpan.GetIntAt(offset + 4);
            return new TableEntry(tableAddress, tableLength);
        }

        public readonly record struct TableEntry(int Address, int Length);
    }
}
