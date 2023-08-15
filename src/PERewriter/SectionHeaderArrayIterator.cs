using PERewriter.PEReader;

namespace PERewriter
{
    public ref struct SectionHeaderArrayIterator
    {
        private readonly PEFileData fileData;
        private readonly int offset;

        private int index;

        public SectionHeaderArrayIterator(PEFileData fileData, int offset, int count)
        {
            this.fileData = fileData;
            this.offset = offset;
            Count = count;
            Reset();
        }

        public int Count { get; }

        public PESectionHeader Current => new PESectionHeader(fileData, offset + index * PESectionHeader.Length);

        public bool MoveNext()
        {
            ++index;
            return index < Count;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}