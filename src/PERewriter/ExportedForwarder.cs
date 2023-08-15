namespace PERewriter
{
    public record ExportedForwarder(short Ordinal, string? Name, string Forwarder)
        : ExportedFunction(Ordinal, Name);
}