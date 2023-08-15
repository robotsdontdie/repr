namespace PERewriter
{
    public ref struct ShortArrayIterator
    {
        private readonly ReadOnlySpan<byte> span;

        private int index;

        public ShortArrayIterator(ReadOnlySpan<byte> span)
        {
            this.span = span;
            Reset();
        }

        public int Count => span.Length / 2;

        public short Current => span.GetShortAt(index * 2);

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