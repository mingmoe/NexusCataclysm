namespace NexusCataclysm.Core;

/// <summary>
/// This interface represent a class has the ability
/// to produce <see cref="ExtendedHash"/> with xxHash128 algorithm.
/// </summary>
public interface IExtendedXX128Hash
{
    ExtendedHash GetExtendedHashCode();
}
