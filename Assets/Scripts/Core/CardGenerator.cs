using System;
using System.Collections.Generic;

namespace Blitz.Core
{
    /// <summary>
    /// Builds single-slot cards for the three-object table with positive vs exclusion modes,
    /// using a fixed <see cref="ActiveOnomatopoeiaSet"/> for the whole match.
    /// </summary>
    public sealed class CardGenerator
    {
        readonly Random _rng;

        public CardGenerator(int seed) => _rng = new Random(seed);

        public CardGenerator(Random rng) => _rng = rng;

        public bool TryGenerateCard(ActiveOnomatopoeiaSet activeSet, out GeneratedCard card, int maxAttempts = 256)
        {
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                if (TryGenerateCardOnce(activeSet, out card))
                    return true;
            }

            card = default;
            return false;
        }

        bool TryGenerateCardOnce(ActiveOnomatopoeiaSet activeSet, out GeneratedCard card)
        {
            var letters = new[] { activeSet.Letter0, activeSet.Letter1, activeSet.Letter2 };
            var cardLetter = letters[_rng.Next(3)];
            var usePositive = _rng.Next(2) == 0;

            var t0 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter0);
            var t1 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter1);
            var t2 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter2);

            if (usePositive)
            {
                var co = activeSet.TrueOnomatopoeiaForLetter(cardLetter);
                if (!CardUniqueness.PositiveHasUniqueSolution(co, activeSet))
                {
                    card = default;
                    return false;
                }

                var figure = ResolveFigureForLetter(activeSet, cardLetter);
                card = new GeneratedCard(cardLetter, new CardPresentationPair(figure, co), CardMode.HasTruePair);
                return true;
            }

            var taught = activeSet.TrueOnomatopoeiaForLetter(cardLetter);
            var cueCandidates = new List<OnomatopoeiaId>(2);
            if (t0 != taught) cueCandidates.Add(t0);
            if (t1 != taught) cueCandidates.Add(t1);
            if (t2 != taught) cueCandidates.Add(t2);

            var cue = cueCandidates[_rng.Next(cueCandidates.Count)];
            if (!CardUniqueness.ExclusionHasUniqueSolution(cardLetter, cue, activeSet))
            {
                card = default;
                return false;
            }

            var fig = ResolveFigureForLetter(activeSet, cardLetter);
            card = new GeneratedCard(cardLetter, new CardPresentationPair(fig, cue), CardMode.ExclusionMismatch);
            return true;
        }

        ushort ResolveFigureForLetter(ActiveOnomatopoeiaSet set, LetterId letter)
        {
            if (letter == set.Letter0) return FigureIdFor(set.TrueOnomatopoeia0, set.Letter0);
            if (letter == set.Letter1) return FigureIdFor(set.TrueOnomatopoeia1, set.Letter1);
            return FigureIdFor(set.TrueOnomatopoeia2, set.Letter2);
        }

        static ushort FigureIdFor(OnomatopoeiaId unit, LetterId letter) =>
            (ushort)(100u + unit.Value + letter.Value);
    }
}
