using System;

namespace Blitz.Core;

public readonly struct LetterId : IEquatable<LetterId>
{
    public readonly byte Value;

    public LetterId(byte value) => Value = value;

    public bool Equals(LetterId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is LetterId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(LetterId left, LetterId right) => left.Equals(right);

    public static bool operator !=(LetterId left, LetterId right) => !left.Equals(right);
}

public readonly struct PhonemeId : IEquatable<PhonemeId>
{
    public readonly byte Value;

    public PhonemeId(byte value) => Value = value;

    public bool Equals(PhonemeId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PhonemeId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(PhonemeId left, PhonemeId right) => left.Equals(right);

    public static bool operator !=(PhonemeId left, PhonemeId right) => !left.Equals(right);
}

public readonly struct SoundObjectId : IEquatable<SoundObjectId>
{
    public readonly byte Slot;

    public SoundObjectId(byte slot)
    {
        if (slot > 2) throw new ArgumentOutOfRangeException(nameof(slot), "Table has exactly three slots (0-2).");
        Slot = slot;
    }

    public bool Equals(SoundObjectId other) => Slot == other.Slot;

    public override bool Equals(object? obj) => obj is SoundObjectId other && Equals(other);

    public override int GetHashCode() => Slot.GetHashCode();

    public static bool operator ==(SoundObjectId left, SoundObjectId right) => left.Equals(right);

    public static bool operator !=(SoundObjectId left, SoundObjectId right) => !left.Equals(right);
}
