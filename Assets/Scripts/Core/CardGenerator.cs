using System;
using System.Collections.Generic;

namespace Blitz.Core
{
    /// <summary>
    /// Builds single-slot cards for the three-object table with positive vs exclusion modes,
    /// using a fixed <see cref="ActiveOnomatopoeiaSet"/> for the whole match.
    /// Cards are drawn without replacement from a shuffled deck (no repeats within one generator lifetime).
    /// </summary>
    public sealed class CardGenerator
    {
        readonly Random _rng;
        readonly List<GeneratedCard> _deck = new();
        int _nextIndex;

        public CardGenerator(int seed) => _rng = new Random(seed);

        public CardGenerator(Random rng) => _rng = rng;

        /// <summary>Maximum distinct valid cards for any 3-letter table (3 PAR + 6 EXCLUSÃO).</summary>
        public const int MaxDistinctCardsPerMatch = 9;

        public int RemainingCards => Math.Max(0, _deck.Count - _nextIndex);

        /// <summary>Builds and shuffles the full valid deck for this match table.</summary>
        public void ResetDeck(ActiveOnomatopoeiaSet activeSet)
        {
            _deck.Clear();
            _nextIndex = 0;
            CollectAllValidCards(activeSet, _deck);
            Shuffle(_deck, _rng);
        }

        public bool TryGenerateCard(ActiveOnomatopoeiaSet activeSet, out GeneratedCard card)
        {
            if (_deck.Count == 0)
                ResetDeck(activeSet);

            if (_nextIndex >= _deck.Count)
            {
                card = default;
                return false;
            }

            card = _deck[_nextIndex++];
            return true;
        }

        public static void CollectAllValidCards(ActiveOnomatopoeiaSet activeSet, ICollection<GeneratedCard> into)
        {
            var letters = new[] { activeSet.Letter0, activeSet.Letter1, activeSet.Letter2 };
            var t0 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter0);
            var t1 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter1);
            var t2 = activeSet.TrueOnomatopoeiaForLetter(activeSet.Letter2);
            var trueUnits = new[] { t0, t1, t2 };

            foreach (var cardLetter in letters)
            {
                var co = activeSet.TrueOnomatopoeiaForLetter(cardLetter);
                if (CardUniqueness.PositiveHasUniqueSolution(co, activeSet))
                {
                    var figure = ResolveFigureForOnomatopoeia(activeSet, co);
                    into.Add(new GeneratedCard(cardLetter, new CardPresentationPair(figure, co), CardMode.HasTruePair));
                }

                var taught = co;
                foreach (var cue in trueUnits)
                {
                    if (cue == taught)
                        continue;

                    if (!CardUniqueness.ExclusionHasUniqueSolution(cardLetter, cue, activeSet))
                        continue;

                    var fig = ResolveFigureForOnomatopoeia(activeSet, cue);
                    into.Add(new GeneratedCard(
                        cardLetter,
                        new CardPresentationPair(fig, cue),
                        CardMode.ExclusionMismatch));
                }
            }
        }

        /// <summary>Card figure channel represents the cue onomatopoeia (same identity as the played sound).</summary>
        static ushort ResolveFigureForOnomatopoeia(ActiveOnomatopoeiaSet set, OnomatopoeiaId cue) =>
            FigureIdFor(cue, set.LetterWhoseTrueOnomatopoeiaIs(cue));

        static ushort FigureIdFor(OnomatopoeiaId unit, LetterId letter) =>
            (ushort)(100u + unit.Value + letter.Value);

        static void Shuffle(List<GeneratedCard> list, Random rng)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
