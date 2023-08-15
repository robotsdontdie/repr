using PERewriter.PEReader;
using System.Runtime.InteropServices;

namespace PERewriter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"C:\temp\kernel32.dll");
            var data = new PEFileData(bytes);
            var optional = data.Headers.OptionalHeader;
            var names = optional.ExportTable.Names;
            while (names.MoveNext())
            {
                int nameAddress = names.Current;
                var str = data.GetString(nameAddress);
                str.TryGetString(out string? name);
                //Console.WriteLine(name);
            }

            var sectionHeaderArray = data.SectionHeaders;
            PESectionHeader lastSectionHeader = default;
            while (sectionHeaderArray.MoveNext())
            {
                lastSectionHeader = sectionHeaderArray.Current;
                Console.WriteLine($"Name : {lastSectionHeader.Name}.");
                Console.WriteLine($"Virtual Size : {lastSectionHeader.PhysicalAddressOrVirtualSize}.");
                Console.WriteLine($"Virtual Address : {lastSectionHeader.VirtualAddress}.");
                Console.WriteLine($"Size of Raw Data : {lastSectionHeader.SizeOfRawData}.");
                Console.WriteLine($"Pointer to Raw Data : {lastSectionHeader.PointerToRawData}.");
                Console.WriteLine($"Pointer to Relocations : {lastSectionHeader.PointerToRelocations}.");
                Console.WriteLine($"Pointer to Line Numbers : {lastSectionHeader.PointerToLineNumbers}.");
                Console.WriteLine($"Number of Relocations : {lastSectionHeader.NumberOfRelocations}.");
                Console.WriteLine($"Number of Line Numbers : {lastSectionHeader.NumberOfLineNumbers}.");
                Console.WriteLine($"Characteristics : {lastSectionHeader.Characteristics}.");
                Console.WriteLine();
                Console.WriteLine();
            }

            int newExportsAddress = lastSectionHeader.PointerToRawData + lastSectionHeader.SizeOfRawData;
            int newExportsVirtualAddress = lastSectionHeader.VirtualAddress + lastSectionHeader.PhysicalAddressOrVirtualSize.RoundUpTo(0x1000);

            var exports = new ExportTable(optional.ExportTable);
            SwapCreateFile(exports);
            var newExports = exports.Serialize(newExportsVirtualAddress);
            var newExportDiskLength = newExports.Length.RoundUpTo(0x1000);
            int newExportsVirtualLength = newExports.Length;

            int sectionHeadersStart = data.SectionHeaderStart;
            short numberOfSections = data.NumberOfSections;
            short newNumberOfSections = (short)(numberOfSections + 1);
            MemoryMarshal.Write(bytes.AsSpan().Slice(data.Headers.FileHeader.Offset + 0x2, 2), ref newNumberOfSections);

            var newSectionHeader = new SectionHeader(".edata")
            {
                PhysicalAddressOrVirtualSize = newExportsVirtualLength,
                VirtualAddress = newExportsVirtualAddress,
                SizeOfRawData = newExportDiskLength,
                PointerToRawData = newExportsAddress,
                PointerToRelocations = 0,
                PointerToLineNumbers = 0,
                NumberOfRelocations = 0,
                NumberOfLineNumbers = 0,
                Characteristics = 0x40000040,
            };

            var newSectionHeaderAddress = sectionHeadersStart + numberOfSections * PESectionHeader.Length;
            var newSectionHeaderBytes = newSectionHeader.Serialize(newSectionHeaderAddress);
            newSectionHeaderBytes.AsSpan().CopyTo(bytes.AsSpan().Slice(newSectionHeaderAddress, PESectionHeader.Length));

            // foreach name to replace
            // find it in the export table
            // if it's local, rename it and emit remap
            // if it's a forwarder, then update forwarder to my dll and emit remap

            var dataDirectoryOffset = optional.Offset + optional.DataDirectoryOffset;


            MemoryMarshal.Write(bytes.AsSpan().Slice(dataDirectoryOffset, 4), ref newExportsVirtualAddress);
            MemoryMarshal.Write(bytes.AsSpan().Slice(dataDirectoryOffset + 4, 4), ref newExportsVirtualLength);

            int sizeOfImage = optional.SizeOfImage;
            int newSizeOfImage = sizeOfImage + newExportDiskLength;
            int sizeOfImageOffset = optional.Offset + 56;
            MemoryMarshal.Write(bytes.AsSpan().Slice(sizeOfImageOffset, 4), ref newSizeOfImage);

            var newBytes = new byte[bytes.Length + newExportDiskLength];
            var part1 = bytes.AsSpan().Slice(0, newExportsAddress);
            var part2 = newExports.AsSpan();
            var part3 = bytes.AsSpan().Slice(newExportsAddress);

            part1.CopyTo(newBytes.AsSpan(0, newExportsAddress));
            part2.CopyTo(newBytes.AsSpan(newExportsAddress, newExportDiskLength));
            int newOverlayAddress = newExportsAddress + newExportDiskLength;
            part3.CopyTo(newBytes.AsSpan(newOverlayAddress));

            File.WriteAllBytes(@"C:\temp\kernel33.dll", newBytes);
        }

        private static void SwapCreateFile(ExportTable exportTable)
        {
            int createFileIndex = exportTable.ExportedFunctions.FindIndex(ef => ef.Name == "CreateFileW");
            var originalCreateFile = exportTable.ExportedFunctions[createFileIndex];
            var createFileForwarder = new ExportedForwarder(originalCreateFile.Ordinal, originalCreateFile.Name, "REPR.CreateFileW");
            var createFileCallback = new ExportedAddress((short)exportTable.ExportedFunctions.Count, "OldCreateFileW", ((ExportedAddress)originalCreateFile).Address);
            exportTable.ExportedFunctions[createFileIndex] = createFileForwarder;
            exportTable.ExportedFunctions.Add(createFileCallback);
        }
    }
}