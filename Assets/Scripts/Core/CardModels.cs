namespace Blitz.Core
{
    public enum CardMode : byte
    {
        HasTruePair = 0,
        ExclusionMismatch = 1
    }

    public readonly struct CardPresentationPair
    {
        public readonly ushort FigureVisualId;
        public readonly OnomatopoeiaId CueOnomatopoeiaId;

        public CardPresentationPair(ushort figureVisualId, OnomatopoeiaId cueOnomatopoeiaId)
        {
            FigureVisualId = figureVisualId;
            CueOnomatopoeiaId = cueOnomatopoeiaId;
        }
    }

    public readonly struct GeneratedCard
    {
        public readonly LetterId CardLetterId;
        public readonly CardPresentationPair Pair;
        public readonly CardMode Mode;

        public OnomatopoeiaId CueOnomatopoeiaId => Pair.CueOnomatopoeiaId;

        public GeneratedCard(LetterId cardLetterId, CardPresentationPair pair, CardMode mode)
        {
            CardLetterId = cardLetterId;
            Pair = pair;
            Mode = mode;
        }
    }
}
