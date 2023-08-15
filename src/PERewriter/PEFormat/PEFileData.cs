using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PERewriter.Program;

namespace PERewriter.PEReader
{
    /// <summary>
    /// http://www.openrce.org/reference_library/files/reference/PE%20Format.pdf
    /// https://upload.wikimedia.org/wikipedia/commons/1/1b/Portable_Executable_32_bit_Structure_in_SVG_fixed.svg
    /// </summary>
    public ref struct PEFileData
    {
        private readonly ReadOnlySpan<byte> fileSpan;

        public PEFileData(ReadOnlySpan<byte> span)
        {
            fileSpan = span;
        }

        public PEDosHeader DosHeader => new PEDosHeader(fileSpan);

        public PEHeaders Headers => new PEHeaders(this, DosHeader.NewHeaderFileAddress, 0xF8);

        public SectionHeaderArrayIterator SectionHeaders => new SectionHeaderArrayIterator(this, SectionHeaderStart, NumberOfSections);

        public int SectionHeaderStart
        {
            get
            {
                var optionalHeader = Headers.OptionalHeader;
                return optionalHeader.Offset + optionalHeader.Length;
            }
        }

        public short NumberOfSections => Headers.FileHeader.NumberOfSections;

        public StringSpan GetString(int offset) => new StringSpan(fileSpan.Slice(offset));

        public StringSpan GetString(int offset, int maxLength) => new StringSpan(fileSpan.Slice(offset, maxLength));

        public ReadOnlySpan<byte> Slice(int offset, int length) => fileSpan.Slice(offset, length);
    }
}
