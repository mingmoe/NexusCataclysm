using Cysharp.Text;
using System.Diagnostics.CodeAnalysis;

namespace NexusCataclysm.Core;

[System.Runtime.CompilerServices.InlineArray(16)]
public struct BufferOf128Bits
{
    private byte element;
}

public struct ExtendedHash : IEquatable<ExtendedHash>
{
    public BufferOf128Bits Buffer;

    public ExtendedHash() { }

    public ExtendedHash(byte[] hash)
    {
        if (hash.Length != 16)
        {
            throw new ArgumentException("Hash must be exactly 16 bytes long.", nameof(hash));
        }

        hash.CopyTo(this.Buffer[..]);
    }

    public static bool operator ==(ExtendedHash left, ExtendedHash right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ExtendedHash left, ExtendedHash right)
    {
        return !(left == right);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is ExtendedHash other)
        {
            return ((IEquatable<ExtendedHash>)this).Equals(other);
        }

        return false;
    }

    public readonly bool Equals(ExtendedHash other)
    {
        return this.Buffer[..].SequenceEqual(other.Buffer[..]);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(Buffer[0], Buffer[1], Buffer[2], Buffer[3],
                             Buffer[4], Buffer[5], Buffer[6], Buffer[7]),
            HashCode.Combine(Buffer[8], Buffer[9], Buffer[10], Buffer[11],
                             Buffer[12], Buffer[13], Buffer[14], Buffer[15]));
    }

    public override readonly string ToString()
    {
        using var builder = ZString.CreateStringBuilder(true);

        foreach (var b in Buffer[..])
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
