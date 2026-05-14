namespace Blitz.Core;

public enum CardMode : byte
{
    HasTruePair = 0,
    ExclusionMismatch = 1
}

public readonly struct CardPresentationPair
{
    public readonly ushort FigureVisualId;
    public readonly PhonemeId CuePhonemeId;

    public CardPresentationPair(ushort figureVisualId, PhonemeId cuePhonemeId)
    {
        FigureVisualId = figureVisualId;
        CuePhonemeId = cuePhonemeId;
    }
}

public readonly struct GeneratedCard
{
    public readonly LetterId CardLetterId;
    public readonly CardPresentationPair Pair;
    public readonly CardMode Mode;

    public PhonemeId CuePhonemeId => Pair.CuePhonemeId;

    public GeneratedCard(LetterId cardLetterId, CardPresentationPair pair, CardMode mode)
    {
        CardLetterId = cardLetterId;
        Pair = pair;
        Mode = mode;
    }
}
