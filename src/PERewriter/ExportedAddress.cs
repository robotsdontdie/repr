namespace PERewriter
{
    public record ExportedAddress(short Ordinal, string? Name, int Address)
        : ExportedFunction(Ordinal, Name);
}