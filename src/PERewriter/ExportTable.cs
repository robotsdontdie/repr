using PERewriter.PEReader;

namespace PERewriter
{
    public class ExportTable
    {
        public ExportTable()
        {
            ExportedFunctions = new List<ExportedFunction>();
            Name = "";
        }

        public ExportTable(PEExportTable tableData)
            : this()
        {
            if (tableData.ExportFlags != ExportFlags)
            {
                throw new ArgumentException($"Export flags must be 0, found {ExportFlags}.");
            }

            Timestamp = tableData.Timestamp;
            MajorVersion = tableData.MajorVersion;
            MinorVersion = tableData.MinorVersion;
            OrdinalBase = tableData.OrdinalBase;

            if (!tableData.GetName(tableData.NameRva).TryGetString(out string? libraryName))
            {
                throw new ArgumentException($"Could not find library name at address {tableData.NameRva}.");
            }
            Name = libraryName;

            var ordinalToName = new Dictionary<short, string>();
            var ordinals = tableData.Ordinals;
            var names = tableData.Names;
            while (ordinals.MoveNext() && names.MoveNext())
            {
                var maybeName = tableData.GetName(names.Current);
                short ordinal = ordinals.Current;
                if (!maybeName.TryGetString(out string? name))
                {
                    throw new ArgumentException($"No valid string at address {names.Current}.");
                }

                if (ordinalToName.ContainsKey(ordinal))
                {
                    throw new ArgumentException($"Duplicate function ordinal: {ordinal}.");
                }

                ordinalToName[ordinal] = name;
            }

            var functions = tableData.Functions;
            short currentOrdinal = 0;
            while (functions.MoveNext())
            {
                int functionAddress = functions.Current;
                string? name = ordinalToName.TryGetValue(currentOrdinal, out string? foundName)
                    ? foundName
                    : null;

                ExportedFunction exportedFunction;
                if (tableData.IsWithinRange(functionAddress))
                {
                    if (!tableData.GetName(functionAddress).TryGetString(out string? forwarder))
                    {
                        throw new ArgumentException($"Forwarder address has invalid string {forwarder}");
                    }

                    exportedFunction = new ExportedForwarder(currentOrdinal, name, forwarder);
                }
                else
                {
                    exportedFunction = new ExportedAddress(currentOrdinal, name, functionAddress);
                }

                ExportedFunctions.Add(exportedFunction);
                ++currentOrdinal;
            }
        }

        public int ExportFlags => 0;
        public int Timestamp { get; set; }
        public short MajorVersion { get; set; }
        public short MinorVersion { get; set; }
        public string Name { get; set; }
        public int OrdinalBase { get; set; }
        public List<ExportedFunction> ExportedFunctions { get; set; }

        public byte[] Serialize(int offset)
        {
            var builder = new ByteArrayBuilder(offset);
            builder.PutInt(0);
            builder.PutInt(Timestamp);
            builder.PutShort(MajorVersion);
            builder.PutShort(MinorVersion);
            builder.PutInt(0); // name
            builder.PutInt(OrdinalBase);
            builder.PutInt(0); // number of functions
            builder.PutInt(0); // number of names
            builder.PutInt(0); // address of functions
            builder.PutInt(0); // address of names
            builder.PutInt(0); // address of name ordinals

            int nameAddress = builder.PutString(Name);
            builder.UpdateInt(0xc, nameAddress);

            var functions = new ByteArrayBuilder();
            var names = new ByteArrayBuilder();
            var ordinals = new ByteArrayBuilder();
            int numFunctions = 0;
            int numNames = 0;
            foreach (var exportedFunction in ExportedFunctions)
            {
                if (!string.IsNullOrWhiteSpace(exportedFunction.Name))
                {
                    int functionNameAddress = builder.PutString(exportedFunction.Name);
                    names.PutInt(functionNameAddress);
                    ordinals.PutShort(exportedFunction.Ordinal);
                    ++numNames;
                }

                if (exportedFunction is ExportedForwarder exportedForwarder)
                {
                    int forwarderAddress = builder.PutString(exportedForwarder.Forwarder);
                    functions.PutInt(forwarderAddress);
                }
                else if (exportedFunction is ExportedAddress exportedAddress)
                {
                    functions.PutInt(exportedAddress.Address);
                }
                else
                {
                    throw new InvalidOperationException($"Unrecognized export type {exportedFunction.GetType()}.");
                }
                ++numFunctions;
            }

            builder.UpdateInt(0x14, numFunctions);
            builder.UpdateInt(0x18, numNames);

            int functionsAddress = builder.PutBytes(functions);
            builder.UpdateInt(0x1c, functionsAddress);

            int namesAddress = builder.PutBytes(names);
            builder.UpdateInt(0x20, namesAddress);

            int ordinalsAddress = builder.PutBytes(ordinals);
            builder.UpdateInt(0x24, ordinalsAddress);

            return builder.Build();
        }
    }
}