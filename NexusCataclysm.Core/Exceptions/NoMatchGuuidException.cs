namespace NexusCataclysm.Core.Exceptions;

public sealed class NoMatchGuuidException : Exception
{
    public NoMatchGuuidException(Guuid guuid, string msg)
        : base($"No match guuid:expect {guuid} but:{msg}")
    {
        ExpectGuuid = guuid;
    }

    public Guuid ExpectGuuid { get; init; }
}
