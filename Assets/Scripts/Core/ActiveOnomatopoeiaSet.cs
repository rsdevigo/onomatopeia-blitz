using System;

namespace Blitz.Core
{
    /// <summary>
    /// The three onomatopoeias in play for one match: each letter maps to its taught unit T(L),
    /// and table slots 0..2 hold a permutation of those three identities.
    /// </summary>
    public readonly struct ActiveOnomatopoeiaSet
    {
        public readonly LetterId Letter0;
        public readonly LetterId Letter1;
        public readonly LetterId Letter2;

        public readonly OnomatopoeiaId TrueOnomatopoeia0;
        public readonly OnomatopoeiaId TrueOnomatopoeia1;
        public readonly OnomatopoeiaId TrueOnomatopoeia2;

        public readonly OnomatopoeiaId OnomatopoeiaOnSlot0;
        public readonly OnomatopoeiaId OnomatopoeiaOnSlot1;
        public readonly OnomatopoeiaId OnomatopoeiaOnSlot2;

        public ActiveOnomatopoeiaSet(
            LetterId letter0, OnomatopoeiaId trueOnomatopoeia0,
            LetterId letter1, OnomatopoeiaId trueOnomatopoeia1,
            LetterId letter2, OnomatopoeiaId trueOnomatopoeia2,
            OnomatopoeiaId onomatopoeiaOnSlot0, OnomatopoeiaId onomatopoeiaOnSlot1, OnomatopoeiaId onomatopoeiaOnSlot2)
        {
            Letter0 = letter0;
            Letter1 = letter1;
            Letter2 = letter2;
            TrueOnomatopoeia0 = trueOnomatopoeia0;
            TrueOnomatopoeia1 = trueOnomatopoeia1;
            TrueOnomatopoeia2 = trueOnomatopoeia2;
            OnomatopoeiaOnSlot0 = onomatopoeiaOnSlot0;
            OnomatopoeiaOnSlot1 = onomatopoeiaOnSlot1;
            OnomatopoeiaOnSlot2 = onomatopoeiaOnSlot2;
        }

        /// <summary>
        /// Dev fallback: letters 0..2, onomatopoeias 10..12, shuffled table permutation.
        /// </summary>
        public static ActiveOnomatopoeiaSet CreateSyntheticDevSet(Random rng)
        {
            var l0 = new LetterId(0);
            var l1 = new LetterId(1);
            var l2 = new LetterId(2);
            var o0 = new OnomatopoeiaId(10);
            var o1 = new OnomatopoeiaId(11);
            var o2 = new OnomatopoeiaId(12);
            var slots = new[] { o0, o1, o2 };
            Shuffle(slots, rng);
            return new ActiveOnomatopoeiaSet(
                l0, o0,
                l1, o1,
                l2, o2,
                slots[0], slots[1], slots[2]);
        }

        static void Shuffle(OnomatopoeiaId[] list, Random rng)
        {
            for (var i = list.Length - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public OnomatopoeiaId GetOnomatopoeiaOnSlot(byte slotIndex)
        {
            return slotIndex switch
            {
                0 => OnomatopoeiaOnSlot0,
                1 => OnomatopoeiaOnSlot1,
                2 => OnomatopoeiaOnSlot2,
                _ => throw new ArgumentOutOfRangeException(nameof(slotIndex))
            };
        }

        public OnomatopoeiaId TrueOnomatopoeiaForLetter(LetterId letter)
        {
            if (letter == Letter0) return TrueOnomatopoeia0;
            if (letter == Letter1) return TrueOnomatopoeia1;
            if (letter == Letter2) return TrueOnomatopoeia2;
            throw new ArgumentException("Letter is not part of this active set.", nameof(letter));
        }

        /// <summary>
        /// L(o): the unique letter in this match whose taught onomatopoeia equals <paramref name="onomatopoeiaId"/>.
        /// </summary>
        public LetterId LetterWhoseTrueOnomatopoeiaIs(OnomatopoeiaId onomatopoeiaId)
        {
            if (onomatopoeiaId == TrueOnomatopoeia0) return Letter0;
            if (onomatopoeiaId == TrueOnomatopoeia1) return Letter1;
            if (onomatopoeiaId == TrueOnomatopoeia2) return Letter2;
            throw new ArgumentException("Onomatopoeia is not one of the three taught units in play.", nameof(onomatopoeiaId));
        }
    }
}
