namespace PERewriter
{
    public ref struct IntArrayIterator
    {
        private readonly ReadOnlySpan<byte> span;

        private int index;

        public IntArrayIterator(ReadOnlySpan<byte> span)
        {
            this.span = span;
            Reset();
        }

        public int Count => span.Length / 4;

        public int Current => span.GetIntAt(index * 4);

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