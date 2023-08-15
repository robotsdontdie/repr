using System.Text;

namespace PERewriter
{
    public class SectionHeader
    {
        public SectionHeader(string name)
        {
            Name = name;
        }

        string Name { get; set; }
        public int PhysicalAddressOrVirtualSize { get; set; }
        public int VirtualAddress { get; set; }
        public int SizeOfRawData { get; set; }
        public int PointerToRawData { get; set; }
        public int PointerToRelocations { get; set; }
        public int PointerToLineNumbers { get; set; }
        public short NumberOfRelocations { get; set; }
        public short NumberOfLineNumbers { get; set; }
        public int Characteristics { get; set; }

        public byte[] Serialize(int offset)
        {
            var bytes = new ByteArrayBuilder(offset);

            bytes.PutBytes(GetNameBytes());
            bytes.PutInt(PhysicalAddressOrVirtualSize);
            bytes.PutInt(VirtualAddress);
            bytes.PutInt(SizeOfRawData);
            bytes.PutInt(PointerToRawData);
            bytes.PutInt(PointerToRelocations);
            bytes.PutInt(PointerToLineNumbers);
            bytes.PutShort(NumberOfRelocations);
            bytes.PutShort(NumberOfLineNumbers);
            bytes.PutInt(Characteristics);

            return bytes.Build();
        }

        private byte[] GetNameBytes()
        {
            var nameBytes = Encoding.ASCII.GetBytes(Name);
            var bytes = new byte[8];
            for (int i = 0; i < Name.Length; i++)
            {
                bytes[i] = nameBytes[i];
            }

            return bytes;
        }
    }
}