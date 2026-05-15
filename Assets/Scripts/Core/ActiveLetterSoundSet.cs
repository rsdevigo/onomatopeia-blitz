using System;

namespace Blitz.Core
{
    /// <summary>
    /// Three letters in play, each with its taught phoneme T(L), and the permutation of phonemes on table slots 0..2.
    /// </summary>
    public readonly struct ActiveLetterSoundSet
    {
        public readonly LetterId Letter0;
        public readonly LetterId Letter1;
        public readonly LetterId Letter2;

        public readonly PhonemeId TruePhoneme0;
        public readonly PhonemeId TruePhoneme1;
        public readonly PhonemeId TruePhoneme2;

        public readonly PhonemeId PhonemeOnSlot0;
        public readonly PhonemeId PhonemeOnSlot1;
        public readonly PhonemeId PhonemeOnSlot2;

        public ActiveLetterSoundSet(
            LetterId letter0, PhonemeId truePhoneme0,
            LetterId letter1, PhonemeId truePhoneme1,
            LetterId letter2, PhonemeId truePhoneme2,
            PhonemeId phonemeOnSlot0, PhonemeId phonemeOnSlot1, PhonemeId phonemeOnSlot2)
        {
            Letter0 = letter0;
            Letter1 = letter1;
            Letter2 = letter2;
            TruePhoneme0 = truePhoneme0;
            TruePhoneme1 = truePhoneme1;
            TruePhoneme2 = truePhoneme2;
            PhonemeOnSlot0 = phonemeOnSlot0;
            PhonemeOnSlot1 = phonemeOnSlot1;
            PhonemeOnSlot2 = phonemeOnSlot2;
        }

        public PhonemeId GetPhonemeOnSlot(byte slotIndex)
        {
            return slotIndex switch
            {
                0 => PhonemeOnSlot0,
                1 => PhonemeOnSlot1,
                2 => PhonemeOnSlot2,
                _ => throw new ArgumentOutOfRangeException(nameof(slotIndex))
            };
        }

        public PhonemeId TruePhonemeForLetter(LetterId letter)
        {
            if (letter == Letter0) return TruePhoneme0;
            if (letter == Letter1) return TruePhoneme1;
            if (letter == Letter2) return TruePhoneme2;
            throw new ArgumentException("Letter is not part of this active set.", nameof(letter));
        }

        /// <summary>
        /// L(p): the unique letter in this match whose taught sound equals <paramref name="phoneme"/>.
        /// </summary>
        public LetterId LetterWhoseTrueSoundIs(PhonemeId phoneme)
        {
            if (phoneme == TruePhoneme0) return Letter0;
            if (phoneme == TruePhoneme1) return Letter1;
            if (phoneme == TruePhoneme2) return Letter2;
            throw new ArgumentException("Phoneme is not one of the three taught sounds in play.", nameof(phoneme));
        }
    }
}
